using IbisWorld_Web.Models;
using IbisWorld_Web.Services.IServices;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace IbisWorld_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse ResponseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
            ResponseModel = new APIResponse();
        }

        public BaseService()
        {
            ResponseModel = new APIResponse();
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("IbisAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Uri);
                if(apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case IbisWorld_Utility.SD.ApiType.Post:
                        message.Method = HttpMethod.Post;
                        break;
                    case IbisWorld_Utility.SD.ApiType.Put:
                        message.Method = HttpMethod.Put;
                        break;
                    case IbisWorld_Utility.SD.ApiType.Delete:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiResponse = null;
                apiResponse = await client.SendAsync(message);
                if (apiResponse != null)
                {
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    if (!apiResponse.IsSuccessStatusCode)
                        throw new Exception($"Request cannot be done: {apiContent}");

                    return APIResponse;
                }
                else
                    throw new Exception("Response is null");
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false,
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
