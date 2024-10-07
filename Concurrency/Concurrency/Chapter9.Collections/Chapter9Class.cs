using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Concurrency.Chapter9.Collections;

internal class Chapter9Class : IChapter
{

    private readonly BlockingCollection<int> _blockingQueue = new BlockingCollection<int>();
    private Random rand = new Random();

    public async Task Execute()
    {
        //var stack = ImmutableStack<int>.Empty;
        //stack = stack.Push(13);
        //stack = stack.Push(7);

        //foreach (int item in stack)
        //{
        //    Console.WriteLine(item);
        //}

        //int lastItem;
        //stack = stack.Pop(out lastItem);
        //Console.WriteLine(lastItem);


        //ImmutableQueue<int> queue = ImmutableQueue<int>.Empty;
        //queue = queue.Enqueue(13);
        //queue = queue.Enqueue(7);
        //foreach (int item in queue)
        //    Console.WriteLine(item);
        //int nextItem;
        //queue = queue.Dequeue(out nextItem);
        //Console.WriteLine(nextItem);


        //ImmutableHashSet<int> hashSet = ImmutableHashSet<int>.Empty;
        //hashSet = hashSet.Add(13);
        //hashSet = hashSet.Add(7);
        //hashSet = hashSet.Add(7);
        //foreach (int item in hashSet)
        //    Console.WriteLine(item);
        //hashSet = hashSet.Remove(7);


        //ImmutableDictionary<int, string> dictionary =
        //ImmutableDictionary<int, string>.Empty;
        //dictionary = dictionary.Add(10, "Ten");
        //dictionary = dictionary.Add(21, "Twenty-One");
        //dictionary = dictionary.SetItem(10, "Diez");
        //// Выводит "10Diez" и "21Twenty-One" в непредсказуемом порядке.
        //foreach (KeyValuePair<int, string> item in dictionary)
        //    Console.WriteLine(item.Key + item.Value);
        //string ten = dictionary[10];
        //// ten == "Diez"
        //dictionary = dictionary.Remove(21);


        //var dictionary = new ConcurrentDictionary<int, string>();
        //string newValue = dictionary.AddOrUpdate(0,
        //    key => "Zero",
        //    (key, oldValue) => "Zero");

        //Task task1 = Task.Run(async () =>
        //{
        //    var doIt = true;
        //    while (doIt)
        //    {
        //        var newElem = rand.Next();
        //        Console.WriteLine($"write {newElem}");

        //        if (newElem % 15 == 0)
        //        {
        //            doIt = false;
        //        }
        //        _blockingQueue.Add(newElem);
        //        await Task.Delay(1000);
        //    }

        //});

        //var task2 = Task.Run(async () =>
        //{
        //    Console.Write("read: ");
        //    foreach (int item in _blockingQueue.GetConsumingEnumerable())
        //    {
        //        Console.Write($"{item} ");
        //    }
        //    Console.WriteLine("\n");

        //});

        //await Task.WhenAll(task1, task2);

        //var queue = Channel.CreateBounded<int>(
        //    new BoundedChannelOptions(1)
        //    {
        //        FullMode = BoundedChannelFullMode.DropOldest,
        //    });

        //ChannelWriter<int> writer = queue.Writer;

        //// операция записи завершается немедленно
        //await writer.WriteAsync(7);

        //// Операция записи тоже завершается немедленно.
        //// Элемент 7 теряется, если только он не был
        //// немедленно извлечен потребителем.
        //await writer.WriteAsync(13);

        //var queue = new BufferBlock<int>();

        //queue.Post(7);
        //queue.Post(13);
        //queue.Complete();

        //while (true)
        //{
        //    int item;

        //    try
        //    {
        //        item = queue.Receive();
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        break;
        //    }

        //    Console.WriteLine(item);
        //}

        // Код-потребитель передается конструктору очереди
        ActionBlock<int> queue = new ActionBlock<int>(item =>
         Trace.WriteLine(item));
        // Асинхронный код-производитель
        await queue.SendAsync(7);
        await queue.SendAsync(13);
        // Синхронный код-производитель
        queue.Post(7);
        queue.Post(13);
        queue.Complete();
    }
}
