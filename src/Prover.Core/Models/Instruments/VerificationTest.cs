﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prover.Core.Models.Instruments
{
    public sealed class VerificationTest : BaseEntity
    {
        public VerificationTest()
        {
        }

        public VerificationTest(int testNumber, Instrument instrument, bool hasVolumeTest = false)
        {
            TestNumber = testNumber;
            Instrument = instrument;
            InstrumentId = Instrument.Id;
        }

        public VerificationTest(int testNumber, Instrument instrument, PressureTest pressureTest,
            TemperatureTest temperatureTest, SuperFactorTest superTest, VolumeTest volumeTest)
            : this(testNumber, instrument)
        {
            PressureTest = pressureTest;
            TemperatureTest = temperatureTest;
            SuperFactorTest = superTest;
            VolumeTest = volumeTest;
        }

        public int TestNumber { get; set; }

        public Guid InstrumentId { get; set; }

        [Required]
        public Instrument Instrument { get; set; }

        public PressureTest PressureTest { get; set; }
        public TemperatureTest TemperatureTest { get; set; }
        public VolumeTest VolumeTest { get; set; }

        [NotMapped]
        public SuperFactorTest SuperFactorTest { get; set; }

        [NotMapped]
        public bool HasPassed
        {
            get
            {
                if (Instrument.CompositionType == CorrectorType.T && TemperatureTest != null)
                    return TemperatureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == CorrectorType.P && PressureTest != null)
                    return PressureTest.HasPassed && (VolumeTest == null || VolumeTest.HasPassed);

                if (Instrument.CompositionType == CorrectorType.PTZ && PressureTest != null && TemperatureTest != null)
                    return TemperatureTest.HasPassed && PressureTest.HasPassed &&
                           (VolumeTest == null || VolumeTest.HasPassed);

                return false;
            }
        }

        public override void OnInitializing()
        {
            base.OnInitializing();

            if (Instrument.CompositionType == CorrectorType.PTZ)
                SuperFactorTest = new SuperFactorTest(this);
        }
    }
}