using SistemaPontos.enums;

namespace SistemaPontos.Dto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
