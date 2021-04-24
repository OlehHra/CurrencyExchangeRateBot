using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ExchangeRateReader.Interfaces.Modes;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace ExchangeRateReader.Interfaces
{
    public abstract class ExchangeRateReaderBase
    {
        protected readonly ILogger Logger;
        private readonly ManualResetEvent _resetEvent = new(true);
        private List<CurrencyExchangeRate> _cache = new List<CurrencyExchangeRate>();
        public CurrencyExchangeRateConfig Config { get; }
        public string Name { get; protected set; }


        protected Func<Task<List<CurrencyExchangeRate>>> UpdateRate;


        protected ExchangeRateReaderBase(ILogger logger, CurrencyExchangeRateConfig config)
        {
            Logger = logger;
            Config = config;
            var t = new Timer(Config.UpdateInterval.TotalMilliseconds);
            t.Elapsed += Update;
            t.Start();
        }

        protected void Start()
        {
            try
            {
                Update(null, null);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        private async void Update(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (UpdateRate != null)
                {
                    var rates = await UpdateRate();
                    SetCache(rates);
                }
                else
                {
                    throw new NotImplementedException(nameof(Update));
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.ToString());
            }
        }


        protected List<CurrencyExchangeRate> GetCache()
        {
            if (_resetEvent.WaitOne(TimeSpan.FromSeconds(20)))
            {
                return _cache;
            }

            throw new Exception("Update currency timeout");
        }

        private void SetCache(List<CurrencyExchangeRate> rates)
        {
            _resetEvent.Reset();
            _cache = rates;
            _resetEvent.Set();
        }

    }
}
