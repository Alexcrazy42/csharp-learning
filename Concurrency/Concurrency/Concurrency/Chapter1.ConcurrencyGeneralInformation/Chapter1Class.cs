namespace Concurrency.Chapter1.ConcurrencyGeneralInformation;

internal class Chapter1Class : IChapter
{
    public async Task Execute()
    {
        var t = NotDeadlock();
        Console.WriteLine("After 2");
        await t;
    }

    async Task WaitAsync()
    {
        // await сохранит текущий контекст ...
        await Task.Delay(TimeSpan.FromSeconds(10));
        // ... и попытается возобновить метод в этой точке с этим контекстом.
        Console.WriteLine("After");
    }

    void Deadlock()
    {
        // Начать задержку.
        Task task = WaitAsync();
        // Синхронное блокирование с ожиданием завершения async-метода.
        task.Wait();
    }

    async Task NotDeadlock()
    {
        await WaitAsync();
    }
}
