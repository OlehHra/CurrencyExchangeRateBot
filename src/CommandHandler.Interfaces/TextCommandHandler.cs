using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CommandHandler.Interfaces
{
    public abstract class TextCommandHandler : ITextCommandHandler
    {
        protected ILogger Logger { get; }
        private ITextCommandHandler _nextHandler;
        public string Command { get; protected set; }
        public string Name { get; protected set; }

        protected TextCommandHandler(ILogger logger)
        {
            Logger = logger;
        }

        public ITextCommandHandler SetNext(ITextCommandHandler handler)
        {
            return _nextHandler = handler;
        }

        public ITextCommandHandler GetNext()
        {
            return _nextHandler;
        }

        public virtual Task<string> Handle(long chatId, string request)
        {
            try
            {
                if (_nextHandler == null)
                {
                    return Task.FromResult((string)null);
                }
                return _nextHandler.Handle(chatId, request);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());

                return Task.FromResult("<b>От халепа!</b><pre>Під час обробки команди сталася помилка :(</pre>");
            }
        }

        protected bool IsCommand(string command)
            => string.Equals(Command, command, StringComparison.OrdinalIgnoreCase) &&
               !string.IsNullOrEmpty(Command);

    }
}
