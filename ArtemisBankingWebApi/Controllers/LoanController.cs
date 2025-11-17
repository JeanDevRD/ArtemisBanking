using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class LoanController : BaseApiController
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<LoanListDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllLoans([FromQuery] int page = 1,[FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,[FromQuery] string? identificationNumber = null)
        {
            try
            {
                pageSize = Math.Min(pageSize, 20); 

                if (!string.IsNullOrWhiteSpace(identificationNumber))
                {
                    
                    bool isActive = status?.ToLower() == "completados" ? false : true;
                    var loansByUser = await _loanService.GetLoansByUserIdentity(identificationNumber, isActive);

                    if (loansByUser.IsError || loansByUser.Result == null || !loansByUser.Result.Any())
                    {
                        return NoContent();
                    }

                    return Ok(new { data = loansByUser.Result });
                }

                bool? isActiveLoan = status?.ToLower() == "completados" ? false :
                           status?.ToLower() == "activos" ? true : null;

                var loansResult = await _loanService.GetAllLoansAsync(isActiveLoan);

                if (loansResult == null || loansResult.IsError || loansResult.Result == null || !loansResult.Result.Any())
                {
                    return NoContent();
                }

                var loans = loansResult.Result;

               
                if (!string.IsNullOrWhiteSpace(status))
                {
                   
                    if (status.ToLower() == "completados")
                    {
                        loans = loans.Where(l => l.PaymentStatus.ToLower() == "en mora").ToList();
                    }
                    else if (status.ToLower() == "completados")
                    {
                        loans = loans.Where(l => l.TotalInstallments > 0 &&l.PaidInstallments == l.TotalInstallments).ToList();
                    }
                }

                var totalCount = loans.Count;
                var paginatedLoans = loans.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = paginatedLoans
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

  
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLoanDetail(string id)
        {
            try
            {
                var result = await _loanService.GetLoanDetailAsync(id);

                if (result.IsError || result.Result == null)
                {
                    return NotFound(new { message = result.Message ?? "Préstamo no encontrado" });
                }

                return Ok(result.Result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LoanDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (adminId == null)
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario administrador." });
                }
                dto.ApprovedByUserId = adminId;

                var result = await _loanService.AddLoanAsync(dto);

                if (result.IsError)
                {
                    if (result.Message != null && result.Message.Contains("alto riesgo"))
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new { message = result.Message });
                    }

                    return BadRequest(new { message = result.Message });
                }

                return StatusCode(StatusCodes.Status201Created, result.Result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/rate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInterestRate(string id, [FromBody] UpdateInterestRateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loanService.UpdateInterestRateAsync(id, dto.AnnualInterestRate);

                if (result.IsError)
                {
                    if (result.Message != null && result.Message.Contains("no encontrado"))
                    {
                        return NotFound(new { message = result.Message });
                    }

                    return BadRequest(new { message = result.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}