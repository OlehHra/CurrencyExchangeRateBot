using System.Text.Json.Serialization;

namespace BankReader.Privat.Models
{
    internal class PrivateExchangeRate
    {
        [JsonPropertyName("ccy")]
        public string Ccy { get; set; }


        [JsonPropertyName("buy")]
        public decimal Buy { get; set; }


        [JsonPropertyName("sale")]
        public decimal Sale { get; set; }
    }
}
