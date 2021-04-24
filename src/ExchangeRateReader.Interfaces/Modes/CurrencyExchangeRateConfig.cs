using System;

namespace ExchangeRateReader.Interfaces.Modes
{
    public class CurrencyExchangeRateConfig
    {
        public string ApiUrl { get; set; }
        public string Url { get; set; }
        public TimeSpan UpdateInterval { get; set; } = TimeSpan.Zero;
    }
}
