
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace csharp_learning.Chapter12_Generalization;

public class Chapter12Class
{
    public void Execute()
    {
        List<DateTime> dtList = new();

        dtList.Add(DateTime.Now); // происходит без упаковки

        dtList.Add(DateTime.MinValue); // происходит без упаковки

        // Убедимся в том, что обобщения повышают производительность
        // Сравниваем производительность необобщенного алгоритма ArrayList из библиотеки классов FCL
        // и обобщенного алгоритма List
        ValueTypePerfTest();

        RefTypePerfTest();

        /*
            Результаты:
            ,11 seconds (GCs=  3) List<Int32>
            1,58 seconds (GCs= 33) ArrayList of Int32
            ,15 seconds (GCs=  2) List<String>
            ,23 seconds (GCs=  4) ArrayList of String
        */

        Object o = null;

        // Dictionary<,> - это открытый тип с двумя параметрами типа
        Type t = typeof(Dictionary<,>);

        // попытка создания экземпляра этого типа (неудачная)
        // неудачна она потому, что мы не задаем ни одного из обобщенные параметров: TKey, TValue
        // Error: Cannot create an instance of System.Collections.Generic.Dictionary`2[TKey,TValue]
        // because Type.ContainsGenericParameters is true.
        // после имени типа, через ` написана арность (arity) типа, это число необходимых для него параметров
        // для dictionary арность = 2, потому что мы должны определить TKey и TValue
        o = Createlnstance(t);
        Console.WriteLine();

        // DictionaryStringKey<Guid> - закрытый тип
        // арность DictionaryStringKey = 1, нужно определить лишь TValue
        t = typeof(DictionaryStringKey<Guid>);

        // попытка создания экземляра этого типа (удачная)
        o = Createlnstance(t);

        Console.WriteLine($"Object type = {o.GetType()}");


        Console.WriteLine("--------------------------------------------");
        Node<Char> head = new('C');
        head = new('B', head);
        head = new('A', head);
        Console.WriteLine(head.ToString());

        Node head1 = new TypedNode<Char>('A');
        head1 = new TypedNode<int>(3, head1);
        head1 = new TypedNode<DateTime>(DateTime.Now, head1);
        Console.WriteLine(head1.ToString());

        Console.WriteLine("--------------------------------------------");

        IList<String> ls = new List<String>();
        ls.Add("String");

        // преобразует IList<String> в IList<Object>
        IList<Object> lo = ConvertClass.ConvertIList<String, Object>(ls);

        // преобразует IList<String> в IList<IComparable>
        IList<IComparable> lc = ConvertClass.ConvertIList<String, IComparable>(ls);

        // преобразует IList<String> в IList<IComparable<String>>
        IList<IComparable<String>> lcs = ConvertClass.ConvertIList<String, IComparable<String>>(ls);

        // преобразует IList<String> в IList<String>
        // тип совместим сам с собой
        IList<String> ls2 = ConvertClass.ConvertIList<String, String>(ls);

        // преобразует IList<String> в IList<Exception>, ошибка
        // Тип string не может быть использован, как параметр типа T, в универсальном типе или методе
        // ConvertIList<T, Base>(IList<T>). Нет преобразования неявной ссылки из string в Exception
        // IList<Exception> le = ConvertClass.ConvertIList<String, Exception>(ls);
    }

    private Object Createlnstance(Type t)
    {
        Object o = null;
        try
        {
            o = Activator.CreateInstance(t);
            Console.WriteLine("Created instance of {0}", t.ToString());
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        return o;
    }

    private void ValueTypePerfTest()
    {
        const Int32 count = 10_000_000;

        using (new OperationTimer("List<Int32>"))
        {
            List<Int32> l = new();
            for (Int32 n = 0; n < count; n++)
            {
                l.Add(n);
                Int32 x = l[n];
            }
            l = null; // объекта из heap удалиться в процессе сборки мусора
        }
        
        using (new OperationTimer("ArrayList of Int32"))
        {
            ArrayList al = new();
            for (Int32 n = 0; n < count; n++)
            {
                al.Add(n);
                Int32 x = (Int32)al[n];
            }
            al = null;
        }
    }

    private void RefTypePerfTest()
    {
        const Int32 count = 10_000_000;
        using(new OperationTimer("List<String>"))
        {
            List<String> l = new();
            for (Int32 n = 0; n < count; n++)
            {
                l.Add("X");
                String x = l[n];
            }
            l = null; // объекта из heap удалиться в процессе сборки мусора
        }
        
        using (new OperationTimer("ArrayList of String"))
        {
            ArrayList al = new();
            for (Int32 n = 0; n < count; n++)
            {
                al.Add("X");
                String x = (String)al[n];
            }
            al = null;
        }
    }
}

// класс для оценки времени выполнения алгоритма
public sealed class OperationTimer : IDisposable
{
    private Int64 startTime;
    private String text;
    private Int32 collectionCount;

    public OperationTimer(String text)
    {
        PrepareForOperation();
        this.text = text;
        collectionCount = GC.CollectionCount(0);

        // это выражение должно быть последним в этом методе
        // чтобы обеспечить макмимально точную оценку быстродействия
        startTime = Stopwatch.GetTimestamp();
    }

    public void Dispose()
    {
        Console.WriteLine("{0,6:###.00} seconds (GCs={1,3}) {2}", 
            (Stopwatch.GetTimestamp() - startTime) / (Double) Stopwatch.Frequency, 
            GC.CollectionCount(0) - collectionCount, 
            text);
    }

    private static void PrepareForOperation()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}

// частично определнный открытый тип
public sealed class DictionaryStringKey<TValue> : Dictionary<String, TValue>
{
    
}

public sealed class GenericTypeThatRequiresAnEnum<T>
{
    static GenericTypeThatRequiresAnEnum()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
    }
}

public sealed class Node<T>
{
    public T data;
    public Node<T> next;

    public Node(T data) : this(data, null)
    {

    }

    public Node(T data, Node<T> next)
    {
        this.data = data;
        this.next = next;
    }

    public override string ToString()
    {
        return data.ToString() + (next != null ? $"->{next}" : "");
    }
}

public class Node
{
    protected Node next;
    
    public Node(Node next)
    {
        this.next = next;
    }
}

// связный список с произвольным типом данных
public class TypedNode<T> : Node
{
    public T data;

    public TypedNode(T data) : this(data, null)
    { }
    
    public TypedNode(T data, Node next) : base(next)
    {
        this.data = data;
    }

    public override string ToString()
    {
        return data.ToString() + (next != null ? 
            "->" + next.ToString() : null);
    }
}

// ограничения виртуальных методов
public class Base
{
    public virtual void M<T1, T2>()
        where T1 : struct
        where T2 : class
    { }
}

public sealed class Derived : Base
{
    public override void M<T3, T4>()
        // where T3 : EventArgs // ограничения для методов интерфейсов с переопределением и явной 
        // реализацией наследуются от базового метода и поэтому
        // не могут быть заданы явно
        where T4 : class // все ок

    {

    }
}

public class ConvertClass
{
    // использование ограничения типа параметра типа
    // определены длва параметра типа, из которых T ограничен параметров типа TBase
    // это значит, что какой бы аргумент типа ни был задан для T, он должен быть совместим с аргументом типа,
    // заданным для TBase
    public static List<TBase> ConvertIList<T, TBase>(IList<T> list)
        where T : TBase
    {
        List<TBase> baseList = new List<TBase>(list.Count);
        for (Int32 index = 0; index < list.Count; index++)
        {
            baseList.Add(list[index]);
        }
        return baseList;
    }
}


public sealed class ConstructorContraint<T> 
    where T : new()
{
    public static T Factory()
    {
        // допустимо, потому что у всех значимых типов неявно
        // есть открытый конструктор без параметрови потому что 
        // это ограничение требует, чтобы у всех указанных ссылочных типов
        // также был открытый конструктор безе параметров
        return new T();
    }
}