using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class GenericService<EntityDto, Entity> : IGenericService<EntityDto>
        where EntityDto : class
        where Entity : class
    {

        private readonly IGenericRepository<Entity> _repo;
        private readonly IMapper _mapper;


        public GenericService(IGenericRepository<Entity> repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        public virtual async Task<EntityDto> AddAsync(EntityDto entityDto)
        {
            try
            {
                if (entityDto == null)
                {
                    return null!;
                }
                var entity = _mapper.Map<Entity>(entityDto);
                var result = await _repo.AddAsync(entity);
                return _mapper.Map<EntityDto>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error deleting entity");

            }

        }

        public virtual async Task<List<EntityDto>> GetAllAsync()
        {
            try
            {
                var entities = await _repo.GetAllListAsync();
                return _mapper.Map<List<EntityDto>>(entities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public virtual async Task<EntityDto> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<EntityDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public virtual async Task<EntityDto> UpdateAsync(int id, EntityDto entityDto)
        {
            try
            {
                if (entityDto == null)
                {
                    return null!;
                }
                var entity = _mapper.Map<Entity>(entityDto);
                var result = await _repo.UpdateAsync(id, entity);
                return _mapper.Map<EntityDto>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
    }
}
