namespace qs.Extensions.UriExtensions
{
    using System;

    public static class UriExtensions
    {
        #region Methods

        /// <summary>
        /// Returns the schme, authority and the absolute path as a string.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetAuthorityAbsolutePath(this Uri uri)
        {
            return string.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.AbsolutePath);
        }

        #endregion Methods
    }
}