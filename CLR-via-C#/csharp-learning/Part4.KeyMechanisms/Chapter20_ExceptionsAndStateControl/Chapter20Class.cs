using System.Diagnostics.Contracts;
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

        var cart = new ShoppingCart();
        Item item = null;
        cart.AddItem(item);
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

public sealed class Item
{

}

public sealed class ShoppingCart
{
    private List<Item> cart = new List<Item>();

    private Decimal totalCost = 0;

    public ShoppingCart()
    {

    }

    public void AddItem(Item item)
    {
        AddItemHelper(cart, item, ref totalCost);
        Console.WriteLine("Успешно добавлен item" + item.GetHashCode());
    }

    public static void AddItemHelper(List<Item> cart, 
        Item newItem, 
        ref Decimal totalCost)
    {
        // предусловия
        Contract.Requires(cart != null);
        Contract.Requires(newItem != null);
        Contract.Requires(Contract.ForAll(cart, s => s != newItem));

        // постусловия
        Contract.Ensures(Contract.Exists(cart, s => s == newItem));
        Contract.Ensures(totalCost >= Contract.OldValue(totalCost));
        Contract.EnsuresOnThrow<IOException>(totalCost == Contract.OldValue(totalCost));

        // какие то действия, способные вызвать IOException
        cart.Add(newItem);
        totalCost += 1.00M;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant()
    {
        Contract.Invariant(totalCost >= 0);
    }
}