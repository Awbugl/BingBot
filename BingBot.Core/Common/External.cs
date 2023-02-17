using BingBot.Core.Utils;
using Konata.Core;
using Konata.Core.Message;

namespace BingBot.Core.Common;

[Serializable]
public static class External
{
    public static void Process(
        Bot bot,
        int type,
        uint fromGroup,
        uint fromQq,
        MessageStruct message)
        => MessageInfo.Process(bot, type, fromGroup, fromQq, message);

    public static void Initialize(BingBotConfig bingBotConfig) => SystemHelper.Init(bingBotConfig);
}
