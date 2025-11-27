using System.Diagnostics;

namespace SignalRServer.Card;

public static class PerformanceTester
{
    public static void Run()
    {
        List<int> iterationsList = new() { 1_000_000, 2_000_000, 5_000_000, 10_000_000, 50_000_000 };

        Console.WriteLine("=== UNO CARD PERFORMANCE TEST (TABLE FORMAT) ===");
        Console.WriteLine();

        // Table header
        Console.WriteLine(
            $"{ "Iterations",15} | { "Method",25} | { "Time (ms)",12} | { "Memory Used (MB)",18}"
        );
        Console.WriteLine(new string('-', 75));

        foreach (int iterations in iterationsList)
        {
            Test("Normal Generation", () => UnoCard.GenerateCard(useFlyweight: false), iterations);
            Test("Flyweight Generation", () => UnoCard.GenerateCard(useFlyweight: true), iterations);
        }
    }

    private static void Test(string label, Func<UnoCard> generator, int iterations)
    {
        // GC cleanup before test
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

        double memUsedMB = (afterMemory - beforeMemory) / 1024.0 / 1024.0;

        Console.WriteLine(
            $"{iterations,15:N0} | {label,25} | {sw.ElapsedMilliseconds,12} | {memUsedMB,18:F2}"
        );
    }
}