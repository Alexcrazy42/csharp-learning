using System;
using System.IO;

namespace csharp_learning.Part3.BasicDataTypes.Chapter17_Delegates;

public class Chapter17Class
{
    public void Execute()
    {
        StaticDelegateDemo();
        InstanceDelegateDemo();
        ChainDelegateDemo1(this);
        ChainDelegateDemo2(this);
    }

    private static void StaticDelegateDemo()
    {
        Console.WriteLine("----- Static Delegate Demo -----");
        Counter(1, 3, null);
        Counter(1, 3, new Feedback(FeedbackToConsole));
        Console.WriteLine();
    }
    private static void InstanceDelegateDemo()
    {
        Console.WriteLine("----- Instance Delegate Demo -----");
        Chapter17Class p = new Chapter17Class();
        Counter(1, 3, new Feedback(p.FeedbackToFile));
        Console.WriteLine();
    }
    private static void ChainDelegateDemo1(Chapter17Class p)
    {
        Console.WriteLine("----- Chain Delegate Demo 1 -----");
        Feedback fb1 = new Feedback(FeedbackToConsole);
        Feedback fb3 = new Feedback(p.FeedbackToFile);
        Feedback fbChain = null;
        fbChain = (Feedback)Delegate.Combine(fbChain, fb1);
        fbChain = (Feedback)Delegate.Combine(fbChain, fb3);
        Counter(1, 2, fbChain);
        Console.WriteLine();
        fbChain = null;
        Counter(1, 2, fbChain);
    }

    private static void ChainDelegateDemo2(Chapter17Class p)
    {
        Console.WriteLine("----- Chain Delegate Demo 2 -----");
        Feedback fb1 = new Feedback(FeedbackToConsole);
        Feedback fb3 = new Feedback(p.FeedbackToFile);
        Feedback fbChain = null;
        fbChain += fb1;
        fbChain += fb3;
        Counter(1, 2, fbChain);
        Console.WriteLine();
        Counter(1, 2, fbChain);
    }
    private static void Counter(Int32 from, Int32 to, Feedback fb)
    {
        for (Int32 val = from; val <= to; val++)
        {
            // Если указаны методы обратного вызова, вызываем их
            if (fb != null)
                fb(val);
        }
    }
    private static void FeedbackToConsole(Int32 value)
    {
        Console.WriteLine("Item=" + value);
    }

    private void FeedbackToFile(Int32 value)
    {
        using (StreamWriter sw = new StreamWriter("Status", true))
        {
            sw.WriteLine("Item=" + value);
        }
    }
}

public delegate void Feedback(Int32 value);