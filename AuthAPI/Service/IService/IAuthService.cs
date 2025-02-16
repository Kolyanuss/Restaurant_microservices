using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dto;

namespace Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        string GenerateToken(ApplicationUser applicationUser);
    }
}
