using SistemaPontos.Dto;
using SistemaPontos.models;

namespace SistemaPontos.Services.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResponseDto> SignUp(RegisterAdminDto dto);
        Task<LoginResponseDto> Login(LoginDto dto);
        Task<RegisterResponseDto> RegisterEmployee(RegisterEmployeeDto dto);
    }
}
