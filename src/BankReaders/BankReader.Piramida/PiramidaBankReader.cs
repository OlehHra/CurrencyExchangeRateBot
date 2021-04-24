using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using HtmlAgilityPack;

namespace BankReader.Piramida
{
    public class PiramidaBankReader : ExchangeRateReaderBase, IExchangeRateReader
    {
        public PiramidaBankReader(CurrencyExchangeRateConfig config) : base(config)
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

            var rows = doc.DocumentNode.SelectNodes("//div[@class='kurs-result']");


            var response = new List<CurrencyExchangeRate>();
            foreach (var row in rows)
            {
                response.Add(new CurrencyExchangeRate
                {
                    Currency = row.SelectSingleNode("span[1]").InnerText,
                    Buy = ParsePrice(row.SelectSingleNode("span[2]").InnerHtml),
                    Sale = ParsePrice(row.SelectSingleNode("span[3]").InnerHtml),
                });
            }

            return response;
        }

        decimal ParsePrice(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            var strInd = text.LastIndexOf(">", StringComparison.OrdinalIgnoreCase);

            var str = strInd != -1
                ? text.Substring(strInd + 1)
                : text;

            return decimal.Parse(str);
        }

    }
}
