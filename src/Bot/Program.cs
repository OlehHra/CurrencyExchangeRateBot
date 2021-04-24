using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using BankReader.Lion;
using BankReader.Mono;
using BankReader.Piramida;
using BankReader.Privat;
using BankReader.Rulya;
using Bot.Domain;
using Bot.Settings;
using CommandHandler.Interfaces;
using CommandHandler.Lion;
using CommandHandler.Mono;
using CommandHandler.Piramida;
using CommandHandler.Privat;
using CommandHandler.Rulya;
using ExchangeRateReader.Interfaces.Modes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;

namespace Bot
{
    class Program
    {
        private static ITextCommandHandler _textCommandHandler;
        private static ITelegramBotClient _botClient;
        private static InlineKeyboardMarkup _inlineKeyboardMarkups;


        static async Task Main(string[] args)
        {



            LogManager.LoadConfiguration("nlog.config");
            var services = new ServiceCollection();
            services.AddLogging();
            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<ILoggerFactory>();
            factory.AddNLog();

            var logger = factory.CreateLogger("general");
            logger.LogInformation($"{new string('-', 10)} Start {new string('-', 10)}");

            var settings = ConfigReader.Read();


            var commandFactory = new CommandFactory(logger,settings);



            var bot = new BotWrapper(logger, commandFactory, settings);
            await bot.Init().ConfigureAwait(false);

            bool isWorking = true;
            while (isWorking)
            {
                Console.WriteLine("Press 'Q' to exit");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Q:
                        logger.LogInformation("User pressed 'Q'");
                        bot?.Dispose();
                        isWorking = false;
                        break;
                }
            }
        }
        
    }
}
