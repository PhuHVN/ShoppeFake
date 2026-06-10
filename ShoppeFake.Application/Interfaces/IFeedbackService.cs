using ShoppeFake.Application.DTOs.FeedbackDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<Result<BasePaginatedList<FeedbackResponse>>> GetFeedbacksByProductIdAsync(int productId, int pageIndex, int pageSize);
        Task<Result<FeedbackResponse>> CreateFeedbackAsync(FeedbackRequest request);       
        Task<Result> DeleteFeedbackAsync(int feedbackId);
    }
}
