using ArtemisBanking.Core.Application.Dtos.Installment;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IInstallmentService : IGenericService<InstallmentDto>
    {
        Task<List<InstallmentDto>> GetAllWithInclude();
    }
}
