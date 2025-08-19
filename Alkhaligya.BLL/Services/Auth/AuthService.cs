using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<CustomRole> _roleManager;


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, IEmailService emailService, IConfiguration configuration
            , RoleManager<CustomRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _roleManager = roleManager;

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<UserReadDto>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 8)
        {
            var UserRole = await _roleManager.FindByNameAsync(Roles.User);
            if (UserRole == null)
                return new ApiResponse<List<UserReadDto>>("User role not found");

            var Users = await _userManager.GetUsersInRoleAsync(UserRole.Name);

            var filteredUserss = Users
                .Where(u => u.IsBanned == false )
                .ToList();

            var totalCount = filteredUserss.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedUsers = filteredUserss
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var UserDtos = _mapper.Map<List<UserReadDto>>(pagedUsers);

            var response = new ApiResponse<List<UserReadDto>>(UserDtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return response;
        }

        public async Task<ApiResponse<List<UserReadDto>>> GetAllAdminsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var adminRole = await _roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
                return new ApiResponse<List<UserReadDto>>("Admin role not found");

            var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);

            var filteredAdmins = admins
                .Where(u => u.IsBanned == false )
                .ToList();

            var totalCount = filteredAdmins.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedAdmins = filteredAdmins
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var adminDtos = _mapper.Map<List<UserReadDto>>(pagedAdmins);

            var response = new ApiResponse<List<UserReadDto>>(adminDtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return response;
        }

        public async Task<ApiResponse<UserReadDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ApiResponse<UserReadDto>("User not found");

            var isInRole = await _userManager.IsInRoleAsync(user, Roles.User);
            if (!isInRole || user.IsBanned || !user.EmailConfirmed)
                return new ApiResponse<UserReadDto>("User is not valid");

            var userDto = _mapper.Map<UserReadDto>(user);
            return new ApiResponse<UserReadDto>(userDto);
        }

        public async Task<ApiResponse<UserReadDto>> GetAdminByIdAsync(string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
                return new ApiResponse<UserReadDto>("Admin not found");

            var isInRole = await _userManager.IsInRoleAsync(admin, Roles.Admin);
            if (!isInRole || admin.IsBanned || !admin.EmailConfirmed)
                return new ApiResponse<UserReadDto>("Admin is not valid");

            var adminDto = _mapper.Map<UserReadDto>(admin);
            return new ApiResponse<UserReadDto>(adminDto);
        }


        public async Task<ApiResponse<List<UserReadDto>>> GetConfirmedAdminsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var adminRole = await _roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
                return new ApiResponse<List<UserReadDto>>("Admin role not found");

            var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);

            var confirmedAdmins = admins
                .Where(a => a.IsConfirmed && !a.IsBanned)
                .ToList();

            var totalCount = confirmedAdmins.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedAdmins = confirmedAdmins
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var adminDtos = _mapper.Map<List<UserReadDto>>(pagedAdmins);

            var response = new ApiResponse<List<UserReadDto>>(adminDtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return response;
        }
        public async Task<ApiResponse<List<UserReadDto>>> GetUnconfirmedAdminsAsync(int pageNumber = 1, int pageSize = 8)
        {
            var adminRole = await _roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
                return new ApiResponse<List<UserReadDto>>("Admin role not found");

            var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);

            var unconfirmedAdmins = admins
                .Where(a => !a.IsConfirmed && !a.IsBanned)
                .ToList();

            var totalCount = unconfirmedAdmins.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedAdmins = unconfirmedAdmins
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var adminDtos = _mapper.Map<List<UserReadDto>>(pagedAdmins);

            var response = new ApiResponse<List<UserReadDto>>(adminDtos);
            response.Pagination = new PaginationDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return response;
        }

        public async Task<GeneralRespnose> ConfirmAdminByIdAsync(string adminId)
        {
            var response = new GeneralRespnose();

            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
            {
                response.Errors.Add("Admin not found");
                return response;
            }

            var isAdmin = await _userManager.IsInRoleAsync(admin, Roles.Admin);
            if (!isAdmin)
            {
                response.Errors.Add("User is not an admin");
                return response;
            }

            if (admin.IsConfirmed)
            {
                response.Errors.Add("Admin is already confirmed");
                return response;
            }

            admin.IsConfirmed = true;
            var result = await _userManager.UpdateAsync(admin);

            if (!result.Succeeded)
            {
                response.Errors = result.Errors.Select(e => e.Description).ToList();
                return response;
            }

            response.successed = true;
            return response;
        }
        public async Task<GeneralRespnose> UnconfirmAdminByIdAsync(string adminId)
        {
            var response = new GeneralRespnose();

            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin == null)
            {
                response.Errors.Add("Admin not found");
                return response;
            }

            var isAdmin = await _userManager.IsInRoleAsync(admin, Roles.Admin);
            if (!isAdmin)
            {
                response.Errors.Add("User is not an admin");
                return response;
            }

            if (!admin.IsConfirmed)
            {
                response.Errors.Add("Admin is already unconfirmed");
                return response;
            }

            admin.IsConfirmed = false;
            var result = await _userManager.UpdateAsync(admin);

            if (!result.Succeeded)
            {
                response.Errors = result.Errors.Select(e => e.Description).ToList();
                return response;
            }

            response.successed = true;
            return response;
        }



        // Register
        public async Task<GeneralRespnose> RegisterAsync(RegisterDto registerDto)
        {
            var response = new GeneralRespnose();
            if (_userManager.Users.Any(s => s.FirstName == registerDto.FirstName && s.LastName == registerDto.LastName ))
            {
                response.Errors.Add("هذا الاسم مستخدم بالفعل. يرجى اختيار اسم آخر.");
                return response;
            }

            if (_userManager.Users.Any(s => s.Email == registerDto.Email))
            {
                response.Errors.Add("البريد الإلكتروني موجود بالفعل");
                return response;

            }

            if (registerDto.Password != registerDto.ConfirmedPassword)
            {
                response.Errors.Add("كلمة المرور وتأكيد كلمة المرور غير متطابقين.");
                return response;
            }

            //  رفع الصورة
            string imageUrl = null;
            if (registerDto.Image != null && registerDto.Image.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDto.Image.FileName);
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerDto.Image.CopyToAsync(stream);
                }

                imageUrl = $"/images/users/{fileName}";
            }

            // Generate New UserName
            string userName = GenerateUserName();

            // إنشاء المستخدم
            User user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = userName,
                City = registerDto.City,
                OTP = GenerateOTP(),
                OtpExpiryTime = DateTime.UtcNow.AddMinutes(10),
                ImageUrl = imageUrl
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                // OTP
                string emailBody = $"Dear {user.FirstName} {user.LastName},\r\n\r\n" +
                          $"Your one-time password (OTP) for verification is:\r\n\r\n" +
                          $"🔹 {user.OTP}\r\n\r\n" +
                          $"This code is valid for 10 minutes. Please do not share this code with anyone for security reasons.\r\n\r\n" +
                          $"If you didn't request this code, please ignore this email.\r\n\r\n" +
                          $"Best regards,\r\n" +
                          $"The Alkhaligya Team";
                await _emailService.SendEmailAsync(user.Email, "Verify Your Email - Alkhaligya Team", emailBody);

                // Assign the role 
                await _userManager.AddToRoleAsync(user, Roles.User);

                response.successed = true;
                return response;

            }

            response.Errors = result.Errors.Select(d => d.Description).ToList();
            return response;

        }

        public async Task<GeneralRespnose> RegisterAdminAsync(RegisterDto registerDto)
        {
            var response = new GeneralRespnose();

            if (_userManager.Users.Any(s => s.FirstName == registerDto.FirstName && s.LastName == registerDto.LastName))
            {
                response.Errors.Add("هذا الاسم مستخدم بالفعل. يرجى اختيار اسم آخر.");
                return response;
            }

            if (_userManager.Users.Any(s => s.Email == registerDto.Email))
            {
                response.Errors.Add("البريد الإلكتروني موجود بالفعل");
                return response;
            }

            if (registerDto.Password != registerDto.ConfirmedPassword)
            {
                response.Errors.Add("كلمة المرور وتأكيد كلمة المرور غير متطابقين.");
                return response;
            }

            //  رفع الصورة
            string imageUrl = null;
            if (registerDto.Image != null && registerDto.Image.Length > 0)
            {
                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDto.Image.FileName);
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerDto.Image.CopyToAsync(stream);
                }

                imageUrl = $"/images/users/{fileName}";
            }

            // Generate New UserName
            string userName = GenerateUserName();

            // إنشاء المستخدم
            Admin admin = new Admin
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = userName,
                IsConfirmed = false,
                City = registerDto.City,
                OTP = GenerateOTP(),
                OtpExpiryTime = DateTime.UtcNow.AddMinutes(10),
                ImageUrl = imageUrl
            };

            var result = await _userManager.CreateAsync(admin, registerDto.Password);
            if (result.Succeeded)
            {
                // إرسال OTP
                string emailBody = $"Dear {admin.FirstName} {admin.LastName},\r\n\r\n" +
                                   $"Your one-time password (OTP) for verification is:\r\n\r\n" +
                                   $"🔹 {admin.OTP}\r\n\r\n" +
                                   $"This code is valid for 10 minutes. Please do not share this code with anyone for security reasons.\r\n\r\n" +
                                   $"If you didn't request this code, please ignore this email.\r\n\r\n" +
                                   $"Best regards,\r\n" +
                                   $"The Alkhaligya Team";

                await _emailService.SendEmailAsync(admin.Email, "Verify Your Email - Alkhaligya Team", emailBody);

                // Assign the role "Admin"
                await _userManager.AddToRoleAsync(admin, Roles.Admin);

                response.successed = true;
                return response;
            }

            response.Errors = result.Errors.Select(d => d.Description).ToList();
            return response;
        }


        public async Task<GeneralRespnose> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var response = new GeneralRespnose();

            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
            {
                response.Errors.Add("البريد الإلكتروني غير موجود. يرجى التأكد من صحة البريد الإلكتروني.");
                return response;
            }

            if (string.IsNullOrEmpty(user.OTP) || user.OTP != verifyOtpDto.OTP)
            {
                response.Errors.Add("رمز التحقق غير صحيح.");
                return response;
            }

            if (!user.OtpExpiryTime.HasValue || DateTime.UtcNow > user.OtpExpiryTime.Value)
            {
                response.Errors.Add("انتهت صلاحية رمز التحقق. يرجى طلب رمز جديد.");
                return response;
            }

            user.EmailConfirmed = true;
            user.OTP = null;
            user.OtpExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            response.successed = true;
            return response;
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }


        // Login
        public async Task<LoginResponce> LoginAsync(LoginDto loginDto)
        {
            var response = new LoginResponce();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                response.Errors.Add(user == null ? "البريد الإلكتروني غير موجود. يرجى التأكد من صحة البريد الإلكتروني." :
                    "البريد الإلكتروني غير مؤكد. يرجى التحقق من صندوق الوارد.");
                return response;
            }

            if (await _userManager.IsInRoleAsync(user, Roles.Admin) && !user.IsConfirmed)
            {
                response.Errors.Add(" SuperAdmin لم تتم الموافقة على حساب المشرف بعد. يرجى انتظار موافقة الـ");
                return response;
            }

            if (user.IsBanned)
            {
                response.Errors.Add("تم حظر حسابك.");
                return response;
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (result)
            {
                #region Claims
                List<Claim> claims = new List<Claim>()
{
                   new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // User ID as Subject
                   new Claim(JwtRegisteredClaimNames.Email, user.Email), // User Email
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token Identifier
                   new Claim("name", $"{user.FirstName} {user.LastName}"),
                   new Claim("image", user.ImageUrl ?? string.Empty)
};

                // Add user roles
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                #endregion

                await _userManager.AddClaimsAsync(user, claims);
                response.Token = GenerateToken(claims, loginDto.RememberMe);

                response.successed = true;
                return response;
            }

            response.Errors.Add("كلمة المرور أو البريد الإلكتروني غير صحيح.");
            return response;
        }
        private string GenerateToken(IList<Claim> claims, bool RememberMe)
        {

            #region Token
            #region SecurityKey
            var SecretKeyString = _configuration.GetSection("SecretKey").Value;
            var SecretKeyByte = Encoding.ASCII.GetBytes(SecretKeyString);
            SecurityKey securityKey = new SymmetricSecurityKey(SecretKeyByte);
            #endregion

            //Combind SecretKey , HasingAlgorithm (SigningCredentials)
            SigningCredentials signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Determine Expiration
            DateTime tokenExpiration = RememberMe ? DateTime.UtcNow.AddHours(24) : DateTime.UtcNow.AddHours(1);

            //Token
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
            (
                 issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                signingCredentials: signingCredential,
                expires: tokenExpiration
            );
            //To convert Token To String
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string token = handler.WriteToken(jwtSecurityToken);
            #endregion
            return token;
        }



        // Reset Password
        public async Task<GeneralRespnose> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new GeneralRespnose();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                response.Errors.Add("البريد الإلكتروني غير موجود. يرجى التأكد من صحة البريد الإلكتروني.");
                return response;
            }

            user.OTP = GenerateOTP();
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(user);

            // Send OTP to user
            string emailBody = $"Dear {user.FirstName} {user.LastName},\r\n\r\n" +
                               $"Your password reset OTP is:\r\n\r\n" +
                               $"🔹 {user.OTP}\r\n\r\n" +
                               $"This code is valid for 5 minutes.\r\n\r\n" +
                               $"If you didn't request this, please ignore this email.\r\n\r\n" +
                               $"Best regards,\r\n" +
                               $"The Alkhaligya Team";

            var result = await _emailService.SendEmailAsync(user.Email, "Password Reset OTP - Alkhaligya Team", emailBody);

            if (result.successed)
            {
                response.successed = result.successed;
                return response;
            }

            response.Errors.AddRange(result.Errors);
            return response;
        }

        public async Task<GeneralRespnose> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = new GeneralRespnose();
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null || user.OTP != resetPasswordDto.OTP)
            {
                response.Errors.Add("رمز التحقق أو البريد الإلكتروني غير صحيح.");
                return response;
            }

            if (user.OtpExpiryTime == null || DateTime.UtcNow > user.OtpExpiryTime)
            {
                response.Errors.Add("انتهت صلاحية رمز التحقق. يرجى طلب رمز جديد.");
                return response;
            }

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmedNewPassword)
            {
                response.Errors.Add("كلمة المرور الجديدة وتأكيد كلمة المرور غير متطابقين.");
                return response;
            }

            // Reset Passowrd
            var ResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, ResetToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                response.Errors.AddRange(result.Errors.Select(e => e.Description));
                return response;
            }

            user.OTP = null;
            user.OtpExpiryTime = null;
            await _userManager.UpdateAsync(user);

            response.successed = true;
            return response;
        }


        // Resend Otp
        public async Task<GeneralRespnose> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            var response = new GeneralRespnose();

            var user = await _userManager.FindByEmailAsync(resendOtpDto.Email);
            if (user == null)
            {
                response.Errors.Add("البريد الإلكتروني غير موجود. يرجى التأكد من صحة البريد الإلكتروني.");
                return response;
            }

            if (!resendOtpDto.isForResetPassword && user.EmailConfirmed)
            {
                response.Errors.Add("تم التحقق من البريد الإلكتروني بالفعل.");
                return response;
            }

            // New OTP
            user.OTP = GenerateOTP();
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                response.Errors.AddRange(updateResult.Errors.Select(e => e.Description));
                return response;
            }

            // Register Or Reset Password
            string subject, emailBody;

            if (resendOtpDto.isForResetPassword)
            {
                subject = "Password Reset OTP - Alkhaligya Team";
                emailBody = $"Dear {user.FirstName} {user.LastName},\r\n\r\n" +
                            $"Your password reset OTP is:\r\n\r\n" +
                            $"🔹 {user.OTP}\r\n\r\n" +
                            $"This code is valid for 10 minutes.\r\n\r\n" +
                            $"If you didn't request this, please ignore this email.\r\n\r\n" +
                            $"Best regards,\r\n" +
                            $"The Alkhaligya Team";
            }
            else
            {
                subject = "Verify Your Email - Alkhaligya Team";
                emailBody = $"Dear {user.FirstName} {user.LastName},\r\n\r\n" +
                            $"Your new OTP for email verification is:\r\n\r\n" +
                            $"🔹 {user.OTP}\r\n\r\n" +
                            $"This code is valid for 10 minutes. Please do not share this code with anyone for security reasons.\r\n\r\n" +
                            $"If you didn't request this, please ignore this email.\r\n\r\n" +
                            $"Best regards,\r\n" +
                            $"The Alkhaligya Team";
            }

            var emailResult = await _emailService.SendEmailAsync(user.Email, subject, emailBody);

            if (emailResult.successed)
            {
                response.successed = true;
                return response;
            }

            response.Errors.AddRange(emailResult.Errors);
            return response;
        }



        //Number Of Users And Admin
        public async Task<int> GetNumberOfUsersAsync()
        {
            var usersRole = await _roleManager.FindByNameAsync(Roles.User); // تحديد دور المسؤول
            if (usersRole == null) return 0;

            var users = await _userManager.GetUsersInRoleAsync(usersRole.Name); // الحصول على المستخدمين الذين لديهم هذا الدور
            return users.Count(s=>s.IsBanned==false );
        }

        public async Task<int> GetNumberOfAdminsAsync()
        {
            var adminsRole = await _roleManager.FindByNameAsync(Roles.Admin); // تحديد دور المسؤول
            if (adminsRole == null) return 0;

            var admins = await _userManager.GetUsersInRoleAsync(adminsRole.Name); // الحصول على المستخدمين الذين لديهم هذا الدور
            return admins.Count(s=>s.IsBanned == false );
        }

        // Generate UserName
        private string GenerateUserName()
        {
            string name = "rased-user-";
            return $"{name}{new Random().Next(1000, 9999)}";
        }


    }
}
