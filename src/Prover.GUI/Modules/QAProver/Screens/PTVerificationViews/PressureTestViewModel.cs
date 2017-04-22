﻿using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class PressureTestViewModel : TestRunViewModelBase<Core.Models.Instruments.PressureTest>
    {
        public PressureTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.PressureTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        public bool ShowATMValues
            =>
                (TransducerType)
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.TransducerType).NumericValue != TransducerType.Absolute;

        public bool ShowATMGaugeInput
            =>
                (TransducerType)
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.TransducerType).NumericValue == TransducerType.Absolute;

        public decimal Gauge
        {
            get { return TestRun.GasGauge.Value; }
            set
            {
                TestRun.GasGauge = value;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal GasPressure
        {
            get
            {
                if (TestRun.GasPressure != null) return TestRun.GasPressure.Value;
                return decimal.Zero;
            }
        }

        public decimal? AtmosphericGauge
        {
            get { return TestRun.AtmosphericGauge; }
            set
            {
                TestRun.AtmosphericGauge = value;
                EventAggregator.PublishOnUIThread(VerificationTestEvent.Raise());
            }
        }

        public decimal? EvcGasPressure
            => TestRun.Items.GetItem(ItemCodes.Pressure.GasPressure).NumericValue
        ;

        public decimal? EvcFactor
            => TestRun.Items.GetItem(ItemCodes.Pressure.Factor).NumericValue;

        public decimal? EvcATMPressure
            =>
                TestRun.VerificationTest.Instrument.Items.GetItem(
                    ItemCodes.Pressure.Atm).NumericValue;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => TestRun.PercentError);
            NotifyOfPropertyChange(() => TestRun.HasPassed);
            NotifyOfPropertyChange(() => EvcGasPressure);
            NotifyOfPropertyChange(() => GasPressure);
            NotifyOfPropertyChange(() => EvcFactor);
            NotifyOfPropertyChange(() => EvcATMPressure);
            NotifyOfPropertyChange(() => Gauge);
            NotifyOfPropertyChange(() => AtmosphericGauge);
            NotifyOfPropertyChange(() => PercentColour);
        }
    }
}