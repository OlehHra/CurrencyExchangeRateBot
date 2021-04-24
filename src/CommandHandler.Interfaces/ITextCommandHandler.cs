using System.Threading.Tasks;

namespace CommandHandler.Interfaces
{
    public interface ITextCommandHandler
    {
        string Command { get;  }
        string Name { get;  }
        ITextCommandHandler SetNext(ITextCommandHandler handler);
        Task<string> Handle(long chatId, string command);
    }
}
