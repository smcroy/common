namespace qs.Extensions.DictionaryExtensions
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        #region Public Methods

        /// <summary>
        /// Try and get the specified key value pair from the indicated dictionary. Returns an
        /// instance of type Tuple&lt;bool, KeyValuePair&lt;K, V&gt;&gt; where Item1 indicates
        /// whether the key was found in the dictionary, and Item2 is the key value pair containing
        /// the specified key.
        /// <para>Example:</para>
        /// <para>Dictionary&lt;string, int&gt; d = new Dictionary&lt;string, int&gt;();</para>
        /// <para>d.Add( "A", 183);</para>
        /// <para>d.Add( "B", 259);</para>
        /// <para>
        /// var r = d.TryGetKeyValuePair( "C" ); // returns Tuple&lt;bool, int&gt; where r.Item1 is false
        /// </para>
        /// <para>
        /// var r = d.TryGetKeyValuePair( "B" ); // returns Tuple&lt;bool, int&gt; where r.Item1 is
        /// true and r.Item2 contains the key value pair associated with the key
        /// </para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Tuple<bool, KeyValuePair<K, V>> TryGetKeyValuePair<K, V>(this Dictionary<K, V> d, K key)
        {
            bool ok = d.TryGetValue(key, out V value);
            return new Tuple<bool, KeyValuePair<K, V>>(ok, ok ? new KeyValuePair<K, V>(key, value) : default);
        }

        /// <summary>
        /// Try and get the specified value from the indicated dictionary. Returns an instance of
        /// type Tuple&lt;bool, V&gt; where Item1 indicates whether the key was found in the
        /// dictionary, and Item2 is the value of the specified key.
        /// <para>Example:</para>
        /// <para>Dictionary&lt;string, int&gt; d = new Dictionary&lt;string, int&gt;();</para>
        /// <para>d.Add( "A", 183);</para>
        /// <para>d.Add( "B", 259);</para>
        /// <para>
        /// var r = d.TryGetValue( "C" ); // returns Tuple&lt;bool, int&gt; where r.Item1 is false
        /// </para>
        /// <para>
        /// var r = d.TryGetValue( "B" ); // returns Tuple&lt;bool, int&gt; where r.Item1 is true and
        /// r.Item2 contains 259
        /// </para>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="d"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Tuple<bool, V> TryGetValue<K, V>(this Dictionary<K, V> d, K key)
        {
            bool ok = d.TryGetValue(key, out V value);
            return new Tuple<bool, V>(ok, value);
        }

        #endregion Public Methods
    }
}