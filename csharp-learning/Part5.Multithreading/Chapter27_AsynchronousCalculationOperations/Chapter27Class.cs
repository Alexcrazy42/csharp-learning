using System.Net.Http.Headers;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;

namespace csharp_learning.Part5.Multithreading.Chapter27_AsynchronousCalculationOperations;

public class Chapter27Class
{
    private static Timer timer;

    public void Execute()
    {
        // ПРИМЕР 1
        // Пример процедуры асинхронного вызова метода потоком из пула

        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: queuing an asynchronous operation");
        ThreadPool.QueueUserWorkItem(ComputeBoundOp, 5);
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Doing other work here...");
        Thread.Sleep(10000); // Имитация другой работы (10 секунд)
        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}: Work done");
        Console.WriteLine("Hit <Enter> to end this program...");
        Console.ReadLine();


        // ПРИМЕР 2
        // Работа с CancellationToken

        CancellationTokenSource cts = new CancellationTokenSource();

        // передаем операции CancellationToken и число
        ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 1000));
        Console.ReadLine();
        cts.Cancel();

        // Cancel немедленно возвращает управление, метод продолжает работу

        Console.ReadLine();

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

        // ПРИМЕР 4.
        // Отмена заданий через CancellationToken

        var cts = new CancellationTokenSource();
        Task<Int32> t = new Task<Int32>(() => Sum(cts.Token, 10000), cts.Token);

        t.Start();

        // отменим cts, чтобы отменить Task
        // это асинхронный запрос, задача уже может быть завершена
        cts.Cancel();

        try
        {
            // в случае отмены задания метод Result генерирует 
            // исключение AggregateException
            Console.WriteLine($"The sum is: {t.Result}");
        }
        catch (AggregateException ex)
        {
            // считаем обработанным все объекты OperationCanceledException
            // все остальные исключения попадают в новый объекты AggregateException,
            // состоящие только из необработанных исключений
            ex.Handle(e => e is OperationCanceledException);

            Console.WriteLine("Sum was canceled");
        }

        // ПРИМЕР 5.
        // Использование цепочки ContinueWith и enum TaskContinuationOptions

        // Создание и запуск задания с продолжением
        CancellationToken ct = new CancellationToken();
        Task<Int32> t1 = new Task<Int32>(() => Sum(ct, 10000));

        t1.Start();


        // Метод ContinueWith возвращает объект Task, но обычно
        // он не используется
        t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result),
         TaskContinuationOptions.OnlyOnRanToCompletion);
        t.ContinueWith(task => Console.WriteLine("Sum threw: " + task.Exception),
         TaskContinuationOptions.OnlyOnFaulted);
        t.ContinueWith(task => Console.WriteLine("Sum was canceled"),
         TaskContinuationOptions.OnlyOnCanceled);

        //t.Wait();


        // ПРИМЕР 6. 
        // Дочерние задания

        Task<Int32[]> parent = new Task<Int32[]>(() =>
        {
            var results = new Int32[3];

            // Создание и запуск 3 дочерних заданий
            new Task(() => results[0] = Sum(10000),
                TaskCreationOptions.AttachedToParent).Start();
            new Task(() => results[1] = Sum(20000),
                TaskCreationOptions.AttachedToParent).Start();
            new Task(() => results[2] = Sum(30000),
                TaskCreationOptions.AttachedToParent).Start();
            // Возвращается ссылка на массив
            // (элементы могут быть не инициализированы)
            return results;
        });

        // Вывод результатов после завершения родительского и дочерних заданий
        var cwt = parent.ContinueWith(parentTask => Array.ForEach(parentTask.Result, Console.WriteLine));

        // Запуск родительского задания, которое запускает дочерние
        parent.Start();

        for (int i = 0; i < 100; i++)
        {
            Thread.Sleep(1);
            Console.WriteLine(i);
        }



        // ПРИМЕР 7
        // TaskFactory

        Task parent1 = new Task(() =>
        {
            var cts = new CancellationTokenSource();
            var tf = new TaskFactory<Int32>(cts.Token,
            TaskCreationOptions.AttachedToParent,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
            // Задание создает и запускает 3 дочерних задания
            var childTasks = new[] {
                tf.StartNew(() => Sum(cts.Token, 10000)),
                tf.StartNew(() => Sum(cts.Token, 20000)),
                tf.StartNew(() => Sum(cts.Token, Int32.MaxValue)) // Исключение
                                                                  // OverflowException
            };

            // если дочернее задание становится источником исключения,
            // отменяем все дочерние задания
            for (Int32 task = 0; task < childTasks.Length; task++)
            {
                childTasks[task].ContinueWith(t => cts.Cancel(),
                    TaskContinuationOptions.OnlyOnFaulted);
            }

            tf.ContinueWhenAll(
                childTasks,
                competedTasks => competedTasks
                    .Where(t => !t.IsFaulted && !t.IsCanceled)
                    .Max(t => t.Result),
                CancellationToken.None)
            .ContinueWith(t => Console.WriteLine("The maximus is: " + t.Result));

        });

        parent1.ContinueWith(p =>
        {
            // текст помщен в StringBuilder и однократно вызван
            // метод Console.WriteLine просто потому, что это задание 
            // может выполняться параллельно с предыдущим,
            // и я не хочу путаницы в выводимом результате
            StringBuilder sb = new StringBuilder(
                "The following exception(s) occured:" + Environment.NewLine
            );

            foreach (var e in p.Exception.Flatten().InnerExceptions)
            {
                sb.AppendLine(" " + e.GetType().ToString());
            }
            Console.WriteLine(sb.ToString());
        }, TaskContinuationOptions.OnlyOnFaulted);

        parent1.Start();



        // ПРИМЕР 8
        // Таймер

        Console.WriteLine("Checking status every 2 seconds");

        // Создание таймера, который никогда не срабатывает. Это гарантирует
        // что ссылка на него будет храниться в timer
        // до активации Status потоком из пула.
        timer = new Timer(Status, null, Timeout.Infinite, Timeout.Infinite);

        // теперь, когда timer присвоено значенрие, можно разрешить таймеру 
        // сратаывать; мы знаем, что вызов Change в Status не выдаст 
        // исклюкчение NullReferenceException
        timer.Change(0, Timeout.Infinite);

        Console.ReadLine();


        // ПРИМЕР 9
        // Переработанный пример 8 с использованием Task.Delay

        Console.WriteLine("Checking status every 2 seconds");

        StatusAsync();

        Console.ReadLine();

        
    }

    private async void StatusAsync()
    {
        while (true)
        {
            Console.WriteLine($"Checking status at {DateTime.Now}");
            // здесь размещается код проверки состояния

            // в конце цикла создается 2-секундная задержка без блокировки потока
            await Task.Delay(2000); // await ожидает возвращение управления потоком
        }
    }

    private static void Status(Object state)
    {
        Console.WriteLine($"In status at {DateTime.Now}");
        Thread.Sleep(1000);

        // Заставляем таймер снова вызвать метод через 2 секунды
        timer.Change(2000, Timeout.Infinite);

        // Когда метод возвращает управление, поток
        // возвращается в пул и ожидает следующего задания
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

    private static Int32 Sum(Int32 n)
    {
        Int32 sum = 0;
        for (; n > 0; n--)
        {
            checked { sum += n; }
        }
        return sum;
    }

    private static Int32 Sum(CancellationToken ct, Int32 n)
    {
        Int32 sum = 0;
        for (; n > 0; n--)
        {
            // Следующая строка приводит к исключению OperationCanceledException
            // при вызове метода Cancel для объекта CancellationTokenSource,
            // на который ссылается маркер
            ct.ThrowIfCancellationRequested();
            checked { sum += n; }
        }
        return sum;
    }
}
