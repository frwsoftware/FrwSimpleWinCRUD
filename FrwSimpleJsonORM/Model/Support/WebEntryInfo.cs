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
using System.Reflection;

namespace FrwSoftware
{
    public enum ViewType
    {
        NONE,
        WORD,
        IE,
        Simple,
        Awesomium,  
        CefBrowser
    }
    public enum ProtocolEnum
    {
        http,
        https,
        ftp,
        sftp,
        ssh,
        rdp
    }
    public class WebEntryInfo
    {
        private IList<string> protocols = new List<string>();
        public IList<string> Protocols {
            get
            {
                return protocols;
            }
        }
        public bool IsHttpsAllowed
        {
            get
            {
                if (url != null)
                {
                    if (url.StartsWith((ProtocolEnum.https.ToString()))) return true;
                    else return false;
                }
                else return (protocols.FirstOrDefault(s => s.Equals(ProtocolEnum.https.ToString())) != null);
            }
        }
        public bool IsHttpAllowed
        {
            get
            {
                if (url != null)
                {
                    if (url.StartsWith((ProtocolEnum.https.ToString()))) return false;
                    else return true;
                }
                return (protocols.FirstOrDefault(s => s.Equals(ProtocolEnum.http.ToString())) != null);
            }
        }
        public bool IsRDPAllowed
        {
            get
            {
                 return (protocols.FirstOrDefault(s => s.Equals(ProtocolEnum.rdp.ToString())) != null);
            }
        }
        private string url = null;
        public string Url {
            get
            {
                //https has prority 
                if (url != null) return url;
                else return MakeHttpUrl((protocols.FirstOrDefault(s => s.Equals(ProtocolEnum.https.ToString())) != null));
            }
            set
            {
                url = value;
            }
        }
        public string HttpUrl
        {
            get
            {
                if (IsHttpAllowed)
                {
                    if (url != null) return url;
                    else return MakeHttpUrl(false);
                }
                else return null;
            }
            set
            {
                url = value;
            }
        }
        public string HttpsUrl
        {
            get
            {
                if (IsHttpsAllowed)
                {
                    if (url != null) return url;
                    else return MakeHttpUrl(true);
                }
                else return null;
            }
            set
            {
                url = value;
            }
        }
        private string MakeHttpUrl(bool isHttps)
        {
            StringBuilder str = new StringBuilder();
            if (isHttps) str.Append("https://");
            else str.Append("http://");
            if (IsInInternalNetwork)
            {
                if (InternalAddress == null) return null;
                str.Append(InternalAddress);
                if (isHttps && string.IsNullOrEmpty(PortHTTPS) == false)
                {
                    str.Append(":");
                    str.Append(PortHTTPS);
                }
                else if (string.IsNullOrEmpty(PortHTTP) == false)
                {
                    str.Append(":");
                    str.Append(PortHTTP);
                }
            }
            else
            {
                if (ExternalAddress == null) return null;
                str.Append(ExternalAddress);
                if (isHttps && string.IsNullOrEmpty(ExtPortHTTPS) == false)
                {
                    str.Append(":");
                    str.Append(ExtPortHTTPS);
                }
                else if (string.IsNullOrEmpty(ExtPortHTTP) == false)
                {
                    str.Append(":");
                    str.Append(ExtPortHTTP);
                }
            }
            if (Path != null)
            {
                if (Path.StartsWith("/") == false) str.Append("/");
                str.Append(Path);
            }
            return str.ToString();

        }

        public string Path { get; set; }
        public string InternalAddress { get; set; }
        public string ExternalAddress { get; set; }
        public bool IsInInternalNetwork { get; set; }
        [JDisplayName("HTTP порт")]
        public string PortHTTP { get; set; }
        [JDisplayName("HTTP внешний порт")]
        public string ExtPortHTTP { get; set; }
        [JDisplayName("HTTPS порт")]
        public string PortHTTPS { get; set; }
        [JDisplayName("HTTPS внешний порт")]
        public string ExtPortHTTPS { get; set; }
        [JDisplayName("SSH порт")]
        public string PortSSH { get; set; }
        [JDisplayName("SSH внешний порт")]
        public string ExtPortSSH { get; set; }
        [JDisplayName("RDP порт")]
        public string PortRDP { get; set; }
        [JDisplayName("RDP внешний порт")]
        public string ExtPortRDP { get; set; }


        public string Login { get; set; }
        public string Password { get; set; }


        static public WebEntryInfo GetWebEntryInfoFromObject(object o)
        {
            if (o == null) return null;
            PropertyInfo p = o.GetType().GetProperties().Where(
                prop => prop.PropertyType == typeof(WebEntryInfo)).FirstOrDefault();
            if (p == null) return null;
            return p.GetValue(o) as WebEntryInfo;
        }
    }
}
