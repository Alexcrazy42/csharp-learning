using System.Diagnostics;

namespace Concurrency.Chapter4.ParallelBasics;

internal class Chapter4Class : IChapter
{
    public async Task Execute()
    {
        var list = GenetateRandomCollection(1_000_000_000_000_000_000);
        Stopwatch stopwatch = new Stopwatch();
        
        stopwatch.Start();

        var sum = Sum(list);
        Console.WriteLine(sum);
        stopwatch.Stop();
        Console.WriteLine($"Sum: {stopwatch.ElapsedMilliseconds}");

        stopwatch.Restart();
        var parallelSum = ParallelSum(list);
        Console.WriteLine(parallelSum);
        stopwatch.Stop();
        Console.WriteLine($"ParallelSum: {stopwatch.ElapsedMilliseconds}");


    }

    public IReadOnlyCollection<int> GenetateRandomCollection(long count)
    {
        var list = new List<int>();
        var rand = new Random();
        list.Add(rand.Next());
        return list;
                
    }

    public int Sum(IEnumerable<int> values)
    {
        return values.Sum();
    }

    public int ParallelSum(IEnumerable<int> values)
    {
        return values.AsParallel().Aggregate(
            seed: 0,
            func: (sum, item) => sum + item
            );
    }
}
