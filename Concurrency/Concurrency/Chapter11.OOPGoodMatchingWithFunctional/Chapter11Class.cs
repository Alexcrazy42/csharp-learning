using Nito.AsyncEx;
using System.Runtime.InteropServices;
using static Concurrency.Chapter11.OOPGoodMatchingWithFunctional.Chapter11Class;

namespace Concurrency.Chapter11.OOPGoodMatchingWithFunctional;

internal class Chapter11Class : IChapter
{
    public async Task Execute()
    {
        // await AsyncVoidMethod(); // ожидание void невозможно

        // не работает с try, не получается перехватить ошибку
        //try
        //{
        //    AsyncVoidMethod();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Что то пошло не так!");
        //}


        var myClass = new MyClass();
        myClass.MyEvent += async (sender, e) =>
        {
            using (e.GetDeferral())
            {
                await Task.Delay(2000); // Имитация асинхронной операции
                Console.WriteLine("Асинхронная операция завершена.");
            }
        };

        await myClass.TriggerEventAsync();
    }

    async void AsyncVoidMethod()
    {
        await Task.Delay(1000); // Имитация асинхронной работы
        Console.WriteLine("AsyncVoidMethod завершён.");
        throw new Exception("Что-то пошло не так!"); // Исключение
    }
}

public class MyClass
{
    public event EventHandler<MyEventArgs> MyEvent;

    public async Task TriggerEventAsync()
    {
        var args = new MyEventArgs();
        MyEvent?.Invoke(this, args);

        await args.WaitForDeferralsAsync(); // Ждем завершения всех отложений
        Console.WriteLine("Все операции завершены.");
    }
}

public class MyEventArgs : EventArgs, IDeferralSource
{
    private readonly DeferralManager deferrals = new DeferralManager();

    public IDisposable GetDeferral()
    {
        return deferrals.GetDeferral();
    }

    internal Task WaitForDeferralsAsync()
    {
        return deferrals.WaitForDeferralsAsync();
    }
}

public class DeferralManager
{
    private readonly List<IDisposable> deferrals = new List<IDisposable>();

    public DeferralSource deferralSource { get; } = new DeferralSource();

    public IDisposable GetDeferral()
    {
        var deferral = deferralSource.GetDeferral();
        deferrals.Add(deferral);
        return new DeferralWrapper(deferral, this);
    }

    public async Task WaitForDeferralsAsync()
    {
        foreach (var deferral in deferrals)
        {
            await Task.Yield(); // Симуляция ожидания
        }
    }

    private void ReleaseDeferral(IDisposable deferral)
    {
        deferrals.Remove(deferral);
    }

    private class DeferralWrapper : IDisposable
    {
        private readonly IDisposable deferral;
        private readonly DeferralManager manager;

        public DeferralWrapper(IDisposable deferral, DeferralManager manager)
        {
            this.deferral = deferral;
            this.manager = manager;
        }

        public void Dispose()
        {
            deferral.Dispose();
            manager.ReleaseDeferral(deferral);
        }
    }

    public class DeferralSource
    {
        public IDisposable GetDeferral() => new Deferral();
    }

    private class Deferral : IDisposable
    {
        public void Dispose() { /* Логика освобождения ресурсов */ }
    }
}