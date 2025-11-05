using ArtemisBanking.Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        public readonly ArtemisBankingContextSqlServer _context;
        public GenericRepository(ArtemisBankingContextSqlServer context)
        {
            _context = context;
        }

        public virtual async Task<Entity?> AddAsync(Entity entity)
        {
            await _context.Set<Entity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<Entity?> UpdateAsync(int Id, Entity entity)
        {
            var entry = await _context.Set<Entity>().FindAsync(Id);
            if (entry != null)
            {
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            return null;
        }

        public virtual async Task DeleteAsync(int Id)
        {
            var entity = await _context.Set<Entity>().FindAsync(Id);
            if (entity != null)
            {
                _context.Set<Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<Entity?> GetByIdAsync(int Id)
        {
            return await _context.Set<Entity>().FindAsync(Id);
        }

        public virtual async Task<List<Entity>> GetAllListAsync()
        {
            return await _context.Set<Entity>().ToListAsync();
        }

        public virtual async Task<List<Entity>> GetAllListIncluideAsync(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var item in properties)
            {
                query = query.Include(item);
            }

            return await query.ToListAsync();
        }

        public virtual IQueryable<Entity> GetAllQueryAsync()
        {
            return _context.Set<Entity>().AsQueryable();
        }

        public virtual IQueryable<Entity> GetQueryWithIncluide(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();
            foreach (var item in properties)
            {
                query = query.Include(item);
            }

            return query;
        }


    }
}
