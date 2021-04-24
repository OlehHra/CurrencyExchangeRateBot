namespace ExchangeRateReader.Interfaces.Modes
{
    public class CurrencyExchangeRate
    {
        public string Currency { get; set; }
        public decimal Buy { get; set; }
        public decimal Sale { get; set; }

        public override string ToString()
        {
            return $"{Currency}: {Buy} / {Sale}";
        }
    }
}
