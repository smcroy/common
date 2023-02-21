using System;
using System.Collections.Generic;
using System.Linq;

namespace qs
{
    /// <summary>
    /// This represents a comma separated values row.
    /// </summary>
    [Serializable]
    public class CsvRow
    {
        /// <summary>
        /// Default constructor to support general serialization.
        /// </summary>
        public CsvRow()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="strict">Indicates whether the column names are handled with case sensitivity. True case sensitive; otherwise case insensitive.</param>
        /// <param name="headers"></param>
        internal CsvRow(bool strict, string[] headers)
        {
            Strict = strict;
            Headers = headers;
        }

        /// <summary>
        /// Get the column indicated by the index to the column.
        /// <para>Exceptions:</para>
        /// <para>
        /// If the index specified is out of range, ie. less than 0, or greater than the last index of the headers, then a 'Column not found; index out of range' exception is returned as part of the ReturnStatus.
        /// </para>
        /// <para>
        /// If the index specified is within the range of the headers, but an unexpected error occurred where the column is blank, then a 'Column not found; name is blank' exception is returned as part of the ReturnStatus.
        /// </para>
        /// </summary>
        /// <param name="index">Specifies the index to the column.</param>
        /// <returns>Returns an instance of ReturnStatus of value type object indicating the success or failure of the request.</returns>
        public ReturnStatus<object> GetColumn(int index)
        {
            ReturnStatus<object> value = null;
            if (index >= 0 && index < Headers.Count())
            {
                string columnName = Headers[index];
                if (!string.IsNullOrEmpty(columnName))
                {
                    value = GetColumn(columnName, true);
                }
                else
                {
                    value = new ReturnStatus<object>(false, new Exception("Column not found; name is blank"), null);
                }
            }
            else
            {
                value = new ReturnStatus<object>(false, new Exception("Column not found; index out of range"), null);
            }
            return value;
        }

        /// <summary>
        /// Get the column indicated by the column name.
        /// <para>Exceptions:</para>
        /// </summary>
        /// <param name="columnName">Specifies the column name.</param>
        /// <returns>Returns an instance of ReturnStatus of value type object indicating the success or failure of the request.
        /// <code>
        /// ReturnStatus&lt;object&gt; rs = row.GetColumn( "column1" );
        /// object a = null;
        /// if (rs.IsOk)
        /// {
        ///     a = rs.Value;
        /// }
        /// </code>
        /// </returns>
        public ReturnStatus<object> GetColumn(string columnName)
        {
            return GetColumn(columnName, Strict);
        }

        /// <summary>
        /// Get the column headers utilized.
        /// </summary>
        public string[] Headers
        {
            get; private set;
        }

        /// <summary>
        /// Get whether a strict comparison of the column name is enforced. Strict comparision is case sensitive; otherwise the comparison is case insensitive. The default is false, the column name comparison is case insensitive.
        /// </summary>
        public bool Strict
        {
            get; private set;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        private ReturnStatus<object> GetColumn(string columnName, bool strict)
        {
            Exception exc = null;
            bool ok = false;
            object value = null;
            if (strict)
            {
                if (Headers.Contains(columnName))
                {
                    if (Columns.ContainsKey(columnName))
                        value = Columns[columnName];
                    else
                        value = null;
                    ok = true;
                }
                else
                {
                    exc = new Exception("Column not found; please verify the name casing");
                    ok = false;
                }
            }
            else
            {
                if (Headers.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                {
                    var l = Headers.Where(a => a.ToLower() == columnName.ToLower());
                    columnName = l.First();
                    if (Columns.ContainsKey(columnName))
                        value = Columns[columnName];
                    else
                        value = null;
                    ok = true;
                }
                else
                {
                    exc = new Exception("Column not found");
                    ok = false;
                }
            }
            return new ReturnStatus<object>(ok, exc, value);
        }

        /// <summary>
        /// Add a column to this instance of CsvRow specifying the index to the column.
        /// The index must indicate one of the header columns. The index to the header columns is 0 based.
        /// </summary>
        /// <param name="index">Specifies the index of the column name.</param>
        /// <param name="value">Specifies the value to associate with the column.</param>
        /// <returns>Returns an instance of ReturnStatus of value type of bool indicating the success or failure of the request.</returns>
        public ReturnStatus<bool> AddColumn(int index, object value)
        {
            ReturnStatus<bool> returnStatus = null;
            if (index >= 0 && index < Headers.Count())
            {
                string columnName = Headers[index];
                if (!string.IsNullOrEmpty(columnName))
                {
                    returnStatus = AddColumn(columnName, value, true);
                }
                else
                {
                    returnStatus = new ReturnStatus<bool>(false, new Exception("Column not found; name is blank"), false);
                }
            }
            else
            {
                returnStatus = new ReturnStatus<bool>(false, new Exception("Column not found; index out of range"), false);
            }
            return returnStatus;
        }

        /// <summary>
        /// Add a column to this instance of CsvRow specifying the column name.
        /// The column name must be one of the header columns specified to the instance of <see cref="CsvReader"/> or <see cref="CsvWriter"/>.
        /// </summary>
        /// <param name="columnName">Specifies the column name to which the value is associated.</param>
        /// <param name="value">Specifies the value to associate with the column.</param>
        /// <returns>Returns an instance of ReturnStatus of value type of bool indicating the success or failure of the of the request.</returns>
        public ReturnStatus<bool> AddColumn(string columnName, object value)
        {
            return AddColumn(columnName, value, Strict);
        }

        /// <summary>
        /// Add a column to this instance of CsvRow specifying the column name.
        /// The column name must be one of the header columns specified to the instance of <see cref="CsvReader"/> or <see cref="CsvWriter"/>.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        private ReturnStatus<bool> AddColumn(string columnName, object value, bool strict)
        {
            Exception exc = null;
            bool ok = false;
            string cName = string.Empty;
            if (strict)
            {
                if (Headers.Contains(columnName))
                {
                    cName = columnName;
                }
                else
                {
                    exc = new Exception("Column not found; please verify the name casing");
                    ok = false;
                }
            }
            else
            {
                if (Headers.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                {
                    var l = Headers.Where(a => a.ToLower() == columnName.ToLower());
                    cName = l.First();
                }
                else
                {
                    exc = new Exception("Column not found");
                    ok = false;
                }
            }
            if (!string.IsNullOrEmpty(cName))
            {
                if (Columns.ContainsKey(cName))
                {
                    exc = new Exception("Column already written to this row");
                    ok = false;
                }
                else
                {
                    Columns.Add(cName, value);
                    ok = true;
                }
            }
            return new ReturnStatus<bool>(ok, exc, ok);
        }

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, object> _Columns;

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, object> Columns
        {
            get
            {
                if (_Columns == null)
                    _Columns = new Dictionary<string, object>();
                return _Columns;
            }
            set
            {
                _Columns = value;
            }
        }
    }
}