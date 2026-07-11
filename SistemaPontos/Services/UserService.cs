using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaPontos.data;
using SistemaPontos.Dto;
using SistemaPontos.enums;
using SistemaPontos.models;
using SistemaPontos.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace SistemaPontos.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BC.Verify(dto.Password, user.Password))
            {
                throw new Exception("Usuário ou senha inválidos.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var generetedToken = tokenHandler.WriteToken(token);

            return new LoginResponseDto
            {
                Token = generetedToken,
                Role = user.Role
            };
        }

        public async Task<RegisterResponseDto> RegisterEmployee(RegisterEmployeeDto dto)
        {
            bool emailExiste = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExiste)
            {
                throw new Exception("Este e-mail já está cadastrado no sistema.");
            }

            var newEmployee = new Users
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
            };

            _context.Users.Add(newEmployee);
            await _context.SaveChangesAsync();
            return new RegisterResponseDto
            {
                Id = newEmployee.Id,
                Message = "Funcionário cadastrado com sucesso!"
            };

        }

        public async Task<RegisterResponseDto> SignUp(RegisterAdminDto dto)
        {
            bool emailExiste = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExiste)
            {
                throw new Exception("Este e-mail já está cadastrado no sistema.");
            }

            var newUser = new Users 
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Password = BC.HashPassword(dto.Password),
                Role = Role.ADMIN
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return new RegisterResponseDto
            {
                Id = newUser.Id,
                Message = "Conta criada com sucesso!"
            };
        }
        
    }
}
