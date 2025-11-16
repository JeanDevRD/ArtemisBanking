using Asp.Versioning;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    public class UsersController : BaseApiController
    {
        private readonly IAccountServiceForApi _accountService;

        public UsersController(IAccountServiceForApi accountService)
        {
            _accountService = accountService;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? role = null)
        {
            try
            {
                var users = await _accountService.GetAllUser();

                if (users == null || !users.Any())
                {
                    return NoContent();
                }

                users = users.Where(u => u.Role != "Merchant").ToList();


                if (!string.IsNullOrWhiteSpace(role))
                {
                    users = users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
                }


                users = users.OrderByDescending(u => u.Id).ToList();

                var totalCount = users.Count;
                var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = paginatedUsers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpGet("commerce")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommerceUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var users = await _accountService.GetAllUser();

                if (users == null || !users.Any())
                {
                    return NoContent();
                }
                var commerceUsers = users.Where(u => u.Role == "Merchant")
                                         .OrderByDescending(u => u.Id)
                                         .ToList();

                if (!commerceUsers.Any())
                {
                    return NoContent();
                }

                var totalCount = commerceUsers.Count;
                var paginatedUsers = commerceUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return Ok(new
                {
                    page,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    data = paginatedUsers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _accountService.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] SaveUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _accountService.RegisterUser(dto, null, true);

                if (result.HasError)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { errors = result.ErrorMessage });
                }

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("commerce/{commerceId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCommerceUser(int commerceId, [FromBody] SaveUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (dto.Role != "Merchant")
                {
                    return BadRequest(new { message = "El rol debe ser Merchant para este endpoint" });
                }

                var result = await _accountService.RegisterUser(dto, null, true);

                if (result.HasError)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { errors = result.ErrorMessage });
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] SaveUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _accountService.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                dto.Id = id;

                var result = await _accountService.EditUser(dto, null, false, true);

                if (result.HasError)
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { errors = result.Errors });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserStatus(string id, [FromBody] ChangeStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _accountService.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
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