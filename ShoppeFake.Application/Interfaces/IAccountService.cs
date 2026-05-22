using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> CreateAccount(AccountRequest request);
        Task<Result<AccountResponse>> UpdateAccount(AccountRequest request);
        Task<Result<AccountResponse>> GetAccountById(string id);
        Task<Result<BasePaginatedList<AccountResponse>>> GetAllAccounts(int pageIndex, int pageSize);
        Task<Result> DeleteAccount(string id);
    }
}
