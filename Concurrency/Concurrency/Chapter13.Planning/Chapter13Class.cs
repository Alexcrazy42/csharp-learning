
namespace Concurrency.Chapter13.Planning;

internal class Chapter13Class : IChapter
{
    public async Task Execute()
    {
        Task<int> task = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            return 13;
        });
        Console.WriteLine(await task);
    }
}