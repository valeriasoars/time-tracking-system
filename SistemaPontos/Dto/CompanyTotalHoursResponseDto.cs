namespace SistemaPontos.Dto
{
    public class CompanyTotalHoursResponseDto
    {
        public double TotalHours { get; set; }
        public List<EmployeeHoursDto> Employees { get; set; } = new List<EmployeeHoursDto>();
    }

    public class EmployeeHoursDto
    {
        public string Name { get; set; } = string.Empty;
        public double HoursWorked { get; set; }
    }
}
