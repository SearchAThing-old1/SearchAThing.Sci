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
using static System.Math;

using sVector3D = Microsoft.Xna.Framework.Vector3;// System.Windows.Media.Media3D.Vector3D;
using System.Globalization;
using System.Collections.Generic;
using SearchAThing.Sci;
using System.Text;
using System.Windows;

namespace SearchAThing
{

    namespace Sci
    {

        public partial class Vector3D
        {

            public static Vector3D Zero = new Vector3D(0, 0, 0);
            public static Vector3D XAxis = new Vector3D(1, 0, 0);
            public static Vector3D YAxis = new Vector3D(0, 1, 0);
            public static Vector3D ZAxis = new Vector3D(0, 0, 1);

            public static Vector3D Axis(int ord)
            {
                switch (ord)
                {
                    case 0: return XAxis;
                    case 1: return YAxis;
                    case 2: return ZAxis;
                    default: throw new ArgumentException($"invalid ord {ord} must between 0,1,2");
                }
            }

            public double X { get; private set; }
            public double Y { get; private set; }
            public double Z { get; private set; }

            public Vector3D()
            {
            }

            public Vector3D(double x, double y, double z)
            {
                X = x; Y = y; Z = z;
            }

            /// <summary>
            /// initialize 3d vector with z implicitly 0
            /// </summary>        
            public Vector3D(double x, double y)
            {
                X = x; Y = y;
            }

            /// <summary>
            /// retrieve the component (0:X, 1:Y, 2:Z)
            /// </summary>        
            public double GetOrd(int ord)
            {
                switch (ord)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new ArgumentException($"invalid ord {ord}. Must between one of 0,1,2");
                }
            }

            public IEnumerable<double> Coordinates
            {
                get
                {
                    yield return X;
                    yield return Y;
                    yield return Z;
                }
            }

            public bool IsZeroLength { get { return (X + Y + Z).EqualsTol(Constants.NormalizedLengthTolerance, 0); } }

            /// <summary>
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool EqualsTol(double tol, Vector3D other)
            {
                return
                    X.EqualsTol(tol, other.X) &&
                    Y.EqualsTol(tol, other.Y) &&
                    Z.EqualsTol(tol, other.Z);
            }

            /// <summary>
            /// checks only x,y
            /// </summary>        
            public bool EqualsTol(double tol, double x, double y)
            {
                return X.EqualsTol(tol, x) && Y.EqualsTol(tol, y);
            }

            public bool EqualsTol(double tol, double x, double y, double z)
            {
                return X.EqualsTol(tol, x) && Y.EqualsTol(tol, y) && Z.EqualsTol(tol, z);
            }

            public double Length { get { return Sqrt(X * X + Y * Y + Z * Z); } }

            public Vector3D Normalized()
            {
                var l = Length;
                return new Vector3D(X / l, Y / l, Z / l);
            }

            public double Distance(Vector3D other)
            {
                return (this - other).Length;
            }

            /// <summary>
            /// distance between two points ( without considering Z )
            /// </summary>            
            public double Distance2D(Vector3D other)
            {
                return Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
            }

            /// <summary>
            /// Dot product
            /// a b = |a| |b| cos(alfa)
            /// </summary>        
            public double DotProduct(Vector3D other)
            {
                return X * other.X + Y * other.Y + Z * other.Z;
            }

            /// <summary>
            /// Cross product ( note that resulting vector is not subjected to normalization )
            /// a x b = |a| |b| sin(alfa) N
            /// a x b = |  x  y  z |
            ///         | ax ay az |
            ///         | bx by bz |
            /// https://en.wikipedia.org/wiki/Cross_product
            /// </summary>        
            public Vector3D CrossProduct(Vector3D other)
            {
                return new Vector3D(
                    Y * other.Z - Z * other.Y,
                    -X * other.Z + Z * other.X,
                    X * other.Y - Y * other.X);
            }

            /// <summary>
            /// Angle (rad) between this and other given vector.
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public double AngleRad(double tolLen, Vector3D to)
            {
                if (this.EqualsTol(tolLen, to)) return 0;

                // dp = |a| |b| cos(alfa)
                var dp = this.DotProduct(to);

                // alfa = acos(dp / (|a| |b|))
                var w = dp / (Length * to.Length);

                return Acos(w);
            }

            /// <summary>
            /// project this vector to the other given,
            /// the resulting vector will be colinear the given one
            /// </summary>        
            public Vector3D Project(Vector3D to)
            {
                // https://en.wikipedia.org/wiki/Vector_projection
                // http://math.oregonstate.edu/bridge/papers/dot+cross.pdf (fig.1)

                return DotProduct(to) / to.Length * to.Normalized();
            }

            /// <summary>
            /// project this vector to the given line
            /// </summary>            
            public Vector3D Project(Line3D line)
            {
                return (this - line.From).Project(line.V) + line.From;
            }

            /// <summary>
            /// return a copy of this vector with ordinate ( 0:x 1:y 2:z ) changed
            /// </summary>            
            public Vector3D Set(OrdIdx ordIdx, double value)
            {
                var x = X;
                var y = Y;
                var z = Z;

                switch (ordIdx)
                {
                    case OrdIdx.X: x = value; break;
                    case OrdIdx.Y: y = value; break;
                    case OrdIdx.Z: z = value; break;
                    default: throw new Exception($"invalid ordIdx:{ordIdx}");
                }

                return new Vector3D(x, y, z);
            }

            /// <summary>
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool Concordant(double tol, Vector3D other)
            {
                return DotProduct(other) > tol;
            }

            /// <summary>
            /// Angle (rad) between this going toward the given other vector
            /// rotating (right-hand-rule) around the given comparing axis
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public double AngleToward(double tolLen, Vector3D to, Vector3D refAxis)
            {
                var c = this.CrossProduct(to);

                if (c.Concordant(tolLen, refAxis))
                    return this.AngleRad(tolLen, to);
                else
                    return 2 * PI - AngleRad(tolLen, to);
            }

            public Vector3D RotateAboutXAxis(double angleRad)
            {
                var t = new Transform3D();
                t.RotateAboutXAxis(angleRad);
                return t.Apply(this);
            }

            public Vector3D RotateAboutYAxis(double angleRad)
            {
                var t = new Transform3D();
                t.RotateAboutYAxis(angleRad);
                return t.Apply(this);
            }

            public Vector3D RotateAboutZAxis(double angleRad)
            {
                var t = new Transform3D();
                t.RotateAboutZAxis(angleRad);
                return t.Apply(this);
            }

            public Vector3D RotateAboutAxis(Vector3D axis, double angleRad)
            {
                var t = new Transform3D();
                t.RotateAboutAxis(axis.Normalized(), angleRad);
                return t.Apply(this);
            }

            public Vector3D RotateAboutAxis(Line3D axisSegment, double angleRad)
            {
                var vrel = this - axisSegment.From;
                var vrot = vrel.RotateAboutAxis(axisSegment.V, angleRad);
                return vrot + axisSegment.From;
            }

            /// <summary>
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public Vector3D RotateAs(double tol, Vector3D from, Vector3D to)
            {
                var angle = from.AngleRad(tol, to);
                var N = from.CrossProduct(to);
                return this.RotateAboutAxis(N, angle);
            }

            /// <summary>
            /// Scale this point about the given origin with the given factor.
            /// </summary>            
            public Vector3D ScaleAbout(Vector3D origin, double factor)
            {
                var d = this - origin;

                return origin + d * factor;
            }

            /// <summary>
            /// Scale this point about the given origin with the given factor as (sx,sy,sz).
            /// </summary>            
            public Vector3D ScaleAbout(Vector3D origin, Vector3D factor)
            {
                var d = this - origin;

                return origin + d * factor;
            }

            /// <summary>
            /// Note: tol must be Constant.NormalizedLengthTolerance
            /// if comparing normalized vectors
            /// </summary>        
            public bool IsParallelTo(double tol, Vector3D other)
            {
                // two vectors a,b are parallel if there is a factor c such that a=cb
                // but first we need to exclude test over null components

                var nullSum = 0;

                var xNull = false;
                var yNull = false;
                var zNull = false;

                if (X.EqualsTol(tol, 0) && other.X.EqualsTol(tol, 0)) { xNull = true; ++nullSum; }
                if (Y.EqualsTol(tol, 0) && other.Y.EqualsTol(tol, 0)) { yNull = true; ++nullSum; }
                if (Z.EqualsTol(tol, 0) && other.Z.EqualsTol(tol, 0)) { zNull = true; ++nullSum; }

                if (nullSum == 0) // 3-d
                {
                    var c = X / other.X;
                    return c.EqualsTol(tol, Y / other.Y) && c.EqualsTol(tol, Z / other.Z);
                }
                else if (nullSum == 1) // 2-d
                {
                    if (xNull) return (Y / other.Y).EqualsTol(tol, Z / other.Z);
                    if (yNull) return (X / other.X).EqualsTol(tol, Z / other.Z);
                    if (zNull) return (X / other.X).EqualsTol(tol, Y / other.Y);
                }
                else if (nullSum == 2) // 1-d
                {
                    return true;
                }

                return false;
            }

            public Vector3D ToUCS(CoordinateSystem3D cs)
            {
                return cs.ToUCS(this);
            }

            public Vector3D ToWCS(CoordinateSystem3D cs)
            {
                return cs.ToWCS(this);
            }

            /// <summary>
            /// Scalar multiply each components
            /// </summary>                
            public Vector3D Scalar(double xs, double ys, double zs)
            {
                return new Vector3D(X * xs, Y * ys, Z * zs);
            }

            public Vector3D Convert(MeasureUnit from, MeasureUnit to)
            {
                return new Vector3D(X.Convert(from, to), Y.Convert(from, to), Z.Convert(from, to));
            }

            #region operators

            /// <summary>
            /// indexed vector component
            /// </summary>        
            public double this[int index]
            {
                get
                {
                    if (index == 0) return X;
                    if (index == 1) return Y;
                    if (index == 2) return Z;
                    throw new ArgumentOutOfRangeException("invalid index must between 0-2");
                }
            }

            /// <summary>
            /// sum
            /// </summary>        
            public static Vector3D operator +(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            }

            /// <summary>
            /// negate
            /// </summary>        
            public static Vector3D operator -(Vector3D a)
            {
                return -1.0 * a;
            }

            /// <summary>
            /// sub
            /// </summary>        
            public static Vector3D operator -(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            }

            /// <summary>
            /// scalar mul
            /// </summary>        
            public static Vector3D operator *(double s, Vector3D v)
            {
                return new Vector3D(s * v.X, s * v.Y, s * v.Z);
            }

            /// <summary>
            /// scalar mul
            /// </summary>        
            public static Vector3D operator *(Vector3D v, double s)
            {
                return new Vector3D(s * v.X, s * v.Y, s * v.Z);
            }

            /// <summary>
            /// scalar multiply vector components V1 * V2 =
            /// (V1.x * V2.x, V1.y * V2.y, V1.z * V2.z)
            /// </summary>        
            public static Vector3D operator *(Vector3D v1, Vector3D v2)
            {
                return v1.Scalar(v2.X, v2.Y, v2.Z);
            }

            /// <summary>
            /// scalar div
            /// </summary>        
            public static Vector3D operator /(double s, Vector3D v)
            {
                return new Vector3D(s / v.X, s / v.Y, s / v.Z);
            }

            /// <summary>
            /// scalar div
            /// </summary>        
            public static Vector3D operator /(Vector3D v, double s)
            {
                return new Vector3D(v.X / s, v.Y / s, v.Z / s);
            }

            #endregion

            /// <summary>
            /// Create an array of Vector3D from given list of 2d coords ( eg. { 100, 200, 300, 400 }
            /// will create follow list of vector3d = { (100,200,0), (300,400,0) }
            /// </summary>        
            public static List<Vector3D> From2DCoords(params double[] coords)
            {
                var res = new List<Vector3D>();

                for (var i = 0; i < coords.Length; i += 2)
                    res.Add(new Vector3D(coords[i], coords[i + 1], 0));

                return res;
            }

            /// <summary>
            /// Create an array of Vector3D from given list of 3d coords ( eg. { 100, 200, 10, 300, 400, 20 }
            /// will create follow list of vector3d = { (100,200,10), (300,400,20) }
            /// </summary>        
            public static List<Vector3D> From3DCoords(params double[] coords)
            {
                var res = new List<Vector3D>();

                for (var i = 0; i < coords.Length; i += 3)
                    res.Add(new Vector3D(coords[i], coords[i + 1], coords[i + 2]));

                return res;
            }

            public static IEnumerable<Vector3D> Random(int N, double L, int seed = 0)
            {
                return Random(N, -L / 2, L / 2, -L / 2, L / 2, -L / 2, L / 2, seed);
            }

            /// <summary>
            /// Span a set of qty vector3d with random coord between given range.
            /// Optionally a seed can be specified for rand.
            /// </summary>        
            public static IEnumerable<Vector3D> Random(int qty,
                double xmin, double xmax, double ymin, double ymax, double zmin, double zmax, int seed = 0)
            {
                var dx = xmax - xmin;
                var dy = ymax - ymin;
                var dz = zmax - zmin;

                var rnd = new Random(seed);
                for (int i = 0; i < qty; ++i)
                {
                    yield return new Vector3D(
                        xmin + dx * rnd.NextDouble(),
                        ymin + dy * rnd.NextDouble(),
                        zmin + dz * rnd.NextDouble());
                }
            }

            public sVector3D ToSystemVector3D()
            {
                return new sVector3D((float)X, (float)Y, (float)Z);
            }

            public override string ToString()
            {
                return $"({X.ToString(3)}, {Y.ToString(3)}, {Z.ToString(3)})";
            }

        }

        public class Vector3DEqualityComparer : IEqualityComparer<Vector3D>
        {
            double tol;
            double tolHc;

            public Vector3DEqualityComparer(double _tol)
            {
                tol = _tol;
                tolHc = 10 * tol; // to avoid rounding
            }

            public bool Equals(Vector3D x, Vector3D y)
            {
                return x.EqualsTol(tol, y);
            }

            public int GetHashCode(Vector3D obj)
            {
                return (int)((obj.X + obj.Y + obj.Z) / tolHc);
            }
        }

    }

    public enum OrdIdx
    {
        X = 0,
        Y = 1,
        Z = 2
    }

    public enum CadPointMode
    {
        Point,
        Circle
    };

    public static partial class Extensions
    {

        /// <summary>
        /// compute length of polyline from given seq_pts
        /// </summary>        
        public static double Length(this IEnumerable<Vector3D> seq_pts)
        {
            var l = 0.0;

            Vector3D prev = null;
            var en = seq_pts.GetEnumerator();
            while (en.MoveNext())
            {
                if (prev != null) l += prev.Distance(en.Current);
                prev = en.Current;
            }

            return l;
        }        

        /// <summary>
        /// from a list of vector3d retrieve x1,y1,z1,x2,y2,z2,... coord sequence
        /// </summary>        
        public static IEnumerable<double> ToCoordSequence(this IEnumerable<Vector3D> pts)
        {
            foreach (var p in pts)
            {
                yield return p.X;
                yield return p.Y;
                yield return p.Z;
            }
        }

        /// <summary>
        /// produce a string with x1,y1,x2,y2, ...
        /// </summary>        
        public static string ToCoordSequence2D(this IEnumerable<Vector3D> points)
        {
            var sb = new StringBuilder();

            var en = points.GetEnumerator();

            if (en.MoveNext())
            {
                while (true)
                {
                    var p = en.Current;
                    sb.Append(string.Format(CultureInfo.InvariantCulture, "{0},{1}", p.X, p.Y));

                    if (en.MoveNext())
                        sb.Append(",");
                    else
                        break;
                }
            }

            return sb.ToString();
        }

        public static string ToCadScript(this IEnumerable<Vector3D> points, CadPointMode mode = CadPointMode.Point, double radius = 10)
        {
            var sb = new StringBuilder();

            switch (mode)
            {
                case CadPointMode.Point:
                    {
                        foreach (var p in points)
                        {
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "POINT {0},{1},{2}", p.X, p.Y, p.Z));
                            sb.AppendLine();
                        }
                    }
                    break;

                case CadPointMode.Circle:
                    {
                        foreach (var p in points)
                        {
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "CIRCLE {0},{1},{2} {3}", p.X, p.Y, p.Z, radius));
                            sb.AppendLine();
                        }
                    }
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// checks two list of vectors are equals and with same order of elements        
        /// </summary>        
        public static bool EqualsTol(this IEnumerable<Vector3D> lst, double tol, IEnumerable<Vector3D> other)
        {
            var thisEn = lst.GetEnumerator();
            var otherEn = other.GetEnumerator();

            while (thisEn.MoveNext())
            {
                var thisValue = thisEn.Current;

                if (!otherEn.MoveNext()) return false; // other smaller than this

                if (!thisValue.EqualsTol(tol, otherEn.Current)) return false;
            }

            if (otherEn.MoveNext()) return false; // other greather than this

            return true;
        }

        public static Vector3D Sum(this IEnumerable<Vector3D> lst)
        {
            var s = Vector3D.Zero;
            foreach (var v in lst) s += v;

            return s;
        }

        /// <summary>
        /// Same as mean
        /// </summary>
        [Obsolete("use Mean instead")]
        public static Vector3D Center(this IEnumerable<Vector3D> lst)
        {
            return lst.Mean();
        }

        /// <summary>
        /// mean of given vetor3d list
        /// </summary>        
        public static Vector3D Mean(this IEnumerable<Vector3D> lst)
        {
            var n = 0;
            var s = Vector3D.Zero;
            foreach (var v in lst) { s += v; ++n; }

            return s / n;
        }

        public static Vector3D ToVector3D(this sVector3D v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }

        public static Vector3D ToVector3D(this netDxf.Vector2 v)
        {
            return new Vector3D(v.X, v.Y);
        }

        /* public static netDxf.Vector3 ToVector3(this Vector3D v)
         {
             return new netDxf.Vector3(v.X, v.Y, v.Z);
         }*/

        public static netDxf.Vector2 ToVector2(this Vector3D v)
        {
            return new netDxf.Vector2(v.X, v.Y);
        }

        /// <summary>
        /// To point (double x, double y)
        /// </summary>        
        public static Point ToPoint(this Vector3D v)
        {
            return new Point(v.X, v.Y);
        }

        /// <summary>
        /// creates a psql double[] string
        /// </summary>
        public static string ToPsql(this Vector3D v)
        {
            return v.Coordinates.ToPsql();
        }

        /// <summary>
        /// create a psql representation of double[] coord sequence x1,y1,z1,x2,y2,z2, ... of given points
        /// </summary>        
        public static string ToPsql(this IEnumerable<Vector3D> pts)
        {
            return pts.ToCoordSequence().ToPsql();
        }

    }

}
