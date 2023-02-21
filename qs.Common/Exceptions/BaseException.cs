namespace qs
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents errors that occur during application execution.
    /// </summary>
    [CLSCompliant(true)]
    [Serializable]
    public abstract class BaseException : Exception
    {
        #region Constructors

        public BaseException()
            : base()
        {
        }

        public BaseException(string message)
            : base(message)
        {
        }

        public BaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BaseException(string map, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, map, args))
        {
        }

        public BaseException(Exception exception, string map, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, map, args), exception)
        {
        }

        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors
    }
}