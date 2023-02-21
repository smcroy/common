namespace qs.Extensions.NameValueCollectionExtensions
{
    using qs.Extensions.StringExtensions;
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    public static class NameValueCollectionExtensions
    {
        #region Methods

        /// <summary>
        /// Returns true if the NameValueCollection contains the specified key; otherwise false.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(this NameValueCollection a, string key)
        {
            return a.AllKeys.Contains(key);
        }

        public static bool GetBoolean(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? false : s.ToBoolean();
            }
            else
                return false;
        }

        /// <summary>
        /// Get the specified item as a nullable DateTime from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime? GetDateTime(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? null : s.ToNullableDateTime();
            }
            else
                return null;
        }

        /// <summary>
        /// Get the specified item as a nullable Guid from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid? GetGuid(this NameValueCollection a, string key)
        {
            try
            {
                if (a.AllKeys.Contains(key))
                {
                    string s = a.Get(key);
                    return string.IsNullOrEmpty(s) ? default(Guid?) : new Guid(s);
                }
                else
                    return default;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get the specified item as a nullable Int16 from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static short? GetInt16(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? null : s.ToNullableInt16();
            }
            else
                return null;
        }

        /// <summary>
        /// Get the specified item as a nullable Int32 from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int? GetInt32(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? null : s.ToNullableInt32();
            }
            else
                return null;
        }

        /// <summary>
        /// Get the specified item as a nullable Int64 from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long? GetInt64(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? null : s.ToNullableInt64();
            }
            else
                return null;
        }

        /// <summary>
        ///  Get the specified item as a string from the NameValueCollection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(this NameValueCollection a, string key)
        {
            if (a.AllKeys.Contains(key))
            {
                string s = a.Get(key);
                return string.IsNullOrEmpty(s) ? string.Empty : s;
            }
            else
                return string.Empty;
        }

        #endregion Methods
    }
}