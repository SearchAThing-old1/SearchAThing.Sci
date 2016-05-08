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

    public static partial class Extensions
    {

        public static bool EqualsTol(this double x, double y, double tol)
        {
            return Abs(x - y) <= tol;
        }

        public static bool EqualsAutoTol(this double x, double y)
        {
            return x.EqualsTol(y, Abs(x * 1e-6));
        }

        public static bool GreatThanTol(this double x, double y, double tol)
        {
            return x > y && !x.EqualsTol(y, tol);
        }

        public static bool GreatThanOrEqualsTol(this double x, double y, double tol)
        {
            return x > y || x.EqualsTol(y, tol);
        }

        public static bool LessThanTol(this double x, double y, double tol)
        {
            return x < y && !x.EqualsTol(y, tol);
        }

        public static bool LessThanOrEqualsTol(this double x, double y, double tol)
        {
            return x < y || x.EqualsTol(y, tol);
        }

        // tolerance helpers for len type

        /// <summary>
        /// States if x equals y apart the Length tolerance specified through the domain model.
        /// x,y length measure unit implicitly from the my domain.
        /// </summary>        
        public static bool EqualsTolLen(this double x, double y, IModel model)
        {
            return Abs(x - y) <= model.MUDomain.Length.Value;
        }

        public static bool EqualsTolNormLen(this double x, double y, IModel model)
        {
            return Abs(x - y) <= 1e-4;
        }

        public static bool GreatThanTolLen(this double x, double y, IModel model)
        {
            return x > y && !x.EqualsTolLen(y, model);
        }

        public static bool GreatThanOrEqualsTolLen(this double x, double y, IModel model)
        {
            return x > y || x.EqualsTolLen(y, model);
        }

        public static bool LessThanTolLen(this double x, double y, IModel model)
        {
            return x < y && !x.EqualsTolLen(y, model);
        }

        public static bool LessThanOrEqualsTolLen(this double x, double y, IModel model)
        {
            return x < y || x.EqualsTolLen(y, model);
        }

    }

}
