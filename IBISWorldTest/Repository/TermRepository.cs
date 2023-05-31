using IBISWorldTest.Data;
using IBISWorldTest.Models;
using IBISWorldTest.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IBISWorldTest.Repository
{
    public class TermRepository : ITermRepository
    {
        private readonly ApplicationDBContext _db;

        public TermRepository(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Term term)
        {
            await _db.Terms.AddAsync(term);
            await SaveAsync();
        }

        public async Task<Term> GetAsync(Expression<Func<Term, bool>>? filter, bool tracked = true)
        {
            IQueryable<Term> query = _db.Terms.OrderByDescending(t => t.Id);
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Term>> GetAllAsync(Expression<Func<Term, bool>>? filter)
        {
            IQueryable<Term> query = _db.Terms;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(Term term)
        {
            _db.Terms.Remove(term);
            await SaveAsync();
        }

        public async Task UpdateAsync(Term term)
        {
            _db.Terms.Update(term);
            await SaveAsync();
        }
        
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

