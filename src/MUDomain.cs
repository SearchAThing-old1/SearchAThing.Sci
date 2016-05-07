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
using static System.Math;

namespace SearchAThing.Sci
{

    // TODO //*

    /// <summary>
    /// Measures here contains information about implicit measure unit
    /// and value of the tolerance
    /// </summary>
    public class MUDomain
    {

        /// <summary>
        /// Implicit measure unit for Length and its tolerance
        /// </summary>
        public Measure Length { get; private set; }

        /// <summary>
        /// Implicit measure unit for Mass and its tolerance
        /// </summary>
        public Measure Mass { get; private set; }

        /// <summary>
        /// Implicit measure unit for Time and its tolerance
        /// </summary>
        public Measure Time { get; private set; }

        /// <summary>
        /// Implicit measure unit for Temperature and its tolerance
        /// </summary>
        public Measure Temperature { get; private set; }

        /// <summary>
        /// Implicit measure unit for PlaneAngle and its tolerance
        /// </summary>
        public Measure PlaneAngle { get; private set; }

        /// <summary>
        /// Implicit measure unit for Acceleration and its tolerance
        /// </summary>
        public Measure Acceleration { get; private set; }

        /// <summary>
        /// Implicit measure unit for Force and its tolerance
        /// </summary>
        public Measure Force { get; private set; }

        /// <summary>
        /// Implicit measure unit for Speed and its tolerance
        /// </summary>
        public Measure Speed { get; private set; }

        public void SetLength(Measure length) { Length = length; }

        public MUDomain()
        {
            Length = new Measure(1e-1, MUCollection.Length.mm);

            PlaneAngle = new Measure(PI / 180.0 / 10.0, MUCollection.PlaneAngle.rad);

            //...
        }

        public Measure ByPhysicalQuantity(PhysicalQuantity physicalQuantity)
        {
            var id = physicalQuantity.id;

            if (Length.MU.PhysicalQuantity.id == id) return Length;
//*            else if (Mass.MU.PhysicalQuantity.id == id) return Mass;
//*            else if (Time.MU.PhysicalQuantity.id == id) return Time;
//*            else if (Temperature.MU.PhysicalQuantity.id == id) return Temperature;
            else if (PlaneAngle.MU.PhysicalQuantity.id == id) return PlaneAngle;
//*            else if (Acceleration.MU.PhysicalQuantity.id == id) return Acceleration;
//*            else if (Force.MU.PhysicalQuantity.id == id) return Force;
//*            else if (Speed.MU.PhysicalQuantity.id == id) return Speed;

            throw new NotImplementedException($"unable to find measure domain for given physical quantity {physicalQuantity}");
        }

    }

}
