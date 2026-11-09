using System;
using System.Text.RegularExpressions;

namespace RepairCenter.Helpers
{
    /// <summary>
    /// Static helper class for validating user input.
    /// </summary>
    public static class Validator
    {
        // Serial format: two capital letters, a dash, five digits. Example: RF-10234
        private static readonly Regex SerialPattern = new Regex(@"^[A-Z]{2}-\d{5}$");

        public static bool IsValidSerialNumber(string serial)
        {
            return !string.IsNullOrWhiteSpace(serial) && SerialPattern.IsMatch(serial);
        }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2;
        }

        public static bool IsPositive(decimal value)
        {
            return value > 0;
        }

        public static bool IsNonNegative(decimal value)
        {
            return value >= 0;
        }

        /// <summary>
        /// The expected delivery date must not be before the received date.
        /// </summary>
        public static bool IsValidDeliveryDate(DateTime received, DateTime expected)
        {
            return expected.Date >= received.Date;
        }
    }
}
