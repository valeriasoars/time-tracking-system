using SistemaPontos.Dto;

namespace SistemaPontos.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<CompanyTotalHoursResponseDto> GetCompanyTotalHours(DateOnly? startDate, DateOnly? endDate);
    }
}
