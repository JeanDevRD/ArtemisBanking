using ArtemisBanking.Core.Application.Dtos.Merchant;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Services
{
    public class MerchantService : GenericService<MerchandDto,Merchant>, IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IMapper _mapper;
        public MerchantService(IMerchantRepository genericRepository, IMapper mapper) : base(genericRepository, mapper)
        {
            _merchantRepository = genericRepository;
            _mapper = mapper;
        }
    }
}
