﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Storage;
using Prover.Core.VerificationTests.TestActions;

namespace Prover.Core.Modules.Clients.VerificationTestActions
{
    public class ItemVerificationManager : IPreTestValidation
    {
        private readonly IHandleInvalidItemVerification _invalidItemHandler;
        private readonly ClientService _clientService;
        private readonly TestRunService _testRunService;

        public ItemVerificationManager(IHandleInvalidItemVerification invalidItemHandler, ClientService clientService,
            TestRunService testRunService)
        {
            _invalidItemHandler = invalidItemHandler;
            _clientService = clientService;
            _testRunService = testRunService;
        }

        public async Task Validate(EvcCommunicationClient commClient, Instrument instrument,
            Subject<string> statusUpdates = null)
        {
            if (!await IsValid(instrument))
            {
                var result = _invalidItemHandler.ShouldInvalidItemsBeChanged(InvalidInstrumentValues);

                if (result) await Update(commClient, instrument);
            }
        }

        private async Task<bool> IsValid(Instrument instrument)
        {
            return await Task.Run(() =>
            {
                if (instrument.Client == null) return true;

                InvalidInstrumentValues.Clear();

                ValidationItems.Clear();
                ValidationItems = instrument.Client.Items
                    .FirstOrDefault(c => c.ItemFileType == ClientItemType.Verify &&
                                         c.InstrumentType == instrument.InstrumentType)?
                    .Items
                    .ToList();

                if (ValidationItems == null) return true;

                foreach (var validItem in ValidationItems)
                {
                    var instrumentItem =
                        instrument.Items.FirstOrDefault(i => i.Metadata.Number == validItem.Metadata.Number);

                    if (instrumentItem != null && !Equals(instrumentItem.NumericValue, validItem.NumericValue))
                    {
                        var values = new Tuple<ItemValue, ItemValue>(validItem, instrumentItem);
                        InvalidInstrumentValues.Add(validItem.Metadata, values);
                    }
                }

                return !InvalidInstrumentValues.Any();
            });
        }

        /// <summary>
        ///     Contains the ItemMetada and the valid & invalid value for that item
        ///     T1 = Client value
        ///     T2 = Instrument value
        /// </summary>
        public Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>> InvalidInstrumentValues =
            new Dictionary<ItemMetadata, Tuple<ItemValue, ItemValue>>();

        public List<ItemValue> ValidationItems { get; private set; } = new List<ItemValue>();

        private async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            foreach (var invalidItem in InvalidInstrumentValues)
            {
                var response =
                    await evcCommunicationClient.SetItemValue(invalidItem.Key.Number, invalidItem.Value.Item1.RawValue);
                if (response)
                    instrument.Items.First(i => i.Metadata.Number == invalidItem.Key.Number).RawValue =
                        invalidItem.Value.Item1.RawValue;
            }
            return true;
        }
    }
}