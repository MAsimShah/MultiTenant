using DAL.ApplicationContext;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class GenericRepository<T>(ApplicationDbContext dBContext) : IGenericRepository<T> where T : class
    {
        public async Task AddAsync(T entity)
        {
            try
            {
                await dBContext.Set<T>().AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await dBContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
