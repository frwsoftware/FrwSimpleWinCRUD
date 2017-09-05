/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Globalization;

namespace FrwSoftware
{
    public static class SafeConvert
    {
        private static readonly CultureInfo CInfo = new CultureInfo("en-US");

        public static double ToDouble(string str)
        {
			if (str == null) return double.NaN; 
			str = str.Replace(',', '.'); 
            return Convert.ToDouble(str, CInfo);
        }

        /// <summary>
        /// Convert a floating-point number to a string
        /// </summary>
        /// <param name="val">Convertible number</param>
        /// <param name="fraction">Number of decimal places</param>
        public static string ToString(double val, int fraction)
        {
            string fractionFormat = "{0:f" + fraction + "}";
            string result = string.Format(CInfo, fractionFormat, val);
            return result;
        }

        public static string ToString(double val)
        {
            return ToString(val, 8);
        }

        public static byte[] ToByteArray(Array val)
        {
            byte[] result = new byte[val.Length];
            for (int i = 0; i < val.Length; i++)
                result[i] = (byte)val.GetValue(i + 1);
            return result;
        }

        //http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c
        //usage StatusEnum MyStatus = "Active".ToEnum<StatusEnum>();
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        //usage StatusEnum MyStatus = "Active".ToEnum(StatusEnum.None);
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }
    }
}
