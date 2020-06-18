using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrwSoftware
{
    public class JPort
    {
        public static int HTTP_DEFAULT = 80;
        public static int HTTPS_DEFAULT = 443;
        public static int FTP_DEFAULT = 21;
        public static int SFTP_DEFAULT = 22;
        public static int SSH_DEFAULT = 22;
        public static int RDP_DEFAULT = 3389;

        [JDisplayName(typeof(FrwUtilsRes), "JPort_Protocol")]
        [JDictProp(DictNames.Protocol, false, DisplyPropertyStyle.TextOnly)]
        [JRequired]
        public string Protocol { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JPort_Name")]
        [JNameProperty, JReadOnly, JsonIgnore]
        public string Name
        {
            get
            {
                StringBuilder str = new StringBuilder(Protocol);
               
                if (Protocol == ProtocolEnum.http.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.HTTP_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.HTTP_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }
                }
                else if (Protocol == ProtocolEnum.https.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.HTTPS_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.HTTPS_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }
                }
                else if (Protocol == ProtocolEnum.ftp.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.FTP_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.FTP_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }
                }
                else if (Protocol == ProtocolEnum.sftp.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.SFTP_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.SFTP_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }
                }
                else if (Protocol == ProtocolEnum.ssh.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.SSH_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.SSH_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }
                }
                else if (Protocol == ProtocolEnum.rdp.ToString())
                {
                    if (string.IsNullOrEmpty(Port) == false)
                    {
                        if (Port != JPort.RDP_DEFAULT.ToString()) str.Append(":" + Port);
                    }
                    if (string.IsNullOrEmpty(ExtPort) == false)
                    {
                        if (ExtPort != JPort.RDP_DEFAULT.ToString()) str.Append(":" + ExtPort + "(ext)");
                    }

                }

                return str.ToString();
            }
        }

        [JDisplayName(typeof(FrwUtilsRes), "JPort_Port")]
        public string Port { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JPort_ExtPort")]
        public string ExtPort { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JPort_Info")]
        public string Info { get; set; }
    }
}
