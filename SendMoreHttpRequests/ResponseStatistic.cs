namespace SendMoreHttpRequests
{
    public class ResponseStatistic : IDisposable
    {
        private Timer? _timer = null;
        private int _total;
        private int _ok;
        private int _failed;

        //public ResponseStatistic()
        //{
        //    _timer = new Timer(GetStatistics, null, 60000, 60000);
        //}

        private void GetStatistics(object? _)
        {
            Console.WriteLine("Всего запросов: {0}", Total);
            Console.WriteLine("Успешных: {0}", Ok);
            Console.WriteLine("Остальные: {0}", Failed);
        }

        public void Collect(StatisticType type)
        {
            _timer ??= new Timer(GetStatistics, null, 60000, 60000);
            _total++;
            switch (type)
            {
                case StatisticType.Ok:
                    _ok++;
                    break;
                case StatisticType.Failed:
                    _failed++;
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public int Ok { get => _ok; }
        public int Failed { get => _failed; }
        public int Total { get => _total; }
    }
}
