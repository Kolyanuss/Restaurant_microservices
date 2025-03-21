﻿using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Dto;
using Services.AuthAPI.Service.IService;

namespace Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            LoginResponseDto loginResponseDto = await _authService.Login(loginRequestDto);
            if (loginResponseDto.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Username or password is incorect";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponseDto;
            return Ok(_responseDto);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignSuccessful = await _authService.AssignRole(model.Email, model.RoleName);
            if (!assignSuccessful)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error encountered while assigning role";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
    }
}
