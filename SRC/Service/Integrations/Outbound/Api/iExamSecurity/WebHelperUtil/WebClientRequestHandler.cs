using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;

using java.lang.reflect;
using Newtonsoft.Json;

namespace iExamSync
{
    public static class WebClientRequestHandler
    {
        public static string PostApiHandler(string ProxyUrl, string ApiUrl, string requestObject, string ContentType, Dictionary<string, string> requestHeader, int timeout = 0)
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = GetProxy(ProxyUrl)
                };
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    if (timeout > 0)
                    {
                        client.Timeout = TimeSpan.FromSeconds(timeout);
                    }
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    if (requestHeader != null && requestHeader.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> entry in requestHeader)
                        {
                            client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                            // do something with entry.Value or entry.Key
                        }

                    }
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    httpResponse = client.PostAsync(ApiUrl, new StringContent(requestObject, UnicodeEncoding.UTF8, ContentType)).Result;
                    if (httpResponse != null)
                    {
                        responseData = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                responseData = ex.Message;
            }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }
        public static string PutApiHandler(string ProxyUrl, string ApiUrl, string requestObject, string ContentType)
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = GetProxy(ProxyUrl)
                };
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    httpResponse = client.PutAsync(ApiUrl, new StringContent(requestObject, UnicodeEncoding.UTF8, ContentType)).Result;
                    if (httpResponse != null && httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        responseData = JsonConvert.DeserializeObject<Object>(httpResponse.Content.ReadAsStringAsync().Result.ToString()).ToString();
                    }
                    else if (httpResponse != null)
                    {
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }
        public static byte[] PostApiHandlerByte(string ProxyUrl, string ApiUrl, string requestObject, string ContentType)
        {
            HttpResponseMessage httpResponse = null;
            byte[] stream = null;
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = GetProxy(ProxyUrl)
                };
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                    httpResponse = client.PostAsync(ApiUrl, new StringContent(requestObject, UnicodeEncoding.UTF8, ContentType)).Result;
                    if (httpResponse != null && httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        stream = JsonConvert.DeserializeObject<byte[]>(httpResponse.Content.ReadAsStringAsync().Result.ToString());
                    }
                    else if (httpResponse != null)
                    {
                    }
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                httpResponse = null;
            }
            return stream;
        }
        public static WebProxy GetProxy(string ProxyUrl)
        {
            var proxy = new WebProxy
            {
                Address = new Uri(ProxyUrl),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
            };
            return proxy;
        }
        public static string GetApiHandler(string ProxyUrl, string ApiUrl, string ContentType, string BaseUrl = "")
        {
            HttpResponseMessage httpResponse = null;
            string responseData = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072 | (SecurityProtocolType)12288;
                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = GetProxy(ProxyUrl)
                };
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(ApiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    httpResponse = client.GetAsync(ApiUrl).Result;
                    responseData = Convert.ToString(httpResponse.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                httpResponse = null;
            }
            return responseData;
        }

        public static System.Net.Http.HttpResponseMessage PostAPIAsJson(string ApiUrl, System.Net.Http.HttpContent postContent, String ContentType, String token = null)
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
                httpResponse = new System.Net.Http.HttpResponseMessage();
                httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                httpResponse.ReasonPhrase = ex.Message;
                httpResponse.Content = new System.Net.Http.StringContent(ex.StackTrace);
            }
            return httpResponse;
        }
    }
}
