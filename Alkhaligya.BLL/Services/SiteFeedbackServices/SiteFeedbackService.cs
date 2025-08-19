using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Dtos.SiteFeedbackDtos;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using Alkhaligya.DAL.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.SiteFeedbackServices
{
    public class SiteFeedbackService : ISiteFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SiteFeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public async Task<ApiResponse<string>> AddSiteFeedbackAsync(SiteFeedbackAddDto dto, string userId)
        //{
        //    if (userId == null)
        //    {
        //        return new ApiResponse<string>("UserId not found", "Failed to add feedback");
        //    }

        //    var feedback = _mapper.Map<SiteFeedback>(dto);
        //    feedback.UserId = userId;

        //    await _unitOfWork.SiteFeedbacks.AddAsync(feedback);
        //    await _unitOfWork.CommitChangesAsync();

        //    return new ApiResponse<string>(null, "Feedback added successfully");
        //}


        public async Task<ApiResponse<string>> AddSiteFeedbackAsync(SiteFeedbackAddDto dto, string userId)
        {
            if (userId == null)
            {
                return new ApiResponse<string>("لم يتم العثور على  المستخدم", "فشل في إضافة التقييم");
            }

            var feedback = _mapper.Map<SiteFeedback>(dto);

            feedback.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));
            feedback.UserId = userId;

            await _unitOfWork.SiteFeedbacks.AddAsync(feedback);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم إضافة التقييم بنجاح");
        }

        public async Task<ApiResponse<List<SiteFeedbackReadDto>>> GetAllSiteFeedbackAsync(int pageNumber = 1, int pageSize = 8)
        {
            // Ensure we use the correct GetAll() override
            var repo = (SiteFeedbackRepository)_unitOfWork.SiteFeedbacks;
            var query = repo.GetAll().Where(f => !f.IsDeleted);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var feedbacks = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var feedbackDtos = _mapper.Map<List<SiteFeedbackReadDto>>(feedbacks);
            var response = new ApiResponse<List<SiteFeedbackReadDto>>(feedbackDtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<string>> DeleteSiteFeedbackAsync(int id)
        {
            var feedbackToDelete = await _unitOfWork.SiteFeedbacks.GetByIdAsync(id);

            if (feedbackToDelete == null)
            {
                return new ApiResponse<string>("لم يتم العثور على التقييم", "فشل في حذف التقييم");
            }

            await _unitOfWork.SiteFeedbacks.DeleteByIdAsync(id); // Use the DeleteByIdAsync from the repository
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم حذف التقييم بنجاح");
        }
    }
}
