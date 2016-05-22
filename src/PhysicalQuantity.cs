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
using System.Linq;
using System.Runtime.Serialization;

namespace SearchAThing.Sci
{

    public enum MeasureUnitConversionTypeEnum
    {
        Linear,
        NonLinear
    };

    [DataContract(IsReference = true)]
    public class PhysicalQuantity : IEquatable<PhysicalQuantity>
    {

        static int global_static_id_counter;

        [DataMember]
        internal int id;

        public MeasureUnitConversionTypeEnum MUConversionType { get; private set; }

        /// <summary>
        /// conversion factor to the ref unit
        /// </summary>
        [DataMember]
        List<double> linearConvFactors = new List<double>();

        double[,] conversionMatrix = null;

        [DataMember]
        public MeasureUnit LinearConversionRefMU { get; private set; }

        internal Func<MeasureUnit, MeasureUnit, double, double> NonLinearConversionFunctor { get; private set; }

        [DataMember]
        List<MeasureUnit> measureUnits;
        public IEnumerable<MeasureUnit> MeasureUnits { get { return measureUnits; } }

        [DataMember]
        public string Name { get; private set; }

        public PhysicalQuantity(string name, MeasureUnitConversionTypeEnum muConversionType = MeasureUnitConversionTypeEnum.Linear)
        {
            id = global_static_id_counter++;

            MUConversionType = muConversionType;

            Name = name;

            measureUnits = new List<MeasureUnit>();
        }

        internal void RegisterMeasureUnit(MeasureUnit mu, MeasureUnit convRefUnit = null, double convRefFactor = 0)
        {
            if (MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                throw new Exception($"MeasureUnit [{mu.Name}] need a non linear conversion rule");

            if (measureUnits.Any(w => w.Name == mu.Name))
                throw new Exception($"MeasureUnit [{mu.Name}] already registered");

            if (LinearConversionRefMU == null)
            {
                LinearConversionRefMU = mu;
                linearConvFactors.Add(1.0); // ref unit to itself
                if (mu.id != 0) throw new Exception("internal error");
            }
            else
            {
                linearConvFactors.Add(linearConvFactors[convRefUnit.id] * convRefFactor);
            }

            measureUnits.Add(mu);
            conversionMatrix = null;
        }

        internal void RegisterMeasureUnit(MeasureUnit mu, Func<MeasureUnit, MeasureUnit, double, double> convRefFunctor)
        {
            if (MUConversionType == MeasureUnitConversionTypeEnum.Linear)
                throw new Exception($"MeasureUnit [{mu.Name}] need a linear conversion factor");

            if (measureUnits.Any(w => w.Name == mu.Name))
                throw new Exception($"MeasureUnit [{mu.Name}] already registered");

            measureUnits.Add(mu);
            conversionMatrix = null;
            NonLinearConversionFunctor = convRefFunctor;
        }

        public double ConvertFactor(MeasureUnit from, MeasureUnit to)
        {
            if (from.PhysicalQuantity.id != this.id || from.PhysicalQuantity.id != to.PhysicalQuantity.id)
                throw new Exception($"MeasureUnit physical quantity doesn't match");

            if (MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                throw new Exception($"invalid usage of convert factor for non linear mu");

            if (conversionMatrix == null) RebuildConversionMatrix();

            return conversionMatrix[from.id, to.id];
        }

        private void RebuildNonlinearConversionMatrix()
        {

        }

        private void RebuildConversionMatrix()
        {
            var mus = measureUnits.Where(r => r.PhysicalQuantity.id == id).ToList();

            conversionMatrix = new double[mus.Count, mus.Count];
            var m = conversionMatrix;

            // https://searchathing.com/?p=1326#MeasureUnitConversionMatrixStructure

            // fill first column
            for (int r = 1; r < mus.Count; ++r) m[r, 0] = linearConvFactors[r];

            // fill diag
            for (int r = 0; r < mus.Count; ++r) m[r, r] = 1;

            // fill lower triangle
            for (int c = 1; c < mus.Count - 1; ++c)
            {
                for (int r = c; r < mus.Count; ++r)
                {
                    m[r, c] = m[r, 0] / m[c, 0];
                }
            }

            // fill upper triangle
            for (int c = 1; c < linearConvFactors.Count; ++c)
            {
                for (int r = 0; r < c; ++r)
                {
                    m[r, c] = 1.0 / m[c, r];
                }
            }
        }

        public bool Equals(PhysicalQuantity other)
        {
            return id == other.id;
        }

        public override string ToString()
        {
            return Name;
        }
    };

}