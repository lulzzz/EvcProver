﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prover.Core.DTOs;
using Prover.Core.Models.Instruments;
using Prover.Web.API.Storage;

namespace Prover.Web.API.Controllers
{
    [Route("api/[controller]")]
    public class InstrumentsController : Controller
    {
        private readonly InstrumentRepository _instrRepo;       

        public InstrumentsController(InstrumentRepository instrRepo)
        {
            _instrRepo = instrRepo;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Instrument instrument)
        {
            //var instrument = instrumentDto.ToObject();

            _instrRepo.UpsertAsync(instrument);
            Console.WriteLine(instrument.ToString());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}