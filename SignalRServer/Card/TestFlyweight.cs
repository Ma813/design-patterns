using System.Diagnostics;
using SignalRServer.Card;

namespace SignalRServer.Card;

public static class PerformanceTester
{
    public static void Run()
    {
        List<int> iterationsList = new() { 1_000_000, 2_000_000, 5_000_000, 10_000_000, 50_000_000 };

        foreach (int iterations in iterationsList)
        {
            Console.WriteLine("=== UNO CARD PERFORMANCE TEST ===");
            Console.WriteLine($"Iterations: {iterations:N0}");
            Console.WriteLine();

            Test("Normal Card Generation", () => UnoCard.GenerateCard(useFlyweight: false), iterations);
            Test("Flyweight Card Generation", () => UnoCard.GenerateCard(useFlyweight: true), iterations);
        }
    }


    private static void Test(string name, Func<UnoCard> generator, int iterations)
    {
        Console.WriteLine($"--- {name} ---");

        // Force a GC before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long beforeMemory = GC.GetTotalMemory(true);

        var sw = Stopwatch.StartNew();

        var cards = new List<UnoCard>(iterations);

        for (int i = 0; i < iterations; i++)
        {
            cards.Add(generator());
        }

        sw.Stop();

        long afterMemory = GC.GetTotalMemory(false);

        Console.WriteLine($"Time:   {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Memory: {(afterMemory - beforeMemory) / 1024.0 / 1024.0:F2} MB used");
        Console.WriteLine();
    }
}