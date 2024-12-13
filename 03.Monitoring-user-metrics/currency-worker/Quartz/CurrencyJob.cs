using System.Net.Http.Json;
using currency_worker.Analytics;
using Quartz;
using Newtonsoft.Json;

namespace currency_worker.Quartz;

public class CurrencyJob(Ga4EventSender ga4EventSender, ILogger<CurrencyJob> logger)
{
	private static readonly HttpClient Client = new()
	{
		BaseAddress = new Uri("https://bank.gov.ua/NBUStatService/v1/")
	};

	public async Task ExecuteAsync()
	{
		logger.LogWarning("Retrieving currency {UtcNow}", DateTime.UtcNow);

		var result = await Client.GetFromJsonAsAsyncEnumerable<NbuStatServiceResponse>(
				"statdirectory/exchange?valcode=USD&json")
			.FirstOrDefaultAsync();

		if (result is null)
		{
			logger.LogWarning("Could not get Exchange value {UtcNow}", DateTime.UtcNow);
			return;
		}

		logger.LogInformation("Exchange value {ExchangeRate} {UtcNow}", result.Rate, DateTime.UtcNow);

		await ga4EventSender.SendExchangeRateEventAsync(result.Rate);
	}
}

public record NbuStatServiceResponse(
	[JsonProperty("r030")] int Id,
	[JsonProperty("txt")] string Txt,
	[JsonProperty("rate")] decimal Rate,
	[JsonProperty("cc")] string Currency,
	[JsonProperty("exchangedate")] string ExchangeDate
);
