﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BankReader.Lion;
using Bot.Domain;
using Bot.Settings;
using CommandHandler.Interfaces;
using CommandHandler.Rulya;
using ExchangeRateReader.Interfaces.Modes;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    internal sealed class BotWrapper : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICommandFactory _factory;
        private readonly AppSettings _settings;
        private readonly ITelegramBotClient _botClient;
        private ITextCommandHandler _textCommandHandlers;
        private InlineKeyboardMarkup _inlineKeyboardMarkups;

        public BotWrapper(ILogger logger, ICommandFactory factory, AppSettings settings)
        {
            _logger = logger;
            _factory = factory;
            _settings = settings;
            _botClient = new TelegramBotClient(settings.Token);
        }

        public async Task Init()
        {
            _textCommandHandlers = _factory.CreateTextCommandHandler();
            await _botClient.SetMyCommandsAsync(new List<BotCommand>
            {
                new()
                {
                    Command = "/start",
                    Description = "Start bot"
                }
            }).ConfigureAwait(false);

            var me = await _botClient.GetMeAsync().ConfigureAwait(false);
            _logger.LogInformation($"{me.Id}:{me.FirstName}");

            _botClient.OnMessage += BotClientOnOnMessage;
            _botClient.OnCallbackQuery += BotClientOnOnCallbackQuery;

            _botClient.StartReceiving();

            InitHandlers();
        }

        private void InitHandlers()
        {
            List<ITextCommandHandler> commands = new List<ITextCommandHandler>();
            ITextCommandHandler currentHandler = _textCommandHandlers;
            do
            {
                commands.Add(currentHandler);
                currentHandler = currentHandler.GetNext();
            } while (currentHandler != null);



            var buttons = new List<List<InlineKeyboardButton>>();
            
            for (int i = 0; i < commands.Count;)
            {
                if (i == 0)
                {
                    if (commands.Count % 2 == 0)
                    {
                        buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(commands[i].Name, commands[i++].Command),
                            InlineKeyboardButton.WithCallbackData(commands[i].Name, commands[i++].Command),
                        });
                    }
                    else
                    {
                        buttons.Add(new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData(commands[i].Name, commands[i++].Command),
                        });
                    }
                }
                else
                {
                    var list = new List<InlineKeyboardButton>();
                    list.Add(InlineKeyboardButton.WithCallbackData(commands[i].Name, commands[i++].Command));
                    if(i < commands.Count)
                    {
                        list.Add(InlineKeyboardButton.WithCallbackData(commands[i].Name, commands[i++].Command));
                    }
                    buttons.Add(list);
                }


            }

            _inlineKeyboardMarkups = new InlineKeyboardMarkup(buttons);
        }

        private async void BotClientOnOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text && e.Message.Text == "/start")
            {
                await _botClient.SendTextMessageAsync(e.Message.Chat.Id,
                    replyMarkup: _inlineKeyboardMarkups,
                    text: "Оберіть банк").ConfigureAwait(false); ;
            }
        }

        private async void BotClientOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            try
            {
                var response = await _textCommandHandlers
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


        public void Dispose()
        {
            _botClient.StopReceiving();
        }
    }
}
