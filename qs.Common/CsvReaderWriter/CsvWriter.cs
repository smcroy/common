using System;
using System.IO;
using System.Text;

namespace qs
{
    /// <summary>
    /// This is a comma separated values file writer. It exports the rows and columns added to the writer as a CSV file.
    /// <code>
    /// // Strict = false
    /// CsvWriter writer = new CsvWriter( new string[ ] { "Column1", "Column2", "Column3" } );
    /// CsvRow row = writer.AddRow( );
    /// ReturnStatus&lt;bool&gt; rs = row.AddColumn( 0, 145678 );
    /// rs = row.AddColumn( "column2", "This is a widget" );
    /// row = writer.AddRow( );
    /// rs = row.AddColumn( "Column1", 15.21 );
    /// rs = row.AddColumn( "column2", "This is another widget" );
    /// ReturnStatus&lt;int&gt; rsExport = writer.Export( @"c:\folder\file.csv" );
    /// </code>
    /// </summary>
    public class CsvWriter
    {
        /// <summary>
        /// Create a new instance of type CsvWriter. The column headers are specified. The column names are handled with case insensitivity.
        /// </summary>
        /// <param name="headers">Specifies the column headers to utilize for the export file.</param>
        public CsvWriter(string[] headers) : this(false, headers)
        {
        }

        /// <summary>
        /// Create a new instance of type CsvWriter. The column headers are specified. How the column names are handled is specified.
        /// </summary>
        /// <param name="strict">Indicates whether the column names are handled with case sensitivity. True case sensitive; otherwise case insensitive.</param>
        /// <param name="headers">Specifies the column headers to utilize for the export file.</param>
        public CsvWriter(bool strict, string[] headers)
        {
            Strict = strict;
            Headers = headers;
        }

        /// <summary>
        /// Export the file as a comma separated values file.
        /// Returns an instance of ReturnStatus with a value of int indicating the number of records (or rows) exported. This number does not include the header row.
        /// </summary>
        /// <param name="path">Specifies the fully qualified path and file name to the target file.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of int indicating the number of records (or rows) exported. This number does not include the header row.</returns>
        public ReturnStatus<int> Export(string path)
        {
            return Export(path, DefaultDelimiter);
        }

        /// <summary>
        /// Export the file as a comma separated values file.
        /// Returns an instance of ReturnStatus with a value of int indicating the number of records (or rows) exported. This number does not include the header row.
        /// </summary>
        /// <param name="path">Specifies the fully qualified path and file name to the target file.</param>
        /// <param name="delimiter">Specifies the column delimiter to utilize.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of int indicating the number of records (or rows) exported. This number does not include the header row.</returns>
        public ReturnStatus<int> Export(string path, char delimiter)
        {
            int i = 0;
            Exception exc = null;
            bool ok = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                char d = ' ';
                foreach (string h in Headers)
                {
                    sb.AppendFormat("{2}{0}{1}{0}", QuotationCharacter, h, d);
                    d = delimiter;
                }
                sb.Append(Environment.NewLine);
                foreach (CsvRow row in Rows)
                {
                    d = ' ';
                    foreach (string h in Headers)
                    {
                        ReturnStatus<object> o = row.GetColumn(h);
                        sb.AppendFormat("{2}{0}{1}{0}", QuotationCharacter, o.Value == null ? string.Empty : EscapeEmbeddedQuotes(o.Value), d);
                        d = delimiter;
                    }
                    sb.Append(Environment.NewLine);
                }
                string[] lines = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                sb = null;
                i = -1;
                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line.Trim());
                        i++;
                    }
                }
                ok = true;
            }
            catch (Exception e)
            {
                exc = e;
                ok = false;
            }
            return new ReturnStatus<int>(ok, exc, i);
        }

        /// <summary>
        /// Adds a new row to the collection of rows and returns the instance of type CsvRow to which columns can be added.
        /// </summary>
        /// <returns>Returns an instance of type CsvRow.</returns>
        public CsvRow AddRow()
        {
            CsvRow row = new CsvRow(Strict, Headers);
            Rows.AddRow(row);
            return row;
        }

        /// <summary>
        /// Get the column headers utilized.
        /// The column headers are specified by the constructor.
        /// </summary>
        public string[] Headers
        {
            get; private set;
        }

        /// <summary>
        /// Get whether a strict comparison of the column name is enforced.
        /// Strict comparision is case sensitive; otherwise the comparison is case insensitive.
        /// The default is false, the column name comparison is case insensitive.
        /// </summary>
        public bool Strict
        {
            get; private set;
        }

        /// <summary>
        /// Specifies the default column delimiter character.
        /// </summary>
        public const char DefaultDelimiter = ',';

        /// <summary>
        /// Backing variable of Rows.
        /// </summary>
        private CsvRows _Rows;

        /// <summary>
        /// Get or set the records (or rows).
        /// </summary>
        private CsvRows Rows
        {
            get
            {
                if (_Rows == null)
                    _Rows = new CsvRows();
                return _Rows;
            }
            set
            {
                _Rows = value;
            }
        }

        /// <summary>
        /// Character assumed to be the column value quotation character if a quotation character is utilized within the source file.
        /// </summary>
        private const char QuotationCharacter = '"';

        /// <summary>
        /// Escape embedded quotes.
        /// </summary>
        /// <param name="value">Specifies the value to escape embedded quotes.</param>
        /// <returns>Returns the value with embedded quotes escaped.</returns>
        private string EscapeEmbeddedQuotes(object value)
        {
            return value.ToString().Replace(QUOTE, ESCAPED_QUOTE);
        }

        /// <summary>
        /// Escaped quote indicator.
        /// </summary>
        private string ESCAPED_QUOTE = "\"\"";

        /// <summary>
        /// Quote character.
        /// </summary>
        private string QUOTE = "\"";
    }
}