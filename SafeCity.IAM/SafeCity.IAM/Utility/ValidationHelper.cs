using System.Text.RegularExpressions;

namespace SafeCity.IAM.Utility
{
    public static class ValidationHelper
    {
        public static string RequestNull { get; set; } = "User Did Not Made Any Request";

        public static string EmailHelper(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Email is required.";

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex))
                return "Invalid email format.";

            return null;
        }

        public static string PasswordHelper(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password is required.";

            if (password.Length < 8)
                return "Password must be at least 8 characters long.";

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return "Password must contain at least one uppercase letter.";

            if (!Regex.IsMatch(password, @"[0-9]"))
                return "Password must contain at least one number.";

            return null;
        }

        public static string PhoneHelper(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "Phone number is required.";

            var phoneRegex = @"^\d{10}$";
            if (!Regex.IsMatch(phone, phoneRegex))
                return "Phone number must be exactly 10 digits.";

            return null;
        }
    }
}