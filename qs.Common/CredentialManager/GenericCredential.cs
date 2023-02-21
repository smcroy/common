using System;
using System.Security;

namespace qs.CredentialManager
{
    /// <summary>
    /// This is a generic credential definition.
    /// </summary>
    [Serializable]
    public class GenericCredential
    {
        /// <summary>
        /// Returns a string representation of the generic credential.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", InternetOrNeworkAddress, UserName);
        }

        internal GenericCredential(string userName, SecureString password, DateTime lastModified, string internetOrNeworkAddress, string persistence, string type)
        {
            UserName = userName;
            Password = password;
            LastModified = lastModified;
            InternetOrNeworkAddress = internetOrNeworkAddress;
            Persistence = persistence;
            Type = type;
        }

        /// <summary>
        /// Gets the type of the credential. For a GenericCredential this returns 'Generic'.
        /// </summary>
        public string Type
        {
            get; private set;
        }

        /// <summary>
        /// Gets the internet or network address (the name) by which the credential is known.
        /// </summary>
        public string InternetOrNeworkAddress
        {
            get; private set;
        }

        /// <summary>
        /// Gets the password as a secure string.
        /// </summary>
        public SecureString Password
        {
            get; private set;
        }

        /// <summary>
        /// Gets the associated user name.
        /// </summary>
        public string UserName
        {
            get; private set;
        }

        /// <summary>
        /// Gets the last date and time the credential was modified.
        /// </summary>
        public DateTime LastModified
        {
            get; private set;
        }

        /// <summary>
        /// Gets the credential defined persistence.
        /// </summary>
        public string Persistence
        {
            get; private set;
        }
    }
}