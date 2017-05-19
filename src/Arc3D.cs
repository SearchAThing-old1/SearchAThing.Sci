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

        public class Arc3D : Circle3D
        {

            public Arc3D(CoordinateSystem3D cs, double r, double angleRadStart, double angleRadEnd) :
                base(cs, r)
            {
                Type = GeometryType.Arc3D;
                AngleStartRad = angleRadStart;
                AngleEndRad = angleRadEnd;
            }

            /// <summary>
            /// Build 3d circle that intersect p1,p2,p3
            /// ( the inside CS will centered in the circle center and Xaxis toward p1 )
            /// </summary>        
            public Arc3D(Vector3D p1, Vector3D p2, Vector3D p3, double angleStart, double angleEnd) :
                base(p1, p2, p3)
            {
                Type = GeometryType.Arc3D;
                AngleStartRad = angleStart;
                AngleEndRad = angleEnd;
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

            public Vector3D PtAtAngle(double angleRad)
            {
                return (CS.BaseX * Radius).RotateAboutZAxis(angleRad).ToWCS(CS);
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
            /// verify if given point is in this arc between its start-to arc angles
            /// </summary>            
            public bool Contains(double tolLen, double tolRad, Vector3D pt)
            {
                // if not in circle stop
                if (!base.Contains(tolLen, pt)) return false;

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

            public IEnumerable<Vector3D> IntersectArc(double tol, Line3D l, bool segment_mode = false, bool arc_mode = true)
            {
                var q = Intersect(tol, l, segment_mode);
                if (q == null) return null;

                q = q.Where(r => this.Contains(tol, r, onlyAtCircumnfere: true)).ToList();
                if (q.Count() == 0) return null;

                return q;
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

}
