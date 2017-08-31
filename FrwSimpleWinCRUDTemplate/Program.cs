using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


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
            MainAppUtils.AjustVideoSetting();
            try
            {
                Form form = null;
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    MainAppUtils.CheckForSingleInstance();
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    log = Log.GetLogger();
                    MainAppUtils.InitAppPaths();
                    AppManager.Instance.MainAppFormType = typeof(FrwTemplateMainForm);
                    form = AppManager.Instance.LoadDocPanelContainersState();
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
            AppManager.Instance.SaveDocPanelContainersState();
            AppManager.Instance.CloseAllDocPanelContainers((Form)sender);
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                MainAppUtils.DestroyApp();
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
