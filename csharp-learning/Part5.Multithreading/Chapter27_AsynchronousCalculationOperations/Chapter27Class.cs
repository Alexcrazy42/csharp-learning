using System.Runtime.Remoting;
using System.Threading;

namespace csharp_learning.Part5.Multithreading.Chapter27_AsynchronousCalculationOperations;

public class Chapter27Class
{
    public void Execute()
    {
        // ПРИМЕР 1
        // Пример процедуры асинхронного вызова метода потоком из пула

        //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: queuing an asynchronous operation");
        //ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);
        //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Doing other work here...");
        //Thread.Sleep(10000); // Имитация другой работы (10 секунд)
        //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Work done");
        //Console.WriteLine("Hit <Enter> to end this program...");
        //Console.ReadLine();


        // ПРИМЕР 2
        // Работа с CancellationToken

        //CancellationTokenSource cts = new CancellationTokenSource();

        //// передаем операции CancellationToken и число
        //ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 1000));
        //Console.ReadLine();
        //cts.Cancel();

        //// Cancel немедленно возвращает управление, метод продолжает работу

        //Console.ReadLine();

        // ПРИМЕР 3
        // связывание CancellationTokenSource

        var cts1 = new CancellationTokenSource();
        cts1.Token.Register(() => Console.WriteLine("cts1 canceled"));

        var cts2 = new CancellationTokenSource();
        cts2.Token.Register(() => Console.WriteLine("cts2 canceled"));

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
        linkedCts.Token.Register(() => Console.WriteLine("linkedCts canceled"));

        cts2.Cancel();

        Console.WriteLine("cts1 canceled={0}, cts2 canceled={1}, linkedCts={2}",
         cts1.IsCancellationRequested, cts2.IsCancellationRequested,
         linkedCts.IsCancellationRequested);

    }

    private static void ComputeBoundOp(Object state)
    {
        // Метод выполняется потоком из пула
        Thread.Sleep(5000); // Имитация другой работы (1 секунда)
                            // После возвращения управления методом поток
                            // возвращается в пул и ожидает следующего задания
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: In ComputeBoundOp: state={0}", state);
    }

    private void Count(CancellationToken token, int countTo)
    {
        for (int count = 0; count < countTo; count++)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Count is cancelled!");
                break;
            }
            Console.WriteLine(count);
            Thread.Sleep(200);
        }
        Console.WriteLine("Count is done");
    }
}
