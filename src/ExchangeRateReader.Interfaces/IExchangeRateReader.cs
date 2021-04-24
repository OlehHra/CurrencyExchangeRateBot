using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeRateReader.Interfaces.Modes;

namespace ExchangeRateReader.Interfaces
{
    public interface IExchangeRateReader
    {
        string Name { get; }
        Task<List<CurrencyExchangeRate>> GetCurrentRate();
    }
}
