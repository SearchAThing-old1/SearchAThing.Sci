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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAThing.Sci
{

    public enum PhysicalQuantity
    {
        Length,
        Time
    };

    public static class MUManager
    {

        static double[,] _LengthConvMatrix;

        static double[] LengthConvFactors = new double[]
        {
            // keep in sync (*)

            1, // mm -> mm
            1e1, // dm -> mm
            1e2, // cm -> mm
            1e3 // m -> mm
        };

        static double[,] BuildConvMatrix(double[] convFactors)
        {
            var m = new double[convFactors.Length, convFactors.Length];

            // https://searchathing.com/?p=1326#MeasureUnitConversionMatrixStructure

            // fill first column
            for (int r = 1; r < convFactors.Length; ++r) m[r, 0] = convFactors[r];

            // fill diag
            for (int r = 0; r < LengthConvFactors.Length; ++r) m[r, r] = 1;

            // fill lower triangle
            for (int c = 1; c < LengthConvFactors.Length - 1; ++c)
            {

            }


            return m;
        }

        internal static double[,] LengthConvMatrix
        {
            get
            {
                if (_LengthConvMatrix == null)
                {
                    _LengthConvMatrix = BuildConvMatrix(LengthConvFactors);
                }
                return _LengthConvMatrix;
            }
        }

    };

    public enum LengthMeasureUnit
    {
        mm,
        dm,
        cm,
        m
    };

    public enum TimeMeasureUnit
    {
        s,
        m,
        h
    };

    public static partial class Ext
    {

        public static double Convert(this double value, LengthMeasureUnit from, LengthMeasureUnit to)
        {
            return value * MUManager.LengthConvMatrix[(int)from, (int)to];
        }

    };

}
