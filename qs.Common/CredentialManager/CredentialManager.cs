using qs.Extensions.SecureStringExtensions;
using qs.Extensions.StringExtensions;
using System;
using System.Security;

/// <summary>
/// Windows Credential Manager lets you view and delete saved credentials for signing into websites, connected applications, and networks. The Windows Credential Manager can manage web credentials and Windows credentials. The CredentialManager static class facilitates management of Windows Credentials -&gt; Generic Credentials.
/// </summary>
namespace qs.CredentialManager
{
    /// <summary>
    /// This facilitates working with the Windows Credential Manager.
    ///
    /// This demonstrates how CredentialManager is utilized. When a credential is added successfully, the credential is returned in the ReturnStatus.
    /// <code>
    /// if ( !CredentialManager.GenericCredentialExists( "ABC" ))
    /// {
    ///     ReturnStatus&lt;GenericCredential&gt; rsNewCred = CredentialManager.AddGenericCredential( "ABC", "useraccount", "password" );
    /// }
    ///
    /// ReturnStatus&lt;GenericCredential&gt; rs = CredentialManager.GetGenericCredential( "ABC" );
    /// if (rs.IsOk )
    /// {
    ///     string username = rs.Value.UserName;
    ///     System.Security.SecureString password = rs.Value.Password;
    /// }
    ///
    /// CredentialManager.DeleteGenericCredential( "abc" )
    /// </code>
    /// </summary>
    public partial class CredentialManager
    {
        /// <summary>
        /// Delete a generic credential from Windows Credential Manager.
        /// Returns an instance of type ReturnStatus&lt;GenericCredential&gt;
        /// <para></para>
        /// If successful, return status IsOk = true and Value = the generic credential prior to removal.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of GenericCredential.</returns>
        public static ReturnStatus<GenericCredential> DeleteGenericCredential(string internetOrNeworkAddress)
        {
            ReturnStatus<GenericCredential> r = GetGenericCredential(internetOrNeworkAddress);
            bool ok = r.IsOk;
            if (ok)
            {
                int i = CredDelete(internetOrNeworkAddress, CredentialType.Generic);
                ok = i == 0;
            }
            return new ReturnStatus<GenericCredential>(ok, ok ? r.Value : null);
        }

        /// <summary>
        /// Adds a generic credential to Windows Credential Manager.
        /// Returns an instance of type ReturnStatus&lt;GenericCredential&gt;
        /// <para></para>
        /// If successful, return status IsOk = true and Value = the added generic credential.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <param name="userName">Specifies the user name to assign to this generic credential.</param>
        /// <param name="password">Specifies the string to assign as the password to this generic credential.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of GenericCredential.</returns>
        public static ReturnStatus<GenericCredential> AddGenericCredential(string internetOrNeworkAddress, string userName, string password)
        {
            return AddGenericCredential(internetOrNeworkAddress, userName, password.ToSecureString());
        }

        /// <summary>
        /// Determine if the named generic credential exists.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <returns>Returns true if the credential exists; otherwise false;</returns>
        public static bool GenericCredentialExists(string internetOrNeworkAddress)
        {
            ReturnStatus<GenericCredential> r = GetGenericCredential(internetOrNeworkAddress);
            return r.IsOk;
        }

        /// <summary>
        /// Adds a generic credential to Windows Credential Manager.
        /// Returns an instance of type ReturnStatus&lt;GenericCredential&gt;
        /// <para></para>
        /// If successful, return status IsOk = true and Value = the added generic credential. If the credential already exists, return status IsOk = false, and an error indicating the credential already exists.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <param name="userName">Specifies the user name to assign to this generic credential.</param>
        /// <param name="password">Specifies the secure string to assign as the password to this generic credential.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of GenericCredential.</returns>
        public static ReturnStatus<GenericCredential> AddGenericCredential(string internetOrNeworkAddress, string userName, SecureString password)
        {
            bool ok = false;
            Exception exc = null;
            GenericCredential credential = null;

            ReturnStatus<GenericCredential> r = GetGenericCredential(internetOrNeworkAddress);
            if (r.IsOk)
            {
                ok = false;
                exc = new Exception("Named generic credential already exists");
            }
            else
            {
                Credential c = new Credential
                {
                    AttributeCount = 0,
                    Flags = 0,
                    Password = password.ToUnsecuredString(),
                    PasswordSize = (uint)(password.Length * 2),
                    Persistence = CredentialPersistance.Enterprise,
                    TargetName = internetOrNeworkAddress,
                    Type = CredentialType.Generic,
                    UserName = userName
                };
                int i = CredWrite(c);
                ok = i == 0;
                if (ok)
                {
                    r = GetGenericCredential(internetOrNeworkAddress);
                    ok = r.IsOk;
                    if (ok)
                        credential = r.Value;
                }
            }
            return new ReturnStatus<GenericCredential>(ok, exc, credential);
        }

        /// <summary>
        /// Updates a generic credential to Windows Credential Manager.
        /// Returns an instance of type ReturnStatus&lt;GenericCredential&gt;
        /// <para></para>
        /// If successful, return status IsOk = true and Value = the updated generic credential. If the credential does not exist, return status IsOk = false, and an error indicating the credential does not exist.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <param name="userName">Specifies the user name to assign to this generic credential.</param>
        /// <param name="password">Specifies the secure string to assign as the password to this generic credential.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of GenericCredential.</returns>
        public static ReturnStatus<GenericCredential> UpdateGenericCredential(string internetOrNeworkAddress, string userName, SecureString password)
        {
            bool ok = false;
            Exception exc = null;
            GenericCredential credential = null;

            ReturnStatus<GenericCredential> r = GetGenericCredential(internetOrNeworkAddress);
            if (r.IsOk)
            {
                Credential c = new Credential
                {
                    AttributeCount = 0,
                    Flags = 0,
                    Password = password.ToUnsecuredString(),
                    PasswordSize = (uint)(password.Length * 2),
                    Persistence = CredentialPersistance.Enterprise,
                    TargetName = internetOrNeworkAddress,
                    Type = CredentialType.Generic,
                    UserName = userName
                };
                int i = CredWrite(c);
                ok = i == 0;
                if (ok)
                {
                    r = GetGenericCredential(internetOrNeworkAddress);
                    ok = r.IsOk;
                    if (ok)
                        credential = r.Value;
                }
            }
            else
            {
                ok = false;
                exc = new Exception("Named generic credential does not exist");
            }
            return new ReturnStatus<GenericCredential>(ok, exc, credential);
        }

        /// <summary>
        /// Get the generic credential from Windows Credential Manager specified by the Internet or network address.
        /// Returns an instance of type ReturnStatus&lt;GenericCredential&gt;
        /// <para></para>
        /// If successful, return status IsOk = true and Value = the retrieved generic credential.
        /// </summary>
        /// <param name="internetOrNeworkAddress">Specifies the name or address by which the credential is known. The name is not case sensitive.</param>
        /// <returns>Returns an instance of ReturnStatus with a value type of GenericCredential.</returns>
        public static ReturnStatus<GenericCredential> GetGenericCredential(string internetOrNeworkAddress)
        {
            int i = CredRead(internetOrNeworkAddress, CredentialType.Generic, out Credential c);

            bool ok = i == 0;
            GenericCredential credential = null;
            if (ok)
                credential = new GenericCredential(c.UserName, c.Password.ToSecureString(), c.LastWritten, c.TargetName, c.Persistence.ToString(), c.Type.ToString());
            return new ReturnStatus<GenericCredential>(ok, credential);
        }
    }
}