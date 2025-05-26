using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;

using Saras.SystemFramework.Core.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace Saras.eMarking.RCScheduler.Jobs.Utilities
{
    public static class HttpClientRequestHandler
    {
        static SarasLogger Log = new SarasLogger(typeof(HttpClientRequestHandler));
        public static string PostApiHandler(string ApiUrl, string requestObject, string ContentType, int timeoutSeconds)
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                Log.LogInfo("Entered API URL: " + ApiUrl);
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.Timeout = httpTimeout;

                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    using (var tokenSource = new CancellationTokenSource(httpTimeout))
                    {
                        httpResponse = client.PostAsync(ApiUrl, new StringContent(requestObject, Encoding.UTF8, ContentType), tokenSource.Token).Result;
                    }
                    if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        responseData = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else if (httpResponse != null)
                    {
                        Log.LogInfo("HTTP ERROR --> " + httpResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("ERROR --> ApiRequestHandler: HttpApiRequestHandler --> Method: PostApiHandler \n Exception: " + ex.Message + "\n StackRace :" + ex.StackTrace, ex);
            }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }
        public static string PutApiHandler(string ApiUrl, string requestObject, string ContentType, int timeoutSeconds)
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                Log.LogInfo("Entered API URL: " + ApiUrl);
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.Timeout = httpTimeout;

                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    using (var tokenSource = new CancellationTokenSource(httpTimeout))
                    {
                        httpResponse = client.PutAsync(ApiUrl, new StringContent(requestObject, Encoding.UTF8, ContentType), tokenSource.Token).Result;
                    }
                    if (httpResponse != null && httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Log.LogInfo("Success: API URL: " + ApiUrl);
                        responseData = JsonConvert.DeserializeObject<object>(httpResponse.Content.ReadAsStringAsync().Result.ToString()).ToString();
                    }
                    else if (httpResponse != null)
                    {
                        Log.LogInfo("HTTP ERROR --> " + httpResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("ERROR --> ApiRequestHandler: HttpApiRequestHandler --> Method: PostApiHandler \n Exception: " + ex.Message + "\n StackRace :" + ex.StackTrace, ex);
            }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }
        public static byte[] PostApiHandlerByte(string ApiUrl, string requestObject, string ContentType)
        {
            HttpResponseMessage httpResponse = null;
            byte[] stream = null;
            try
            {
                Log.LogInfo("Entered API URL: " + ApiUrl);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    httpResponse = client.PostAsync(ApiUrl, new StringContent(requestObject, UnicodeEncoding.UTF8, ContentType)).Result;
                    if (httpResponse != null && httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Log.LogInfo("Success: API URL: " + ApiUrl);
                        stream = JsonConvert.DeserializeObject<byte[]>(httpResponse.Content.ReadAsStringAsync().Result.ToString());
                    }
                    else if (httpResponse != null)
                    {
                        Log.LogInfo("HTTP ERROR --> " + httpResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("ERROR --> ApiRequestHandler: HttpApiRequestHandler --> Method: PostApiHandler \n Exception: " + ex.Message + "\n StackRace :" + ex.StackTrace, ex);
            }
            finally
            {
                httpResponse = null;
            }
            return stream;
        }

        public static string GetApiHandler(string ApiUrl, string ContentType, string BaseUrl = "", int timeoutSeconds = 0)
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                Log.LogInfo("Entered API URL: " + ApiUrl);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                using (var client = new HttpClient())
                {
                    var httpTimeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.Timeout = httpTimeout;

                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    using (var tokenSource = new CancellationTokenSource(httpTimeout))
                    {
                        httpResponse = client.GetAsync(ApiUrl, tokenSource.Token).Result;
                    }
                    if (httpResponse != null && httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        Log.LogInfo("Success: API URL: " + ApiUrl);
                        responseData = JsonConvert.DeserializeObject<object>(httpResponse.Content.ReadAsStringAsync().Result.ToString()).ToString();
                    }
                    else if (httpResponse != null)
                    {
                        Log.LogInfo("HTTP ERROR --> " + httpResponse.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError("ERROR --> ApiRequestHandler: HttpApiRequestHandler --> Method: PostApiHandler \n Exception: " + ex.Message + "\n StackRace :" + ex.StackTrace, ex);
            }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }

        public static HttpResponseMessage PostAPIAsJson(string ApiUrl, HttpContent postContent, string ContentType, string token = null)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
            Uri postUri = null;
            System.Net.Http.HttpResponseMessage httpResponse = null;
            try
            {
                if (!String.IsNullOrEmpty(ApiUrl))
                {
                    postUri = new Uri(ApiUrl);
                }
                else
                {
                    throw new Exception("APi url is empty");
                }
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(ContentType));

                    if (!String.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                    postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(ContentType);
                    httpResponse = httpClient.PostAsync(postUri, postContent).Result;
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error while posting data via API" + ex.StackTrace, ex);
                httpResponse = new System.Net.Http.HttpResponseMessage();
                httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                httpResponse.ReasonPhrase = ex.Message;
                httpResponse.Content = new System.Net.Http.StringContent(ex.StackTrace);
            }
            return httpResponse;
        }

        public static string PostApiMultiFormHandler(string url, string requestObject, string filePath = null)
        {
            var responseContent = string.Empty;

            using (var client = new HttpClient())
            {
                var httpTimeout = TimeSpan.FromSeconds(600);
                client.Timeout = httpTimeout;

                using (var formData = new MultipartFormDataContent())
                {
                    // Add class object as JSON 
                    var content = new StringContent(requestObject);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    formData.Add(content, "Payload");

                    // Add file to form data
                    if (filePath != null)
                    {
                        StreamContent fileContent = new StreamContent(File.OpenRead(filePath));
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = Path.GetFileName(filePath)
                        };
                        formData.Add(fileContent, "FileContent", Path.GetFileName(filePath));
                    }

                    // Send POST request with form data

                    var response = client.PostAsync(url, formData).Result;
                    response.EnsureSuccessStatusCode();
                    responseContent = response.Content.ReadAsStringAsync().Result;


                }
            }

            return responseContent;
        }
    }
}
