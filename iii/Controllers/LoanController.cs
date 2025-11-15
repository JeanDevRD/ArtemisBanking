using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewM.Loan;
using ArtemisBanking.Core.Application.ViewModel.Loan;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20, bool? isActive = null)
        {
            var loansResult = await _loanService.GetAllLoansAsync(isActive);

            var data = new HomeLoanViewModel
            {
                Loans = _mapper.Map<List<LoanListViewModel>>(loansResult.Result),
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)loansResult.Result!.Count / pageSize)
            };

            return View("Index", data);
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
                return View("Create", vm);
            }

            var dto = _mapper.Map<CreateLoanRequestDto>(vm);

            var result = await _loanService.AddLoanAsync(dto);

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al crear el préstamo");
                return View("Create", vm);
            }

            TempData["Success"] = $"Préstamo #{result.Result!.LoanNumber} creado exitosamente.";
            return RedirectToAction("Index");
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
