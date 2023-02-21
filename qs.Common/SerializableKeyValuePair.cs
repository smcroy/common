namespace qs.common
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SerializableKeyValuePair<TKey, TVal>
    {
        #region Constructors

        public SerializableKeyValuePair(KeyValuePair<TKey, TVal> keyValue)
        {
            Key = keyValue.Key;
            Value = keyValue.Value;
        }

        public SerializableKeyValuePair()
        {
        }

        public SerializableKeyValuePair(TKey key, TVal value)
        {
            Key = key;
            Value = value;
        }

        #endregion Constructors

        #region Properties

        public TKey Key
        {
            get;
            set;
        }

        public TVal Value
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public static SerializableKeyValuePair<TKey, TVal> Instance(TKey key, TVal value)
        {
            return new SerializableKeyValuePair<TKey, TVal>(key, value);
        }

        public static SerializableKeyValuePair<TKey, TVal> Instance(KeyValuePair<TKey, TVal> keyValue)
        {
            return new SerializableKeyValuePair<TKey, TVal>(keyValue);
        }

        public KeyValuePair<TKey, TVal> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TVal>(Key, Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion Methods
    }
}