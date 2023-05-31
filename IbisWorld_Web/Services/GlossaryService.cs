using IbisWorld_Web.Models;
using IbisWorld_Web.Services.IServices;

namespace IbisWorld_Web.Services
{
    public class GlossaryService : BaseService, IGlossaryService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string glossaryUrl;

        public GlossaryService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            glossaryUrl = configuration.GetValue<string>("ServiceUrls:IbisAPI");
        }

        public Task<T> CreateAsync<T>(TermDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = IbisWorld_Utility.SD.ApiType.Post,
                Data = dto,
                Uri = glossaryUrl + "/api/glossaryAPI"
            });      
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = IbisWorld_Utility.SD.ApiType.Delete,
                Uri = glossaryUrl + $"/api/glossaryAPI/{id}"
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = IbisWorld_Utility.SD.ApiType.Get,
                Uri = glossaryUrl + "/api/glossaryAPI"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = IbisWorld_Utility.SD.ApiType.Get,
                Uri = glossaryUrl + $"/api/glossaryAPI/{id}"
            });
        }

        public Task<T> UpdateAsync<T>(TermDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = IbisWorld_Utility.SD.ApiType.Put,
                Data = dto,
                Uri = glossaryUrl + $"/api/glossaryAPI/{dto.Id}"
            });
        }
    }
}
