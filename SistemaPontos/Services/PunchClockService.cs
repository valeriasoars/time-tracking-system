using Microsoft.EntityFrameworkCore;
using SistemaPontos.data;
using SistemaPontos.Dto;
using SistemaPontos.enums;
using SistemaPontos.models;
using SistemaPontos.Services.Interfaces;

namespace SistemaPontos.Services
{
    public class PunchClockService : IPunchClockService
    {
        private readonly AppDbContext _context;
        public PunchClockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdminTimeEntryResponseDto>> GetAllTimeEntries(Guid? employeeId, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.PunchClocks.Include(p => p.User).AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(p => p.UserId == employeeId.Value);
            }

            if (startDate.HasValue)
            {
                var startDateTime = startDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                query = query.Where(p => p.TimeStamp >=  startDateTime);
            }

            if (endDate.HasValue)
            {
                var endDateTime = endDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
                query = query.Where(p => p.TimeStamp <= endDateTime);
            }

            var entries = await query.ToListAsync();


            return ProcessAndCalculateHours(entries);
        }

        public async Task<IEnumerable<TimeEntryHistoryResponseDto>> GetTimeEntryHistory(Guid userId)
        {
            var entries = await _context.PunchClocks
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var calculatedData = ProcessAndCalculateHours(entries);

            return calculatedData.Select(c => new TimeEntryHistoryResponseDto
            {
                Date = c.Date,
                CheckIn = c.CheckIn ?? default,
                CheckOut = c.CheckOut ?? default,
                HoursWorked = c.HoursWorked
            });
        }

        public async Task<RecordTimeEntryResponseDto> RecordTimeEntry(Guid userId, RecordTimeEntryDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception("Usuário não encontrado.");
            }

            var currentTimestamp = DateTime.UtcNow;

            var newPunch = new PunchClock
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TimeStamp = currentTimestamp,
                TypePunch = dto.TypePunch,
            };

            _context.PunchClocks.Add(newPunch);
            await _context.SaveChangesAsync();

            string actionMessage = dto.TypePunch == TypePunch.CHECKIN ? "Entrada" : "Saída";

            return new RecordTimeEntryResponseDto
            {
                Message = $"{actionMessage} registrada com sucesso!",
                TimeStamp = currentTimestamp
            };
        }


        private IEnumerable<AdminTimeEntryResponseDto> ProcessAndCalculateHours(List<PunchClock> entries)
        {
            var groupedEntries = entries.GroupBy(p => new
            {
                p.UserId,
                EmployeeName = p.User?.Name ?? "Funcionário",
                Date = DateOnly.FromDateTime(p.TimeStamp)
            });

            var result = new List<AdminTimeEntryResponseDto>();

            foreach (var group in groupedEntries)
            {
                var checkInEntry = group.FirstOrDefault(p => p.TypePunch == TypePunch.CHECKIN);
                var checkOutEntry = group.FirstOrDefault(p => p.TypePunch == TypePunch.CHECKOUT);

                double hoursWorked = 0;

                if (checkInEntry != null && checkOutEntry != null)
                {
                    TimeSpan difference = checkOutEntry.TimeStamp - checkInEntry.TimeStamp;
                    hoursWorked = Math.Round(difference.TotalHours, 2);
                }

                result.Add(new AdminTimeEntryResponseDto
                {
                    Employee = group.Key.EmployeeName,
                    Date = group.Key.Date,
                    CheckIn = checkInEntry != null ? TimeOnly.FromDateTime(checkInEntry.TimeStamp) : null,
                    CheckOut = checkOutEntry != null ? TimeOnly.FromDateTime(checkOutEntry.TimeStamp) : null,
                    HoursWorked = hoursWorked
                });
            }
            return result;
        }
    }
}
