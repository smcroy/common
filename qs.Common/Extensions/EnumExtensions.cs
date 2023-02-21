namespace qs.Extensions.EnumExtensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public static class EnumExtensions
    {
        #region Methods

        /// <summary>
        /// Get the DefaultValueAttribute value of the enumeration.
        /// Specify the DefaultValueAttribute:
        /// using System.ComponentModel;
        ///
        /// [DefaultValue(MyEnum.Spanish)]
        /// public enum MyEnum
        /// {
        ///     [Description("en-US")]
        ///     English,
        ///     [Description("en-ES")]
        ///     Spanish
        /// }
        /// Enum en = MyEnum.English.GetDefaultValue();
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static Enum GetDefaultValue(this Enum enumeration)
        {
            DefaultValueAttribute[] attributes = (DefaultValueAttribute[])enumeration.GetType().GetCustomAttributes(typeof(DefaultValueAttribute), false);
            return attributes.Length > 0 ? (Enum)attributes[0].Value : enumeration;
        }

        /// <summary>
        /// Get the DescriptionAttribute value of the specified enumeration.
        /// Specify the DescriptionAttribute:
        /// using System.ComponentModel;
        ///
        /// [DefaultValue(MyEnum.Spanish)]
        /// public enum MyEnum
        /// {
        ///     [Description("en-US")]
        ///     English,
        ///     [Description("en-ES")]
        ///     Spanish
        /// }
        /// string s = MyEnum.English.GetDescription();
        /// </summary>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumeration)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])enumeration.GetType().GetField(enumeration.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static string GetName<T>(this Enum value)
        {
            return Enum.GetName(typeof(T), value);
        }

        public static IEnumerable<string> GetNames<T>(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Enum.GetNames(typeof(T));
        }

        public static IEnumerable<T> GetValues<T>(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            foreach (object i in Enum.GetValues(typeof(T)))
                yield return (T)i;
        }

        #endregion Methods
    }
}