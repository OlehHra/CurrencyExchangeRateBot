using System;
using System.Collections.Generic;
using System.Text;
using ExchangeRateReader.Interfaces.Modes;

namespace ExchangeRateTextBuilderHelper
{
    public static class ExchangeRateTextBuilder
    {
        public static string Build(string url, string name, List<CurrencyExchangeRate> rates)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<b>{0}</b>", name);
            sb.AppendLine();
            sb.AppendFormat("<code>Курс валют станом на {0:dd.MM.yyyy}</code>", DateTime.Now);
            sb.AppendLine();
            sb.AppendFormat("<code>Останнє оновлення o {0:HH:mm}</code>", DateTime.Now);
            
            sb.AppendLine("<code>");
            sb.AppendLine();
            foreach (var rate in rates)
            {
                sb.AppendFormat("{0} ", GetFlag(rate.Currency));
                sb.AppendFormat("{0,-4}", rate.Currency);
                sb.AppendFormat("{0,10:#0.000}", rate.Buy);
                sb.AppendFormat("{0,10:#0.000}", rate.Sale);
                sb.AppendLine();

            }
            sb.AppendLine("</code>");

            if (!string.IsNullOrEmpty(url))
            {
                sb.AppendLine(url);
            }

            return sb.ToString();
        }

        private static string GetFlag(string currency)
        {
            switch (currency.ToUpper())
            {
                case "USD": return "\U0001F1FA\U0001F1F8";
                case "EUR": return "\U0001F1EA\U0001F1FA";
                case "RUR":
                case "RUB": return "\U0001F1F7\U0001F1FA";
                case "GBP": return "\U0001F1FB\U0001F1EC";
                case "CHF": return "\U0001F1E8\U0001F1ED";
                case "CAD": return "\U0001F1E8\U0001F1E6";
                case "PLZ": return "\U0001F1F5\U0001F1F1";
                case "CZK": return "\U0001f1e8\U0001f1ff";
            }

            return "\U00002b50";
        }
    }
}
