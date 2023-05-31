using IBISWorldTest.Data;
using IBISWorldTest.Models;
using IBISWorldTest.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IBISWorldTest.Repository
{
    public class TermRepository : Repository<Term>, ITermRepository
    {
        private readonly ApplicationDBContext _db;

        public TermRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Term> UpdateAsync(Term term)
        {
            term.UpdatedDate = DateTime.UtcNow;
            _db.Terms.Update(term);
            await _db.SaveChangesAsync();
            return term;
        }
        
    }
}

