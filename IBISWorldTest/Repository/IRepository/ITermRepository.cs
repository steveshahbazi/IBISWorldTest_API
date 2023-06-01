using IBISWorld_API.Models;
using System.Linq.Expressions;

namespace IBISWorld_API.Repository.IRepository
{
    public interface ITermRepository : IRepository<Term>
    {
        Task<Term> UpdateAsync(Term term);
    }
}
