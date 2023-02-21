namespace qs.Extensions.IEnumerableExtensions
{
    using System;
    using System.Collections.Generic;

    public static class IEnumerableExtensions
    {
        #region Methods

        /// <summary>
        /// For each item in the collection implementing the IEnumerable interface, execute the specified action.
        /// <code>
        /// IEnumerable<int> i = new int[ ] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        /// i.ForEach( delegate ( int j )
        /// {
        ///     Console.WriteLine( j );
        /// } );
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> a, Action<T> action)
        {
            foreach (var v in a)
                action(v);
        }

        #endregion Methods
    }
}