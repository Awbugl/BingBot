using BingBot.Core.Common;
using BingBot.Core.Message;
using Konata.Core.Interfaces.Api;

namespace BingBot.Core.Executor;

[Serializable]
internal class PermissionExecutor : ExecutorBase
{
    public PermissionExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/echo")]
    private async Task<MessageChain?> Echo()
    {
        if (Info.MasterCheck()) await Info.SendMessageOnly(string.Join(" ", Command));
        return null;
    }

    [CommandPrefix("/remove")]
    private async Task<MessageChain?> RemoveGroup()
    {
        if (!Info.MasterCheck()) return null;
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        await Info.Bot.GroupLeave(uint.Parse(Command[0]));
        return "Removed.";
    }
}
