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

namespace SearchAThing.Sci
{

    public static partial class Extentions
    {

        /// <summary>
        /// Area of a polygon (does not consider z)
        /// https://en.wikipedia.org/wiki/Centroid        
        /// </summary>        
        public static double Area(this List<Vector3D> pts, double tol)
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
        /// https://en.wikipedia.org/wiki/Centroid        
        /// </summary>        
        public static Vector3D Centroid(this List<Vector3D> pts, double tol, double area)
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

    }

}
