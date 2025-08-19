using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.Responce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Auth
{
    public interface IAuthService
    {

        Task<LoginResponce> LoginAsync(LoginDto loginDto);


        Task<GeneralRespnose> RegisterAsync(RegisterDto registerDto);
        Task<GeneralRespnose> RegisterAdminAsync(RegisterDto registerDto);
        Task<GeneralRespnose> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);


        Task<GeneralRespnose> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<GeneralRespnose> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        Task<GeneralRespnose> ResendOtpAsync(ResendOtpDto resendOtpDto);

        Task<int> GetNumberOfUsersAsync();
        Task<int> GetNumberOfAdminsAsync();

        Task<ApiResponse<List<UserReadDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<List<UserReadDto>>> GetAllAdminsAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<UserReadDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<UserReadDto>> GetAdminByIdAsync(string adminId);

        Task<ApiResponse<List<UserReadDto>>> GetConfirmedAdminsAsync(int pageNumber = 1, int pageSize = 8);
        Task<ApiResponse<List<UserReadDto>>> GetUnconfirmedAdminsAsync(int pageNumber = 1, int pageSize = 8);
        Task<GeneralRespnose> ConfirmAdminByIdAsync(string adminId);
        Task<GeneralRespnose> UnconfirmAdminByIdAsync(string adminId);



    }
}

