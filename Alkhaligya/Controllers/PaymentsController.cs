using Alkhaligya.BLL.Dtos.PayMob;
using Alkhaligya.BLL.Services.PayMob;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Alkhaligya.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentsController(IUnitOfWork unitOfWork, ILogger<PaymentsController> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }



        [HttpGet("response")]
        [AllowAnonymous]
        public IActionResult PaymentResponse(
         [FromQuery(Name = "success")] bool success,
         [FromQuery(Name = "order")] string order_id,
         [FromQuery(Name = "id")] string txn_id)
        {
          

            if (success)
            {
                // ✅ يعيد توجيه المستخدم لصفحة نجاح الدفع
                return Redirect("/payment-result/success.html");
            }
            else
            {
                // ❌ يعيد توجيه المستخدم لصفحة فشل الدفع
                return Redirect("/payment-result/fail.html");
            }
        }



        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
      

            if (Request.ContentLength == null || Request.ContentLength == 0)
                return BadRequest("Empty request body");

            Request.EnableBuffering();
            Request.Body.Position = 0;

            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(body))
                return BadRequest("Request body is empty");


            JObject json;
            JObject obj;

            try
            {
                json = JObject.Parse(body);
                obj = json["obj"] as JObject;
                if (obj == null) return BadRequest("Missing 'obj' section");
            }
            catch (Exception ex)
            {
           
                return BadRequest("Invalid JSON");
            }

            // ✅ HMAC: Body ثم Header fallback
            string receivedHmac = Request.Query["hmac"].ToString();

          

            // ✅ تحقق من HMAC إن وُجد
            if (!string.IsNullOrEmpty(receivedHmac))
            {
                string hmacSecret = _configuration["Paymob:HmacSecret"];

            List<string> values = new List<string>
            {
              JsonConvert.SerializeObject(obj["amount_cents"]).Trim('"') ,
              JsonConvert.SerializeObject(obj["created_at"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["currency"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["error_occured"]).Trim('"'),
                JsonConvert.SerializeObject(obj["has_parent_transaction"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["id"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["integration_id"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["is_3d_secure"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_auth"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_capture"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["is_refunded"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_standalone_payment"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["is_voided"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["order"]?["id"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["owner"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["pending"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["source_data"]?["pan"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["source_data"]?["sub_type"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["source_data"]?["type"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["success"]).Trim('"')
            };
                string concatenatedString = string.Join("", values);
                string computedHmac = ComputeHmac(hmacSecret, concatenatedString);
            
                if (computedHmac != receivedHmac)
                {
                  
                    return Unauthorized(new { c = concatenatedString, computedhmac = computedHmac, message = "Invalid HMAC" });
                }
               

            }
       

            string transactionId = obj["id"]?.ToString();
            string merchantOrderId = obj["order"]?["merchant_order_id"]?.ToString();
            bool success = obj["success"]?.ToObject<bool>() ?? false;

            if (string.IsNullOrEmpty(transactionId))
                return BadRequest("Missing transaction ID");

            var transaction = await _unitOfWork.PaymentTransactions.GetByPaymobTransactionIdAsync(transactionId);

            if (transaction == null && !string.IsNullOrEmpty(merchantOrderId))
            {
                transaction = await _unitOfWork.PaymentTransactions.GetByOrderIdAsync(merchantOrderId);
                if (transaction != null)
                {
                    transaction.PaymobTransactionId = transactionId;
                }
            }

            if (transaction == null)
                return NotFound("Transaction not found");

            transaction.Status = success ? "Success" : "Failed";
            await _unitOfWork.PaymentTransactions.UpdateAsync(transaction);

            var order = await _unitOfWork.Orders.GetByIdAsync(transaction.OrderId);
            if (order == null)
                return NotFound("Order not found");

            order.PaymentStatus = success ? PaymentStatus.Paid : PaymentStatus.Cancelled;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitChangesAsync();


            return Ok("Webhook processed");
        }


        private static string ComputeHmac(string secret, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }



       

    }

}