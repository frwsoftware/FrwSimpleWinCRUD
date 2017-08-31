using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class DataUtils
    {
        static public string DATE_TIME_PATTERN_FOR_DISPLAY = "yyyy/MM/dd HH:mm:ss";

        static private Random _rnd = new Random();

        static public string genKey(Random rnd)
        {
            if (rnd == null) rnd = _rnd;
            string chars = "23456789ABCDEFGHIJKMNPQRSTUVWXZ";
            string randomstring = "";
            for (var i = 0; i < 8; i++)
            {
                //The random number is from 0 (inclusive) to 1 Math.random
                double r = rnd.NextDouble();

                int rnum = (int)Math.Floor(r * (double)(chars.Length));
                randomstring += chars.Substring(rnum, 1);
            }
            return randomstring;

        }


        [DllImport("kernel32.dll")]
        private static extern Int32 WideCharToMultiByte(UInt32 CodePage, UInt32 dwFlags, [MarshalAs(UnmanagedType.LPWStr)] String lpWideCharStr, Int32 cchWideChar, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpMultiByteStr, Int32 cbMultiByte, IntPtr lpDefaultChar, IntPtr lpUsedDefaultChar);


        public static string Utf16ToUtf8_1(string str)
        {

            byte[] utf16Bytes4 = Encoding.UTF8.GetBytes(str);
            StringBuilder bwww = new StringBuilder();
            for (int i = 0; i < utf16Bytes4.Length; ++i)
            {
                char ch = (char)utf16Bytes4[i];
                bwww.Append(ch);
            }
            return bwww.ToString();
        }


        public static string Utf16ToUtf8(string utf16String)
        {
            Int32 iNewDataLen = WideCharToMultiByte(Convert.ToUInt32(Encoding.UTF8.CodePage), 0, utf16String, utf16String.Length, null, 0, IntPtr.Zero, IntPtr.Zero);
            if (iNewDataLen > 1)
            {
                StringBuilder utf8String = new StringBuilder(iNewDataLen);
                WideCharToMultiByte(Convert.ToUInt32(Encoding.UTF8.CodePage), 0, utf16String, -1, utf8String, utf8String.Capacity, IntPtr.Zero, IntPtr.Zero);

                return utf8String.ToString();
            }
            else
            {
                return String.Empty;
            }
        }
        public static string Utf8ToUtf16(string utf8String)
        {
            /***************************************************************
             * Every .NET string will store text with the UTF-16 encoding, *
             * known as Encoding.Unicode. Other encodings may exist as     *
             * Byte-Array or incorrectly stored with the UTF-16 encoding.  *
             *                                                             *
             * UTF-8 = 1 bytes per char                                    *
             *    ["100" for the ansi 'd']                                 *
             *    ["206" and "186" for the russian '?']                    *
             *                                                             *
             * UTF-16 = 2 bytes per char                                   *
             *    ["100, 0" for the ansi 'd']                              *
             *    ["186, 3" for the russian '?']                           *
             *                                                             *
             * UTF-8 inside UTF-16                                         *
             *    ["100, 0" for the ansi 'd']                              *
             *    ["206, 0" and "186, 0" for the russian '?']              *
             *                                                             *
             * First we need to get the UTF-8 Byte-Array and remove all    *
             * 0 byte (binary 0) while doing so.                           *
             *                                                             *
             * Binary 0 means end of string on UTF-8 encoding while on     *
             * UTF-16 one binary 0 does not end the string. Only if there  *
             * are 2 binary 0, than the UTF-16 encoding will end the       *
             * string. Because of .NET we don't have to handle this.       *
             *                                                             *
             * After removing binary 0 and receiving the Byte-Array, we    *
             * can use the UTF-8 encoding to string method now to get a    *
             * UTF-16 string.                                              *
             *                                                             *
             ***************************************************************/

            // Get UTF-8 bytes and remove binary 0 bytes (filler)
            List<byte> utf8Bytes = new List<byte>(utf8String.Length);
            foreach (byte utf8Byte in utf8String)
            {
                // Remove binary 0 bytes (filler)
                if (utf8Byte > 0)
                {
                    utf8Bytes.Add(utf8Byte);
                }
            }

            // Convert UTF-8 bytes to UTF-16 string
            return Encoding.UTF8.GetString(utf8Bytes.ToArray());
        }
    }
}
