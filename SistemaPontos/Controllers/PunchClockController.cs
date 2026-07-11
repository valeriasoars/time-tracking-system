using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using SistemaPontos.Dto;
using SistemaPontos.Services.Interfaces;
using System.Security.Claims;

namespace SistemaPontos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PunchClockController : ControllerBase
    {
        private readonly IPunchClockService _punchClockService;
        public PunchClockController(IPunchClockService punchClockService)
        {
            _punchClockService = punchClockService;
        }

        
        [HttpGet("history")]
        public async Task<IActionResult> GetTimeEntryHistory()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var user = await _punchClockService.GetTimeEntryHistory(userId);
                return Ok(user);
            }catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }  
        }

        [HttpGet("admin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTimeEntries([FromQuery] Guid? employeeId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            try
            {
                var allEntries = await _punchClockService.GetAllTimeEntries(employeeId, startDate, endDate);
                return Ok(allEntries);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RecordTimeEntry([FromBody] RecordTimeEntryDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var result = await _punchClockService.RecordTimeEntry(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
