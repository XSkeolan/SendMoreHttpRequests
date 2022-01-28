using System.Net;
namespace SendMoreHttpRequests
{
    class Program
    {
        private static readonly ResponseStatistic statistic = new ResponseStatistic();
        private static readonly HttpClient httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            //var t1 = Task.Factory.StartNew(() => Send());
            //var t2 = Task.Factory.StartNew(() => Send());
            //while (!t2.IsCompleted)
            //    continue;
            //Console.WriteLine("end");

            //Thread[] threads = new Thread[5];
            //for (int i = 0; i < threads.Length; i++)
            //{
            //    threads[i] = new Thread(Send) { IsBackground = true };
            //    threads[i].Start();
            //}

            Task[] tasks = new Task[5];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(()=> Send());
            }
            while (true) { }
            //var tasks = new List<Task>(20);
            //for (int i = 0; i < 10; i++)
            //    tasks.Add(Task.Run(() => Send()));
            //Task.WaitAll(tasks.ToArray());
        }

        static async void Send()
        {
            try
            {
                string url = "http://webcode.me";
                List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
                for (int i = 0; i < 10000; i++)
                {
                    tasks.Add(httpClient.GetAsync(url));
                }
                foreach (var task in tasks)
                {
                    try
                    {
                        if ((await task).StatusCode == HttpStatusCode.OK)
                        {
                            statistic.Collect(StatisticType.Ok);
                        }
                        else
                        {
                            statistic.Collect(StatisticType.Failed);
                        }
                    }
                    catch (Exception e)
                    {
                        statistic.Collect(StatisticType.Failed);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - all");
            }
        }
    }
}
