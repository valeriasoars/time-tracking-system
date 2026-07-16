using Microsoft.EntityFrameworkCore;
using SistemaPontos.data;
using SistemaPontos.Dto;
using SistemaPontos.enums;
using SistemaPontos.Helpers;
using SistemaPontos.models;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CompanyTotalHoursResponseDto> GetCompanyTotalHours(DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.PunchClocks.Include(p => p.User).AsQueryable();

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                query = query.Where(p => p.TimeStamp >= startDateTime);
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
                query = query.Where(p => p.TimeStamp <= endDateTime);
            }

            var entries = await query.ToListAsync();

            var dailyEntries = TimeCalculationHelper.CalculateDailyHours(entries);

            var employeesSummary = dailyEntries
                .GroupBy(e => e.Employee)
                .Select(g => new EmployeeHoursDto
                {
                    Name = g.Key,
                    HoursWorked = Math.Round(g.Sum(e => e.HoursWorked), 2)
                })
                .ToList();

            double totalCompanyHours = Math.Round(employeesSummary.Sum(e => e.HoursWorked), 2);

            return new CompanyTotalHoursResponseDto
            {
                TotalHours = totalCompanyHours,
                Employees = employeesSummary
            };
        }
    }
}
