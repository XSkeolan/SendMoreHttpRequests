namespace SendMoreHttpRequests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            string url = "http://webcode.me";
            List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
            List<HttpResponseMessage> data = new List<HttpResponseMessage>();
            Timer timer = new Timer(Statictic, data, 60000, 60000);
            while (true)
            {
                for (int i = 0; i < 10000; i++)
                    tasks.Add(httpClient.GetAsync(url));
                foreach (var task in tasks)
                    data.Add(await task);
                tasks.Clear();
            }
        }
        private static void Statictic(object? data)
        {
            if (data != null)
            {
                ResponseStatistic statistic = new ResponseStatistic(data as List<HttpResponseMessage>);
                Console.WriteLine("Всего запросов за минуту - {0}", statistic.Total);
                Console.WriteLine("Количество успешных - {0}", statistic.Ok);
                Console.WriteLine("Остальные(не успешные) - {0}", statistic.Failed);
            }
        }
    }
}
