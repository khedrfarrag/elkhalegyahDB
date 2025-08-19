using Alkhaligya.BLL.Dtos.Contact;
using Alkhaligya.BLL.Dtos.Responce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Contact
{
    public interface IContactMessageService
    {
        Task<ApiResponse<List<ReadContactMessageDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<ReadContactMessageDto>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(CreateContactMessageDto dto);
        Task<ApiResponse<string>> UpdateMessageAsync(UpdateContactMessageDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<List<ReadContactMessageDto>>> GetMessagesByUserIdAsync(string userId);

    }

}
