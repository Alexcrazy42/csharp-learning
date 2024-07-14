namespace csharp_learning.Part4.KeyMechanisms.Chapter21_GarbageCollection;

public class Chapter21Class
{
    

    public void Execute()
    {
        //Timer t = new Timer(TimerCallback, null, 0, 2000);

        //Console.ReadLine();

        //t.Dispose();

        GCNotification gcN;
        for (int i = 0; i < 1_000_000; i++)
        {
            gcN = new GCNotification();
        }


    }

    private static void TimerCallback(Object o)
    {
        Console.WriteLine($"In timerCallback: {DateTime.Now}");
        // принудительный вызов уборщика мусора в этой программе
        GC.Collect();
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