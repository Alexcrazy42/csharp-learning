using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace csharp_learning.Part5.Multithreading.Chapter29_PrimitiveDesignsOfThreadSynchronization;

internal class Chapter29Class
{
    public void Execute()
    {
        // ПРИМЕР 1
        //MultiWebRequests mwr = new MultiWebRequests();

        //Thread.Sleep(1000);

        // ПРИМЕР 3
        Int32 x = 0;

        const Int32 iterations = 10000000;

        Stopwatch sw = Stopwatch.StartNew();
        for (Int32 i = 0; i < iterations; i++)
        {
            x++;
        }
        Console.WriteLine("Incrementing x: {0:N0}", sw.ElapsedMilliseconds);

        // Сколько времени займет инкремент x 10 миллионов раз, если
        // добавить вызов ничего не делающего метода?
        sw.Restart();
        for (Int32 i = 0; i < iterations; i++)
        {
            M(); x++; M();
        }
        Console.WriteLine("Incrementing x in M: {0:N0}", sw.ElapsedMilliseconds);


        // Сколько времени займет инкремент x 10 миллионов раз, если
        // добавить вызов неконкурирующего объекта SimpleSpinLock?
        SpinLock sl = new SpinLock(false);
        sw.Restart();
        for (Int32 i = 0; i < iterations; i++)
        {
            Boolean taken = false; sl.Enter(ref taken); x++; sl.Exit();
        }
        Console.WriteLine("Incrementing x in SpinLock: {0:N0}",
        sw.ElapsedMilliseconds);

        // Сколько времени займет инкремент x 10 миллионов раз, если
        // добавить вызов неконкурирующего объекта SimpleWaitLock?
        using (SimpleWaitLock swl = new SimpleWaitLock())
        {
            sw.Restart();
            for (Int32 i = 0; i < iterations; i++)
            {
                swl.Enter(); x++; swl.Leave();
            }
            Console.WriteLine(
            "Incrementing x in SimpleWaitLock: {0:N0}", sw.ElapsedMilliseconds);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void M() { /* Этот метод только возвращает управление */ }

    // ПРИМЕР 2
    public Int32 Maximum(ref Int32 target, Int32 value)
    {
        Int32 currentVal = target, startVal, desiredVal;

        // параметр target может использоваться другим потоком,
        // его трогать не стоит
        do
        {
            // запись начального значение этой итерации
            startVal = currentVal;

            // вычисление желаемого значения в контексте startVal и value
            desiredVal = Math.Max(startVal, value);

            // ПРИМЕЧАНИЕ. Здесь поток может быть прерван
            
            // if (target == startVal) target = desiredVal
            // Возвращает значения, предшествующего потенциальным значениям
            currentVal = Interlocked.CompareExchange(ref target, desiredVal, startVal);
        }
        // если начальное значение на этой итерации изменилось повторить
        while (startVal != currentVal);

        return desiredVal;
    }
}


// ПРИМЕР 1
internal sealed class MultiWebRequests
{
    // Этот класс Helper координирует все асинхронные операции
    private AsyncCoordinator m_ac = new AsyncCoordinator();

    // Набор веб-серверов, к которым будут посылаться запросы
    // Хотя к этому словарю возможны одновременные обращения,
    // в синхронизации доступа нет необходимости, потому что
    // ключи после создания доступны только для чтения
    private Dictionary<String, Object> m_servers = new Dictionary<String, Object> 
    {
        { "http://Wintellect.com/", null },
        { "http://Microsoft.com/", null },
        { "http://1.1.1.1/", null }
    };

    public MultiWebRequests(Int32 timeout = Timeout.Infinite)
    {
        
        var httpClient = new HttpClient();
        foreach (var server in m_servers.Keys)
        {
            m_ac.AboutToBegin(1);
            httpClient.GetByteArrayAsync(server)
                .ContinueWith(task => ComputeResult(server, task));
        }
        
        // Сообщаем AsyncCoordinator, что все операции были инициированы
        // и что он должен вызвать AllDone после завершения всех операций,
        // вызова Cancel или тайм-аута
        m_ac.AllBegun(AllDone, timeout);
    }
    private void ComputeResult(String server, Task<Byte[]> task)
    {
        Object result;
        if (task.Exception != null)
        {
            result = task.Exception.InnerException;
        }
        else
        {
            result = task.Result.Length;
        }

        // Сохранение результата (исключение/сумма)
        // и обозначение одной завершенной операции
        m_servers[server] = result;
        m_ac.JustEnded();
    }

    // При вызове этого метода результаты игнорируются
    public void Cancel() { m_ac.Cancel(); }

    // Этот метод вызывается после получения ответа от всех веб-серверов, 
    // вызова Cancel или тайм-аута
    private void AllDone(CoordinationStatus status)
    {
        switch (status)
        {
            case CoordinationStatus.Cancel:
                Console.WriteLine("Operation canceled.");
                break;
            case CoordinationStatus.Timeout:
                Console.WriteLine("Operation timedout.");
                break;
            case CoordinationStatus.AllDone:
                Console.WriteLine("Operation completed; results below:");
                foreach (var server in m_servers)
                {
                    Console.Write("{0} ", server.Key);
                    Object result = server.Value;
                    if (result is Exception)
                    {
                        Console.WriteLine("failed due to {0}.", result.GetType().Name);
                    }
                    else
                    {
                        Console.WriteLine("returned {0:N0} bytes.", result);
                    }
                }
                break;
        }
    }
}

internal enum CoordinationStatus { AllDone, Timeout, Cancel };

internal sealed class AsyncCoordinator
{
    private Int32 m_opCount = 1; // Уменьшается на 1 методом AllBegun
    private Int32 m_statusReported = 0; // 0=false, 1=true
    private Action<CoordinationStatus> m_callback;
    private Timer m_timer;

    // Этот метод ДОЛЖЕН быть вызван ДО инициирования операции
    public void AboutToBegin(Int32 opsToAdd = 1)
    {
        Interlocked.Add(ref m_opCount, opsToAdd);
    }

    // Этот метод ДОЛЖЕН быть вызван ПОСЛЕ обработки результата
    public void JustEnded()
    {
        if (Interlocked.Decrement(ref m_opCount) == 0)
            ReportStatus(CoordinationStatus.AllDone);
    }

    // Этот метод ДОЛЖЕН быть вызван ПОСЛЕ инициирования ВСЕХ операций
    public void AllBegun(Action<CoordinationStatus> callback,
        Int32 timeout = Timeout.Infinite)
    {
        m_callback = callback;
        if (timeout != Timeout.Infinite)
            m_timer = new Timer(TimeExpired, null, timeout, Timeout.Infinite);
        JustEnded();
    }

    private void TimeExpired(Object o)
    {
        ReportStatus(CoordinationStatus.Timeout);
    }

    public void Cancel() { 
        ReportStatus(CoordinationStatus.Cancel); 
    }

    private void ReportStatus(CoordinationStatus status)
    {
        // Если состояние ни разу не передавалось, передать его;
        // в противном случае оно игнорируется
        if (Interlocked.Exchange(ref m_statusReported, 1) == 0)
        {
            m_callback(status);
        }
    }
}

internal sealed class SimpleWaitLock : IDisposable
{
    private readonly AutoResetEvent m_available;

    public SimpleWaitLock()
    {
        m_available = new AutoResetEvent(true); // Изначально свободен
    }

    public void Enter()
    {
        // Блокирование на уровне ядра до освобождения ресурса
        m_available.WaitOne();
    }

    public void Leave()
    {
        // Позволяем другому потоку обратиться к ресурсу
        m_available.Set();
    }

    public void Dispose() { m_available.Dispose(); }
}

// ПРИМЕР 4
public sealed class SimpleWaitLockOnSemathore : IDisposable
{
    private Semaphore availableResources;

    public SimpleWaitLockOnSemathore(Int32 maximumConcurrentThreads)
    {
        availableResources = new Semaphore(maximumConcurrentThreads, maximumConcurrentThreads);
    }

    public void Enter()
    {
        // ожидаем в ядре доступа к ресурсу и возвращаем управление
        availableResources.WaitOne();
    }

    public void Leave()
    {
        // этому потоку доступ больше не нужен; его может получить другой поток
        availableResources.Release();
    }

    public void Dispose() { availableResources.Close(); }
}