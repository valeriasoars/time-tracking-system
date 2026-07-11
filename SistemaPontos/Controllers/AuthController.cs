using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaPontos.Dto;
using SistemaPontos.Services;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var login = await _userService.Login(dto);
                return Ok(login);

            }catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(RegisterAdminDto dto)
        {
            try
            {
                var user = await _userService.SignUp(dto); 
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
