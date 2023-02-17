using BingBot.Core.Message;
using Newtonsoft.Json;

namespace BingBot.Core.Common;

public class RobotReply
{
    [JsonProperty("ParameterLengthError")]
    public string ParameterLengthError { get; set; } = "";

    [JsonProperty("Querying")]
    public string Querying { get; set; } = "";
    
    [JsonProperty("SendMessageFailed")]
    public string SendMessageFailed { get; set; } = "";

    [JsonProperty("HelpMessage")]
    public string HelpMessage { get; set; } = "";

    [JsonProperty("GroupLeave")]
    public string GroupLeave { get; set; } = "";

    [JsonProperty("ExceptionOccured")]
    public string ExceptionOccured { get; set; } = "";

    internal TextMessage OnExceptionOccured(Exception ex) => ExceptionOccured.Replace("$exception$", ex.Message);
}
