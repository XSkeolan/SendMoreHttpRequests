using System.Net;
using System.Threading.Channels;

namespace SendMoreHttpRequests
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            ResponseStatistic statistic;
            string url = "http://localhost/";
            CancellationTokenSource cts = new CancellationTokenSource();
            int number = 1;
            Console.WriteLine("Time start - {0}", DateTime.Now);
            statistic = new ResponseStatistic();
            var channel = Channel.CreateBounded<Task>(100);
            var writer = channel.Writer;
            var reader = channel.Reader;
            Task tWrite = Task.Run(async () =>
            {
                //CancellationTokenSource ctsT = new CancellationTokenSource();
                while (await writer.WaitToWriteAsync())
                {
                    Console.WriteLine("Writting task...");
                    writer.TryWrite(Task.Run(() =>
                    {
                        List<Task> tasksRequest = new List<Task>();
                        for (int j = 0; j < 15000; j++)
                            tasksRequest.Add(httpClient.GetAsync(url).ContinueWith((antecedent) =>
                            {
                                try
                                {
                                    if (antecedent.Exception != null)
                                    {
                                        statistic.Collect(StatisticType.Failed);
                                        return;
                                    }
                                    if (antecedent.Result.StatusCode == HttpStatusCode.OK)
                                        statistic.Collect(StatisticType.Ok);
                                    else
                                        statistic.Collect(StatisticType.Failed);
                                }
                                catch (Exception)
                                {
                                    statistic.Collect(StatisticType.Failed);
                                }
                            }));
                        Task.WaitAll(tasksRequest.ToArray());
                        Console.WriteLine("{0} task of tasks completed 15000 tasks!!!", number++);
                        tasksRequest.Clear();
                    }));
                }
            }, cts.Token);

            Task tRead = Task.Run(async () =>
            {
                Task currentTask;
                while (await reader.WaitToReadAsync())
                {
                    Console.WriteLine("Reading task...");
                    reader.TryRead(out currentTask);
                    if(currentTask != null)
                        await currentTask;
                }
            }, cts.Token);

            Console.WriteLine("Input Q or ESC for quit from application");
            ConsoleKeyInfo k;
            while(true)
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
    }
}
