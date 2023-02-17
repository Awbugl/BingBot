using BingBot.Core.Common;
using BingChat;

namespace BingBot.Core.Common;

public static class BingChatHelper
{
    private static IBingChattable? _conversation;
    private static string? _chatCookie;

    public static void Init(string? bingChatCookie)
    {
        _chatCookie = bingChatCookie;
        Create();
    }

    private static void Create()
    {
        var client = new BingChatClient(new() { Cookie = _chatCookie });

        client.CreateConversation().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                ExceptionLogger.Log(t.Exception!);
                return;
            }

            _conversation = t.Result;
        });
    }

    internal static async Task<string> GetBingChat(string message)
    {
        if (_conversation == null) return "Bing Chat Not Ready";

        try
        {
            var answer = await _conversation.AskAsync(message);
            return answer;
        }
        catch (Exception ex)
        {
            ExceptionLogger.Log(ex);
            Create();
            return "Bing Chat Error";
        }
    }
}
