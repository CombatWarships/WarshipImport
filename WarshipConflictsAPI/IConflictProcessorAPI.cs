namespace WarshipEnrichmentAPI
{
	public interface IConflictProcessorAPI
	{
		Task PostWarship(WarshipConflict ship);
		Task PostWarships(IEnumerable<WarshipConflict> ships);
	}
}