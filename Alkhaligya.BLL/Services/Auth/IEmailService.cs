using Alkhaligya.BLL.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Auth
{
    public interface IEmailService
    {
        Task<GeneralRespnose> SendEmailAsync(string email, string subject, string message);
    }
}
