using WarshipImport.Data;

namespace WarshipImport.Interfaces
{
    public interface IProposedShipsDatabase
    {
		List<ProposedShip> GetUserShips(string userId);
		
		ProposedShip? GetShip(Guid shipId);

		Task<ProposedShip> CreateOrUpdate(ProposedShip ship, string userId);

		Task<bool> Delete(Guid shipId, string userId);
	 }
}
