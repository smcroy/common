namespace qs
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;

    [CLSCompliant(true)]
    [Serializable]
    public class ArgumentNotOfTypeException : BaseException
    {
        #region Constructors

        public ArgumentNotOfTypeException(ParameterInfo parameterInfo, Exception innerException)
            : base(getMessage(parameterInfo), innerException)
        {
        }

        public ArgumentNotOfTypeException(ParameterInfo parameterInfo)
            : base(getMessage(parameterInfo))
        {
        }

        protected ArgumentNotOfTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion Constructors

        #region Methods

        private static string getMessage(ParameterInfo parameterInfo)
        {
            const string MSG = "Argument {1} is not of type {0}.";
            ParameterInfo pi = parameterInfo as ParameterInfo;
            if (pi == null)
                return MSG;
            else
                return string.Format(CultureInfo.InvariantCulture, MSG, parameterInfo.ParameterType.ToString(), parameterInfo.Name);
        }

        #endregion Methods
    }
}