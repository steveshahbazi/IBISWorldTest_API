﻿using IBISWorldTest.Models;
using System.Linq.Expressions;

namespace IBISWorldTest.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(T term);
        Task RemoveAsync(T term);
        Task SaveAsync();
    }
}
