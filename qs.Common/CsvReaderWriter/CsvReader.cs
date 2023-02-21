using System;
using System.Collections.Generic;
using System.IO;

namespace qs
{
    /// <summary>
    /// This is a comma separated values file reader. It imports a CSV file and presents the rows as a list of type CsvRow and the columns as Dictionary of KeyValue pairs associated with the CsvRow.
    /// <code>
    /// // Strict = false
    /// // Headers taken from first row of source
    /// CsvReader reader = new CsvReader( );
    /// ReturnStatus&lt;CsvRows&gt; rows = reader.Import( @"c:\folder\file.csv" );
    /// ReturnStatus&lt;object&gt; a = rows.Value[0].GetColumn( "ApplicantNo" ); // get value by column name
    /// ReturnStatus&lt;object&gt; c = rows.Value[0].GetColumn( "APPLICANTNO" ); // get value by column name
    /// // because Strict = false, a.Value == c.Value
    /// ReturnStatus&lt;object&gt; b = rows.Value[0].GetColumn( 2 ); // get value by column index
    /// // if ApplicantNo is the 3rd column, then a.Value == b.Value == c.Value
    ///
    /// // Strict = false
    /// // Headers specified to the reader; no header row in the source.
    /// CsvReader reader = new CsvReader( new string[] { "Column1", "Column2", "Column3" });
    /// ReturnStatus&lt;CsvRows&gt; rows = reader.Import( @"c:\folder\file.csv" );
    /// ReturnStatus&lt;object&gt; a = rows.Value[0].GetColumn( "Column3" ); // get value by column name
    /// ReturnStatus&lt;object&gt; c = rows.Value[0].GetColumn( "COLUMN3" ); // get value by column name
    /// // because Strict = false, a.Value == c.Value
    /// ReturnStatus&lt;object&gt; b = rows.Value[0].GetColumn( 2 ); // get value by column index
    /// // if Column3 is the 3rd column, then a.Value == b.Value == c.Value
    /// </code>
    /// <para>CSV Comma Separated Values file</para>
    /// A comma-separated values (CSV) file is a delimited text file that uses a comma to separate values. A CSV file stores tabular data (numbers and text) in plain text. Each line of the file is a data record (or row). Each record consists of one or more fields, separated by commas. The use of the comma as a field separator is the source of the name for this file format. A carriage return/line feed combination delimits the data records (or rows).
    /// </summary>
    public class CsvReader
    {
        /// <summary>
        /// Create a new instance of type CsvReader. The column headers are found in the first line of the source file. The column names are handled with case insensitivity.
        /// </summary>
        public CsvReader() : this(false, null)
        {
        }

        /// <summary>
        /// Create a new instance of type CsvReader. The column headers are found in the first line of the source file. How the column names are handled is specified.
        /// </summary>
        /// <param name="strict">Indicates whether the column names are handled with case sensitivity. True case sensitive; otherwise case insensitive.</param>
        public CsvReader(bool strict) : this(strict, null)
        {
        }

        /// <summary>
        /// Create a new instance of type CsvReader. The column headers are specified. The first line of the source file is considered a data line, not a column line. The column names are handled with case insensitivity.
        /// </summary>
        /// <param name="headers">Specifies the column headers to utilize for the file. When specified, no headers are assumed within the imported file.</param>
        public CsvReader(string[] headers) : this(false, headers)
        {
        }

        /// <summary>
        /// Create a new instance of type CsvReader. The column headers are specified. The first line of the source file is considered a data line, not a column line. How the column names are handled is specified.
        /// </summary>
        /// <param name="strict">Indicates whether the column names are handled with case sensitivity. True case sensitive; otherwise case insensitive.</param>
        /// <param name="headers">Specifies the column headers to utilize for the file. When specified, no headers are assumed within the imported file.</param>
        public CsvReader(bool strict, string[] headers)
        {
            Strict = strict;
            Headers = headers;
        }

        /// <summary>
        /// Import the file as a comma separated values file.
        /// Returns an instance of ReturnStatus with a value type of CsvRows.
        /// </summary>
        /// <param name="path">Specifies the fully qualified path and file name to the source file.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of CsvRows.</returns>
        public ReturnStatus<CsvRows> Import(string path)
        {
            return Import(path, DefaultDelimiter);
        }

        /// <summary>
        /// Import the file as a comma separated values file.
        /// Returns an instance of ReturnStatus with a value type of CsvRows.
        /// </summary>
        /// <param name="path">Specifies the fully qualified path and file name to the source file.</param>
        /// <param name="delimiter">Specifies the column delimiter to utilize.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of CsvRows.</returns>
        public ReturnStatus<CsvRows> Import(string path, char delimiter)
        {
            Exception exc = null;
            bool ok;
            CsvRows rows = new CsvRows();
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                try
                {
                    bool headerRowLoaded = Headers == null ? false : Headers.Length > 0;
                    char[] delimiters = new char[] { delimiter };
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (true)
                        {
                            string line = reader.ReadLine();
                            if (line == null)
                                break;
                            if (!headerRowLoaded)
                            {
                                Headers = GetColumns(line, delimiter);
                                headerRowLoaded = true;
                            }
                            else
                            {
                                CsvRow row = new CsvRow(this.Strict, this.Headers);
                                string[] rawColumns = GetColumns(line, delimiter);
                                for (int i = 0; i < Headers.Length; i++)
                                {
                                    object value = null;
                                    if (i < rawColumns.Length)
                                    {
                                        string s = rawColumns[i].Trim();
                                        value = string.IsNullOrEmpty(s) ? null : s;
                                    }
                                    row.AddColumn(Headers[i], value);
                                }
                                rows.AddRow(row);
                            }
                        }
                    }
                    ok = true;
                }
                catch (Exception e)
                {
                    exc = e;
                    ok = false;
                }
            }
            else
            {
                exc = new FileNotFoundException();
                ok = false;
            }
            return new ReturnStatus<CsvRows>(ok, exc, rows);
        }

        /// <summary>
        /// Get the column headers utilized.
        /// The column headers can be specified by the constructor.
        /// If specified by the constructor, then the source file is not looked to for the column headers.
        /// Otherwise, the first line of the source file is considered the column headers.
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
        /// Get the columns value from the specified line utilizing the specified column delimiter.
        /// </summary>
        /// <param name="line">Specifies a source line from the source file.</param>
        /// <param name="delimiter">Specifies the column delimiter.</param>
        /// <returns>Returns an array of type string that is the value of all columns found in the specified line.</returns>
        private string[] GetColumns(string line, char delimiter)
        {
            Delimiter = delimiter;
            List<string> l = new List<string>();

            bool quotation = false;

            string v = string.Empty;

            line = EncodeEmbeddedQuotes(line);
            foreach (char c in line.ToCharArray())
            {
                if (quotation && c == QuotationCharacter)
                {
                    quotation = false;
                }
                else if (!quotation && c == QuotationCharacter)
                {
                    quotation = true;
                }
                else if (c == delimiter)
                {
                    l.Add(DecodeEmbeddedQuotes(v));
                    v = string.Empty;
                }
                else
                {
                    v = string.Format("{0}{1}", v, c);
                }
            }
            if (!string.IsNullOrEmpty(v))
            {
                l.Add(DecodeEmbeddedQuotes(v));
            }

            return l.ToArray();
        }

        /// <summary>
        /// Decode any embedded quotes.
        /// </summary>
        /// <param name="value">Specifies the value to decode.</param>
        /// <returns>Returns the value with any embedded quotes decoded.</returns>
        private string DecodeEmbeddedQuotes(string value)
        {
            return value.Replace(ESCAPED_QUOTE_ENCODED, QUOTE);
        }

        /// <summary>
        /// Encode any embedded quotes.
        /// </summary>
        /// <param name="value">Specifies the value to encode.</param>
        /// <returns>Returns the value with any embedded quotes encoded.</returns>
        private string EncodeEmbeddedQuotes(string value)
        {
            return value
                   .Replace(QUOTE_COMMA_QUOTE, QUOTE_COMMA_QUOTE_ENCODED)
                   .Replace(QUOTE_COMMA, QUOTE_COMMA_ENCODED)
                   .Replace(COMMA_QUOTE, COMMA_QUOTE_ENCODED)
                   .Replace(ESCAPED_QUOTE, ESCAPED_QUOTE_ENCODED)
                   .Replace(QUOTE_COMMA_QUOTE_ENCODED, QUOTE_COMMA_QUOTE)
                   .Replace(QUOTE_COMMA_ENCODED, QUOTE_COMMA)
                   .Replace(COMMA_QUOTE_ENCODED, COMMA_QUOTE);
        }

        /// <summary>
        /// Currently specified delimiter.
        /// </summary>
        private char Delimiter
        {
            get; set;
        }

        /// <summary>
        /// Character assumed to be the column value quotation character if a quotation character is utilized within the source file.
        /// </summary>
        private const char QuotationCharacter = '"';

        /// <summary>
        /// Character to encode comma quote (,") combination.
        /// </summary>
        private string COMMA_QUOTE_ENCODED = ((char)24).ToString();

        /// <summary>
        /// Character to encode quote comma (",) combination.
        /// </summary>
        private string QUOTE_COMMA_ENCODED = ((char)25).ToString();

        /// <summary>
        /// Character to encode an escaped quote.
        /// </summary>
        private string ESCAPED_QUOTE_ENCODED = ((char)26).ToString();

        /// <summary>
        /// Escaped quote indicator.
        /// </summary>
        private string ESCAPED_QUOTE = "\"\"";

        /// <summary>
        /// Character to encode a quote comma quote (",") combination. (Note: the comma is superceded by any specified delimiter.)
        /// </summary>
        private string QUOTE_COMMA_QUOTE_ENCODED = ((char)27).ToString();

        /// <summary>
        /// Quote character.
        /// </summary>
        private string QUOTE = "\"";

        /// <summary>
        /// Embedded comma quote (,") combination. (Note: the comma is superceded by any specified delimiter.)
        /// </summary>
        private string COMMA_QUOTE
        {
            get
            {
                return string.Format("{0}\"", Delimiter);
            }
        }

        /// <summary>
        /// Embedded quote comma (",) combination. (Note: the comma is superceded by any specified delimiter.)
        /// </summary>
        private string QUOTE_COMMA
        {
            get
            {
                return string.Format("\"{0}", Delimiter);
            }
        }

        /// <summary>
        /// Embedded quote comma quote (",") combination. (Note: the comma is superceded by any specified delimiter.)
        /// </summary>
        private string QUOTE_COMMA_QUOTE
        {
            get
            {
                return string.Format("\"{0}\"", Delimiter);
            }
        }
    }
}