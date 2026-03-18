# دليل تكامل بوابات الدفع الحقيقية

## الوضع الحالي
الكود الحالي **تجريبي (Mock)** وليس دفعًا حقيقيًا. يحتاج إلى تكامل فعلي مع بوابات الدفع.

## الخطوات المطلوبة للدفع الحقيقي

### 1. إضافة NuGet Packages

#### للـ Stripe:
```bash
dotnet add package Stripe.net
```

#### للـ PayPal:
```bash
dotnet add package PayPalCheckoutSdk
```

#### للـ Paymob:
```bash
# Paymob لا يوجد NuGet package رسمي، سنستخدم HttpClient مباشرة
```

### 2. تحديث ملف .csproj

أضف هذه الـ packages في `Hospital Mangement System.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Stripe.net" Version="43.0.0" />
  <PackageReference Include="PayPalCheckoutSdk" Version="1.0.3" />
</ItemGroup>
```

### 3. إنشاء Service للدفع

إنشاء ملف `Services/IPaymentGatewayService.cs`:

```csharp
public interface IPaymentGatewayService
{
    Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency);
    Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency);
    Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency);
    Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId);
}
```

### 4. تنفيذ Stripe الحقيقي

#### أ. إضافة Stripe Service:

```csharp
using Stripe;
using Stripe.Checkout;

public class StripePaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _configuration;
    private readonly IBillService _billService;

    public StripePaymentService(IConfiguration configuration, IBillService billService)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["PaymentGateways:Stripe:SecretKey"];
        _billService = billService;
    }

    public async Task<StripeSessionResponse> CreateStripeSessionAsync(int billId, decimal amount, string currency)
    {
        var bill = await _billService.GetBillByIdAsync(billId);
        if (bill == null)
            throw new Exception("Bill not found");

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
                            Description = $"Payment for hospital bill"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = $"{_configuration["FrontendBaseUrl"]}/bills/{billId}/payment/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_configuration["FrontendBaseUrl"]}/bills/{billId}/payment/cancel",
            Metadata = new Dictionary<string, string>
            {
                { "billId", billId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new StripeSessionResponse
        {
            SessionId = session.Id,
            CheckoutUrl = session.Url
        };
    }

    public async Task<bool> VerifyPaymentAsync(string gateway, string orderId, string paymentId)
    {
        if (gateway != "stripe") return false;

        var service = new SessionService();
        var session = await service.GetAsync(orderId);

        return session.PaymentStatus == "paid";
    }
}
```

### 5. تنفيذ PayPal الحقيقي

```csharp
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

public class PayPalPaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _configuration;
    private readonly IBillService _billService;

    public PayPalPaymentService(IConfiguration configuration, IBillService billService)
    {
        _configuration = configuration;
        _billService = billService;
    }

    public async Task<PayPalOrderResponse> CreatePayPalOrderAsync(int billId, decimal amount, string currency)
    {
        var bill = await _billService.GetBillByIdAsync(billId);
        if (bill == null)
            throw new Exception("Bill not found");

        var environment = new SandboxEnvironment(
            _configuration["PaymentGateways:PayPal:ClientId"],
            _configuration["PaymentGateways:PayPal:ClientSecret"]
        );

        var client = new PayPalHttpClient(environment);

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            ApplicationContext = new ApplicationContext
            {
                BrandName = "Al Hayah Hospital",
                LandingPage = "BILLING",
                UserAction = "PAY_NOW",
                ReturnUrl = $"{_configuration["FrontendBaseUrl"]}/bills/{billId}/payment/success",
                CancelUrl = $"{_configuration["FrontendBaseUrl"]}/bills/{billId}/payment/cancel"
            },
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    ReferenceId = billId.ToString(),
                    Description = $"Bill #{bill.BillNumber}",
                    Amount = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2")
                    }
                }
            }
        });

        var response = await client.Execute(request);
        var order = response.Result<Order>();

        var approvalUrl = order.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;

        return new PayPalOrderResponse
        {
            OrderId = order.Id,
            ApprovalUrl = approvalUrl
        };
    }
}
```

### 6. تنفيذ Paymob الحقيقي

```csharp
public class PaymobPaymentService : IPaymentGatewayService
{
    private readonly IConfiguration _configuration;
    private readonly IBillService _billService;
    private readonly HttpClient _httpClient;

    public PaymobPaymentService(IConfiguration configuration, IBillService billService, HttpClient httpClient)
    {
        _configuration = configuration;
        _billService = billService;
        _httpClient = httpClient;
    }

    public async Task<PaymobOrderResponse> CreatePaymobOrderAsync(int billId, decimal amount, string currency)
    {
        var bill = await _billService.GetBillByIdAsync(billId);
        if (bill == null)
            throw new Exception("Bill not found");

        var apiKey = _configuration["PaymentGateways:Paymob:ApiKey"];
        var integrationId = _configuration["PaymentGateways:Paymob:IntegrationId"];

        // Step 1: Get authentication token
        var authRequest = new
        {
            api_key = apiKey
        };

        var authResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", authRequest);
        var authResult = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponse>();
        var token = authResult.Token;

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
                    description = "Hospital bill payment",
                    quantity = 1
                }
            }
        };

        var orderResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", orderRequest);
        var orderResult = await orderResponse.Content.ReadFromJsonAsync<PaymobOrderCreateResponse>();

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
                email = bill.Patient?.Email ?? "patient@hospital.com",
                floor = "NA",
                first_name = bill.Patient?.FirstName ?? "Patient",
                street = "NA",
                building = "NA",
                phone_number = bill.Patient?.Phone ?? "01000000000",
                shipping_method = "NA",
                postal_code = "NA",
                city = "NA",
                country = "EG",
                last_name = bill.Patient?.LastName ?? "Name",
                state = "NA"
            },
            currency = currency,
            integration_id = int.Parse(integrationId)
        };

        var paymentKeyResponse = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyRequest);
        var paymentKeyResult = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>();

        // Step 4: Return iframe URL
        var iframeId = _configuration["PaymentGateways:Paymob:IframeId"];
        var iframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKeyResult.Token}";

        return new PaymobOrderResponse
        {
            OrderId = orderResult.Id.ToString(),
            IframeUrl = iframeUrl
        };
    }
}

// DTOs for Paymob
public class PaymobAuthResponse
{
    public string Token { get; set; }
}

public class PaymobOrderCreateResponse
{
    public int Id { get; set; }
}

public class PaymobPaymentKeyResponse
{
    public string Token { get; set; }
}
```

### 7. تحديث PaymentController

استبدل الكود Mock بالكود الحقيقي:

```csharp
[HttpPost("create-stripe-session")]
public async Task<ActionResult> CreateStripeSession([FromBody] StripePaymentRequest request)
{
    try
    {
        var result = await _paymentGatewayService.CreateStripeSessionAsync(
            request.BillId, 
            request.Amount, 
            request.Currency ?? "EGP"
        );
        
        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating Stripe session");
        return StatusCode(500, new { message = "Error creating payment session" });
    }
}
```

### 8. إضافة Webhook Handler

لـ Stripe:

```csharp
[HttpPost("stripe-webhook")]
[AllowAnonymous]
public async Task<ActionResult> StripeWebhook()
{
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    
    try
    {
        var stripeEvent = EventUtility.ParseEvent(json);
        var signatureHeader = Request.Headers["Stripe-Signature"];

        stripeEvent = EventUtility.ConstructEvent(
            json,
            signatureHeader,
            _configuration["PaymentGateways:Stripe:WebhookSecret"]
        );

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            var billId = int.Parse(session.Metadata["billId"]);
            
            var paymentDto = new PaymentDto
            {
                Amount = (decimal)(session.AmountTotal / 100.0),
                PaymentMethod = "Stripe",
                Notes = $"Stripe Payment - Session: {session.Id}"
            };

            await _billService.ProcessPaymentAsync(billId, paymentDto);
        }

        return Ok();
    }
    catch (StripeException e)
    {
        _logger.LogError(e, "Stripe webhook error");
        return BadRequest();
    }
}
```

### 9. تسجيل Services في Program.cs

```csharp
builder.Services.AddScoped<IPaymentGatewayService, StripePaymentService>();
builder.Services.AddScoped<PayPalPaymentService>();
builder.Services.AddScoped<PaymobPaymentService>();
builder.Services.AddHttpClient<PaymobPaymentService>();
```

### 10. إعدادات appsettings.json

```json
{
  "PaymentGateways": {
    "Stripe": {
      "SecretKey": "sk_live_YOUR_REAL_SECRET_KEY",
      "PublishableKey": "pk_live_YOUR_REAL_PUBLISHABLE_KEY",
      "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET"
    },
    "PayPal": {
      "ClientId": "YOUR_REAL_PAYPAL_CLIENT_ID",
      "ClientSecret": "YOUR_REAL_PAYPAL_CLIENT_SECRET",
      "Mode": "live"
    },
    "Paymob": {
      "ApiKey": "YOUR_REAL_PAYMOB_API_KEY",
      "IntegrationId": "YOUR_INTEGRATION_ID",
      "IframeId": "YOUR_IFRAME_ID"
    }
  }
}
```

## ملاحظات مهمة

1. **البيئة التجريبية (Sandbox)**: استخدم `sk_test_` و `pk_test_` للـ Stripe في التطوير
2. **البيئة الحقيقية (Production)**: استخدم `sk_live_` و `pk_live_` عند النشر
3. **الأمان**: لا تضع المفاتيح الحقيقية في الكود، استخدم Environment Variables أو Azure Key Vault
4. **Webhooks**: تأكد من إعداد webhooks في لوحة تحكم Stripe/PayPal/Paymob
5. **SSL**: يجب أن يكون الموقع على HTTPS في الإنتاج

## الخطوات التالية

1. تثبيت الـ NuGet packages
2. إنشاء Services للدفع
3. تحديث PaymentController
4. إضافة Webhook handlers
5. اختبار في بيئة Sandbox أولاً
6. الانتقال للبيئة الحقيقية بعد التأكد من كل شيء

