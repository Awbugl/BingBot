using BingBot.Core.Common;
using BingBot.Core.Message;

namespace BingBot.Core.Executor;

[Serializable]
internal class BingChatExecutor : ExecutorBase
{
    public BingChatExecutor(MessageInfo info) : base(info) { }

    internal static bool IsEnabled { get; set; }

    [CommandPrefix("/enable bing")]
    private MessageChain Enable()
    {
       // if (!Info.MasterCheck()) return null;
        IsEnabled = true;
        return "Bing Chat Enabled";
    }

    [CommandPrefix("/disable bing")]
    private MessageChain Disable()
    {
       // if (!Info.MasterCheck()) return null;
        IsEnabled = false;
        return "Bing Chat Disabled";
    }

    [CommandPrefix("/bing")]
    private async Task<MessageChain?> BingChat()
    {
        if (!IsEnabled) return null;
        await Info.SendMessage(RobotReply.Querying);
        return await BingChatHelper.GetBingChat(string.Join(" ", Command));
    }
}
