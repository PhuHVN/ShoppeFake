using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<Account>> GetUserIdLoginsAsync();
    }
}
