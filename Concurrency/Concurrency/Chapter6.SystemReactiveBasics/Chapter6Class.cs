using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;

namespace Concurrency.Chapter6.SystemReactiveBasics;

internal class Chapter6Class : IChapter
{
    public async Task Execute()
    {
        await Task.Delay(0);

        var progress = new Progress<int>();

        IObservable<EventPattern<int>> progressReports = Observable.FromEventPattern<int>(
            handler => progress.ProgressChanged += handler,
            handler => progress.ProgressChanged -= handler);

        progressReports.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs));
    }
}
