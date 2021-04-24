using System;
using System.Threading.Tasks;

namespace CommandHandler.Interfaces
{
    public abstract class TextCommandHandler : ITextCommandHandler
    {
        private ITextCommandHandler _nextHandler;
        public string Command { get; protected set; }
        public string Name { get; protected set; }

        public ITextCommandHandler SetNext(ITextCommandHandler handler)
        {
            return _nextHandler = handler;
        }

        public virtual Task<string> Handle(long chatId, string request)
        {
            if (_nextHandler == null)
            {
                return Task.FromResult((string)null);
            }
            return _nextHandler.Handle(chatId, request);
        }

        protected bool IsCommand(string command)
            => string.Equals(Command, command, StringComparison.OrdinalIgnoreCase) &&
               !string.IsNullOrEmpty(Command);

    }
}
