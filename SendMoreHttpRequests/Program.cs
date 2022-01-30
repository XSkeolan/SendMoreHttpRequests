using System.Net;
namespace SendMoreHttpRequests
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        static void Main(string[] args)
        {
            ResponseStatistic statistic;
            string url = "https://reqbin.com/echo";
            int number = 1;
            Console.WriteLine("Time start - {0}", DateTime.Now);
            var tasks = new List<Task>();
            statistic = new ResponseStatistic();
            for (int i = 0; i < 60;i++)
                tasks.Add(Task.Run(() =>
                {
                    List<Task> tasksRequest = new List<Task>();
                    for (int j = 0; j < 10000; j++)
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
                            catch(Exception)
                            {
                                statistic.Collect(StatisticType.Failed);
                            }
                        }));
                    Console.WriteLine("{0} of the 1000 tasks waiting...", number++);
                    Task.WaitAll(tasksRequest.ToArray());
                    //foreach (var t in tasksRequest)
                    //    Console.WriteLine("{0,10} {1,20}", t.Id, t.Status);
                    Console.WriteLine("{0} of the 1000 tasks already ran 600 tasks!!!", --number);
                    tasksRequest.Clear();

                    //for (int j = 0; j < 600; j++)
                    //{
                    //    try
                    //    {
                    //        if ((await tasksRequest[j]).StatusCode == HttpStatusCode.OK)
                    //            statistic.Collect(StatisticType.Ok);
                    //        else
                    //            statistic.Collect(StatisticType.Failed);
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        statistic.Collect(StatisticType.Failed);
                    //    }
                    //}
                    ////List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
                    //for (int i = 0; i < 50000; i++)
                    //{
                    //    //tasks.Add(httpClient.GetAsync(url));
                    //    try
                    //    {
                    //        if ((await httpClient.GetAsync(url)).StatusCode == HttpStatusCode.OK)
                    //            statistic.Collect(StatisticType.Ok);
                    //        else
                    //            statistic.Collect(StatisticType.Failed);
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        statistic.Collect(StatisticType.Failed);
                    //    }
                    //}
                    ////foreach (var task in tasks)
                    ////{

                    ////}
                    //Console.WriteLine("Task ended");
                }));
            try
            {
                Task.WaitAll(tasks.ToArray());
                foreach (var t in tasks)
                    Console.WriteLine("{0,10} {1,20}",
                                      t.Id, t.Status);
                Console.WriteLine(statistic.Total);
            }
            catch (AggregateException)
            {
                Console.WriteLine("Status of tasks:\n");
                Console.WriteLine("{0,10} {1,20}", "Task Id",
                                  "Status");
                foreach (var t in tasks)
                    Console.WriteLine("{0,10} {1,20}",
                                      t.Id, t.Status);
            }
        }

        static async Task Send()
        {
            //string url = "http://webcode.me";
            ////List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
            //for (int i = 0; i < 50000; i++)
            //{
            //    //tasks.Add(httpClient.GetAsync(url));
            //    try
            //    {
            //        if ((await httpClient.GetAsync(url)).StatusCode == HttpStatusCode.OK)
            //            statistic.Collect(StatisticType.Ok);
            //        else
            //            statistic.Collect(StatisticType.Failed);
            //    }
            //    catch (Exception e)
            //    {
            //        statistic.Collect(StatisticType.Failed);
            //    }
            //}
            //foreach (var task in tasks)
            //{

            //}
            Console.WriteLine("Task ended");
        }
    }
}
