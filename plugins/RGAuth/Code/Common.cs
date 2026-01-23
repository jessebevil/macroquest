using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RGAuth.Code
{
    public static class Common
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        public static unsafe void ClearTempHistory()
        {
            int option = 3;
            int* optionPtr = &option;
            bool success;

            success = InternetSetOption(0, 81, new IntPtr(optionPtr), sizeof(int));
        }
    }
}
