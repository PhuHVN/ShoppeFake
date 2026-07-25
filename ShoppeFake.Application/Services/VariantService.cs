using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.ExcelDtos;
using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using ShoppeFake.Domain.Enums;

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
                .AsNoTracking()
                .Where(x => x.Status == Domain.Enums.StatusEnum.Active)
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.Product)
                .Include(x => x.ProductImages)
                .Include(x => x.VariantAttributeValues)
                    .ThenInclude(x => x.Attribute)
                .Include(x => x.VariantAttributeValues)
                    .ThenInclude(x => x.AttributeValue);

            var result = await _unitOfWork
                .GetRepository<ProductVariant>()
                .GetPagging(query, pageIndex, pageSize);

            var response = _mapper.Map<BasePaginatedList<VariantResponse>>(result);
            return Result<BasePaginatedList<VariantResponse>>.Success(response);
        }

        public async Task<IList<ProductVariantExportDto>> GetAllToExportAsync()
        {
            var variants = await _unitOfWork.GetRepository<ProductVariant>().Entity
                .AsNoTracking()
                .Where(x => x.Status == StatusEnum.Active)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new ProductVariantExportDto
                {
                    ProductId = x.ProductId,
                    VariantId = x.Id,
                    ProductName = x.Product.Name,
                    ProductDescription = x.Product.Description,
                    CategoryName = x.Product.Category.Name,
                    BrandName = x.Product.Brand,
                    VariantName = x.VariantName,
                    Sku = x.Sku,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity,
                    WeightGrams = x.WeightGrams,
                    Status = x.Status.ToString(),
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            var variantIds = variants.Select(x => x.VariantId).ToList();

            var images = await _unitOfWork.GetRepository<ProductImage>().Entity
                .AsNoTracking()
                .Where(x => variantIds.Contains(x.VariantId))
                .Select(x => new { x.VariantId, x.ImageUrl })
                .ToListAsync();

            var attributes = await _unitOfWork.GetRepository<VariantAttributeValue>().Entity
                .AsNoTracking()
                .Where(x => variantIds.Contains(x.ProductVariantId))
                .Select(x => new
                {
                    x.ProductVariantId,
                    AttributeCode = x.Attribute.Code,
                    ValueText = x.AttributeValue.ValueText
                })
                .ToListAsync();

            var imagesByVariant = images
                .GroupBy(x => x.VariantId)
                .ToDictionary(g => g.Key, g => string.Join("|", g.Select(x => x.ImageUrl)));

            var attrsByVariant = attributes
                .GroupBy(x => x.ProductVariantId)
                .ToDictionary(g => g.Key, g => string.Join("; ", g.Select(x => $"{x.AttributeCode}: {x.ValueText}")));

            foreach (var item in variants)
            {
                item.ImageUrls = imagesByVariant.GetValueOrDefault(item.VariantId) ?? "";
                item.Attributes = attrsByVariant.GetValueOrDefault(item.VariantId) ?? "";
            }

            return variants;
        }

        public async Task<int> CountActiveVariantsAsync()
        {
            return await _unitOfWork.GetRepository<ProductVariant>()
                .Entity
                .AsNoTracking()
                .CountAsync(x => x.Status == StatusEnum.Active);
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
                .AsNoTracking()
                .Where(x => x.Id == id && x.Status == Domain.Enums.StatusEnum.Active)
                .Include(x => x.Product)
                .Include(x => x.ProductImages)
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

        public async Task<Result<VariantResponse>> UpdateVariantAsync(int id, VariantUpdateRequest request)
        {
            var variant = await _unitOfWork.GetRepository<ProductVariant>().FindAsync(x => x.Id == id && x.Status == Domain.Enums.StatusEnum.Active);
            if (variant == null)
            {
                return Result<VariantResponse>.Fail("NotFound", "Variant not found.");
            }
            request.VariantName = string.IsNullOrEmpty(request.VariantName) ? variant.VariantName : request.VariantName;
            request.Price = request.Price <= 0 ? variant.Price : request.Price;
            request.StockQuantity = request.StockQuantity < 0 ? variant.StockQuantity : request.StockQuantity;
            request.Sku = string.IsNullOrEmpty(request.Sku) ? variant.Sku : request.Sku;
            request.WeightGrams = request.WeightGrams <= 0 ? variant.WeightGrams : request.WeightGrams;
            await _unitOfWork.GetRepository<ProductVariant>().UpdateAsync(variant);
            await _unitOfWork.SaveChangesAsync();
            return Result<VariantResponse>.Success(_mapper.Map<VariantResponse>(variant));
        }
    }
}
