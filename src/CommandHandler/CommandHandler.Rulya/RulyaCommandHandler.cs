using System;
using System.Threading.Tasks;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateTextBuilderHelper;
using Microsoft.Extensions.Logging;

namespace CommandHandler.Rulya
{
    public class RulyaCommandHandler : TextCommandHandler
    {
        private readonly IExchangeRateReader _exchangeRateReader;

        public RulyaCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader) : base(logger)
        {
            _exchangeRateReader = exchangeRateReader;
            Command = "/rulya";
            Name = "Rulya-bank";
        }
        public RulyaCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader, ITextCommandHandler handler) : this(logger, exchangeRateReader)
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
