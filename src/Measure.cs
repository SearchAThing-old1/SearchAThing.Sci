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
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System;
using System.Linq;

namespace SearchAThing.Sci
{

    [BsonIgnoreExtraElements]
    [DataContract(IsReference = true)]
    public class Measure
    {

        [BsonElement("Value")]
        public string BsonValue
        {
            get
            {
                return ToString(CultureInfo.InvariantCulture, true);
            }
            set
            {
                var measure = TryParse(value, null, CultureInfo.InvariantCulture);
                MU = measure.MU;
                Value = measure.Value;
            }
        }

        [BsonIgnore]
        [DataMember]
        public double Value { get; private set; }

        [BsonIgnore]
        [DataMember]
        public MeasureUnit MU { get; private set; }

        public Measure(double value, MeasureUnit mu)
        {
            Value = value;
            MU = mu;
        }

        #region operators

        /// <summary>
        /// scalar mul
        /// </summary>        
        public static Measure operator *(double s, Measure v)
        {
            return new Measure(v.Value * s, v.MU);
        }

        /// <summary>
        /// scalar mul
        /// </summary>        
        public static Measure operator *(Measure v, double s)
        {
            return new Measure(v.Value * s, v.MU);
        }

        #endregion

        /// <summary>
        /// Convert to the implicit measure of the given mu domain
        /// </summary>
        public Measure ConvertTo(IMUDomain mud)
        {
            if (MU == MUCollection.Adimensional.adim) return new Measure(Value, MU);

            return ConvertTo(mud.ByPhysicalQuantity(MU.PhysicalQuantity).MU);
        }

        public Measure ConvertTo(MeasureUnit toMU)
        {
            return new Measure(Convert(Value, MU, toMU), toMU);
        }

        /// <summary>
        /// convert given value from to measure units
        /// </summary>        
        public static double Convert(double value, MeasureUnit from, MeasureUnit to)
        {
            if (from.PhysicalQuantity.MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                return from.PhysicalQuantity.NonLinearConversionFunctor(from, to, value);
            else
                return from.PhysicalQuantity.ConvertFactor(from, to) * value;
        }

        /// <summary>
        /// convert given value from to measure units
        /// to measure unit is given from the correspondent physical quantity measure unit in the given domain
        /// </summary>        
        public static double Convert(double value, MeasureUnit from, IMUDomain to)
        {
            if (from == MUCollection.Adimensional.adim) return value;

            return value.Convert(from, to.ByPhysicalQuantity(from.PhysicalQuantity).MU);
        }

        /// <summary>
        /// convert given value from to measure units
        /// from measure unit is given from the correspondent physical quantity measure unit in the given domain
        /// </summary>        
        public static double Convert(double value, IMUDomain from, MeasureUnit to)
        {
            if (from == MUCollection.Adimensional.adim) return value;

            return value.Convert(from.ByPhysicalQuantity(to.PhysicalQuantity).MU, to);
        }

        public string ToString(CultureInfo culture, bool includePQ = false)
        {
            var res = string.Format(culture, "{0}{1}", Value, MU);

            if (includePQ) res += $" [{MU.PhysicalQuantity}]";

            return res;
        }

        public override string ToString()
        {
            return $"{Value}{MU}";
        }

        public static Measure TryParse(string text, PhysicalQuantity pq = null, CultureInfo culture = null)
        {
            if (culture == null) culture = CultureInfo.InvariantCulture;

            if (pq == null)
            {
                var pqstart = text.LastIndexOf('[') + 1;
                if (pqstart == 0) return null;

                var pqname = text.Substring(pqstart, text.Length - pqstart - 1);
                pq = PQCollection.PhysicalQuantities.First(w => w.Name == pqname);

                text = text.Substring(0, pqstart - 1);
            }

            if (pq.Equals(PQCollection.Adimensional))
            {
                double n;
                if (double.TryParse(text, NumberStyles.Number, culture, out n)) return new Measure(n, MUCollection.Adimensional.adim);
            }
            else
            {
                var s = text.Trim();

                MeasureUnit mu = null;

                foreach (var _mu in pq.MeasureUnits.OrderByDescending(w => w.Name.Length))
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
                if (double.TryParse(s, NumberStyles.Number, culture, out n)) return new Measure(n, mu);
            }

            return null;
        }
    }

}
