using Alkhaligya.BLL.Dtos.Contact;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Contact
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContactMessageService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ApiResponse<List<ReadContactMessageDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 8)
        {
            var query = _unitOfWork.ContactMessages.GetAll().OrderByDescending(m => m.SentAt);
            var totalCount = await Task.FromResult(query.Count());
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedMessages = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<List<ReadContactMessageDto>>(pagedMessages);
            var response = new ApiResponse<List<ReadContactMessageDto>>(dtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
            return response;
        }

        public async Task<ApiResponse<ReadContactMessageDto>> GetByIdAsync(int id)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
            if (message == null)
                return new ApiResponse<ReadContactMessageDto>("لم يتم العثور على الرسالة");

            var dto = _mapper.Map<ReadContactMessageDto>(message);
            return new ApiResponse<ReadContactMessageDto>(dto);
        }

        public async Task<ApiResponse<string>> CreateAsync(CreateContactMessageDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.UserId))
            {
                var user = await _userManager.FindByIdAsync(dto.UserId);
                if (user == null)
                    return new ApiResponse<string>("المستخدم غير موجود");
            }
            var message = _mapper.Map<ContactMessage>(dto);

            // Set SentAt to Egypt Standard Time
            message.SentAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));

            await _unitOfWork.ContactMessages.AddAsync(message);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم إرسال الرسالة بنجاح");
        }


        public async Task<ApiResponse<string>> UpdateMessageAsync(UpdateContactMessageDto dto)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(dto.Id);
            if (message == null)
                return new ApiResponse<string>("لم يتم العثور على الرسالة");

            message.Message = dto.Message;
            message.IsRead = dto.IsRead;

            await _unitOfWork.ContactMessages.UpdateAsync(message);
            await _unitOfWork.CommitChangesAsync();

            return new ApiResponse<string>(null, "تم تحديث الرسالة بنجاح");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
            if (message == null)
                return new ApiResponse<string>("لم يتم العثور على الرسالة");

            await _unitOfWork.ContactMessages.DeleteByIdAsync(id);
            await _unitOfWork.CommitChangesAsync();
            return new ApiResponse<string>(null, "تم حذف الرسالة بنجاح");
        }

        public async Task<ApiResponse<List<ReadContactMessageDto>>> GetMessagesByUserIdAsync(string userId)
        {
            var messages = _unitOfWork.ContactMessages.FindAll(m => m.UserId == userId);

            if (messages == null || !messages.Any())
                return new ApiResponse<List<ReadContactMessageDto>>("لم يتم العثور على رسائل لهذا المستخدم");

            var result = _mapper.Map<List<ReadContactMessageDto>>(messages);
            return new ApiResponse<List<ReadContactMessageDto>>(result);
        }

    }


}
