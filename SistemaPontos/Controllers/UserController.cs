using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaPontos.Dto;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("register/employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto dto)
        {
            try
            {
                var user = await _userService.RegisterEmployee(dto); 
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
