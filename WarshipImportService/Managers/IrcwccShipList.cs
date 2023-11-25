using Serilog;
using ShipDomain;
using System.Text.Json;
using WarshipImport.DTOs;
using WarshipImport.Interfaces;

namespace WarshipImport.Managers
{
	public class IrcwccShipList : IShipList
	{
		private readonly HttpClient _client = new HttpClient();
		
		public async Task<List<IShipIdentity>> GetShipIdentities()
		{
			var result = await _client.GetAsync("https://ircwcc.org/common/shiplist/ships.php");

			if (!result.IsSuccessStatusCode)
			{
				Log.Error($"IRCWCC Ship List result: {result.ReasonPhrase}");
				return new List<IShipIdentity>();
			}

			string jsonShipData = await result.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			List<ShipIdentity>? shipData = JsonSerializer.Deserialize<List<ShipIdentity>>(jsonShipData, options);

			if (!(shipData?.Count > 0))
			{
				Log.Warning("No IRCWCC ship data records");
				return new List<IShipIdentity>(); ;
			}
		
			return shipData.OfType<IShipIdentity>().ToList();
		}
	}
}