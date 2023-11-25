using ShipDomain;

namespace WarshipImport.Interfaces
{
	public interface IShipList
    {
        Task<List<IShipIdentity>> GetShipIdentities();
    }
}