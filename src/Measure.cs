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
using SearchAThing.Core;
using System.Runtime.Serialization;

namespace SearchAThing.Sci
{

    [DataContract(IsReference = true)]
    public class Measure
    {

        [DataMember]
        public double Value { get; private set; }
        
        [DataMember]
        public MeasureUnit MU { get; private set; }

        public Measure(double value, MeasureUnit mu)
        {
            Value = value;
            MU = mu;
        }

        /// <summary>
        /// Convert to the implicit measure of the given mu domain
        /// </summary>
        public Measure ConvertTo(MUDomain mud)
        {
            return ConvertTo(mud.ByPhysicalQuantity(MU.PhysicalQuantity).MU);
        }

        public Measure ConvertTo(MeasureUnit toMU)
        {
            if (MU.PhysicalQuantity.MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                return new Measure(MU.PhysicalQuantity.NonLinearConversionFunctor(MU, toMU, Value), toMU);
            else
                return new Measure(Value * MU.PhysicalQuantity.ConvertFactor(MU, toMU), toMU);
        }

        public override string ToString()
        {
            return $"{Value}{MU}";
        }

        public static Measure TryParse(PhysicalQuantity pq, string text)
        {
            if (pq.Equals(PQCollection.Adimensional))
            {
                double n;
                if (double.TryParse(text, out n)) return new Measure(n, MUCollection.Adimensional.adim);
            }
            else
            {
                var s = text.Trim();

                MeasureUnit mu = null;

                foreach (var _mu in pq.MeasureUnits)
                {
                    if (s.EndsWith(_mu.ToString()))
                    {
                        mu = _mu;
                        break;
                    }
                }

                if (mu == null) return null;

                s = s.StripEnd(mu.ToString());

                double n;
                if (double.TryParse(s, out n)) return new Measure(n, mu);
            }

            return null;
        }
    }

}
