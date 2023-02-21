using System;

namespace qs.Extensions.SecureStringExtensions
{
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringExtensions
    {
        /// <summary>
        /// Returns the value of an instance of System.Security.SecureString as an instance of type System.String.
        /// </summary>
        /// <param name="a">An instance of type System.Security.SecureString.</param>
        /// <returns>The value of the instance of System.Security.SecureString as an instance of type System.String.</returns>
        public static string ToUnsecuredString(this SecureString a)
        {
            if (a == null)
                return string.Empty;

            IntPtr i = Marshal.SecureStringToBSTR(a);
            string s = Marshal.PtrToStringBSTR(i);
            Marshal.ZeroFreeBSTR(i);
            return s;
        }
    }
}