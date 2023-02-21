using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace qs.Extensions.ByteExtensions
{
    /// <summary>
    /// These are byte extension methods.
    /// </summary>
    public static class ByteExtensions
    {
        #region Methods

        /// <summary>
        /// Compress this byte array and returns a compressed byte array.
        /// </summary>
        /// <param name="buffer">The byte array to compress using GZipStream compression.</param>
        /// <returns>Returns a compressed byte array.</returns>
        public static byte[] Compress(this byte[] buffer)
        {
            byte[] gzBuffer;
            byte[] compressed;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                    zip.Close();
                }
                ms.Position = 0;

                compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
            }

            gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;
        }

        /// <summary>
        /// Decompress this byte array (compressed using GZipStream compression) and returns a decompressed byte array.
        /// </summary>
        /// <param name="gzBuffer">The compressed byte array compressed as a GZipStream.</param>
        /// <returns>Returns the byte array decompressed.</returns>
        public static byte[] Decompress(this byte[] gzBuffer)
        {
            byte[] buffer;

            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Returns true if this byte array is equal to a specified byte array; otherwise false;
        /// </summary>
        /// <param name="a">The byte array which is used for base comparison.</param>
        /// <param name="byteArray">Specifies the byte array to compare.</param>
        /// <returns>Returns true if the comparison is equal; otherwise false.</returns>
        public static bool EqualsByteArray(this byte[] a, byte[] byteArray)
        {
            bool ok = true;
            if (a.Length.Equals(byteArray.Length))
            {
                for (int i = 0; i < a.Length; i++)
                {
                    ok = a[i].CompareTo(byteArray[i]) == 0 ? true : false;
                    if (!ok)
                        break;
                }
            }
            else
                ok = false;
            return ok;
        }

        /// <summary>
        /// Decodes all the bytes in the specified byte array into a string. The encoding for the operating system's current ANSI code page is utilized to decode the specified bytes.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <returns>Returns a string containing the results of decoding the specified sequence of bytes.</returns>
        public static string GetString(this byte[] bytes)
        {
            return bytes.GetString(System.Text.Encoding.Default);
        }

        /// <summary>
        /// Decodes all the bytes in the specified byte array into a string.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="encoding">The encoding to utilize to decode the specified bytes.</param>
        /// <returns>Returns a string containing the results of decoding the specified sequence of bytes.</returns>
        public static string GetString(this byte[] bytes, System.Text.Encoding encoding)
        {
            if (!(encoding is System.Text.Encoding e))
                e = System.Text.Encoding.Default;
            return e.GetString(bytes);
        }

        /// <summary>
        /// Returns the byte array as a base 64 string.
        /// </summary>
        /// <param name="bytes">The byte array to convert to a base 64 string.</param>
        /// <returns>Returns a base 64 string representation of the byte array.</returns>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Returns the byte array as an image.
        /// </summary>
        /// <param name="bytes">The byte array representing an image.</param>
        /// <returns>Returns an image.</returns>
        public static Image ToImage(this byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return Image.FromStream(stream);
            }
        }

        #endregion Methods
    }
}