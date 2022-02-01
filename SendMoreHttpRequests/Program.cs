using System.Net;
using System.Threading.Channels;

namespace SendMoreHttpRequests
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();
        static ResponseStatistic statistic = new ResponseStatistic();
        static string url = "http://localhost/";

        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.WriteLine("Time start - {0}", DateTime.Now);

            var channel = Channel.CreateBounded<int>(15000);
            var writer = channel.Writer;
            var reader = channel.Reader;
            Task taskWriter = Task.Run(async () =>
                {
                    while (await writer.WaitToWriteAsync())
                    {
                        writer.TryWrite(1);
                    }
                }, cts.Token);
            Task taskReader = Task.Run(async () =>
                {
                    while (await reader.WaitToReadAsync())
                    {
                        if (reader.TryRead(out _))
                        {
                            ReadAsync();
                        }
                    }
                }, cts.Token);

            Console.WriteLine("Input Q or ESC for quit from application");
            ConsoleKeyInfo k;
            while (true)
            {
                k = Console.ReadKey(false);
                switch (k.Key)
                {
                    case ConsoleKey.Escape:
                    case ConsoleKey.Q:
                        {
                            channel.Writer.Complete();
                            cts.Cancel();
                            Console.WriteLine("\nTime end - {0}", DateTime.Now);
                            Environment.Exit(0);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        static Task ReadAsync()
        {
            return httpClient.GetAsync(url).ContinueWith((antecedent) =>
            {
                try
                {
                    if (antecedent.Exception != null)
                    {
                        statistic.Collect(StatisticType.Failed);
                        return;
                    }
                    if (antecedent.Result.StatusCode == HttpStatusCode.OK)
                    {
                        statistic.Collect(StatisticType.Ok);
                    }
                    else
                    {
                        statistic.Collect(StatisticType.Failed);
                    }
                }
                catch (Exception)
                {
                    statistic.Collect(StatisticType.Failed);
                }
            });
        }
    }
}
