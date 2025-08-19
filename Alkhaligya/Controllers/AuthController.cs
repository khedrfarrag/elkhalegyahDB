using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.BLL.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Alkhaligya.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.successed)
                return BadRequest(result);

            return Ok(new
            {
                Message = "Register successful! Welcome In Alkhaligya",
                successed = result.successed
            });
        }

        [AllowAnonymous]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAdminAsync(registerDto);
            if (!result.successed)
                return BadRequest(result);

            return Ok(new
            {
                Message = " Admin Is Register successful! Welcome In Alkhaligya",
                successed = result.successed
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var result = await _authService.VerifyOtpAsync(verifyOtpDto);

            if (!result.successed)
                return BadRequest(result);

            return Ok(new
            {
                result.successed,
                Message = "OTP verification successful! Your account is now verified."
            });
        }



        // Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);

            if (!response.successed)
                return BadRequest(response);

            return Ok(new
            {
                response.successed,
                response.Token,
                Message = "Login successful! Welcome back to your account."
            });
        }



        // Reset Password
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result.successed)
                return BadRequest(result);

            return Ok(new { success = true, message = "OTP has been sent successfully to your email." });
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result.successed)
                return BadRequest(result);

            return Ok(new { successed = result.successed, message = "Your password has been reset successfully." });
        }

        // Resend OTP
        [AllowAnonymous]
        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto resendOtpDto)
        {

            var result = await _authService.ResendOtpAsync(resendOtpDto);

            if (!result.successed)
            {
                return BadRequest(result);
            }

            return Ok(new { message = "OTP has been resent successfully.", successed = result.successed });
        }





        // Number Of Users And Admin
        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _authService.GetAllUsersAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }


        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [HttpGet("all-admins")]
        public async Task<IActionResult> GetAllAdmins([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _authService.GetAllAdminsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [HttpGet("user-count")]
        public async Task<ActionResult<int>> GetNumberOfUsers()
        {
            var numberOfUsers = await _authService.GetNumberOfUsersAsync();
            return Ok(numberOfUsers);
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [HttpGet("admin-count")]
        public async Task<ActionResult<int>> GetNumberOfAdmins()
        {
            var numberOfAdmins = await _authService.GetNumberOfAdminsAsync();
            return Ok(numberOfAdmins);
        }



        [Authorize(Roles = Roles.User)]
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
           

            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized(new ApiResponse<string>("Invalid token"));

            var result = await _authService.GetUserByIdAsync(CurrentUserId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.SuperAdmin)]
        [HttpGet("admin-profile")]
        public async Task<IActionResult> GetAdminProfile()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return Unauthorized(new ApiResponse<string>("Invalid token"));

            var result = await _authService.GetAdminByIdAsync(CurrentUserId);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }




        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet("confirmed-admins")]
        public async Task<IActionResult> GetConfirmedAdmins([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _authService.GetConfirmedAdminsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }

        [Authorize(Roles =Roles.SuperAdmin )]
        [HttpGet("unconfirmed-admins")]
        public async Task<IActionResult> GetUnconfirmedAdmins([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _authService.GetUnconfirmedAdminsAsync(pageNumber, pageSize);
            return response.Succeeded
                ? Ok(new { data = response.Data, pagination = response.Pagination })
                : BadRequest(response.Errors);
        }


        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("confirm-admin/{adminId}")]
        public async Task<IActionResult> ConfirmAdminById(string adminId)
        {
            var response = await _authService.ConfirmAdminByIdAsync(adminId);
            return response.successed
                ? Ok(new { message = "Admin confirmed successfully" })
                : BadRequest(response.Errors);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost("unconfirm-admin/{adminId}")]
        public async Task<IActionResult> UnconfirmAdminById(string adminId)
        {
            var response = await _authService.UnconfirmAdminByIdAsync(adminId);
            return response.successed
                ? Ok(new { message = "Admin unconfirmed successfully" })
                : BadRequest(response.Errors);
        }



    }
}
