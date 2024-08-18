namespace csharp_learning.Part5.Multithreading.Chapter28_IOAsynhronousOperations;

public class Chapter28Class
{
    public void Execute()
    {

    }
}


// ПРИМЕР 1
internal sealed class Type1 { }

internal sealed class Type2 { }

internal sealed class Methods
{
    private async Task<Type1> Method1Async()
    {
        throw new NotImplementedException();
    }

    private async Task<Type2> Method2Async()
    {
        throw new NotImplementedException();
    }

    private async Task<String> MyMethodAsync(Int32 arg)
    {
        Int32 local = arg;
        try
        {
            Type1 res1 = await Method1Async();
            for (int x = 0; x < 3; x++)
            {
                Type2 result2 = await Method2Async();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Catch");
        }
        finally
        {
            Console.WriteLine("Finally");
        }
        return "Done";
    }

}