using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;

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
        o = Createlnstance(t);
        Console.WriteLine();

        // DictionaryStringKey<Guid> - закрытый тип
        t = typeof(DictionaryStringKey<Guid>);

        // попытка создания экземляра этого типа (удачная)
        o = Createlnstance(t);

        Console.WriteLine($"Object type = {o.GetType()}");

    }

    private Object Createlnstance(Type t)
    {
        Object o = null;
        try
        {
            о = Activator.CreateInstance(t);
            Console.Write("Created instance of {0}", t.ToString());
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