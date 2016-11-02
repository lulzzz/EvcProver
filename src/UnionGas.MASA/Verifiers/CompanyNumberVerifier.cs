﻿using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public CompanyNumberVerifier(IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap webService,
            IUpdater updater)
        {
            InstrumentStore = instrumentStore;
            WebService = webService;
            Updater = updater;
        }

        public IUpdater Updater { get; set; }

        public IInstrumentStore<Instrument> InstrumentStore { get; set; }

        public DCRWebServiceSoap WebService { get; }

        public async Task<object> Verify(EvcCommunicationClient commClient, Instrument instrument)
        {
            var companyNumberItem = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);
            var companyNumber = companyNumberItem.RawValue;

            var meterDto = await VerifyWithWebService(companyNumber);
            while (meterDto != null && meterDto.InventoryCode == null)
            {
                _log.Debug($"Company number wasn't present in an open job.");
                var newCompanyNumber = await Updater.Update(commClient, instrument);

                meterDto = await VerifyWithWebService(newCompanyNumber.ToString());
            }

            return meterDto;
        }

        private async Task<MeterDTO> VerifyWithWebService(string companyNumber)
        {
            _log.Debug($"Verifying company number {companyNumber} with web service.");

            try
            {
                var request = new GetValidatedEvcDeviceByInventoryCodeRequest
                {
                    Body = new GetValidatedEvcDeviceByInventoryCodeRequestBody(companyNumber)
                };

                var response = await WebService.GetValidatedEvcDeviceByInventoryCodeAsync(request);
                return response.Body.GetValidatedEvcDeviceByInventoryCodeResult;
            }
            catch (EndpointNotFoundException)
            {
                _log.Warn($"Web service not available. Skipping company number verification.");
                return null;
                throw;
            }
        }
    }
}