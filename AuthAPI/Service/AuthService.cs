using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.AuthAPI.Data;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dto;
using Services.AuthAPI.Service.IService;

namespace Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;

        public AuthService(AppDbContext dbContext,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _dbContext.applicationUsers.FirstAsync(u => u.UserName == loginRequestDto.UserName);
            if (user is null)
            {
                return new LoginResponseDto();
            }
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isValid)
            {
                return new LoginResponseDto();
            }
            UserDto userDto = _mapper.Map<UserDto>(user);

            // generate token

            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = ""
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = _mapper.Map<ApplicationUser>(registrationRequestDto);
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (!result.Succeeded)
                {
                    return result.Errors.FirstOrDefault().Description;
                }
                else
                {
                    //var userToReturn = _dbContext.applicationUsers.First(u => u.UserName == registrationRequestDto.Email);
                    //UserDto userDto = _mapper.Map<UserDto>(userToReturn);
                    return "";
                }

            }
            catch (Exception) { }

            return "Error Encounter";
        }
    }
}
