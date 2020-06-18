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
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;


namespace FrwSoftware
{
    public static class AppLocator
    {
        private const int BrwIE = 0;
        private const int BrwFirefox = 1;
        private const int BrwOpera = 2;
        private const int BrwChrome = 3;
        private const int BrwSafari = 4;
        private const int BrwEdge = 5;

        private static Dictionary<int, string> m_dictPaths =
            new Dictionary<int, string>();

        public static string InternetExplorerPath
        {
            get { return GetPath(BrwIE, FindInternetExplorer); }
        }

        public static string FirefoxProtablePath
        {
            get; set;
        }
        public static string ChromeProtablePath
        {
            get; set;
        }
        public static string KittyPath
        {
            get; set;
        }
        public static string RDPPath
        {
            get { return "mstsc"; }
        }
        public static string WinSCPPath
        {
            get { return "WinSCP.exe"; }
        }

        public static string FirefoxPath
        {
            get { return GetPath(BrwFirefox, FindFirefox); }
        }

        public static string OperaPath
        {
            get { return GetPath(BrwOpera, FindOpera); }
        }

        public static string ChromePath
        {
            get { return GetPath(BrwChrome, FindChrome); }
        }

        public static string SafariPath
        {
            get { return GetPath(BrwSafari, FindSafari); }
        }


        /// <summary>
        /// Edge executable cannot be run normally.
        /// !!!! do not use for open Edge - see https://stackoverflow.com/questions/31164253/how-to-open-url-in-microsoft-edge-from-the-command-line
        /// </summary>
        public static string EdgePath
        {
            get { return GetPath(BrwEdge, FindEdge); }
        }

        private static bool? m_obEdgeProtocol = null;
        public static bool EdgeProtocolSupported
        {
            get
            {
                if (m_obEdgeProtocol.HasValue)
                    return m_obEdgeProtocol.Value;

                bool b = false;
                RegistryKey rk = null;
                try
                {
                    rk = Registry.ClassesRoot.OpenSubKey(
                        "microsoft-edge", false);
                    if (rk != null)
                        b = (rk.GetValue("URL Protocol") != null);
                }
                catch (Exception ex) { throw ex; }
                finally { if (rk != null) rk.Close(); }

                m_obEdgeProtocol = b;
                return b;
            }
        }

        private delegate string FindAppDelegate();

        private static string GetPath(int iBrwID, FindAppDelegate f)
        {
            string strPath;
            if (m_dictPaths.TryGetValue(iBrwID, out strPath)) return strPath;

            try
            {
                strPath = f();
                if ((strPath != null) && (strPath.Length == 0)) strPath = null;
            }
            catch (Exception) { strPath = null; }

            m_dictPaths[iBrwID] = strPath;
            return strPath;
        }


        private static string FindInternetExplorer()
        {
            const string strIEDef = "SOFTWARE\\Clients\\StartMenuInternet\\IEXPLORE.EXE\\shell\\open\\command";
            const string strIEWow = "SOFTWARE\\Wow6432Node\\Clients\\StartMenuInternet\\IEXPLORE.EXE\\shell\\open\\command";

            for (int i = 0; i < 6; ++i)
            {
                RegistryKey k = null;

                // https://msdn.microsoft.com/en-us/library/windows/desktop/dd203067.aspx
                if (i == 0)
                    k = Registry.CurrentUser.OpenSubKey(strIEDef, false);
                else if (i == 1)
                    k = Registry.CurrentUser.OpenSubKey(strIEWow, false);
                else if (i == 2)
                    k = Registry.LocalMachine.OpenSubKey(strIEDef, false);
                else if (i == 3)
                    k = Registry.LocalMachine.OpenSubKey(strIEWow, false);
                else if (i == 4)
                    k = Registry.ClassesRoot.OpenSubKey(
                        "IE.AssocFile.HTM\\shell\\open\\command", false);
                else
                    k = Registry.ClassesRoot.OpenSubKey(
                        "Applications\\iexplore.exe\\shell\\open\\command", false);

                if (k == null) continue;

                string str = (k.GetValue(string.Empty) as string);
                k.Close();

                if (str == null) continue;

                str = UrlUtil.GetQuotedAppPath(str).Trim();
                if (str.Length == 0) continue;
                // https://sourceforge.net/p/keepass/discussion/329221/thread/6b292ede/
                if (str.StartsWith("iexplore.exe", StrUtil.CaseIgnoreCmp)) continue;

                return str;
            }

            return null;
        }

        private static string FindFirefox()
        {

            try
            {
                string strPath = FindFirefoxWin(false);
                if (!string.IsNullOrEmpty(strPath)) return strPath;
            }
            catch (Exception) { }

            return FindFirefoxWin(true);
        }

        private static string FindFirefoxWin(bool bWowNode)
        {
            RegistryKey kFirefox = Registry.LocalMachine.OpenSubKey(bWowNode ?
                "SOFTWARE\\Wow6432Node\\Mozilla\\Mozilla Firefox" :
                "SOFTWARE\\Mozilla\\Mozilla Firefox", false);
            if (kFirefox == null) return null;

            string strCurVer = (kFirefox.GetValue("CurrentVersion") as string);
            if (string.IsNullOrEmpty(strCurVer))
            {
                // The ESR version stores the 'CurrentVersion' value under
                // 'Mozilla Firefox ESR', but the version-specific info
                // under 'Mozilla Firefox\\<Version>' (without 'ESR')
                RegistryKey kESR = Registry.LocalMachine.OpenSubKey(bWowNode ?
                    "SOFTWARE\\Wow6432Node\\Mozilla\\Mozilla Firefox ESR" :
                    "SOFTWARE\\Mozilla\\Mozilla Firefox ESR", false);
                if (kESR != null)
                {
                    strCurVer = (kESR.GetValue("CurrentVersion") as string);
                    kESR.Close();
                }

                if (string.IsNullOrEmpty(strCurVer))
                {
                    kFirefox.Close();
                    return null;
                }
            }

            RegistryKey kMain = kFirefox.OpenSubKey(strCurVer + "\\Main", false);
            if (kMain == null)
            {
                Debug.Assert(false);
                kFirefox.Close();
                return null;
            }

            string strPath = (kMain.GetValue("PathToExe") as string);
            if (!string.IsNullOrEmpty(strPath))
            {
                strPath = strPath.Trim();
                strPath = UrlUtil.GetQuotedAppPath(strPath).Trim();
            }
            else { Debug.Assert(false); }

            kMain.Close();
            kFirefox.Close();
            return strPath;
        }

        private static string FindOpera()
        {

            // Old Opera versions
            const string strOp12 = "SOFTWARE\\Clients\\StartMenuInternet\\Opera\\shell\\open\\command";
            // Opera >= 20.0.1387.77
            const string strOp20 = "SOFTWARE\\Clients\\StartMenuInternet\\OperaStable\\shell\\open\\command";

            for (int i = 0; i < 5; ++i)
            {
                RegistryKey k = null;

                // https://msdn.microsoft.com/en-us/library/windows/desktop/dd203067.aspx
                if (i == 0)
                    k = Registry.CurrentUser.OpenSubKey(strOp20, false);
                else if (i == 1)
                    k = Registry.CurrentUser.OpenSubKey(strOp12, false);
                else if (i == 2)
                    k = Registry.LocalMachine.OpenSubKey(strOp20, false);
                else if (i == 3)
                    k = Registry.LocalMachine.OpenSubKey(strOp12, false);
                else // Old Opera versions
                    k = Registry.ClassesRoot.OpenSubKey(
                        "Opera.HTML\\shell\\open\\command", false);

                if (k == null) continue;

                string strPath = (k.GetValue(string.Empty) as string);
                if (!string.IsNullOrEmpty(strPath))
                    strPath = UrlUtil.GetQuotedAppPath(strPath).Trim();
                else { Debug.Assert(false); }

                k.Close();
                if (!string.IsNullOrEmpty(strPath)) return strPath;
            }

            return null;
        }

        private static string FindChrome()
        {
            string strPath = FindChromeNew();
            if (string.IsNullOrEmpty(strPath))
                strPath = FindChromeOld();
            return strPath;
        }

        // HKEY_CLASSES_ROOT\\ChromeHTML[.ID]\\shell\\open\\command
        private static string FindChromeNew()
        {
            RegistryKey kHtml = Registry.ClassesRoot.OpenSubKey("ChromeHTML", false);
            if (kHtml == null) // New versions append an ID
            {
                string[] vKeys = Registry.ClassesRoot.GetSubKeyNames();
                foreach (string strEnum in vKeys)
                {
                    if (strEnum.StartsWith("ChromeHTML.", StrUtil.CaseIgnoreCmp))
                    {
                        kHtml = Registry.ClassesRoot.OpenSubKey(strEnum, false);
                        break;
                    }
                }

                if (kHtml == null) return null;
            }

            RegistryKey kCommand = kHtml.OpenSubKey("shell\\open\\command", false);
            if (kCommand == null)
            {
                Debug.Assert(false);
                kHtml.Close();
                return null;
            }

            string strPath = (kCommand.GetValue(string.Empty) as string);
            if (!string.IsNullOrEmpty(strPath))
            {
                strPath = strPath.Trim();
                strPath = UrlUtil.GetQuotedAppPath(strPath).Trim();
            }
            else { Debug.Assert(false); }

            kCommand.Close();
            kHtml.Close();
            return strPath;
        }

        // HKEY_CLASSES_ROOT\\Applications\\chrome.exe\\shell\\open\\command
        private static string FindChromeOld()
        {
            RegistryKey kCommand = Registry.ClassesRoot.OpenSubKey(
                "Applications\\chrome.exe\\shell\\open\\command", false);
            if (kCommand == null) return null;

            string strPath = (kCommand.GetValue(string.Empty) as string);
            if (!string.IsNullOrEmpty(strPath))
            {
                strPath = strPath.Trim();
                strPath = UrlUtil.GetQuotedAppPath(strPath).Trim();
            }
            else { Debug.Assert(false); }

            kCommand.Close();
            return strPath;
        }

        // HKEY_LOCAL_MACHINE\\SOFTWARE\\Apple Computer, Inc.\\Safari\\BrowserExe
        private static string FindSafari()
        {
            RegistryKey kSafari = Registry.LocalMachine.OpenSubKey(
                "SOFTWARE\\Apple Computer, Inc.\\Safari", false);
            if (kSafari == null) return null;

            string strPath = (kSafari.GetValue("BrowserExe") as string);
            if (!string.IsNullOrEmpty(strPath))
            {
                strPath = strPath.Trim();
                strPath = UrlUtil.GetQuotedAppPath(strPath).Trim();
            }
            else { Debug.Assert(false); }

            kSafari.Close();
            return strPath;
        }

        private static string FindEdge()
        {
            string strSys = Environment.SystemDirectory.TrimEnd(
                UrlUtil.LocalDirSepChar);
            if (strSys.EndsWith("32"))
                strSys = strSys.Substring(0, strSys.Length - 2);
            strSys += "Apps";

            if (!Directory.Exists(strSys)) return null;

            string[] vEdgeDirs = Directory.GetDirectories(strSys,
                "Microsoft.MicrosoftEdge*", SearchOption.TopDirectoryOnly);
            if (vEdgeDirs == null) { Debug.Assert(false); return null; }

            foreach (string strEdgeDir in vEdgeDirs)
            {
                string strExe = UrlUtil.EnsureTerminatingSeparator(
                    strEdgeDir, false) + "MicrosoftEdge.exe";
                if (File.Exists(strExe)) return strExe;
            }

            return null;
        }


    }
    class UrlUtil
    {
        private static readonly char[] m_vDirSeps = new char[] {
            '\\', '/', UrlUtil.LocalDirSepChar };
        private static readonly char[] m_vPathTrimCharsWs = new char[] {
            '\"', ' ', '\t', '\r', '\n' };
        public static char LocalDirSepChar
        {
            get { return Path.DirectorySeparatorChar; }
        }
        /// <summary>
        /// Ensure that a path is terminated with a directory separator character.
        /// </summary>
        /// <param name="strPath">Input path.</param>
        /// <param name="bUrl">If <c>true</c>, a slash (<c>/</c>) is appended to
        /// the string if it's not terminated already. If <c>false</c>, the
        /// default system directory separator character is used.</param>
        /// <returns>Path having a directory separator as last character.</returns>
        public static string EnsureTerminatingSeparator(string strPath, bool bUrl)
        {
            Debug.Assert(strPath != null); if (strPath == null) throw new ArgumentNullException("strPath");

            int nLength = strPath.Length;
            if (nLength <= 0) return string.Empty;

            char chLast = strPath[nLength - 1];

            for (int i = 0; i < m_vDirSeps.Length; ++i)
            {
                if (chLast == m_vDirSeps[i]) return strPath;
            }

            if (bUrl) return (strPath + '/');
            return (strPath + UrlUtil.LocalDirSepChar);
        }
        public static string GetQuotedAppPath(string strPath)
        {
            if (strPath == null) { Debug.Assert(false); return string.Empty; }

            // int nFirst = strPath.IndexOf('\"');
            // int nSecond = strPath.IndexOf('\"', nFirst + 1);
            // if((nFirst >= 0) && (nSecond >= 0))
            //	return strPath.Substring(nFirst + 1, nSecond - nFirst - 1);
            // return strPath;

            string str = strPath.Trim();
            if (str.Length <= 1) return str;
            if (str[0] != '\"') return str;

            int iSecond = str.IndexOf('\"', 1);
            if (iSecond <= 0) return str;

            return str.Substring(1, iSecond - 1);
        }
    }
    class StrUtil
    {
        public const StringComparison CaseIgnoreCmp = StringComparison.OrdinalIgnoreCase;
    }
}
