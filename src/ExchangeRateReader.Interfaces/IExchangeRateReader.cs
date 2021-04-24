using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeRateReader.Interfaces.Modes;

namespace ExchangeRateReader.Interfaces
{
    public interface IExchangeRateReader
    {
        Task<List<CurrencyExchangeRate>> GetCurrentRate();
    }
}
