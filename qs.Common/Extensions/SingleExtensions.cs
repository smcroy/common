namespace qs.Extensions.SingleExtensions
{
    using System;

    public static class SingleExtensions
    {
        #region Methods

        /// <summary>
        /// Compares this instance with another instance of nullable System.Single.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Single to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Single.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands. Value Condition Less than zero This instance is less than a -or- this instance is null. Zero This instance is equal to a -or- this instance and a are both null. Greater than zero This instance is greater than a -or- a is null.</returns>
        public static int CompareTo(this float? a, float? b)
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
        /// Returns the absolute value of a specified Single.
        /// </summary>
        /// <param name="a">Instance of type System.Single.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static float ToAbs(this float a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Rounds the single-precision floating-point value to the nearest integer.
        /// </summary>
        /// <param name="a">Instance of type System.Int32.</param>
        /// <returns>Returns the result of rounding the floating-point value as a Int32.</returns>
        public static int ToInt32(this float a)
        {
            return (int)Math.Round(a) * 1;
        }

        #endregion Methods
    }
}