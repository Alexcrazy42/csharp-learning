using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Concurrency.Chapter9.Collections;

internal class Chapter9Class : IChapter
{
    public Task Execute()
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


        var dictionary = new ConcurrentDictionary<int, string>();
        string newValue = dictionary.AddOrUpdate(0,
            key => "Zero",
            (key, oldValue) => "Zero");

        return Task.CompletedTask;
    }
}
