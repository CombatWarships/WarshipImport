using Microsoft.AspNetCore.Mvc;
using Serilog.Context;
using WarshipEnrichmentAPI;
using WarshipImport.DTOs;

namespace WarshipImport.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public partial class WikiController : ControllerBase
	{
		private readonly IWarshipProcessorAPI _warshipProcessor;

		public WikiController(IWarshipProcessorAPI warshipProcessor)
		{
			_warshipProcessor = warshipProcessor;
		}

		[HttpPut()]
		public async Task<bool> CreateNewShip(string url)
		{
			using (LogContext.PushProperty("Url", url))
			{
				if (string.IsNullOrEmpty(url))
				{
					throw new ArgumentNullException(nameof(url));
				}

				if (!url.StartsWith("https://en.wikipedia.org/"))
				{
					throw new ArgumentException("Wiki link must start with https://en.wikipedia.org/.");
				}

				var newShip = new ShipIdentity()
				{
					WikiLink = url
				};

				await _warshipProcessor.PostWarship(newShip);

				return true;
			}
		}
	}
}