using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows;

namespace RGAuth.Code
{
    class SSEncryption
    {
        static byte[] entropy = Encoding.Unicode.GetBytes("Really Cool Salt For A Really Cool Encryption");

        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                Encoding.ASCII.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.LocalMachine);

            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] encryptionArr = Encoding.ASCII.GetBytes(encryptedData);
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.LocalMachine);

                return ToSecureString(Encoding.ASCII.GetString(decryptedData));
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an issue with pulling account information."
                    + "\nIf the problem persists, contact will with a screenshot of the following:"
                    + "\n\n" + ex.ToString(),
                    "Account Info Error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();

            foreach (char c in input)
            {
                secure.AppendChar(c);
            }

            secure.MakeReadOnly();

            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);

            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }

            return returnValue;
        }
    }
}
