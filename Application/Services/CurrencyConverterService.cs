using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CurrencyConverterService
    {
        private readonly HttpClient _httpClient;

        public CurrencyConverterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> ConvertCurrency(string fromCurrency, string toCurrency, double amount)
        {
            var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";
            var response = await _httpClient.GetStringAsync(url);
            var exchangeData = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);

            if (exchangeData == null || !exchangeData.Rates.TryGetValue(toCurrency, out double value))
            {
                throw new Exception("Currency conversion failed.");
            }

            var conversionRate = value;
            return amount * conversionRate;
        }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, double>? Rates { get; set; }
    }
}
