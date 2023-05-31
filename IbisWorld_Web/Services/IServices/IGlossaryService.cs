using IbisWorld_Web.Models;

namespace IbisWorld_Web.Services.IServices
{
    public interface IGlossaryService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(TermDTO dto);
        Task<T> UpdateAsync<T>(TermDTO dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
