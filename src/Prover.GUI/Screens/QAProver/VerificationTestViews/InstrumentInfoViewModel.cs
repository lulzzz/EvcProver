﻿using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Extensions;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class InstrumentInfoViewModel : ViewModelBase, IHandle<InstrumentUpdateEvent>
    {
        public InstrumentInfoViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, Instrument instrument) : base(screenManager, eventAggregator)
        {
            Instrument = instrument;
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

        public void Handle(InstrumentUpdateEvent message)
        {
            Instrument = message.InstrumentManager.Instrument;
            NotifyOfPropertyChange(() => Instrument);
        }
    }
}