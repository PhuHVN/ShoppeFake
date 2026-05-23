using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Application.DTOs.AuthDtos;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginEmail(AuthRequest request);
        Task<Result<string>> RegisterEmail(AccountRequest request);
        Task<Result> VerifyEmail(string email, string otp);
        Task<Result> ResendOtpAsync(string email);
    }
}
