using ModelLibrary.Dto;
using Services.AuthAPI.Models;

namespace Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
        Task<bool> AssignRole(string email, string roleName);
    }
}
