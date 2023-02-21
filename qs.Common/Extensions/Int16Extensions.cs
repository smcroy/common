namespace qs.Extensions.Int16Extensions
{
    using System;

    public static class Int16Extensions
    {
        #region Public Methods

        /// <summary>
        /// Compares this instance with another instance of nullable System.Int16.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Int16 to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Int16.</param>
        /// <returns>
        /// A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// Value Condition Less than zero This instance is less than a -or- this instance is null.
        /// Zero This instance is equal to a -or- this instance and a are both null. Greater than
        /// zero This instance is greater than a -or- a is null.
        /// </returns>
        public static int CompareTo(this short? a, short? b)
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
        /// Returns the absolute value of a specified Int16.
        /// </summary>
        /// <param name="a">Instance of type System.Int16.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static short ToAbs(this short a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Convert the Int16 to a System.Double.
        /// </summary>
        /// <param name="a">Instance of type System.Int16 to be converted.</param>
        /// <returns>Returns a System.Double.</returns>
        public static double ToDouble(this short a)
        {
            return Convert.ToDouble(a);
        }

        /// <summary>
        /// Convert the Int16 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int16</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToFloat(this short a)
        {
            return (float)a;
        }

        /// <summary>
        /// Convert the Int16 to its corresponding string hexadecimal representation.
        /// </summary>
        /// <param name="a">Instance of type System.Int16 to be converted.</param>
        /// <returns>Returns as a System.String the corresponding hexadecimal representation.</returns>
        public static string ToHex(this short a)
        {
            return a.ToString("x");
        }

        /// <summary>
        /// Convert the Int16 to the equivalent Roman Numeral.
        /// </summary>
        /// <param name="a">Instance of type System.Int16 to be converted.</param>
        /// <returns>Returns an instance of Roman Numeral.</returns>
        public static RomanNumeral ToRomanNumeral(this short a)
        {
            return new RomanNumeral(a);
        }

        /// <summary>
        /// Convert the Int16 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int16</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToSingle(this short a)
        {
            return (float)a;
        }

        #endregion Public Methods
    }
}