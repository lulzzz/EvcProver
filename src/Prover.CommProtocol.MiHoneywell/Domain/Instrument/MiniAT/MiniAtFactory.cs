using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniAT
{
    public class MiniAtFactory : HoneywellInstrumentFactoryBase
    {
        public int AccessCode = 33333;

        public override string Description => Name;
        public int Id => 3;
        public string ItemFilePath => "MiniATItems.xml";
        public string Name => "Mini-AT";

        internal override HoneywellInstrument CreateType(bool isReadOnly)
        {
            if (isReadOnly)
                return new HoneywellReadOnlyInstrument(Id, AccessCode, Name, ItemFilePath);

            var commClient = new HoneywellClient(CommPort);
            return new HoneywellInstrument(commClient, Id, AccessCode, Name, ItemFilePath);
        }
    }
}