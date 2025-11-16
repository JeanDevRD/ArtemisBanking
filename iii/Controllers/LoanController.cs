using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewM.Loan;
using ArtemisBanking.Core.Application.ViewModel.Loan;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ArtemisBankingWebApp.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IMapper _mapper;

        public LoanController(ILoanService loanService, IMapper mapper)
        {
            _loanService = loanService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? page, string? cedula = null, bool? isActive = null)
        {
            var pageNumber = page ?? 1;
            var pageSize = 20;

            ResultDto<List<LoanListDto>> loansResult;

            if (!string.IsNullOrEmpty(cedula))
            {
                loansResult = await _loanService.GetLoansByUserIdentity(cedula, isActive ?? true);
            }
            else
            {
                loansResult = await _loanService.GetAllLoansAsync(isActive);
            }

            if (loansResult.IsError || loansResult.Result == null || loansResult.Result.Count == 0)
            {

                return View("Index", new StaticPagedList<LoanListViewModel>(new List<LoanListViewModel>(),
                    pageNumber,
                    pageSize,
                    0));
            }

            var allLoans = _mapper.Map<List<LoanListViewModel>>(loansResult.Result);
            var totalCount = allLoans.Count;
            var pagedLoans = allLoans.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var pagedList = new StaticPagedList<LoanListViewModel>(pagedLoans, pageNumber, pageSize, totalCount);

            ViewBag.FilterCedula = cedula;
            ViewBag.IsActive = isActive;

            return View("Index", pagedList);
        }





        public IActionResult Create(string userId)
        {
            return View("Create", new CreateLoanRequestViewModel
            {
                UserId = userId,
                ApprovedByUserId = "", 
                Amount = 0,
                TermMonths = 6,
                AnnualInterestRate = 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLoanRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Valores no validos, intentelo denuevo";
                return View("Create", vm);
            }

            var dto = _mapper.Map<CreateLoanRequestDto>(vm);

            var result = await _loanService.AddLoanAsync(dto);

            if (result.IsError)
            {
                TempData["Error"] = result.Message ?? "Error al crear prestamo" + result.Message;
                return View("Create", vm);
            }

            TempData["Success"] = $"Préstamo #{result.Result!.LoanNumber} creado exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SelectClient()
        {
            var result = await _loanService.GetElegibleUserForLoan();

            if (result.IsError || result.Result == null)
            {
                TempData["Error"] = result.Message ?? "No hay clientes elegibles";
                return RedirectToAction("Index");
            }

            var clients = _mapper.Map<List<ElegibleUserForLoanViewModel>>(result.Result);
            return View("SelectClient", clients);
        }

        public async Task<IActionResult> Detail(int loanId)
        {
            var loanResult = await _loanService.GetLoanDetailAsync(loanId);

            if (loanResult.IsError || loanResult.Result == null)
            {
                TempData["Error"] = loanResult.Message ?? "Préstamo no encontrado";
                return RedirectToAction("Index");
            }

            var vm = _mapper.Map<LoanDetailViewModel>(loanResult.Result);

            return View("Detail", vm);
        }

        public async Task<IActionResult> Edit(int loanId)
        {
            var loanResult = await _loanService.GetLoanDetailAsync(loanId);

            if (loanResult.IsError || loanResult.Result == null)
            {
                TempData["Error"] = loanResult.Message ?? "Préstamo no encontrado";
                return RedirectToAction("Index");
            }

            var vm = new UpdateInterestRateViewModel
            {
                AnnualInterestRate = loanResult.Result.AnnualInterestRate
            };

            ViewBag.LoanId = loanId;
            return View("Edit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int loanId, UpdateInterestRateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", vm);
            }

            var result = await _loanService.UpdateInterestRateAsync(loanId, vm.AnnualInterestRate);

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al actualizar la tasa de interés");
                return View("Edit", vm);
            }

            TempData["Success"] = $"Tasa de interés actualizada para el préstamo #{result.Result!.LoanNumber}.";
            return RedirectToAction("Index");
        }
    }
}
