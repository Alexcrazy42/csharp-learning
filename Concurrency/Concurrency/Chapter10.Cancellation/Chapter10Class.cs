using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.Chapter10.Cancellation;

internal class Chapter10Class : IChapter
{
    public async Task Execute()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        cts.CancelAfter(TimeSpan.FromSeconds(3));
        await Do(token);

        using var cts1 = new CancellationTokenSource();
        var token1 = cts1.Token;
        cts1.CancelAfter(TimeSpan.FromSeconds(3));

        await CancelableMethodAsync(token1);
    }

    public async Task Do(CancellationToken ct)
    {
        while (true)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }
            Console.WriteLine("Do");
            await Task.Delay(100);
        }
    }

    public async Task CancelableMethodAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        Console.WriteLine("CancelableMethodAsync");
    }
}
