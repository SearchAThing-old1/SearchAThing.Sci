#region SearchAThing.Sci, Copyright(C) 2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System.Linq;
using System.Collections.Generic;
using SearchAThing.Sci;
using MIConvexHull;
using System.Globalization;

namespace SearchAThing
{

    public class Mesh2DPoly
    {
        public List<Vector3D> Poly { get; private set; }
        public Vector3D Point { get; private set; }
        public bool PointIsConvexHull { get; private set; }

        public Mesh2DPoly(Vector3D _Point, bool _PointIsConvexHull, List<Vector3D> _Poly)
        {
            Point = _Point;
            PointIsConvexHull = _PointIsConvexHull;
            Poly = _Poly;
        }
    }

    public class Mesh2D
    {

        double tol;
        VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>> _voronoiMesh = null;
        HashSet<Vector3D> _convexHull = null;
        List<Vector3D> _convexHullPoly = null;
        Dictionary<Vector3D, List<Cell>> _vectorToCell = null;
        Dictionary<Vector3D, Mesh2DPoly> _pointToPoly = null;
        List<Line3D> _closures = null;
        HashSet<Vector3D> _boundarySplitPts = null;
        Line3DEqualityComparer lCmp = null;
        Vector3DEqualityComparer vCmp = null;

        public IEnumerable<Vector3D> Points { get; private set; }
        public List<Vector3D> Boundary { get; private set; }
        public IEnumerable<Line3D> Closures { get { return _closures; } }
        public IEnumerable<Vector3D> BoundarySplitPts { get { return _boundarySplitPts; } }

        public IEnumerable<Vector3D> ConvexHull
        {
            get
            {
                if (_convexHullPoly == null)
                    _convexHullPoly = _convexHull.ToList().SortPoly(tol).ToList();

                return _convexHullPoly;
            }
        }

        /// <summary>
        /// mesh polygon that contains given input mesh point
        /// note : the point must be one of those in constructor input
        /// </summary>        
        public Mesh2DPoly PointToPoly(Vector3D pt)
        {
            Mesh2DPoly mply = null;
            if (_pointToPoly.TryGetValue(pt, out mply))
                return mply;
            else
                return null;
        }

        public IEnumerable<IEnumerable<Vector3D>> PointToTriangles(Vector3D pt)
        {
            var cells = _vectorToCell[pt];

            foreach (var c in cells)
                yield return c.Vertices.Select(r => r.V);
        }

        public IEnumerable<IEnumerable<Vector3D>> Triangles
        {
            get
            {
                foreach (var cell in _voronoiMesh.Vertices)
                    yield return cell.Vertices.Select(r => r.V);
            }
        }

        public IEnumerable<Line3D> AllSegs
        {
            get
            {
                return Boundary.Union(BoundarySplitPts).ToList().SortPoly(tol).PolygonSegments(tol)
                    .Union(_pointToPoly.SelectMany(k => k.Value.Poly.PolygonSegments(tol)))
                    .Union(_closures)
                    .Distinct(lCmp)
                    .ToList();
            }
        }

        /// <summary>
        /// Build mesh2-d
        /// </summary>
        /// <param name="_tol">lenght tolerance</param>
        /// <param name="_pts">point inside polygon mesh</param>
        /// <param name="_boundaryPts">external boundary polygon</param>
        /// <param name="failedPoints">list of failed points</param>
        /// <param name="boundaryPolyIntersectToleranceFactor">tolerance factor when detect additional split points on the boundary</param>
        /// <param name="boundaryPolyBooleanMapToleranceFactor">tolerance factor when map to integer to use boolean poly functions.
        /// reducer factor should be used to mantain precision.</param>
        /// <param name="closedPolyToleranceFactor">tolerance factor when detect a polygon is it</param>
        public Mesh2D(double _tol, IEnumerable<Vector3D> _pts, IEnumerable<Vector3D> _boundaryPts,
            List<Vector3D> failedPoints = null,
            double boundaryPolyIntersectToleranceFactor = 10.0,
            double boundaryPolyBooleanMapToleranceFactor = 1e-1,
            double closedPolyToleranceFactor = 1.0,
            bool disableBoundary = false)
        {
            tol = _tol;
            vCmp = new Vector3DEqualityComparer(tol * 2);
            lCmp = new Line3DEqualityComparer(tol * 2);
            Points = _pts;
            Boundary = _boundaryPts.PolyPoints(tol).ToList();
            var boundarySegs = Boundary.PolygonSegments(tol);
            _closures = new List<Line3D>();
            _boundarySplitPts = new HashSet<Vector3D>(vCmp);
            var extPolys = new List<List<Vector3D>>();
            var boundaryMean = Boundary.Mean();
            if (failedPoints != null) failedPoints.Clear();
            _pointToPoly = new Dictionary<Vector3D, Mesh2DPoly>(vCmp);

            var config = new TriangulationComputationConfig
            {
                PointTranslationType = PointTranslationType.TranslateInternal,
                PlaneDistanceTolerance = 1e-6,
                PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(1e-6, 0)
            };

            var vs = Points.Select(v => new Vertex(v)).ToList();

            _voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vs, config);

            _vectorToCell = new Dictionary<Vector3D, List<Cell>>(vCmp);

            #region build vector -> cells dict
            {
                List<Cell> cells = null;

                foreach (var cell in _voronoiMesh.Vertices)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        if (!_vectorToCell.TryGetValue(cell.Vertices[i].V, out cells))
                        {
                            cells = new List<Cell>();
                            _vectorToCell.Add(cell.Vertices[i].V, cells);
                        }
                        cells.Add(cell);
                    }
                }
            }
            #endregion

            #region scan convex hull
            {
                _convexHull = new HashSet<Vector3D>();

                foreach (var cell in _voronoiMesh.Vertices)
                {
                    if (cell.Adjacency.Any(a => a == null))
                    {
                        var q1 = cell.Adjacency
                            .Where(a => a != null)
                            .SelectMany(c => c.Vertices.Select(h => h.V));

                        var q2 = q1
                            .GroupBy(g => g);

                        var qInternalVertex = q2
                            .Where(g => g.Count() == 2);

                        if (qInternalVertex.Count() > 0)
                        {
                            var internalVertex = qInternalVertex.First().Key;

                            var external = cell.Vertices
                                .Where(v => !v.V.EqualsTol(tol, internalVertex))
                                .ToList();

                            foreach (var j in external) _convexHull.Add(j.V);
                        }
                        else
                        {
                            var external = cell.Vertices
                                .Where(v => !q2.Any(f => f.First().EqualsTol(tol, v.V)))
                                .ToList();

                            foreach (var j in external) _convexHull.Add(j.V);
                        }

                        continue; // exclude borderline cells
                    }
                }
            }
            #endregion            

            #region build polygons            
            {
                var polyVertexProcessed = new HashSet<Vector3D>(vCmp);

                foreach (var cell in _voronoiMesh.Vertices)
                {
                    foreach (var v in cell.Vertices.Select(f => f.V))
                    {
                        if (polyVertexProcessed.Contains(v)) continue;
                        polyVertexProcessed.Add(v);                        

                        var cells = _vectorToCell[v];
                        if (cells.Count < 3) continue;

                        var trPts = cells
                            .Select(r => r.Vertices.Select(t => t.V).CircleBy3Points().Center)
                            .Distinct(vCmp)
                            .ToList();

                        var orderedPts = trPts
                            .ToList()
                            .SortPoly(tol).ToList();

                        // do if there any of the adjacency without external vertex                            
                        if (cells.Any(c => c.Vertices.All(vc => !_convexHull.Contains(vc.V))))
                        {
                            var p = cells.SelectMany(c => c.Vertices.Select(vv => vv.V))
                                .GroupBy(vv => vv, vCmp)
                                .First(r => r.Count() == cells.Count)
                                .Key;

                            var polySegs = orderedPts.PolygonSegments(tol);
                            if (!disableBoundary && polySegs.Any(ps => boundarySegs.Any(bs => bs.Intersect(tol, ps, true, true) != null)))
                            {
                                var resPoly = orderedPts.Boolean(tol * boundaryPolyBooleanMapToleranceFactor, Boundary, ClipperLib.ClipType.ctIntersection);
                                if (resPoly.Count() > 0)
                                {
                                    var extPoly = resPoly.First().ToList();
                                    _pointToPoly.Add(p, new Mesh2DPoly(p, false, extPoly));

                                    // register poly intersect bound points
                                    foreach (var ep in extPoly)
                                    {
                                        if (boundarySegs.Any(f => f.SegmentContainsPoint(tol * boundaryPolyIntersectToleranceFactor, ep)))
                                            _boundarySplitPts.Add(ep);
                                    }

                                    // register poly as ext poly
                                    extPolys.Add(extPoly);
                                }
                            }
                            else
                                _pointToPoly.Add(p, new Mesh2DPoly(p, false, orderedPts));
                        }
                    }

                }
            }
            #endregion

            #region cache already polygonalized segments
            var hsSegMid = new HashSet<Vector3D>(vCmp);
            if (!disableBoundary)
            {
                foreach (var s in _pointToPoly)
                {
                    foreach (var seg in s.Value.Poly.PolygonSegments(tol))
                    {
                        hsSegMid.Add(seg.MidPoint);
                    }
                }
            }
            #endregion

            #region boundary edges            
            {
                // given a triangle
                //
                //            a[2]
                //    V[0] +--------+ V[1]
                //         |       /
                //         |      /
                //         |     /
                //    a[1] |    / a[0]
                //         |   /
                //         |  /
                //         | /
                //    V[2] |/
                //
                // vertices 2d-3d are ordered CCW
                // adjacencie index are opposite the vertex                

                foreach (var cell in _voronoiMesh.Vertices)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        if (cell.Adjacency[i] == null)
                        {
                            var from = cell.Vertices.Select(w => w.V).CircleBy3Points().Center;

                            //                            if (!Boundary.ContainsPoint(tol, from)) continue;

                            var vExtSeg = cell.Vertices.Where((_, j) => j != i).ToArray();
                            var extSeg = new Line3D(vExtSeg[0].V, vExtSeg[1].V);

                            var perpV = extSeg.Perpendicular(tol, from);

                            var perpVTest = perpV.From + perpV.Dir * (2 * tol);
                            var perpVTestInv = perpV.From - perpV.Dir * (2 * tol);
                            if (perpVTest.Distance(boundaryMean) < perpVTestInv.Distance(boundaryMean)) perpV *= -1;

                            if (!disableBoundary)
                            {
                                var qi = boundarySegs.Intersect(tol, perpV, Line3DSegmentMode.From);
                                if (qi.Count() > 0)
                                {
                                    var closureLine = new Line3D(perpV.From, qi.First());
                                    _closures.Add(closureLine);

                                    // add boundary split points
                                    foreach (var ep in closureLine.Points
                                        .Where(r => boundarySegs.Any(f => f.SegmentContainsPoint(tol * boundaryPolyIntersectToleranceFactor, r))))
                                        _boundarySplitPts.Add(ep);
                                }
                                //                                else
                                //                                  _closures.Add(perpV);
                            }
                            else
                                _closures.Add(perpV);
                        }
                    }
                }

                foreach (var edge in _voronoiMesh.Edges)
                {
                    var from = edge.Source.Vertices.Select(w => w.V).CircleBy3Points().Center;
                    var to = edge.Target.Vertices.Select(w => w.V).CircleBy3Points().Center;
                    if (from.EqualsTol(tol, to)) continue;

                    if (hsSegMid.Contains((from + to) / 2)) continue;
                    //                    if (!Boundary.ContainsPoint(tol, from) || !Boundary.ContainsPoint(tol, to)) continue;

                    var closureLine = new Line3D(from, to);

                    var intersect = boundarySegs.Intersect(tol, closureLine, Line3DSegmentMode.FromTo);
                    if (intersect.Count() > 0)
                    {
                        var ip = intersect.First();
                        _boundarySplitPts.Add(ip);
                        if (Boundary.ContainsPoint(tol, closureLine.From))
                            closureLine = new Line3D(closureLine.From, ip);
                        else
                            closureLine = new Line3D(closureLine.To, ip);
                    }

                    _closures.Add(closureLine);
                }
            }
            #endregion

            #region boundary polys            
            {
                var allSegsCount = AllSegs.Count();

                var ds = new DiscreteSpace<Line3D>(tol, AllSegs, (s) => s.Points, 2);

                var dsSearchRadius = AllSegs.Select(w => w.Length).Mean() * 0.5;

                var missingPolyPoints = vs.Where(r => !_pointToPoly.ContainsKey(r.V)).Select(v => v.V).ToList();

                foreach (var mp in missingPolyPoints)
                {
                    var sr = dsSearchRadius;

                    while (true)
                    {
                        var innerOrdered = ds.GetItemsAt(mp, sr)
                            .OrderBy(l => l.MidPoint.Distance(mp)).ToList();

                        if (innerOrdered.Count == allSegsCount)
                        {
                            // should not happens but if any lets report to this list
                            if (failedPoints != null) failedPoints.Add(mp);
                            break;
                        }

                        var outerOrdered = new List<Line3D>(innerOrdered);
                        outerOrdered.Reverse();

                        // place check outer vs inner to statistically reduce nr. of checks
                        var excluded = new List<Line3D>();
                        foreach (var outer in outerOrdered)
                        {
                            foreach (var inner in innerOrdered)
                            {
                                if (outer == inner) continue;

                                if (new Line3D(outer.MidPoint, mp).Intersect(tol, inner, true, true) != null)
                                {
                                    excluded.Add(outer);
                                    break;
                                }
                            }
                        }

                        var candidate = outerOrdered.Except(excluded);

                        var candidateCnt = candidate.Count();

                        if (candidateCnt > 0)
                        {
                            var closedPoly = candidate.IsAClosedPoly(tol * closedPolyToleranceFactor);

                            if (closedPoly != null)
                            {
                                _pointToPoly.Add(mp, new Mesh2DPoly(mp, true, closedPoly.Select(w => w.From).ToList()));
                                break;
                            }
                        }

                        sr *= 2;
                    }
                }

            }
            #endregion
        }

        public class Vertex : IVertex
        {
            public Vector3D V { get; private set; }

            public Vertex(Vector3D v)
            {
                V = v;
            }

            double[] _position;
            public double[] Position
            {
                get
                {
                    if (_position == null) _position = new double[] { V.X, V.Y };

                    return _position;
                }
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "({0},{1})", V.X, V.Y);
            }
        }

        public class Cell : TriangulationCell<Vertex, Cell>
        {

            public override string ToString()
            {
                return string.Format($"cvs={Vertices[0].ToString()} {Vertices[1].ToString()} {Vertices[2].ToString()}");
            }

        }

    }

}
