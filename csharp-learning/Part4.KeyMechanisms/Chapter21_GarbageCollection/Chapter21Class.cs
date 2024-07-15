using System.Runtime.InteropServices;

namespace csharp_learning.Part4.KeyMechanisms.Chapter21_GarbageCollection;

public class Chapter21Class
{
    

    public void Execute()
    {
        //Timer t = new Timer(TimerCallback, null, 0, 2000);

        //Console.ReadLine();

        //t.Dispose();
        Byte[] bytes = new Byte[] { 1, 2, 3, 4, 5 };

        FileStream fs = new FileStream("temp.dat", FileMode.Create);
        fs.Write(bytes, 0, bytes.Length);

        fs.Dispose();

        // Это не сработает сразу после записи в файл, так как вызов статического метода Delete
        // объекта File заставляет Windows удалить открытый файл, поэтому 
        // Delete генерирует исключение System.IO.IOException
        // Однако если другой поток инициализировал уборку мусора между вызовами
        // Write и Delete поле SafeFileHandle объекта FileStream вызывает свой
        // метод финализации, который закрывает файл
        File.Delete("temp.dat");

        MemoryPressureDemo(0); // 0 вызывает нечастую уборку мусора
        MemoryPressureDemo(10 * 1024 * 1024); // 10 Мб вызываеют частую 
                                              // уборку мусора

        HandleCollectorDemo();


    }

    private static void TimerCallback(Object o)
    {
        Console.WriteLine($"In timerCallback: {DateTime.Now}");
        // принудительный вызов уборщика мусора в этой программе
        GC.Collect();
    }

    private static void MemoryPressureDemo(Int32 size)
    {
        Console.WriteLine();
        Console.WriteLine($"MemoryPressureDemo, size={size}");

        for (int count = 0; count < 15; count++)
        {
            new BigNativeResource(size);
        }

        GC.Collect();
    }

    private sealed class BigNativeResource
    {
        private int size;

        public BigNativeResource(int size)
        {
            this.size = size;

            if (size > 0)
            {
                // пусть уборщик думает, что объект занимает больше памяти
                GC.AddMemoryPressure(size);
                Console.WriteLine("BigNativeResouce create.");
            }
        }

        ~BigNativeResource()
        {
            if (size > 0)
            {
                // пусть уборщик думает, что объект освободил больше память
                GC.RemoveMemoryPressure(size);
                Console.WriteLine("BigNativeResource destroy.");
            }
        }
    }

    private void HandleCollectorDemo()
    {
        Console.WriteLine();
        Console.WriteLine("HandleCollectorDemo");

        for(int count = 0; count < 10; count++)
        {
            new LimitedResource();
        }

        // в демонстрационных целях очищаем все
        GC.Collect();
    }

    private sealed class LimitedResource
    {
        // создаем объект HandleCollector и передаем ему указание
        // перейти к очистке, когда в куче появится два или более 
        // объекта LimitedResource
        private static HandleCollector hc = new HandleCollector("LimitedResource", 2);

        public LimitedResource()
        {
            hc.Add();
            Console.WriteLine($"LimitedResource create. Count={hc.Count}");
        }

        ~LimitedResource()
        {
            // сообщаем HandleCollector, что один объект LimitedResource
            // удален из кучи
            hc.Remove();
            Console.WriteLine($"LimitedResource destroy. Count={hc.Count}");
        }
    }
}

public sealed class SomeType
{
    public int a;

    ~SomeType() 
    {
        Console.WriteLine("Some type destructor");
    }
}

public sealed class GCNotification
{
    private static Action<Int32> gcDone = null;

    public static event Action<Int32> GCDone
    {
        add
        {
            // Если зарегистрированные делегаты отсутствуют, начинаем оповещение
            if (gcDone == null)
            {
                new GenObject(0);
                new GenObject(2);
            }
            gcDone += value;
        }
        remove
        {
            gcDone -= value;
        }
    }

    private sealed class GenObject
    {
        private Int32 generation;

        public GenObject(Int32 gen)
        {
            generation = gen;
        }

        // метод финализации
        ~GenObject()
        {
            // если объект принадлежит нужному нам поколению (или выше),
            // оповещаем делегает о выполненной уборке мусора
            Action<Int32> temp = Volatile.Read(ref gcDone);
            if (temp != null)
            {
                temp(generation);
            }

            // продолжаем оповещение, пока остается хотя один 
            // зарегестрированный делегат, домен приложений не выгружен
            // и процесс не завершен
            if ((gcDone != null)
                && !AppDomain.CurrentDomain.IsFinalizingForUnload()
                && !Environment.HasShutdownStarted)
            {
                // для поколения 0 создаем объект; для поколения 2 воскрешаем
                // объект и позволяем уборщику вызвать метод финализации
                // при следующей уборку мусора для поколения 2
                if (generation == 0)
                {
                    new GenObject(0);
                }
                else
                {
                    GC.ReRegisterForFinalize(this);
                }
            }
            else
            {
                // позволяем объекту исчезнуть
            }
        }
    }
}
