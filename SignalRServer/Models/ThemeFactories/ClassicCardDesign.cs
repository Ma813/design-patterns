namespace SignalRServer.Models.ThemeFactories;

public class ClassicCardDesign : ICardDesign
{
    public object GetDesignInfo()
    {
        return new Dictionary<string, string>
        {
            { "red", "#FF4C4C" },
            { "green", "#4CFF4C" },
            { "yellow", "#FFFF66" },
            { "blue", "#4C4CFF" }
        };
    }
}

public class ClassicBackground : IBackground
{
    public string GetBackgroundInfo() => "#1E1E1E";
}

public class ClassicSoundEffect : ISoundEffect
{
    public string GetSoundInfo() => "annoying.mp3";
}

public class ClassicThemeFactory : IUnoThemeFactory
{
    public ICardDesign CreateCardDesign() => new ClassicCardDesign();
    public IBackground CreateBackground() => new ClassicBackground();
    public ISoundEffect CreateSoundEffect() => new ClassicSoundEffect();
}
