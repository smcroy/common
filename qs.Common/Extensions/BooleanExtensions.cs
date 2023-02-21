namespace qs.Extensions.BooleanExtensions
{
    /// <summary>
    /// These are boolean extension methods.
    /// </summary>
    public static class BooleanExtensions
    {
        #region Methods

        /// <summary>
        /// Compares this instance with another instance of nullable System.Boolean.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Boolean to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Boolean.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands. Value Condition Less than zero This instance is false and a is true -or- this instance is null and a is not null. Zero This instance is equal to a -or- this instance and a are both null. Greater than zero This instance is true and a is false -or- a is null.</returns>
        public static int CompareTo(this bool? a, bool? b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;
            if (a == b)
                return 0;
            if (!a.Value)
                return -1;
            if (a.Value)
                return 1;
            return 0;
        }

        /// <summary>
        /// Returns the boolean as an integer. If True returns 1; otherwise 0.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int ToInt(this bool a)
        {
            return a ? 1 : 0;
        }

        #endregion Methods
    }
}