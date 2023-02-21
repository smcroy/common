namespace qs.Extensions.ReflectionExtensions
{
    using qs.Extensions.StringExtensions;
    using System;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        #region Methods

        /// <summary>
        /// Convert the string value to the indicated reflected value type.
        /// </summary>
        /// <param name="a">Instance of type System.String whose value is converted.</param>
        /// <param name="type">Type declaration of the returned value.</param>
        /// <returns>Returns the converted String value.</returns>
        public static object ConvertToPropertyType(this PropertyInfo a, string value)
        {
            if (a == null)
                return null;
            Type type = a.PropertyType;
            if (type == typeof(String))
                return value.ToString();
            if (type == typeof(Boolean))
                return value.ToBoolean();
            if (type == typeof(DateTime))
                return value.ToDateTime();
            if (type == typeof(Decimal))
                return value.ToDecimal();
            if (type == typeof(Double))
                return value.ToDouble();
            if (type == typeof(float))
                return value.ToFloat();
            if (type == typeof(Int16))
                return value.ToInt16();
            if (type == typeof(Int32))
                return value.ToInt32();
            if (type == typeof(Int64))
                return value.ToInt64();
            if (type == typeof(Nullable<Boolean>))
                return value.ToNullableBoolean();
            if (type == typeof(Nullable<DateTime>))
                return value.ToNullableDateTime();
            if (type == typeof(Nullable<Decimal>))
                return value.ToNullableDecimal();
            if (type == typeof(Nullable<Double>))
                return value.ToNullableDouble();
            if (type == typeof(Nullable<float>))
                return value.ToNullableFloat();
            if (type == typeof(Nullable<Int16>))
                return value.ToNullableInt16();
            if (type == typeof(Nullable<Int32>))
                return value.ToNullableInt32();
            if (type == typeof(Nullable<Int64>))
                return value.ToNullableInt64();
            return (object)value;
        }

        /// <summary>
        /// Get the attributes and metadata of the indicated parameter from the specified method.
        /// </summary>
        /// <param name="currentMethod">A MethodBase object representing the current executing method from which the parameter information is derived.</param>
        /// <param name="signaturePositionForTheParameter">Signature position of the parameter.</param>
        /// <returns>The attributes and metadata of the indicated parameter as an instance of ParameterInfo.</returns>
        public static ParameterInfo GetParameterInfo(this MethodBase currentMethod, int signaturePositionForTheParameter)
        {
            if (currentMethod == null)
                return null;
            MethodBase method = currentMethod as MethodBase;
            if (method == null)
                return null;

            ParameterInfo[] parameters = currentMethod.GetParameters();
            if (signaturePositionForTheParameter < parameters.Length && signaturePositionForTheParameter >= 0)
                return parameters[signaturePositionForTheParameter];
            else if (signaturePositionForTheParameter >= parameters.Length && parameters.Length > 0)
                return parameters[parameters.Length - 1];
            else if (signaturePositionForTheParameter < 0 && parameters.Length > 0)
                return parameters[0];
            else
                return null;
        }

        #endregion Methods
    }
}