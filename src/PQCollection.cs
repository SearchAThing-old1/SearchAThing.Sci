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

using System.Collections.Generic;
using System.Linq;

namespace SearchAThing.Sci
{

    public static class PQCollection
    {

        //-------------------------------------------------------------------
        // Physical Quantities
        //-------------------------------------------------------------------

        static List<PhysicalQuantity> physicalQuantities;
        public static IEnumerable<PhysicalQuantity> PhysicalQuantities
        {
            get
            {
                if (physicalQuantities == null)
                {
                    physicalQuantities = new List<PhysicalQuantity>();

                    physicalQuantities.Add(Length);
                    physicalQuantities.Add(Mass);
                    physicalQuantities.Add(Time);
                    physicalQuantities.Add(Temperature);

                    physicalQuantities.Add(PlaneAngle);
                    physicalQuantities.Add(Pressure);
                    physicalQuantities.Add(Acceleration);
                    physicalQuantities.Add(Force);
                    physicalQuantities.Add(Speed);
                }
                return physicalQuantities;
            }
        }

        //-------------------------------------------------------------------
        // Base quantity
        //-------------------------------------------------------------------

        #region Base quantity        

        public static readonly PhysicalQuantity Adimensional = new PhysicalQuantity("Adimensional", typeof(MUCollection.Adimensional));

        // https://en.wikipedia.org/wiki/List_of_physical_quantities

        public static readonly PhysicalQuantity Length = new PhysicalQuantity("Length", typeof(MUCollection.Length));

        public static readonly PhysicalQuantity Mass = new PhysicalQuantity("Mass", typeof(MUCollection.Mass));

        public static readonly PhysicalQuantity Time = new PhysicalQuantity("Time", typeof(MUCollection.Time));

        /*
        static PhysicalQuantity electricCurrent;
        public static PhysicalQuantity ElectricCurrent
        {
            get
            {
                if (electricCurrent == null) electricCurrent = new PhysicalQuantity("ElectricCurrent");

                return electricCurrent;
            }
        }
        */

        public static readonly PhysicalQuantity Temperature = new PhysicalQuantity("Temperature",
            typeof(MUCollection.Temperature), MeasureUnitConversionTypeEnum.NonLinear);

        // amountOfSubstance

        // luminousIntensity        

        #endregion

        //-------------------------------------------------------------------
        // Derived quantity
        //-------------------------------------------------------------------

        #region Derived quantity

        public static readonly PhysicalQuantity PlaneAngle = new PhysicalQuantity("PlaneAngle", typeof(MUCollection.PlaneAngle));

        // solidAngle

        // absorbedDoseRate

        public static readonly PhysicalQuantity Acceleration = new PhysicalQuantity("Acceleration", typeof(MUCollection.Acceleration));

        // angularAcceleration

        /*
        static PhysicalQuantity angularSpeed;
        public static PhysicalQuantity AngularSpeed
        {
            get
            {
                if (angularSpeed == null) angularSpeed = new PhysicalQuantity("AngularSpeed");

                return angularSpeed;
            }
        }
        */

        // angularMomentum

        // area

        // areaDensity

        // capacitance

        // catalyticActivity

        // catalyticActivityConcentration

        // checmicalPotential

        // molarConcentration

        // crackle

        // currentDensity

        // doseEquivalent

        // dynamicViscosity

        // electricCharge

        // electricChargeDensity

        // electricDisplacement

        // electricFieldStrength

        // electricalConductance

        // electicPotential

        // electicalResistance

        /*
        static PhysicalQuantity energy;
        public static PhysicalQuantity Energy
        {
            get
            {
                if (energy == null) energy = new PhysicalQuantity("Energy");

                return energy;
            }
        }
        */

        // energyDensity

        // entropy

        public static readonly PhysicalQuantity Force = new PhysicalQuantity("Force", typeof(MUCollection.Force));

        // fuelEfficiency

        // impulse

        /*
        static PhysicalQuantity frequency;
        public static PhysicalQuantity Frequency
        {
            get
            {
                if (frequency == null) frequency = new PhysicalQuantity("Frequency");

                return frequency;
            }
        }
        */

        // halfLite

        // heat

        // heatCapacity

        // heatFluxDensity

        // illuminance

        // impedance

        // indexOfRefraction

        // inductance

        // irradiance

        // indensity

        // jerk

        // jounce

        /*
        static PhysicalQuantity linearDensity;
        public static PhysicalQuantity LinearDensity
        {
            get
            {
                if (linearDensity == null) linearDensity = new PhysicalQuantity("LinearDensity");

                return linearDensity;
            }
        }
        */

        // luminous flux

        // machNumber

        // magneticFieldStrength

        // magneticFlux

        // magneticFluxDensity

        // magnetization

        // massFraction

        /*
        static PhysicalQuantity massDensity;
        public static PhysicalQuantity MassDensity
        {
            get
            {
                if (massDensity == null) massDensity = new PhysicalQuantity("MassDensity");

                return massDensity;
            }
        }
        */

        // meanLifetime

        // molarEnergy

        // molarHeatCapacity

        /*
        static PhysicalQuantity momentOfIntertia;
        public static PhysicalQuantity MomentOfInertia
        {
            get
            {
                if (momentOfIntertia == null) momentOfIntertia = new PhysicalQuantity("MomentOfInertia");

                return momentOfIntertia;
            }
        }
        */

        /*
        static PhysicalQuantity momentum;
        public static PhysicalQuantity Momentum
        {
            get
            {
                if (momentum == null) momentum = new PhysicalQuantity("Momentum");

                return momentum;
            }
        }
        */

        // permeability

        // permittivity

        /*
        static PhysicalQuantity power;
        public static PhysicalQuantity Power
        {
            get
            {
                if (power == null) power = new PhysicalQuantity("Power");

                return power;
            }
        }
        */

        public static readonly PhysicalQuantity Pressure = new PhysicalQuantity("Pressure", typeof(MUCollection.Pressure));

        // pop

        // radioActivity

        // radioDose

        // radiance

        // radiantIntensity

        // reactionRrate

        // refractiveIndex

        public static readonly PhysicalQuantity Speed = new PhysicalQuantity("Speed", typeof(MUCollection.Speed));

        // specificEnergy

        // specificHeatCapacity

        // specificVolume

        // spin

        // strain        
        //Spublic static readonly PhysicalQuantity Strain = new PhysicalQuantity("Strain");

        /*
        static PhysicalQuantity stress;
        public static PhysicalQuantity Stress
        {
            get
            {
                if (stress == null) stress = new PhysicalQuantity("Stress");

                return stress;
            }
        }
        */

        // surfaceTension

        // thermalConductivity

        // torque

        // velocity

        /*
        static PhysicalQuantity volume;
        public static PhysicalQuantity Volume
        {
            get
            {
                if (volume == null) volume = new PhysicalQuantity("Volume");

                return volume;
            }
        }
        */

        // waveLength

        // waveNumber

        /*
        static PhysicalQuantity weight;
        public static PhysicalQuantity Weight
        {
            get
            {
                if (weight == null) weight = new PhysicalQuantity("Weight");

                return weight;
            }
        }
        */

        // work

        // youngModulus
        #endregion

    }

}
