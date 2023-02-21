namespace qs
{
    using Microsoft.Win32.SafeHandles;
    using qs.Extensions.DoubleExtensions;
    using qs.Extensions.ExceptionExtensions;
    using qs.Extensions.SecureStringExtensions;
    using qs.Extensions.StringExtensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Authentication;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;
    using static qs.ElevatedSecurity;

    /// http://www.pinvoke.net/default.aspx/advapi32/LogonUser.html
    /// <summary>
    /// Provides the ability to run a delegated method as an impersonated user.
    /// </summary>
    public class ElevatedSecurity
    {
        #region Fields

        const int ERROR_IO_PENDING = 997;
        const int ERROR_UNKNOWNUSER = 1326;
        const int RETRYCOUNTTHRESHOLD = 8;
        const int RETRYWAIT = 1000; // milliseconds
        const int SUCCESS = 0;

        #endregion Fields

        #region Enumerations

        public enum LogonType
        {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
            /// </summary>
            LOGON32_LOGON_INTERACTIVE = 2, // LOGON32_LOGON_INTERACTIVE
            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords. Credentials are not cached for this logon type.
            /// </summary>
            LOGON32_LOGON_NETWORK = 3, // LOGON32_LOGON_NETWORK
            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers. Credentials are not cached for this logon type.
            /// </summary>
            LOGON32_LOGON_BATCH = 4, // LOGON32_LOGON_BATCH
            /// <summary>
            /// This is a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            LOGON32_LOGON_SERVICE = 5, // LOGON32_LOGON_SERVICE
            /// <summary>
            /// This logon type is for GINA DLLs that log on users who will be interactively using the computer. This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </summary>
            LOGON32_LOGON_UNLOCK = 7, // LOGON32_LOGON_UNLOCK
            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make connections to other network servers while impersonating the client. A server can accept plaintext credentials from a client, call LogonUser, verify that the user can access the system across the network, and still communicate with other servers. NOTE: Windows NT: This value is not supported.
            /// </summary>
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // LOGON32_LOGON_NETWORK_CLEARTEXT
            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections. The new logon session has the same local identifier but uses different credentials for other network connections. NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider. NOTE: Windows NT:  This value is not supported.
            /// </summary>
            LOGON32_LOGON_NEW_CREDENTIALS = 9 // LOGON32_LOGON_NEW_CREDENTIALS
        }

        #endregion Enumerations

        #region Delegates

        public delegate void RunAsDelegate( );

        #endregion Delegates

        #region Methods

        public static LogonType? ParseForLogonType( string value )
        {
            LogonType? type = null;
            if ( !string.IsNullOrEmpty( value ) )
                try
                {
                    type = ( LogonType? ) Enum.Parse( typeof( LogonType ), value, true );
                }
                catch ( Exception e )
                {
                    e.IgnoreError( );
                }
            return type;
        }

        public static LogonType? ParseForLogonType( int value )
        {
            Array a = Enum.GetValues( typeof( LogonType ) );
            foreach ( LogonType t in a )
            {
                if ( value == ( int ) t )
                    return t;
            }
            return null;
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">Fully qualified user name to impersonate (ex. domain\username).</param>
        /// <param name="password">User's password.</param>
        /// <param name="logontype">Logon type.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string password, LogonType logontype )
        {
            User u = new User( username );
            RunAs( methodToRunAs, u.Name, u.Domain, password.ToSecureString( ), logontype );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username. Logon type defaulted to LOGON32_LOGON_NETWORK_CLEARTEXT.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">Fully qualified user name to impersonate (ex. domain\username).</param>
        /// <param name="password">User's password.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string password )
        {
            RunAs( methodToRunAs, username, password, LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">User name to impersonate.</param>
        /// <param name="domain">Domain in which the user is hosted.</param>
        /// <param name="password">User's password.</param>
        /// <param name="logontype">Logon type.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string domain, string password, LogonType logontype )
        {
            User u = new User( username, domain );
            RunAs( methodToRunAs, u.Name, u.Domain, password.ToSecureString( ), logontype );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username. Logon type defaulted to LOGON32_LOGON_NETWORK_CLEARTEXT.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">User name to impersonate.</param>
        /// <param name="domain">Domain in which the user is hosted.</param>
        /// <param name="password">User's password.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string domain, string password )
        {
            RunAs( methodToRunAs, username, domain, password, LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">User name to impersonate.</param>
        /// <param name="domain">Domain in which the user is hosted.</param>
        /// <param name="password">User's password.</param>
        /// <param name="logontype">Logon type.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string domain, SecureString password, LogonType logontype )
        {
            User u = new User( username, domain );
            ElevatedSecurity o = new ElevatedSecurity( );
            o.runCoordinator( methodToRunAs, u.Name, u.Domain, password, logontype );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username. Logon type defaulted to LOGON32_LOGON_NETWORK_CLEARTEXT.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">User name to impersonate.</param>
        /// <param name="domain">Domain in which the user is hosted.</param>
        /// <param name="password">User's password.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, string domain, SecureString password )
        {
            RunAs( methodToRunAs, username, domain, password, LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">Fully qualified user name to impersonate (ex. domain\username).</param>
        /// <param name="password">User's password.</param>
        /// <param name="logontype">Logon type.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, SecureString password, LogonType logontype )
        {
            User u = new User( username );
            RunAs( methodToRunAs, u.Name, u.Domain, password, logontype );
        }

        /// <summary>
        /// Run the specified method as the impersonated user specified by username. Logon type defaulted to LOGON32_LOGON_NETWORK_CLEARTEXT.
        /// </summary>
        /// <param name="methodToRunAs">Delegated method to run.</param>
        /// <param name="username">Fully qualified user name to impersonate (ex. domain\username).</param>
        /// <param name="password">User's password.</param>
        /// <exception cref="Win32Exception">Thrown when the impersonation attempt fails.</exception>
        public static void RunAs( RunAsDelegate methodToRunAs, string username, SecureString password )
        {
            RunAs( methodToRunAs, username, password, LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT );
        }

        public static bool Validate( string username, string domain, string password, LogonType logontype )
        {
            bool ok = true;
            try
            {
                RunAs( delegate ( )
                {
                    WindowsIdentity user = WindowsIdentity.GetCurrent( );
                }, username, domain, password, logontype );
            }
            catch
            {
                ok = false;
            }

            return ok;
        }

        [DllImport( "advapi32.dll", SetLastError = true )]
        private static extern bool LogonUser( string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken );

        string getErrorMessage( int errorCode )
        {
            string m = string.Empty;
            switch ( errorCode )
            {
                case 997: //ERROR_IO_PENDING
                    m = "Overlapped I/O operation is in progress. (See: http://msdn.microsoft.com/en-us/library/ms681388(v=VS.85).aspx)";
                    break;
                case 1326: //ERROR_LOGON_FAILURE
                    m = "Unknown user name or bad password. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1327: //ERROR_ACCOUNT_RESTRICTION
                    m = "User account restriction. Possible reasons are blank passwords not allowed, logon hour restrictions, or a policy restriction has been enforced. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1328: //ERROR_INVALID_LOGON_HOURS
                    m = "Account logon time restriction violation. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1329: //ERROR_INVALID_WORKSTATION
                    m = "User not allowed to log on to this computer. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1330: //ERROR_PASSWORD_EXPIRED
                    m = "The specified account password has expired. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1331: //ERROR_ACCOUNT_DISABLED
                    m = "Account currently disabled. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1380: //ERROR_LOGON_NOT_GRANTED
                    m = "The user has not been granted the requested logon type at this computer. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1385: //ERROR_LOGON_TYPE_NOT_GRANTED
                    m = "The user has not been granted the requested logon type at this computer. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                case 1396: //ERROR_WRONG_TARGET_NAME
                    m = "The target account name is incorrect. (See: http://msdn.microsoft.com/en-us/library/ms681385(v=VS.85).aspx)";
                    break;
                default:
                    break;
            }
            string n = string.Format( "NativeErrorCode: {0}", errorCode );
            return string.IsNullOrEmpty( m ) ? n : string.Format( "{0} Logon failure: {1}", n, m );
        }

        Status runas (RunAsDelegate methodToRunAs, string username,string domain, SecureString password, LogonType logonType)
        {
            int errorCode = SUCCESS;
            string errorMessage = string.Empty;

            const int LOGON32_PROVIDER_DEFAULT = 0;
            SafeTokenHandle safeTokenHandle = null;

            // obtain handle to an access token
            if (LogonUser(username, string.IsNullOrEmpty(domain) ? "." : domain, password.ToUnsecuredString(), (int)logonType, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle))
            {
                // get the token handle of impersonated user
                WindowsIdentity windowsIdentity = new WindowsIdentity(safeTokenHandle.DangerousGetHandle());
                using (windowsIdentity.i
            }
        }
        Status DEPRECATED_runas( RunAsDelegate methodToRunAs, string username, string domain, SecureString password, LogonType logontype )
        {
            int errorCode = SUCCESS;
            string errorMessage = string.Empty;

            const int LOGON32_PROVIDER_DEFAULT = 0;
            //const int LOGON32_LOGON_INTERACTIVE = 2;
            //const int LOGON32_LOGON_NETWORK = 3;
            //const int LOGON32_LOGON_BATCH = 4;

            // See: http://msdn.microsoft.com/en-us/library/aa378184(VS.85).aspx
            // The LogonUser function attempts to log a user on to the local computer. The local computer is the computer from which LogonUser was called. You cannot use LogonUser to log on to a remote computer. You specify the user with a user name and domain and authenticate the user with a plaintext password. If the function succeeds, you receive a handle to a token that represents the logged-on user. You can then use this token handle to impersonate the specified user or, in most cases, to create a process that runs in the context of the specified user.
            // Parameters:
            //  lpszUsername [in] = A pointer to a null-terminated string that specifies the name of the user. This is the name of the user account to log on to. If you use the user principal name (UPN) format, User@DNSDomainName, the lpszDomain parameter must be NULL.
            //  lpszDomain [in, optional] = A pointer to a null-terminated string that specifies the name of the domain or server whose account database contains the lpszUsername account. If this parameter is NULL, the user name must be specified in UPN format. If this parameter is ".", the function validates the account by using only the local account database.
            //  lpszPassword [in] = A pointer to a null-terminated string that specifies the plaintext password for the user account specified by lpszUsername.
            //  dwLogonType [in] = The type of logon operation to perform.
            //    LOGON32_LOGON_BATCH = This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers.
            //    LOGON32_LOGON_INTERACTIVE = This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
            //    LOGON32_LOGON_NETWORK = This logon type is intended for high performance servers to authenticate plaintext passwords. The LogonUser function does not cache credentials for this logon type.
            //  dwLogonProvider [in] = Specifies the logon provider.
            //    LOGON32_PROVIDER_DEFAULT = Use the standard logon provider for the system. The default security provider is negotiate, unless you pass NULL for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
            //  phToken [out] = A pointer to a handle variable that receives a handle to a token that represents the specified user.
            try
            {
                SafeTokenHandle safeTokenHandle = null;

                // obtain handle to an access token
                if ( LogonUser( username, string.IsNullOrEmpty( domain ) ? "." : domain, password.ToUnsecuredString( ), ( int ) logontype, LOGON32_PROVIDER_DEFAULT, out safeTokenHandle ) )
                {
                    // get the token handle of impersonated user
                    WindowsIdentity windowsIdentity = new WindowsIdentity( safeTokenHandle.DangerousGetHandle( ) );
                    // releasing the context object stops the impersonation
                    using ( WindowsImpersonationContext impersonatedUser = windowsIdentity.Impersonate( ) )
                    {
                        try
                        {
                            methodToRunAs( );
                        }
                        catch // catch any error in the method that is executed and rethrow
                        {
                            throw;
                        }
                    };
                }
                else
                {
                    errorCode = Marshal.GetLastWin32Error( );
                    //throw new Win32Exception( errorCode, string.Format( "Failed. Could not impersonate user '{0}' in domain '{1}' with the specified password. {2}", username.ToLower( ), domain.ToLower( ), getErrorMessage( errorCode ) ) );
                    errorMessage = string.Format( "Failed. Could not impersonate user '{0}' in domain '{1}' with the specified password. {2}", username.ToLower( ), domain.ToLower( ), getErrorMessage( errorCode ) );
                }
            }
            catch ( Exception e ) // catch any error in the impersonation attempt; if received a Win32 error then format message with Win32 error information and any .NET exception information; if no Win32 error then rethrow error
            {
                errorCode = Marshal.GetLastWin32Error( );
                if ( errorCode == 0 )
                    throw;
                else
                    //throw new Win32Exception( errorCode, string.Format( "Failed attempting to impersonate user '{0}' in domain '{1}'. {2}\r\n{3}\r\n{4}", username.ToLower( ), domain.ToLower( ), getErrorMessage( errorCode ), e.Message, e.StackTrace ) );
                    errorMessage = string.Format( "Failed attempting to impersonate user '{0}' in domain '{1}'. {2}\r\n{3}\r\n{4}", username.ToLower( ), domain.ToLower( ), getErrorMessage( errorCode ), e.Message, e.StackTrace );
            }
            finally
            {
            }

            return new Status( errorCode, errorMessage );
        }

        void runCoordinator( RunAsDelegate methodToRunAs, string username, string domain, SecureString password, LogonType logontype )
        {
            List<int> retryErrorCodes = new int[ ] { ERROR_IO_PENDING, ERROR_UNKNOWNUSER }.ToList( );
            List<LogonType> logonTypes = new List<LogonType>( );
            bool ok = false;
            int retryCount = 0;
            int retryWait = RETRYWAIT;
            while ( !ok )
            {
                Status status = runas( methodToRunAs, username, domain, password, logontype );
                //if ( retryErrorCodes.Contains( status.ErrorCode ) && retryCount < RETRYCOUNTTHRESHOLD )
                if ( retryErrorCodes.Contains( status.ErrorCode ) )
                {
                    ok = false;
                    switch ( status.ErrorCode )
                    {
                        case ERROR_IO_PENDING:
                        {
                            Thread.Sleep( retryWait );
                            retryCount++;
                            // NOTE: for every additional retry the wait time is incremented 40%
                            // that means that for 8 retries the total time elapsed includes 34,399 milliseconds of wait time
                            retryWait = ( retryWait * 1.4 ).ToInt32( );
                        }
                        break;
                        case ERROR_UNKNOWNUSER:
                        {
                            logonTypes.Add( logontype );
                            if ( !logonTypes.Contains( LogonType.LOGON32_LOGON_NETWORK ) )
                                logontype = LogonType.LOGON32_LOGON_NETWORK;
                            else if ( !logonTypes.Contains( LogonType.LOGON32_LOGON_BATCH ) )
                                logontype = LogonType.LOGON32_LOGON_BATCH;
                            else if ( !logonTypes.Contains( LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT ) )
                                logontype = LogonType.LOGON32_LOGON_NETWORK_CLEARTEXT;
                            else if ( !logonTypes.Contains( LogonType.LOGON32_LOGON_SERVICE ) )
                                logontype = LogonType.LOGON32_LOGON_SERVICE;
                            else
                                throw new Win32Exception( status.ErrorCode, status.ErrorMessage );
                        }
                        break;
                        default:
                            break;
                    }
                }
                else if ( status.ErrorCode == SUCCESS )
                    ok = true;
                else
                    throw new Win32Exception( status.ErrorCode, status.ErrorMessage );
            }
        }

        #endregion Methods

        #region Nested Types

        // http://msdn.microsoft.com/en-us/magazine/cc163823.aspx
        public sealed class SafeTokenHandle : SafeHandleMinusOneIsInvalid
        {
            #region Constructors

            internal SafeTokenHandle( IntPtr handle )
                : base( true )
            {
                SetHandle( handle );
            }

            private SafeTokenHandle( )
                : base( true )
            {
            }

            #endregion Constructors

            #region Properties

            internal static SafeTokenHandle InvalidHandle
            {
                get
                {
                    return new SafeTokenHandle( IntPtr.Zero );
                }
            }

            #endregion Properties

            #region Methods

            protected override bool ReleaseHandle( )
            {
                return CloseHandle( handle );
            }

            [DllImport( "kernel32.dll", SetLastError = true )]
            [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
            [SuppressUnmanagedCodeSecurity]
            static extern bool CloseHandle( IntPtr handle );

            #endregion Methods
        }

        class Status
        {
            #region Constructors

            public Status( int errorCode, string errorMessage )
            {
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }

            #endregion Constructors

            #region Properties

            public int ErrorCode
            {
                get;
                private set;
            }

            public string ErrorMessage
            {
                get;
                private set;
            }

            #endregion Properties
        }

        class User
        {
            #region Constructors

            internal User( string name, string domain )
            {
                if ( string.IsNullOrEmpty( name ) )
                    throw new InvalidCredentialException( "No user name specified." );
                if ( string.IsNullOrEmpty( domain ) )
                    throw new InvalidCredentialException( "No user domain specified." );

                Name = name;
                Domain = domain;
            }

            internal User( string fqn )
            {
                if ( string.IsNullOrEmpty( fqn ) )
                    throw new InvalidCredentialException( "No user name specified. The fully qualified name should be specified either as domain\\user-name, or user-name@domain." );

                if ( fqn.Contains( '\\' ) )
                {
                    char v = '\\';
                    string[ ] s = fqn.Split( v );
                    if ( s.Length >= 2 )
                    {
                        Domain = s[ 0 ];
                        Name = s[ 1 ];
                    }
                }
                else if ( fqn.Contains( '/' ) )
                {
                    char v = '/';
                    string[ ] s = fqn.Split( v );
                    if ( s.Length >= 2 )
                    {
                        Domain = s[ 0 ];
                        Name = s[ 1 ];
                    }
                }
                else if ( fqn.Contains( '@' ) )
                {
                    char v = '@';
                    string[ ] s = fqn.Split( v );
                    if ( s.Length >= 2 )
                    {
                        Name = s[ 0 ];
                        string d = s[ 1 ];
                        s = d.Split( '.' );
                        if ( s.Length >= 1 )
                            Domain = s[ 0 ];
                    }
                }
                else
                {
                    throw new InvalidCredentialException( "Cannot determine format of fully qualified name. The fully qualified name should be specified either as domain\\user-name, or user-name@domain." );
                }
            }

            #endregion Constructors

            #region Properties

            internal string Domain
            {
                get;
                set;
            }

            internal string Name
            {
                get;
                set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}