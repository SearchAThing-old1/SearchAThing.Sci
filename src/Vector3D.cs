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

using sVector3D = System.Windows.Media.Media3D.Vector3D;
using sMatrix3D = System.Windows.Media.Media3D.Matrix3D;
using sQuaternion = System.Windows.Media.Media3D.Quaternion;
using System.Globalization;

namespace SearchAThing.Sci
{

    public class Vector3D
    {

        public static Vector3D XAxis = new Vector3D(1, 0, 0);
        public static Vector3D YAxis = new Vector3D(0, 1, 0);
        public static Vector3D ZAxis = new Vector3D(0, 0, 1);

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Vector3D(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public bool EqualsTolLen(Vector3D other, IModel model)
        {
            return
                X.EqualsTolLen(other.X, model) &&
                Y.EqualsTolLen(other.Y, model) &&
                Z.EqualsTolLen(other.Z, model);
        }

        public bool EqualsTolNormLen(Vector3D other, IModel model)
        {
            return
                X.EqualsTolNormLen(other.X, model) &&
                Y.EqualsTolNormLen(other.Y, model) &&
                Z.EqualsTolNormLen(other.Z, model);
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
        /// Dot product
        /// a b = |a| |b| cos(alfa)
        /// </summary>        
        public double DotProduct(Vector3D other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        /// <summary>
        /// Cross product
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
        /// </summary>        
        public double AngleRad(Vector3D to, IModel model)
        {
            if (this.EqualsTolLen(to, model)) return 0;

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

            return DotProduct(to) / Length * Normalized();
        }

        public bool Concordant(Vector3D other)
        {
            return DotProduct(other) > 0;
        }

        /// <summary>
        /// Angle (rad) between this going toward the given other vector
        /// rotating (right-hand-rule) around the given comparing axis
        /// </summary>        
        public double AngleToward(Vector3D to, Vector3D refAxis, IModel model)
        {
            var c = this.CrossProduct(to);

            if (c.Concordant(refAxis))
                return this.AngleRad(to, model);
            else
                return 2 * PI - AngleRad(to, model);
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
            t.RotateAboutAxis(axis, angleRad);
            return t.Apply(this);
        }

        public Vector3D RotateAs(Vector3D from, Vector3D to, IModel model)
        {
            var angle = from.AngleRad(to, model);
            var N = from.CrossProduct(to);
            return this.RotateAboutAxis(N, angle);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a)
        {
            return -1.0 * a;
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3D operator *(double s, Vector3D v)
        {
            return new Vector3D(s * v.X, s * v.Y, s * v.Z);
        }

        public static Vector3D operator /(double s, Vector3D v)
        {
            return new Vector3D(s / v.X, s / v.Y, s / v.Z);
        }

        public sVector3D ToSystemVector3D()
        {
            return new sVector3D(X, Y, Z);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1},{2})", X, Y, Z);
        }

    }

    public static partial class Extensions
    {

        public static Vector3D ToVector3D(this sVector3D v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }

    }

}
