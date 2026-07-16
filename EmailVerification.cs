using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;

namespace CareerConnect.USER
{
    public static class EmailVerification
    {
        public const int TokenExpiryHours = 48;

        public static string GenerateToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static bool TrySendVerificationEmail(string toEmail, string displayName, string verificationToken)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return false;

            if (!MailSender.IsSmtpConfigured())
                return false;

            var baseUrl = (ConfigurationManager.AppSettings["SitePublicUrl"] ?? "").Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(baseUrl) && HttpContext.Current != null)
            {
                var req = HttpContext.Current.Request;
                baseUrl = req.Url.GetLeftPart(UriPartial.Authority) +
                    (string.IsNullOrEmpty(req.ApplicationPath) ? "" : req.ApplicationPath.TrimEnd('/'));
            }
            if (string.IsNullOrEmpty(baseUrl))
                baseUrl = "http://localhost";

            var verifyPath = "USER/VerifyEmail.aspx?token=" + Uri.EscapeDataString(verificationToken);
            var link = baseUrl.Contains("://")
                ? baseUrl.TrimEnd('/') + "/" + verifyPath
                : baseUrl + "/" + verifyPath;

            var body =
                "Hello" + (string.IsNullOrWhiteSpace(displayName) ? "," : " " + displayName + ",") + "\r\n\r\n" +
                "Please verify your email address for your Career Connect account by clicking the link below (valid " +
                TokenExpiryHours + " hours):\r\n\r\n" +
                link + "\r\n\r\n" +
                "If you did not create an account, you can ignore this message.\r\n";

            return MailSender.TrySend(toEmail, "Verify your Career Connect email", body, isHtml: false);
        }

        public static bool TrySendOtpEmail(string toEmail, string displayName, string otpCode)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(otpCode))
                return false;
            if (!MailSender.IsSmtpConfigured())
                return false;

            var body =
                "Hello" + (string.IsNullOrWhiteSpace(displayName) ? "," : " " + displayName + ",") + "\r\n\r\n" +
                "Your Career Connect email verification code is:\r\n\r\n" +
                "  " + otpCode + "\r\n\r\n" +
                "Enter this code on the website after you sign in. The code expires in " + EmailOtpHelper.OtpExpiryMinutes + " minutes.\r\n\r\n" +
                "If you did not create an account, you can ignore this message.\r\n";

            return MailSender.TrySend(toEmail, "Your Career Connect verification code", body, isHtml: false);
        }
    }
}
