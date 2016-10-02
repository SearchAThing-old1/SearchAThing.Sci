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
using static System.Math;
using System.Collections.Generic;
using SearchAThing.Sci;

namespace SearchAThing
{

    namespace Sci
    {

        public class Circle3D
        {

            public CoordinateSystem3D CS { get; private set; }
            public double Radius { get; private set; }

            public Circle3D(CoordinateSystem3D cs, double r)
            {
                CS = cs;
                Radius = r;
            }

            /// <summary>
            /// Build 3d circle that intersect p1,p2,p3
            /// ( the inside CS will centered in the circle center and Xaxis toward p1 )
            /// </summary>        
            public Circle3D(Vector3D p1, Vector3D p2, Vector3D p3)
            {
                // https://en.wikipedia.org/wiki/Circumscribed_circle
                // Cartesian coordinates from cross- and dot-products

                var d = ((p1 - p2).CrossProduct(p2 - p3)).Length;

                Radius = ((p1 - p2).Length * (p2 - p3).Length * (p3 - p1).Length) / (2 * d);

                var alpha = Pow((p2 - p3).Length, 2) * (p1 - p2).DotProduct(p1 - p3) / (2 * Pow(d, 2));
                var beta = Pow((p1 - p3).Length, 2) * (p2 - p1).DotProduct(p2 - p3) / (2 * Pow(d, 2));
                var gamma = Pow((p1 - p2).Length, 2) * (p3 - p1).DotProduct(p3 - p2) / (2 * Pow(d, 2));

                var c = alpha * p1 + beta * p2 + gamma * p3;

                CS = new CoordinateSystem3D(c, p1 - c, p2 - c);
            }

            /// <summary>
            /// build 3d circle that tangent to lines t1,t2 and that intersects point p
            /// note: point p must contained in one of t1,t2
            /// circle will be inside region t1.V toward t2.V
            /// </summary>            
            public Circle3D(double tol_len, Line3D t1, Line3D t2, Vector3D p)
            {
                var ip = t1.Intersect(tol_len, t2);
                var angle = t1.V.AngleRad(tol_len, t2.V);
                var t3 = new Line3D(ip, t1.V.RotateAs(tol_len, t1.V, t2.V, .5), Line3DConstructMode.PointAndVector);

                Line3D lp = null;
                if (t1.LineContainsPoint(tol_len, p)) lp = t1;
                else if (t2.LineContainsPoint(tol_len, p)) lp = t2;
                else throw new Exception($"circle 2 tan 1 point : pt must contained in one of given tan");

                var lpp = new Line3D(p, lp.V.RotateAboutAxis(t1.V.CrossProduct(t2.V), PI / 2), Line3DConstructMode.PointAndVector);
                var c = lpp.Intersect(tol_len, t3);

                Radius = p.Distance(c);
                CS = new CoordinateSystem3D(c, lpp.V, t2.Perpendicular(tol_len, c).V);
            }

            public double Area { get { return PI * Radius * Radius; } }
            public double Length { get { return 2 * PI * Radius; } }

            public Vector3D Center { get { return CS.Origin; } }

            public bool Contains(double tol, Vector3D pt)
            {
                return pt.ToUCS(CS).Z.EqualsTol(tol, 0) && pt.Distance(CS.Origin).LessThanOrEqualsTol(tol, Radius);
            }

            /// <summary>
            /// intersect this 3d circle with given 3d line
            /// </summary>            
            public IEnumerable<Vector3D> Intersect(double tol, Line3D l, bool segment_mode = false)
            {
                var lprj = new Line3D(l.From.ToUCS(CS).Set(OrdIdx.Z, 0), l.To.ToUCS(CS).Set(OrdIdx.Z, 0));

                var a = Pow(lprj.To.X - lprj.From.X, 2) + Pow(lprj.To.Y - lprj.From.Y, 2);
                var b = 2 * lprj.From.X * (lprj.To.X - lprj.From.X) + 2 * lprj.From.Y * (lprj.To.Y - lprj.From.Y);
                var c = Pow(lprj.From.X, 2) + Pow(lprj.From.Y, 2) - Pow(Radius, 2);
                var d = Pow(b, 2) - 4 * a * c;

                if (d.LessThanTol(tol, 0)) yield break; // no intersection at all

                var sd = Sqrt(Abs(d));
                var f1 = (-b + sd) / (2 * a);
                var f2 = (-b - sd) / (2 * a);

                // one intersection point is
                var ip = new Vector3D(
                    lprj.From.X + (lprj.To.X - lprj.From.X) * f1,
                    lprj.From.Y + (lprj.To.Y - lprj.From.Y) * f1,
                    0);

                Vector3D ip2 = null;

                if (!f1.EqualsTol(Constants.NormalizedLengthTolerance, f2))
                {
                    // second intersection point is
                    ip2 = new Vector3D(
                        lprj.From.X + (lprj.To.X - lprj.From.X) * f2,
                        lprj.From.Y + (lprj.To.Y - lprj.From.Y) * f2,
                        0);
                }

                // back to wcs, check line contains point
                var wcs_ip = ip.ToWCS(CS);
                Vector3D wcs_ip2 = null;
                if (ip2 != null) wcs_ip2 = ip2.ToWCS(CS);

                if (l.LineContainsPoint(tol, wcs_ip, segment_mode))
                    yield return wcs_ip;

                if (ip2 != null && l.LineContainsPoint(tol, wcs_ip2, segment_mode))
                    yield return wcs_ip2;
            }
        }
    }

    public static partial class Extensions
    {

        public static Circle3D CircleBy3Points(this IEnumerable<Vector3D> _pts)
        {
            var pts = _pts.ToArray();
            if (pts.Length != 3) throw new Exception("expected 3 points for circle3d");

            return new Circle3D(pts[0], pts[1], pts[2]);
        }

    }

}
