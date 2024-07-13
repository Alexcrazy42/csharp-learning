using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace csharp_learning.Part4.KeyMechanisms.Chapter20_ExceptionsAndStateControl;

public class Chapter20Class
{
    public void Execute()
    {
        try
        {
            Console.WriteLine("In try");
        }
        finally
        {
            Type1.M();
        }

        RuntimeHelpers.PrepareConstrainedRegions();

        try
        {
            Console.WriteLine("In try 2");
        }
        finally
        {
            Type2.M();
        }
    }
}

public sealed class Type1
{
    static Type1()
    {
        Console.WriteLine("Type1's static ctor called");
    }

    public static void M()
    {

    }
}

public class Type2
{
    static Type2()
    {
        Console.WriteLine("Type2's static ctor called");
    }

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    public static void M()
    {

    }
}