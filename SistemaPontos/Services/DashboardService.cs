using Microsoft.EntityFrameworkCore;
using SistemaPontos.data;
using SistemaPontos.Dto;
using SistemaPontos.enums;
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

            var groupedByDay = entries.GroupBy(p => new
            {
                p.UserId,
                EmployeeName = p.User?.Name ?? "Funcionário",
                Date = DateOnly.FromDateTime(p.TimeStamp)
            });

            var dailyCalculatedHours = new List<EmployeeDailyHours>();

            foreach (var group in groupedByDay)
            {
                var checkInEntry = group.FirstOrDefault(p => p.TypePunch == TypePunch.CHECKIN);
                var checkOutEntry = group.FirstOrDefault(p => p.TypePunch == TypePunch.CHECKOUT);

                double hoursWorked = 0;

                if (checkInEntry != null && checkOutEntry != null)
                {
                    TimeSpan difference = checkOutEntry.TimeStamp - checkInEntry.TimeStamp;
                    hoursWorked = Math.Round(difference.TotalHours, 2);
                }

                dailyCalculatedHours.Add(new EmployeeDailyHours
                {
                    UserId = group.Key.UserId,
                    Name = group.Key.EmployeeName,
                    Hours = hoursWorked
                });
            }
            
            var employeesSummary = dailyCalculatedHours.GroupBy(e => new {e.UserId, e.Name}).Select(g => new EmployeeHoursDto
            {
                Name = g.Key.Name,
                HoursWorked = Math.Round(g.Sum(e => e.Hours), 2)
            }).ToList();

            double totalCompanyHours = Math.Round(employeesSummary.Sum(e => e.HoursWorked), 2);

            return new CompanyTotalHoursResponseDto
            {
                TotalHours = totalCompanyHours,
                Employees = employeesSummary
            };
        }

        private class EmployeeDailyHours
        {
            public Guid UserId { get; set; }
            public string Name { get; set; } = string.Empty;
            public double Hours { get; set; }
        }
    }
}
