using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.AuthAPI.Data;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dto;
using Services.AuthAPI.Service.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtOptions _jwtOptions;
        private IMapper _mapper;

        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IMapper mapper, IOptions<JwtOptions> jwtOptions)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _jwtOptions = jwtOptions.Value;
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

            LoginResponseDto loginResponseDto = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = GenerateToken(user)
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

        public string GenerateToken(ApplicationUser applicationUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
