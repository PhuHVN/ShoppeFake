namespace ShoppeFake.Application.DTOs.AccountDtos
{
    public class AccountRequest
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
