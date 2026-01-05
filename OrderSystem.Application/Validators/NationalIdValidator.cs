using System.Text.RegularExpressions;

namespace OrderSystem.Application.Validators
{
    public static class NationalIdValidator
    {
        public static bool IsValid(string value)
        {
            Regex rgx = new Regex(
                @"^([2,3])([0-9]{2})(0[1-9]|1[0-2])(?:0[1-9]|[12][0-9]|3[01])([0-9]{2})[0-9]{3}([0-9])[0-9]$"
            );

            if (!rgx.IsMatch(value)) return false;

            int century = Convert.ToInt32(value.Substring(0, 1));
            if (century < 2 || century > 3) return false;

            int year = Convert.ToInt32(value.Substring(1, 2));
            int month = Convert.ToInt32(value.Substring(3, 2));
            int day = Convert.ToInt32(value.Substring(5, 2));

            if (year < 0 || year > 99) return false;
            if (month < 1 || month > 12) return false;
            if (day < 1 || day > 31) return false;

            return true;
        }
    }
}