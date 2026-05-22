namespace ShoppeFake.Application.DTOs.AuthDtos
{
    public class AuthRequest
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
