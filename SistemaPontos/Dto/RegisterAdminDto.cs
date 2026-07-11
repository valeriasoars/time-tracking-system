using SistemaPontos.enums;

namespace SistemaPontos.Dto
{
    public class RegisterAdminDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.ADMIN;
    }
}
