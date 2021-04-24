using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using HtmlAgilityPack;

namespace BankReader.Lion
{
    public class LionBankReader : ExchangeRateReaderBase, IExchangeRateReader
    {
        public LionBankReader(CurrencyExchangeRateConfig config) : base(config)
        {
            UpdateRate = Update;
            Start();
        }

        public Task<List<CurrencyExchangeRate>> GetCurrentRate()
        {
            return Task.FromResult(GetCache());
        }

        private async Task<List<CurrencyExchangeRate>> Update()
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(Config.ApiUrl);

            var table = doc.DocumentNode.SelectSingleNode("//center/div[5]/table");

            var response = new List<CurrencyExchangeRate>();
            var index = 0;
            foreach (var row in table.SelectNodes("//tr").Skip(1).Take(8))
            {
                response.Add(new CurrencyExchangeRate
                {
                    Currency = GetCurrency(++index),
                    Buy = decimal.Parse(row.SelectSingleNode("td[1]").InnerText),
                    Sale = decimal.Parse(row.SelectSingleNode("td[3]").InnerText),
                });
            }

            return response;
        }

        private static string GetCurrency(int index)
        {
            switch (index)
            {
                case 1: return "USD";
                case 2: return "EUR";
                case 3: return "RUB";
                case 4: return "PLZ";
                case 5: return "GBP";
                case 6: return "CZK";
                case 7: return "CAD";
                case 8: return "CHF";
            }

            return "XXX";
        }
    }
}
