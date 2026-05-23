using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Services
{
    public class VariantService : IVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VariantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<VariantResponse>> CreateVariantAsync(IList<int> valueIds, VariantRequest request)
        {
            if (string.IsNullOrEmpty(request.VariantName) || request.Price <= 0 || request.StockQuantity < 0 || string.IsNullOrEmpty(request.Sku) || request.WeightGrams <= 0)
            {
                return Result<VariantResponse>.Fail("InvalidData", "All fields are required and must be valid.");
            }

            var existingVariant = await _unitOfWork.GetRepository<ProductVariant>().FindAsync(v => v.Sku == request.Sku);
            if (existingVariant != null)
            {
                return Result<VariantResponse>.Fail("DuplicateSku", "A variant with the same SKU already exists.");
            }

            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == request.ProductId);
            if (product == null)
            {
                return Result<VariantResponse>.Fail("ProductNotFound", "The specified product does not exist.");
            }
            // Create the new variant
            var newVariant = new ProductVariant
            {
                VariantName = request.VariantName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Sku = request.Sku,
                ProductId = request.ProductId,
                WeightGrams = request.WeightGrams
            };

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.GetRepository<ProductVariant>().AddAsync(newVariant);
                await _unitOfWork.SaveChangesAsync();

                // Validate and create VariantAttributeValue entries
                var variantAttributeValues = new List<VariantAttributeValue>();
                foreach (var valueId in valueIds)
                {
                    // Check if the attribute value exists
                    var value = await _unitOfWork.GetRepository<AttributeValue>().FindAsync(x => x.Id == valueId);
                    if (value == null)
                    {
                        await _unitOfWork.RollBackAsync();
                        return Result<VariantResponse>.Fail("AttributeValueNotFound", $"Attribute value with ID {valueId} not found.");
                    }
                    variantAttributeValues.Add(new VariantAttributeValue
                    {
                        //get attribute id from value
                        AttributeId = value.AttributeId,
                        ProductVariantId = newVariant.Id,
                        AttributeValueId = valueId
                    });

                }
                await _unitOfWork.GetRepository<VariantAttributeValue>().AddRangeAsync(variantAttributeValues);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
                var response = _mapper.Map<VariantResponse>(newVariant);
                return Result<VariantResponse>.Success(response);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                return Result<VariantResponse>.Fail("Error", $"An error occurred while creating the variant: {ex.Message}");

            }

        }

        public async Task<Result> DeleteVariantAsync(int id)
        {
            var variant = await _unitOfWork.GetRepository<ProductVariant>().FindAsync(x => x.Id == id);
            if (variant == null)
            {
                return Result.Fail(Error.NotFound);
            }
            variant.Status = Domain.Enums.StatusEnum.Inactive;
            await _unitOfWork.GetRepository<ProductVariant>().UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<BasePaginatedList<VariantResponse>>> GetAllVariantsAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<ProductVariant>().Entity
                .Where(x => x.Status == Domain.Enums.StatusEnum.Active)
                .Include(x => x.Product)
                .Include(v => v.VariantAttributeValues)
                .ThenInclude(x => x.Attribute)
                .Include(v => v.VariantAttributeValues)
                .ThenInclude(x => x.AttributeValue);
            var rs = await _unitOfWork.GetRepository<ProductVariant>().GetPagging(query, pageIndex, pageSize);
            return Result<BasePaginatedList<VariantResponse>>.Success(_mapper.Map<BasePaginatedList<VariantResponse>>(rs));
        }

        public async Task<Result<VariantResponse>> GetVariantByIdAsync(int id)
        {
            var variant = await _unitOfWork.GetRepository<ProductVariant>().FindAsync(x => x.Id == id && x.Status == Domain.Enums.StatusEnum.Active);
            if (variant == null)
            {
                return Result<VariantResponse>.Fail("NotFound", "Variant not found.");
            }

            // Eager load related data
            var query = _unitOfWork.GetRepository<ProductVariant>().Entity
                .Where(x => x.Id == id && x.Status == Domain.Enums.StatusEnum.Active)
                .Include(x => x.Product)
                .Include(v => v.VariantAttributeValues)
                .ThenInclude(x => x.Attribute)
                .Include(v => v.VariantAttributeValues)
                .ThenInclude(x => x.AttributeValue);

            var variantWithDetails = await query.FirstOrDefaultAsync();
            if (variantWithDetails == null)
            {
                return Result<VariantResponse>.Fail("NotFound", "Variant not found.");
            }

            var rs = _mapper.Map<VariantResponse>(variantWithDetails);
            return Result<VariantResponse>.Success(rs);
        }

        public Task<Result<VariantResponse>> UpdateVariantAsync(int id, VariantRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
