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

using System;
using System.Linq;
using SearchAThing.Core;
using static System.Math;
using System.Collections.Generic;
using SearchAThing.Sci;
using MIConvexHull;
using System.Globalization;
using System.Collections.ObjectModel;

namespace SearchAThing
{

    public class Mesh2D
    {

        double tol;
        VoronoiMesh<Vertex, Cell, VoronoiEdge<Vertex, Cell>> _voronoiMesh = null;
        HashSet<Vector3D> _convexHull = null;
        List<Vector3D> _convexHullPoly = null;
        Dictionary<Vector3D, List<Cell>> _vectorToCell = null;
        Dictionary<Vector3D, List<Vector3D>> _vectorToPoly = null;
        List<Line3D> _closures = null;

        public IEnumerable<Line3D> Closures { get { return _closures; } }
        public IEnumerable<Vector3D> Points { get; private set; }
        public List<Vector3D> Boundary { get; private set; }

        public IEnumerable<Vector3D> ConvexHull
        {
            get
            {
                if (_convexHullPoly == null)
                    _convexHullPoly = _convexHull.ToList().SortPoly(tol).ToList();

                return _convexHullPoly;
            }
        }

        public IEnumerable<Vector3D> VectorToPoly(Vector3D pt)
        {
            List<Vector3D> lst = null;
            if (_vectorToPoly.TryGetValue(pt, out lst))
                return lst;
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

        public Mesh2D(double _tol, IEnumerable<Vector3D> _pts, IEnumerable<Vector3D> _boundaryPts)
        {
            tol = _tol;
            Points = _pts;
            Boundary = _boundaryPts.OpenPolyPoints(tol).ToList();
            var boundarySegs = Boundary.PolygonSegments(tol);
            _closures = new List<Line3D>();

            var config = new TriangulationComputationConfig
            {
                PointTranslationType = PointTranslationType.TranslateInternal,
                PlaneDistanceTolerance = 1e-6,
                PointTranslationGenerator = TriangulationComputationConfig.RandomShiftByRadius(1e-6, 0)
            };

            var vs = Points.Select(v => new Vertex(v)).ToList();

            _voronoiMesh = VoronoiMesh.Create<Vertex, Cell>(vs, config);

            var vCmp = new Vector3DEqualityComparer(tol);

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
                _vectorToPoly = new Dictionary<Vector3D, List<Vector3D>>();
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
                            if (polySegs.Any(ps => boundarySegs.Any(bs => bs.Intersect(tol, ps, true, true) != null)))
                            {
                                var resPoly = orderedPts.Boolean(tol, Boundary, ClipperLib.ClipType.ctIntersection);
                                if (resPoly.Count() > 0)
                                    _vectorToPoly.Add(p, resPoly.First().ToList());
                            }
                            else
                                _vectorToPoly.Add(p, orderedPts);
                        }
                    }

                }
            }
            #endregion

            #region cache already polygonalized segments
            var hsSegMid = new HashSet<Vector3D>(vCmp);
            {
                foreach (var s in _vectorToPoly)
                {
                    foreach (var seg in s.Value.PolygonSegments(tol))
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
                // adjancies are opposite the vertex                

                foreach (var cell in _voronoiMesh.Vertices)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        if (cell.Adjacency[i] == null)
                        {
                            var from = cell.Vertices.Select(w => w.V).CircleBy3Points().Center;

                            if (!Boundary.ContainsPoint(tol, from)) continue;

                            var vExtSeg = cell.Vertices.Where((_, j) => j != i).ToArray();
                            var extSeg = new Line3D(vExtSeg[0].V, vExtSeg[1].V);

                            var perpV = extSeg.PerpendicularToIntersection(tol, from);

                            var closureLine = new Line3D(perpV.From, boundarySegs.Intersect(tol, perpV, Line3DSegmentMode.From).First());
                            _closures.Add(closureLine);                            
                        }
                    }
                }

                foreach (var edge in _voronoiMesh.Edges)
                {
                    var from = edge.Source.Vertices.Select(w => w.V).CircleBy3Points().Center;
                    var to = edge.Target.Vertices.Select(w => w.V).CircleBy3Points().Center;
                    if (from.EqualsTol(tol, to)) continue;

                    if (hsSegMid.Contains((from + to) / 2)) continue;
                    if (!Boundary.ContainsPoint(tol, from) || !Boundary.ContainsPoint(tol, to)) continue;

                    _closures.Add(new Line3D(from, to));
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
