using ServiceBus.Core;

namespace WarshipImport
{
	public class WarshipConflictConsumerHost : ServiceBusConsumerHost
	{
		public WarshipConflictConsumerHost(IConfiguration configuration, IMessageProcessor messageProcessor)
			: base(configuration.GetConnectionString("ConflictedWarshipServiceBus"), "warshipconflicts", messageProcessor)
		{
		}
	}
}
