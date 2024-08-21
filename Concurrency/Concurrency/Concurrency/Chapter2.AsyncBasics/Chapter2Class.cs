namespace Concurrency.Chapter2.AsyncBasics;

public class Chapter2Class : IChapter
{
    public async Task Execute()
    {
        await CallMyMethodAsync();
    }

    async Task CallMyMethodAsync()
    {
        var progress = new Progress<double>();
        progress.ProgressChanged += (sender, args) =>
        {
            Console.WriteLine($"Status: {args}");
	    };

        await MyMethodAsync(progress);
    }

    async Task MyMethodAsync(IProgress<double> progress = null)
    {
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(100);
            progress?.Report(i);
        }
    }
}
