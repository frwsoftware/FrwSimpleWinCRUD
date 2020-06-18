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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using FrwSoftware.Model.Chinook;

namespace FrwSoftware
{
    static class Program
    {
        private static Log log;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppManager.AjustVideoSetting();
            try
            {
                Form form = null;
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (!AppManager.CheckForSingleInstance()) return;
                    
                    //for testing, remove this lines in real app
                    var culture = new CultureInfo("en-US");
                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;

                    log = Log.GetLogger();
                    AppManager.Instance = new DemoAppManager();
                    //crearte config manager instance 
                    if (!FrwConfig.IsInstanceSet)
                    {
                        FrwConfig.Instance = new FrwSimpleWinCRUDConfig();
                    }
                    AppManager.Instance.InitApplication();
                    AppManager.Instance.MainAppFormType = typeof(FrwMainAppForm);
                    form = AppManager.Instance.LoadDocPanelContainersState(true);
                    form.FormClosing += Form_FormClosing;
                    form.FormClosed += Form_FormClosed;

                    Console.WriteLine("FrwConfig.Instance.GlobalDir: " + FrwConfig.Instance.GlobalDir);
                    Console.WriteLine("FrwConfig.Instance.ProfileDir: " + FrwConfig.Instance.ProfileDir);
                    Console.WriteLine("FrwConfig.Instance.ComputerUserDir: " + FrwConfig.Instance.ComputerUserDir);
                    Console.WriteLine("FrwConfig.Instance.UserTempDir: " + FrwConfig.Instance.UserTempDir);

                }
                catch (Exception ex)
                {
                    Log.ShowError("Error start app", ex);
                    Application.Exit();
                }
                if (form != null && !form.IsDisposed)
                {
                    Application.ThreadException += Application_ThreadException;
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError("Error running main app form", ex);
                MessageBox.Show("Unexpected error: " + ex);
                Application.Exit();
            }
        }

        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppManager.Instance.SaveAndClose((Form)sender);
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                AppManager.Instance.DestroyApp();
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            log.Error("OnApplicationThreadException", e.Exception);
        }

    }
}
