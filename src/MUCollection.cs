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

using SearchAThing.Core;
using System;
using System.Collections.Generic;
using static SearchAThing.Sci.PQCollection;

namespace SearchAThing.Sci
{

    public static class MUCollection
    {

        //-------------------------------------------------------------------
        // Measure Units
        //-------------------------------------------------------------------

        public static class Adimensional
        {
            static MeasureUnit _adimensional;
            public static MeasureUnit adim
            {
                get
                {
                    if (_adimensional == null)
                    {
                        _adimensional = new MeasureUnit(PQCollection.Adimensional, "adim");
                    }
                    return _adimensional;
                }
            }
        }

        #region Length

        public static class Length
        {

            static MeasureUnit _mm;
            public static MeasureUnit mm
            {
                get
                {
                    if (_mm == null)
                    {
                        _mm = new MeasureUnit(PQCollection.Length, "mm");
                    }
                    return _mm;
                }
            }

            static MeasureUnit _m;
            public static MeasureUnit m
            {
                get
                {
                    if (_m == null)
                    {
                        _m = new MeasureUnit(PQCollection.Length, "m", mm, 1e3);
                    }
                    return _m;
                }
            }

            static MeasureUnit _km;
            public static MeasureUnit km
            {
                get
                {
                    if (_km == null)
                    {
                        _km = new MeasureUnit(PQCollection.Length, "km", m, 1e3);
                    }
                    return _km;
                }
            }

        }

        #endregion

        #region Mass

        public static class Mass
        {

            static MeasureUnit _g;
            public static MeasureUnit g
            {
                get
                {
                    if (_g == null)
                    {
                        _g = new MeasureUnit(PQCollection.Mass, "g");
                    }
                    return _g;
                }
            }

            static MeasureUnit _kg;
            public static MeasureUnit kg
            {
                get
                {
                    if (_kg == null)
                    {
                        _kg = new MeasureUnit(PQCollection.Mass, "kg", g, 1e3);
                    }
                    return _kg;
                }
            }

        }

        #endregion

        #region Time

        public static class Time
        {

            static MeasureUnit _sec;
            public static MeasureUnit sec
            {
                get
                {
                    if (_sec == null)
                    {
                        _sec = new MeasureUnit(PQCollection.Time, "sec");
                    }
                    return _sec;
                }
            }

            static MeasureUnit _min;
            public static MeasureUnit min
            {
                get
                {
                    if (_min == null)
                    {
                        _min = new MeasureUnit(PQCollection.Time, "min", sec, 60);
                    }
                    return _min;
                }
            }

            static MeasureUnit _hr;
            public static MeasureUnit hr
            {
                get
                {
                    if (_hr == null)
                    {
                        _hr = new MeasureUnit(PQCollection.Time, "hr", min, 60);
                    }
                    return _hr;
                }
            }

        }

        #endregion

        #region Temperature

        public static class Temperature
        {

            static MeasureUnit _C;
            public static MeasureUnit C
            {
                get
                {
                    if (_C == null)
                    {
                        _C = new MeasureUnit(PQCollection.Temperature, "C");
                    }
                    return _C;
                }
            } 

        }

        #endregion

        #region PlaneAngle

        public static class PlaneAngle
        {

            static MeasureUnit _rad;
            public static MeasureUnit rad
            {
                get
                {
                    if (_rad == null)
                    {
                        _rad = new MeasureUnit(PQCollection.PlaneAngle, "rad");
                    }
                    return _rad;
                }
            }

        }

        #endregion

        #region Pressure

        public static class Pressure
        {

            static MeasureUnit _Pa;
            public static MeasureUnit Pa
            {
                get
                {
                    if (_Pa == null)
                    {
                        _Pa = new MeasureUnit(PQCollection.Pressure, "Pa");
                    }
                    return _Pa;
                }
            }

        }

        #endregion

        #region Acceleration

        public static class Acceleration
        {

            static MeasureUnit _m_s2;
            public static MeasureUnit m_s2
            {
                get
                {
                    if (_m_s2 == null)
                    {
                        _m_s2 = new MeasureUnit(PQCollection.Acceleration, "m_s2");
                    }
                    return _m_s2;
                }
            }

        }

        #endregion

        #region Force

        public static class Force
        {

            static MeasureUnit _N;
            public static MeasureUnit N
            {
                get
                {
                    if (_N == null)
                    {
                        _N = new MeasureUnit(PQCollection.Force, "N");
                    }
                    return _N;
                }
            }

        }

        #endregion

        #region Speed

        public static class Speed
        {

            static MeasureUnit _m_s;
            public static MeasureUnit m_s
            {
                get
                {
                    if (_m_s == null)
                    {
                        _m_s = new MeasureUnit(PQCollection.Speed, "m_s");
                    }
                    return _m_s;
                }
            }

        }

        #endregion

    }

}
