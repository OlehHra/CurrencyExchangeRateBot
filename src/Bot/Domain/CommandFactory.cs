using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bot.Settings;
using CommandHandler.Interfaces;
using ExchangeRateReader.Interfaces;
using ExchangeRateReader.Interfaces.Modes;
using Microsoft.Extensions.Logging;

namespace Bot.Domain
{
    interface ICommandFactory
    {
        ITextCommandHandler CreateTextCommandHandler();
    }

    class CommandFactory : ICommandFactory
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;

        private List<ExchangeRateReaderBase> _rateReaderBases = new List<ExchangeRateReaderBase>();
        private List<IExchangeRateReader> _rateReaders = new List<IExchangeRateReader>();

        public CommandFactory(ILogger logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }


        
        public ITextCommandHandler CreateTextCommandHandler()
        {
            LoadReaders();

            var bankSettings = _appSettings.BankSettings
                .Where(x => x.Enabled)
                .Select(x => x.Name).ToList();

            ITextCommandHandler root = null;
            ITextCommandHandler last = null;
            foreach (var type in GetFilesStartsWith("CommandHandler.", typeof(TextCommandHandler))
                .OrderBy(x=> bankSettings.IndexOf(GetName(x))))
            {
                try
                {
                    var name = GetName(type);

                    var reader = _rateReaders.SingleOrDefault(x =>
                        string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

                    if (reader == null)
                    {
                        throw new Exception($"There is no reader for '{name}'");
                    }

                    ITextCommandHandler instance = (ITextCommandHandler) Activator.CreateInstance(type, reader);
                    if (root == null)
                    {
                        root = last = instance;
                    }
                    else
                    {
                        last = last.SetNext(instance);
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }

            return root;
        }

        private void LoadReaders()
        {
            foreach (var type in GetFilesStartsWith("BankReader.", typeof(ExchangeRateReaderBase)))
            {
                try
                {
                    var name = GetName(type);

                    var bankSetting = _appSettings.BankSettings
                        .SingleOrDefault(x =>
                            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

                    if (bankSetting == null)
                    {
                        _logger.LogError($"There is no settings for '{name}'");
                    }
                    else if (bankSetting.Enabled)
                    {
                        var reader = (IExchangeRateReader)Activator.CreateInstance(type,
                            _logger,
                            new CurrencyExchangeRateConfig
                            {
                                ApiUrl = bankSetting.ApiUrl,
                                Url = bankSetting.Url,
                                UpdateInterval = TimeSpan.FromSeconds(_appSettings.ExchangeUpdateIntervalSec)
                            });
                        _rateReaders.Add(reader);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
            }
        }


        private IEnumerable<Type> GetFilesStartsWith(string startWith, Type baseType)
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                           ?? string.Empty, "*.dll");

            foreach (var file in files)
            {
                if (Path.GetFileName(file).StartsWith(startWith, StringComparison.OrdinalIgnoreCase))
                {

                    var assembly = Assembly.LoadFile(file);
                    var handler = assembly.GetTypes().SingleOrDefault(x => x.IsClass && x.BaseType == baseType);
                    if (handler == null) continue;
                    yield return handler;
                }
            }

            yield break;
        }

        private static string GetName(Type type)
        {
            var fileName = Path.GetFileNameWithoutExtension(type.Assembly.Location);
            var index = fileName.LastIndexOf(".", StringComparison.Ordinal);

            if (index >= 0)
            {
                return fileName.Substring(index + 1);
            }

            throw new Exception($"Unsupported assembly name '{fileName}'");
        }
    }
}
