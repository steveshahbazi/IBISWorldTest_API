using IBISWorldTest.Models;
using System.Linq.Expressions;

namespace IBISWorldTest.Repository.IRepository
{
    public interface ITermRepository : IRepository<Term>
    {
        Task<Term> UpdateAsync(Term term);
    }
}
