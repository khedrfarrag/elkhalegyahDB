
using Alkhaligya.BLL.Dtos.PayMob;
using Alkhaligya.BLL.Dtos.Responce;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Models.PayMob;
using Alkhaligya.DAL.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.PayMob
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentService> _logger;

        private readonly string _secretKey;
        private readonly string _publicKey;
        private readonly int _cardIntegrationId;
        private readonly int _walletIntegrationId;
        private readonly string _hmacSecret;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _httpClient = httpClient;
            _logger = logger;

            _secretKey = configuration["Paymob:SecretKey"];
            _publicKey = configuration["Paymob:PublicKey"];
            _cardIntegrationId = int.Parse(configuration["Paymob:CardIntegrationId"]);
            _walletIntegrationId = int.Parse(configuration["Paymob:WalletIntegrationId"]);
            _hmacSecret = configuration["Paymob:HmacSecret"];
        }

        //public async Task<ApiResponse<PaymentResponseDto>> InitiateUnifiedCheckoutAsync(CreatePaymentRequestDto request)
        //{
        //    try
        //    {
        //        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

        //        if (order == null || order.PaymentStatus != PaymentStatus.Pending)
        //            return new ApiResponse<PaymentResponseDto>("Invalid order");



        //        //NEW: التحقق من وجود معاملات معلقة فعالة للطلب
        //       var pendingTransactions = await _unitOfWork.PaymentTransactions.GetPendingTransactionsByOrderIdAsync(request.OrderId);
        //        if (pendingTransactions.Any())
        //        {
        //            // التحقق من عدم انتهاء مهلة المعاملة
        //            var mostRecentTransaction = pendingTransactions.OrderByDescending(t => t.CreatedAt).First();
        //            var paymentTimeout = TimeSpan.FromMinutes(30); // تعيين مهلة 30 دقيقة للدفع

        //            if (DateTime.UtcNow.Subtract(mostRecentTransaction.CreatedAt) < paymentTimeout)
        //            {
        //                _logger.LogWarning("Active pending payment transaction exists for OrderId: {OrderId}", request.OrderId);
        //                return new ApiResponse<PaymentResponseDto>(null, "A payment is already in progress for this order");
        //            }
        //            else
        //            {
        //                // تحديث المعاملات القديمة كمنتهية المهلة
        //                foreach (var trans in pendingTransactions)
        //                {
        //                    trans.Status = "Timeout";
        //                    await _unitOfWork.PaymentTransactions.UpdateAsync(trans);
        //                }
        //                await _unitOfWork.CommitChangesAsync();
        //                _logger.LogInformation("Timed out {Count} stale payment transactions for OrderId: {OrderId}",
        //                    pendingTransactions.Count(), request.OrderId);
        //            }
        //        }

        //        // 3. التحقق من المبلغ
        //        if (request.Amount != order.TotalPrice)
        //        {
        //            _logger.LogWarning("Payment amount does not match order total: {OrderId}, Expected: {Expected}, Provided: {Provided}",
        //                request.OrderId, order.TotalPrice, request.Amount);
        //            return new ApiResponse<PaymentResponseDto>("Payment amount does not match order total");
        //        }

        //        // NEW: التحقق من الحد الأدنى والأقصى للمبلغ
        //        const decimal MinimumPaymentAmount = 1.0m; // الحد الأدنى للدفع بالجنيه المصري
        //        const decimal MaximumPaymentAmount = 100000.0m; // الحد الأقصى للدفع بالجنيه المصري

        //        if (request.Amount < MinimumPaymentAmount)
        //        {
        //            _logger.LogWarning("Payment amount is below minimum threshold: {OrderId}, Amount: {Amount}",
        //                request.OrderId, request.Amount);
        //            return new ApiResponse<PaymentResponseDto>($"Payment amount must be at least {MinimumPaymentAmount} {request.Currency}");
        //        }

        //        if (request.Amount > MaximumPaymentAmount)
        //        {
        //            _logger.LogWarning("Payment amount exceeds maximum threshold: {OrderId}, Amount: {Amount}",
        //                request.OrderId, request.Amount);
        //            return new ApiResponse<PaymentResponseDto>($"Payment amount cannot exceed {MaximumPaymentAmount} {request.Currency}");
        //        }

        //        // NEW: التحقق من العملة المدعومة
        //        var supportedCurrencies = new[] { "EGP" }; // يمكن توسيعها حسب احتياجك
        //        if (!supportedCurrencies.Contains(request.Currency))
        //        {
        //            _logger.LogWarning("Unsupported currency: {OrderId}, Currency: {Currency}",
        //                request.OrderId, request.Currency);
        //            return new ApiResponse<PaymentResponseDto>($"Currency {request.Currency} is not supported");
        //        }

        //        var user = await _userManager.FindByIdAsync(order.UserId);
        //        if (user == null)
        //            return new ApiResponse<PaymentResponseDto>("User not found");

        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

        //        var payload = new
        //        {
        //            amount = (int)(request.Amount * 100),
        //            currency = request.Currency,
        //            payment_methods = new[] { _cardIntegrationId, _walletIntegrationId },
        //            items = new[]
        //            {
        //                new {
        //                    name = "Order Payment",
        //                    amount = (int)(request.Amount * 100),
        //                    description = "Payment for order",
        //                    quantity = 1
        //                }
        //            },
        //            billing_data = new
        //            {
        //                apartment = "NA",
        //                first_name = user.FirstName ?? "Guest",
        //                last_name = user.LastName ?? "User",
        //                street = "NA",
        //                building = "NA",
        //                phone_number = user.PhoneNumber ?? "0000000000",
        //                country = "EG",
        //                email = user.Email ?? "no@email.com",
        //                floor = "NA",
        //                state = "NA"
        //            },
        //            customer = new
        //            {
        //                first_name = user.FirstName ?? "Guest",
        //                last_name = user.LastName ?? "User",
        //                email = user.Email ?? "no@email.com",
        //                extras = new { }
        //            },
        //            extras = new { }
        //        };

        //        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PostAsync("https://accept.paymob.com/v1/intention/", content);
        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            _logger.LogError("Failed to create intention: {Response}", responseContent);
        //            return new ApiResponse<PaymentResponseDto>("Failed to create intention");
        //        }

        //        var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
        //        var clientSecret = json.GetProperty("client_secret").GetString();
        //        var intentionId = json.GetProperty("id").GetString(); // ← بدون تحويل

        //        var transaction = new PaymentTransaction
        //        {
        //            TransactionId = Guid.NewGuid().ToString(),
        //            OrderId = order.Id,
        //            Amount = request.Amount,
        //            Currency = request.Currency,
        //            PaymentMethod = "UnifiedCheckout",
        //            Status = "Pending",
        //            CreatedAt = DateTime.UtcNow,
        //            PaymobOrderId = intentionId, // ← خزن الـ id كما هو
        //            IsDeleted = false
        //        };

        //        await _unitOfWork.PaymentTransactions.AddAsync(transaction);
        //        order.PaymentTransactionId = transaction.Id;
        //        await _unitOfWork.Orders.UpdateAsync(order);
        //        await _unitOfWork.CommitChangesAsync();

        //        var paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_publicKey}&clientSecret={clientSecret}";

        //        return new ApiResponse<PaymentResponseDto>(new PaymentResponseDto
        //        {
        //            PaymentUrl = paymentUrl,
        //            TransactionId = transaction.TransactionId,
        //            Status = "Pending"
        //        }, "Unified checkout initiated successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in unified checkout");
        //        return new ApiResponse<PaymentResponseDto>("Error initiating unified checkout");
        //    }
        //}

        public async Task<ApiResponse<PaymentResponseDto>> InitiateUnifiedCheckoutAsync(CreatePaymentRequestDto request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

                if (order == null || order.PaymentStatus != PaymentStatus.Pending)
                    return new ApiResponse<PaymentResponseDto>("Invalid order");

                // التحقق من وجود معاملات معلقة
                var pendingTransactions = await _unitOfWork.PaymentTransactions.GetPendingTransactionsByOrderIdAsync(request.OrderId);
                if (pendingTransactions.Any())
                {
                    var mostRecentTransaction = pendingTransactions.OrderByDescending(t => t.CreatedAt).First();
                    var paymentTimeout = TimeSpan.FromMinutes(30);

                    if (DateTime.UtcNow.Subtract(mostRecentTransaction.CreatedAt) < paymentTimeout)
                    {
                        _logger.LogWarning("Active pending payment transaction exists for OrderId: {OrderId}", request.OrderId);
                        return new ApiResponse<PaymentResponseDto>(null, "A payment is already in progress for this order");
                    }

                    foreach (var trans in pendingTransactions)
                    {
                        trans.Status = "Timeout";
                        await _unitOfWork.PaymentTransactions.UpdateAsync(trans);
                    }
                    await _unitOfWork.CommitChangesAsync();
                }

                if (request.Amount != order.TotalPrice)
                    return new ApiResponse<PaymentResponseDto>("Payment amount does not match order total");

                const decimal MinimumPaymentAmount = 1.0m;
                const decimal MaximumPaymentAmount = 100000.0m;

                if (request.Amount < MinimumPaymentAmount)
                    return new ApiResponse<PaymentResponseDto>($"Payment amount must be at least {MinimumPaymentAmount} {request.Currency}");

                if (request.Amount > MaximumPaymentAmount)
                    return new ApiResponse<PaymentResponseDto>($"Payment amount cannot exceed {MaximumPaymentAmount} {request.Currency}");

                var supportedCurrencies = new[] { "EGP" };
                if (!supportedCurrencies.Contains(request.Currency))
                    return new ApiResponse<PaymentResponseDto>($"Currency {request.Currency} is not supported");

                ApplicationUser user = null;
                if (!string.IsNullOrEmpty(order.UserId))
                {
                    user = await _userManager.FindByIdAsync(order.UserId);
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

                string firstName = order.IsGuestOrder ? order.FirstName : (user?.FirstName ?? "Guest");
                string lastName = order.IsGuestOrder ? order.LastName : (user?.LastName ?? "User");
                string phone = order.IsGuestOrder ? order.MobileNumber : (user?.PhoneNumber ?? "0000000000");
                string email = order.IsGuestOrder ? "guest@example.com" : (user?.Email ?? "no@email.com");

                var payload = new
                {
                    amount = (int)(request.Amount * 100),
                    currency = request.Currency,
                    payment_methods = new[] { _cardIntegrationId, _walletIntegrationId },
                    items = new[]
                    {
                new {
                    name = "Order Payment",
                    amount = (int)(request.Amount * 100),
                    description = "Payment for order",
                    quantity = 1
                }
            },
                    billing_data = new
                    {
                        apartment = "NA",
                        first_name = firstName,
                        last_name = lastName,
                        street = "NA",
                        building = "NA",
                        phone_number = phone,
                        country = "EG",
                        email = email,
                        floor = "NA",
                        state = order.Governorate.ToString()
                    },
                    customer = new
                    {
                        first_name = firstName,
                        last_name = lastName,
                        email = email,
                        extras = new { }
                    },
                    extras = new { }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://accept.paymob.com/v1/intention/", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create intention: {Response}", responseContent);
                    return new ApiResponse<PaymentResponseDto>("Failed to create intention");
                }

                var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var clientSecret = json.GetProperty("client_secret").GetString();
                var intentionId = json.GetProperty("id").GetString();

                var transaction = new PaymentTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    OrderId = order.Id,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    PaymentMethod = "UnifiedCheckout",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    PaymobOrderId = intentionId,
                    IsDeleted = false
                };

                await _unitOfWork.PaymentTransactions.AddAsync(transaction);
                order.PaymentTransactionId = transaction.Id;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CommitChangesAsync();

                var paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_publicKey}&clientSecret={clientSecret}";

                return new ApiResponse<PaymentResponseDto>(new PaymentResponseDto
                {
                    PaymentUrl = paymentUrl,
                    TransactionId = transaction.TransactionId,
                    Status = "Pending"
                }, "Unified checkout initiated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in unified checkout");
                return new ApiResponse<PaymentResponseDto>("Error initiating unified checkout");
            }
        }


        public async Task<ApiResponse<string>> ProcessWebhookAsync(PaymobWebhookDto webhook)
        {
            const int MaxRetries = 3;
            int currentRetry = 0;

            while (true)
            {
                try
                {
                    if (!VerifyHmac(webhook))
                    {
                        _logger.LogWarning("Invalid HMAC signature for TransactionId: {TransactionId}", webhook.Obj.Id);
                        return new ApiResponse<string>("Invalid HMAC signature");
                    }

                    var transaction = await _unitOfWork.PaymentTransactions.GetByPaymobTransactionIdAsync(webhook.Obj.Id.ToString());

                    if (transaction == null)
                    {
                        transaction = await _unitOfWork.PaymentTransactions.GetByOrderIdAsync(webhook.Obj.MerchantOrderId);

                        if (transaction != null)
                        {
                            transaction.PaymobTransactionId = webhook.Obj.Id.ToString();
                            transaction.Status = webhook.Obj.Success ? "Success" : "Failed";
                            await _unitOfWork.PaymentTransactions.UpdateAsync(transaction);
                        }
                        else
                        {
                            _logger.LogError("No matching transaction found for webhook: PaymobTransactionId={TransactionId}, PaymobOrderId={OrderId}", webhook.Obj.Id, webhook.Obj.Order.Id);
                            return new ApiResponse<string>("No matching transaction found for this webhook");
                        }
                    }
                    else
                    {
                        transaction.Status = webhook.Obj.Success ? "Success" : "Failed";
                        await _unitOfWork.PaymentTransactions.UpdateAsync(transaction);
                    }

                    var order = await _unitOfWork.Orders.GetByIdAsync(transaction.OrderId);
                    if (order == null)
                    {
                        _logger.LogWarning("Order not found for TransactionId: {TransactionId}", webhook.Obj.Id);
                        return new ApiResponse<string>("Order not found");
                    }

                    order.PaymentStatus = webhook.Obj.Success ? PaymentStatus.Paid : PaymentStatus.Cancelled;
                    await _unitOfWork.Orders.UpdateAsync(order);
                    await _unitOfWork.CommitChangesAsync();

                    _logger.LogInformation("Webhook processed successfully for TransactionId: {TransactionId}", webhook.Obj.Id);
                    return new ApiResponse<string>(null, "Webhook processed successfully");
                }
                catch (Exception ex)
                {
                    currentRetry++;
                    _logger.LogError(ex, "Error processing webhook attempt {Retry} for TransactionId: {TransactionId}", currentRetry, webhook.Obj.Id);

                    if (currentRetry >= MaxRetries)
                    {
                        return new ApiResponse<string>("An error occurred while processing webhook after multiple attempts");
                    }

                    await Task.Delay(500);
                }
            }
        }

        private bool VerifyHmac(PaymobWebhookDto webhook)
        {
            try
            {
                var fields = new Dictionary<string, string>
                {
                    { "amount_cents", webhook.Obj.AmountCents.ToString("F0") },
                    { "created_at", webhook.Obj.CreatedAt.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFF") },
                    { "currency", webhook.Obj.Currency },
                    { "error_occured", webhook.Obj.ErrorOccured.ToString().ToLower() },
                    { "has_parent_transaction", webhook.Obj.HasParentTransaction.ToString().ToLower() },
                    { "id", webhook.Obj.Id.ToString() },
                    { "integration_id", webhook.Obj.IntegrationId.ToString() },
                    { "is_3d_secure", webhook.Obj.Is3DS.ToString().ToLower() },
                    { "is_auth", webhook.Obj.IsAuth.ToString().ToLower() },
                    { "is_capture", webhook.Obj.IsCapture.ToString().ToLower() },
                    { "is_refunded", webhook.Obj.IsRefunded.ToString().ToLower() },
                    { "is_standalone_payment", webhook.Obj.IsStandalonePayment.ToString().ToLower() },
                    { "is_voided", webhook.Obj.IsVoided.ToString().ToLower() },
                    { "order.id", webhook.Obj.Order.Id.ToString() },
                    { "owner", webhook.Obj.Owner.ToString() },
                    { "pending", webhook.Obj.Pending.ToString().ToLower() },
                    { "source_data.pan", webhook.Obj.SourceData.Pan ?? string.Empty },
                    { "source_data.sub_type", webhook.Obj.SourceData.SubType ?? string.Empty },
                    { "source_data.type", webhook.Obj.SourceData.Type ?? string.Empty },
                    { "success", webhook.Obj.Success.ToString().ToLower() }
                };

                var concatenated = string.Join("", fields.Values);
                using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
                var calculatedHmac = BitConverter.ToString(hash).Replace("-", "").ToLower();

                return calculatedHmac == webhook.Hmac.ToLower();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying HMAC for TransactionId: {TransactionId}", webhook.Obj.Id);
                return false;
            }
        }
    }
}










