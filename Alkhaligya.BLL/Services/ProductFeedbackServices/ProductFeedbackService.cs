using Alkhaligya.BLL.Dtos.ProductFeedbackDto;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.ProductFeedbackServices
{
    public class ProductFeedbackService : IProductFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductFeedbackService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<ProductFeedbackReadDto>>> GetProductFeedbacksAsync(int productId)
        {
            var feedbacks = await _unitOfWork.ProductFeedbacks.GetAll()
                .Where(f => f.ProductId == productId && !f.IsDeleted)
                .ProjectTo<ProductFeedbackReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new ApiResponse<List<ProductFeedbackReadDto>>(feedbacks);
        }

        public async Task<ApiResponse<string>> AddProductFeedbackAsync(ProductFeedbackAddDto dto)
        {
            if (dto.UserId == null)
            {
                return new ApiResponse<string>("لم يتم العثور على المستخدم", "فشل في إضافة التقييم");
            }
            var feedback = _mapper.Map<ProductFeedback>(dto);

            // Set CreatedAt to Egypt Standard Time
            feedback.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));

            await _unitOfWork.ProductFeedbacks.AddAsync(feedback);
            await _unitOfWork.CommitChangesAsync();

            // Recalculate and update product's Rate
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product != null)
            {
                var feedbacks = await _unitOfWork.ProductFeedbacks.GetAll()
                    .Where(f => f.ProductId == dto.ProductId && !f.IsDeleted)
                    .ToListAsync();
                if (feedbacks.Any())
                {
                    product.Rate = (int)Math.Round(feedbacks.Average(f => f.Rate));
                }
                else
                {
                    product.Rate = 0;
                }
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.CommitChangesAsync();
            }

            return new ApiResponse<string>(null, "تم إضافة التقييم بنجاح");
        }

        public async Task<ApiResponse<string>> DeleteProductFeedbackAsync(int id)
        {
            var feedback = await _unitOfWork.ProductFeedbacks.GetByIdAsync(id);
            if (feedback == null)
                return new ApiResponse<string>("لم يتم العثور على التقييم");

            await _unitOfWork.ProductFeedbacks.DeleteByIdAsync(id);
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم حذف التقييم بنجاح");
        }
    }
}
