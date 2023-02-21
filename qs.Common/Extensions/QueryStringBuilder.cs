namespace qs
{
    using qs.Extensions.StringExtensions;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a custom constructor for building a query string. The following example utilizes an instance of QueryStringBuilder and UriBuilder to create a fully qualified Url.
    /// <para>
    /// Usage:
    /// </para>
    /// <para>
    /// QueryStringBuilder qsb = new QueryStringBuilder( );
    /// </para>
    /// qsb.Add( "key1", "value1" );
    /// <para>
    /// qsb.Add( "key2", "value2" );
    /// </para>
    /// string qsbs = qsb.ToString( ); // formatted and Url encode query string; value of qsbs is key1=value1&key2=value2
    /// <para>
    /// UriBuilder ub = new UriBuilder( );
    /// </para>
    /// ub.Query = qsbs;
    /// <para>
    /// // Note: Set host, scheme, path, etc.
    /// </para>
    /// string ubs = ub.ToString( ); // formatted Url
    /// </summary>
    public class QueryStringBuilder : NameValueCollection
    {
        #region Methods

        /// <summary>
        /// Adds an entry with the specified name and value to this instance. If the name already exists, it is removed prior to adding the new entry.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void Add(string name, string value)
        {
            Add(name, value, true);
        }

        /// <summary>
        /// Adds an entry with the specified name and value to this instance. If unique is specified as true and if the name already exists, it is removed prior to adding the new entry. If unique is specified as false, then the value is appended to any existing value separated from the existing value with a comma.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="unique"></param>
        public void Add(string name, string value, bool unique)
        {
            if (unique && AllKeys.Contains(name))
                Remove(name);

            base.Add(name, value);
        }

        /// <summary>
        /// Converts the value of this instance to a Url encoded System.String representing a query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<string> keys = AllKeys.Cast<string>().ToList();
            keys.Sort();
            foreach (string key in keys)
                sb.AppendFormat("&{0}={1}", key, Get(key).Trim().UrlEncode());
            string s = sb.ToString().TrimStart('&');
            return s;
        }

        #endregion Methods
    }
}