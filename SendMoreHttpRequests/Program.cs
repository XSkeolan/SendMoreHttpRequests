namespace SendMoreHttpRequests
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();
        static readonly string url = "https://google.com";
        static readonly List<Task<string>> tasks = new List<Task<string>>();

        static async Task Main(string[] args)
        {
            for (int i = 0; i < 10000; i++)
                tasks.Add(httpClient.GetStringAsync(url));

            Task.WaitAll(tasks.ToArray());

            var data = new List<string>();
            foreach (var task in tasks)
                data.Add(await task);

            foreach (var response in data)
                Console.WriteLine(response);
        }
    }
}
