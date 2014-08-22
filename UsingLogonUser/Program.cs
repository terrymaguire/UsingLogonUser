using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UsingLogonUser
{
    class Program
    {

        // obtains user token
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        static void Main(string[] args)
        {
            AttemptLogin();

            Console.ReadLine();
        }



        public static void AttemptLogin()
        {
            //elevate privileges before doing file copy to handle domain security
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            const int LOGON32_LOGON_NETWORK = 3;

            string domain = "domain";
            string user = "username";
            string password = "password";

            try
            {

                // Call LogonUser to get a token for the user
                bool loggedOn = LogonUser(user,
                                            domain,
                                            password,
                                            LOGON32_LOGON_NETWORK,
                                            LOGON32_PROVIDER_DEFAULT,
                                            ref userHandle);

                if (!loggedOn)
                {
                    Console.WriteLine("Exception authenticating user, error code: " + Marshal.GetLastWin32Error());
                    return;
                }
                else
                {
                    Console.WriteLine("Successfully logged in!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception authenticating user: " + ex.Message);
            }
            finally
            {
                // Clean up
                if (userHandle != IntPtr.Zero)
                {
                    CloseHandle(userHandle);
                }
            }
        }



    }
}