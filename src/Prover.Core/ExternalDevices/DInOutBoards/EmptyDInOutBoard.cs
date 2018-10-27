﻿using System;
using System.Threading.Tasks;

namespace Prover.Core.ExternalDevices.DInOutBoards
{
    public class EmptyDInOutBoard : IDInOutBoard
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<int> ReadInput()
        {
            return 0;
        }

        public void StartMotor()
        {
        }

        public void StopMotor()
        {
        }

        public decimal PulseTiming { get; set; }
    }
}