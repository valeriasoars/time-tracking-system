using SistemaPontos.enums;

namespace SistemaPontos.models
{
    public class Users
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public Role Role { get; set; }

        public List<PunchClock> PunchClocks { get; set; } = new List<PunchClock>();
    }
}
