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

using SearchAThing.Sci;
using System;
using System.Runtime.Serialization;
using static System.Math;

namespace SearchAThing.Sci
{

    public interface IMUDomain
    {
        
        Measure Length { get; }        
        Measure Mass { get; }        
        Measure Time { get; }        
        Measure Temperature { get; }        
        Measure PlaneAngle { get; }        
        Measure Pressure { get; }       
        Measure Acceleration { get; }        
        Measure Force { get; }        
        Measure Speed { get; }

    }

    /// <summary>
    /// Measures here contains information about implicit measure unit
    /// and value of the tolerance
    /// </summary>
    [DataContract(IsReference = true)]
    public class MUDomain : IMUDomain
    {

        /// <summary>
        /// Implicit measure unit for Length and its tolerance
        /// </summary>
        [DataMember]
        public Measure Length { get; private set; }

        /// <summary>
        /// Implicit measure unit for Mass and its tolerance
        /// </summary>
        [DataMember]
        public Measure Mass { get; private set; }

        /// <summary>
        /// Implicit measure unit for Time and its tolerance
        /// </summary>
        [DataMember]
        public Measure Time { get; private set; }

        /// <summary>
        /// Implicit measure unit for Temperature and its tolerance
        /// </summary>
        [DataMember]
        public Measure Temperature { get; private set; }

        /// <summary>
        /// Implicit measure unit for PlaneAngle and its tolerance
        /// </summary>
        [DataMember]
        public Measure PlaneAngle { get; private set; }

        /// <summary>
        /// Implicit measure unit for Pressure and its tolerance
        /// </summary>
        [DataMember]
        public Measure Pressure { get; private set; }

        /// <summary>
        /// Implicit measure unit for Acceleration and its tolerance
        /// </summary>
        [DataMember]
        public Measure Acceleration { get; private set; }

        /// <summary>
        /// Implicit measure unit for Force and its tolerance
        /// </summary>
        [DataMember]
        public Measure Force { get; private set; }

        /// <summary>
        /// Implicit measure unit for Speed and its tolerance
        /// </summary>
        [DataMember]
        public Measure Speed { get; private set; }

        public void SetLength(Measure length) { Length = length; }

        public MUDomain()
        {
            Length = new Measure(1e-4, MUCollection.Length.m);
            Mass = new Measure(1e-4, MUCollection.Mass.kg);
            Time = new Measure(1e-1, MUCollection.Time.sec);
            Temperature = new Measure(1e-1, MUCollection.Temperature.C);
            Force = new Measure(1e-1, MUCollection.Force.N);
            PlaneAngle = new Measure(PI / 180.0 / 10.0, MUCollection.PlaneAngle.rad);

            // implicit            
            Pressure = new Measure(1e-1, MUCollection.Pressure.Pa);
            Acceleration = new Measure(1e-1, MUCollection.Acceleration.m_s2);            
            Speed = new Measure(1e-1, MUCollection.Speed.m_s);
        }
    }

    public static partial class Extensions
    {         

        public static Measure ByPhysicalQuantity(this IMUDomain mud, PhysicalQuantity physicalQuantity)
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
