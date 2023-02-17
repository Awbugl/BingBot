using Konata.Core.Message;

namespace BingBot.Core.Message;

[Serializable]
public class ReplyMessage : IMessage
{
    public readonly MessageStruct Message;

    internal ReplyMessage(MessageStruct message)
    {
        Message = message;
    }
}
