
namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public interface IGenericRepository<Entity> where Entity : class
    {

        Task<Entity?> AddAsync(Entity entity);

        Task DeleteAsync(int Id);
        Task<List<Entity>> GetAllListAsync();
        Task<List<Entity>> GetAllListIncluideAsync(List<string> properties);
        IQueryable<Entity> GetAllQueryAsync();
        Task<Entity?> GetByIdAsync(int Id);
        IQueryable<Entity> GetQueryWithIncluide(List<string> properties);
        Task<Entity?> UpdateAsync(int Id, Entity entity);
    }
}