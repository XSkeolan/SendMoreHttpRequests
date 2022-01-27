using System.Net;

namespace SendMoreHttpRequests
{
    public class ResponseStatistic
    {
        public ResponseStatistic(List<HttpResponseMessage> responses)
        {
            Ok = responses.Count(x => x.StatusCode == HttpStatusCode.OK);
            Total = responses.Count;
            Failed = responses.Count(x=>x.StatusCode != HttpStatusCode.OK);
        }

        public int Ok { get; set; }
        public int Total { get; set; }
        public int Failed { get; set; }
    }
}
