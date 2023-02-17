using Newtonsoft.Json;

namespace BingBot.Core.Common;

public static class GlobalConfig
{
    public static RobotReply RobotReply = JsonConvert.DeserializeObject<RobotReply>(File.ReadAllText(Path.RobotReply))!;

    internal static void Init(string file)
    {
        switch (file)
        {
            case "replytemplate.json":
                RobotReply = JsonConvert.DeserializeObject<RobotReply>(File.ReadAllText(Path.RobotReply))!;
                return;
        }
    }
}
