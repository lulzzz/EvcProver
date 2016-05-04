﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionGas.MASA.Models
{
    public class EvcQARun
    {
        public DateTime DateTime { get; set; }
        public string InstrumentType { get; set; }
        public List<VerificationTest> VerificationTests { get; set; }

        //Pressure Info
        public string TransducerType { get; set; }
        public string PressureUnits { get; set; }
        public decimal EvcBasePressure { get; set; }
        public decimal EvcPressureRange { get; set; }

        //Temperature Info
        public string Range { get; set; }
        public string TemperatureUnits { get; set; }
        public decimal EvcBaseTemperature { get; set; }

        //Supercompress. Info
        public decimal SpecGr { get; set; }
        public decimal CO2 { get; set; }
        public decimal N2 { get; set; }
        public string SuperTable { get; set; }

        //Volume Info
        public string PulseASelect { get; set; }
        public string PulseBSelect { get; set; }
        public string DriveTypeDiscriminator { get; set; }

        public class VerificationTest
        {
            public virtual PressureDetail Pressure { get; set; }
            public virtual TemperatureDetail Temperature { get; set; }
            public virtual VolumeDetail VolumeTest { get; set; }
            public virtual SuperFactorDetail SuperFactorTest { get; set; }

            public class PressureDetail
            {
                public decimal GasPressure { get; set; }
                public decimal GaugePressure { get; set; }
                public decimal AtmosphericGauge { get; set; }
                public decimal ActualFactor { get; set; }

                public decimal EvcGasPressure { get; set; }
                public decimal EvcPressureFactor { get; set; }
                public decimal PercentError { get; set; }
            }

            public class TemperatureDetail
            {
                public decimal GaugeTemperature { get; set; }
                public decimal EvcTemperature { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal ActualFactor { get; set; }
                public decimal PercentError { get; set; }
            }

            public class VolumeDetail
            {
                public int PulseACount { get; set; }
                public int PulseBCount { get; set; }
                public decimal AppliedInput { get; set; }         
                public decimal UnCorrectedPercentError { get; set; }
                public decimal CorrectedPercentError { get; set; }
                public int UncPulseCount { get; set; }
                public int CorPulseCount { get; set; }
                public decimal TrueCorrected { get; set; }
                public decimal EvcCorrected { get; set; }
                public decimal EvcUncorrected { get; set; }

                public decimal Corrected { get; set; }
                public decimal Uncorrected { get; set; }

                public string DriveRateDescription { get; set; }
                public string CorrectedMultiplierDescription { get; set; }
                public string UnCorrectedMultiplierDescription { get; set; }
            }

            public class SuperFactorDetail
            {
                public decimal GaugeTemp { get; set; }
                public decimal GaugePressure { get; set; }

                public decimal EVCUnsqrFactor { get; set; }
                public decimal EvcFactor { get; set; }
                public decimal ActualFactor { get; set; }
                public decimal PercentError { get; set; }
            }
        }
    }
}
