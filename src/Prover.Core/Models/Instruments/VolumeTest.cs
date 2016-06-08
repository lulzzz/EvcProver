﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using NLog;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.DriveTypes;
using Prover.Core.Extensions;
using Prover.Core.EVCTypes;

namespace Prover.Core.Models.Instruments
{
    public sealed class VolumeTest : ProverTable
    {

        public VolumeTest(VerificationTest verificationTest, IDriveType driveType) : base(verificationTest.Instrument.)
        {
            VerificationTest = verificationTest;
            VerificationTestId = VerificationTest.Id;
            
            DriveType = driveType;
            AppliedInput = DriveType.MaxUnCorrected();
            DriveTypeDiscriminator = DriveType.Discriminator;
        }

        public int PulseACount { get; set; }
        public int PulseBCount { get; set; }
        public decimal AppliedInput { get; set; }

        public IDriveType DriveType { get; set; }

        private string _testInstrumentData;
        public string TestInstrumentData
        {
            get { return AfterTestItems.Serialize(); }
            set
            {
                _testInstrumentData = value;
            }
        }

        public Instrument Instrument => VerificationTest.Instrument;

        public Guid VerificationTestId { get; set; }

        [Required]
        public VerificationTest VerificationTest { get; set; }

        [NotMapped]
        public IEnumerable<ItemValue> AfterTestItems { get; set; }

        [NotMapped]
        public decimal? UnCorrectedPercentError
        {
            get
            {
                if (DriveType?.UnCorrectedInputVolume(AppliedInput) != 0 && DriveType?.UnCorrectedInputVolume(AppliedInput) != null)
                {
                    return Math.Round((decimal)(((EvcUncorrected - DriveType.UnCorrectedInputVolume(AppliedInput) / DriveType.UnCorrectedInputVolume(AppliedInput) * 100))), 2);
                }

                return null;
            }
        }

        [NotMapped]
        public decimal? CorrectedPercentError
        {
            get
            {
                if (TrueCorrected != 0 && TrueCorrected != null)
                {
                    return Math.Round((decimal)(((EvcCorrected - TrueCorrected) / TrueCorrected) * 100), 2);
                }
                else
                {
                    return 0;
                }
            }
        }

        [NotMapped]
        public bool CorrectedHasPassed => CorrectedPercentError.IsBetween(Global.COR_ERROR_THRESHOLD);

        [NotMapped]
        public bool UnCorrectedHasPassed => (UnCorrectedPercentError.IsBetween(Global.UNCOR_ERROR_THRESHOLD));

        [NotMapped]
        public bool HasPassed => CorrectedHasPassed && UnCorrectedHasPassed && DriveType.HasPassed;

        [NotMapped]
        public int UncPulseCount
        {

            get
            {
                if (Instrument.PulseASelect() == "UncVol")
                    return PulseACount;

                return PulseBCount;
            }

        }

        [NotMapped]
        public int CorPulseCount
        {
            get
            {
                if (Instrument.PulseASelect() == "CorVol")
                    return PulseACount;

                return PulseBCount;
            }
        }

        [NotMapped]
        public decimal? TrueCorrected
        {
            get
            {
                if (VerificationTest == null) return null;

                if (VerificationTest.Instrument.CompositionType == CorrectorType.T && VerificationTest.TemperatureTest != null)
                {
                    return (VerificationTest.TemperatureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                if (VerificationTest.Instrument.CompositionType == CorrectorType.P && VerificationTest.PressureTest != null)
                {
                    return (VerificationTest.PressureTest.ActualFactor * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                if (VerificationTest.Instrument.CompositionType == CorrectorType.PTZ)
                {
                    return (VerificationTest.PressureTest?.ActualFactor * VerificationTest.TemperatureTest?.ActualFactor * VerificationTest.SuperFactorTest.SuperFactorSquared * DriveType.UnCorrectedInputVolume(AppliedInput));
                }

                return null;
            }
        }

        public decimal EvcCorrected
        {
            get
            {
                return VerificationTest.Instrument.EvcCorrected(Items, AfterTestItems).Value;
            }
        }

        [NotMapped]
        public decimal EvcUncorrected
        {
            get
            {
                return VerificationTest.Instrument.EvcUncorrected(Items, AfterTestItems).Value;
            }
        }

        public string DriveTypeDiscriminator { get; set; }

        private void CreateDriveType()
        {
            if (DriveType == null && DriveTypeDiscriminator != null && VerificationTest != null)
            {
                switch (DriveTypeDiscriminator)
                {
                    case "Rotary":
                        DriveType = new RotaryDrive(this.VerificationTest.Instrument);
                        break;
                    case "Mechanical":
                        DriveType = new MechanicalDrive(this.VerificationTest.Instrument);
                        AppliedInput = DriveType.MaxUnCorrected();
                        break;
                    default:
                        throw new NotSupportedException($"Drive type {DriveTypeDiscriminator} is not supported.");
                }
            }
        }

        [NotMapped]
        public override InstrumentType InstrumentType => Instrument.Type;

        public override void OnInitializing()
        {
            base.OnInitializing();

            CreateDriveType();

            var afterItemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_testInstrumentData);
            AfterTestItems = ItemHelpers.LoadItems(Instrument.Type, afterItemValues);
        }
    }
}
