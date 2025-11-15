using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.AdminDashboard;
using ArtemisBanking.Core.Application.ViewModels.HomeAdmin;
using ArtemisBankingWebApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace ArtemisBankingWebApp.Controllers
{

    public class HomeAdminController : Controller
    {
        private readonly IAdminDashboardService _adminHomeService;
        private readonly IMapper _mapper;

        public HomeAdminController(IAdminDashboardService adminHomeService, IMapper mapper)
        {
            _adminHomeService = adminHomeService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var averageLoansPerClient = await _adminHomeService.AverageLoansPerClient();
            var totalClientsForWebApi = await _adminHomeService.TotalClientsForWebApi();
            var totalClientsForWebApp = await _adminHomeService.TotalClientsForWebApp();
            var totalCreditCardActive = await _adminHomeService.TotalCreditCardActive();
            var totalFinancialProducts = await _adminHomeService.TotalFinancialProducts();
            var totalLoads = await _adminHomeService.TotalLoads();
            var totalPaymentHistory = await _adminHomeService.TotalPaymentHistory();
            var totalSavingAccount = await _adminHomeService.TotalSavingAccount();
            var totalTransactions = await _adminHomeService.TotalTransactions();

            var homeAdminViewModel = new HomeAdminViewModel
            {
                loanAverageClient = _mapper.Map<LoanAverageClientViewModel>(averageLoansPerClient.Result),
                clientsViewModel = _mapper.Map<TotalClientsViewModel>(totalClientsForWebApp.Result),
                creditCardCount = _mapper.Map<CreditCardCountViewModel>(totalCreditCardActive.Result),
                totalFinancial = _mapper.Map<TotalFinancialProductViewModel>(totalFinancialProducts.Result),
                loadCount = _mapper.Map<LoadCountViewModel>(totalLoads.Result),
                paymentHistory = _mapper.Map<PaymentHistoryViewModel>(totalPaymentHistory.Result),
                totalSaving = _mapper.Map<TotalSavingAccountViewModel>(totalSavingAccount.Result),
                historicTransaction = _mapper.Map<HistoricTransactionViewModel>(totalTransactions.Result)
            };

            return View(homeAdminViewModel);
        }

    }
}


/*
using System.Diagnostics;
using iii.Models;
using Microsoft.AspNetCore.Mvc;

namespace iii.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
*/