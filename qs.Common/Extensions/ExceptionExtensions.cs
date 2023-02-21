namespace qs.Extensions.ExceptionExtensions
{
    using System;

    public static class ExceptionExtensions
    {
        #region Methods

        /// <summary>
        /// This documents that the error condition is ignored.
        /// </summary>
        /// <param name="exception"></param>
        public static string IgnoreError(this Exception exception)
        {
            const string Notice = "This documents that the error condition is ignored.";
            return Notice;
        }

        #endregion Methods
    }
}