using System.Net;

namespace Concurrency.Chapter8.Interaction;

internal class Chapter8Class : IChapter
{
    public Task Execute()
    {
        throw new NotImplementedException();
    }

    
}

public static class WebClientExtension
{
    public static Task<string> DownloadStringTaskAsync(this WebClient client,
        Uri adress)
    {
        var tcs = new TaskCompletionSource<string>();

        // Обработка события заверщит задачу и отменит свою регистрацию

        DownloadStringCompletedEventHandler handler = null;

        handler = (_, e) =>
        {
            client.DownloadStringCompleted -= handler;
            if (e.Cancelled)
            {
                tcs.TrySetCanceled();
            }
            else if (e.Error != null)
            {
                tcs.TrySetException(e.Error);
            }
            else
            {
                tcs.TrySetResult(e.Result);
            }
        };

        client.DownloadStringCompleted += handler;
        client.DownloadStringAsync(adress);
        return tcs.Task;
    }

    public static Task<WebResponse> GetResponseAsync(this WebRequest client)
    {
        return Task<WebResponse>.Factory.FromAsync(client.BeginGetResponse, client.EndGetResponse, null);
    }
}