using Alkhaligya.BLL.Dtos.PayMob;
using Alkhaligya.BLL.Dtos.Responce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.PayMob
{
    
    public interface IPaymentService
    {
        //Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(CreatePaymentRequestDto request);
        Task<ApiResponse<string>> ProcessWebhookAsync(PaymobWebhookDto webhook);
        Task<ApiResponse<PaymentResponseDto>> InitiateUnifiedCheckoutAsync(CreatePaymentRequestDto request);

    }
}
