using System.Text.Json.Serialization;

namespace BankReader.Mono.Models
{
    internal class MonoExchangeRate
    {
        [JsonPropertyName("currencyCodeA")]
        public string CurrencyCodeA { get; set; }


        [JsonPropertyName("currencyCodeA")]
        public string CurrencyCodeB { get; set; }


        [JsonPropertyName("currencyCodeA")]
        public string Date { get; set; }


        [JsonPropertyName("currencyCodeA")]
        public decimal RateSell { get; set; }


        [JsonPropertyName("currencyCodeA")]
        public decimal RateBuy { get; set; }


        [JsonPropertyName("currencyCodeA")]
        public decimal RateCross { get; set; }
    }
}
