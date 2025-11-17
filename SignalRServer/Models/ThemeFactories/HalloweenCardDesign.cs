namespace SignalRServer.Models.ThemeFactories;

public class HalloweenCardDesign : ICardDesign
{
    public object GetDesignInfo()
    {
        return new Dictionary<string, string>
        {
            { "red", "#e8992a" },
            { "green", "#ced340" },
            { "yellow",  "#4bc13b" },
            { "blue",  "#65577e" }
        };
    }
}

public class HalloweenBackground : IBackground
{
    public string GetBackgroundInfo() => "#2A001E";
}

public class HalloweenSoundEffect : ISoundEffect
{
    public string GetSoundInfo() => "horror.mp3";
}

public class HalloweenThemeFactory : IUnoThemeFactory
{
    public ICardDesign CreateCardDesign() => new HalloweenCardDesign();
    public IBackground CreateBackground() => new HalloweenBackground();
    public ISoundEffect CreateSoundEffect() => new HalloweenSoundEffect();
}
