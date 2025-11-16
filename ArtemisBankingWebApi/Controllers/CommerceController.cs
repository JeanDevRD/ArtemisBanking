using Asp.Versioning;
using ArtemisBanking.Core.Application.Dtos.Merchant;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class CommerceController : BaseApiController
    {
        private readonly IMerchantService _merchantService;

        public CommerceController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

       
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MerchandDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCommerces([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            try
            {
                var commerces = await _merchantService.GetAllAsync();

                if (commerces == null || !commerces.Any())
                {
                    return NoContent();
                }

                commerces = commerces.OrderByDescending(c => c.CreatedAt).ToList();

                if (!page.HasValue || !pageSize.HasValue)
                {
                    return Ok(new { data = commerces });
                }

                var validPageSize = Math.Min(pageSize.Value, 20);
                var totalCount = commerces.Count;
                var paginatedCommerces = commerces.Skip((page.Value - 1) * validPageSize).Take(validPageSize).ToList();

                return Ok(new
                {
                    page = page.Value,
                    pageSize = validPageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)validPageSize),
                    data = paginatedCommerces
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

  
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MerchandDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommerceById(int id)
        {
            try
            {
                var commerce = await _merchantService.GetByIdAsync(id);

                if (commerce == null)
                {
                    return NotFound(new { message = "Comercio no encontrado" });
                }

                return Ok(commerce);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MerchandDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCommerce([FromBody] CreateMerchantDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var merchantDto = new MerchandDto
                {
                    Id = 0,
                    Name = dto.Name,
                    Email = dto.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    AssociatedAccount = string.Empty, 
                    UserId = string.Empty 
                };

                var result = await _merchantService.AddAsync(merchantDto);

                if (result == null)
                {
                    return BadRequest(new { message = "Error al crear el comercio" });
                }

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

     
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCommerce(int id, [FromBody] UpdateMerchantDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var commerce = await _merchantService.GetByIdAsync(id);
                if (commerce == null)
                {
                    return NotFound(new { message = "Comercio no encontrado" });
                }

                commerce.Name = dto.Name;
                commerce.Email = dto.Email;

                var result = await _merchantService.UpdateAsync(id, commerce);

                if (result == null)
                {
                    return BadRequest(new { message = "Error al actualizar el comercio" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeCommerceStatus(int id, [FromBody] ChangeCommerceStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var commerce = await _merchantService.GetByIdAsync(id);
                if (commerce == null)
                {
                    return NotFound(new { message = "Comercio no encontrado" });
                }

                commerce.IsActive = dto.Status;

                var result = await _merchantService.UpdateAsync(id, commerce);

                if (result == null)
                {
                    return BadRequest(new { message = "Error al cambiar el estado del comercio" });
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