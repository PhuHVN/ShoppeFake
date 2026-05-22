using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Application.DTOs.AuthDtos;
using ShoppeFake.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [SwaggerOperation(summary: "Login with email and password")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var token = await _authService.LoginEmail(request);
            if (token.IsFailure)
            {
                return BadRequest(ApiResponse<AuthResponse>.BadRequestResponse(token.Error.Message));
            }
            return Ok(ApiResponse<AuthResponse>.OkResponse(token.Value, "Login successful", "201"));
        }

        [HttpPost("register")]
        [SwaggerOperation(summary: "Register a new account")]
        public async Task<IActionResult> Register([FromBody] AccountRequest request)
        {
            var result = await _authService.Register(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(request.UsernameOrEmail, "Registration successful please check email", "201"));
        }

        

        [HttpPatch("verifyOtp")]
        [SwaggerOperation(summary: "Verify the user's email using OTP")]
        public async Task<IActionResult> VerifyEmail(VerifyOtpDtos verifyOtp)
        {
            await _authService.VerifyEmail(verifyOtp.Email, verifyOtp.Otp);
            if (verifyOtp == null)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid OTP or email"));
            }
            return Ok(ApiResponse<string>.OkResponse(null, "Email verified successfully", "200"));
        }

        [HttpPost("resendOtp/{email}")]
        [SwaggerOperation(summary: "Resend OTP to the user's email")]
        public async Task<IActionResult> ResendOtp([FromRoute] string email)
        {
            await _authService.ResendOtpAsync(email);
            if (email == null)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse("Invalid email"));
            }
            return Ok(ApiResponse<string>.OkResponse(null, "OTP resent successfully", "200"));
        }
     
    }
}