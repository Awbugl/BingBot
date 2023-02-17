using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;

namespace BingBot.Core.Common;

public static class BingChatHelper
{
    internal static readonly BlockingCollection<(string, TaskCompletionSource<string>)> Tasks = new();

    private static BingChatConversation? _conversation;
    private static string? _chatCookie;

    public static void Init(string? bingChatCookie)
    {
        _chatCookie = bingChatCookie;
        Create();
        
        var thread = new Thread(() =>
        {
            while (!Tasks.IsCompleted)
            {
                (string, TaskCompletionSource<string>) msg = Tasks.Take();
                ExecuteTask(msg);
            }
        }) { IsBackground = true };
        thread.Start();
    }

    internal static void Create()
    {
        CreateConversation().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                ExceptionLogger.Log(t.Exception!);
                return;
            }

            _conversation = t.Result;
        });
    }

    internal static void SetTask(TaskCompletionSource<string> tcs, string message)
    {
        if (_conversation == null)
        {
            tcs.SetResult("Bing Chat Not Ready");
            return;
        }

        Tasks.Add((message, tcs));
    }

    private static void ExecuteTask((string, TaskCompletionSource<string>) message)
    {
        _conversation?.AskAsync(message.Item1).ContinueWith((Action<Task<string>>)(t =>
                                                                                      {
                                                                                          if (t.IsFaulted)
                                                                                          {
                                                                                              ExceptionLogger.Log(t.Exception!);
                                                                                              message.Item2.SetException(t.Exception!);
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              message.Item2.SetResult(t.Result);
                                                                                          }
                                                                                      })).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Create a chat conversation, so we can chat multiple times in the same context.
    /// </summary>
    private static async Task<BingChatConversation> CreateConversation()
    {
        var requestId = Guid.NewGuid();

        var cookies = new CookieContainer();
        cookies.Add(new Uri("https://www.bing.com"), new Cookie("_U", _chatCookie));

        using var handler = new HttpClientHandler { CookieContainer = cookies };
        using var client = new HttpClient(handler);
        var headers = client.DefaultRequestHeaders;

        headers.Add("accept-language", "en-US,en;q=0.9");
        headers.Add("sec-ch-ua", "\"Not_A Brand\";v=\"99\", \"Microsoft Edge\";v=\"109\", \"Chromium\";v=\"109\"");
        headers.Add("sec-ch-ua-arch", "\"x86\"");
        headers.Add("sec-ch-ua-bitness", "\"64\"");
        headers.Add("sec-ch-ua-full-version", "\"109.0.1518.78\"");
        headers.Add("sec-ch-ua-full-version-list",
                    "\"Not_A Brand\";v=\"99.0.0.0\", \"Microsoft Edge\";v=\"109.0.1518.78\", \"Chromium\";v=\"109.0.5414.120\"");
        headers.Add("sec-ch-ua-mobile", "?0");
        headers.Add("sec-ch-ua-model", "");
        headers.Add("sec-ch-ua-platform", "\"macOS\"");
        headers.Add("sec-ch-ua-platform-version", "\"12.6.0\"");
        headers.Add("sec-fetch-dest", "empty");
        headers.Add("sec-fetch-mode", "cors");
        headers.Add("sec-fetch-site", "same-origin");
        headers.Add("x-edge-shopping-flag", "1");
        headers.Add("x-ms-client-request-id", requestId.ToString().ToLower());
        headers.Add("x-ms-useragent", "azsdk-js-api-client-factory/1.0.0-beta.1 core-rest-pipeline/1.10.0 OS/MacIntel");
        headers.Add("referer", "https://www.bing.com/search");
        headers.Add("referer-policy", "origin-when-cross-origin");

        var response = await client.GetFromJsonAsync<BingCreateConversationResponse>("https://www.bing.com/turing/conversation/create");

        return new BingChatConversation(response!.ClientId, response.ConversationId, response.ConversationSignature);
    }

    internal sealed class BingCreateConversationResponse
    {
        public string ConversationId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ConversationSignature { get; set; } = null!;
    }
}
