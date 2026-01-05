namespace OrderSystem.Application.Validators
{
    public static class PasswordValidator
    {
        public static string? Validate(string password)
        {
            if (password.Length < 8)
                return "Password must be at least 8 characters";

            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter";

            if (!password.Any(char.IsLower))
                return "Password must contain at least one lowercase letter";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one number";

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                return "Password must contain at least one special character (!@#$%)";

            return null;
        }
    }
}