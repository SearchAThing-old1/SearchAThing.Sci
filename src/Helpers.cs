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
using System.Globalization;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// convert given angle(rad) to degree
        /// </summary>        
        public static double ToDeg(this double angleRad)
        {
            return angleRad / PI * 180.0;
        }

        /// <summary>
        /// convert given angle(grad) to radians
        /// </summary>        
        public static double ToRad(this double angleGrad)
        {
            return angleGrad / 180.0 * PI;
        }

        /// <summary>
        /// Return an invariant string representation rounded to given dec.        
        public static string Stringify(this double x, int dec)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", Round(x, dec));
        }

        /// <summary>
        /// Magnitude of given number. (eg. 190 -> 1.9e2 -> 2)
        /// (eg. 0.0034 -> 3.4e-3 -> -3)
        /// </summary>        
        public static int Magnitude(this double value)
        {
            var a = Abs(value);

            if (a < double.Epsilon) return 0;

            return (int)Floor(Log10(a));
        }

        /// <summary>
        /// Round the given value using the multiple basis
        /// </summary>        
        public static double MRound(this double value, double multiple)
        {
            var p = Round(value / multiple);

            return Truncate(p) * multiple;
        }

    }

}
