using System.Collections;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Net.Http.Headers;

public interface ICurrencyRateService
{
    public Task<decimal> GetCurrencyRate(DateTime date);
}

public class CurrencyRateService : ICurrencyRateService
{
    private readonly IHttpClientFactory _clientFactory;

    public CurrencyRateService(IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
    }

    public async Task<decimal> GetCurrencyRate(DateTime date)
    {
        var dateStr = date.ToString("yyyyMMdd");
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchangenew?json&valcode=USD&date={dateStr}")
        {
            Headers =
            {
                { HeaderNames.Accept, "application/json" },
                { HeaderNames.UserAgent, "HttpRequestsSample" }
            }
        };

        var httpClient = _clientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        var value = new Hashtable();
        value.Add("asd", "sdf");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var contentString = Regex.Unescape((await httpResponseMessage.Content.ReadAsStringAsync())
                .Replace("\r", "")
                .Replace(Environment.NewLine, "")
                .Trim('"'));
            
            var currencyRates = JsonSerializer.Deserialize<CurrencyRateItem[]>(contentString);
            if (currencyRates != null)
            {
                return currencyRates[0].rate;
            }
        }

        return 0;
    }
}

public class CurrencyRateItem 
{
    public string? txt { get; set; }
    public string? exchangedate { get; set; }
    public string? cc { get; set; }
    public decimal rate { get; set; }
}