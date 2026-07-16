using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CareerConnect.USER
{
    public static class InputValidation
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim();
            if (email.Length > 120) return false;
            try
            {
                _ = new MailAddress(email).Address;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>3–32 chars: letters, digits, underscore.</summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var s = username.Trim();
            if (s.Length < 3 || s.Length > 32) return false;
            return Regex.IsMatch(s, @"^[a-zA-Z0-9_]+$");
        }

        /// <summary>Min 8 chars, at least one letter and one digit.</summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8 || password.Length > 128)
                return false;
            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);
            return hasLetter && hasDigit;
        }

        public static string ValidatePasswordMessage =>
            "Password must be 8–128 characters and include at least one letter and one number.";

        /// <summary>Reasonable full name for registration.</summary>
        public static bool IsValidFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return false;
            var s = fullName.Trim();
            if (s.Length < 2 || s.Length > 120) return false;
            if (s.Any(c => char.IsControl(c))) return false;
            return true;
        }

        /// <summary>Optional phone: digits, spaces, + - () between 10 and 20 visible chars.</summary>
        public static bool IsValidPhone(string phone, bool required)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return !required;
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length >= 10 && digits.Length <= 15;
        }

        public static bool IsValidExperienceYears(int years) => years >= 0 && years <= 60;

        public static string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            return phone.Trim();
        }
    }
}
