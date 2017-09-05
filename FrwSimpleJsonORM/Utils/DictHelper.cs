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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    static public class DictHelper
    {
        public static object Get(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary == null) return null;
            object ret;
            // Ignore return value
            dictionary.TryGetValue(key, out ret);
            return ret;
        }
        public static string GetString(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary == null) return null;
            object ret;
            // Ignore return value
            dictionary.TryGetValue(key, out ret);
            return ret as string;
        }

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue ret;
            // Ignore return value
            dictionary.TryGetValue(key, out ret);
            return ret;
        }

        static public bool GetValueAsBool(IDictionary<string, object> dictionary, string key)
        {
            return GetValueAsBool(dictionary, key, false);
        }
        static public bool GetValueAsBool(IDictionary<string, object> dictionary, string name, bool defaultValue)
        {
            object value = Get(dictionary, name);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            else if (value is string)
            {
                return bool.Parse(value as string);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

}
