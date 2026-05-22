namespace ShoppeFake.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string email, string otp);
        Task SendEmailAsync(string to, string subject, string body);
    }
}
