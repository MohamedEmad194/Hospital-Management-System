using System.Net;
using System.Net.Mail;

namespace Hospital_Management_System.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = smtpSettings.GetValue<int>("Port", 587);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var smtpFromEmail = smtpSettings["FromEmail"] ?? smtpUsername;
                var smtpFromName = smtpSettings["FromName"] ?? "Hospital Management System";
                var enableSsl = smtpSettings.GetValue<bool>("EnableSsl", true);

                // If SMTP is not configured, log and return false
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername))
                {
                    _logger.LogWarning("SMTP settings not configured. Email not sent to {Email}. Subject: {Subject}", to, subject);
                    _logger.LogInformation("Email content that would be sent:\nTo: {To}\nSubject: {Subject}\nBody: {Body}", to, subject, body);
                    return false;
                }

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = enableSsl;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(smtpFromEmail ?? "noreply@hospital.com", smtpFromName);
                        message.To.Add(to);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;

                        await client.SendMailAsync(message);
                        _logger.LogInformation("Password reset email sent successfully to {Email}", to);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Subject: {Subject}", to, subject);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetLink, string userName)
        {
            var subject = "إعادة تعيين كلمة المرور - نظام إدارة المستشفى";
            var body = $@"
<!DOCTYPE html>
<html dir='rtl' lang='ar'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>نظام إدارة المستشفى</h1>
        </div>
        <div class='content'>
            <h2>مرحباً {userName}</h2>
            <p>لقد طلبت إعادة تعيين كلمة المرور لحسابك.</p>
            <p>انقر على الزر أدناه لإعادة تعيين كلمة المرور:</p>
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>إعادة تعيين كلمة المرور</a>
            </div>
            <p>أو انسخ الرابط التالي ولصقه في المتصفح:</p>
            <p style='word-break: break-all; background: #fff; padding: 10px; border-radius: 5px;'>{resetLink}</p>
            <p><strong>ملاحظة:</strong> هذا الرابط صالح لمدة ساعة واحدة فقط.</p>
            <p>إذا لم تطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذا البريد بأمان.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} نظام إدارة المستشفى. جميع الحقوق محفوظة.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(to, subject, body, true);
        }
    }
}

