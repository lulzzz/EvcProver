﻿using Prover.Core.Storage;

namespace Prover.Modules.Certificates
{
    public interface ICertificateStore<T> : IProverStore<T>
        where T : class
    {
    }
}