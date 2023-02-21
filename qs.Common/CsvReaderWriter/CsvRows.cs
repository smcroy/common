using System;
using System.Collections.Generic;

namespace qs
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class CsvRows : List<CsvRow>
    {
        /// <summary>
        ///
        /// </summary>
        public CsvRows()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        public void AddRow(CsvRow item)
        {
            this.Add(item);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ReturnStatus<CsvRow> GetRow(int index)
        {
            CsvRow value = null;
            Exception exc = null;
            bool ok = false;
            if (index >= 0 && this.Count > index)
            {
                value = this[index];
                ok = true;
            }
            else
            {
                exc = new Exception("Index specified outside of valid range of rows");
                ok = false;
            }
            return new ReturnStatus<CsvRow>(ok, exc, value);
        }
    }
}