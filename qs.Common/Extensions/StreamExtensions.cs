namespace qs.Extensions.StreamExtensions
{
    using qs.Extensions.Int64Extensions;
    using System.Collections.Generic;
    using System.IO;

    public static class StreamExtensions
    {
        #region Methods

        /// <summary>
        /// Read a sequence of bytes from the current stream starting at the specified offset for the specified count and returns the bytes read as an array of bytes. Only the bytes read are returned. If the offset is less than zero, then the offset is forced to zero. If the offset is equal to or greater than the stream length, then the offset is forced to the stream length -1.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset">The zero-based byte offset to begin reading the stream.</param>
        /// <param name="count">The maximum number of bytes to read from the current stream.</param>
        /// <returns>The stream as an array of bytes.</returns>
        public static byte[] ToArray(this Stream stream, long offset, long count)
        {
            List<byte> b = new List<byte>();
            BinaryReader r = new BinaryReader(stream);
            long l = r.BaseStream.Length;
            long s = offset.ToAbs();
            if (s >= l)
                s = l - 1;
            long e = s + count.ToAbs() - 1;
            if (e > l)
                e = l - 1;
            stream.Position = s;
            while (stream.Position <= e)
                b.Add(r.ReadByte());
            return b.ToArray();
        }

        /// <summary>
        /// Read from the current stream and returns the stream as an array of bytes.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The stream as an array of bytes.</returns>
        public static byte[] ToArray(this Stream stream)
        {
            long l = stream.Length;
            return stream.ToArray(0, l);
        }

        #endregion Methods
    }
}