using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace csharp_learning.Part4.KeyMechanisms.Chapter21_GarbageCollection;

public class Chapter21Class
{
    

    unsafe public void Execute()
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

        //MemoryPressureDemo(0); // 0 вызывает нечастую уборку мусора
        //MemoryPressureDemo(10 * 1024 * 1024); // 10 Мб вызываеют частую 
        //                                      // уборку мусора

        //HandleCollectorDemo();

        for (int x = 0; x < 10000; x++) new Object();
        
        IntPtr originalMemoryAddress;
        Byte[] bytes1 = new byte[1000];

        // получаем адрес в памяти массива Byte[]
        // инструкция fixed работает эффективней, чем выделение
        // фиксированного GC-дескриптора. В данном случае она 
        // заставляет установить специальный "блокирующий" флаг на локальную 
        // переменную pbytes. Уборщик мусора, исследуя содержимое этого корня
        // и обнаруживая отличные от null значения, понимает, что во время
        // сжатия перемещать объект, на который ссылается эта переменная 
        // нельзя. Компилятор C# создает IL-код, присваивающий локальной 
        // переменной pbytes адрес объекта из начала блока fixed. При достижении
        // конца блока компилятор создает IL-инструкцию, возвращаемую переменной
        // pbytes значения null. Она перестает ссылаться на объект, позволяя
        // удалить объект в ходе следующей уборки мусора
        fixed (Byte* pbytes = bytes1)
        {
            originalMemoryAddress = (IntPtr)pbytes;
        }

        // мусор исчезает, позволяя сжать массив Byte[]
        GC.Collect();


        fixed(Byte* pbytes = bytes1)
        {
            string isMove = originalMemoryAddress == (IntPtr)pbytes ? " not" : "";
            Console.WriteLine($"The Byte[] did{isMove} move during the GC");

            Console.WriteLine((IntPtr)pbytes);
            Console.WriteLine(originalMemoryAddress);
        }



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
