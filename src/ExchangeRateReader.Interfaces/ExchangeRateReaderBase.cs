using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ExchangeRateReader.Interfaces.Modes;
using Timer = System.Timers.Timer;

namespace ExchangeRateReader.Interfaces
{
    public abstract class ExchangeRateReaderBase
    {
        private readonly ManualResetEvent _resetEvent = new(false);
        private List<CurrencyExchangeRate> _cache;
        public CurrencyExchangeRateConfig Config { get; }


        protected Func<Task<List<CurrencyExchangeRate>>> UpdateRate;


        protected ExchangeRateReaderBase(CurrencyExchangeRateConfig config)
        {
            Config = config;
            var t = new Timer(Config.UpdateInterval.TotalMilliseconds);
            t.Elapsed += Update;
            t.Start();
        }

        protected void Start()
        {
            Update(null, null);
        }

        private async void Update(object sender, ElapsedEventArgs e)
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
