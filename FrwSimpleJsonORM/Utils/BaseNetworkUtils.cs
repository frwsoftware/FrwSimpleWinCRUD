using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class BaseNetworkUtils
    {
        ///static public string LOCAL_NETWORK_IP_PREFIX = "192.168.";

        static public string GetCPUId()
        {
            //https://stackoverflow.com/questions/2004666/get-unique-machine-id
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuInfo;
        }
        static public string GetUserName()
        {
            return Environment.UserName;
        }
        static public string GetCompAndUserUniqueId()
        {
            return Dm.CPUId + "_" + Dm.UserName;
        }






        //возвращает все ip адреса в порядке приоритета 
        //приоритет важен , т.к. потом первый адрес может использоваться для ссылок
        static public string[] GetHostIps(string prefif)
        {
            List<IPAddress> addresses = new List<IPAddress>();
            List<string> ads = new List<string>();
            string hostName = Dns.GetHostName();

            //сначала пытаемся найти адрес и предпочитаемых сетей
            IPAddress[] addrs = Dns.GetHostEntry(hostName).AddressList;

            //сперва пытаемся добавить адреса приоритетных сетей 
            foreach (IPAddress addr in addrs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)//ignore ipv6
                {
                    if (addr.ToString().Contains(prefif))
                    {
                        addresses.Add(addr);
                    }
                }
            }
            //затем все остальные 
            foreach (IPAddress addr in addrs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)//ignore ipv6
                {
                    if (addresses.Contains(addr) == false)
                    {
                        addresses.Add(addr);
                    }
                }
            }
            foreach (IPAddress addr in addresses)
            {
                ads.Add(addr.ToString());
            }
            return ads.ToArray();
        }
        static public string GetHostIp()
        {
            string localIP = null;
            try
            {
                //https://stackoverflow.com/questions/6803073/get-local-ip-address
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            catch(Exception ex)
            {
                Log.LogError(ex);
            }
            return (localIP != null) ? localIP : "127.0.0.1";
        }
    
        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="checkDns"></param>
        /// <returns></returns>
        static public string GetHostIpOld(string prefix, bool checkDns)
        {

            IPAddress address = null;
            foreach (NetworkInterface inter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (inter.OperationalStatus == OperationalStatus.Up)
                {
                    bool valid = false;
                    if (checkDns)
                    {

                        IPInterfaceProperties adapterProperties = inter.GetIPProperties();
                        IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                        if (dnsServers.Count > 0)
                        {

                            foreach (IPAddress dns in dnsServers)
                            {
                                if (dns.ToString().Contains(prefix))
                                {
                                    valid = true;
                                    break;
                                }
                            }

                        }
                    }
                    else valid = true;
                    if (valid)
                    {
                        foreach (var addr in inter.GetIPProperties().UnicastAddresses)
                        {
                            IPAddress adr = addr.Address;
                            if (adr.AddressFamily != AddressFamily.InterNetwork) continue;
                            if (adr.ToString().Contains(prefix))
                            {
                                address = adr;
                                break;
                            }
                        }
                    }
                }
            }

            if (address == null)
            {
                foreach (NetworkInterface inter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (inter.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var addr in inter.GetIPProperties().UnicastAddresses)
                        {
                            IPAddress adr = addr.Address;
                            if (adr.AddressFamily != AddressFamily.InterNetwork) continue;
                            address = adr;
                            break;
                        }
                    }
                }
            }
            return address != null ? address.ToString() : "127.0.0.1";
        }
        static IPAddress FindGetGatewayAddress()
        {
            //https://stackoverflow.com/questions/1069103/how-to-get-the-ip-address-of-the-server-on-which-my-c-sharp-application-is-runni
            IPGlobalProperties ipGlobProps = IPGlobalProperties.GetIPGlobalProperties();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipInfProps = ni.GetIPProperties();
                foreach (GatewayIPAddressInformation gi in ipInfProps.GatewayAddresses)
                    return gi.Address;
            }
            return null;
        }
        public static string GetDnsAddressForIP(string ip)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

                    bool ipInThisNetwork = false;
                    foreach (var addr in adapterProperties.UnicastAddresses)
                    {
                        IPAddress adr = addr.Address;
                        if (adr.AddressFamily != AddressFamily.InterNetwork) continue;
                        if (adr.ToString().Contains(ip))
                        {
                            ipInThisNetwork = true;
                            break;
                        }
                    }

                    if (ipInThisNetwork)
                    {
                        IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                        if (dnsServers.Count > 0)
                        {

                            foreach (IPAddress dns in dnsServers)
                            {
                                if (dns.AddressFamily != AddressFamily.InterNetwork) continue;
                                return dns.ToString();
                            }

                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetDnsAddresOld(string prefix)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {

                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                    if (dnsServers.Count > 0)
                    {

                        foreach (IPAddress dns in dnsServers)
                        {
                            if (dns.ToString().StartsWith(prefix)) return dns.ToString();
                        }

                    }
                }
            }
            return null;
        }



                    
        public static string MACStringToString(string address, string separator = ":")
        {
            if (address == null) return null;
            string addressNew = address;
            int insertedCount = 0;
            for (int i = 2; i < address.Length; i = i + 2)
                addressNew = addressNew.Insert(i + insertedCount++, ":");

            return addressNew;
        }
        public static bool CompareMACs(string address1, string address2)
        {
            if (address1 == null || address2 == null) return false;
            if (address1.Contains(":") == false) address1 = MACStringToString(address1);
            if (address2.Contains(":") == false) address2 = MACStringToString(address2);
            address1 = address1.ToUpper();
            address2 = address2.ToUpper();
            if (address1.Equals(address2)) return true;
            else return false;
        }


        #region arp
        //команда arp -a

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(uint DestIP, uint SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        public static PhysicalAddress GetPhysicalAddress(IPAddress ipAddress)
        {
            PhysicalAddress pa = LocateMacAddress(ipAddress);
            if (!pa.Equals(PhysicalAddress.None))
            {
                return pa;
            }
            else
            {
                Ping p = new Ping();
                PingReply result = p.Send(ipAddress);

                return LocateMacAddress(ipAddress);
            }
        }
        enum ArpErrorCodes
        {
            None = 0,
            ERROR_GEN_FAILURE = 31,
            ERROR_NOT_SUPPORTED = 50,
            ERROR_BAD_NET_NAME = 67,
            ERROR_BUFFER_OVERFLOW = 111,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_USER_BUFFER = 1784,
        }
        static public PhysicalAddress LocateMacAddress(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] macAddressBytes = new byte[6];
                int length = macAddressBytes.Length;
#pragma warning disable 0618
                ArpErrorCodes c = (ArpErrorCodes)SendARP((uint)ipAddress.Address, 0, macAddressBytes, ref length);
#pragma warning restore 0618
                if (c == ArpErrorCodes.None)
                {
                    return new PhysicalAddress(macAddressBytes);
                }
            }
            return PhysicalAddress.None;
        }

        static public string GetMacByIP(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return null;
            PhysicalAddress pa = LocateMacAddress(IPAddress.Parse(ip));
            return pa.ToString();
        }
        #endregion

        public static void SetupNetworkAddressChangedListener()
        {
            //http://stackoverflow.com/questions/4457773/i-need-a-event-to-detect-internet-connect-disconnect
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            Console.WriteLine("Setup Listening for address changes. ");

        }

        private static void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface n in adapters)
            {
                Console.WriteLine("   {0} is {1}", n.Name, n.OperationalStatus);
            }
        }

        public static void UnSetupNetworkAddressChangedListener()
        {
            NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
            Console.WriteLine("UnSetup Listening for address changes. ");
        }






        //https://www.codeproject.com/Questions/988841/how-can-get-wifi-router-info-with-csharp
        //https://msdn.microsoft.com/en-us/library/system.net.networkinformation.networkinterface(v=vs.110).aspx
        public static void ShowNetworkInterfaces()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);
            if (nics == null || nics.Length < 1)
            {
                Console.WriteLine("  No network interfaces found.");
                return;
            }

            Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}",
                    adapter.OperationalStatus);
                string versions = "";

                // Create a display string for the supported IP versions. 
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);
                ShowIPAddresses(properties);

                // The following information is not useful for loopback adapters. 
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                Console.WriteLine("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix);

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            Console.WriteLine(label);
                            foreach (var ip in winsServers)
                            {
                                Console.WriteLine(ip);
                            }

                            //ShowIPAddresses(winsServers);
                        }
                    }
                }

                Console.WriteLine("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled);
                Console.WriteLine("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled);
                Console.WriteLine("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast);
                ShowInterfaceStatistics(adapter);
                Console.WriteLine();
            }
        }
        private static void ShowInterfaceStatistics(NetworkInterface adapter)
        {
            IPv4InterfaceStatistics stats = adapter.GetIPv4Statistics();
            Console.WriteLine(" Stats:");
            Console.WriteLine(" Packets Received ....... : {0}",
            stats.UnicastPacketsReceived);
            Console.WriteLine(" Bytes Sent ............. : {0}",
            stats.BytesSent);
        }
        //
        public static void ShowIPAddresses(IPInterfaceProperties adapterProperties)
        {
            IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
            if (dnsServers != null)
            {
                foreach (IPAddress dns in dnsServers)
                {
                    Console.WriteLine("  DNS Servers ............................. : {0}",
                        dns.ToString()
                   );
                }
            }
            IPAddressInformationCollection anyCast = adapterProperties.AnycastAddresses;
            if (anyCast != null)
            {
                foreach (IPAddressInformation any in anyCast)
                {
                    Console.WriteLine("  Anycast Address .......................... : {0} {1} {2}",
                        any.Address,
                        any.IsTransient ? "Transient" : "",
                        any.IsDnsEligible ? "DNS Eligible" : ""
                    );
                }
                Console.WriteLine();
            }

            MulticastIPAddressInformationCollection multiCast = adapterProperties.MulticastAddresses;
            if (multiCast != null)
            {
                foreach (IPAddressInformation multi in multiCast)
                {
                    Console.WriteLine("  Multicast Address ....................... : {0} {1} {2}",
                        multi.Address,
                        multi.IsTransient ? "Transient" : "",
                        multi.IsDnsEligible ? "DNS Eligible" : ""
                    );
                }
                Console.WriteLine();
            }
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            if (uniCast != null)
            {
                string lifeTimeFormat = "dddd, MMMM dd, yyyy  hh:mm:ss tt";
                foreach (UnicastIPAddressInformation uni in uniCast)
                {
                    DateTime when;

                    Console.WriteLine("  Unicast Address ......................... : {0}", uni.Address);
                    Console.WriteLine("     Prefix Origin ........................ : {0}", uni.PrefixOrigin);
                    Console.WriteLine("     Suffix Origin ........................ : {0}", uni.SuffixOrigin);
                    Console.WriteLine("     Duplicate Address Detection .......... : {0}",
                        uni.DuplicateAddressDetectionState);

                    // Format the lifetimes as Sunday, February 16, 2003 11:33:44 PM
                    // if en-us is the current culture.

                    // Calculate the date and time at the end of the lifetimes.    
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressValidLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     Valid Life Time ...................... : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );
                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.AddressPreferredLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     Preferred life time .................. : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );

                    when = DateTime.UtcNow + TimeSpan.FromSeconds(uni.DhcpLeaseLifetime);
                    when = when.ToLocalTime();
                    Console.WriteLine("     DHCP Leased Life Time ................ : {0}",
                        when.ToString(lifeTimeFormat, System.Globalization.CultureInfo.CurrentCulture)
                    );
                }
                Console.WriteLine();
            }
        }




    }
}
