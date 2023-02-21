using System;

namespace qs
{
    /// <summary>
    /// Return status object that encapsulates the status, exception, and any return object of an associated method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnStatus<T>
    {
        /// <summary>
        /// Constructor specifying the success/failure, and exception.
        /// Value set to the type default.
        /// </summary>
        /// <param name="isOk"></param>
        /// <param name="exception"></param>
        public ReturnStatus(bool isOk, Exception exception) : this(isOk, exception, default) { }

        /// <summary>
        /// Constructor specifying the success/failure, exception, and value.
        /// </summary>
        /// <param name="isOk"></param>
        /// <param name="exception"></param>
        /// <param name="value"></param>
        public ReturnStatus(bool isOk, Exception exception, T value)
        {
            IsOk = isOk;
            if (exception != null)
                Message = exception.Message;
            Exception = exception;
            Value = value;
        }

        /// <summary>
        /// Constructor specifying the success/failure only.
        /// Value set to the type default.
        /// Exception set to null.
        /// </summary>
        /// <param name="isOk"></param>
        public ReturnStatus(bool isOk) : this(isOk, null, default) { }

        /// <summary>
        /// Constructor specifying the value only.
        /// IsOk set to true.
        /// Exception set to null.
        /// </summary>
        /// <param name="value"></param>
        public ReturnStatus(T value) : this(true, null, value) { }

        /// <summary>
        /// Constructor specifying success/failure, and the value.
        /// Exception set to null.
        /// </summary>
        /// <param name="isOk"></param>
        /// <param name="value"></param>
        public ReturnStatus(bool isOk, T value) : this(isOk, null, value) { }

        /// <summary>
        /// Constructor specifying the exception only.
        /// IsOk set to false.
        /// Value is set to the type default.
        /// </summary>
        /// <param name="exception"></param>
        public ReturnStatus(Exception exception)
        {
            if (exception != null)
            {
                IsOk = false;
                Message = exception.Message;
                Value = default;
            }
            else
            {
                IsOk = true;
                Value = default;
            }
            Exception = exception;
        }

        /// <summary>
        /// Returns true if request was successful; otherwise false.
        /// </summary>
        public bool IsOk
        {
            get; private set;
        }

        /// <summary>
        /// Returns any exception message.
        /// </summary>
        public string Message
        {
            get; private set;
        }

        /// <summary>
        /// Returns any exception that was generated as a result of the request.
        /// </summary>
        public Exception Exception
        {
            get; private set;
        }

        /// <summary>
        /// Returns the return value that was acquired as a result of the request.
        /// NOTE: This is valid only if IsOk equals true.
        /// </summary>
        public T Value
        {
            get; private set;
        }
    }
}