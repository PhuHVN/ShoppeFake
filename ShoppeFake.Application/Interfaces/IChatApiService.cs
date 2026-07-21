using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiService
    {
        Task<Result<ChatApiResponse?>> SendAsync(string message, CancellationToken cancellationToken = default);
    }
}
