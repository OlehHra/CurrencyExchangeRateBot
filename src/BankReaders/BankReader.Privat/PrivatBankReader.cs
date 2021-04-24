using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BankReader.Privat.Models;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankReader.Privat
{
    public class PrivatBankReader : ExchangeRateReaderBase, IExchangeRateReader
    {
        public PrivatBankReader(ILogger logger, CurrencyExchangeRateConfig config) : base(logger, config)
        {
            Name = "Privat";
            UpdateRate = Update;
            Start();
        }

        public Task<List<CurrencyExchangeRate>> GetCurrentRate()
        {
            return Task.FromResult(GetCache());
        }

        public async Task<List<CurrencyExchangeRate>> Update()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(Config.ApiUrl);
            var text = await response.Content.ReadAsStringAsync();

            var arr = JsonConvert.DeserializeObject<List<PrivateExchangeRate>>(text);

            return arr.Where(x => !string.Equals(x.Ccy, "BTC")).Select(x => new CurrencyExchangeRate
            {
                Currency = x.Ccy,
                Buy = x.Buy,
                Sale = x.Sale
            }).ToList();
        }
    }
}
