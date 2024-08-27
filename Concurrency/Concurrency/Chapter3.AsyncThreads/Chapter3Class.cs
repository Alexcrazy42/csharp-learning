namespace Concurrency.Chapter3.AsyncThreads;

internal class Chapter3Class : IChapter
{
    public async Task Execute()
    {
        foreach (var number in await GetNumbersAsync())
        {
            Console.WriteLine(number);
        }
        await foreach (var number in GetNumbersAsyncWithAsyncEnumerable())
        {
            Console.WriteLine(number);
        }
    }

    public async Task<IReadOnlyCollection<int>> GetNumbersAsync()
    {
        var result = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            
            await Task.Delay(500);
            result.Add(i);
        }
        return result;
    }

    public async IAsyncEnumerable<int> GetNumbersAsyncWithAsyncEnumerable()
    {
        for (int i = 0; i < 6; i++)
        {
            await Task.Delay(500);
            yield return i;
        }
    }
}
