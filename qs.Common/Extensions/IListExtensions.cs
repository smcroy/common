namespace qs.Extensions.IListExtensions
{
    using qs.Extensions.ByteExtensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class IListExtensions
    {
        #region Private Fields

        private static Random r = new Random();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Returns a value indicating whether the specified System.String object occurs with this
        /// instance of type IList. Returns true if the value parameter occurs within this instance
        /// of IList; otherwise false.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="comparisonType"></param>
        /// <returns>
        /// true if the value parameter occurs within this instance of IList; otherwise false
        /// </returns>
        public static bool Contains(this IList<string> list, string item, StringComparison comparisonType)
        {
            return list.Where(a => string.Compare(a, item, comparisonType) == 0).Any();
        }

        /// <summary>
        /// This deals a random item from the specified list removing it from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Deal<T>(this List<T> list)
        {
            if (list.Count <= 0)
                return default(T);
            var i = r.Next(0, list.Count);
            var o = list[i];
            list.RemoveAt(i);
            return o;
        }

        /// <summary>
        /// Return the mean of the list of Int64. A mean is the sum of the observations divided by
        /// the number of observations. It is the average.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMean(this IList<long> list)
        {
            int c = list.Count;
            double t = 0;
            foreach (long i in list)
                t = t + i;
            return c > 0 ? t / c : 0;
        }

        /// <summary>
        /// Return the mean of the list of integers. A mean is the sum of the observations divided by
        /// the number of observations. It is the average.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMean(this IList<int> list)
        {
            int c = list.Count;
            double t = 0;
            foreach (int i in list)
                t = t + i;
            return c > 0 ? t / c : 0;
        }

        /// <summary>
        /// Return the mean of the list of doubles. A mean is the sum of the observations divided by
        /// the number of observations. It is the average.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMean(this IList<double> list)
        {
            int c = list.Count;
            double t = 0;
            foreach (double d in list)
                t = t + d;
            return c > 0 ? t / c : 0;
        }

        /// <summary>
        /// Return the median of the list of integers. A median is described as the numeric value
        /// separating the higher half of a sample, a population, or a probability distribution, from
        /// the lower half. The median of a finite list of numbers can be found by arranging all the
        /// observations from lowest value to highest value and picking the middle one. If there is
        /// an even number of observations, then there is no single middle value, so one often takes
        /// the mean of the two middle values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMedian(this IList<int> list)
        {
            double median = 0d;
            var l = Sort(list);
            int c = l.Count;
            if (c > 0)
            {
                if ((c % 2) == 0)
                {
                    int i = (c / 2) - 1;
                    median = ((double)(l[i] + l[i + 1])) / 2;
                }
                else if (c > 1)
                {
                    int i = ((c - 1) / 2);
                    median = l[i];
                }
            }
            return median;
        }

        /// <summary>
        /// Return the median of the list of Int64. A median is described as the numeric value
        /// separating the higher half of a sample, a population, or a probability distribution, from
        /// the lower half. The median of a finite list of numbers can be found by arranging all the
        /// observations from lowest value to highest value and picking the middle one. If there is
        /// an even number of observations, then there is no single middle value, so one often takes
        /// the mean of the two middle values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMedian(this IList<long> list)
        {
            double median = 0d;
            var l = Sort(list);
            int c = l.Count;
            if (c > 0)
            {
                if ((c % 2) == 0)
                {
                    int i = (c / 2) - 1;
                    median = ((double)(l[i] + l[i + 1])) / 2;
                }
                else if (c > 1)
                {
                    int i = ((c - 1) / 2);
                    median = l[i];
                }
            }
            return median;
        }

        /// <summary>
        /// Return the median of the list of doubles. A median is described as the numeric value
        /// separating the higher half of a sample, a population, or a probability distribution, from
        /// the lower half. The median of a finite list of numbers can be found by arranging all the
        /// observations from lowest value to highest value and picking the middle one. If there is
        /// an even number of observations, then there is no single middle value, so one often takes
        /// the mean of the two middle values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMedian(this IList<double> list)
        {
            double median = 0d;
            var l = Sort(list);
            int c = l.Count;
            if (c > 0)
            {
                if ((c % 2) == 0)
                {
                    int i = (c / 2) - 1;
                    median = (l[i] + l[i + 1]) / 2;
                }
                else if (c > 1)
                {
                    int i = ((c - 1) / 2);
                    median = l[i];
                }
            }
            return median;
        }

        /// <summary>
        /// Return the mode of the list of integers. The mode is the value that occurs the most
        /// frequently in a data set or a probability distribution.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetMode(this IList<int> list)
        {
            int mode = 0;
            int value = 0;

            if (list.Count > 1)
            {
                var l = Sort(list);
                Dictionary<int, int> d = new Dictionary<int, int>();
                foreach (int i in l)
                {
                    if (!d.ContainsKey(i))
                        d.Add(i, 0);
                    d[i]++;
                }
                foreach (var i in d)
                    if (i.Value > value)
                    {
                        value = i.Value;
                        mode = i.Key;
                    }
            }
            return mode;
        }

        /// <summary>
        /// Return the mode of the list of Int64. The mode is the value that occurs the most
        /// frequently in a data set or a probability distribution.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static long GetMode(this IList<long> list)
        {
            long mode = 0;
            int value = 0;

            if (list.Count > 1)
            {
                var l = Sort(list);
                Dictionary<long, int> d = new Dictionary<long, int>();
                foreach (int i in l)
                {
                    if (!d.ContainsKey(i))
                        d.Add(i, 0);
                    d[i]++;
                }
                foreach (var i in d)
                    if (i.Value > value)
                    {
                        value = i.Value;
                        mode = i.Key;
                    }
            }
            return mode;
        }

        /// <summary>
        /// Return the mode of the list of doubles. The mode is the value that occurs the most
        /// frequently in a data set or a probability distribution.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetMode(this IList<double> list)
        {
            double mode = 0;
            int value = 0;

            if (list.Count > 1)
            {
                var l = Sort(list);
                Dictionary<double, int> d = new Dictionary<double, int>();
                foreach (double i in l)
                {
                    if (!d.ContainsKey(i))
                        d.Add(i, 0);
                    d[i]++;
                }
                foreach (var i in d)
                    if (i.Value > value)
                    {
                        value = i.Value;
                        mode = i.Key;
                    }
            }
            return mode;
        }

        /// <summary>
        /// This returns a random item from the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count <= 0)
                return default(T);
            return list[r.Next(0, list.Count)];
        }

        /// <summary>
        /// Return the range of the list of integers. The range is the difference between the largest
        /// and smallest values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetRange(this IList<int> list)
        {
            if (list.Count > 1)
            {
                var l = Sort(list);
                return l[l.Count - 1] - l[0];
            }
            else
                return 0;
        }

        /// <summary>
        /// Return the range of the list of Int64. The range is the difference between the largest
        /// and smallest values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static long GetRange(this IList<long> list)
        {
            if (list.Count > 1)
            {
                var l = Sort(list);
                return l[l.Count - 1] - l[0];
            }
            else
                return 0;
        }

        /// <summary>
        /// Return the range of the list of doubles. The range is the difference between the largest
        /// and smallest values.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetRange(this IList<double> list)
        {
            if (list.Count > 1)
            {
                var l = Sort(list);
                return l[l.Count - 1] - l[0];
            }
            else
                return 0;
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified string in this
        /// instance. It return the zero-base index position of the value if that string is found;
        /// otherwise -1. If the value is System.String.Empty, the return value is -1.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static int IndexOf(this IList<string> list, string item, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(item))
                return -1;
            return list.ToList().FindIndex(a => string.Compare(a, item, comparisonType) == 0);
        }

        /// <summary>
        /// Shrinks the specified list by removing null items from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IList<T> ShrinkNullEntries<T>(this IList<T> list)
        {
            if (list.Count == 1 && list[0] == null)
                list.Clear();
            else if (list.Count > 0)
            {
                List<int> l = new List<int>();
                int i = 0;
                foreach (T t in list)
                {
                    if (t == null)
                        l.Add(i);
                    i++;
                }
                if (l.Count > 0)
                    for (int j = l.Count - 1; j >= 0; j--)
                        list.RemoveAt(l[j]);
            }
            return list;
        }

        /// <summary>
        /// Shuffle the specified list. This is an in-place shuffle implementation of the
        /// Fisher-Yates shuffle based upon Durstenfeld's algorithm. Given the preinitialilzed list,
        /// it shuffles the elements in-place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this List<T> list)
        {
            if (list.Count <= 1)
                return;
            Random r = new Random();
            int i = list.Count;
            while (i > 1)
            {
                int j = r.Next(i);
                i--;
                T x = list[i];
                list[i] = list[j];
                list[j] = x;
            }
        }

        /// <summary>
        /// Returns the specified list as a table of in-memory data. The property names are utilized
        /// as the table column names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            string n = typeof(T).Name;
            return data.ToDataTable<T>(n);
        }

        /// <summary>
        /// Returns the specified list as a named table of in-memory data. The property names are
        /// utilized as the table column names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data, string tableName)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            using (DataTable table = new DataTable(tableName))
            {
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
                return table;
            }
        }

        /// <summary>
        /// Returns the specified list as a tab separated values string with each row delimited by a
        /// new line character. The property names are utilized as the column headers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToTsv<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                sb.Append(prop.Name);
                if (i < props.Count - 1)
                {
                    sb.Append("\t");
                }
            }
            sb.Append(Environment.NewLine);
            var values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                    sb.Append(values[i].ToString());
                    if (i < values.Length - 1)
                    {
                        sb.Append("\t");
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the specified list as a string of XML. The property names are utilized as the
        /// element names. The name of the type of the instance is utilized as the node name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static string ToXml<T>(this IList<T> data, string rootName)
        {
            return data.ToXml(rootName, XmlWriteMode.IgnoreSchema);
        }

        /// <summary>
        /// Returns the specified list as a string of XML. The property names are utilized as the
        /// element names. The name of the type of the instance is utilized as the node name. Writes
        /// the instance of IList data as XML data with the relational structure as inline XSD
        /// schema. If the instance of IList has no data, only the inline schema is written. If the
        /// instance of IList does not have a current schema, nothing is written.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static string ToXmlWithSchema<T>(this IList<T> data, string rootName)
        {
            return data.ToXml(rootName, XmlWriteMode.WriteSchema);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="logfile"></param>
        /// <param name="delimiter"></param>
        public static void WriteDelimitedFile<T>(this IEnumerable<T> list, string logfile, string delimiter)
        {
            using (var sw = File.AppendText(logfile))
            {
                var props = typeof(T).GetProperties();
                var headerNames = props.Select(x => x.Name);
                sw.WriteLine(string.Join(delimiter, headerNames.ToArray()));
                foreach (var item in list)
                {
                    var item1 = item;
                    var values = props
                    .Select(x => x.GetValue(item1, null) ?? "")
                    .Select(x => x.ToString())
                    .Select(x => x.Contains(delimiter) || x.Contains("\n") ? "\"" + x + "\"" : x);
                    sw.WriteLine(string.Join(delimiter, values.ToArray()));
                }
                sw.Close();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static IList<T> Sort<T>(IList<T> list)
        {
            T[] a = new T[list.Count];
            list.CopyTo(a, 0);
            List<T> l = a.ToList();
            l.Sort();
            return l;
        }

        private static string ToXml<T>(this IList<T> data, string rootName, XmlWriteMode mode)
        {
            DataTable table = data.ToDataTable<T>();
            DataSet dataSet = new DataSet(rootName);
            dataSet.Tables.Add(table);
            MemoryStream stream = new MemoryStream();
            dataSet.WriteXml(stream, mode);
            return stream.ToArray().GetString();
        }

        #endregion Private Methods
    }
}