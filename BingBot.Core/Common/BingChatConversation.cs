using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text.Json;
using Websocket.Client;

namespace BingBot.Core.Common;

/// <summary>
/// A chat conversation, enables us to chat multiple times in the same context.
/// </summary>
internal sealed class BingChatConversation
{
    private const char TerminalChar = '\u001e';
    private readonly BingChatRequest _request;

    internal BingChatConversation(string clientId, string conversationId, string conversationSignature)
    {
        _request = new BingChatRequest(clientId, conversationId, conversationSignature);
    }

    public Task<string> AskAsync(string message)
    {
        var wsClient = new WebsocketClient(new Uri("wss://sydney.bing.com/sydney/ChatHub"));
        var tcs = new TaskCompletionSource<string>();

        void Cleanup()
        {
            wsClient.Stop(WebSocketCloseStatus.Empty, string.Empty).ContinueWith(t =>
            {
                if (t.IsFaulted) tcs.SetException(t.Exception!);
                wsClient.Dispose();
                wsClient = null;
            });
        }

        string? GetAnswer(BingChatConversationResponse response)
        {
            var responseItem = response.Item;
            
            if(responseItem.Result.Value != "Success") return responseItem.Result.Message;

            for (var index = responseItem.Messages.Length - 1; index >= 0; index--)
            {
                var itemMessage = responseItem.Messages[index];

                if (itemMessage.ContentOrigin == "TurnLimiter")
                {
                    return itemMessage.Text;
                }
                else
                {
                    if (itemMessage.MessageType != null) continue;
                    if (itemMessage.Author != "bot") continue;

                    // maybe is possible to use itemMessage.Text directly, but some extra information will be lost
                    return itemMessage.AdaptiveCards?[0].Body?[0].Text ?? itemMessage.Text;
                }
            }

            return null;
        }

        void MessageReceived(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            try
            {
                foreach (var part in text.Split(TerminalChar, StringSplitOptions.RemoveEmptyEntries))
                {
                    var json = JsonSerializer.Deserialize<BingChatConversationResponse>(part);

                    if (json is not { Type: 2 }) continue;

                    Cleanup();
                    ExceptionLogger.LogJson(part);

                    tcs.SetResult(GetAnswer(json) ?? "<empty answer>");
                    return;
                }
            }
            catch (Exception e)
            {
                Cleanup();

                tcs.SetException(e);
            }
        }

        wsClient.MessageReceived.Where(msg => msg.MessageType == WebSocketMessageType.Text).Select(msg => msg.Text).Subscribe(MessageReceived);

        // Start the WebSocket client
        wsClient.Start().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Cleanup();
                tcs.SetException(t.Exception!);
                return;
            }

            // Send initial messages
            wsClient.Send("{\"protocol\":\"json\",\"version\":1}" + TerminalChar);
            wsClient.Send(_request.ConstructInitialPayload(message) + TerminalChar);
        });

        return tcs.Task;
    }
}
