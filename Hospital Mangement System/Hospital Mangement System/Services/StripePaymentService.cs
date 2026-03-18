using Stripe;
using Stripe.Checkout;
using Hospital_Management_System.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hospital_Management_System.Services
{
    public class StripePaymentService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly IBillService _billService;
        private readonly ILogger<StripePaymentService> _logger;

        public StripePaymentService(
            IConfiguration configuration,
            IBillService billService,
            ILogger<StripePaymentService> logger)
        {
            _configuration = configuration;
            _billService = billService;
            _logger = logger;

            // Initialize Stripe API key
            var secretKey = _configuration["PaymentGateways:Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(secretKey) && secretKey != "sk_test_your_stripe_secret_key")
            {
                StripeConfiguration.ApiKey = secretKey;
            }
            else
            {
                _logger.LogWarning("Stripe secret key not configured. Using test mode.");
                StripeConfiguration.ApiKey = "sk_test_placeholder"; // Will fail gracefully if not set
            }
        }

        public async Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(billId);
                if (bill == null)
                    throw new InvalidOperationException("Bill not found");

                if (amount <= 0 || amount > bill.RemainingAmount)
                    throw new InvalidOperationException("Invalid payment amount");

                var frontendUrl = _configuration["FrontendBaseUrl"] ?? "http://localhost:3000";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(amount * 100), // Convert to cents
                                Currency = currency.ToLower(),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Bill #{bill.BillNumber}",
                                    Description = $"Payment for hospital bill - Patient: {bill.PatientName ?? "N/A"}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{frontendUrl}/bills/{billId}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{frontendUrl}/bills/{billId}/payment/cancel",
                    Metadata = new Dictionary<string, string>
                    {
                        { "billId", billId.ToString() },
                        { "billNumber", bill.BillNumber },
                        { "patientId", bill.PatientId.ToString() }
                    },
                    CustomerEmail = !string.IsNullOrEmpty(bill.PatientEmail) ? bill.PatientEmail : null,
                    ExpiresAt = DateTime.UtcNow.AddHours(24) // Session expires in 24 hours
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                _logger.LogInformation("Stripe session created successfully. SessionId: {SessionId}, BillId: {BillId}", 
                    session.Id, billId);

                return new StripeSessionResponse
                {
                    SessionId = session.Id,
                    CheckoutUrl = session.Url,
                    Amount = amount,
                    Currency = currency
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe API error while creating session for bill {BillId}", billId);
                throw new Exception($"Stripe error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe session for bill {BillId}", billId);
                throw;
            }
        }

        public async Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId)
        {
            if (gateway.ToLower() != "stripe")
                return false;

            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(orderId);

                return session.PaymentStatus == "paid" && session.Status == "complete";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Stripe payment. OrderId: {OrderId}", orderId);
                return false;
            }
        }

        // PayPal and Paymob implementations - delegated to other services
        public Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Use PayPalPaymentService for PayPal payments");
        }

        public Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Paymob integration will be implemented soon");
        }
    }
}

