namespace qs.Extensions.FileStreamExtensions
{
    using qs.Extensions.StringExtensions;
    using System.IO;

    public static class FileStreamExtensions
    {
        #region Methods

        /// <summary>
        /// Clears all buffers for this instance of type FileStream and causes any buffered data to be written to the file system.
        /// Closes the current instance of type FileStream and releases the associated file handle.
        /// </summary>
        /// <param name="stream"></param>
        public static void SafeClose(this FileStream stream)
        {
            try
            {
                stream.Flush();
                stream.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public static void Write(this FileStream stream, string value, int offset, int count)
        {
            if (!string.IsNullOrEmpty(value))
            {
                byte[] b = value.ToArray();
                int o = offset;
                if (o > b.Length)
                    o = 0;
                int c = count;
                if (o + count > b.Length)
                    c = b.Length;
                stream.Write(b, o, c);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this FileStream stream, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                byte[] b = value.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this FileStream stream, string format, params object[] args)
        {
            if (args.Length > 0 && !string.IsNullOrEmpty(format))
            {
                string s = string.Format(format, args);
                byte[] b = s.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void Write(this FileStream stream, string format, object arg)
        {
            if (!string.IsNullOrEmpty(format))
            {
                string s = string.Format(format, arg);
                byte[] b = s.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public static void WriteLine(this FileStream stream, string value, int offset, int count)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string s = string.Format("{0}\r\n", value);
                byte[] b = s.ToArray();
                int o = offset;
                if (o > b.Length)
                    o = 0;
                int c = count;
                if (o + count > b.Length)
                    c = b.Length;
                stream.Write(b, o, c);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteLine(this FileStream stream, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string s = string.Format("{0}\r\n", value);
                byte[] b = s.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteLine(this FileStream stream, string format, params object[] args)
        {
            if (args.Length > 0 && !string.IsNullOrEmpty(format))
            {
                string s = string.Format("{0}\r\n", string.Format(format, args));
                byte[] b = s.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Writes the value to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public static void WriteLine(this FileStream stream, string format, object arg)
        {
            if (!string.IsNullOrEmpty(format))
            {
                string s = string.Format("{0}\r\n", string.Format(format, arg));
                byte[] b = s.ToArray();
                stream.Write(b, 0, b.Length);
            }
        }

        /// <summary>
        /// Write a carriage return line feed to this instance of type FileStream.
        /// </summary>
        /// <param name="stream"></param>
        public static void WriteLine(this FileStream stream)
        {
            string s = "\r\n";
            byte[] b = s.ToArray();
            stream.Write(b, 0, b.Length);
        }

        #endregion Methods
    }
}