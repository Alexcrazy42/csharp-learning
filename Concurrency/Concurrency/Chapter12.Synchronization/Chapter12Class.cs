
using Nito.AsyncEx;

namespace Concurrency.Chapter12.Synchronization;

internal class Chapter12Class : IChapter
{
    public async Task Execute()
    {

        //var myClass = new MyClass();
        //Task.Run(async () =>
        //{
        //    await myClass.DelayAndIncrementAsync();
        //});

        //Task.Run(async () =>
        //{
        //    await myClass.DelayAndIncrementAsync();
        //});

        //await Task.Delay(10);

        //Console.WriteLine(myClass.Value);

        var myClass = new MyClass1();

        // Создаем поток, который будет инициализировать объект
        var initThread = new Thread(myClass.InitializeFromAnotherThread);
        initThread.Start();

        // В основном потоке ожидаем инициализации
        int value = myClass.WaitForInitialization();
        Console.WriteLine($"Value after initialization: {value}");

        // Дожидаемся завершения потока инициализации
        initThread.Join();
    }
}


class MyClass
{
    // Блокировка защищает поле _value.
    private readonly AsyncLock _mutex = new AsyncLock();
    private int _value;

    public int Value { get { return _value; } }
    public async Task DelayAndIncrementAsync()
    {
        using (await _mutex.LockAsync())
        {
            int oldValue = _value;
            await Task.Delay(TimeSpan.FromSeconds(oldValue));
            _value = oldValue + 1;
        }
    }
}

public class MyClass1
{
    private readonly ManualResetEventSlim _initialized = new ManualResetEventSlim();

    private int _value;

    public int WaitForInitialization()
    {
        _initialized.Wait();
        return _value;
    }

    public void InitializeFromAnotherThread()
    {
        _value = 13;
        _initialized.Set();
    }
}