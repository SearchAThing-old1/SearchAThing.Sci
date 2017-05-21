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
using netDxf.Entities;

namespace SearchAThing
{

    namespace Sci
    {

        public class Arc3D : Geometry
        {
            public CoordinateSystem3D CS { get; private set; }
            public double Radius { get; private set; }

            public Arc3D(CoordinateSystem3D cs, double r, double angleRadStart, double angleRadEnd) :
                base(GeometryType.Arc3D)
            {
                AngleStartRad = angleRadStart;
                AngleEndRad = angleRadEnd;
                CS = cs;
                Radius = r;
            }

            /// <summary>
            /// Build 3d circle that intersect p1,p2,p3
            /// ( the inside CS will centered in the circle center and Xaxis toward p1 )
            /// </summary>        
            public Arc3D(Vector3D p1, Vector3D p2, Vector3D p3, double angleStart, double angleEnd) :
                base(GeometryType.Arc3D)
            {
                Type = GeometryType.Arc3D;
                AngleStartRad = angleStart;
                AngleEndRad = angleEnd;

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

            public double AngleStartRad { get; private set; }
            public double AngleEndRad { get; private set; }
            public double AngleRad
            {
                get
                {
                    return AngleEndRad - AngleStartRad;
                }
            }

            public override Vector3D GeomFrom => From;
            public override Vector3D GeomTo => To;

            public Vector3D PtAtAngle(double angleRad)
            {
                return (CS.BaseX * Radius).RotateAboutZAxis(angleRad).ToWCS(CS);
            }

            public Vector3D MidPoint
            {
                get
                {
                    return PtAtAngle(AngleStartRad + AngleRad / 2);
                }
            }

            /// <summary>
            /// assuming pt is a point on the arc
            /// return the angle of the point ( rad )
            /// </summary>            
            public double PtAngle(double tolLen, Vector3D pt)
            {
                var v_x = CS.BaseX;
                var v_pt = pt - CS.Origin;

                return v_x.AngleToward(tolLen, v_pt, CS.BaseZ);
            }

            public Vector3D From { get { return PtAtAngle(AngleStartRad); } }
            public Vector3D To { get { return PtAtAngle(AngleEndRad); } }
            /// <summary>
            /// return From,To segment
            /// </summary>
            public Line3D Segment { get { return new Line3D(From, To); } }

            public override IEnumerable<Vector3D> Vertexes
            {
                get
                {
                    yield return From;
                    yield return To;
                }
            }

            /// <summary>
            /// Checks if two arcs are equals ( it checks agains swapped from-to too )
            /// </summary>        
            public bool EqualsTol(double tolLen, double tolRad, Arc3D other)
            {
                if (!Center.EqualsTol(tolLen, other.Center)) return false;
                if (!Radius.EqualsTol(tolLen, other.Radius)) return false;
                if (Segment.EqualsTol(tolLen, other.Segment)) return false;
                return true;
            }

            /// <summary>
            /// http://www.lee-mac.com/bulgeconversion.html
            /// </summary>            
            public double Bulge(double tolLen, Vector3D from, Vector3D to)
            {
                var factor = 1.0;
                if (from.CrossProduct(to).Z < 0) // TODO unit test 3d
                    factor = -1.0;

                return factor * AngleRad / 4;
            }

            /// <summary>
            /// check if this circle contains given point
            /// </summary>            
            public bool Contains(double tol, Vector3D pt, bool onlyAtCircumnfere = false)
            {
                var onplane = pt.ToUCS(CS).Z.EqualsTol(tol, 0);
                var center_dst = pt.Distance(CS.Origin);

                if (onlyAtCircumnfere)
                    return onplane && center_dst.EqualsTol(tol, Radius);
                else
                    return onplane && center_dst.LessThanOrEqualsTol(tol, Radius);
            }

            /// <summary>
            /// verify if given point is in this arc between its start-to arc angles
            /// </summary>            
            public bool Contains(double tolLen, double tolRad, Vector3D pt)
            {
                // if not in circle stop
                if (!this.Contains(tolLen, pt)) return false;

                var pt_angle = PtAngle(tolLen, pt);

                return
                    pt_angle.GreatThanOrEqualsTol(tolRad, AngleStartRad)
                    &&
                    pt_angle.LessThanOrEqualsTol(tolRad, AngleEndRad);
            }

            /// <summary>
            /// if validate_pts false it assume all given split points are valid point on the arc
            /// </summary>            
            public IEnumerable<Arc3D> Split(double tolLen, double tolRad, IEnumerable<Vector3D> _splitPts, bool validate_pts = false)
            {
                if (_splitPts == null || _splitPts.Count() == 0) yield break;

                IEnumerable<Vector3D> splitPts = _splitPts;

                if (validate_pts) splitPts = _splitPts.Where(pt => Contains(tolLen, tolRad, pt)).ToList();

                var radCmp = new DoubleEqualityComparer(tolRad);

                var hs_angles_rad = new HashSet<double>(radCmp) { AngleStartRad };
                foreach (var splitPt in splitPts.Select(pt => PtAngle(tolLen, pt)))
                    hs_angles_rad.Add(splitPt.NormalizeAngle2PI());
                hs_angles_rad.Add(AngleEndRad.NormalizeAngle2PI());

                var angles_rad = hs_angles_rad.OrderBy(w => w).ToList();

                if (angles_rad[1] < angles_rad[0])
                    throw new Exception($"split at angle_rad [{angles_rad[1]}] must great than start angle_rad [{angles_rad[0]}]");

                if (angles_rad[angles_rad.Count - 1] < angles_rad[angles_rad.Count - 2])
                    throw new Exception($"split at angle_rad [{angles_rad[angles_rad.Count - 2]}] must smallers than end angle_rad [{angles_rad[angles_rad.Count]}]");

                for (int i = 0; i < angles_rad.Count - 1; ++i)
                {
                    yield return new Arc3D(CS, Radius, angles_rad[i], angles_rad[i + 1]);
                }
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

            public Vector3D Center { get { return CS.Origin; } }

            /// <summary>
            /// centre of mass of circular segment
            /// </summary>            
            public Vector3D CentreOfMass(out double A)
            {
                // https://en.wikipedia.org/wiki/List_of_centroids

                var alpha = AngleRad / 2;
                A = Pow(Radius, 2) / 2 * (2 * alpha - Sin(2 * alpha));

                var x = (4 * Radius * Pow(Sin(alpha), 3)) / (3 * (2 * alpha - Sin(2 * alpha)));

                return (MidPoint - Center).Normalized() * x;
            }

            public override EntityObject DxfEntity
            {
                get
                {
                    var arc = new Arc(Center, Radius, AngleStartRad.ToDeg(), AngleEndRad.ToDeg());
                    arc.Normal = CS.BaseZ;
                    return arc;
                }
            }

            public IEnumerable<Vector3D> IntersectArc(double tol, Line3D l, bool segment_mode = false, bool arc_mode = true)
            {
                var q = Intersect(tol, l, segment_mode);
                if (q == null) return null;

                q = q.Where(r => this.Contains(tol, r, onlyAtCircumnfere: true)).ToList();
                if (q.Count() == 0) return null;

                return q;
            }

            public override string ToString()
            {
                return $"C:{Center} r:{Round(Radius, 3)} from:{Round(AngleStartRad.ToDeg(), 1)} to:{Round(AngleEndRad.ToDeg(), 1)}";
            }

        }

    }

    public static partial class Extensions
    {

        public static Arc3D ToArc3D(this netDxf.Entities.Arc dxf_arc)
        {
            return new Arc3D(new CoordinateSystem3D(dxf_arc.Center, dxf_arc.Normal, CoordinateSystem3DAutoEnum.AAA), dxf_arc.Radius,
                dxf_arc.StartAngle.ToRad(), dxf_arc.EndAngle.ToRad());
        }

    }

    public class Arc3DEqualityComparer : IEqualityComparer<Arc3D>
    {
        double tolLen;
        double tolRad;
        double tolHc;

        public Arc3DEqualityComparer(double _tolLen, double _tolRad)
        {
            tolLen = _tolLen;
            tolRad = _tolRad;
            tolHc = 10 * tolLen; // to avoid rounding
        }

        public bool Equals(Arc3D x, Arc3D y)
        {
            return x.EqualsTol(tolLen, tolRad, y);
        }

        public int GetHashCode(Arc3D obj)
        {
            return (int)(Round((obj.Center.X + obj.Center.Y + obj.Center.Z) / tolHc));
        }

    }

}
