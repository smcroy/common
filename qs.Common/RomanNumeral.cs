using System;
using System.Text;
using System.Text.RegularExpressions;

namespace qs
{
    /// <summary>
    /// This class supports Roman numerals.
    /// Roman numerals are a numeral system that originated in ancient Rome and remained the usual way of writing numbers throughout Europe well into the Late Middle Ages. Numbers in this system are represented by combinations of letters from the Latin alphabet. Modern usage employs seven symbols, each with a fixed integer value: Symbol: I V X L C D M; Value: 1 5 10 50 100 500 1000.
    /// </summary>
    [Serializable]
    public class RomanNumeral : IDisposable
    {
        #region Fields

        private static readonly string[,] values =
            {
                { "M", "1000" },
                { "CM", "900" },
                { "D", "500" },
                { "CD", "400" },
                { "C", "100" },
                { "XC", "90" },
                { "L", "50" },
                { "XL", "40" },
                { "X", "10" },
                { "IX", "9" },
                { "V", "5" },
                { "IV", "4" },
                { "I", "1" }
            };

        private bool disposed;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create an instance of RomanNumeral initialized with a type of DateTime.
        /// </summary>
        /// <param name="value">Specifies a date. A date and time can be specified, however, only the year is utilized.</param>
        public RomanNumeral(DateTime value)
            : this(value.Year)
        {
        }

        /// <summary>
        /// Create an instance of RomanNumeral with the specified value.
        /// </summary>
        /// <param name="value">Specifies a Roman numeral as a string.</param>
        public RomanNumeral(string value)
            : this(ToInt64(value))
        {
        }

        /// <summary>
        /// Create an instance of RomanNumeral initialized with a type of Int16 (short).
        /// </summary>
        /// <param name="int16">Specifies an Int16.</param>
        public RomanNumeral(short int16)
            : this(Convert.ToInt64(int16))
        {
        }

        /// <summary>
        /// Create an instance of RomanNumeral initialized with a type of Int32.
        /// </summary>
        /// <param name="int32">Specifies an Int32.</param>
        public RomanNumeral(int int32)
            : this(Convert.ToInt64(int32))
        {
        }

        /// <summary>
        /// Create an instance of RomanNumeral initialized with a type of Int64 (long).
        /// </summary>
        /// <param name="int64">Specifies an Int64.</param>
        public RomanNumeral(long int64)
        {
            NumericValue = int64;
            Value = ToRomanNumeral(int64);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the numeric value of the Roman numeral as a Int64.
        /// </summary>
        public long NumericValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the Roman numeral as a string. See: ToString()
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns true if the the specified string is a Roman numeral; otherwise false.
        /// </summary>
        /// <param name="value">Specifies the value that might be a Roman numeral.</param>
        /// <returns>Returns true if the string is a Roman numeral; otherwise false.</returns>
        public static bool IsRomanNumeral(string value)
        {
            long i = ToInt64(value);
            return i > 0;
        }

        /// <summary>
        /// This IDisposable implementation follows Microsoft coding best practices.
        /// Dispose is also initiated by the destructor of this class.
        /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Because the Dispose is implemented, this class can be used in a 'using' statement.
        /// Do not make this method virtual. A derived class should not be able to override this method.
        /// Note: Within the destructor use Dispose(false).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, call GC.SuppressFinalize to take this object off the finalization queue
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the value of this instance of RomanNumeral as a string.
        /// </summary>
        /// <returns>Returns the value of this instance of RomanNumeral as a string</returns>
        public override string ToString()
        {
            return Value;
        }

        private static string Adjust(string value)
        {
            const string chars = "MDCLXVI";
            StringBuilder sb = new StringBuilder();
            foreach (char c in value.ToUpper().ToCharArray())
                if (chars.Contains(c))
                    sb.Append(c);
            string s = sb.ToString();
            return s;
        }

        private static int IndexOf(string value)
        {
            for (int x = 0; x <= values.GetUpperBound(0); x++)
            {
                if (values[x, 0] == value)
                {
                    return Convert.ToInt32(x);
                }
            }

            return -1;
        }

        private static long ToInt64(string value)
        {
            //string regex = "(^M{0,4})(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";
            value = Adjust(value);
            string regex = @"(^		# beginning of string
                M{0,4})				# thousands - 0 to 4 M's
                (CM|CD|D?C{0,3})	# hundreds - 900 (CM), 400 (CD), 0-300 (0 to 3 C's),
                                    # or 500-800 (D, followed by 0 to 3 C's)
                (XC|XL|L?X{0,3})	# tens - 90 (XC), 40 (XL), 0-30 (0 to 3 X's),
                                    # or 50-80 (L, followed by 0 to 3 X's)
                (IX|IV|V?I{0,3})	# ones - 9 (IX), 4 (IV), 0-3 (0 to 3 I's),
                                    # or 5-8 (V, followed by 0 to 3 I's)
                $					# end of string";

            RegexOptions options =
                ((RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Multiline) |
                RegexOptions.IgnoreCase);

            Regex reg = new Regex(regex, options);

            string[] rn = reg.Split(value.ToUpper());

            long y = 0;
            foreach (string s in rn)
            {
                if (s != string.Empty)
                {
                    y += ValueOf(s);
                }
            }

            return y;
        }

        private static int ValueOf(string value)
        {
            int x = IndexOf(value);
            if (x > -1)
            {
                return Convert.ToInt32(values[x, 1]);
            }
            else
                return 0;
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // if displosing equals true, dispose all managed and unmanaged resources
                if (disposing)
                {
                    // clear managed resources
                }
                disposed = true;
            }
        }

        private string ToRomanNumeral(long a)
        {
            long i = a;
            string s = string.Empty;
            for (int x = 0; x <= values.GetUpperBound(0); x++)
                while (i >= Convert.ToInt32(values[x, 1]))
                {
                    s = string.Concat(s, values[x, 0]);
                    i -= Convert.ToInt32(values[x, 1]);
                }
            return s;
        }

        #endregion Methods
    }
}