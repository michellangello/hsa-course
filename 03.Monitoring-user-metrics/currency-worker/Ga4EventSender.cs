using System.Text;
using Newtonsoft.Json;

namespace currency_worker;

public class Ga4EventSender(ILogger<Ga4EventSender> logger)
{
	private static readonly HttpClient HttpClient = new()
	{
		BaseAddress = new Uri("https://www.google-analytics.com/mp/collect")
	};

	private readonly string _measurementId = Environment.GetEnvironmentVariable("GA_MEASUREMENT_ID")
	                                         ?? throw new ArgumentNullException("GA_MEASUREMENT_ID");
	private readonly string _apiSecret = Environment.GetEnvironmentVariable("GA_API_SECRET")
	                                     ?? throw new InvalidOperationException("GA_API_SECRET");

	public async Task SendExchangeRateEventAsync(decimal uahToUsdRate)
	{
		var eventData = new
		{
			client_id = Guid.NewGuid().ToString(),
			events = new[]
			{
				new
				{
					name = "exchange_rate",
					@params = new
					{
						currency = "UAH/USD",
						rate = uahToUsdRate
					}
				}
			}
		};

		var jsonPayload = JsonConvert.SerializeObject(eventData);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await HttpClient.PostAsync($"?measurement_id={_measurementId}&api_secret={_apiSecret}", content);

		if (response.IsSuccessStatusCode)
		{
			logger.LogInformation("Event sent successfully!");
		}
		else
		{
			logger.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
		}
	}
}
