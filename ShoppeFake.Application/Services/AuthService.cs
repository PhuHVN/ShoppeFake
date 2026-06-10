using AutoMapper;
using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Application.DTOs.AuthDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using ShoppeFake.Domain.Enums;
using System.Text.RegularExpressions;

namespace ShoppeFake.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;


        private readonly IMapper _mapper;
        public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;

            _mapper = mapper;
        }
        public async Task<Result<AuthResponse>> LoginEmail(AuthRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return Result<AuthResponse>.Fail("InvalidInput", "Email and password must be provided.");
            }
            var requestEmail = request.Email.Trim();
            var user = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == requestEmail && x.Status == Domain.Enums.StatusEnum.Active);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Result<AuthResponse>.Fail("Unauthorized", "Invalid email or password.");
            }
            // Generate JWT token
            var token = await _jwtProvider.GenerateTokenAsync(user);

            await _unitOfWork.SaveChangesAsync();
            var rs = new AuthResponse
            {
                Token = token,

            };
            return Result<AuthResponse>.Success(rs);
        }
        public async Task<Result<string>> RegisterEmail(AccountRequest request)
        {
            if (request.Email == null || request.Password == null || request.FullName == null)
            {
                return Result<string>.Fail("InvalidInput", "Email, password and full name must be provided.");
            }
            var requestEmail = request.Email.Trim();
            var existingUser = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == requestEmail);
            if (existingUser != null && existingUser.Status == StatusEnum.Active)
            {
                return Result<string>.Fail("EmailAlreadyInUse", "An account with this email already exists.");
            }
            if (existingUser != null && existingUser.Status == StatusEnum.Inactive)
            {
                return Result<string>.Fail("InactiveAccount", "An account with this email is inactive. Please contact support for assistance.");
            }

            //if email not exist, create new account with pending status and send otp to email
            if (!Regex.IsMatch(requestEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return Result<string>.Fail("InvalidEmail", "The email format is invalid.");
            }
            if (request.Password.Length < 6)
            {
                return Result<string>.Fail("WeakPassword", "Password must be at least 6 characters long.");
            }
            var newUser = new Account
            {
                Email = requestEmail,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Status = Domain.Enums.StatusEnum.Active,
                Role = Domain.Enums.RoleEnum.Customer,
                CreatedAt = DateTime.UtcNow
            };

            try
            {

                await _unitOfWork.GetRepository<Account>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();
                return Result<string>.Success("Registration successful.");
            }
            catch (Exception ex)
            {

                return Result<string>.Fail("RegistrationError", "An error occurred while registering the account.");
            }

        }
        
    }
}
