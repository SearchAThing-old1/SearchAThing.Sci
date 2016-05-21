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

        static PhysicalQuantity adimensional;
        public static PhysicalQuantity Adimensional
        {
            get
            {
                if (adimensional == null) adimensional = new PhysicalQuantity("Adimensional");

                return adimensional;
            }
        }

        // https://en.wikipedia.org/wiki/List_of_physical_quantities

        static PhysicalQuantity length;
        public static PhysicalQuantity Length
        {
            get
            {
                if (length == null) length = new PhysicalQuantity("Length");

                return length;
            }
        }

        static PhysicalQuantity mass;
        public static PhysicalQuantity Mass
        {
            get
            {
                if (mass == null) mass = new PhysicalQuantity("Mass");

                return mass;
            }
        }

        static PhysicalQuantity time;
        public static PhysicalQuantity Time
        {
            get
            {
                if (time == null) time = new PhysicalQuantity("Time");

                return time;
            }
        }

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

        static PhysicalQuantity temperature;
        public static PhysicalQuantity Temperature
        {
            get
            {
                if (temperature == null) temperature = new PhysicalQuantity("Temperature");

                return temperature;
            }
        }

        // amountOfSubstance

        // luminousIntensity        

        #endregion

        //-------------------------------------------------------------------
        // Derived quantity
        //-------------------------------------------------------------------

        #region Derived quantity

        static PhysicalQuantity planeAngle;
        public static PhysicalQuantity PlaneAngle
        {
            get
            {
                if (planeAngle == null) planeAngle = new PhysicalQuantity("PlaneAngle");

                return planeAngle;
            }
        }

        // solidAngle

        // absorbedDoseRate

        static PhysicalQuantity acceleration;
        public static PhysicalQuantity Acceleration
        {
            get
            {
                if (acceleration == null) acceleration = new PhysicalQuantity("Acceleration");

                return acceleration;
            }
        }

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

        static PhysicalQuantity force;
        public static PhysicalQuantity Force
        {
            get
            {
                if (force == null) force = new PhysicalQuantity("Force");

                return force;
            }
        }

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

        static PhysicalQuantity pressure;
        public static PhysicalQuantity Pressure
        {
            get
            {
                if (pressure == null) pressure = new PhysicalQuantity("Pressure");

                return pressure;
            }
        }

        // pop

        // radioActivity

        // radioDose

        // radiance

        // radiantIntensity

        // reactionRrate

        // refractiveIndex

        static PhysicalQuantity speed;
        public static PhysicalQuantity Speed
        {
            get
            {
                if (speed == null) speed = new PhysicalQuantity("Speed");

                return speed;
            }
        }

        // specificEnergy

        // specificHeatCapacity

        // specificVolume

        // spin

        // strain
        static PhysicalQuantity strain;
        public static PhysicalQuantity Strain
        {
            get
            {
                if (strain == null) strain = new PhysicalQuantity("Strain");

                return strain;
            }
        }

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
