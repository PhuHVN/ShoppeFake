using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Interfaces
{
    public interface IJwtProvider
    {
        Task<string> GenerateTokenAsync(Account acc);
        string RefreshToken();
    }
}
