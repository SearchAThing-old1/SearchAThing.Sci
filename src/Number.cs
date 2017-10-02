#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016-2017 Lorenzo Delana, https://searchathing.com
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
using SearchAThing;
using System.Collections.Generic;
using System.Linq;
using SearchAThing.Sci;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// ensure given angle in [0,2*PI] range
        /// </summary>        
        public static double NormalizeAngle2PI(this double angle_rad)
        {
            var n = (int)(angle_rad / (2 * PI));

            var excess = (n != 0) ? (n.Sign() * 2 * PI) : 0;

            return angle_rad - excess;
        }

        /// <summary>
        /// retrieve min,max w/single sweep
        /// </summary>        
        public static (double min, double max) MinMax(this IEnumerable<double> input)
        {
            double? _min = null;
            double? _max = null;

            foreach (var x in input)
            {
                if (_min.HasValue) _min = Min(_min.Value, x); else _min = x;
                if (_max.HasValue) _max = Max(_max.Value, x); else _max = x;
            }

            return (_min.Value, _max.Value);
        }

        /// <summary>
        /// retrieve given input set ordered with only distinct values after comparing through tolerance
        /// in this case result set contains only values from the input set (default) or rounding to given tol if maintain_original_values is false;
        /// if keep_ends true (default) min and max already exists at begin/end of returned sequence
        /// </summary>        
        public static List<double> Thin(this IEnumerable<double> input, double tol, bool keep_ends = true, bool maintain_original_values = true)
        {
            var res = new List<double>();

            var dcmp = new DoubleEqualityComparer(tol);

            if (maintain_original_values)
                res = input.Distinct(dcmp).OrderBy(w => w).ToList();
            else
                res = input.Select(w => w.MRound(tol)).Distinct(dcmp).OrderBy(w => w).ToList();

            var minmax = input.MinMax();

            if (!res.First().EqualsTol(tol, minmax.min)) res.Insert(0, minmax.min);
            if (!res.Last().EqualsTol(tol, minmax.max)) res.Add(minmax.max);

            return res;
        }

    }

}
