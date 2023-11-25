using Microsoft.AspNetCore.Mvc;
using Serilog;
using WarshipEnrichmentAPI;
using WarshipImport.Interfaces;

namespace WarshipImport.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class IrcwccShipListController : ControllerBase
	{
		private readonly IShipList _shipList;
		private readonly IWarshipProcessorAPI _warshipProcessor;

		public IrcwccShipListController(IShipList shipList, IWarshipProcessorAPI warshipProcessor)
		{
			_shipList = shipList;
			_warshipProcessor = warshipProcessor;
		}

		[HttpPost]
		public async Task<bool> ImportShipList()
		{
			Log.Information($"Importing Ircwcc shiplist.");
			var shiplist = await _shipList.GetShipIdentities();
			Log.Information($"Shiplist retrieved with {shiplist.Count} ships.");

			try
			{
				await _warshipProcessor.PostWarships(shiplist);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error posing Warships to Enrichment service");
				throw;
			}

			return true;
		}
	}
}