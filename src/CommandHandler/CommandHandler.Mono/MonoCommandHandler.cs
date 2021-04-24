using System.Threading.Tasks;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateTextBuilderHelper;
using Microsoft.Extensions.Logging;

namespace CommandHandler.Mono
{
    public class MonoCommandHandler : TextCommandHandler
    {
        private readonly IExchangeRateReader _exchangeRateReader;
        public MonoCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader) : base(logger)
        {
            _exchangeRateReader = exchangeRateReader;
            Command = "/mono";
            Name = "Monobank";
        }
        public MonoCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader, ITextCommandHandler handler) : this(logger, exchangeRateReader)
        {
            base.SetNext(handler);
        }

        public override async Task<string> Handle(long chatId, string command)
        {
            if (IsCommand(command))
            {
                var rates = await _exchangeRateReader.GetCurrentRate();
                return ExchangeRateTextBuilder.Build(string.Empty, Name, rates);
            }
            else
            {
                return await base.Handle(chatId, command);
            }
        }
    }
}
