using ShipDomain;
using WarshipEnrichmentAPI;

namespace WarshipImport.DTOs
{
    internal class ShipIdentity : IShipIdentity
    {
        public Guid? ID { get; set; }

        public string? WikiLink { get; set; }

        public int? ShiplistKey { get; set; }
    }
}