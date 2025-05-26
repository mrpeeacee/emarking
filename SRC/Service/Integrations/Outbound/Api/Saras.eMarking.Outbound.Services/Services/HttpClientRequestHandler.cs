using iExamSync;
using System.Net.Http.Headers;
using System.Net;
using Renci.SshNet;
using Saras.eMarking.Outbound.Services.Model;
using System.Net.Cache;
using Microsoft.Extensions.Logging;

namespace Saras.eMarking.Outbound.Services.Services
{
    public static class HttpClientRequestHandler
    {
        private static readonly ILogger _logger;

        public static async Task<HttpResponseMessage> PostAsync(string PostUrl, string BodyContent, Dictionary<string, string> Headers, string ProxyUrl, string ContentType = "application/json")
        {
            HttpClientHandler handler = new()
            {
                UseProxy = true,
                Proxy = WebClientRequestHandler.GetProxy(ProxyUrl)
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, PostUrl);

            if (Headers != null && Headers.Count > 0)
            {
                foreach (KeyValuePair<string, string> entry in Headers)
                {
                    client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                }
            }

            var content = new StringContent(BodyContent, null, ContentType);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }
       
        public static async Task<HttpResponseMessage> GetAsync(string ProxyUrl, string ApiUrl)
        {
            HttpResponseMessage httpResponse;

            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
            HttpClientHandler handler = new()
            {
                UseProxy = true,
                Proxy = WebClientRequestHandler.GetProxy(ProxyUrl)
            };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpResponse = await client.GetAsync(ApiUrl);
                httpResponse.EnsureSuccessStatusCode();
            }

            return httpResponse;
        }
    }
}
