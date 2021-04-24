using System.Collections.Generic;

namespace ExchangeRateReader.Interfaces.Modes
{
    public class CurrencyExchangeRateResponse
    {
        public string Url { get; }
        public IReadOnlyList<CurrencyExchangeRate> Rates { get; }

        public CurrencyExchangeRateResponse(string url, List<CurrencyExchangeRate> rates)
        {
            Url = url;
            Rates = rates;
        }
    }
}
