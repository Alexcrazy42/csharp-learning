
using System.Diagnostics;

namespace Concurrency.Chapter14.Scenarios;

internal class Chapter14Class : IChapter
{
    static int _simpleValue;
    private static AsyncLocal<Guid> _operationId = new AsyncLocal<Guid>();


    static readonly AsyncLazy<int> MySharedAsyncInteger =
     new AsyncLazy<int>(() => Task.Run(async () =>
     {
         await Task.Delay(TimeSpan.FromSeconds(2));
         return _simpleValue++;
     }));

    async Task GetSharedIntegerAsync()
    {
        int sharedValue = await MySharedAsyncInteger.Task;
    }

    async Task DoLongOperationAsync()
    {
        _operationId.Value = Guid.NewGuid();
        await DoSomeStepOfOperationAsync();
    }
    async Task DoSomeStepOfOperationAsync()
    {
        await Task.Delay(100); // Некоторая асинхронная работа
                               // Вывод в журнал.
        Trace.WriteLine("In operation: " + _operationId.Value);
    }


    public Task Execute()
    {
        Lazy<MyClass> lazyInstance = new Lazy<MyClass>(() => new MyClass());

        // Объект MyClass будет создан только при первом обращении
        Console.WriteLine("Объект еще не создан");
        MyClass instance = lazyInstance.Value; // Здесь произойдёт создание объекта
        Console.WriteLine("Объект создан");

        return Task.CompletedTask;
    }
}

public class MyClass
{
    public MyClass()
    {
        Console.WriteLine("Конструктор MyClass вызван");
    }
}

public sealed class AsyncLazy<T>
{
    private readonly object _mutex;
    private readonly Func<Task<T>> _factory;
    private Lazy<Task<T>> _instance;
    
    public AsyncLazy(Func<Task<T>> factory)
    {
        _mutex = new object();
        _factory = RetryOnFailure(factory);
        _instance = new Lazy<Task<T>>(_factory);
    }

    private Func<Task<T>> RetryOnFailure(Func<Task<T>> factory)
    {
        return async () =>
        {
            try
            {
                return await factory().ConfigureAwait(false);
            }
            catch
            {
                lock (_mutex)
                {
                    _instance = new Lazy<Task<T>>(_factory);
                }
                throw;
            }
        };
    }
    public Task<T> Task
    {
        get
        {
            lock (_mutex)
                return _instance.Value;
        }
    }
}