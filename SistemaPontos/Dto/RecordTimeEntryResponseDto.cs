namespace SistemaPontos.Dto
{
    public class RecordTimeEntryResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow; 

    }
}
