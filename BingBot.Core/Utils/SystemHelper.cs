using System.Net;
using BingBot.Core.Common;

namespace BingBot.Core.Utils;

internal static class SystemHelper
{
    public static void Init(BingBotConfig bingBotConfig)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.ServerCertificateValidationCallback = (
            _,
            _,
            _,
            _) => true;
        
        ServicePointManager.DefaultConnectionLimit = 512;
        ServicePointManager.Expect100Continue = false;
        ServicePointManager.UseNagleAlgorithm = false;
        ServicePointManager.ReusePort = true;
        ServicePointManager.CheckCertificateRevocationList = true;
        WebRequest.DefaultWebProxy = null;

        BingChatHelper.Init(bingBotConfig.BingChatCookie);
        MessageInfo.Init(bingBotConfig.Master);
        ConfigWatcher.Init();
    }
}
