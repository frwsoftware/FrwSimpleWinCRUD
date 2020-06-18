using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class BaseAppManager
    {



        virtual public void InitApplication()
        {
            if (!FrwConfig.IsInstanceSet)
            {
                FrwConfig.Instance = new FrwConfig();
            }
            ////////////////////////////////////////////
            //set paths
            ////////////////////////////////////////////
            //executing assemply path
            string pathPerUserRoamingAndLocal = new FileInfo(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath).DirectoryName;
            string pathAssembly = Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);//возможная альтернатива 
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
            if (!Directory.Exists(FrwConfig.Instance.GlobalDir)) new Exception(FrwUtilsRes.Application_Folder_Not_Found + FrwConfig.Instance.GlobalDir);
            if (!Directory.Exists(FrwConfig.Instance.ProfileDir)) new Exception(FrwUtilsRes.Application_Folder_Not_Found + FrwConfig.Instance.ProfileDir);
            if (!Directory.Exists(FrwConfig.Instance.ComputerUserDir)) new Exception(FrwUtilsRes.Application_Folder_Not_Found + FrwConfig.Instance.ComputerUserDir);
            //directory for temp
            FrwConfig.Instance.UserTempDir = Path.GetTempPath();
            //load settings 
            FrwConfig.Instance.LoadConfig();
            //create database manager instance (includes database loading and relationship resolving) 
            Dm.Instance.Init();
            //attache entities to settings 
            FrwConfig.Instance.ComplateSettingsRelations();

            Dm.UserName = BaseNetworkUtils.GetUserName();
            Dm.CPUId = BaseNetworkUtils.GetCPUId();

            Console.WriteLine("pathPerUserRoamingAndLocal: " + pathPerUserRoamingAndLocal);//C: \Users\DELL\AppData\Local\JustDesktop\JustDesktop.vshost.exe_Url_zzctwk4sz1b53a2f32tyibzjk5zcg2f3\1.0.0.0\user.config
            Console.WriteLine("pathAssembly: " + pathAssembly);//C:\Windows\assembly\GAC_MSIL\Microsoft.VisualStudio.HostingProcess.Utilities\14.0.0.0__b03f5f7f11d50a3a
            Console.WriteLine("runPath: " + runPath);//F:\doc\Just\Output\Debug\JustDesktop
            Console.WriteLine("FrwConfig.Instance.GlobalDir: " + FrwConfig.Instance.GlobalDir);
            Console.WriteLine("FrwConfig.Instance.ProfileDir: " + FrwConfig.Instance.ProfileDir);
            Console.WriteLine("FrwConfig.Instance.ComputerUserDir: " + FrwConfig.Instance.ComputerUserDir);
            Console.WriteLine("FrwConfig.Instance.UserTempDir: " + FrwConfig.Instance.UserTempDir);
            Console.WriteLine("UserName: {0}", Dm.UserName);
            Console.WriteLine("CPU Id: {0}", Dm.CPUId);

        }

        virtual public void DestroyApp()
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
