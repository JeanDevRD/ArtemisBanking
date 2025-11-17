using Asp.Versioning;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class SavingsAccountController : BaseApiController
    {
        private readonly ISavingsAccountService _savingsAccountService;

        public SavingsAccountController(ISavingsAccountService savingsAccountService)
        {
            _savingsAccountService = savingsAccountService;
        }

      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SavingsAccountsHomeDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSavingsAccounts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? identificationNumber = null,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null)
        {
            try
            {
                pageSize = Math.Min(pageSize, 20);

                bool? isActive = status?.ToLower() == "cancelado" ? false : (status?.ToLower() == "activo" ? true : null);
                int? accountType = type?.ToLower() == "principal" ? 1 : (type?.ToLower() == "secundaria" ? 2 : null);

                var result = await _savingsAccountService.GetSavingAccountHome(identificationNumber, page, isActive, accountType);

                if (result.IsError || result.Result == null || !result.Result.Any())
                {
                    return NoContent();
                }

                var totalCount = result.Result.Count;

                return Ok(new
                {
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = result.Result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

   
        [HttpGet("{accountNumber}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SavingsAccountDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccountTransactions(string accountNumber)
        {
            try
            {
                var accounts = await _savingsAccountService.GetAllAsync();
                var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account == null)
                {
                    return NotFound(new { message = "Cuenta no encontrada" });
                }

                var result = await _savingsAccountService.GetSavingsAccountDetail(account.AccountNumber);

                if (result.IsError || result.Result == null)
                {
                    return NotFound(new { message = result.Message ?? "Cuenta no encontrada" });
                }

                return Ok(result.Result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SavingsAccountDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSecondarySavingsAccount([FromBody] CreateSecundarySavingsAccountsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(dto.UserId) || string.IsNullOrWhiteSpace(dto.AdminUserId))
                {
                    return BadRequest(new { message = "Se requieren los IDs de usuario y administrador" });
                }

                var result = await _savingsAccountService.AddSecondarySavingsAccount(dto);

                if (result.IsError)
                {
                    return BadRequest(new { message = result.Message });
                }

                return StatusCode(StatusCodes.Status201Created, result.Result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

  
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelSecondarySavingsAccount(string id)
        {
            try
            {
                var result = await _savingsAccountService.CancelSecondarySavingsAccount(id);

                if (result.IsError)
                {
                    if (result.Message != null && result.Message.Contains("no encontrada"))
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