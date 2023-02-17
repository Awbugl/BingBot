﻿using Path = BingBot.Core.Common.Path;

namespace BingBot.Core.Message;

[Serializable]
public class ImageMessage : IMessage
{
    private readonly string _path;

    private ImageMessage(string path)
    {
        _path = path;
    }

    public override string ToString() => _path;

    internal static ImageMessage FromPath(Common.Path path) => new(path);

    /*
    public static implicit operator ImageMessage(Image value)
    {
        var pth = Path.RandImageFileName();
        value.SaveAsJpgWithQuality(pth);
        value.Dispose();
        return FromPath(pth);
    }

    public static implicit operator ImageMessage(BackGround value)
    {
        var pth = Path.RandImageFileName();
        value.SaveAsJpgWithQuality(pth);
        value.Dispose();
        return FromPath(pth);
    }
    */
}
