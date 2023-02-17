using BingBot.Core.Common;
using BingBot.Core.Message;

namespace BingBot.Core.Executor;

[Serializable]
internal class BingChatExecutor : ExecutorBase
{
    public BingChatExecutor(MessageInfo info) : base(info) { }

    internal static bool IsEnabled { get; set; }

    [CommandPrefix("/enable bing")]
    private MessageChain? Enable()
    {
        if (!Info.MasterCheck()) return null;
        IsEnabled = true;
        return "Bing Chat Enabled";
    }

    [CommandPrefix("/disable bing")]
    private MessageChain? Disable()
    {
        if (!Info.MasterCheck()) return null;
        IsEnabled = false;
        return "Bing Chat Disabled";
    }

    [CommandPrefix("/newchat")]
    private MessageChain NewChat()
    {
        BingChatHelper.Create();
        return "Bing Chat Conversation Recreated";
    }

    [CommandPrefix("/bing")]
    private async Task<MessageChain?> BingChat()
    {
        if (!IsEnabled) return null;

        // Create a task completion source, and add it to the queue.
        var tcs = new TaskCompletionSource<string>();
        BingChatHelper.SetTask(tcs, string.Join(" ", Command));

        await Info.SendMessage(RobotReply.Querying + "\n目前有 " + BingChatHelper.Tasks.Count + " 个问题在排队");

        // Wait for the task completion source to be completed.
        var result = await tcs.Task;
        return result;
    }

    private static Action<Task<string>> ContinuationFunction(TaskCompletionSource<string> tcs)
        => t =>
        {
            if (t.IsFaulted)
            {
                ExceptionLogger.Log(t.Exception!);
                tcs.SetException(t.Exception!);
            }
            else
            {
                tcs.SetResult(t.Result);
            }
        };
}
