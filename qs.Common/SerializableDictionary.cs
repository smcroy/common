﻿namespace qs.common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable()]
    public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
    {
        #region Fields

        private const string DictionaryNodeName = "Dictionary";
        private const string ItemNodeName = "Item";
        private const string KeyNodeName = "Key";
        private const string ValueNodeName = "Value";

        private XmlSerializer keySerializer = null;
        private XmlSerializer valueSerializer = null;

        #endregion Fields

        #region Constructors

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TVal> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
        {
            int itemCount = info.GetInt32("ItemCount");
            for (int i = 0; i < itemCount; i++)
            {
                KeyValuePair<TKey, TVal> kvp = (KeyValuePair<TKey, TVal>)info.GetValue(String.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
                this.Add(kvp.Key, kvp.Value);
            }
        }

        #endregion Constructors

        #region Properties

        protected XmlSerializer ValueSerializer
        {
            get
            {
                if (valueSerializer == null)
                {
                    valueSerializer = new XmlSerializer(typeof(TVal));
                }
                return valueSerializer;
            }
        }

        private XmlSerializer KeySerializer
        {
            get
            {
                if (keySerializer == null)
                {
                    keySerializer = new XmlSerializer(typeof(TKey));
                }
                return keySerializer;
            }
        }

        #endregion Properties

        #region Methods

        public void Add(KeyValuePair<TKey, TVal> value)
        {
            this.Add(value.Key, value.Value);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemCount", this.Count);
            int itemIdx = 0;
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                info.AddValue(String.Format("Item{0}", itemIdx), kvp, typeof(KeyValuePair<TKey, TVal>));
                itemIdx++;
            }
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            // Move past container
            if (!reader.Read())
            {
                throw new XmlException("Error in Deserialization of Dictionary");
            }

            //reader.ReadStartElement(DictionaryNodeName);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement(ItemNodeName);
                reader.ReadStartElement(KeyNodeName);
                TKey key = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement(ValueNodeName);
                TVal value = (TVal)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.Add(key, value);
                reader.MoveToContent();
            }
            //reader.ReadEndElement();

            reader.ReadEndElement(); // Read End Element to close Read of containing node
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            //writer.WriteStartElement(DictionaryNodeName);
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                writer.WriteStartElement(ItemNodeName);
                writer.WriteStartElement(KeyNodeName);
                KeySerializer.Serialize(writer, kvp.Key);
                writer.WriteEndElement();
                writer.WriteStartElement(ValueNodeName);
                ValueSerializer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            //writer.WriteEndElement();
        }

        #endregion Methods
    }
}