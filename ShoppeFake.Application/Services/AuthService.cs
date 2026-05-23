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

        private readonly IEmailService _emailService;
        private readonly IRedisService _redisService;
        private readonly IMapper _mapper;
        public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider, IEmailService emailService, IRedisService redisService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;

            _emailService = emailService;
            _redisService = redisService;
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
            if (existingUser != null && existingUser.Status == StatusEnum.Pending)
            {
                //remove old otp and resend new otp to email
                await _redisService.RemoveOtpAsync(requestEmail);
                var newOtp = _redisService.GenerateOTP();
                await _redisService.StoreOtpAsync(requestEmail, newOtp, TimeSpan.FromMinutes(5));
                await _emailService.SendOtpAsync(requestEmail, newOtp);
                return Result<string>.Success(requestEmail);
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
                Status = Domain.Enums.StatusEnum.Pending,
                Role = Domain.Enums.RoleEnum.Customer,
                CreatedAt = DateTime.UtcNow
            };

            var otp = _redisService.GenerateOTP();
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                await _unitOfWork.GetRepository<Account>().AddAsync(newUser);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                await _redisService.StoreOtpAsync(requestEmail, otp, TimeSpan.FromMinutes(5));
                await _emailService.SendOtpAsync(requestEmail, otp);
                return Result<string>.Success(requestEmail);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                return Result<string>.Fail("RegistrationError", "An error occurred while registering the account.");
            }

        }
        public async Task<Result> ResendOtpAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result.Fail(Error.Invalid);
            }
            var user = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == email && x.Status == Domain.Enums.StatusEnum.Pending);
            if (user == null)
            {
                return Result.Fail(Error.NotFound);
            }
            await _redisService.RemoveOtpAsync(email);
            var otp = _redisService.GenerateOTP();
            await _emailService.SendOtpAsync(email, otp);
            await _redisService.StoreOtpAsync(email, otp, TimeSpan.FromMinutes(5));
            return Result.Success();
        }
        public async Task<Result> VerifyEmail(string email, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
            {
                return Result.Fail(Error.Invalid);
            }
            var storedOtp = await _redisService.RetrieveOtpAsync(email);
            if (storedOtp == null || storedOtp != otp)
            {
                return Result.Fail(Error.Unauthorized);
            }
            var user = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Email == email && x.Status == Domain.Enums.StatusEnum.Pending);
            if (user == null)
            {
                return Result.Fail(Error.NotFound);
            }
            user.Status = Domain.Enums.StatusEnum.Active;

            await _unitOfWork.GetRepository<Account>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _redisService.RemoveOtpAsync(email);
            return Result.Success();
        }


    }
}
