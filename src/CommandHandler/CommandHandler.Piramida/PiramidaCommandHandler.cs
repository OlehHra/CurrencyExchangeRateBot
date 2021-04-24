using System;
using System.Threading.Tasks;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateTextBuilderHelper;
using Microsoft.Extensions.Logging;

namespace CommandHandler.Piramida
{
    public class PiramidaCommandHandler : TextCommandHandler
    {
        private readonly IExchangeRateReader _exchangeRateReader;

        public PiramidaCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader) : base(logger)
        {
            _exchangeRateReader = exchangeRateReader;
            Command = "/piramida";
            Name = "Piramida";
        }
        public PiramidaCommandHandler(ILogger logger, IExchangeRateReader exchangeRateReader, ITextCommandHandler handler) 
            : this(logger, exchangeRateReader)
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
