using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task SaveAsync();
    }
}
