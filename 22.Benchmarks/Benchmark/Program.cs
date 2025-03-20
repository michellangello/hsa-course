using System.Diagnostics;
using System.Globalization;

namespace Benchmark;

class Program
{
    static void Main(string[] args)
    {
        MeasureSpaceComplexity();
        MeasureAverageInsertTime();
    }

    static void MeasureAverageInsertTime()
    {
        using StreamWriter writer = new StreamWriter("time_log.tsv");
        writer.WriteLine("Size\tInsertTime(ns)\tSearchTime(ns)\tDeleteTime(ns)");

        var repetitions = 100;

        for (var size = 1; size <= 10000; size += size < 100 ? 1 : 100)
        {
            var totalInsertTime = 0d;
            var totalFindTime = 0d;
            var totalDeleteTime = 0d;
            var totalOps = 0;

            //pre-heat
            var avl1 = new AVLTree();
            for (var i = 0; i < 100; i++)
            {
                avl1.Insert(100 * i);
                avl1.Find(100 * i);
                avl1.Delete(1000 * i);
            }

            for (var rep = 0; rep < repetitions; rep++)
            {
                var avl = new AVLTree();
                for (var i = 0; i < size; i++)
                {
                    totalInsertTime += MeasureExecutionTime(() => avl.Insert(Random.Shared.Next(1_000_000)));
                    totalFindTime += MeasureExecutionTime(() => avl.Find(Random.Shared.Next(1_000_000)));
                    totalDeleteTime += MeasureExecutionTime(() => avl.Delete(Random.Shared.Next(1_000_000)));
                    totalOps++;
                }
            }


            var avgInsertTime = totalInsertTime / totalOps;
            var avgFindTime = totalFindTime / totalOps;
            var avgDeleteTime = totalDeleteTime / totalOps;

            var insertTime = avgInsertTime.ToString("F0", CultureInfo.InvariantCulture);
            var findTime = avgFindTime.ToString("F0", CultureInfo.InvariantCulture);
            var deleteTime = avgDeleteTime.ToString("F0", CultureInfo.InvariantCulture);

            writer.WriteLine($"{size}\t{insertTime}\t{findTime}\t{deleteTime}");
            Console.WriteLine(
                $"Size: {size} - Avg Insert Time: {insertTime}ns,  Avg Find Time: {findTime} ns,  Avg Delete Time: {deleteTime}ns");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }


    private static void MeasureSpaceComplexity()
    {
        var random = new Random();
        var avl = new AVLTree();
        var values = Enumerable.Range(1, 6_000_000).OrderBy(_ => random.Next()).ToList();

        using var writer = new StreamWriter("space_log.csv");
        writer.WriteLine("InsertionIndex,MemoryUsage(MB)");

        var memoryBefore = GC.GetTotalMemory(true);

        for (var i = 0; i < values.Count; i++)
        {
            avl.Insert(values[i]);

            if (i % 500 == 0)
            {
                var memoryAfter = GC.GetTotalMemory(true);
                var memoryUsed = (memoryAfter - memoryBefore) / (1024.0 * 1024.0);
                writer.WriteLine($"{i},{memoryUsed.ToString("F2", CultureInfo.InvariantCulture)}");
            }
        }

        Console.WriteLine("Space complexity logged in space_log.csv.");
    }


    private static double MeasureExecutionTime(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed.TotalNanoseconds;
    }
}
