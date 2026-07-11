using SistemaPontos.Dto;
using SistemaPontos.models;
using System.Timers;

namespace SistemaPontos.Services.Interfaces
{
    public interface IPunchClockService
    {
        Task<IEnumerable<AdminTimeEntryResponseDto>> GetAllTimeEntries(Guid? employeeId, DateOnly? startDate, DateOnly? endDate);
        Task<IEnumerable<TimeEntryHistoryResponseDto>> GetTimeEntryHistory(Guid userId);
        Task<RecordTimeEntryResponseDto> RecordTimeEntry(Guid userId, RecordTimeEntryDto dto);
    }
}
