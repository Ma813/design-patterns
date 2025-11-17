namespace SignalRServer.Card;

/*
Flyweight
To test performance, run in cmd:

dotnet run --benchmark
*/

public static class UnoCardFlyweightFactory
{
    private static readonly Dictionary<string, UnoCard> cache = new();   


    public static UnoCard GetNumberCard(string color, int digit)
    {
        string key = $"number-{color}-{digit}";

        if (!cache.ContainsKey(key))
        {
            cache[key] = new NumberCard(color, digit);
        }

        return cache[key];
    }

    public static UnoCard GetPowerCard(string color, string powerType)
    {
        string key = $"power-{color}-{powerType}";

        if (!cache.ContainsKey(key))
        {
            cache[key] = new PowerCard(color, powerType);
        }

        return cache[key];
    }
}