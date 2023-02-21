namespace qs.Extensions.AssemblyExtensions
{
    using qs.Extensions.StreamExtensions;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    public static class AssemblyExtensions
    {
        #region Methods

        /// <summary>
        /// Returns the full qualified name of the specified resource if the resource exists within this assembly; otherwise returns an empty string.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string GetFullyQualifiedManifestResourceName(this Assembly assembly, string resourceName)
        {
            var v = assembly.GetManifestResourceNames();
            string fqResourceName = string.Empty;
            foreach (string s in v)
            {
                if (s.EndsWith(string.Format(".{0}", resourceName), StringComparison.OrdinalIgnoreCase) ||
                    s.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
                {
                    fqResourceName = s;
                    break;
                }
            }
            return fqResourceName;
        }

        /// <summary>
        /// This extension method returns the resource as a byte array if the resource exists within this assembly; otherwise returns null.
        /// </summary>
        /// <param name="assembly">Specifies this assembly.</param>
        /// <param name="resourceName">Specifies a resource name as defined within the assembly.</param>
        /// <returns>the resource as a byte array if the resource exists; otherwise returns null</returns>
        public static byte[] GetManifestResourceAsByteArray(this Assembly assembly, string resourceName)
        {
            byte[] value = null;
            string fqResourceName = assembly.GetFullyQualifiedManifestResourceName(resourceName);
            if (!string.IsNullOrEmpty(fqResourceName))
            {
                using (Stream stream = assembly.GetManifestResourceStream(fqResourceName))
                {
                    value = stream.ToArray();
                }
            }
            return value;
        }

        /// <summary>
        /// Returns the resource as a image if the resource exists within this assembly; otherwise returns null.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Image GetManifestResourceAsImage(this Assembly assembly, string resourceName)
        {
            Bitmap value = null;
            string fqResourceName = assembly.GetFullyQualifiedManifestResourceName(resourceName);
            if (!string.IsNullOrEmpty(fqResourceName))
            {
                using (Stream stream = assembly.GetManifestResourceStream(fqResourceName))
                {
                    value = new Bitmap(stream);
                }
            }
            return value;
        }

        /// <summary>
        /// Returns the specified resource as a string if the resource exists within this assembly; otherwise returns an empty string.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string GetManifestResourceAsString(this Assembly assembly, string resourceName)
        {
            string value = string.Empty;
            string fqResourceName = assembly.GetFullyQualifiedManifestResourceName(resourceName);
            if (!string.IsNullOrEmpty(fqResourceName))
            {
                using (TextReader reader = new StreamReader(assembly.GetManifestResourceStream(fqResourceName)))
                {
                    value = reader.ReadToEnd();
                    reader.Close();
                }
            }
            return value;
        }

        /// <summary>
        /// Returns true if the specified resource exists in this assembly; otherwise false.
        /// </summary>
        /// <param name="asssembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static bool ManifestResourceExists(this Assembly asssembly, string resourceName)
        {
            return string.IsNullOrEmpty(asssembly.GetFullyQualifiedManifestResourceName(resourceName)) ? false : true;
        }

        #endregion Methods
    }
}