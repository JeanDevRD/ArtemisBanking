namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IGenericService<TDto>
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<TDto> AddAsync(TDto dto);
        Task<TDto> UpdateAsync(int id, TDto dto);
        Task DeleteAsync(int id);

    }
}