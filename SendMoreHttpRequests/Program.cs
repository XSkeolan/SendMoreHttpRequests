using System.Net;
namespace SendMoreHttpRequests
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            ResponseStatistic statistic;
            string url = "http://localhost/";
            int number = 101;
            Console.WriteLine("Time start - {0}", DateTime.Now);
            List<Task> tasks = new List<Task>();
            statistic = new ResponseStatistic();
            for (int i = 0; i < 100; i++)
                tasks.Add(Task.Run(() =>
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
                    Console.WriteLine("{0} of 100 tasks completed 15000 tasks!!!", 100 - --number);
                    tasksRequest.Clear();
                }));
            try
            {
                Task.WaitAll(tasks.ToArray());
                foreach (var t in tasks)
                    Console.WriteLine("{0,10} {1,20}", t.Id, t.Status);
            }
            catch (AggregateException)
            {
                Console.WriteLine("Status of tasks:\n");
                Console.WriteLine("{0,10} {1,20}", "Task Id", "Status");
                foreach (var t in tasks)
                    Console.WriteLine("{0,10} {1,20}", t.Id, t.Status);
            }
            Console.WriteLine("Time end - {0}", DateTime.Now);
        }
    }
}
