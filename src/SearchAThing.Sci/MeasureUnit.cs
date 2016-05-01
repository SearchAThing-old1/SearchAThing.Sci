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

    // keep in sync (*) don't change previous order, append only
    public enum LengthMeasureUnit
    {
        mm,        
        cm,
        dm,
        m
    };

    // keep in sync (*) don't change previous order, append only
    public enum TimeMeasureUnit
    {
        s,
        m,
        h
    };

    public static class MUManager
    {

        static double[,] BuildConvMatrix(double[] convFactors)
        {
            var m = new double[convFactors.Length, convFactors.Length];

            // https://searchathing.com/?p=1326#MeasureUnitConversionMatrixStructure

            // fill first column
            for (int r = 1; r < convFactors.Length; ++r) m[r, 0] = convFactors[r];

            // fill diag
            for (int r = 0; r < convFactors.Length; ++r) m[r, r] = 1;

            // fill lower triangle
            for (int c = 1; c < convFactors.Length - 1; ++c)
            {
                for (int r = c; r < convFactors.Length; ++r)
                {
                    m[r, c] = m[r, 0] / m[c, 0];
                }
            }

            // fill upper triangle
            for (int c = 1; c < convFactors.Length; ++c)
            {
                for (int r = 0; r < c; ++r)
                {
                    m[r, c] = 1.0 / m[c, r];
                }
            }

            return m;
        }

        #region Length

        static double[,] _LengthConvMatrix;

        static double[] LengthConvFactors = new double[]
        {
            // keep in sync (*)

            1, // mm -> mm
            1e1, // dm -> mm
            1e2, // cm -> mm
            1e3 // m -> mm
        };        

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

        #endregion

        #region Time

        static double[,] _TimeConvMatrix;

        static double[] TimeConvFactors = new double[]
        {
            // keep in sync (*)

            1, // s -> s
            60, // m -> s
            3600 // h -> s            
        };

        internal static double[,] TimeConvMatrix
        {
            get
            {
                if (_TimeConvMatrix == null)
                {
                    _TimeConvMatrix = BuildConvMatrix(TimeConvFactors);
                }
                return _TimeConvMatrix;
            }
        }

        #endregion

    };

    public static partial class Ext
    {

        public static double LengthConvert(this double value, LengthMeasureUnit from, LengthMeasureUnit to)
        {
            return value * MUManager.LengthConvMatrix[(int)from, (int)to];
        }

        public static double TimeConvert(this double value, TimeMeasureUnit from, TimeMeasureUnit to)
        {
            return value * MUManager.TimeConvMatrix[(int)from, (int)to];
        }

    };

}
