using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.FeedbackDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<FeedbackResponse>> CreateFeedbackAsync(FeedbackRequest request)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == request.ProductId && x.Status == Domain.Enums.StatusEnum.Active);
            if (product == null)
            {
                return Result<FeedbackResponse>.Fail("ProductNotFound", "The specified product does not exist or is not active.");
            }
            var account = await _unitOfWork.GetRepository<Account>().FindAsync(x => x.Id == request.AccountId && x.Status == Domain.Enums.StatusEnum.Active);
            if (account == null)
            {
                return Result<FeedbackResponse>.Fail("AccountNotFound", "The specified account does not exist or is not active.");
            }
            if (request.Rating < 1 || request.Rating > 5)
            {
                return Result<FeedbackResponse>.Fail("InvalidRating", "Rating must be between 1 and 5.");
            }
            var feedback = new Feedback
            {
                ProductId = request.ProductId,
                AccountId = request.AccountId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Feedback>().AddAsync(feedback);
            await _unitOfWork.SaveChangesAsync();
            var feedbackResponse = _mapper.Map<FeedbackResponse>(feedback);
            return Result<FeedbackResponse>.Success(feedbackResponse);
        }

        public Task<Result> DeleteFeedbackAsync(int feedbackId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<BasePaginatedList<FeedbackResponse>>> GetFeedbacksByProductIdAsync(int productId, int pageIndex, int pageSize)
        {
            var feedbacks = _unitOfWork.GetRepository<Feedback>().Entity
                .Include(x => x.Product).Include(x => x.Account)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.CreatedAt);
            var results = await _unitOfWork.GetRepository<Feedback>().GetPagging(feedbacks, pageIndex, pageSize);
            var feedbackResponses = _mapper.Map<BasePaginatedList<FeedbackResponse>>(results);
            return Result<BasePaginatedList<FeedbackResponse>>.Success(feedbackResponses);
        }
    }
}
