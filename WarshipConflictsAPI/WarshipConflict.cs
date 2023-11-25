using ShipDomain;
using WarshipConflictsAPI;

namespace WarshipEnrichmentAPI
{

	public class WarshipConflict
	{
		public Ship Ship { get; set; }
		public List<Conflict> Conflicts { get; set; } = new List<Conflict>();
	}
}
