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
using System.Collections;
using System.Linq;
using static System.Math;

namespace SearchAThing.Sci
{

    public static class MUCollection
    {

        //-------------------------------------------------------------------
        // Measure Units
        //-------------------------------------------------------------------

        public static class Adimensional
        {
            public static readonly MeasureUnit adim = new MeasureUnit(PQCollection.Adimensional, "adim");
        }

        #region Length

        public static class Length
        {
            public static readonly MeasureUnit mm = new MeasureUnit(PQCollection.Length, "mm");
            public static readonly MeasureUnit cm = new MeasureUnit(PQCollection.Length, "cm", mm, 1e1);
            public static readonly MeasureUnit m = new MeasureUnit(PQCollection.Length, "m", mm, 1e3);
            public static readonly MeasureUnit km = new MeasureUnit(PQCollection.Length, "km", m, 1e3);
        }

        #endregion

        #region Mass

        public static class Mass
        {
            public static readonly MeasureUnit g = new MeasureUnit(PQCollection.Mass, "g");
            public static readonly MeasureUnit kg = new MeasureUnit(PQCollection.Mass, "kg", g, 1e3);
            public static readonly MeasureUnit T = new MeasureUnit(PQCollection.Mass, "T", kg, 1e3);
        }

        #endregion

        #region Time

        public static class Time
        {
            public static readonly MeasureUnit sec = new MeasureUnit(PQCollection.Time, "sec");
            public static readonly MeasureUnit min = new MeasureUnit(PQCollection.Time, "min", sec, 60);
            public static readonly MeasureUnit hr = new MeasureUnit(PQCollection.Time, "hr", min, 60);
        }

        #endregion

        #region Temperature

        public static class Temperature
        {
            public static readonly MeasureUnit C = new MeasureUnit(PQCollection.Temperature, "C", NonLinearConvFunctor);
            public static readonly MeasureUnit K = new MeasureUnit(PQCollection.Temperature, "K", NonLinearConvFunctor);
            public static readonly MeasureUnit F = new MeasureUnit(PQCollection.Temperature, "F", NonLinearConvFunctor);

            static Func<MeasureUnit, MeasureUnit, double, double> _nonLinearConvFunctor;
            public static Func<MeasureUnit, MeasureUnit, double, double> NonLinearConvFunctor
            {
                get
                {
                    if (_nonLinearConvFunctor == null)
                    {
                        _nonLinearConvFunctor = (muFrom, muTo, valueFrom) =>
                        {
                            if (muFrom == C)
                            {
                                if (muTo == K) return valueFrom + 273.15;
                                if (muTo == F) return valueFrom * (9.0 / 5) + 32;
                            }
                            else if (muFrom == K)
                            {
                                if (muTo == C) return valueFrom - 273.15;
                                if (muTo == F) return valueFrom * (9.0 / 5) - 459.67;
                            }
                            else if (muFrom == F)
                            {
                                if (muTo == C) return (valueFrom - 32) * (5.0 / 9);
                                if (muTo == K) return (valueFrom + 459.67) * (5.0 / 9);
                            }

                            throw new NotImplementedException($"not yet implemented non linear conversion from [{muFrom}] to [{muTo}]");
                        };
                    }
                    return _nonLinearConvFunctor;
                }
            }

        }

        #endregion

        #region PlaneAngle

        public static class PlaneAngle
        {
            public static readonly MeasureUnit rad = new MeasureUnit(PQCollection.PlaneAngle, "rad");
            public static readonly MeasureUnit grad = new MeasureUnit(PQCollection.PlaneAngle, "grad", rad, PI / 180.0);
        }

        #endregion

        #region Pressure

        public static class Pressure
        {
            public static readonly MeasureUnit Pa = new MeasureUnit(PQCollection.Pressure, "Pa");
            public static readonly MeasureUnit kPa = new MeasureUnit(PQCollection.Pressure, "kPa", Pa, 1e3);
            public static readonly MeasureUnit MPa = new MeasureUnit(PQCollection.Pressure, "MPa", kPa, 1e3);
            public static readonly MeasureUnit GPa = new MeasureUnit(PQCollection.Pressure, "GPa", MPa, 1e3);

            public static MeasureUnit Auto(MeasureUnit force, MeasureUnit length)
            {
                #region force=[N]
                if (force.Equals(Force.N))
                {
                    if (length.Equals(Length.m)) return Pa;
                }
                #endregion

                #region force=[kN]
                else if (force.Equals(Force.kN))
                {
                    if (length.Equals(Length.m)) return Pressure.kPa;
                }
                #endregion

                throw new NotImplementedException($"pressure mu automatic not defined for input force=[{force.Name}] and length=[{length.Name}]");
            }
        }

        #endregion

        #region Acceleration

        public static class Acceleration
        {
            public static readonly MeasureUnit m_s2 = new MeasureUnit(PQCollection.Acceleration, "m_s2");
        }

        #endregion

        #region Force

        public static class Force
        {
            public static readonly MeasureUnit N = new MeasureUnit(PQCollection.Force, "N");
            public static readonly MeasureUnit kN = new MeasureUnit(PQCollection.Force, "kN", N, 1e3);
        }

        #endregion

        #region Speed

        public static class Speed
        {
            public static readonly MeasureUnit m_s = new MeasureUnit(PQCollection.Speed, "m_s");
        }

        #endregion

    }

}
