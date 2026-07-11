namespace SistemaPontos.Dto
{
    public class AdminTimeEntryResponseDto
    {
        public string Employee {  get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly? CheckIn { get; set; }
        public TimeOnly? CheckOut { get; set; }
        public double HoursWorked { get; set; }
    }
}
