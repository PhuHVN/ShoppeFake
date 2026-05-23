using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.AttributeDtos;
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
    public class AttributeService : IAttributeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AttributeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<AttributeResponse>> CreateAttributeAsync(AttributeRequest request)
        {
            if(request.Name == null || request.Code == null)
            {
                return Result<AttributeResponse>.Fail("InvalidData", "Name and Code are required.");
            }
            var exitsAttribute = await _unitOfWork.GetRepository<Domain.Entities.Attribute>().FindAsync(a => a.Code == request.Code);
            if (exitsAttribute != null)
            {
                return Result<AttributeResponse>.Fail("DuplicateCode", "An attribute with the same code already exists.");
            }
            var attribute = new Domain.Entities.Attribute
            {
                Name = request.Name,
                Code = request.Code,
                IsFilterable = true
            };
            await _unitOfWork.GetRepository<Domain.Entities.Attribute>().AddAsync(attribute);
            await _unitOfWork.SaveChangesAsync();
            return Result<AttributeResponse>.Success(_mapper.Map<AttributeResponse>(attribute));
        }

        public async Task DeleteAttributeAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<BasePaginatedList<AttributeResponse>>> GetAllAttributesAsync(int pageIndex, int pageSize)
        {
            var attributes = _unitOfWork.GetRepository<Domain.Entities.Attribute>().Entity.Include(x => x.AttributeValues) ;
            var rs =await _unitOfWork.GetRepository<Domain.Entities.Attribute>().GetPagging(attributes, pageIndex, pageSize);
            return Result<BasePaginatedList<AttributeResponse>>.Success(_mapper.Map<BasePaginatedList<AttributeResponse>>(rs));
        }

        public async Task<Result<AttributeResponse>> GetAttributeByIdAsync(int id)
        {
            var attribute = await _unitOfWork.GetRepository<Domain.Entities.Attribute>().FindAsync(a => a.Id == id);
            if (attribute == null)
            {
                return Result<AttributeResponse>.Fail("NotFound", "Attribute not found.");
            }
            return Result<AttributeResponse>.Success(_mapper.Map<AttributeResponse>(attribute));
        }

        public Task<Result<AttributeResponse>> UpdateAttributeAsync(int id, AttributeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
