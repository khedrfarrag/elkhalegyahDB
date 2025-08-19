using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Dtos.PayMob
{


    //---------------
    public class CreatePaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
    }

    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
    }


    public class PaymobWebhookDto
    {
        [JsonProperty("hmac")]
        public string Hmac { get; set; }

        [JsonProperty("obj")]
        public PaymobWebhookObj Obj { get; set; }
    }

    public class PaymobWebhookObj
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("amount_cents")]
        public decimal AmountCents { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("error_occured")]
        public bool ErrorOccured { get; set; }

        [JsonProperty("has_parent_transaction")]
        public bool HasParentTransaction { get; set; }

        [JsonProperty("integration_id")]
        public int IntegrationId { get; set; }

        [JsonProperty("is_3d_secure")]
        public bool Is3DS { get; set; }

        [JsonProperty("is_auth")]
        public bool IsAuth { get; set; }

        [JsonProperty("is_capture")]
        public bool IsCapture { get; set; }

        [JsonProperty("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonProperty("is_standalone_payment")]
        public bool IsStandalonePayment { get; set; }

        [JsonProperty("is_voided")]
        public bool IsVoided { get; set; }

        [JsonProperty("merchant_order_id")]
        public string MerchantOrderId { get; set; }

        [JsonProperty("order")]
        public PaymobOrder Order { get; set; }

        [JsonProperty("owner")]
        public int Owner { get; set; }

        [JsonProperty("pending")]
        public bool Pending { get; set; }

        [JsonProperty("source_data")]
        public PaymobSourceData SourceData { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class PaymobOrder
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class PaymobSourceData
    {
        [JsonProperty("pan")]
        public string Pan { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("sub_type")]
        public string SubType { get; set; }
    }


}
