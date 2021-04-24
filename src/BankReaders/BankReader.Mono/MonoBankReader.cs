using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using BankReader.Mono.Models;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using Newtonsoft.Json;

namespace BankReader.Mono
{
    public class MonoBankReader : ExchangeRateReaderBase, IExchangeRateReader
    {

        public MonoBankReader(CurrencyExchangeRateConfig config) : base(config)
        {
            UpdateRate = Update;
            Start();
        }

        public Task<List<CurrencyExchangeRate>> GetCurrentRate()
        {
            return Task.FromResult(base.GetCache());
        }

        private async Task<List<CurrencyExchangeRate>> Update()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(Config.ApiUrl);
            var text = await response.Content.ReadAsStringAsync();

            var arr = JsonConvert.DeserializeObject<List<MonoExchangeRate>>(text);

            return arr.Where(x => x.CurrencyCodeB == "980" && x.RateSell != 0)
                .Select(x => new CurrencyExchangeRate
                {
                    Currency = GetCurrencyNameBy(x.CurrencyCodeA),
                    Buy = x.RateBuy,
                    Sale = x.RateSell
                }).ToList();
        }
        
        private static string GetCurrencyNameBy(string code)
        {
            return code switch
            {
                "840" => "USD",
                "978" => "EUR",
                "643" => "RUB",
                "985" => "PLZ",
                _ => code
            };
        }
    }
}
