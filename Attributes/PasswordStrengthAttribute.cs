using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiChitra.Attributes
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string password)
                return false;

            // At least 8 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }

        public override string FormatErrorMessage(string name)
        {
            return "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&).";
        }
    }
}