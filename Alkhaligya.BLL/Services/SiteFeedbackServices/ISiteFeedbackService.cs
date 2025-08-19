using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Dtos.SiteFeedbackDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.SiteFeedbackServices
{
    public interface ISiteFeedbackService
    {
        Task<ApiResponse<string>> AddSiteFeedbackAsync(SiteFeedbackAddDto dto, string userId);

        Task<ApiResponse<List<SiteFeedbackReadDto>>> GetAllSiteFeedbackAsync(int pageNumber = 1, int pageSize = 8);


        Task<ApiResponse<string>> DeleteSiteFeedbackAsync(int id); // The new method definition

    }
}
