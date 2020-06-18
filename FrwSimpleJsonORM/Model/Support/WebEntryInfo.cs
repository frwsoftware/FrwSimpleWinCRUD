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
using System.Reflection;

namespace FrwSoftware
{

    public enum ViewType
    {
        NONE,
        WORD,
        Simple,
        CefBrowser
    }

    public enum BrowserPrivateType
    {
        COMMON_CACHE,
        PERSONAL_IN_MEMORY_CACHE,
        PERSONAL_OLD_DISK_CACHE
    }

    public enum ProtocolEnum
    {
        http,
        https,
        ftp,
        sftp,
        ssh,
        rdp,
        tcp
    }

    public class WebEntryInfoWrap
    {
        public WebEntryInfo WebEntryInfo { get; set; }
        public PropertyInfo Property { get; set; }
    }

    public class WebEntryInfo
    {

        public string Login { get; set; }
        public string Password { get; set; }
        public string BasicAuthLogin { get; set; }
        public string BasicAuthPassword { get; set; }
        public ViewType RecоmmendedViewType { get; set; } = ViewType.NONE;
        public string CachePath { get; set; }
        public BrowserPrivateType BrowserPrivateType { get; set; } = BrowserPrivateType.COMMON_CACHE;
        public JUserAgent JUserAgent { get; set; }
        public string AllowedVPNServerId { get; set; }
        public string Path { get; set; }
        public string InternalAddress { get; set; }
        public string ExternalAddress { get; set; }
        public bool IsInInternalNetwork { get; set; }
        private string url = null;
        public string Url
        {
            get
            {
                //https has prority 
                if (url != null) return url;
                else return MakeHttpUrl((AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.https.ToString())) != null));
            }
            set
            {
                url = value;
            }
        }

        public IList<JPort> AccessPorts { get;  } = new List<JPort>();
    
        public bool IsHttpsAllowed
        {
            get
            {
                if (url != null)
                {
                    if (url.StartsWith((ProtocolEnum.https.ToString()))) return true;
                    else return false;
                }
                else return (AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.https.ToString())) != null);
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
                return (AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.http.ToString())) != null);
            }
        }
        public bool IsRDPAllowed
        {
            get
            {
                 return (AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.rdp.ToString())) != null);
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
                str.Append(InternalAddress.Trim());
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
                str.Append(ExternalAddress.Trim());
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

        [JDisplayName("HTTP port")]
        public string PortHTTP {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.http.ToString()));
                return (p != null) ? (p.Port) : null;
            }
        }
        [JDisplayName("HTTP external port")]
        public string ExtPortHTTP {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.http.ToString()));
                return (p != null) ? (p.ExtPort) : null;
            }
        }
        [JDisplayName("HTTPS port")]
        public string PortHTTPS {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.https.ToString()));
                return (p != null) ? (p.Port) : null;
            }

        }
        [JDisplayName("HTTPS external port")]
        public string ExtPortHTTPS {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.https.ToString()));
                return (p != null) ? (p.ExtPort) : null;
            }
        }
        [JDisplayName("SSH port")]
        public string PortSSH {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.ssh.ToString()));
                return (p != null) ? (p.Port) : null;
            }
        }
        [JDisplayName("SSH external port")]
        public string ExtPortSSH {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.ssh.ToString()));
                return (p != null) ? (p.ExtPort) : null;
            }
        }
        [JDisplayName("RDP port")]
        public string PortRDP {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.rdp.ToString()));
                return (p != null) ? (p.Port) : null;
            }
        }
        [JDisplayName("RDP external port")]
        public string ExtPortRDP {
            get
            {
                JPort p = AccessPorts.FirstOrDefault(s => s.Protocol.Equals(ProtocolEnum.rdp.ToString()));
                return (p != null) ? (p.ExtPort) : null;
            }
        }

        ////////////////////////////////
        static public WebEntryInfo GetWebEntryInfoFromObject(object o)
        {
            if (o == null) return null;
            PropertyInfo p = o.GetType().GetProperties().Where(
                prop => prop.PropertyType == typeof(WebEntryInfo)).FirstOrDefault();
            if (p == null) return null;
            return p.GetValue(o) as WebEntryInfo;
        }
        static public IList<WebEntryInfoWrap> GetWebEntryInfosFromObject(object o)
        {
            IList<WebEntryInfoWrap> list = new List<WebEntryInfoWrap>();
            IEnumerable<PropertyInfo> ps = o.GetType().GetProperties().Where(
                prop => prop.PropertyType == typeof(WebEntryInfo));
            foreach(var p in ps)
            {
                WebEntryInfoWrap wrap = new WebEntryInfoWrap();
                wrap.Property = p;
                wrap.WebEntryInfo = p.GetValue(o) as WebEntryInfo;
                list.Add(wrap);
            }
            return list;
        }
    }
}
