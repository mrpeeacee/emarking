using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MediaLibrary.Utility
{
    public static class ApiHandler
    {
        public static HttpResponseMessage GetAPIAsJson(Uri postUri, string? token = null)
        {
            HttpResponseMessage? httpResponse = null;
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                httpClient.Timeout = TimeSpan.FromSeconds(300);
                httpResponse = httpClient.GetAsync(postUri).Result;
            }
            catch (Exception ex)
            {
                httpResponse = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message
                };
                if (!string.IsNullOrEmpty(ex.StackTrace))
                    httpResponse.Content = new StringContent(ex.StackTrace);
            }
            return httpResponse;
        }
        public static string PostApiHandler(string ApiUrl, string requestObject, string ContentType, int timeout = 0)
        {
            HttpResponseMessage? httpResponse = null;
            string responseData = string.Empty;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(ApiUrl);
                if (timeout > 0)
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                }
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));

                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                httpResponse = client.PostAsync(ApiUrl, new StringContent(requestObject, Encoding.UTF8, ContentType)).Result;
                if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    responseData = httpResponse.Content.ReadAsStringAsync().Result;
                }

            }
            return responseData;
        }
    }
}
