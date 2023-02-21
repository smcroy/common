namespace qs.Extensions.DecimalExtensions
{
    using System;
    using System.Globalization;

    public static class DecimalExtensions
    {
        #region Private Properties

        private static string CurrencyFormat
        {
            get
            {
                return string.Format("{{0:0.{0}}}", "0".PadRight(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, '0'));
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Returns the smallest integer greater than or equal to the specified decimal.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int Ceiling(this decimal a)
        {
            return (int)Math.Ceiling(a);
        }

        /// <summary>
        /// Compares this instance with another instance of nullable System.Decimal.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Decimal to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Decimal.</param>
        /// <returns>
        /// A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// Value Condition Less than zero This instance is less than a -or- this instance is null.
        /// Zero This instance is equal to a -or- this instance and a are both null. Greater than
        /// zero This instance is greater than a -or- a is null.
        /// </returns>
        public static int CompareTo(this decimal? a, decimal? b)
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
        /// Rounds the decimal value to the nearest integer.
        /// </summary>
        /// <param name="a">Instance of type System.Decimal.</param>
        /// <returns>Returns the result of rounding the decimal value as a decimal.</returns>
        public static decimal Round(this decimal a)
        {
            return Math.Round(a, MidpointRounding.AwayFromZero);
        }

        public static decimal Round(this decimal a, int digits)
        {
            return Math.Round(a, digits, MidpointRounding.AwayFromZero);
        }

        public static decimal RoundAsCurrency(this decimal a)
        {
            return Math.Round(a, CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the absolute value of a specified Decimal.
        /// </summary>
        /// <param name="a">System.Decimal number.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static decimal ToAbs(this decimal a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Returns the decimal formatted corresponding to the current culture currency decimal digits.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToCurrencyFormat(this decimal a)
        {
            return string.Format(CurrencyFormat, a);
        }

        /// <summary>
        /// Returns a decimal from an instance of type nullable System.Decimal.
        /// </summary>
        /// <param name="a">Instance of type nullable System.Decimal.</param>
        /// <returns>
        /// If the nullable decimal has a value, then returns the value as a decimal; otherwise
        /// returns 0.
        /// </returns>
        public static decimal ToDecimal(this decimal? a)
        {
            return a ?? 0;
        }

        /// <summary>
        /// Converts the value of the specified decimal number to an equivalent double-precision
        /// floating-point number.
        /// </summary>
        /// <param name="a">System.Decimal number.</param>
        /// <returns>Returns the value as a double-precision floating-point number.</returns>
        public static double ToDouble(this decimal a)
        {
            return Convert.ToDouble(a);
        }

        /// <summary>
        /// Converts the value of the specified nullable decimal number to an equivalent
        /// double-precision floating-point number. If the nullable decimal number does not have a
        /// value, returns 0.
        /// </summary>
        /// <param name="a">Nullable System.Decimal number.</param>
        /// <returns>Returns the value as a double-precision floating-point number.</returns>
        public static double ToDouble(this decimal? a)
        {
            return a.HasValue ? Convert.ToDouble(a.Value) : 0;
        }

        /// <summary>
        /// Rounds the decimal value to the nearest short.
        /// </summary>
        /// <param name="a">System.Int16 number.</param>
        /// <returns>Returns the result of rounding the decimal value as a Int16.</returns>
        public static short ToInt16(this decimal a)
        {
            if (a < short.MinValue || a > short.MaxValue)
                throw new OverflowException(string.Format("Decimal '{0}' can not be cast as an Int16.", a));
            else
                return (short)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Rounds the decimal value to the nearest integer.
        /// </summary>
        /// <param name="a">Instance of type System.Int32.</param>
        /// <returns>Returns the result of rounding the decimal value as a Int32.</returns>
        public static int ToInt32(this decimal a)
        {
            if (a < int.MinValue || a > int.MaxValue)
                throw new OverflowException(string.Format("Decimal '{0}' can not be cast as an Int32.", a));
            else
                return (int)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Rounds the decimal value to the nearest long.
        /// </summary>
        /// <param name="a">Instance of type System.Int64.</param>
        /// <returns>Returns the result of rounding the decimal value as a Int64.</returns>
        public static long ToInt64(this decimal a)
        {
            if (a < long.MinValue || a > long.MaxValue)
                throw new OverflowException(string.Format("Decimal '{0}' can not be cast as an Int64.", a));
            else
                return (long)(Math.Round(a, MidpointRounding.AwayFromZero));
        }

        #endregion Public Methods
    }
}