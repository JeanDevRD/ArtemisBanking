using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Transaction;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    public class CasherDashboardController : Controller
    {
        private readonly ICasherDashboardService _casherService;
        private readonly IMapper _mapper;

        public CasherDashboardController(ICasherDashboardService casherService, IMapper mapper)
        {
            _casherService = casherService;
            _mapper = mapper;
        } 

        public async Task<IActionResult> Index()
        {

            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var totalTransactions = await _casherService.CountTransactionsByCasherLog(cashierId);
            var totalDeposits = await _casherService.CountDepositByCasherLog(cashierId);
            var totalWithdrawals = await _casherService.CountWithdrawaltByCasherLog(cashierId);
            var totalPaids = await _casherService.CountPaidsByCasherLog(cashierId);

            var homeCasherViewModel = new HomeCasherViewModel
            {
                TotalTransaction = _mapper.Map<CountTransactionViewModel>(totalTransactions.Result),
                TotalDeposit = _mapper.Map<CountDepositViewModel>(totalDeposits.Result),
                TotalWithdrawal = _mapper.Map<CountWithdrawalViewModel>(totalWithdrawals.Result),
                TotalPaids = _mapper.Map<CountPaidsViewModel>(totalPaids.Result)
            };

            return View("Index", homeCasherViewModel);
        }
    }
}
