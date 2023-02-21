namespace qs.Extensions.DoubleExtensions
{
    using System;
    using System.Globalization;

    public static class DoubleExtensions
    {
        #region Properties

        private static string CurrencyFormat
        {
            get
            {
                return string.Format("{{0:0.{0}}}", "0".PadRight(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, '0'));
            }
        }

        private static string CurrencyFormatWithSeparators
        {
            get
            {
                return string.Format("{{0:#,0.{0}}}", "0".PadRight(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, '0'));
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///  Returns the smallest integer greater than or equal to the specified double.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int Ceiling(this double a)
        {
            return (int)Math.Ceiling(a);
        }

        /// <summary>
        /// Compares this instance with another instance of nullable System.Double.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Double to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Double.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands. Value Condition Less than zero This instance is less than a -or- this instance is null. Zero This instance is equal to a -or- this instance and a are both null. Greater than zero This instance is greater than a -or- a is null.</returns>
        public static int CompareTo(this double? a, double? b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;
            if (a == b)
                return 0;
            if (a < b)
                return -1;
            if (a > b)
                return 1;
            return 0;
        }

        /// <summary>
        /// Rounds the double-precision floating-point value to the nearest integer.
        /// When a number is halfway between two others, it is rounded toward the nearest number that is away from zero.
        /// </summary>
        /// <param name="a">Instance of type System.Double.</param>
        /// <returns>Returns the result of rounding the floating-point value as a double.</returns>
        public static double Round(this double a)
        {
            return Math.Round(a, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds a double-precision floating-point value to the specified number of fractional digits.
        /// digits: The number of fractional digits in the return value.
        /// When a number is halfway between two others, it is rounded toward the nearest number that is away from zero.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="digits">The number of fractional digits in the return value.</param>
        /// <returns></returns>
        public static double Round(this double a, int digits)
        {
            return Math.Round(a, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds a double-precision floating-point value to the number of fractional digits specified by the
        /// current culture's currency decimal digits.
        /// When a number is halfway between two others, it is rounded toward the nearest number that is away from zero.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double RoundAsCurrency(this double a)
        {
            return Math.Round(a, CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the absolute value of a specified Double.
        /// </summary>
        /// <param name="a">Instance of type System.Double.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static double ToAbs(this double a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Returns the double formatted corresponding to the current culture currency decimal digits.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToCurrencyFormat(this double a)
        {
            return string.Format(CurrencyFormat, a);
        }

        /// <summary>
        /// Returns the double formatted corresponding to the current culture currency decimal digits with separators.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToCurrencyFormatWithSeparators(this double a)
        {
            return string.Format(CurrencyFormatWithSeparators, a);
        }

        public static decimal ToDecimal(this double a)
        {
            return Convert.ToDecimal(a);
        }

        /// <summary>
        /// Returns a double from an instance of type nullable System.Double.
        /// </summary>
        /// <param name="a">Instance of type nullable System.Double.</param>
        /// <returns>If the nullable double has a value, then returns the value as a double; otherwise returns 0.</returns>
        public static double ToDouble(this double? a)
        {
            return a ?? 0;
        }

        /// <summary>
        /// Rounds the double-precision floating-point value to the nearest short.
        /// </summary>
        /// <param name="a">Instance of type System.Int16.</param>
        /// <returns>Returns the result of rounding the floating-point value as a Int16.</returns>
        public static short ToInt16(this double a)
        {
            if (a < short.MinValue || a > short.MaxValue)
                throw new OverflowException(string.Format("Double '{0}' can not be cast as an Int16.", a));
            else
                return (short)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Rounds the double-precision floating-point value to the nearest integer.
        /// </summary>
        /// <param name="a">Instance of type System.Int32.</param>
        /// <returns>Returns the result of rounding the floating-point value as a Int32.</returns>
        public static int ToInt32(this double a)
        {
            if (a < int.MinValue || a > int.MaxValue)
                throw new OverflowException(string.Format("Double '{0}' can not be cast as an Int32.", a));
            else
                return (int)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Rounds the double-precision floating-point value to the nearest long.
        /// </summary>
        /// <param name="a">Instance of type System.Int64.</param>
        /// <returns>Returns the result of rounding the floating-point value as a Int64.</returns>
        public static long ToInt64(this double a)
        {
            if (a < long.MinValue || a > long.MaxValue)
                throw new OverflowException(string.Format("Double '{0}' can not be cast as an Int64.", a));
            else
                return (long)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        #endregion Methods
    }
}