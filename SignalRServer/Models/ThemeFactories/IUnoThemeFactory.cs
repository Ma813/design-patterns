namespace SignalRServer.Models.ThemeFactories;

public interface ICardDesign
{
    object GetDesignInfo();
}

public interface IBackground
{
    string GetBackgroundInfo();
}

public interface ISoundEffect
{
    string GetSoundInfo();
}

public interface IUnoThemeFactory
{
    ICardDesign CreateCardDesign();
    IBackground CreateBackground();
    ISoundEffect CreateSoundEffect();
}
