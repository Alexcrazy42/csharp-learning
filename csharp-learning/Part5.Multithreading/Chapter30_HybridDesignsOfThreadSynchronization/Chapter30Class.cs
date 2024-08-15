using System.Threading;

namespace csharp_learning.Part5.Multithreading.Chapter30_HybridDesignsOfThreadSynchronization;

public class Chapter30Class
{
    public void Execute()
    {

    }
}

// ПРИМЕР 1
public sealed class SimpleHybridLock : IDisposable
{
    // Int32 используется примитивными конструкциями
    // пользовательского режима (Interlocked-методы)
    private Int32 waiters = 0;

    private AutoResetEvent waiterLock = new AutoResetEvent(false);

    public void Enter()
    {
        // поток хочет получить блокировку
        if(Interlocked.Increment(ref waiters) == 1)
        {
            return; // блокировка свободна, конкурекции нет, возвращает управление
        }

        // блокировка захвачена другим потоком (конкуренция),
        // приходится ждать
        waiterLock.WaitOne(); // значительное снижение производительности
        // Когда WaitOne возвращает управление, этот поток блокируется
    }

    public void Leave()
    {
        // поток освобождает блокировку
        if (Interlocked.Decrement(ref waiters) == 0)
        {
            return; // другие потоки не заблокированы, возвращает управление
        }

        // другие потоки заблокированы
        waiterLock.Set(); // значительное снижение производительности
    }

    public void Dispose()
    {
        waiterLock.Dispose();
    }
}

// ПРИМЕР 2
// гибридное блокирование с одновременными зацикливанием, владением потоком и рекурсией
public sealed class AnotherHybridLock : IDisposable
{
    private Int32 waiters = 0;

    private AutoResetEvent waiterLock = new AutoResetEvent(false);

    // это поле контролирует зацикливание с целью поднять производительность
    private Int32 spinCount = 4000;

    // Эти поля указывают, какой поток и сколько раз блокируется
    private Int32 owningThreadId = 0, recursion = 0;

    public void Enter()
    {
        // Если вызывающий поток уже захватил блокировку, увеличим рекурсивный счетчик
        // на 1 и вернем управление
        Int32 threadId = Thread.CurrentThread.ManagedThreadId;

        if(threadId == owningThreadId)
        {
            recursion++;
            return;
        }

        SpinWait spinWait = new SpinWait();

        for (Int32 spinCount = 0; spinCount < this.spinCount; spinCount++)
        {
            if (Interlocked.CompareExchange(ref waiters, 1, 0) == 0)
            {
                goto GotLock;
            }
            spinWait.SpinOnce();
        }

        if (Interlocked.Increment(ref waiters) > 1)
        {
            waiterLock.WaitOne();
        }

        GotLock:
            owningThreadId = threadId; recursion = 1;
    }

    public void Leave()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        if (threadId != owningThreadId)
        {
            throw new SynchronizationLockException("Lock not owned by calling thread");
        }

        // уменьшает на 1 рекурсивный счетчик. Если поток все еще
        // заперт, возвращаем управление
        if (--recursion > 0) return;

        owningThreadId = 0; // запертых потоков больше нет

        // если нет других заблокированных потоков, возврращаем управление
        if (Interlocked.Decrement(ref waiters) == 0)
        {
            return;
        }

        // остальные поток заблокированы, пробуждаем один из них
        waiterLock.Set(); // значительное падение производительности
    }

    public void Dispose()
    {
        waiterLock.Dispose();
    }
}