using IBISWorldTest.Models;
using System.Linq.Expressions;

namespace IBISWorldTest.Repository.IRepository
{
    public interface ITermRepository
    {
        Task<List<Term>> GetAllAsync(Expression<Func<Term, bool>>? filter = null);
        Task<Term> GetAsync(Expression<Func<Term, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(Term term);
        Task RemoveAsync(Term term);
        Task UpdateAsync(Term term);
        Task SaveAsync();
    }
}
