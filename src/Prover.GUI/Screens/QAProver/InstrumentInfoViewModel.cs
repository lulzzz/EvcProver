﻿using Caliburn.Micro;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Screens.QAProver
{
    public class InstrumentInfoViewModel : ViewModelBase, IHandle<InstrumentUpdateEvent>
    {
        public InstrumentInfoViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public Instrument Instrument { get; set; }

        public string CorrectorType
        {
            get
            {
                switch (Instrument.CompositionType)
                {
                    case Core.Models.Instruments.CorrectorType.PTZ:
                        return "PTZ";
                    case Core.Models.Instruments.CorrectorType.P:
                        return "P";
                    default:
                        return "T";
                }
            }
        }

        public string BasePressure
            =>
            $"{Instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue} {Instrument.Items.GetItem(ItemCodes.Pressure.Units).Description}"
            ;

        public string BaseTemperature => $"{Instrument.EvcBaseTemperature()} {Instrument.TemperatureUnits()}";

        public string TestDatePretty => $"{Instrument.TestDateTime:MMMM d, yyyy h:mm tt}";

        public string JobIdDisplay => !string.IsNullOrEmpty(Instrument.JobId) ? $"Job #{Instrument.JobId}" : string.Empty;

        public bool EventLogChecked
        {
            get { return Instrument.EventLogPassed != null && Instrument.EventLogPassed.Value; }
            set { Instrument.EventLogPassed = value; }
        }

        public bool CommPortChecked
        {
            get { return Instrument.CommPortsPassed != null && Instrument.CommPortsPassed.Value; }
            set { Instrument.CommPortsPassed = value; }
        }

        public void Handle(InstrumentUpdateEvent message)
        {
            Instrument = message.InstrumentManager.Instrument;
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}