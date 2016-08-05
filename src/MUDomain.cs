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

using MongoDB.Bson.Serialization.Attributes;
using SearchAThing.Sci;
using System;
using System.Runtime.Serialization;
using static System.Math;

namespace SearchAThing.Sci
{

    public class MeasureUnitWithDefaultTolerance
    {

        public double DefaultTolerance { get; private set; }

        [BsonElement("MU")]
        public string MUName { get { return MU.ToString(); } }

        [BsonIgnore]
        public MeasureUnit MU { get; private set; }

        public MeasureUnitWithDefaultTolerance(double _DefaultTolerance, MeasureUnit _MU)
        {
            MU = _MU;
            DefaultTolerance = _DefaultTolerance;
        }

        public MeasureUnitWithDefaultTolerance ConvertTo(MeasureUnit toMU)
        {
            if (MU.PhysicalQuantity.MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                return new MeasureUnitWithDefaultTolerance(MU.PhysicalQuantity.NonLinearConversionFunctor(MU, toMU, DefaultTolerance), toMU);
            else
                return new MeasureUnitWithDefaultTolerance(DefaultTolerance * MU.PhysicalQuantity.ConvertFactor(MU, toMU), toMU);
        }

    }

    public interface IMUDomain
    {

        MeasureUnitWithDefaultTolerance Length { get; set; }
        MeasureUnitWithDefaultTolerance Mass { get; set; }
        MeasureUnitWithDefaultTolerance Time { get; set; }
        MeasureUnitWithDefaultTolerance Temperature { get; set; }
        MeasureUnitWithDefaultTolerance PlaneAngle { get; set; }
        MeasureUnitWithDefaultTolerance Pressure { get; set; }
        MeasureUnitWithDefaultTolerance Acceleration { get; set; }
        MeasureUnitWithDefaultTolerance Force { get; set; }
        MeasureUnitWithDefaultTolerance Speed { get; set; }

    }

    /// <summary>
    /// Measures here contains information about implicit measure unit
    /// and value of the tolerance.
    /// 
    /// Note that all measure must be dimensionally equivalent.
    /// For example:
    /// [length] = m
    /// [length2] = [length] * [length] = m2
    /// [time] = s
    /// [time2] = [time] * [time] = s2
    /// [speed] = [length] / [time] = m/s
    /// [acceleration] = [length] / [time2] = m/s2
    /// [mass] = kg
    /// [force] = [mass] * [acceleration] = kg * m/s2 = N
    /// [pressure] = [force] / [length2] = N / m2 = Pa
    /// 
    /// This will ensure measure comparision without further conversion, for example
    /// m1 = 1 [kg]
    /// a1 = 2 [m/s2]
    /// f1 = 4 [N]
    /// 
    /// test = m1 * a1 > f1
    /// </summary>
    [DataContract(IsReference = true)]
    public class MUDomain : IMUDomain
    {

        /// <summary>
        /// Implicit measure unit for Length and its tolerance
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Length { get; set; }

        /// <summary>
        /// Implicit measure unit for Mass and its tolerance
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Mass { get; set; }

        /// <summary>
        /// Implicit measure unit for Time and its tolerance
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Time { get; set; }

        /// <summary>
        /// Implicit measure unit for Temperature and its tolerance
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Temperature { get; set; }

        /// <summary>
        /// Implicit measure unit for PlaneAngle and its tolerance
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance PlaneAngle { get; set; }

        /// <summary>
        /// Implicit measure unit for Pressure and its tolerance
        ///     [pressure] = [force] / [length]^2
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Pressure { get; set; }

        /// <summary>
        /// Implicit measure unit for Acceleration and its tolerance
        ///     [acceleration] = [length] / [time]^2
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Acceleration { get; set; }

        /// <summary>
        /// Implicit measure unit for Force and its tolerance
        ///     [force] = [mass] * ( [length] / [time]^2 )
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Force { get; set; }

        /// <summary>
        /// Implicit measure unit for Speed and its tolerance
        ///     [speed] = [length] / [time]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Speed { get; set; }

        public MUDomain()
        {
            Length = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Length.m);
            Mass = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Mass.kg);
            Time = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Time.sec);
            Temperature = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Temperature.C);
            Force = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Force.N);
            PlaneAngle = new MeasureUnitWithDefaultTolerance(PI / 180.0 / 10.0, MUCollection.PlaneAngle.rad);
            Pressure = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Pressure.Pa);
            Acceleration = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Acceleration.m_s2);
            Speed = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Speed.m_s);
        }
    }

    public static partial class Extensions
    {

        public static MeasureUnitWithDefaultTolerance ByPhysicalQuantity(this IMUDomain mud, PhysicalQuantity physicalQuantity)
        {
            var id = physicalQuantity.id;

            if (mud.Length.MU.PhysicalQuantity.id == id) return mud.Length;
            else if (mud.Mass.MU.PhysicalQuantity.id == id) return mud.Mass;
            else if (mud.Time.MU.PhysicalQuantity.id == id) return mud.Time;
            else if (mud.Temperature.MU.PhysicalQuantity.id == id) return mud.Temperature;
            else if (mud.PlaneAngle.MU.PhysicalQuantity.id == id) return mud.PlaneAngle;
            else if (mud.Pressure.MU.PhysicalQuantity.id == id) return mud.Pressure;
            else if (mud.Acceleration.MU.PhysicalQuantity.id == id) return mud.Acceleration;
            else if (mud.Force.MU.PhysicalQuantity.id == id) return mud.Force;
            else if (mud.Speed.MU.PhysicalQuantity.id == id) return mud.Speed;

            throw new NotImplementedException($"unable to find measure domain for given physical quantity {physicalQuantity}");
        }

    }

}
