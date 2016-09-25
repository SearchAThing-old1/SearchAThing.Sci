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

using static System.Math;
using System.Collections.Generic;
using SearchAThing.Sci;

namespace SearchAThing
{

    namespace Sci
    {

        public class BBox3D
        {
            public bool IsEmpty { get { return Min == null; } }

            public Vector3D Min { get; private set; }
            public Vector3D Max { get; private set; }

            public BBox3D()
            {
            }

            public BBox3D Scale(double factor)
            {
                var center = (Min + Max) / 2;

                return new BBox3D(new Vector3D[]
                {
                    Min.ScaleAbout(center, factor),
                    Max.ScaleAbout(center, factor)
                });
            }

            public BBox3D(IEnumerable<Vector3D> pts)
            {
                double xmin = 0, ymin = 0, zmin = 0;
                double xmax = 0, ymax = 0, zmax = 0;

                bool firstPt = true;

                foreach (var p in pts)
                {
                    if (firstPt)
                    {
                        xmin = xmax = p.X;
                        ymin = ymax = p.Y;
                        zmin = zmax = p.Z;
                        firstPt = false;
                    }
                    else
                    {
                        xmin = Min(xmin, p.X);
                        ymin = Min(ymin, p.Y);
                        zmin = Min(zmin, p.Z);

                        xmax = Max(xmax, p.X);
                        ymax = Max(ymax, p.Y);
                        zmax = Max(zmax, p.Z);
                    }
                }
                Min = new Vector3D(xmin, ymin, zmin);
                Max = new Vector3D(xmax, ymax, zmax);
            }

            public BBox3D Union(Vector3D p)
            {
                if (IsEmpty)
                {
                    return new BBox3D()
                    {
                        Min = p,
                        Max = p
                    };
                }
                else
                {
                    return new BBox3D()
                    {
                        Min = new Vector3D(Min(Min.X, p.X), Min(Min.Y, p.Y), Min(Min.Z, p.Z)),
                        Max = new Vector3D(Max(Max.X, p.X), Max(Max.Y, p.Y), Max(Max.Z, p.Z))
                    };
                }
            }

            public BBox3D Union(BBox3D other)
            {
                if (IsEmpty) return other;
                if (other.IsEmpty) return this;
                return this.Union(other.Min).Union(other.Max);
            }

            public bool EqualsTol(double tol, BBox3D other)
            {
                if (IsEmpty) return other.IsEmpty;
                if (other.IsEmpty) return false;
                return Min.EqualsTol(tol, other.Min) && Max.EqualsTol(tol, other.Max);
            }

            public bool Contains(double tol, BBox3D other)
            {
                if (IsEmpty) return false;
                if (other.IsEmpty) return true;
                return
                    other.Min.X.GreatThanOrEqualsTol(tol, Min.X) &&
                    other.Min.Y.GreatThanOrEqualsTol(tol, Min.Y) &&
                    other.Min.Z.GreatThanOrEqualsTol(tol, Min.Z) &&
                    other.Max.X.LessThanOrEqualsTol(tol, Max.X) &&
                    other.Max.Y.LessThanOrEqualsTol(tol, Max.Y) &&
                    other.Max.Z.LessThanOrEqualsTol(tol, Max.Z);
            }

            public override string ToString()
            {
                return $"{Min}-{Max}";
            }
        }

    }

    public static partial class Extensions
    {

        public static BBox3D BBox(this IEnumerable<Vector3D> pts)
        {
            return new BBox3D(pts);
        }

    }

}
