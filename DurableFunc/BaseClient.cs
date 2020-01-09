using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunc
{
    public class BaseClient
    {
        readonly HttpClient client;
        readonly BaseResponse baseresponse;

        
        public BaseClient(string baseAddress, string username, string password)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://127.0.0.1:8888"),
                UseProxy = false,
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            baseresponse = new BaseResponse();

        }

        public BaseClient(string baseAddress)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://127.0.0.1:8888"),
                UseProxy = false,
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            baseresponse = new BaseResponse();
        }

        //public async Task<string> GetcallAsync(string endpoint)
        //{
        //    HttpResponseMessage response = await client.GetAsync(endpoint + "/");
        //    HttpContent content = response.Content;
        //    return await content.ReadAsStringAsync();
        //}

        public async Task<BaseResponse> PostCallAsync(string endpoint, string jsonObject)
        {
            try
            {
                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                var response = client.PostAsync(endpoint, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                else
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                return baseresponse;

            }
            catch (Exception ex)
            {
                baseresponse.StatusCode = 0;
                baseresponse.ResponseMessage = (ex.Message ?? ex.InnerException.ToString());
            }
            return baseresponse;
        }

        public async Task<BaseResponse> PostCallbyPassingUrlParametersAsync(string endpoint, Dictionary<string, string> parameters)
        {
            try
            {
                var content = new FormUrlEncodedContent(parameters);

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(endpoint, UriKind.RelativeOrAbsolute),
                    Method = HttpMethod.Post,
                    Content = content
                };

                var response = client.SendAsync(request).GetAwaiter().GetResult();
                //var response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                else
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                return baseresponse;
            }
            catch (Exception ex)
            {
                baseresponse.StatusCode = 0;
                baseresponse.ResponseMessage = (ex.Message ?? ex.InnerException.ToString());
            }
            return baseresponse;
        }

        public async Task<BaseResponse> GetCallAsync(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(endpoint + "/").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                else
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                return baseresponse;
            }
            catch (Exception ex)
            {
                baseresponse.StatusCode = 0;
                baseresponse.ResponseMessage = (ex.Message ?? ex.InnerException.ToString());
            }
            return baseresponse;
        }

        public async Task<BaseResponse> GetCallV2Async(string endpoint)
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(endpoint + "/", UriKind.RelativeOrAbsolute),
                    Method = HttpMethod.Get
                };
                HttpResponseMessage response = Task.Run(() => client.SendAsync(request)).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }
                else
                {
                    baseresponse.ResponseMessage = await response.Content.ReadAsStringAsync();
                    baseresponse.StatusCode = (int)response.StatusCode;
                }

                return baseresponse;
            }
            catch (Exception ex)
            {
                baseresponse.StatusCode = 0;
                baseresponse.ResponseMessage = (ex.Message ?? ex.InnerException.ToString());
            }
            return baseresponse;
        }
    }
}
