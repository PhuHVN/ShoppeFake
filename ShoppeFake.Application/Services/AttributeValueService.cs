using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.ValueDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Services
{
    
    public class AttributeValueService : IAttributeValueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AttributeValueService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ValueResponse>> CreateValueAsync(ValueRequest request)
        {
            if(string.IsNullOrEmpty(request.ValueText) || string.IsNullOrEmpty(request.Slug))
            {
                return Result<ValueResponse>.Fail("InvalidData", "ValueText and Slug are required.");
            }
            var attribute = await _unitOfWork.GetRepository<Domain.Entities.Attribute>().FindAsync(x => x.Id == request.AttributeId);
            if(attribute == null)
            {
                return Result<ValueResponse>.Fail("NotFound", "Attribute not found.");
            }
            var existingValue = await _unitOfWork.GetRepository<Domain.Entities.AttributeValue>().FindAsync(x => x.Slug == request.Slug || x.ValueText == request.ValueText);
            if (existingValue != null) {
                return Result<ValueResponse>.Fail("Conflict", "A value with the same slug or value text already exists.");
            }
            var newValue = new Domain.Entities.AttributeValue
            {
                AttributeId = attribute.Id,
                ValueText = request.ValueText,
                Slug = request.Slug
            };
            await _unitOfWork.GetRepository<Domain.Entities.AttributeValue>().AddAsync(newValue);
            await _unitOfWork.SaveChangesAsync();
            return Result<ValueResponse>.Success(_mapper.Map<ValueResponse>(newValue));
        }

        public Task<Result<bool>> DeleteValueAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<BasePaginatedList<ValueResponse>>> GetAllValuesAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Domain.Entities.AttributeValue>().Entity;
            var rs = await _unitOfWork.GetRepository<Domain.Entities.AttributeValue>().GetPagging(query, pageIndex, pageSize);
            var mappedItems = _mapper.Map<BasePaginatedList<ValueResponse>>(rs);
            return Result<BasePaginatedList<ValueResponse>>.Success(mappedItems);
        }

        public async Task<Result<ValueResponse>> GetValueByIdAsync(int id)
        {
            var value = await _unitOfWork.GetRepository<Domain.Entities.AttributeValue>().FindAsync(x => x.Id == id);
            if (value == null)
            {
                return Result<ValueResponse>.Fail("NotFound", "Value not found.");
            }
            return Result<ValueResponse>.Success(_mapper.Map<ValueResponse>(value));
        }

        public Task<Result<ValueResponse>> UpdateValueAsync(int id, ValueRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
