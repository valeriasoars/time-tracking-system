using SistemaPontos.enums;

namespace SistemaPontos.models
{
    public class PunchClock
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; } = null!;
        public TypePunch TypePunch { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        
    }
}
