using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Ini
{
    class IniUtil
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(
            String section,
            String key,
            String def,
            StringBuilder retVal,
            int size,
            String filePath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(
            String section,
            String key,
            String val,
            String findPath);

        public string SetIniValue(String section, String key, String value, String file_path)
        {
            StringBuilder temp = new StringBuilder(255);
            WritePrivateProfileString(section, key, value, file_path);
            return temp.ToString();
        }

        public string GetIniValue(String section, String key, String file_path)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, file_path);                  
            return temp.ToString();
        }
    }
}
