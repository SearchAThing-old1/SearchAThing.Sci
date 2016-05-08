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

namespace SearchAThing.Sci
{

    public class Vector3D
    {

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

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
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

    }

}
