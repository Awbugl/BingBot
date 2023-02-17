using System.Text.Json.Serialization;

// ReSharper disable MemberCanBeInternal
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable CS8618

namespace BingBot.Core.Common;

public sealed class BingChatConversationResponse
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("item")]
    public Item Item { get; set; }
}

public sealed class Item
{
    [JsonPropertyName("messages")]
    public BingChatMessage[] Messages { get; set; }

    [JsonPropertyName("result")]
    public BingChatResult Result { get; set; }
}

public sealed class BingChatResult
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}

public sealed class BingChatMessage
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; }

    [JsonPropertyName("contentOrigin")]
    public string? ContentOrigin { get; set; }

    [JsonPropertyName("adaptiveCards")]
    public BingChatAdaptiveCard[] AdaptiveCards { get; set; }
}

public sealed class BingChatAdaptiveCard
{
    [JsonPropertyName("body")]
    public BingChatBody[] Body { get; set; }
}

public sealed class BingChatBody
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("wrap")]
    public bool Wrap { get; set; }
}
