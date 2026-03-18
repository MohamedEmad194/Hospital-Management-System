using Hospital_Management_System.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hospital_Management_System.Services
{
    public class PayPalPaymentService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly IBillService _billService;
        private readonly ILogger<PayPalPaymentService> _logger;

        public PayPalPaymentService(
            IConfiguration configuration,
            IBillService billService,
            ILogger<PayPalPaymentService> logger)
        {
            _configuration = configuration;
            _billService = billService;
            _logger = logger;
        }

        public async Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency)
        {
            // NOTE: This is a **mock** implementation to avoid PayPal SDK issues.
            // It validates the bill and returns a fake PayPal order/URL so the app can compile and run.
            var bill = await _billService.GetBillByIdAsync(billId);
            if (bill == null)
                throw new InvalidOperationException("Bill not found");

            if (amount <= 0 || amount > bill.RemainingAmount)
                throw new InvalidOperationException("Invalid payment amount");

            var mode = (_configuration["PaymentGateways:PayPal:Mode"] ?? "sandbox").ToLower();
            var orderId = Guid.NewGuid().ToString("N");

            // Fake approval URL (for testing / front‑end redirection only)
            var baseUrl = mode == "live"
                ? "https://www.paypal.com/checkoutnow"
                : "https://www.sandbox.paypal.com/checkoutnow";

            var approvalUrl = $"{baseUrl}?token={orderId}";

            _logger.LogInformation(
                "Mock PayPal order created. OrderId: {OrderId}, BillId: {BillId}, Amount: {Amount}, Currency: {Currency}",
                orderId, billId, amount, currency);

            return new PayPalOrderResponse
            {
                OrderId = orderId,
                ApprovalUrl = approvalUrl,
                Amount = amount,
                Currency = currency
            };
        }

        public Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId)
        {
            // Mock verification: consider any PayPal payment as successful
            if (!string.Equals(gateway, "paypal", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            _logger.LogInformation("Mock PayPal payment verification. OrderId: {OrderId}, PaymentId: {PaymentId}",
                orderId, paymentId);

            return Task.FromResult(true);
        }

        // Stripe and Paymob implementations - not used here
        public Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Use StripePaymentService for Stripe payments");
        }

        public Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Paymob integration will be implemented soon");
        }
    }
}

