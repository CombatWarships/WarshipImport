using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;
using ShipDomain;
using System.Text.Json;
using System.Web;
using WarshipImport.Data;
using WarshipImport.DTOs;
using WarshipImport.Interfaces;

namespace WarshipImport.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ProposedShipsController
	{
		private readonly IProposedShipsDatabase _database;
		private readonly IMapper _proposedToShipMapper;
		private readonly IMapper _shipToProposedMapper;

		public ProposedShipsController(IProposedShipsDatabase proposedShipsDatabase)
		{
			_database = proposedShipsDatabase;

			_proposedToShipMapper = new MapperConfiguration(cfg => cfg.CreateMap<ProposedShip, Ship>()).CreateMapper();
			_shipToProposedMapper = new MapperConfiguration(cfg => cfg.CreateMap<Ship, ProposedShip>()).CreateMapper();
		}

		[HttpGet]
		public async Task<List<Ship>> GetAll()
		{
			var userId = "Justin";

			using (LogContext.PushProperty("UserID", userId))
			{
				Log.Information($"Searching for users ships");

				var usersShips = _database.GetUserShips(userId);

				Log.Information($"Found {usersShips.Count} associated with this user");

				return usersShips.Select(s => _proposedToShipMapper.Map<ProposedShip, Ship>(s)).ToList();
			}
		}

		[HttpGet("{shipId}")]
		public async Task<ProposedShipWorksheet?> GetShip(string shipId)
		{
			var userId = "Justin";
			using (LogContext.PushProperty("UserID", userId))
			using (LogContext.PushProperty("ShipID", shipId))
			{
				if (!Guid.TryParse(shipId, out Guid shipIDGuid))
					throw new ArgumentNullException(nameof(shipId));

				if (shipIDGuid == Guid.Empty)
					throw new ArgumentNullException(nameof(shipId));

				Log.Information($"Finding ship by id");

				var requestedShip = _database.GetShip(shipIDGuid);

				if (requestedShip == null)
				{
					Log.Warning("Ship not found by ID");
					return null;
				}

				Log.Information("Ship found");
				var worksheet = new ProposedShipWorksheet();
				worksheet.ProposedShip = _proposedToShipMapper.Map<ProposedShip, Ship>(requestedShip);
								
				return worksheet;
			}
		}

		[HttpPut()]
		public async Task<Ship> Put(Ship ship)
		{
			var userId = "Justin";
			using (LogContext.PushProperty("UserID", userId))
			{
				var json = JsonSerializer.Serialize(ship);
				using (LogContext.PushProperty("Ship", json))
				{
					Log.Information("Adding ship to database");

					ProposedShip proposedShip = _shipToProposedMapper.Map<Ship, ProposedShip>(ship);
					proposedShip.UserID = userId;

					var resultShip = await _database.CreateOrUpdate(proposedShip, userId);

					return _proposedToShipMapper.Map<ProposedShip, Ship>(resultShip);
				}
			}
		}

		[HttpPost()]
		public async Task<Ship> Post(Ship[] ships)
		{
			var userId = "Justin";
			using (LogContext.PushProperty("UserID", userId))
			{
				var json = JsonSerializer.Serialize(ships);
				using (LogContext.PushProperty("Ship", json))
				{
					//Log.Information("Adding ship to database");

					//ProposedShip proposedShip = _shipToProposedMapper.Map<Ship, ProposedShip>(ship);
					//proposedShip.UserID = userId;

					//var resultShip = await _database.CreateOrUpdate(proposedShip, userId);

					//return _proposedToShipMapper.Map<ProposedShip, Ship>(resultShip);

					return null;
				}
			}
		}

		[HttpDelete]
		public async Task Delete(string shipIdsJson)
		{

			var userId = "Justin";
			using (LogContext.PushProperty("UserID", userId))
			{
				using (LogContext.PushProperty("ShipID", shipIdsJson))
				{
					var shipIds = JsonSerializer.Deserialize<string[]>(shipIdsJson);

					var shipGuids = shipIds.Select(shipId =>
					{
						if (!Guid.TryParse(shipId, out var id))
							throw new ArgumentException($"{shipId} could not be converted to GUID");

					
						return id;
					}).ToList();

					foreach (var shipId in shipGuids)
					{		
						Log.Information($"Attempting to removing ship {shipId} from database");

						await _database.Delete(shipId, userId);
					}
				}
			}
		}
	}
}