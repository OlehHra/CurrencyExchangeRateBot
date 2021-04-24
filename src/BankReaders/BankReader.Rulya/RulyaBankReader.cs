using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using HtmlAgilityPack;

namespace BankReader.Rulya
{
    public class RulyaBankReader : ExchangeRateReaderBase, IExchangeRateReader
    {
        public RulyaBankReader(CurrencyExchangeRateConfig config) : base(config)
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

            var table = doc.DocumentNode.SelectSingleNode("//table[@id='ltblBAK']");

            var response = new List<CurrencyExchangeRate>();
            foreach (var row in table.SelectNodes("//tr").Skip(1).Take(7))
            {
                response.Add(new CurrencyExchangeRate
                {
                    Currency = row.SelectSingleNode("td[1]/b").InnerText,
                    Buy = decimal.Parse(row.SelectSingleNode("td[2]").InnerText),
                    Sale = decimal.Parse(row.SelectSingleNode("td[3]").InnerText),
                });
            }

            return response;
        }
    }
}
