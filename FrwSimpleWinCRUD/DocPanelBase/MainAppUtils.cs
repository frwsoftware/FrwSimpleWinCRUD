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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using FrwSoftware;

namespace FrwSoftware
{
    public enum DeviceCap
    {
        VERTRES = 10,
        DESKTOPVERTRES = 117,

        // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
    }

    public class MainAppUtils
    {
        private static Mutex m = null;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
        //[System.Runtime.InteropServices.DllImport("shcore.dll")]//It is not available on Windows 7
        //static extern int SetProcessDpiAwareness(_Process_DPI_Awareness value);
        //enum _Process_DPI_Awareness
        //{
        //    Process_DPI_Unaware = 0,
        //    Process_System_DPI_Aware = 1,
        //    Process_Per_Monitor_DPI_Aware = 2
        //}
        /// <summary>
        /// dc
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        static public void AjustVideoSetting()
        {
            bool setProcessDPIAware = Properties.Settings.Default.SetProcessDPIAware;

            //https://msdn.microsoft.com/ru-ru/library/windows/desktop/dn469266.aspx
            //https://toster.ru/q/230782 
            //Once you go past 100 % (or 125 % with the "XP-style DPI scaling" checkbox ticked), 
            //Windows by default takes over the scaling of your UI. 
            //It does so by having your app render its output to a bitmap and drawing that bitmap to the screen.
            //The rescaling of that bitmap makes the text inevitably look fuzzy.A feature called "DPI virtualization", it keeps old programs usable on high resolution monitors.
            //http://stackoverflow.com/questions/13228185/how-to-configure-an-app-to-run-correctly-on-a-machine-with-a-high-dpi-setting-e/13228495#13228495
            //Этот вызов должен быть в самом начале программы иначе он неправильно отрабатываетя 
            if (setProcessDPIAware)
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDPIAware();
                    //SetProcessDpiAwareness(_Process_DPI_Awareness.Process_Per_Monitor_DPI_Aware);
                }
            }
        }

        static public void CheckForSingleInstance()
        {
            string appName = System.AppDomain.CurrentDomain.FriendlyName;
            //test for single instance 
            bool mutexCreated;
            m = new Mutex(false, appName, out mutexCreated);
            if (!mutexCreated)
            {
                MessageBox.Show(null, FrwCRUDRes.Application_Allready_Running, FrwCRUDRes.WARNING,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }
        }

        static public void InitAppPaths()
        {
            //crearte config manager instance 
            if (!FrwConfig.IsInstanceSet)
            {
                FrwConfig.Instance = new FrwSimpleWinCRUDConfig();
            }
            ////////////////////////////////////////////
            //set paths
            ////////////////////////////////////////////
            //executing assemply path
            string runPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            //Global directory. 
            //In the simplest case, the global directory is the same as the program's start directory.
            //But sometimes it is convenient to define it in a special way. For example, then you can work 
            //with the same database files and settings in debug and working mode without permanently copying 
            //them to the working directory from the project directory and back. 
            if (FrwConfig.Instance.GlobalDir == null)
            {
                FrwConfig.Instance.GlobalDir = runPath;
            }
            else if (!Path.IsPathRooted(FrwConfig.Instance.GlobalDir))
            {
                FrwConfig.Instance.GlobalDir = Path.Combine(runPath, FrwConfig.Instance.GlobalDir);
            }
            //profile directory (database, saving state of winform objects, user settings, etc.) 
            if (FrwConfig.Instance.ProfileDir == null)
            {
                FrwConfig.Instance.ProfileDir = Path.Combine(FrwConfig.Instance.GlobalDir, FrwConfig.DEFAULT_PROFILE_PREFIX);
            }
            else if (!Path.IsPathRooted(FrwConfig.Instance.ProfileDir))
            {
                FrwConfig.Instance.ProfileDir = Path.Combine(runPath, FrwConfig.Instance.ProfileDir);
            }
            // mode definition: working or debugging
            FrwConfig.Instance.DevelopMode = (runPath.IndexOf("\\Debug\\") > -1 || runPath.IndexOf("\\Release\\") > -1);
            //direcory for user settinns that attached to this computer 
            FrwConfig.Instance.ComputerUserDir = new FileInfo(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath).DirectoryName;  //comp specific settings
            //check for exists
            if (!Directory.Exists(FrwConfig.Instance.GlobalDir)) new Exception(FrwCRUDRes.Application_Folder_Not_Found + FrwConfig.Instance.GlobalDir);
            if (!Directory.Exists(FrwConfig.Instance.ProfileDir)) new Exception(FrwCRUDRes.Application_Folder_Not_Found + FrwConfig.Instance.ProfileDir);
            if (!Directory.Exists(FrwConfig.Instance.ComputerUserDir)) new Exception(FrwCRUDRes.Application_Folder_Not_Found + FrwConfig.Instance.ComputerUserDir);
            //directory for temp
            FrwConfig.Instance.UserTempDir = Path.GetTempPath();
            //load settings 
            FrwConfig.Instance.LoadConfig();
            //create database manager instance (includes database loading and relationship resolving) 
            Dm.Instance.Init();
            //attache entities to settings 
            FrwConfig.Instance.ComplateSettingsRelations();
        }

        static public void DestroyApp()
        {
            try
            {
                Dm.Instance.Destroy();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            try
            {
                FrwConfig.Instance.SaveConfig();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            //Log.ProcessDebug("Application ended");
            try
            {
                Log.CloseAndFlush();
            }
            catch (Exception)
            {
            }

        }
    }
}
