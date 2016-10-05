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

using System.Collections.Generic;
using SearchAThing.Sci;
using System.Text;
using System.Globalization;
using System.Linq;

namespace SearchAThing
{

    namespace Sci
    {

        public enum Line3DConstructMode { PointAndVector };

        public enum Line3DSegmentMode
        {
            /// <summary>
            /// infinite line
            /// </summary>
            None,

            /// <summary>
            /// Semi-line start at From
            /// </summary>
            From,

            /// <summary>
            /// Semi-line ending at To
            /// </summary>
            To,

            /// <summary>
            /// Segment from-to
            /// </summary>
            FromTo
        };

        public class Line3D
        {
            public static Line3D XAxisLine = new Line3D(Vector3D.Zero, Vector3D.XAxis);
            public static Line3D YAxisLine = new Line3D(Vector3D.Zero, Vector3D.YAxis);
            public static Line3D ZAxisLine = new Line3D(Vector3D.Zero, Vector3D.ZAxis);

            public Vector3D From { get; private set; }
            public Vector3D V { get; private set; }
            public Vector3D To { get { return From + V; } }
            public Vector3D Dir { get { return (To - From).Normalized(); } }

            /// <summary>
            /// retrieve a unique endpoint representation of this line3d segment (regardless its from-to or to-from order)
            /// such that From.Distance(Vector3D.Zero) less than To.Distance(Vector3D.Zero)
            /// </summary>
            public IEnumerable<Vector3D> DisambiguatedPoints
            {
                get
                {
                    if (From.Distance(Vector3D.Zero) < To.Distance(Vector3D.Zero))
                    {
                        yield return From;
                        yield return To;
                    }
                    else
                    {
                        yield return To;
                        yield return From;
                    }
                }
            }

            public IEnumerable<Vector3D> Points
            {
                get
                {
                    yield return From;
                    yield return To;
                }
            }

            /// <summary>
            /// build segment
            /// </summary>            
            public Line3D(Vector3D from, Vector3D to)
            {
                From = from;
                V = to - from;
            }

            /// <summary>
            /// build segment from plus the given vector form to
            /// </summary>            
            public Line3D(Vector3D from, Vector3D v, Line3DConstructMode mode)
            {
                From = from;
                V = v;
            }

            public double Length { get { return V.Length; } }

            /// <summary>
            /// Checks if two lines are equals ( it checks agains swapped from-to too )
            /// </summary>        
            public bool EqualsTol(double tol, Line3D other)
            {
                return
                    (From.EqualsTol(tol, other.From) && To.EqualsTol(tol, other.To))
                    ||
                    (From.EqualsTol(tol, other.To) && To.EqualsTol(tol, other.From));
            }

            /// <summary>
            /// returns the common point from,to between two lines or null if not consecutives
            /// </summary>        
            public Vector3D CommonPoint(double tol, Line3D other)
            {
                if (From.EqualsTol(tol, other.From)) return From;
                if (To.EqualsTol(tol, other.To)) return To;

                return null;
            }

            /// <summary>
            /// return the segment with swapped from,to
            /// </summary>            
            public Line3D Reverse()
            {
                return new Line3D(To, From);
            }

            #region operators
            /// <summary>
            /// multiply Length by given scalar factor
            /// Note : this will change To
            /// </summary>        
            public static Line3D operator *(double s, Line3D l)
            {
                return new Line3D(l.From, (l.To - l.From) * s, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// multiply Length by given scalar factor
            /// Note : this will change To
            /// </summary>        
            public static Line3D operator *(Line3D l, double s)
            {
                return new Line3D(l.From, (l.To - l.From) * s, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// Move this line of given delta adding value either at From, To
            /// </summary>            
            public static Line3D operator +(Line3D l, Vector3D delta)
            {
                return new Sci.Line3D(l.From + delta, l.V, Line3DConstructMode.PointAndVector);
            }

            /// <summary>
            /// Move this line of given delta subtracting value either at From, To
            /// </summary>            
            public static Line3D operator -(Line3D l, Vector3D delta)
            {
                return new Sci.Line3D(l.From - delta, l.V, Line3DConstructMode.PointAndVector);
            }
            #endregion

            /// <summary>
            /// Infinite line contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool LineContainsPoint(double tol, double x, double y, double z, bool segmentMode = false)
            {
                return LineContainsPoint(tol, new Vector3D(x, y, z), segmentMode);
            }

            /// <summary>
            /// Infinite line contains point.            
            /// </summary>        
            public bool LineContainsPoint(double tol, Vector3D p, bool segmentMode = false)
            {
                var prj = p.Project(this);

                var dprj = p.Distance(prj);

                // check if line contains point
                if (dprj > tol) return false;

                if (segmentMode)
                {
                    // line contains given point if there is a scalar s 
                    // for which p = From + s * V 
                    var s = 0.0;

                    // to find out the scalar we need to test the first non null component 

                    if (!(V.X.EqualsTol(tol, 0))) s = (p.X - From.X) / V.X;
                    else if (!(V.Y.EqualsTol(tol, 0))) s = (p.Y - From.Y) / V.Y;
                    else if (!(V.Z.EqualsTol(tol, 0))) s = (p.Z - From.Z) / V.Z;

                    // s is the scalar of V vector that runs From->To 

                    if (s >= 0.0 && s <= 1.0) return true;

                    // point on the line but outside exact segment
                    // check with tolerance

                    if (s < 0)
                        return p.EqualsTol(tol, From);
                    else
                        return p.EqualsTol(tol, To);
                }

                return true;
            }

            /// <summary>
            /// Finite segment contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool SegmentContainsPoint(double tol, Vector3D p)
            {
                return LineContainsPoint(tol, p, segmentMode: true);
            }

            /// <summary>
            /// Finite segment contains point.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool SegmentContainsPoint(double tol, double x, double y, double z)
            {
                return LineContainsPoint(tol, x, y, z, segmentMode: true);
            }

            /// <summary>
            /// Find intersection of two 3d lines
            /// </summary>        
            public Vector3D Intersect(double tol, Line3D other)
            {
                var f1x = From.X;
                var f1y = From.Y;
                var f1z = From.Z;

                var v1x = V.X;
                var v1y = V.Y;
                var v1z = V.Z;

                var f2x = other.From.X;
                var f2y = other.From.Y;
                var f2z = other.From.Z;

                var v2x = other.V.X;
                var v2y = other.V.Y;
                var v2z = other.V.Z;

                // this line  : F + alpha * V
                // other line : other.F + beta * V
                //
                // i = { F + alpha * V == other.F + beta * V }
                //   = { F1 + alpha * V1 == F2 + beta * V2 }
                // 
                // i = 
                //   f1x + alpha * v1x == f2x + beta * v2x &&
                //   f1y + alpha * v1y == f2y + beta * v2y &&
                //   f1z + alpha * v1z == f2z + beta * v2z

                // XY
                //   f1x + alpha * v1x == f2x + beta * v2x &&
                //   f1y + alpha * v1y == f2y + beta * v2y
                {
                    var alpha_denom = (v1y * v2x - v1x * v2y);
                    var beta_denom = (v1y * v2x - v1x * v2y);

                    if (!alpha_denom.EqualsTol(tol, 0) && !beta_denom.EqualsTol(tol, 0))
                    {
                        var alpha = -(f1y * v2x - f2y * v2x - f1x * v2y + f2x * v2y) / alpha_denom;
                        var beta = -(f1y * v1x - f2y * v1x - f1x * v1y + f2x * v1y) / beta_denom;

                        var i = From + alpha * V;

                        if (i.EqualsTol(tol, other.From + beta * other.V)) return i;
                    }
                }

                // XZ
                //   f1x + alpha * v1x == f2x + beta * v2x &&            
                //   f1z + alpha * v1z == f2z + beta * v2z
                {
                    var alpha_denom = (v1z * v2x - v1x * v2z);
                    var beta_denom = (v1z * v2x - v1x * v2z);

                    if (!alpha_denom.EqualsTol(tol, 0) && !beta_denom.EqualsTol(tol, 0))
                    {
                        var alpha = -(f1z * v2x - f2z * v2x - f1x * v2z + f2x * v2z) / alpha_denom;
                        var beta = -(f1z * v1x - f2z * v1x - f1x * v1z + f2x * v1z) / beta_denom;

                        var i = From + alpha * V;

                        if (i.EqualsTol(tol, other.From + beta * other.V)) return i;
                    }
                }

                // YZ            
                //   f1y + alpha * v1y == f2y + beta * v2y &&
                //   f1z + alpha * v1z == f2z + beta * v2z
                {
                    var alpha_denom = (v1z * v2y - v1y * v2z);
                    var beta_denom = (v1z * v2y - v1y * v2z);

                    if (!alpha_denom.EqualsTol(tol, 0) && !beta_denom.EqualsTol(tol, 0))
                    {
                        var alpha = -(f1z * v2y - f2z * v2y - f1y * v2z + f2y * v2z) / alpha_denom;
                        var beta = -(f1z * v1y - f2z * v1y - f1y * v1z + f2y * v1z) / beta_denom;

                        var i = From + alpha * V;

                        if (i.EqualsTol(tol, other.From + beta * other.V)) return i;
                    }
                }

                // no intersection

                return null;
            }

            /// <summary>
            /// Intersects two lines with arbitrary segment mode for each.
            /// </summary>        
            public Vector3D Intersect(double tol, Line3D other, bool thisSegment, bool otherSegment)
            {
                var i = Intersect(tol, other);
                if (i == null) return null;

                if (thisSegment && !SegmentContainsPoint(tol, i)) return null;
                if (otherSegment && !other.SegmentContainsPoint(tol, i)) return null;

                return i;
            }

            /// <summary>
            /// Build a perpendicular vector to this one starting from the given point p.
            /// </summary>        
            public Line3D Perpendicular(double tol, Vector3D p)
            {
                if (LineContainsPoint(tol, p)) return null;

                return new Line3D(p, p.Project(V));
            }

            /// <summary>
            /// Build a perpendicular vector to this one starting from the given point p
            /// and with To at the intersection point betweens.
            /// </summary>        
            public Line3D PerpendicularToIntersection(double tol, Vector3D p)
            {
                if (LineContainsPoint(tol, p)) return null;

                var i = new Line3D(p, p.Project(V)).Intersect(tol, this);

                return new Line3D(p, i);
            }

            public bool Colinear(double tol, Line3D other)
            {
                return LineContainsPoint(tol, other.From) && LineContainsPoint(tol, other.To);
            }

            public bool IsParallelTo(double tol, Plane3D plane)
            {
                return V.IsParallelTo(tol, plane.CS.BaseX) && V.IsParallelTo(tol, plane.CS.BaseY);
            }

            /// <summary>
            /// returns null if this line is parallel to the plane,
            /// the intersection point otherwise
            /// </summary>        
            public Vector3D Intersect(double tol, Plane3D plane)
            {
                if (IsParallelTo(tol, plane)) return null;

                // O = plane.Origin    Vx = plane.CS.BaseX    Vy = plane.CS.BaseY
                //
                // plane : O + alpha * Vx + beta * Vy
                // line  : From + gamma * V
                //
                // => m:{ alpha * Vx + beta * Vy - gamma * V } * s = n:{ From - O }

                var m = Matrix3D.FromVectorsAsColumns(plane.CS.BaseX, plane.CS.BaseY, -V);
                var n = From - plane.CS.Origin;
                var s = m.Solve(n);

                return From + s.Z * V;
            }
            public Vector3D MidPoint { get { return (From + To) / 2; } }

            public string CadScript
            {
                get
                {
                    return string.Format(CultureInfo.InvariantCulture, "LINE {0},{1},{2} {3},{4},{5}\r\n",
                        From.X, From.Y, From.Z, To.X, To.Y, To.Z);
                }
            }

            public override string ToString()
            {
                return $"{From}-{To} L={Length.ToString(2)} Δ={To - From}";
            }

        }

        public class Line3DEqualityComparer : IEqualityComparer<Line3D>
        {
            double tol;
            double tolHc;

            public Line3DEqualityComparer(double _tol)
            {
                tol = _tol;
                tolHc = 10 * tol; // to avoid rounding
            }

            public bool Equals(Line3D x, Line3D y)
            {
                return x.EqualsTol(tol, y);
            }

            public int GetHashCode(Line3D obj)
            {
                return (int)((
                    (obj.From.X + obj.To.X) / 2 +
                    (obj.From.Y + obj.To.Y) / 2 +
                    (obj.From.Z + obj.To.Z) / 2) / tolHc);
            }

        }

    }

    public static partial class Extensions
    {

        public static string ToCadScript(this IEnumerable<Line3D> lines)
        {
            var sb = new StringBuilder();

            foreach (var l in lines)
            {
                sb.AppendLine(l.CadScript);
            }

            return sb.ToString();
        }

        public static Line3D ToLine3D(this netDxf.Entities.Line line)
        {
            return new Line3D(line.StartPoint, line.EndPoint);
        }

        /// <summary>
        /// retrieve s[0].from, s[1].from, ... s[n-1].from, s[n-1].to points
        /// </summary>        
        public static IEnumerable<Vector3D> PolyPoints(this IEnumerable<Line3D> segs)
        {
            var en = segs.GetEnumerator();

            Line3D seg = null;

            while (en.MoveNext())
            {
                seg = en.Current;
                yield return seg.From;
            }

            yield return seg.To;
        }

        /// <summary>
        /// merge colinear overlapped segments into single
        /// result segments direction and order is not ensured
        /// pre: segs must colinear
        /// </summary>        
        public static IEnumerable<Line3D> MergeColinearSegments(this IEnumerable<Line3D> _segs, double tol_len)
        {
            var result_dir = _segs.First().V;
            var segs = new List<Line3D>(_segs);

            bool found_overlaps;
            do
            {
                var to_remove = new List<Line3D>();
                var to_add = new List<Line3D>();
                found_overlaps = false;

                for (int i = 0; !found_overlaps && i < segs.Count; ++i)
                {
                    for (int j = i + 1; j < segs.Count; ++j)
                    {
                        var i_contains_j_from = segs[i].SegmentContainsPoint(tol_len, segs[j].From);
                        var i_contains_j_to = segs[i].SegmentContainsPoint(tol_len, segs[j].To);

                        // i contains j entirely
                        if (i_contains_j_from && i_contains_j_to)
                        {
                            to_remove.Add(segs[j]);
                            found_overlaps = true;
                            break;
                        }

                        // i contains only j from but not j to
                        if (i_contains_j_from)
                        {
                            to_remove.Add(segs[i]);
                            to_remove.Add(segs[j]);
                            if (segs[i].V.Concordant(tol_len, segs[j].V))
                                to_add.Add(new Line3D(segs[i].From, segs[j].To));
                            else
                                to_add.Add(new Line3D(segs[i].To, segs[j].To));

                            found_overlaps = true;
                            break;
                        }

                        // i contains only j to but not j from
                        if (i_contains_j_to)
                        {
                            to_remove.Add(segs[i]);
                            to_remove.Add(segs[j]);
                            if (segs[i].V.Concordant(tol_len, segs[j].V))
                                to_add.Add(new Line3D(segs[j].From, segs[i].To));
                            else
                                to_add.Add(new Line3D(segs[i].From, segs[j].From));

                            found_overlaps = true;
                            break;
                        }
                    }
                }

                to_remove.ForEach(w => segs.Remove(w));
                segs.AddRange(to_add);
            }
            while (found_overlaps);

            return segs;
        }

    }

}
