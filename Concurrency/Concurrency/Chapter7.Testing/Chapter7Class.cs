namespace Concurrency.Chapter7.Testing;

internal class Chapter7Class : IChapter
{
    public async Task Execute()
    {
        await ThrowsAsync<InvalidOperationException>(Get);


        await ThrowsAsync<NotImplementedException>(Get);
    }

    public async Task<int> Get()
    {
        return await Task.FromException<int>(new NotImplementedException());
    }

    public async static Task ThrowsAsync<TException>(Func<Task> action,
        bool allowDerivedTypes = true)
        where TException : Exception
    {
        try
        {
            await action();
            var name = typeof(Exception).Name;
            Assert.Fail("Not exception");
        }
        catch (Exception ex)
        {
            if (allowDerivedTypes && !(ex is TException))
            {
                Assert.Fail($"Delegate threw exception of type { ex.GetType().Name}" +
                    $", but {typeof(TException).Name} or a derived type was expected.");

            }

            else if (!allowDerivedTypes && ex.GetType() != typeof(TException))
            {
                Assert.Fail($"Delegate threw exception of type {ex.GetType().Name}"
                    + $", but {typeof(TException).Name} was expected.");
            }

            else
            {
                Console.WriteLine("Ok!");
            }
        }
    }
}

public static class Assert
{
    public static void True(bool condition, string message = "Condition was not true")
    {
        if (!condition)
        {
            Fail(message);
        }
    }

    public static void Equal<T>(T expected, T actual, string message = "Values are not equal")
    {
        if (!expected.Equals(actual))
        {
            Fail(message);
        }
    }

    public static void Fail(string message)
    {
        Console.WriteLine(message);
    }
}
