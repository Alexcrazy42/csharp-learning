using System.Threading;

namespace csharp_learning.Part5.Multithreading.Chapter30_HybridDesignsOfThreadSynchronization;

public class Chapter30Class
{
    public void Execute()
    {
        // ПРИМЕР 7
        Lazy<String> s = new Lazy<String>(
            () => DateTime.Now.ToLongTimeString(),
            LazyThreadSafetyMode.PublicationOnly
        );

        Console.WriteLine(s.IsValueCreated); // false

        Console.WriteLine(s.Value);
        Console.WriteLine(s.IsValueCreated); // true, тк было обращение к Value

        Thread.Sleep(10000);

        Console.WriteLine(s.Value); // теперь делегат НЕ вызывается, результат прежний

        // ПРИМЕР 8

        String name = null;

        // Так как имя равно null, запускается делегат и инициализирует поле имени
        LazyInitializer.EnsureInitialized(ref name, () => "Jeffrey");
        Console.WriteLine(name);

        // Так как имя отлично от null, делегат не запускается и имя не меняется
        LazyInitializer.EnsureInitialized(ref name, () => "Richter");
        Console.WriteLine(name);
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


// ПРИМЕР 3
public sealed class Transaction
{
    private DateTime timeOflastTransaction;

    public void PerformTransaction()
    {
        Monitor.Enter(this);
        // этот код имеет эксклюзимвный доступ к данным
        timeOflastTransaction = DateTime.UtcNow;
        Monitor.Exit(this);
    }

    public DateTime LastTransaction
    {
        get
        {
            Monitor.Enter(this);
            // этот код имеет эксклюзимвный доступ к данным
            DateTime temp = timeOflastTransaction;
            Monitor.Exit(this);
            return temp;
        }
    }
}

// ПРИМЕР 4

public sealed class TransactionWithHiddenLock
{
    // теперь блокирование в рамках каждой транзакции ЗАКРЫТО
    private readonly Object m_lock = new Object();

    private DateTime timeOfLastTrans;

    public void PerformTransaction()
    {
        Monitor.Enter(m_lock); // Вход в закрытую блокировку
                               // Этот код имеет эксклюзивный доступ к данным...
        timeOfLastTrans = DateTime.Now;
        Monitor.Exit(m_lock); // Выход из закрытой блокировки
    }

    public DateTime LastTransaction
    {
        get
        {
            Monitor.Enter(m_lock); // Вход в закрытую блокировку
                                   // Этот код имеет монопольный доступ к данным...
            DateTime temp = timeOfLastTrans;
            Monitor.Exit(m_lock); // Завершаем закрытое блокирование
            return temp;
        }
    }
}

// ПРИМЕР 5
public sealed class Singleton
{
    private static readonly Object s_lock = new object();

    private static Singleton value = null;

    private Singleton()
    {

    }

    public static Singleton GetSingleton()
    {
        if (value != null) return value;

        Monitor.Enter(s_lock);

        if (value == null)
        {
            Singleton temp = new Singleton();

            Volatile.Write(ref value, temp);
        }

        Monitor.Exit(s_lock);

        return value;
    }
}

// ПРИМЕР 6
public sealed class Singleton1
{
    private static Singleton1 value = null;

    private Singleton1()
    {

    }

    public static Singleton1 GetSingleton()
    {
        if(value != null) return value;

        // создание нового объекта Singleton и превращение его в корень,
        // если этого еще не сделал другой поток
        Singleton1 temp = new();
        Interlocked.CompareExchange(ref value, temp, null);

        return value;
    }
}

// ПРИМЕР 9
internal sealed class ConditionVariablePattern
{
    private readonly Object m_lock = new Object();
    private Boolean condition = false;

    public void Thread1()
    {
        Monitor.Enter(m_lock);

        // "Атомарная" проверка сложного условия блокирования
        while (!condition)
        {
            // Если условие не соблюдается, ждем, что его поменяет другой поток
            Monitor.Wait(m_lock); // На время снимаем блокировку
                                  // чтобы другой поток мог ее получить
        }

        // Условие соблюдено, обрабатываем данные...

        Monitor.Exit(m_lock);  // Снятие блокировки
    }

    public void Thread2()
    {
        Monitor.Enter(m_lock); // Взаимоисключающая блокировка

        // Обрабатываем данные и изменяем условие...
        condition = true;

        // Monitor.Pulse(m_lock); // Будим одного ожидающего ПОСЛЕ отмены блокировки
        Monitor.PulseAll(m_lock); // Будим всех ожидающих ПОСЛЕ отмены блокировки

        Monitor.Exit(m_lock); // Снятие блокировки
    }
}