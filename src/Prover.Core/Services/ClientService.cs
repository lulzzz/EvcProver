﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Data;
using Prover.Core.Storage;

namespace Prover.Core.Services
{
    public class ClientService
    {
        private readonly IProverStore<Client> _clientStore;
        private readonly IProverStore<ClientCsvTemplate> _csvTemplateStore;

        public ClientService(IProverStore<Client> clientStore, IProverStore<ClientCsvTemplate> csvTemplateStore)
        {
            _clientStore = clientStore;
            _csvTemplateStore = csvTemplateStore;
        }

        public List<Client> GetAllClients()
        {
            var clients = _clientStore.GetAll();
            return clients
                .OrderBy(c => c.Name)
                .ToList();                
        }

        public IEnumerable<Client> GetActiveClients()
        {
            return _clientStore
                .Query(x => x.ArchivedDateTime == null)
                .OrderBy(c => c.Name);
        }

        public async Task ArchiveClient(Client client)
        {
            client.ArchivedDateTime = DateTime.UtcNow;
            await _clientStore.Upsert(client);
        }

        public async Task<bool> DeleteCsvTemplate(ClientCsvTemplate template)
        {
            await _csvTemplateStore.Delete(template);
            return true;
        }

        public async Task Save(Client client)
        {
            await _clientStore.Upsert(client);
        }
    }
}