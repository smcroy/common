namespace qs.Extensions.StringExtensions
{
    using qs.Extensions.ByteExtensions;
    using qs.Extensions.StreamExtensions;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    public static class StringExtensions
    {
        #region Private Fields

        private static Dictionary<string, string> _encodingPairs;
        private static Dictionary<char, string> _EP;
        private static Regex isNumericEx;
        private static Regex isValidEmailSyntaxEx;

        #endregion Private Fields

        #region Private Properties

        private static Dictionary<string, string> EncodingPairs
        {
            get
            {
                if (_encodingPairs == null)
                {
                    _encodingPairs = new Dictionary<string, string>
                    {
                        { "™", "&#8482;" },
                        { "!", "&#33;" },
                        { "$", "&#36;" },
                        { "%", "&#37;" },
                        { "'", "&#39;" },
                        { "=", "&#61;" },
                        { "?", "&#63;" }
                    };
                }
                return _encodingPairs;
            }
        }

        private static Dictionary<char, string> EP
        {
            get
            {
                if (_EP == null)
                {
                    _EP = new Dictionary<char, string>
                    {
                        //_EP.Add( " ".ToCharArray( )[ 0 ], "/*32//" );
                        //_EP.Add( '!', "/*33//" );
                        //_EP.Add( '"', "/*34//" );
                        { '#', "/*35//" },
                        //_EP.Add( '$', "/*36//" );
                        //_EP.Add( '%', "/*37//" );
                        //_EP.Add( '&', "/*38//" );
                        //_EP.Add( "'".ToCharArray( )[ 0 ], "/*39//" );
                        //_EP.Add( '+', "/*43//" );
                        //_EP.Add( ',', "/*44//" );
                        //_EP.Add( '/', "/*47//" );
                        //_EP.Add( ':', "/*58//" );
                        { ';', "/*59//" }
                    };
                    //_EP.Add( '<', "/*60//" );
                    //_EP.Add( '=', "/*61//" );
                    //_EP.Add( '>', "/*62//" );
                    //_EP.Add( '?', "/*63//" );
                    //_EP.Add( '@', "/*64//" );
                    //_EP.Add( '[', "/*91//" );
                    //_EP.Add( "\\".ToCharArray( )[ 0 ], "/*92//" );
                    //_EP.Add( ']', "/*93//" );
                    //_EP.Add( '^', "/*94//" );
                    //_EP.Add( '`', "/*96//" );
                    //_EP.Add( '{', "/*123//" );
                    //_EP.Add( '|', "/*124//" );
                    //_EP.Add( '}', "/*125//" );
                    //_EP.Add( '~', "/*126//" );
                }
                return _EP;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Append the string to this instance of System.String.
        /// </summary>
        /// <param path="a">Instance of type System.String to which a is appended.</param>
        /// <param path="b">Instance of type System.String to be appended.</param>
        /// <returns>An instance of type System.String where a is concatenated to this instance.</returns>
        public static string Append(this string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return string.Empty;
            return string.Concat(a, b);
        }

        /// <summary>
        /// Compresses the instance of System.String to its equivalent compressed System.String
        /// representation encoded with base 64 digits. See: DecompressFromBase64String. Note:
        /// Compressing a short string may increase its length.
        /// </summary>
        /// <param name="a">Instance of type System.String.</param>
        /// <returns>An instance of System.String encoded with base 64 digits compressed.</returns>
        public static string CompressToBase64String(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;
            byte[] buffer = Encoding.Unicode.GetBytes(a);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }
                ms.Position = 0;
                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
                byte[] gzBuffer = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                return Convert.ToBase64String(gzBuffer);
            }
        }

        public static bool Contains(this string a, string value, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            return (a.IndexOf(value, comparisonType)) >= 0;
        }

        /// <summary>
        /// Decompresses the compressed instance of System.String encoded with base 64 digits to its
        /// equivalent uncompressed System.String representation. See: CompressToBase64String.
        /// </summary>
        /// <param name="a">Instance of type System.String encoded with base 64 digits and compressed.</param>
        /// <returns>An instance of System.String uncompressed.</returns>
        public static string DecompressFromBase64String(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;
            byte[] gzBuffer = Convert.FromBase64String(a);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Deserializes the JSON formatted string and returns an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object to serialize.</typeparam>
        /// <param name="a"></param>
        /// <returns>
        /// If the JSON formatted string can be deserialized as the specified object, returns an
        /// object of the specified type; otherwise returns null.
        /// </returns>
        public static T DeserializeFromJson<T>(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return default;
            return (T)DeserializeFromJson(a, typeof(T));
        }

        /// <summary>
        /// Deserializes the JSON formatted string and returns an object of the specified type.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type">Specifies the type of object to serialize.</param>
        /// <returns>
        /// If the JSON formatted string can be deserialized as the specified object, returns an
        /// object of the specified type; otherwise returns null.
        /// </returns>
        public static object DeserializeFromJson(this string s, Type type)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            System.Text.Encoding e = System.Text.Encoding.Default;
            byte[] b = e.GetBytes(s.ToCharArray());
            using (MemoryStream stream = new MemoryStream(b))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Deserializes the XML formatted string and returns an object of the specified type.
        /// </summary>
        /// <typeparam name="T">Specifies the type of object to serialize.</typeparam>
        /// <param name="a"></param>
        /// <returns>
        /// If the XML formatted string can be deserialized as the specfied object, returns an object
        /// of the specified type; otherwise returns null.
        /// </returns>
        public static T DeserializeFromXml<T>(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return default;
            return (T)a.DeserializeFromXml(typeof(T));
        }

        /// <summary>
        /// Deserialies the XML formatted string and returns an object of the specified type.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="type">Specifies the type of object to serialize.</param>
        /// <returns>
        /// If the XML formatted string can be deserialized as the specfied object, returns an object
        /// of the specified type; otherwise returns null.
        /// </returns>
        public static object DeserializeFromXml(this string a, Type type)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            XmlSerializer s = new XmlSerializer(type);
            using (MemoryStream stream = new MemoryStream(a.ToArray()))
            {
                return s.Deserialize(stream);
            }
        }

        /// <summary>
        /// Checks for existence of the specified directory path.
        /// </summary>
        /// <param name="a">
        /// Instance of type System.String for which the directory existence is checked.
        /// </param>
        /// <returns>true if the directory exists, otherwise false.</returns>
        public static bool DirectoryExists(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            if (a.IsDirectoryNameLegal())
            {
                DirectoryInfo di = new DirectoryInfo(a);
                bool ok = di.Exists;
                return ok;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks for existence of the specified file.
        /// </summary>
        /// <param name="a">Instance of type System.String for which the file existence is checked.</param>
        /// <returns>true if the file exists, otherwise false.</returns>
        public static bool FileExists(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            if (a.IsFileNameLegal())
            {
                FileInfo fi = new FileInfo(a);
                bool ok = fi.Exists;
                return ok;
            }
            else
                return false;
        }

        /// <summary>
        /// Converts the instance of System.String encoded with base 64 digits to its equivalent
        /// System.String representation.
        /// </summary>
        /// <param name="a">An instance of System.String encoded with base 64 digits.</param>
        /// <returns>An instance of System.String.</returns>
        public static string FromBase64String(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : Encoding.Default.GetString(Convert.FromBase64String(a));
        }

        public static byte[] FromBase64StringToArray(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? new byte[0] : Convert.FromBase64String(a);
        }

        /// <summary>
        /// Convert this specified instance of a short Guid (a 22-byte string) to a Guid. To convert
        /// a Guid to a short Guid, see GuidExtensions.ToShortGuid.
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static Guid FromShortGuid(this string shortGuid)
        {
            if (string.IsNullOrEmpty(shortGuid))
                return Guid.Empty;
            string s = shortGuid.Replace("_", "/").Replace("-", "+");
            byte[] b = Convert.FromBase64String(string.Format("{0}==", s));
            return new Guid(b);
        }

        /// <summary>
        /// Returns the display width in pixels of the specified string based upon the specified font
        /// family and font size. Returns 0 if the specified string is null or empty, if the font
        /// family is not specified, or if the font size is less than or equal to 0.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static int GetDisplayWidth(this string a, string fontFamily, int fontSize)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(fontFamily) || fontSize <= 0)
                return 0;

            Font font = new Font(fontFamily, fontSize, GraphicsUnit.Pixel);
            return a.GetDisplayWidth(font);
        }

        /// <summary>
        /// Returns the display width in pixels of the specified string based upon the specified font.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static int GetDisplayWidth(this string a, Font font)
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                Size i = MeasureDisplayStringWidth(g, a, font);
                return i.Width;
            }
        }

        /// <summary>
        /// Get the file contents as a byte array. If the string represents an existing file, then
        /// return the file contents as a byte array, otherwise return null.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static byte[] GetFileAsArray(this string a)
        {
            if (!string.IsNullOrWhiteSpace(a) && a.FileExists())
            {
                FileInfo fi = new FileInfo(a);
                if (fi.Exists)
                {
                    using (FileStream stream = (FileStream)new StreamReader(a).BaseStream)
                    {
                        //byte[ ] b = new byte[ stream.Length ];
                        //stream.Read ( b, 0, ( Int32 ) stream.Length );
                        //stream.Flush ( );
                        //stream.Close ( );
                        //stream.Dispose ( );
                        //return b;

                        List<byte> b = new List<byte>();
                        try
                        {
                            using (BinaryReader r = new BinaryReader(stream))
                            {
                                while (0 == 0)
                                    b.Add(r.ReadByte());
                            }
                        }
                        catch (EndOfStreamException)
                        {
                        }
                        return b.ToArray();
                    }
                }
            }
            return null;
        }

        public static char GetFirstChar(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return ' ';
            return Convert.ToChar(a.PadRight(1).TruncateToMaxLength(1));
        }

        /// <summary>
        /// Get unique identifier based upon the specified string. This is a repeatable identifier
        /// generator, i.e. given the same string the same unique identifer is returned.
        /// </summary>
        /// <param name="a">Specified string from which a unique identifier is generated.</param>
        /// <returns>Retrns a unique identifier based upon the specified string.</returns>
        public static string GetUniqueID(this string a)
        {
            return a.GetHashCode().ToString("x").PadLeft(8, '0');
        }

        public static string HtmlDecode(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;
            string s = System.Web.HttpUtility.HtmlDecode(a);
            foreach (var v in EncodingPairs)
                s = s.Replace(v.Value, v.Key);
            return s;
        }

        public static string HtmlEncode(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;
            string s = System.Web.HttpUtility.HtmlEncode(a);
            foreach (var v in EncodingPairs)
                s = s.Replace(v.Key, v.Value);
            return s;
        }

        /// <summary>
        /// Determines if the System.String is a valid representation of a date and time.
        /// </summary>
        /// <param path="a">Instance of type System.String used to determine validity.</param>
        /// <returns>
        /// true if the System.String is a valid representation of a date and time, otherwise returns false.
        /// </returns>
        public static bool IsDateTime(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            bool ok = DateTime.TryParse(a, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out _);
            if (ok)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if the directory path is syntactically correct.
        /// </summary>
        /// <param name="a">
        /// Instance of type System.String which is checked for syntax correctness as a name of a
        /// directory path.
        /// </param>
        /// <returns>true if the directory path is syntactically correct, otherwise false;</returns>
        public static bool IsDirectoryNameLegal(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            return VerifySyntaxLegality(a, Path.GetInvalidPathChars());
        }

        /// <summary>
        /// Determines if the file name is syntactically correct. If the string represents a full
        /// qualified file name, then the file name and the directory name are syntactically verified.
        /// </summary>
        /// <param name="a">
        /// Instance of type System.String which is checked for syntax correctness as a name of a
        /// file path.
        /// </param>
        /// ///
        /// <returns>true if the file name is syntactically correct, otherwise false.</returns>
        public static bool IsFileNameLegal(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            string[] s = a.Split(Path.DirectorySeparatorChar);

            string file = s[s.Length - 1];
            bool fileOk;
            if (!string.IsNullOrEmpty(file))
                fileOk = VerifySyntaxLegality(file, Path.GetInvalidFileNameChars());
            else
                fileOk = false;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length - 1; i++)
            {
                sb.Append(s[i]);
                sb.Append(Path.DirectorySeparatorChar);
            }

            string path = sb.ToString();
            bool pathOk;
            if (!string.IsNullOrEmpty(path))
                pathOk = VerifySyntaxLegality(path, Path.GetInvalidPathChars());
            else
                pathOk = false;

            return pathOk && fileOk;
        }

        public static bool IsLike(this string a, string b, StringComparison comparisonType)
        {
            const string wildcard = "*";

            string l;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                return false;
            }
            if (b.StartsWith(wildcard) && b.EndsWith(wildcard))
            {
                l = b.Remove(b.Length - 1, 1).Remove(0, 1);
                return (a.IndexOf(l, comparisonType) >= 0);
            }
            if (b.StartsWith(wildcard))
            {
                l = b.Remove(0, 1);
                return a.EndsWith(l, comparisonType);
            }
            if (b.EndsWith(wildcard))
            {
                l = b.Remove(b.Length - 1, 1);
                return a.StartsWith(l, comparisonType);
            }
            return a.Equals(b, comparisonType);
        }

        /// <summary>
        /// Determines if the System.String is numeric. It is culture aware. It looks at the
        /// character string for the following culture aware symbols and separators: currency decimal
        /// separator, currency group separator, currency symbol, negative sign, negative infinity
        /// symbol, number decimal separator, number group separator, percent decimal separator,
        /// percent group separator, percent symbol, per mille symbol, positive infinity symbol, and
        /// positive sign.
        /// </summary>
        /// <param path="a">Instance of type System.String used to determine validity.</param>
        /// <returns>true if the character string is numeric, otherwise false</returns>
        public static bool IsNumeric(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            string s = StripCultureAwareNumberFormatInfoStrings(StripCultureAwareStrings(a));
            if (string.IsNullOrEmpty(s))
                return false;
            if (isNumericEx == null)
                isNumericEx = new Regex("^(?<number>[0-9])+$");

            Match match = isNumericEx.Match(s);
            return match.Success;
        }

        /// <summary>
        /// Determines if the character string is a valid representation of an Email address.
        /// </summary>
        /// <param path="value">Instance of type System.String used to determine validity.</param>
        /// <returns>
        /// true if the System.String is a valid representation of an Email address, otherwise false.
        /// </returns>
        public static bool IsValidEmailSyntax(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;

            if (isValidEmailSyntaxEx == null)
                isValidEmailSyntaxEx = new Regex(@"^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|&quot;((?=[\x01-\x7f])[^&quot;\\]|\\[\x01-\x7f])*&quot;\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|&quot;((?=[\x01-\x7f])[^&quot;\\]|\\[\x01-\x7f])*&quot;)@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$".Replace("&quot;", "\""), RegexOptions.IgnoreCase);
            // new Regex(
            // @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$",
            // RegexOptions.IgnoreCase );

            Match match = isValidEmailSyntaxEx.Match(a.Trim());
            return match.Success;
        }

        /// <summary>
        /// Returns the instance of System.String packaged as inline JavaScript.
        /// <para>&lt;script type='text/javascript' language='javascript'&gt;</para>
        /// <para>// &lt;![CDATA[</para>
        /// <para>script...</para>
        /// <para>// ]]&gt;</para>
        /// <para>&lt;/script&gt;</para>
        /// </summary>
        /// <param name="script">Javascript</param>
        /// <returns>inline JavaScript as an instance of System.String</returns>
        public static string PackageAsInlineJavaScript(this string script)
        {
            return string.Format("{0}<script type='text/javascript' language='javascript'>{0}// <![CDATA[{0}{1}{0}// ]]>{0}</script>", Environment.NewLine, script);
        }

        /// <summary>
        /// Returns the instance of System.String packaged as a JavaScript function call.
        /// <para>javascript:functionname('arg1','arg2');</para>
        /// </summary>
        /// <param name="functionname">JavaScript function name</param>
        /// <param name="args">Optional: arguments or parameters to pass to the function</param>
        /// <returns>JavaScript function call as an instance of System.String</returns>
        public static string PackageAsJavaScriptFunctionCall(this string functionname, params string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string a in args)
                sb.AppendFormat("'{0}',", a);
            string s = sb.ToString().TrimEnd(',');
            return string.Format("javascript:{0}({1});", functionname, s);
        }

        public static string PadRightToBoundary(this string a, int boundary)
        {
            if (boundary <= 0)
                return a;
            if (string.IsNullOrWhiteSpace(a))
                return " ".PadRight(boundary);
            int x = a.Length % boundary;
            int y = x == 0 ? 0 : boundary - x;
            return a.PadRight(a.Length + y);
        }

        public static string PadRightToBoundary(this string a, int boundary, char paddingChar)
        {
            if (boundary <= 0)
                return a;
            if (string.IsNullOrWhiteSpace(a))
                return " ".PadRight(boundary, paddingChar);
            int x = a.Length % boundary;
            int y = x == 0 ? 0 : boundary - x;
            return a.PadRight(a.Length + y, paddingChar);
        }

        /// <summary>
        /// Prepend the string to this instance of System.String.
        /// </summary>
        /// <param path="a">Instance of type System.String to which a is prepended.</param>
        /// <param path="b">Instance of type System.String to be prepended.</param>
        /// <returns>
        /// An instance of type System.String where this instance is concatenated to a.
        /// </returns>
        public static string Prepend(this string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
                return string.Empty;
            return string.Concat(b, a);
        }

        /// <summary>
        /// Remove punctuation from this instance of System.String.
        /// </summary>
        /// <param name="a">Instance of type System.String from which punctuation is removed.</param>
        /// <returns>An instance of type System.String where all punctuation has been removed.</returns>
        public static string RemovePunctuation(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return string.Empty;
            char[] c = a.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (var v in c)
                if (char.IsLetterOrDigit(v) || char.IsWhiteSpace(v))
                    sb.Append(v);
            string s = sb.ToString();
            return s;
        }

        /// <summary>
        /// Remove whitespace from this instance of System.String.
        /// </summary>
        /// <param name="a">Instance of type System.String from which whitespace is removed.</param>
        /// <returns>An instance of type System.String where all whitespace has been removed.</returns>
        public static string RemoveWhiteSpace(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return string.Empty;
            char[] c = a.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (var v in c)
                if (!char.IsWhiteSpace(v))
                    sb.Append(v);
            string s = sb.ToString();
            return s;
        }

        /// <summary>
        /// Replaces all occurrences of a specified System.String in this instance, with another
        /// specified System.String.
        ///
        /// Parameters: oldValue: A System.String to be replaced.
        ///
        /// newValue: A System.String to replace all occurrences of oldValue. comparisonType: One of
        /// the System.StringComparison values.
        ///
        /// Returns: A System.String equivalent to this instance but with all instances of oldValue
        /// replaced with newValue.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static string Replace(this string a, string oldValue, string newValue, StringComparison stringComparison)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(oldValue))
                return a;

            string value = string.IsNullOrEmpty(newValue) ? string.Empty : newValue;

            RegexOptions regexOptions = RegexOptions.None;
            switch (stringComparison)
            {
                case StringComparison.CurrentCulture:
                    break;

                case StringComparison.CurrentCultureIgnoreCase:
                    regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
                    break;

                case StringComparison.InvariantCulture:
                    regexOptions = RegexOptions.CultureInvariant;
                    break;

                case StringComparison.InvariantCultureIgnoreCase:
                    regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;
                    break;

                case StringComparison.Ordinal:
                    regexOptions = RegexOptions.IgnoreCase;
                    break;

                case StringComparison.OrdinalIgnoreCase:
                    regexOptions = RegexOptions.IgnoreCase;
                    break;

                default:
                    break;
            }
            string s = Regex.Replace(a, oldValue, value, regexOptions);
            return s;
        }

        /// <summary>
        /// Returns the specified string reversed. For example, "ABC" is returned as "CBA".
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string Reverse(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return string.Empty;
            char[] c = a.ToCharArray();
            Array.Reverse(c);
            return new string(c);
        }

        /// <summary>
        /// Returns an empty string if the specified string is null, otherwise returns the specified
        /// string trimmed.
        /// </summary>
        /// <param name="a">The string to trim.</param>
        /// <returns>
        /// If the specified string is null, an empty string; otherwise the specified string trimmed.
        /// </returns>
        public static string SafeTrim(this string a)
        {
            return string.IsNullOrEmpty(a) ? string.Empty : a.Trim();
        }

        public static string SpecialDecode(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            string t = s;
            foreach (KeyValuePair<char, string> u in EP)
                t = t.Replace(u.Value, u.Key.ToString());
            return t;
        }

        public static string SpecialEncode(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            char[] array = s.ToCharArray();
            foreach (char c in array)
            {
                if (EP.ContainsKey(c))
                    sb.Append(EP.First(a => a.Key.Equals(c)).Value);
                else
                    sb.Append(c);
            }
            string t = sb.ToString();
            sb = null;
            return t;
        }

        public static string StripHTML(this string a)
        {
            const string HTMLTAGPATTERN = "<.*?>";
            string s = string.IsNullOrWhiteSpace(a) ? string.Empty : a;
            return Regex.Replace(s, HTMLTAGPATTERN, string.Empty);
        }

        /// <summary>
        /// Returns a string after removing the HTML tags from the specified string.
        /// </summary>
        /// <param name="a">The string from which to HTML tags are to be removed.</param>
        /// <returns>The string with the HTML tags removed.</returns>
        public static string StripHtmlTags(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return a;

            a = a.Replace("&#160;", " ");
            char[] array = new char[a.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < a.Length; i++)
            {
                char let = a[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// Encodes the specified string into a sequence of bytes. The encoding for the operating
        /// system's current ANSI code page is utilized to encode the specified string.
        /// </summary>
        /// <param name="a">The string to encode.</param>
        /// <returns>A byte array containing the results of encoding the specified string.</returns>
        public static byte[] ToArray(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? new byte[0] : a.ToByteArray(System.Text.Encoding.Default);
        }

        /// <summary>
        /// Encodes the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="a">The string to encode.</param>
        /// <param name="encoding">The encoding to utilize to encode the specified string.</param>
        /// <returns>A byte array containing the results of encoding the specified string.</returns>
        public static byte[] ToArray(this string a, System.Text.Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(a))
                return new byte[0];
            if (!(encoding is System.Text.Encoding e))
                e = System.Text.Encoding.Default;
            return e.GetBytes(a.ToCharArray());
        }

        /// <summary>
        /// Converts the instance of System.String to its equivalent System.String representation
        /// encoded with base 64 digits.
        /// </summary>
        /// <param name="a">An instance of System.String.</param>
        /// <returns>An instance of System.String encoded with base 64 digits.</returns>
        public static string ToBase64String(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : Convert.ToBase64String(Encoding.Default.GetBytes(a.ToCharArray()));
        }

        /// <summary>
        /// Convert the string to a System.Boolean. If the string equals "on" or "true" or "1" or
        /// "yes", return true; otherwise false.
        /// </summary>
        /// <param name="a">Instance of type System.String to be converted.</param>
        /// <returns>true if the string equals "on" or "true" or "1" or "yes", otherwise false.</returns>
        /// <remarks>Change 2010/12/30: Added equality to "1" return true.</remarks>
        /// <remarks>Change 2014/02/17: Added equality to "yes" return true.</remarks>
        public static bool ToBoolean(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return false;
            if (a.Equals("on", StringComparison.OrdinalIgnoreCase) || a.Equals("true", StringComparison.OrdinalIgnoreCase) || a.StartsWith("t", StringComparison.OrdinalIgnoreCase) || a.Equals("1") || a.Equals("yes", StringComparison.OrdinalIgnoreCase) || a.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Encodes the specified string into a sequence of bytes. The encoding for the operating
        /// system's current ANSI code page is utilized to encode the specified string.
        /// </summary>
        /// <param name="a">The string to encode.</param>
        /// <returns>A byte array containing the results of encoding the specified string.</returns>
        public static byte[] ToByteArray(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? new byte[0] : a.ToArray(System.Text.Encoding.Default);
        }

        /// <summary>
        /// Encodes the specified string into a sequence of bytes.
        /// </summary>
        /// <param name="a">The string to encode.</param>
        /// <param name="encoding">The encoding to utilize to encode the specified string.</param>
        /// <returns>A byte array containing the results of encoding the specified string.</returns>
        public static byte[] ToByteArray(this string a, System.Text.Encoding encoding)
        {
            return string.IsNullOrWhiteSpace(a) ? new byte[0] : a.ToArray(encoding);
        }

        ///// <summary>
        ///// Convert the instance of System.String to a new instance of System.Data.Linq.Binary.
        ///// </summary>
        ///// <param name="a">
        ///// An instance of System.String which encodes an instance of System.Data.Linq.Binary as base
        ///// 64 digits.
        ///// </param>
        ///// <returns>
        ///// A new instance of System.Data.Linq.Binary created from the encoded instance of System.String.
        ///// </returns>
        //public static System.Data.Linq.Binary ToDataLinqBinary( this string a )
        //{
        //    return string.IsNullOrWhiteSpace( a ) ? null : new System.Data.Linq.Binary( Convert.FromBase64String( a ) );
        //}

        /// <summary>
        /// Converts the specified string representation of a date and time to its System.DateTime
        /// equivalent. If not a valid string representation of a date and time, returns a
        /// System.DateTime with a minimum value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a date and time, returns its System.DateTime
        /// equivalent, otherwise returns a System.DateTime with a minimum value.
        /// </returns>
        public static DateTime ToDateTime(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return new DateTime();
            bool ok = DateTime.TryParse(a, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out DateTime dt);
            if (ok)
                return dt;
            else
                return new DateTime();
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Decimal equivalent.
        /// If not a valid string representation of a number, returns a System.Decimal with a value
        /// of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Decimal equivalent,
        /// otherwise returns a System.Decimal with a value of 0.
        /// </returns>
        public static decimal ToDecimal(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            return d;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Double equivalent. If
        /// not a valid string representation of a number, returns a System.Double with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Double equivalent,
        /// otherwise returns a System.Double with a value of 0.
        /// </returns>
        public static double ToDouble(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out double d);
            return d;
        }

        /// <summary>
        /// Returns an empty string if the specified string is null, otherwise returns the specified string.
        /// </summary>
        /// <param name="a">The string to test.</param>
        /// <returns>
        /// If the specified string is null, an empty string; otherwise the specified string.
        /// </returns>
        public static string ToEmptyStringIfNull(this string a)
        {
            return string.IsNullOrEmpty(a) ? string.Empty : a;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Single
        /// (single-precision floating-point number) equivalent. If not a valid string representation
        /// of a number, returns a System.Single with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Single equivalent,
        /// otherwise returns a System.Single with a value of 0.
        /// </returns>
        public static float ToFloat(this string a)
        {
            return string.IsNullOrEmpty(a) ? 0 : a.ToSingle();
        }

        /// <summary>
        /// Converts the System.String representation of a globally unique identifier (Guid) to its
        /// System.Guid equivalent. If not a valid string representation of a Guid, returns an empty Guid.
        /// </summary>
        /// <param name="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a Guid, returns its System.Guid equivalent, otherwise
        /// returns an empty Guid.
        /// </returns>
        public static Guid ToGuid(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return Guid.Empty;
            try
            {
                return new Guid(a);
            }
            catch
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Replaces any line feed characters (\n) with HTML line break (&lt;br /&gt;).
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Line formatted as HTML string.</returns>
        public static string ToHTMLString(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : a.Replace("\n", "<br />");
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int16 equivalent. If
        /// not a valid string representation of a number, returns a System.Int16 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int16 equivalent,
        /// otherwise returns a System.Int16 with a value of 0.
        /// </returns>
        public static short ToInt16(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            return decimal.ToInt16(decimal.Round(d, 0));
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int16 equivalent. If
        /// not a valid string representation of a number, returns a System.Int16 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int16 equivalent,
        /// otherwise returns a System.Int16 with a value of 0.
        /// </returns>
        public static short ToInt16(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            short.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out short i);
            return i;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int32 equivalent
        /// using the specified style and culture-specific format. If not a valid string
        /// representation of a number, returns a System.Int32 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int32 equivalent,
        /// otherwise returns a System.Int32 with a value of 0.
        /// </returns>
        public static int ToInt32(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            return decimal.ToInt32(decimal.Round(d, 0));
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int32 equivalent. If
        /// not a valid string representation of a number, returns a System.Int32 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int32 equivalent,
        /// otherwise returns a System.Int32 with a value of 0.
        /// </returns>
        public static int ToInt32(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            int.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out int i);
            return i;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int64 equivalent. If
        /// not a valid string representation of a number, returns a System.Int64 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int64 equivalent,
        /// otherwise returns a System.Int64 with a value of 0.
        /// </returns>
        public static long ToInt64(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            return decimal.ToInt64(decimal.Round(d, 0));
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Int64 equivalent. If
        /// not a valid string representation of a number, returns a System.Int64 with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Int64 equivalent,
        /// otherwise returns a System.Int64 with a value of 0.
        /// </returns>
        public static long ToInt64(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            long.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out long i);
            return i;
        }

        /// <summary>
        /// Returns a KeyValuePair&lt;int, string&gt; based upon the instance of System.String and
        /// the specified key. If the value is null, empty, or consists of only white-space
        /// characters and the key is null returns a new KeyValuePair&lt;int, string&gt;, otherwise
        /// if the value is not null, empty, or consists of only white-space characters and the key
        /// is null returns a new KeyValuePair&lt;int, string&gt; valued with this instance of
        /// System.String with a key of 0, otherwise if the value is null, empty, or consists of only
        /// white-space characters and the key is not null returns a new KeyValuePair&lt;int,
        /// string&gt; valued with an empty string, otherwise returns a new KeyValuePair&lt;int,
        /// string&gt; valued with this instance of System.String with the specified key.
        /// </summary>
        /// <param name="a">An instance of System.String that will become the value of the KeyValuePair</param>
        /// <param name="key">The key of the KeyValuePair</param>
        /// <returns>
        /// If the value is null, empty, or consists of only white-space characters and the key is
        /// null returns a new KeyValuePair&lt;int, string&gt;, otherwise if the value is not null,
        /// empty, or consists of only white-space characters and the key is null returns a new
        /// KeyValuePair&lt;int, string&gt; valued with this instance of System.String with a key of
        /// 0, otherwise if the value is null, empty, or consists of only white-space characters and
        /// the key is not null returns a new KeyValuePair&lt;int, string&gt; valued with an empty
        /// string, otherwise returns a new KeyValuePair&lt;int, string&gt; valued with this instance
        /// of System.String with the specified key.
        /// </returns>
        public static KeyValuePair<int, string> ToKeyValuePair(this string a, int? key)
        {
            if (string.IsNullOrWhiteSpace(a))
            {
                if (!key.HasValue)
                    return new KeyValuePair<int, string>();
                else
                    return new KeyValuePair<int, string>(key.Value, string.Empty);
            }
            else
            {
                if (!key.HasValue)
                    return new KeyValuePair<int, string>(0, a);
                else
                    return new KeyValuePair<int, string>(key.Value, a);
            }
        }

        public static string ToLegalFileName(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return "NewFile";
            string[] s = a.Split(Path.DirectorySeparatorChar);

            bool ok = false;
            string file = s[s.Length - 1];
            if (!string.IsNullOrEmpty(file))
                ok = VerifySyntaxLegality(file, Path.GetInvalidFileNameChars());

            if (ok)
                return file;

            char DEFAULT = '_';
            string invalid = Path.GetInvalidFileNameChars().ToString();
            StringBuilder sb = new StringBuilder();
            foreach (char c in file.ToCharArray())
                if (invalid.Contains(c.ToString()))
                    sb.Append(DEFAULT);
                else
                    sb.Append(c);

            string t = sb.ToString();
            return t;
        }

        //public static string ToLegalFileName( this string a )
        //{
        //    if ( string.IsNullOrEmpty( a ) )
        //    {
        //        return "New File";
        //    }
        //    string[ ] s = a.Split( new char[ ] { Path.DirectorySeparatorChar } );
        //    string file = s[ s.Length - 1 ];
        //    if ( string.IsNullOrEmpty( file ) )
        //    {
        //        return "New File";
        //    }
        //    foreach ( char c in Path.GetInvalidFileNameChars( ) )
        //    {
        //        file = file.Replace( c.ToString( CultureInfo.InvariantCulture ), " " );
        //    }
        //    return file;
        //}
        /// <summary>
        /// Converts the specified string representation of a boolean to its
        /// System.Nullable&lt;Boolean&gt; equivalent. If the string is null or empty returns a null,
        /// else true if the string equals "on" or "true" or "1", otherwise false.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If the string is null or empty returns a null, else true if the string equals "on" or
        /// "true" or "1", otherwise false.
        /// </returns>
        /// <remarks>Change 2014/2/7: Added equality to "1" return true.</remarks>
        public static bool? ToNullableBoolean(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            if (a.Equals("on", StringComparison.OrdinalIgnoreCase) || a.Equals("true", StringComparison.OrdinalIgnoreCase) || a.StartsWith("t", StringComparison.OrdinalIgnoreCase) || a.Equals("1"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Converts the specified string representation of a date and time to its
        /// System.Nullable&lt;DateTime&gt; equivalent. If not a valid string representation of a
        /// date and time, returns a System.Nullable&lt;DateTime&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a date and time, returns its System.DateTime
        /// equivalent, otherwise returns a System.DateTime with a minimum value.
        /// </returns>
        public static DateTime? ToNullableDateTime(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            bool ok = DateTime.TryParse(a, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out DateTime dt);
            if (ok)
                return dt;
            else
                return null;
        }

        /// <summary> Converts the System.String representation of a number to its
        /// System.Nullable<Decimal> equivalent. If not a valid string representation of a number,
        /// returns a System.Nullable<Decimal> with a null value. </summary> <param path="a">Instance
        /// of type System.String to be converted.</param> <returns>If valid string representation of
        /// a number, returns its System.Nullable<Decimal> equivalent, otherwise returns a
        /// System.Nullable<Decimal> with a null value.</returns>
        public static decimal? ToNullableDecimal(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            if (ok)
                return d;
            else
                return null;
        }

        /// <summary> Converts the System.String representation of a number to its
        /// System.Nullable<Double> equivalent. If not a valid string representation of a number,
        /// returns a System.Nullable<Double> with a null value. </summary> <param path="a">Instance
        /// of type System.String to be converted.</param> <returns>If valid string representation of
        /// a number, returns its System.Nullable<Double> equivalent, otherwise returns a
        /// System.Nullable<Double> with a null value.</returns>
        public static double? ToNullableDouble(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out double d);
            if (ok)
                return d;
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its
        /// System.Nullable&lt;Single&gt; (single-precision floating-point number) equivalent. If not
        /// a valid string representation of a number, returns a System.Nullable&lt;Single&gt; with a
        /// null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Single&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Single&gt; with a null value.
        /// </returns>
        public static float? ToNullableFloat(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            return a.ToNullableSingle();
        }

        /// <summary>
        /// Converts the System.String representation of a globally unique identifier (Guid) to its
        /// System.Nullable&lt;Guid&gt; equivalent. If not a valid string representation of a Guid,
        /// returns a null.
        /// </summary>
        /// <param name="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a Guid, returns its System.Guid equivalent, otherwise
        /// returns a null.
        /// </returns>
        public static Guid? ToNullableGuid(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            try
            {
                return new Guid(a);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int16&gt;
        /// equivalent using the specified style and culture-specific format. If not a valid string
        /// representation of a number, returns a System.Nullable&lt;Int16&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int16&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int16&gt; with a null value.
        /// </returns>
        public static short? ToNullableInt16(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            if (ok)
                return decimal.ToInt16(decimal.Round(d, 0));
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int16&gt;
        /// equivalent. If not a valid string representation of a number, returns a
        /// System.Nullable&lt;Int16&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int16&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int16&gt; with a null value.
        /// </returns>
        public static short? ToNullableInt16(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = short.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out short i);
            if (ok)
                return i;
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int32&gt;
        /// equivalent. If not a valid string representation of a number, returns a
        /// System.Nullable&lt;Int32&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int32&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int32&gt; with a null value.
        /// </returns>
        public static int? ToNullableInt32(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = int.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out int i);
            if (ok)
                return i;
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int32&gt;
        /// equivalent. If not a valid string representation of a number, returns a
        /// System.Nullable&lt;Int32&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int32&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int32&gt; with a null value.
        /// </returns>
        public static int? ToNullableInt32(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            if (ok)
                return decimal.ToInt32(decimal.Round(d, 0));
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int64&gt;
        /// equivalent. If not a valid string representation of a number, returns a
        /// System.Nullable&lt;Int64&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <param name="numberStyle">
        /// A bitwise combination of System.Globalization.NumberStyles values that indicates the
        /// permitted format of the string. A typical value to specify is System.Globalization.NumberStyles.HexNumber.
        /// </param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int64&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int64&gt; with a null value.
        /// </returns>
        public static long? ToNullableInt64(this string a, NumberStyles numberStyle)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = long.TryParse(s, numberStyle, CultureInfo.CurrentCulture, out long i);
            if (ok)
                return i;
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Nullable&lt;Int64&gt;
        /// equivalent. If not a valid string representation of a number, returns a
        /// System.Nullable&lt;Int64&gt; with a null value.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Nullable&lt;Int64&gt;
        /// equivalent, otherwise returns a System.Nullable&lt;Int64&gt; with a null value.
        /// </returns>
        public static long? ToNullableInt64(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d);
            if (ok)
                return decimal.ToInt64(decimal.Round(d, 0));
            else
                return null;
        }

        /// <summary>
        /// Returns a KeyValuePair&lt;int?, string&gt; based upon the instance of System.String and
        /// the specified key.
        /// </summary>
        /// <param name="a">An instance of System.String that will become the value of the KeyValuePair</param>
        /// <param name="key">The key of the KeyValuePair</param>
        /// <returns>Returns a new KeyValuePair&lt;int?, string&gt;.</returns>
        public static KeyValuePair<int?, string> ToNullableKeyValuePair(this string a, int? key)
        {
            return new KeyValuePair<int?, string>(key, a);
        }

        /// <summary> Converts the System.String representation of a number to its
        /// System.Nullable<Single> (single-precision floating-point number) equivalent. If not a
        /// valid string representation of a number, returns a System.Nullable<Single> with a null
        /// value. </summary> <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>If valid string representation of a number, returns its System.Nullable<Single>
        /// equivalent, otherwise returns a System.Nullable<Single> with a null value.</returns>
        public static float? ToNullableSingle(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;
            string s = StripCultureAwareStrings(a);
            bool ok = float.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out float f);
            if (ok)
                return f;
            else
                return null;
        }

        /// <summary>
        /// Converts the System.String to its RomanNumeral equivalent.
        /// </summary>
        /// <param name="a">Instance of type System.String to be converted.</param>
        /// <returns>Instance of type RomanNumeral.</returns>
        public static RomanNumeral ToRomanNumeral(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? null : new RomanNumeral(a);
        }

        /// <summary>
        /// Return an instance of type System.Security.SecureString with the value of this instance
        /// of System.String.
        /// </summary>
        /// <param name="a">
        /// Instance of type System.String from which an instance of type
        /// System.Security.SecureString is created.
        /// </param>
        /// <returns>
        /// An instance of type System.Security.SecureString with the value of this instance of type System.String.
        /// </returns>
        public static SecureString ToSecureString(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return null;

            SecureString s = new SecureString();
            foreach (var c in a.ToCharArray())
                s.AppendChar(c);

            return s;
        }

        /// <summary>
        /// Converts the System.String representation of a number to its System.Single
        /// (single-precision floating-point number) equivalent. If not a valid string representation
        /// of a number, returns a System.Single with a value of 0.
        /// </summary>
        /// <param path="a">Instance of type System.String to be converted.</param>
        /// <returns>
        /// If valid string representation of a number, returns its System.Single equivalent,
        /// otherwise returns a System.Single with a value of 0.
        /// </returns>
        public static float ToSingle(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return 0;
            string s = StripCultureAwareStrings(a);
            float.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out float f);
            return f;
        }

        /// <summary>
        /// Converts the System.String to its Soundex equivalent.
        /// </summary>
        /// <param path="value">Instance of type System.String to be converted.</param>
        /// <returns>An instance of System.String that is the Soundex equivalent of this instance.</returns>
        public static string ToSoundex(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;
            string result = string.Empty;

            if (!string.IsNullOrEmpty(a))
            {
                string v = a.ToUpperInvariant();
                if (char.IsLetter(v.ToCharArray(0, 1)[0])) // 1st character must be a letter
                {
                    // flush all but characters
                    StringBuilder temp = new StringBuilder();
                    foreach (char c in v.ToCharArray())
                    {
                        if (char.IsLetter(c))
                            temp.Append(c);
                        else if (c.ToString(CultureInfo.InvariantCulture) == " ")
                            temp.Append(c);
                    }
                    v = temp.ToString();

                    // replace special abbreviations
                    temp = new StringBuilder();
                    if (v.StartsWith("ST "))
                    {
                        temp.Append("SAINT");
                        temp.Append(v.Substring(2));
                        v = temp.ToString();
                    }

                    // append 1st letter to result
                    StringBuilder sb = new StringBuilder();
                    sb.Append(v.Substring(0, 1));

                    // convert to soundex
                    foreach (char c in v.Substring(1).ToCharArray())
                        sb.Append(GetSoundexCategory(c));

                    // remove duplicates
                    result = sb.ToString();
                    while (result.IndexOf("11") >= 0 || result.IndexOf("22") >= 0 || result.IndexOf("33") >= 0 || result.IndexOf("44") >= 0 || result.IndexOf("55") >= 0 || result.IndexOf("66") >= 0)
                        result = result.Replace("11", "1").Replace("22", "2").Replace("33", "3").Replace("44", "4").Replace("55", "5").Replace("66", "6");

                    // remove vowel place holders
                    result = result.Replace("0", string.Empty).Trim();

                    // size to length of 4
                    result = string.Format(CultureInfo.InvariantCulture, "{0}0000", result).Substring(0, 4);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert the instance of System.String, which encodes binary data as base 64 digits, to an
        /// equivalent string.
        /// </summary>
        /// <param name="a">An instance of System.String which encodes binary data as base 64 digits.</param>
        /// <returns>An instance of System.String as an equivalent to the encoded data.</returns>
        public static string ToStringFromBase64String(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : Encoding.Default.GetString(Convert.FromBase64String(a));
        }

        public static string ToTelephoneFormat(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return string.Empty;
            string s = a.PadLeft(10, '0');
            return string.Format("({0}) {1}-{2}", s.Substring(0, 3), s.Substring(3, 3), s.Substring(6, 4));
        }

        /// <summary>
        /// Convert the System.String representation of a System.TimeSpan to its System.TimeSpan
        /// equivalent. If not a valid string representation of a TimeSpan, returns a new instance of
        /// System.TimeSpan. The string can be delimited by ':' or '.', e.g. 1:2:3:4 returns a
        /// TimeSpan valued 1 day, 2 hours, 3 minutes, 4 seconds; 1:2:3 returns a TimeSpan valued 1
        /// hour, 2 minutes, 3 seconds; 1:2 returns a TimeSpan valued 1 hour, 2 minutes, 0 seconds; 1
        /// returns a TimeSpan valued 1 hour, 0 minutes, 0 seconds.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return new TimeSpan();
            string[] s;
            if (a.Contains(":"))
                s = a.Split(':');
            else if (a.Contains("."))
                s = a.Split('.');
            else
                return new TimeSpan();

            if (s.Length == 4) // days:hours:minutes:seconds
                return new TimeSpan(s[0].ToInt32(), s[1].ToInt32(), s[2].ToInt32(), s[3].ToInt32());
            else if (s.Length == 3) // hours:minutes:seconds
                return new TimeSpan(s[0].ToInt32(), s[1].ToInt32(), s[2].ToInt32());
            else if (s.Length == 2) // hours:minutes:seconds
                return new TimeSpan(s[0].ToInt32(), s[1].ToInt32(), 0);
            else // hours:minutes:seconds
                return new TimeSpan(s[0].ToInt32(), 0, 0);
        }

        /// <summary>
        /// Converts the specified string to titlecase.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string a)
        {
            if (string.IsNullOrWhiteSpace(a))
                return string.Empty;
            TextInfo ti = Thread.CurrentThread.CurrentCulture.TextInfo;
            return ti.ToTitleCase(a);
        }

        /// <summary>
        /// Returns the XML formatted string as an XML formatted string with indenting. Returns an
        /// empty string if the XML string is empty or null. Returns the original string if the
        /// string is not a valid XML formatted string.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ToXmlFormattedString(this string a)
        {
            if (string.IsNullOrEmpty(a))
                return string.Empty;

            string xml = a;
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(a);
                using (Stream stream = new MemoryStream(xmlDocument.OuterXml.ToArray(Encoding.Default)))
                {
                    using (Stream stream2 = new MemoryStream())
                    {
                        using (XmlTextWriter writer = new XmlTextWriter(stream2, Encoding.Default))
                        {
                            writer.Formatting = Formatting.Indented;
                            xmlDocument.Save(writer);
                            xml = stream2.ToArray().GetString();
                        }
                    }
                }
            }
            catch
            {
            }
            return xml;
        }

        public static string TrimNewLine(this string a)
        {
            return string.IsNullOrEmpty(a) ? string.Empty : a.Trim().Trim(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Returns the string truncated to the specified maximum length. If the string's length is
        /// less than or equal to the specified maximum length, the original string is returned. No
        /// trimming is incorporated.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string TruncateToMaxLength(this string a, int length)
        {
            if (string.IsNullOrEmpty(a) || a.Length <= length)
                return a;
            return a.Substring(0, length);
        }

        public static DateTime? TryParseRFC1123DateTime(this string a)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string pattern = @"[a-zA-Z]+, [0-9]+ [a-zA-Z]+ [0-9]+ [0-9]+:[0-9]+:[0-9]+ (?<timezone>[a-zA-Z]+)";

            Regex findTz = new Regex(pattern, RegexOptions.Compiled);

            string tz = findTz.Match(a).Result("${timezone}");

            string format = "ddd, dd MMM yyyy HH:mm:ss " + tz;

            bool ok = DateTime.TryParseExact(a, format, provider, DateTimeStyles.None, out DateTime dt);
            if (!ok)
            {
                format = "ddd, dd MMMM yyyy HH:mm:ss " + tz;
                ok = DateTime.TryParseExact(a, format, provider, DateTimeStyles.None, out dt);
            }
            return ok ? new DateTime?(dt) : null;
        }

        public static DateTime? TryParseToDateTime(this string a)
        {
            bool ok = DateTime.TryParse(a, out DateTime d);
            if (ok)
                return d;
            else
                return null;
        }

        /// <summary>
        /// Try trim the specified string. If the string is null or empty, return empty string;
        /// otherwise trimmed string.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string TryTrim(this string a)
        {
            return string.IsNullOrEmpty(a) ? string.Empty : a.Trim();
        }

        public static string UriEscapeDataString(this string a)
        {
            return Uri.EscapeDataString(a);
        }

        public static string UrlDecode(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : System.Web.HttpUtility.UrlDecode(a);
        }

        public static string UrlEncode(this string a)
        {
            return string.IsNullOrWhiteSpace(a) ? string.Empty : System.Web.HttpUtility.UrlEncode(a);
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetSoundexCategory(char value)
        {
            string s = value.ToString(CultureInfo.InvariantCulture);

            string[] c = new string[] { "AEIOUY", "BPFV", "CSKGJQXZ", "DT", "L", "MN", "R" };

            for (int i = c.GetLowerBound(0); i <= c.GetUpperBound(0); i++)
                if (c[i].Contains(s))
                    return i.ToString(CultureInfo.InvariantCulture);

            return string.Empty;
        }

        private static Size MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return new Size(0, 0);
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };

            format.SetMeasurableCharacterRanges(ranges);

            Region[] regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);
            int h = (int)(rect.Bottom + 1.0f); // height
            int w = (int)(rect.Right + 1.0f); // width
            return new Size(w, h);
        }

        private static string StripCultureAwareNumberFormatInfoStrings(string value)
        {
            return value.Replace(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, "").Replace(NumberFormatInfo.CurrentInfo.NegativeSign, "").Replace(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, "").Replace(NumberFormatInfo.CurrentInfo.PercentDecimalSeparator, "");
        }

        private static string StripCultureAwareStrings(string value)
        {
            return value.Replace(NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator, "").Replace(NumberFormatInfo.CurrentInfo.CurrencySymbol, "").Replace(NumberFormatInfo.CurrentInfo.NegativeInfinitySymbol, "").Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, "").Replace(NumberFormatInfo.CurrentInfo.PercentGroupSeparator, "").Replace(NumberFormatInfo.CurrentInfo.PercentSymbol, "").Replace(NumberFormatInfo.CurrentInfo.PerMilleSymbol, "").Replace(NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol, "").Replace(NumberFormatInfo.CurrentInfo.PositiveSign, "").Trim();
        }

        private static bool VerifySyntaxLegality(string value, char[] invalidChars)
        {
            bool ok = true;
            if (!string.IsNullOrEmpty(value))
            {
                foreach (char c in invalidChars)
                    if (value.Contains(c.ToString(CultureInfo.InvariantCulture)))
                    {
                        ok = false;
                        break;
                    }
            }
            else
                ok = false;

            return ok;
        }

        #endregion Private Methods
    }
}