using System.Threading.Tasks;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateTextBuilderHelper;

namespace CommandHandler.Mono
{
    public class MonoCommandHandler : TextCommandHandler
    {
        private readonly IExchangeRateReader _exchangeRateReader;
        public MonoCommandHandler(IExchangeRateReader exchangeRateReader)
        {
            _exchangeRateReader = exchangeRateReader;
            Command = "/mono";
            Name = "Monobank";
        }
        public MonoCommandHandler(IExchangeRateReader exchangeRateReader, ITextCommandHandler handler) : this(exchangeRateReader)
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
