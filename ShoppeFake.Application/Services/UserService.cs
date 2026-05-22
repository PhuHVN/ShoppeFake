using Microsoft.AspNetCore.Http;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using System.Security.Claims;

namespace ShoppeFake.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _http;

        public UserService(IUnitOfWork unitOfWork, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _http = http;
        }

        public async Task<Result<Account>> GetUserIdLoginsAsync()
        {
            var context = _http.HttpContext;
            var userId = context?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Result<Account>.Fail(Error.NotFound);
            }
            var user = await _unitOfWork.GetRepository<Domain.Entities.Account>().FindAsync(a => a.Id == userId.Value);
            if (user == null)
            {
                return Result<Account>.Fail(Error.NotFound);
            }
            return Result<Account>.Success(user);
        }
    }
}
