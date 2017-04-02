using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SieveofEratosthenes.Validations
{
    public class NumericValidationRule : ValidationRule
    {
        public Type ValidationType { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, "Input is required.");

            if (string.IsNullOrWhiteSpace(strValue.Trim('0')) || strValue.TrimStart('0') == "1")
                return new ValidationResult(false, "Input is invalid.");


            if (IsTextAllowed(strValue))
            {
                switch (ValidationType.Name)
                {
                    case "Int32":
                        Int32 longVal;
                        bool canConvert = Int32.TryParse(strValue, out longVal);
                        if(longVal > 1000000000)
                        {
                            return new ValidationResult(false, "Input cannot be bigger than 1 Billion.");
                        }
                        return canConvert
                                   ? new ValidationResult(true, null)
                                   : new ValidationResult(false, "Input value is invalid");
                    default:
                        throw new InvalidCastException("{ValidationType.Name} is not supported");
                }
            }
            return new ValidationResult(false, "Input is invalid.");
        }

        private static bool IsTextAllowed(string text)
        {
            var regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
