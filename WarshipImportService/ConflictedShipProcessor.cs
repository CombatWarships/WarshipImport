using AutoMapper;
using Serilog;
using Serilog.Context;
using ServiceBus.Core;
using ShipDomain;
using System.Text.Json;
using WarshipEnrichmentAPI;
using WarshipImport.Data;
using WarshipImport.Interfaces;

namespace WarshipEnrichment
{

	public class ConflictedShipProcessor : IMessageProcessor
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IMapper _shipToProposedMapper;

		public ConflictedShipProcessor(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_shipToProposedMapper = new MapperConfiguration(cfg => cfg.CreateMap<Ship, ProposedShip>()).CreateMapper();
		}

		public async Task ProcessMessage(string message)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var database = scope.ServiceProvider.GetService<IProposedShipsDatabase>();
			
				using (LogContext.PushProperty("MessageJSON", message))
				{
					var conflictedShip = JsonSerializer.Deserialize<WarshipConflict>(message);

					Log.Information("Adding ship to database");

					ProposedShip proposedShip = _shipToProposedMapper.Map<Ship, ProposedShip>(conflictedShip.Ship);
					proposedShip.UserID = "Justin";

					var resultShip = await database.CreateOrUpdate(proposedShip, proposedShip.UserID);
				}
			}
		}
	}
}
