﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using RJCP.IO.Ports;

namespace Prover.CommProtocol.Common.IO
{
    public class SerialPortV2 : CommPort
    {
        public static List<int> BaudRates = new List<int> {300, 600, 1200, 2400, 4800, 9600, 19200, 38400};
        private readonly ISubject<char> _rcvSubject = new Subject<char>();
        private readonly SerialPortStream _serialStream;

        public SerialPortV2(string portName, int baudRate, int timeoutMs = 250)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException(nameof(portName));

            if (!SerialPortStream.GetPortNames().ToList().Contains(portName))
                throw new Exception($"{portName} does not exist.");

            if (!BaudRates.Contains(baudRate))
                throw new ArgumentException("Baud rate is invalid.");

            _serialStream = new SerialPortStream
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = timeoutMs,
                WriteTimeout = timeoutMs
            };

            TryOpen();

            DataReceivedObservable = DataReceived().Publish();
            DataReceivedObservable.Connect();

            DataSentObservable = new Subject<string>();
        }

        public sealed override IConnectableObservable<char> DataReceivedObservable { get; protected set; }
        public sealed override ISubject<string> DataSentObservable { get; protected set; }
        public override string Name => _serialStream.PortName;

        private void TryOpen()
        {
            if (_serialStream.IsOpen) return;

            _serialStream.Open();
        }

        protected sealed override IObservable<char> DataReceived()
        {
            return Observable.FromEventPattern<SerialDataReceivedEventArgs>(_serialStream, "DataReceived")
                .SelectMany(_ =>
                {
                    var dataLength = _serialStream.BytesToRead;
                    var data = new byte[dataLength];
                    var nbrDataRead = _serialStream.Read(data, 0, dataLength);
                    if (nbrDataRead == 0)
                        return new char[0];

                    var chars = Encoding.ASCII.GetChars(data);
                    return chars;
                });
        }

        public override bool IsOpen() => _serialStream.IsOpen;

        public override async Task OpenAsync()
        {
            await Task.Run(() => TryOpen());
        }

        public override async Task CloseAsync()
        {
            await Task.Run(() => _serialStream.Close());
        }

        public override void Send(string data)
        {
            _serialStream.DiscardInBuffer();
            _serialStream.DiscardOutBuffer();

            var content = new List<byte>();
            content.AddRange(Encoding.ASCII.GetBytes(data));

            var buffer = content.ToArray();
            _serialStream.Write(buffer, 0, buffer.Length);
            DataSentObservable.OnNext(data);
        }

        public override void Dispose()
        {
            _serialStream.Dispose();
        }
    }
}