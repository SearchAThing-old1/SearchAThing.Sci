﻿#region SearchAThing.Sci, Copyright(C) 2016 Lorenzo Delana, License under MIT
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

namespace SearchAThing.Sci
{

    public class MeasureUnit
    {
        /// <summary>
        /// all measure units for any physical quantity
        /// this list is used to avoid double registration of a measure unit with same name
        /// for a given physical quantity
        /// </summary>
        static List<MeasureUnit> AllMeasureUnits = new List<MeasureUnit>();

        static Dictionary<int, int> global_static_id_counter = new Dictionary<int, int>();

        internal int id;

        public string Name { get; private set; }

        public PhysicalQuantity PhysicalQuantity { get; private set; }

        public MeasureUnit(PhysicalQuantity physicalQuantity, string name, MeasureUnit convRefUnit = null, double convRefFactor = 0)
        {
            PhysicalQuantity = physicalQuantity;

            if (AllMeasureUnits
                .Where(r => r.PhysicalQuantity.id == physicalQuantity.id)
                .Any(r => r.Name == name))
                throw new Exception($"A registered measure unit [{name}] already exists for the physical quantity [{physicalQuantity.Name}]");

            if (convRefUnit == null && physicalQuantity.ReferenceMeasureUnit != null)
                throw new Exception(
                    $"A reference measure unit [{physicalQuantity.ReferenceMeasureUnit}] already exists for the physical quantity [{physicalQuantity.Name}]" +
                    $"Need to specify a valid existing convRefUnit with related convRefFactor to specify measure unit scale factor");

            if (global_static_id_counter.ContainsKey(physicalQuantity.id))
                id = ++global_static_id_counter[physicalQuantity.id];
            else
                global_static_id_counter.Add(physicalQuantity.id, id = 0);

            Name = name;
            PhysicalQuantity = PhysicalQuantity;

            physicalQuantity.RegisterMeasureUnit(this, convRefUnit, convRefFactor);
        }

        /// <summary>
        /// Builds a Measure object of value * given mu
        /// </summary>        
        public static Measure operator *(double value, MeasureUnit mu)
        {
            return new Measure(value, mu);
        }

    };

}
