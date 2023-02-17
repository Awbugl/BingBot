using BingBot.Core.Common;
using BingBot.Core.Message;
using BingBot.Core.Utils;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message.Model;

namespace BingBot.Core.Executor;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

[Serializable]
internal class OtherExecutor : ExecutorBase
{
    public OtherExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/help", "/arc help")]
    private MessageChain HelpMessage() => CommandLength == 0 ? RobotReply.HelpMessage : null;

    [CommandPrefix("/dismiss")]
    private async Task<MessageChain?> Dismiss()
    {
        if (Info.Message.Chain.FindChain<AtChain>()?.Any(i => i.AtUin == Info.Bot.Uin) != true) return null;
        if (!IsGroup) return null;
        if (!Info.MasterCheck() && !await Info.PermissionCheck()) return null;

        await Info.SendMessage(RobotReply.GroupLeave);
        await Task.Delay(5000);
        await Bot.GroupLeave(Info.FromGroup);

        return null;
    }

    [CommandPrefix("/geterr")]
    private static MessageChain ExceptionReport() => LastExceptionHelper.GetDetails();

    [CommandPrefix("/state")]
    private static MessageChain Statement() => BotStatementHelper.Statement;
}
