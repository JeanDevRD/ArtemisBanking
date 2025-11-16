using Asp.Versioning;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class CreditCardController : BaseApiController
    {
        private readonly ICreditCardService _creditCardService;

        public CreditCardController(ICreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CreditCardListDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCreditCards(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? identificationNumber = null,
            [FromQuery] string? status = null)
        {
            try
            {
                pageSize = Math.Min(pageSize, 20);

                if (!string.IsNullOrWhiteSpace(identificationNumber))
                {
                    bool? isActive = status?.ToLower() == "cancelada" ? false : (status?.ToLower() == "activa" ? true : null);
                    var cardsByUser = await _creditCardService.GetAllCreditCardByIdentityUser(identificationNumber, isActive);

                    if (cardsByUser.IsError || cardsByUser.Result == null || !cardsByUser.Result.Any())
                    {
                        return NoContent();
                    }

                    return Ok(new { data = cardsByUser.Result });
                }

                var cardsResult = await _creditCardService.GetAllActiveCreditCard();

                if (cardsResult.IsError || cardsResult.Result == null || !cardsResult.Result.Any())
                {
                    return NoContent();
                }

                var cards = cardsResult.Result;

                var totalCount = cards.Count;
                var paginatedCards = cards.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = paginatedCards
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

      
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreditCardDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCreditCardDetail(int id)
        {
            try
            {
                var result = await _creditCardService.DetailCard(id);

                if (result.IsError || result.Result == null)
                {
                    return NotFound(new { message = result.Message ?? "Tarjeta no encontrada" });
                }

                return Ok(result.Result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreditCardDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCreditCard([FromBody] CreditCardRequestDto dto, [FromQuery] string adminUserId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(adminUserId))
                {
                    return BadRequest(new { message = "Se requiere el ID del administrador" });
                }

                var result = await _creditCardService.AddCreditCardAsync(dto, adminUserId);

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

        [HttpPatch("{id}/limit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCreditLimit(int id, [FromBody] UpdateCreditCardDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _creditCardService.UpdateCard(dto, id);

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

        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelCreditCard(int id)
        {
            try
            {
                var success = await _creditCardService.CancelatedCreditCard(id);

                if (!success)
                {
                    return BadRequest(new { message = "No se pudo cancelar la tarjeta. Verifique que no tenga deuda pendiente." });
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