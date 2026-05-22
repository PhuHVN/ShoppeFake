namespace ShoppeFake.Application.Interfaces
{
    public interface IRedisService
    {
        string GenerateOTP();
        Task RemoveOtpAsync(string key);
        Task<string?> RetrieveOtpAsync(string key);
        Task StoreOtpAsync(string key, string otp, TimeSpan expiration);
    }
}
