namespace BingBot.Core.Common;

[Serializable]
public class Path
{
    private static readonly string BaseDirectory = AppContext.BaseDirectory;

    public static readonly string ConfigRoot = BaseDirectory + "/BingChatConfig/";

    public static readonly Path Config = new(ConfigRoot + "config.json"),
                                RobotReply = new(ConfigRoot + "replytemplate.json"),
                                ExceptionReport = new(BaseDirectory + "BingChatExceptionReport.log");

    private readonly string _rawpath;

    private FileInfo? _fileInfo;

    static Path()
    {
        Directory.CreateDirectory(ConfigRoot + "BotInfo/");
    }

    private Path(string rawpath)
    {
        _rawpath = rawpath;
    }

    public FileInfo FileInfo => _fileInfo ??= new(this);

    public static Path BotConfig(uint qqid) => new(ConfigRoot + $"BotInfo/{qqid}.botinfo");

    public static implicit operator string(Path path) => path._rawpath;
}
