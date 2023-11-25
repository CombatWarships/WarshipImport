using ShipDomain;
using WarshipImport.Data;

namespace WarshipImport.DTOs
{
	public class ProposedShipWorksheet
	{
		public Ship ProposedShip { get; set; }
	
		public Ship WikiShip { get; set; }
		
		public Ship IrcwccShip { get; set; }
	}
}
