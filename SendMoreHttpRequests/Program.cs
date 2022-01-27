using System.Net;
namespace SendMoreHttpRequests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            string url = "http://webcode.me";
            List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
            ResponseStatistic statistic = new ResponseStatistic();

            while (true)
            {
                for (int i = 0; i < 10000; i++)
                    tasks.Add(httpClient.GetAsync(url));
                foreach (var task in tasks)
                {
                    if ((await task).StatusCode == HttpStatusCode.OK)
                        statistic.CollectStatict(StatisticType.Ok);
                    else
                        statistic.CollectStatict(StatisticType.Failed);
                }
                tasks.Clear();
            }
        }
    }
}
