﻿using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.ExternalIntegrations.Validators;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Common;

namespace UnionGas.MASA.Validators.InventoryCode
{
    public class CompanyNumberUpdater : IUpdater
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly IGetValue _valueRequestor;

        public CompanyNumberUpdater(IInstrumentStore<Instrument> instrumentStore, IGetValue valueRequestor)
        {
            _instrumentStore = instrumentStore;
            _valueRequestor = valueRequestor;
        }

        public async Task<object> Update(EvcCommunicationClient evcCommunicationClient, Instrument instrument)
        {
            var newCompanyNumber = _valueRequestor.GetValue();
            if (newCompanyNumber == null) return null;

            await evcCommunicationClient.Connect();

            var response =
                await
                    evcCommunicationClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            
            await evcCommunicationClient.Disconnect();

            if (response)
            {
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber;
                await _instrumentStore.UpsertAsync(instrument);
            }
            else
            {
                //TODO throw exception that the inventory couldn't be updated
            }

            return newCompanyNumber;
        }
    }
}