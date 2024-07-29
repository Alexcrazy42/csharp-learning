using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_learning.Part5.Multithreading.Chapter26_ExecutionThreads;

public class Chapter26Class
{
    public void Execute()
    {
        Console.WriteLine("Main thread: starting a dedicated thread to do an asynchronous operation");

        Thread dedicatedThread = new Thread(ComputeBoundOp);
        dedicatedThread.Start(5);

        Console.WriteLine("Main thread: Doing other work here...");
        //Thread.Sleep(10000); // Имитация другой работы (10 секунд)
        while (true)
        {
            Console.WriteLine("Hello from execute");
            Thread.Sleep(1000);
        }

        dedicatedThread.Join(); // Ожидание завершения потока
        Console.WriteLine("Hit <Enter> to end this program...");
        Console.ReadLine();

        Thread t = new Thread(Worker);

        // Превращение потока в фоновый
        t.IsBackground = true;

        t.Start(); // Старт потока
                   // В случае активного потока приложение будет работать около 10 секунд
                   // В случае фонового потока приложение немедленно прекратит работу
        Console.WriteLine("Returning from Main");
    }

    // Сигнатура метода должна совпадать
    // с сигнатурой делегата ParameterizedThreadStart
    private void ComputeBoundOp(Object state)
    {
        // Метод, выполняемый выделенным потоком
        while (true)
        {
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
            Thread.Sleep(1000); // Имитация другой работы (1 секунда)
        }
        
        // После возвращения методом управления выделенный поток завершается
    }

    private void Worker()
    {
        Thread.Sleep(1000);

        // Следующая строка выводится только для кода,
        // исполняемого активным потоком
        Console.WriteLine("Returning from Worker");
    }
}
