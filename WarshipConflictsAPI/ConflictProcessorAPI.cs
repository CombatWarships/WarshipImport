using ServiceBus.Core;

namespace WarshipEnrichmentAPI
{
	public sealed class ConflictProcessorAPI : ServiceBusProducer<WarshipConflict>, IConflictProcessorAPI
	{
		public ConflictProcessorAPI(string connectionString) :
			base(connectionString, "WarshipConflicts")
		{ }

		public Task PostWarship(WarshipConflict ship)
		{
			return PostMessage(ship);
		}

		public Task PostWarships(IEnumerable<WarshipConflict> ships)
		{
			return PostMessages(ships);
		}
	}
}
