using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Bot.Settings
{
    public class AppSettings
    {
        public string Token { get; set; }
        public int ExchangeUpdateIntervalSec { get; set; }
        public bool LogUsers { get; set; }
        public BankSetting[] BankSettings { get; set; }
    }

    public class BankSetting
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ApiUrl { get; set; }
        public bool Enabled { get; set; }
    }
}
