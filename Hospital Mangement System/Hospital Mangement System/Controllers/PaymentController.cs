using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Services;
using Stripe;
using Stripe.Checkout;
using System.Text;

namespace Hospital_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly PayPalPaymentService _paypalPaymentService;
        private readonly PaymobPaymentService _paymobPaymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IBillService billService,
            IPaymentGatewayService paymentGatewayService,
            PayPalPaymentService paypalPaymentService,
            PaymobPaymentService paymobPaymentService,
            ILogger<PaymentController> logger,
            IConfiguration configuration)
        {
            _billService = billService;
            _paymentGatewayService = paymentGatewayService;
            _paypalPaymentService = paypalPaymentService;
            _paymobPaymentService = paymobPaymentService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Create Stripe payment session
        /// </summary>
        [HttpPost("create-stripe-session")]
        public async Task<ActionResult> CreateStripeSession([FromBody] StripePaymentRequest request)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(request.BillId);
                if (bill == null)
                    return NotFound(new { message = "Bill not found" });

                if (request.Amount <= 0 || request.Amount > bill.RemainingAmount)
                    return BadRequest(new { message = "Invalid payment amount" });

                var result = await _paymentGatewayService.CreateStripeSessionAsync(
                    request.BillId,
                    request.Amount,
                    request.Currency ?? "EGP"
                );

                _logger.LogInformation("Stripe session created successfully. SessionId: {SessionId}, BillId: {BillId}", 
                    result.SessionId, request.BillId);

                return Ok(new
                {
                    sessionId = result.SessionId,
                    checkoutUrl = result.CheckoutUrl,
                    amount = result.Amount,
                    currency = result.Currency
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe session for bill {BillId}", request.BillId);
                return StatusCode(500, new { message = $"Error creating payment session: {ex.Message}" });
            }
        }

        /// <summary>
        /// Create PayPal payment order
        /// </summary>
        [HttpPost("create-paypal-order")]
        public async Task<ActionResult> CreatePayPalOrder([FromBody] PayPalPaymentRequest request)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(request.BillId);
                if (bill == null)
                    return NotFound(new { message = "Bill not found" });

                if (request.Amount <= 0 || request.Amount > bill.RemainingAmount)
                    return BadRequest(new { message = "Invalid payment amount" });

                var result = await _paypalPaymentService.CreatePayPalOrderAsync(
                    request.BillId,
                    request.Amount,
                    request.Currency ?? "EGP"
                );

                _logger.LogInformation("PayPal order created successfully. OrderId: {OrderId}, BillId: {BillId}", 
                    result.OrderId, request.BillId);

                return Ok(new
                {
                    orderId = result.OrderId,
                    approvalUrl = result.ApprovalUrl,
                    amount = result.Amount,
                    currency = result.Currency
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for bill {BillId}", request.BillId);
                return StatusCode(500, new { message = $"Error creating payment order: {ex.Message}" });
            }
        }

        /// <summary>
        /// Create Paymob payment order
        /// </summary>
        [HttpPost("create-paymob-order")]
        public async Task<ActionResult> CreatePaymobOrder([FromBody] PaymobPaymentRequest request)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(request.BillId);
                if (bill == null)
                    return NotFound(new { message = "Bill not found" });

                if (request.Amount <= 0 || request.Amount > bill.RemainingAmount)
                    return BadRequest(new { message = "Invalid payment amount" });

                var result = await _paymobPaymentService.CreatePaymobOrderAsync(
                    request.BillId,
                    request.Amount,
                    request.Currency ?? "EGP"
                );

                _logger.LogInformation("Paymob order created successfully. OrderId: {OrderId}, BillId: {BillId}", 
                    result.OrderId, request.BillId);

                return Ok(new
                {
                    orderId = result.OrderId,
                    iframeUrl = result.IframeUrl,
                    amount = result.Amount,
                    currency = result.Currency
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Paymob order for bill {BillId}", request.BillId);
                return StatusCode(500, new { message = $"Error creating payment order: {ex.Message}" });
            }
        }

        /// <summary>
        /// Stripe webhook handler
        /// </summary>
        [HttpPost("stripe-webhook")]
        [AllowAnonymous]
        public async Task<ActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();
                var webhookSecret = _configuration["PaymentGateways:Stripe:WebhookSecret"];

                if (string.IsNullOrEmpty(webhookSecret) || webhookSecret == "whsec_your_webhook_secret")
                {
                    _logger.LogWarning("Stripe webhook secret not configured. Skipping signature verification.");
                    // In development, you might want to parse without verification
                    // In production, always verify the signature
                }

                Stripe.Event stripeEvent;
                
                try
                {
                    if (!string.IsNullOrEmpty(webhookSecret) && webhookSecret != "whsec_your_webhook_secret")
                    {
                        stripeEvent = EventUtility.ConstructEvent(
                            json,
                            signatureHeader,
                            webhookSecret
                        );
                    }
                    else
                    {
                        // Development mode - parse without verification
                        stripeEvent = EventUtility.ParseEvent(json);
                    }
                }
                catch (StripeException ex)
                {
                    _logger.LogError(ex, "Stripe webhook signature verification failed");
                    return BadRequest(new { message = "Invalid signature" });
                }

                _logger.LogInformation("Stripe webhook received: {EventType}, Id: {EventId}", 
                    stripeEvent.Type, stripeEvent.Id);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    
                    if (session != null && session.Metadata != null && session.Metadata.ContainsKey("billId"))
                    {
                        var billId = int.Parse(session.Metadata["billId"]);
                        var amount = (decimal)(session.AmountTotal / 100.0); // Convert from cents

                        var paymentDto = new PaymentDto
                        {
                            Amount = amount,
                            PaymentMethod = "Stripe",
                            Notes = $"Stripe Payment - Session: {session.Id}, Customer: {session.CustomerEmail ?? "N/A"}"
                        };

                        var result = await _billService.ProcessPaymentAsync(billId, paymentDto);
                        
                        if (result)
                        {
                            _logger.LogInformation("Payment processed successfully via Stripe. BillId: {BillId}, Amount: {Amount}", 
                                billId, amount);
                            return Ok(new { message = "Payment processed successfully" });
                        }
                        else
                        {
                            _logger.LogWarning("Failed to process payment. BillId: {BillId}", billId);
                            return StatusCode(500, new { message = "Failed to process payment" });
                        }
                    }
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation("PaymentIntent succeeded: {PaymentIntentId}", paymentIntent?.Id);
                }

                return Ok(new { message = "Webhook received" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500, new { message = "Error processing webhook" });
            }
        }

        /// <summary>
        /// Handle payment callback/webhook from payment gateway (generic)
        /// </summary>
        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<ActionResult> PaymentCallback([FromBody] PaymentCallbackRequest request)
        {
            try
            {
                _logger.LogInformation("Payment callback received: {Gateway}, OrderId: {OrderId}, Status: {Status}",
                    request.Gateway, request.OrderId, request.Status);

                // Verify payment with gateway
                var isVerified = await _paymentGatewayService.VerifyPaymentAsync(
                    request.Gateway, 
                    request.OrderId, 
                    request.PaymentId ?? request.OrderId
                );

                if (isVerified && (request.Status == "success" || request.Status == "paid"))
                {
                    var paymentDto = new PaymentDto
                    {
                        Amount = request.Amount,
                        PaymentMethod = request.Gateway,
                        Notes = $"Payment via {request.Gateway} - Order ID: {request.OrderId}"
                    };

                    var result = await _billService.ProcessPaymentAsync(request.BillId, paymentDto);
                    if (result)
                    {
                        return Ok(new { message = "Payment processed successfully" });
                    }
                }

                return BadRequest(new { message = "Payment verification failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback");
                return StatusCode(500, new { message = "Error processing payment" });
            }
        }
    }

    // DTOs for payment requests
    public class StripePaymentRequest
    {
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }

    public class PayPalPaymentRequest
    {
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }

    public class PaymobPaymentRequest
    {
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }

    public class PaymentCallbackRequest
    {
        public string Gateway { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentId { get; set; }
    }
}

