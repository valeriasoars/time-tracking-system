using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin/reports")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetCompanyTotalHours([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
        {
            try
            {
                var companyHoursReport = await _dashboardService.GetCompanyTotalHours(startDate, endDate);
                return Ok(companyHoursReport);
            }catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
