﻿using Caliburn.Micro;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;


namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews
{
    public class SuperFactorTestViewModel : TestRunViewModelBase<Core.Models.Instruments.SuperFactorTest>
    {
        public SuperFactorTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            Core.Models.Instruments.SuperFactorTest testRun) : base(screenManager, eventAggregator, testRun)
        {
        }

        public decimal? EVCUnsqrFactor => TestRun.EvcUnsqrFactor;

        public override void Handle(VerificationTestEvent message)
        {
            NotifyOfPropertyChange(() => TestRun);
            NotifyOfPropertyChange(() => PercentError);
            NotifyOfPropertyChange(() => EVCUnsqrFactor);
        }
    }
}