namespace Hospital_Management_System.Services
{
    public interface IPaymentGatewayService
    {
        Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency);
        Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency);
        Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency);
        Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId);
    }

    public class StripeSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
    }

    public class PayPalOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string ApprovalUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
    }

    public class PaymobOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string IframeUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
    }
}

