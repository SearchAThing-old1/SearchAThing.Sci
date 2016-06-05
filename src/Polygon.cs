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
using ClipperLib;

namespace SearchAThing
{

    public static partial class Extentions
    {

        /// <summary>
        /// Area of a polygon (does not consider z)
        /// https://en.wikipedia.org/wiki/Centroid        
        /// </summary>        
        public static double Area(this IList<Vector3D> pts, double tol)
        {
            var lastEqualsFirst = pts[pts.Count - 1].EqualsTol(tol, pts[0]);
            double a = 0;

            for (int i = 0; i < pts.Count - 1; ++i)
                a += pts[i].X * pts[i + 1].Y - pts[i + 1].X * pts[i].Y;

            if (!lastEqualsFirst)
                a += pts[pts.Count - 1].X * pts[0].Y - pts[0].X * pts[pts.Count - 1].Y;

            return a / 2;
        }

        /// <summary>
        /// Centroid of a polygon (does not consider z)
        /// points must ordered
        /// ( if have area specify the parameter to avoid recomputation )
        /// https://en.wikipedia.org/wiki/Centroid        
        /// </summary>        
        public static Vector3D Centroid(this IList<Vector3D> pts, double tol)
        {
            var area = pts.Area(tol);
            return pts.Centroid(tol, area);
        }

        /// <summary>
        /// Centroid of a polygon (does not consider z)
        /// points must ordered
        /// https://en.wikipedia.org/wiki/Centroid        
        /// </summary>        
        public static Vector3D Centroid(this IList<Vector3D> pts, double tol, double area)
        {
            var lastEqualsFirst = pts[pts.Count - 1].EqualsTol(tol, pts[0]);
            double x = 0;
            double y = 0;

            for (int i = 0; i < pts.Count - 1; ++i)
            {
                x += (pts[i].X + pts[i + 1].X) * (pts[i].X * pts[i + 1].Y - pts[i + 1].X * pts[i].Y);
                y += (pts[i].Y + pts[i + 1].Y) * (pts[i].X * pts[i + 1].Y - pts[i + 1].X * pts[i].Y);
            }

            if (!lastEqualsFirst)
            {
                x += (pts[pts.Count - 1].X + pts[0].X) * (pts[pts.Count - 1].X * pts[0].Y - pts[0].X * pts[pts.Count - 1].Y);
                y += (pts[pts.Count - 1].Y + pts[0].Y) * (pts[pts.Count - 1].X * pts[0].Y - pts[0].X * pts[pts.Count - 1].Y);
            }

            return new Vector3D(x / (6 * area), y / (6 * area), 0);
        }

        /// <summary>
        /// given a set of polygon pts, returns the enumeation of all pts except the last if equals the first
        /// </summary>        
        public static IEnumerable<Vector3D> OpenPolyPoints(this IEnumerable<Vector3D> pts, double tol)
        {
            Vector3D first = null;

            foreach (var p in pts)
            {
                if (first == null)
                    first = p;
                else
                    if (first.EqualsTol(tol, p)) yield break;

                yield return p;
            }
        }

        /// <summary>
        /// yields an ienumerable of polygon segments corresponding to the given polygon pts ( z is not considered )
        /// works even last point not equals the first one
        /// </summary>       
        public static IEnumerable<Line3D> PolygonSegments(this IEnumerable<Vector3D> pts, double tol)
        {
            Vector3D first = null;
            Vector3D prev = null;

            foreach (var p in pts)
            {
                if (first == null)
                {
                    first = prev = p;
                    continue;
                }

                var seg = new Line3D(prev, p);
                prev = p;

                yield return seg;
            }

            if (!prev.EqualsTol(tol, first)) yield return new Line3D(prev, first);
        }

        /// <summary>        
        /// states if the given polygon contains the test point ( z not considered )
        /// https://en.wikipedia.org/wiki/Point_in_polygon
        /// By default check the point contained in the polygon perimeter.
        /// </summary>        
        /// <param name="excludePerimeter">Exclude check point contained in the perimeter</param>
        public static bool ContainsPoint(this IList<Vector3D> pts, double tol, Vector3D pt, bool excludePerimeter = false)
        {
            var ray = new Line3D(pt, Vector3D.XAxis, Line3DConstructMode.PointAndVector);

            var segs = pts.PolygonSegments(tol);

            var intCnt = 0;

            foreach (var seg in segs)
            {
                if (seg.SegmentContainsPoint(tol, pt))
                {
                    if (excludePerimeter) return false;
                    return true;
                }

                Vector3D ip = null;
                var segMinY = Min(seg.From.Y, seg.To.Y);
                var segMaxY = Max(seg.From.Y, seg.To.Y);
                if (pt.Y.GreatThanOrEqualsTol(tol, segMinY) && pt.Y.LessThanOrEqualsTol(tol, segMaxY))
                    ip = ray.Intersect(tol, seg);
                if (ip != null && pt.X.GreatThanOrEqualsTol(tol, ip.X) && seg.SegmentContainsPoint(tol, ip)) ++intCnt;
            }

            return intCnt % 2 != 0;
        }

        public static IEnumerable<Vector3D> SortPoly(this IList<Vector3D> pts, double tol)
        {
            var c = pts.Mean();

            // search non-null ref axis
            Vector3D N = null;
            var r = pts.First() - c;
            foreach (var r2 in pts.Skip(1))
            {
                N = r.CrossProduct(r2 - c);
                if (!N.Length.EqualsTol(tol, 0)) break;
            }

            var q = pts.Select(p => new
            {
                pt = p,
                ang = r.AngleToward(tol, p - c, N)
            });
            var res = q.OrderBy(w => w.ang).Select(w => w.pt);

            return res;
        }

        /// <summary>
        /// Find intersection points (0,1,2) of the given line with the given polygon
        /// TODO unit test
        /// </summary>        
        public static IEnumerable<Vector3D> Intersect(this IEnumerable<Line3D> polygonSegments,
            double tol, Line3D line, Line3DSegmentMode segmentMode)
        {
            var lineDir = line.To - line.From;
            foreach (var s in polygonSegments)
            {
                var i = s.Intersect(tol, line, true, false);
                if (i == null) continue;
                switch (segmentMode)
                {
                    case Line3DSegmentMode.None: yield return i; break;
                    case Line3DSegmentMode.From: if ((i - line.From).Concordant(tol, lineDir)) yield return i; break;
                    case Line3DSegmentMode.To: if (!(i - line.To).Concordant(tol, lineDir)) yield return i; break;
                    case Line3DSegmentMode.FromTo: if (line.SegmentContainsPoint(tol, i)) yield return i; break;
                }
            }

            yield break;
        }

        /// <summary>
        /// build 2d dxf polyline
        /// </summary>        
        public static netDxf.Entities.LwPolyline ToLwPolyline(this IEnumerable<Vector3D> pts, double tol)
        {
            return new netDxf.Entities.LwPolyline(pts.OpenPolyPoints(tol).Select(r => r.ToVector2()).ToList(), true);
        }

        /// <summary>
        /// build 2d dxf polyline
        /// </summary>        
        public static netDxf.Entities.LwPolyline ToLwPolyline(this IEnumerable<Line3D> segs, double tol)
        {
            var X = segs.Select(w => w.From).OpenPolyPoints(tol);
            return new netDxf.Entities.LwPolyline(X.Select(r => r.ToVector2()).ToList(), true);
        }

        /// <summary>
        /// build 3d dxf polyline
        /// </summary>        
        public static netDxf.Entities.Polyline ToPolyline(this IEnumerable<Vector3D> pts, double tol)
        {
            return new netDxf.Entities.Polyline(pts.OpenPolyPoints(tol).Select(r => r.ToVector3()).ToList(), true);
        }

        /// <summary>
        /// build 3d dxf polyline
        /// </summary>        
        public static netDxf.Entities.Polyline ToPolyline(this IEnumerable<Line3D> segs, double tol)
        {
            var X = segs.Select(w => w.From).OpenPolyPoints(tol);
            return new netDxf.Entities.Polyline(X.Select(r => r.ToVector3()).ToList(), true);
        }
        /// <summary>
        /// can generate a Int64MapExceptionRange exception if double values can't fit into a In64 representation.
        /// In that case try with tolerances not too small.
        /// </summary>        
        public static IEnumerable<IEnumerable<Vector3D>> Boolean(this IEnumerable<Vector3D> polyA, double tol, IEnumerable<Vector3D> polyB, ClipType type)
        {
            var intmap = new Int64Map(tol, polyA.SelectMany(x => x.Coordinates).Union(polyB.SelectMany(x => x.Coordinates)));

            var clipper = new Clipper();
            {
                var path = polyA.Select(p => new IntPoint(intmap.ToInt64(p.X), intmap.ToInt64(p.Y))).ToList();
                clipper.AddPath(path, PolyType.ptSubject, true);
            }
            {
                var path = polyB.Select(p => new IntPoint(intmap.ToInt64(p.X), intmap.ToInt64(p.Y))).ToList();
                clipper.AddPath(path, PolyType.ptClip, true);
            }

            var sol = new List<List<IntPoint>>();
            clipper.Execute(type, sol);

            return sol.Select(s => s.Select(si => new Vector3D(intmap.FromInt64(si.X), intmap.FromInt64(si.Y), 0)));
        }

    }

}
