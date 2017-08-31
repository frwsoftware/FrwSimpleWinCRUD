﻿using System;
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
using FrwSoftware;
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
                    AppManager.Instance = new DemoAppManager();
                    MainAppUtils.InitAppPaths();
                    AppManager.Instance.MainAppFormType = typeof(FrwMainAppForm);
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
            // before we exit, let forms clean themselves up.
            //if (appManager != null)
            //{
            //myMainApplycationForm.Close();
            AppManager.Instance.SaveDocPanelContainersState();
            AppManager.Instance.CloseAllDocPanelContainers((Form)sender);
            // }
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
