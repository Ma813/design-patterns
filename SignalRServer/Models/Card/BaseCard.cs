namespace SignalRServer.Models;

public abstract class BaseCard
{
    public string Color { get; protected set; }
    public string Name { get; protected set; }

    public BaseCard(string color, string name)
    {
        Color = color;
        Name = name;
    }

    public abstract void Play(AbstractGame game);

    public abstract bool CanPlay(BaseCard card);
}

public enum Colors
{
    Red,
    Green,
    Blue,
    Yellow
}

public enum ActionTypes
{
    Skip,
    Reverse
}