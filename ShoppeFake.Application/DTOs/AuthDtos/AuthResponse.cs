namespace ShoppeFake.Application.DTOs.AuthDtos
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

    }
}
