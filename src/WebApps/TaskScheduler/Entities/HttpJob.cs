using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TaskScheduler.Entities
{
    public class HttpJob : Job
    {
        public HttpJob()
        {
            JobType = "HttpJob";
        }

        public string Url { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public string Body { get; set; }


        public override string Execute()
        {
            using (var http = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.RequestUri = new Uri(Url);
                request.Method = new HttpMethod(Method);

                if (!string.IsNullOrWhiteSpace(Body))
                {
                    request.Content = new StringContent(Body);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
                }

                http.Timeout = TimeSpan.FromMinutes(30);

                var response = http.SendAsync(request).Result.Content.ReadAsStringAsync().Result;

                return response;
            }
        }
    }
}