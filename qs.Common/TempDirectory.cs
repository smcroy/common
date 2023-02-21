using System.IO;

namespace qs
{
    /// <summary>
    /// Static class to assist with the creation and deletion of temporary directories.
    /// </summary>
    public static class TempDirectory
    {
        /// <summary>
        /// Returns the path to the newly added temporary directory. The temporary directory is added as a subfolder of the user's temporary directory.
        /// </summary>
        /// <returns>Returns the full path of the temporary directory.</returns>
        public static string AddTempDirectory()
        {
            string d = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            DirectoryInfo di = Directory.CreateDirectory(d);
            return di.FullName;
        }

        /// <summary>
        /// Removes the specified directory and any subdirectories and files in the directory.
        /// The directory is removed only if it is a subdirectory of the user's temporary directory.
        /// </summary>
        /// <param name="path">Specifies the full path of the temporary directory.</param>
        /// <returns>Returns true if the directory exists, is a subdirectory of the user's temporary directory, and was successfully removed; otherwise false.</returns>
        public static bool RemoveTempDirectory(string path)
        {
            bool ok = false;
            if (!string.IsNullOrEmpty(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists && di.FullName.StartsWith(Path.GetTempPath()))
                {
                    try
                    {
                        Directory.Delete(path, true);
                        ok = true;
                    }
                    catch
                    {
                    }
                }
            }

            return ok;
        }
    }
}