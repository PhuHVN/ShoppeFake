using ShoppeFake.Application.DTOs.FeedbackDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<Result<BasePaginatedList<FeedbackResponse>>> GetFeedbacksByProductIdAsync(int productId, int pageIndex, int pageSize);
        Task<Result<FeedbackResponse>> CreateFeedbackAsync(FeedbackRequest request);
        Task<Result> DeleteFeedbackAsync(int feedbackId);
    }
}
