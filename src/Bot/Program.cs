using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BankReader.Lion;
using BankReader.Mono;
using BankReader.Piramida;
using BankReader.Privat;
using BankReader.Rulya;
using CommandHandler.Interfaces;
using CommandHandler.Lion;
using CommandHandler.Mono;
using CommandHandler.Piramida;
using CommandHandler.Privat;
using CommandHandler.Rulya;
using ExchangeRateReader.Interfaces.Modes;
using ExchangeRateTextBuilderHelper;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    class Program
    {
        private static ITextCommandHandler _textCommandHandler;
        private static ITelegramBotClient _botClient;
        private static InlineKeyboardMarkup _inlineKeyboardMarkups;

        static async Task Main(string[] args)
        {

            InitHandlers();
            await Init().ConfigureAwait(false);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Press 'Q' to exit");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            _botClient.StopReceiving();
        }

        static async Task Init()
        {
            var client = new HttpClient();

            _botClient = new TelegramBotClient(ConfigurationManager.AppSettings.Get("Token"), client);
            
            await _botClient.SetMyCommandsAsync(new List<BotCommand>
            {
                new()
                {
                    Command = "/start",
                    Description = "Start bot"
                }
            }).ConfigureAwait(false);

            var me = await _botClient.GetMeAsync().ConfigureAwait(false);

            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            _botClient.OnMessage += BotClientOnOnMessage;
            _botClient.OnCallbackQuery += BotClientOnOnCallbackQuery;

            _botClient.StartReceiving();
        }

        private static async void BotClientOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                var response = await _textCommandHandler
                    .Handle(e.CallbackQuery.Message.Chat.Id, e.CallbackQuery.Data)
                    .ConfigureAwait(false);


                await _botClient.EditMessageTextAsync(e.CallbackQuery.Message.Chat.Id,
                    e.CallbackQuery.Message.MessageId,
                    text: response,
                    parseMode: ParseMode.Html,
                    replyMarkup: _inlineKeyboardMarkups).ConfigureAwait(false);


                await _botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, string.Empty).ConfigureAwait(false); ;
            }
            catch (MessageIsNotModifiedException)
            {
                // todo: avoid this exception
                await _botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, string.Empty).ConfigureAwait(false); ;
            }
            catch (Exception exception)
            {
                await _botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, exception.Message).ConfigureAwait(false); ;
            }
        }

        private static async void BotClientOnOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text && e.Message.Text == "/start")
            {
                await _botClient.SendTextMessageAsync(e.Message.Chat.Id,
                    replyMarkup: _inlineKeyboardMarkups,
                    text: "Оберіть банк").ConfigureAwait(false); ;
            }
        }

        private static void InitHandlers()
        {
            var updateIntervalStr = ConfigurationManager.AppSettings.Get("ExchangeUpdateIntervalSec");
            var updateInterval = TimeSpan.FromSeconds(double.Parse(updateIntervalStr ?? "180"));


            var privat = new PrivatCommandHandler(new PrivatBankReader(new CurrencyExchangeRateConfig
            {
                ApiUrl = GetApiUrl("Privat"),
                Url = GetUrl("Privat"),
                UpdateInterval = updateInterval
            }));

            var mono = new MonoCommandHandler(new MonoBankReader(new CurrencyExchangeRateConfig
            {
                ApiUrl = GetApiUrl("Mono"),
                Url = GetUrl("Mono"),
                UpdateInterval = updateInterval
            }));

            var rulya = new RulyaCommandHandler(new RulyaBankReader(new CurrencyExchangeRateConfig
            {
                ApiUrl = GetApiUrl("Rulya"),
                Url = GetUrl("Rulya"),
                UpdateInterval = updateInterval
            }));

            var lion = new LionCommandHandler(new LionBankReader(new CurrencyExchangeRateConfig
            {
                ApiUrl = GetApiUrl("Lion"),
                Url = GetUrl("Lion"),
                UpdateInterval = updateInterval
            }));

            var piramida = new PiramidaCommandHandler(new PiramidaBankReader(new CurrencyExchangeRateConfig
            {
                ApiUrl = GetApiUrl("Piramida"),
                Url = GetUrl("Piramida"),
                UpdateInterval = updateInterval
            }));

            privat.SetNext(mono).SetNext(rulya).SetNext(lion).SetNext(piramida);

            _textCommandHandler = privat;


            _inlineKeyboardMarkups = new InlineKeyboardMarkup(
                new IEnumerable<InlineKeyboardButton>[]
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(rulya.Name, rulya.Command),
                    },
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(lion.Name, lion.Command),
                        InlineKeyboardButton.WithCallbackData(piramida.Name, piramida.Command),
                    },
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(mono.Name, mono.Command),
                        InlineKeyboardButton.WithCallbackData(privat.Name, privat.Command),
                    },
                }
            );
        }

        private static string GetApiUrl(string bank) => ConfigurationManager.AppSettings.Get($"{bank}.ApiUrl");
        private static string GetUrl(string bank) => ConfigurationManager.AppSettings.Get($"{bank}.Url");
    }
}
