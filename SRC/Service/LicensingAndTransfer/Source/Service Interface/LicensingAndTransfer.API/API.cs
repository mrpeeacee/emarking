using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicensingAndTransfer.API
{
    public class API
    {
        public System.Net.Http.HttpResponseMessage PostAPIAsJson(Uri postUri, System.Net.Http.HttpContent postContent, String token = null)
        {
            System.Net.Http.HttpResponseMessage httpResponse = null;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    if (!String.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json");
                    httpResponse = httpClient.PostAsync(postUri, postContent).Result;
                }
            }
            catch (Exception ex) {
                Constants.Log.Error("Error while posting data via API", ex);
                httpResponse = new System.Net.Http.HttpResponseMessage();
                httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                httpResponse.ReasonPhrase = ex.Message;
                httpResponse.Content = new System.Net.Http.StringContent(ex.StackTrace);
            }

            return httpResponse;
        }
        public System.Net.Http.HttpResponseMessage PostAPIAsUrlEncoded(Uri postUri, System.Net.Http.HttpContent postContent, String token = null)
        {
            System.Net.Http.HttpResponseMessage httpResponse = null;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    if (!String.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    postContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                    httpResponse = httpClient.PostAsync(postUri, postContent).Result;
                }
            }
            catch (Exception ex)
            {
                Constants.Log.Error("Error while posting data via API", ex);
                httpResponse = new System.Net.Http.HttpResponseMessage();
                httpResponse.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                httpResponse.ReasonPhrase = ex.Message;
                httpResponse.Content = new System.Net.Http.StringContent(ex.StackTrace);
            }

            return httpResponse;
        }
    }
}
