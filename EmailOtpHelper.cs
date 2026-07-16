using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace CareerConnect.USER
{
    public static class EmailOtpHelper
    {
        public const int OtpExpiryMinutes = 20;

        public static string GenerateSixDigitOtp()
        {
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            var n = BitConverter.ToUInt32(bytes, 0) % 900000u + 100000u;
            return n.ToString();
        }

        public static string HashOtp(int userId, string otp)
        {
            var pepper = (ConfigurationManager.AppSettings["EmailOtpPepper"] ?? "CareerConnectOtpPepper").Trim();
            using (var sha = SHA256.Create())
            {
                var raw = Encoding.UTF8.GetBytes(pepper + ":" + userId + ":" + otp.Trim());
                return Convert.ToBase64String(sha.ComputeHash(raw));
            }
        }

        /// <summary>Stores hashed OTP, clears link token; sends email. Connection must be open.</summary>
        public static bool IssueAndSendOtp(MySqlConnection conn, int userId, string email, string displayName)
        {
            if (conn == null || string.IsNullOrWhiteSpace(email) || userId <= 0)
                return false;

            var otp = GenerateSixDigitOtp();
            var hash = HashOtp(userId, otp);
            var exp = DateTime.Now.AddMinutes(OtpExpiryMinutes);

            using (var cmd = new MySqlCommand(@"
                UPDATE users SET
                  email_verify_otp_hash = @h,
                  email_verify_otp_expires = @e,
                  email_verification_token = NULL,
                  email_verification_expires = NULL
                WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@e", exp);
                cmd.Parameters.AddWithValue("@id", userId);
                if (cmd.ExecuteNonQuery() <= 0)
                    return false;
            }

            return EmailVerification.TrySendOtpEmail(email, displayName, otp);
        }

        public static bool VerifyOtp(MySqlConnection conn, int userId, string otp)
        {
            if (string.IsNullOrWhiteSpace(otp) || userId <= 0)
                return false;

            string stored = null;
            DateTime? exp = null;
            using (var cmd = new MySqlCommand(
                "SELECT email_verify_otp_hash, email_verify_otp_expires FROM users WHERE id = @id LIMIT 1", conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return false;
                    stored = r["email_verify_otp_hash"]?.ToString();
                    if (r["email_verify_otp_expires"] != DBNull.Value)
                        exp = Convert.ToDateTime(r["email_verify_otp_expires"]);
                }
            }

            if (string.IsNullOrEmpty(stored) || !exp.HasValue || exp.Value < DateTime.Now)
                return false;

            var hash = HashOtp(userId, otp.Trim());
            return FixedTimeEquals(stored, hash);
        }

        public static void ClearOtpAndVerify(MySqlConnection conn, int userId)
        {
            using (var cmd = new MySqlCommand(@"
                UPDATE users SET
                  email_verified = 1,
                  email_verify_otp_hash = NULL,
                  email_verify_otp_expires = NULL,
                  email_verification_token = NULL,
                  email_verification_expires = NULL
                WHERE id = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
