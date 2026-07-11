namespace SistemaPontos.Dto
{
    public class TimeEntryHistoryResponseDto
    {
        public DateOnly Date {  get; set; }
        public TimeOnly CheckIn { get; set; }
        public TimeOnly CheckOut { get; set; }
        public double HoursWorked { get; set; }
    }
}
