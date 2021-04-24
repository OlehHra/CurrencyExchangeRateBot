using System;
using System.Threading.Tasks;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateTextBuilderHelper;

namespace CommandHandler.Piramida
{
    public class PiramidaCommandHandler : TextCommandHandler
    {
        private readonly IExchangeRateReader _exchangeRateReader;

        public PiramidaCommandHandler(IExchangeRateReader exchangeRateReader)
        {
            _exchangeRateReader = exchangeRateReader;
            Command = "/piramida";
            Name = "Piramida";
        }
        public PiramidaCommandHandler(IExchangeRateReader exchangeRateReader, ITextCommandHandler handler) : this(exchangeRateReader)
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
