using System;

namespace TextMessageBuilder.Interfaces
{
    public interface ITextMessageBuilder
    {
        string Build<T>(T data);
    }
}
