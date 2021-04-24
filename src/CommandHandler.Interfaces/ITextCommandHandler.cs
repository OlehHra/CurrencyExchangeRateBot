using System.Collections;
using System.Threading.Tasks;

namespace CommandHandler.Interfaces
{
    public interface ITextCommandHandler
    {
        string Command { get;  }
        string Name { get;  }
        ITextCommandHandler SetNext(ITextCommandHandler handler);
        ITextCommandHandler GetNext();
        Task<string> Handle(long chatId, string command);
    }
}
