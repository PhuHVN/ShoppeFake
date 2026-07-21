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
        [SwaggerOperation(summary: "Public - Login with email and password", description: "Authenticates a user using email and password, then returns an access token when credentials are valid.")]
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
        [SwaggerOperation(summary: "Public - Register a new customer account", description: "Registers a new user account with the provided details and sends an email confirmation when successful.")]
        public async Task<IActionResult> Register([FromBody] AccountRequest request)
        {
            var result = await _authService.RegisterEmail(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(request.Email, "Registration successful please check email", "201"));
        }




    }
}
