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
using System.Collections;
using System.Collections.Generic;

namespace SearchAThing.Sci
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

        public double Area { get { return PI * Radius * Radius; } }
        public double Length { get { return 2 * PI * Radius; } }

        public Vector3D Center { get { return CS.Origin; } }

        public bool Contains(double tol, Vector3D pt)
        {
            return pt.ToUCS(CS).Z.EqualsTol(tol, 0) && pt.Distance(CS.Origin).LessThanOrEqualsTol(tol, Radius);
        }

    }


}
