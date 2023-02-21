namespace qs.Extensions.Int64Extensions
{
    using System;

    //using System.Numeric;
    public static class Int64Extensions
    {
        #region Methods

        /// <summary>
        /// Compares this instance with another instance of nullable System.Int64.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Int64 to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Int64.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands. Value Condition Less than zero This instance is less than a -or- this instance is null. Zero This instance is equal to a -or- this instance and a are both null. Greater than zero This instance is greater than a -or- a is null.</returns>
        public static int CompareTo(this long? a, long? b)
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
        /// Returns the absolute value of a specified Int64.
        /// </summary>
        /// <param name="a">Instance of type System.Int64.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static long ToAbs(this long a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Convert the Int64 to a System.Double.
        /// </summary>
        /// <param name="a">Instance of type System.Int64 to be converted.</param>
        /// <returns>Returns a System.Double.</returns>
        public static double ToDouble(this long a)
        {
            return Convert.ToDouble(a);
        }

        /// <summary>
        /// Convert the Int64 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int64</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToFloat(this long a)
        {
            return (float)a;
        }

        /// <summary>
        /// Convert the Int64 to its corresponding string hexadecimal representation.
        /// </summary>
        /// <param name="a">Instance of type System.Int64 to be converted.</param>
        /// <returns>Returns as a System.String the corresponding hexadecimal representation.</returns>
        public static string ToHex(this long a)
        {
            return a.ToString("x");
        }

        public static short ToInt16(this long a)
        {
            return Convert.ToInt16(a);
        }

        public static int ToInt32(this long a)
        {
            return Convert.ToInt32(a);
        }

        /// <summary>
        /// Convert the Int64 to the equivalent Roman Numeral.
        /// </summary>
        /// <param name="a">Instance of type System.Int64 to be converted.</param>
        /// <returns>Returns an instance of Roman Numeral.</returns>
        public static RomanNumeral ToRomanNumeral(this long a)
        {
            return new RomanNumeral(a);
        }

        /// <summary>
        /// Convert the Int64 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int64</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToSingle(this long a)
        {
            return (float)a;
        }

        #endregion Methods
    }
}