using SistemaPontos.Dto;
using SistemaPontos.enums;
using SistemaPontos.models;

namespace SistemaPontos.Helpers
{
    public static class TimeCalculationHelper
    {
        public static IEnumerable<AdminTimeEntryResponseDto> CalculateDailyHours(IEnumerable<PunchClock> entries)
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
                var orderedPunches = group.OrderBy(p => p.TimeStamp).ToList();

                double totalHoursWorked = 0;
                TimeOnly? firstCheckIn = null;
                TimeOnly? lastCheckOut = null;

                int i = 0;
                while (i < orderedPunches.Count - 1)
                {
                    var current = orderedPunches[i];
                    var next = orderedPunches[i + 1];

                    if (current.TypePunch == TypePunch.CHECKIN && next.TypePunch == TypePunch.CHECKOUT)
                    {
                        totalHoursWorked += (next.TimeStamp - current.TimeStamp).TotalHours;

                        firstCheckIn ??= TimeOnly.FromDateTime(current.TimeStamp);
                        lastCheckOut = TimeOnly.FromDateTime(next.TimeStamp);

                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                }
                result.Add(new AdminTimeEntryResponseDto
                {
                    Employee = group.Key.EmployeeName,
                    Date = group.Key.Date,
                    CheckIn = firstCheckIn,
                    CheckOut = lastCheckOut,
                    HoursWorked = Math.Round(totalHoursWorked, 2)
                });

            }
            return result;
        }
    }
}
