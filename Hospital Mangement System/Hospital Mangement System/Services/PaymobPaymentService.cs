using Hospital_Management_System.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Hospital_Management_System.Services
{
    public class PaymobPaymentService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly IBillService _billService;
        private readonly ILogger<PaymobPaymentService> _logger;
        private readonly HttpClient _httpClient;

        public PaymobPaymentService(
            IConfiguration configuration,
            IBillService billService,
            ILogger<PaymobPaymentService> logger,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _billService = billService;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://accept.paymob.com/api/");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(billId);
                if (bill == null)
                    throw new InvalidOperationException("Bill not found");

                if (amount <= 0 || amount > bill.RemainingAmount)
                    throw new InvalidOperationException("Invalid payment amount");

                var apiKey = _configuration["PaymentGateways:Paymob:ApiKey"];
                var integrationId = _configuration["PaymentGateways:Paymob:IntegrationId"];
                var iframeId = _configuration["PaymentGateways:Paymob:IframeId"];

                if (string.IsNullOrEmpty(apiKey) || apiKey == "your_paymob_api_key")
                {
                    throw new InvalidOperationException("Paymob API key not configured");
                }

                // Step 1: Get authentication token
                var authRequest = new
                {
                    api_key = apiKey
                };

                var authResponse = await _httpClient.PostAsJsonAsync("auth/tokens", authRequest);
                authResponse.EnsureSuccessStatusCode();
                
                var authResult = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponse>();
                if (authResult == null || string.IsNullOrEmpty(authResult.Token))
                {
                    throw new Exception("Failed to get Paymob authentication token");
                }

                var token = authResult.Token;
                _logger.LogInformation("Paymob authentication token obtained successfully");

                // Step 2: Create order
                var orderRequest = new
                {
                    auth_token = token,
                    delivery_needed = false,
                    amount_cents = (long)(amount * 100), // Convert to cents
                    currency = currency,
                    items = new[]
                    {
                        new
                        {
                            name = $"Bill #{bill.BillNumber}",
                            amount_cents = (long)(amount * 100),
                            description = $"Hospital bill payment - Patient: {bill.PatientName ?? "N/A"}",
                            quantity = 1
                        }
                    }
                };

                var orderResponse = await _httpClient.PostAsJsonAsync("ecommerce/orders", orderRequest);
                orderResponse.EnsureSuccessStatusCode();
                
                var orderResult = await orderResponse.Content.ReadFromJsonAsync<PaymobOrderCreateResponse>();
                if (orderResult == null)
                {
                    throw new Exception("Failed to create Paymob order");
                }

                _logger.LogInformation("Paymob order created successfully. OrderId: {OrderId}", orderResult.Id);

                // Step 3: Get payment key
                var paymentKeyRequest = new
                {
                    auth_token = token,
                    amount_cents = (long)(amount * 100),
                    expiration = 3600,
                    order_id = orderResult.Id,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = bill.PatientEmail ?? "patient@hospital.com",
                        floor = "NA",
                        first_name = bill.PatientName?.Split(' ').FirstOrDefault() ?? "Patient",
                        street = "NA",
                        building = "NA",
                        phone_number = "01000000000",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "EG",
                        last_name = bill.PatientName?.Split(' ').LastOrDefault() ?? "Name",
                        state = "NA"
                    },
                    currency = currency,
                    integration_id = int.Parse(integrationId ?? "0")
                };

                var paymentKeyResponse = await _httpClient.PostAsJsonAsync("acceptance/payment_keys", paymentKeyRequest);
                paymentKeyResponse.EnsureSuccessStatusCode();
                
                var paymentKeyResult = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>();
                if (paymentKeyResult == null || string.IsNullOrEmpty(paymentKeyResult.Token))
                {
                    throw new Exception("Failed to get Paymob payment key");
                }

                // Step 4: Return iframe URL
                var iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKeyResult.Token}";

                _logger.LogInformation("Paymob payment key obtained successfully. BillId: {BillId}", billId);

                return new PaymobOrderResponse
                {
                    OrderId = orderResult.Id.ToString(),
                    IframeUrl = iframeUrl,
                    Amount = amount,
                    Currency = currency
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Paymob API error while creating order for bill {BillId}", billId);
                throw new Exception($"Paymob API error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Paymob order for bill {BillId}", billId);
                throw;
            }
        }

        public async Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId)
        {
            if (gateway.ToLower() != "paymob")
                return false;

            try
            {
                // Paymob verification would typically be done via webhook
                // For now, we'll return true if orderId is provided
                // In production, implement proper verification via webhook
                return !string.IsNullOrEmpty(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Paymob payment. OrderId: {OrderId}", orderId);
                return false;
            }
        }

        // Stripe and PayPal implementations - not used here
        public Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Use StripePaymentService for Stripe payments");
        }

        public Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency)
        {
            throw new NotImplementedException("Use PayPalPaymentService for PayPal payments");
        }
    }

    // DTOs for Paymob
    public class PaymobAuthResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }

    public class PaymobOrderCreateResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class PaymobPaymentKeyResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}

