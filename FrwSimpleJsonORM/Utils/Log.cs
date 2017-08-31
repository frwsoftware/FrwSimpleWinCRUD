using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using System.Windows.Forms;
using System.Text;

namespace FrwSoftware
{

    public class EventLogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
    public delegate void EventLogEventHandler(object sender, EventLogEventArgs e);


    //strategy

    // logError
    // minor errors are output only to the log (in fact, this is analogous to simply calling the logger)

    // eventError
    // errors of background rectification, which do not prevent further execution of the program, are output to the log and system of events with indication in the status line

    // processError
    // interactive errors in the process of user actions are output to the log and message box
    // this output is not allowed in background processes
    // the handlers of such errors should be on the self-level top level of the user operation
    // this also prevents multiple input of a message
    public class Log
    {
        private ILog log4net = null;//todo static or not?
        private static bool configured = false;

        public Log()
        {
            configureLogs();
            log4net = LogManager.GetLogger(typeof(Log));
        }
        public Log(string module_name)
        {
            configureLogs();
            log4net = LogManager.GetLogger(module_name);
        }

        static public Log GetLogger()
        {
            return new Log();
        }
        static public Log GetLogger(string module_name)
        {
            return new Log(module_name);
        }

        static public void CloseAndFlush()
        {
        }

        #region enabled_properties
        public bool isDebugEnabled
        {
            get
            {
                if (log4net != null)
                {
                    return log4net.IsDebugEnabled;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool isInfoEnabled
        {
            get
            {
                if (log4net != null)
                {
                    return log4net.IsInfoEnabled;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool isWarnEnabled
        {
            get
            {
                if (log4net != null)
                {
                    return log4net.IsWarnEnabled;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        private static void configureLogs()
        {
            if (configured == false)
            {
                lock (typeof(Log))
                {
                    if (configured == false)   
                    {
                        configureLog4net();
                        configured = true;
                    }
                }
            }
        }
        private static void configureLog4net()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            FileInfo logConfig = new FileInfo(path + "\\" + "logConfig.xml");
            if (logConfig.Exists)
            {
                XmlConfigurator.Configure(logConfig);//This file path is relative to the application base directory (AppDomain.CurrentDomain.BaseDirectory). 
            }
            else
            {
                BasicConfigurator.Configure();
            }
        }

        #region log functions
        public void Info(string message)
        {
            if (log4net != null) log4net.Info(message);
        }
        public void Debug(string message)
        {
            if (log4net != null) log4net.Debug(message);
        }
        public void Fatal(string message)
        {
            if (log4net != null) log4net.Fatal(message);
        }
        public void Fatal(string message, Exception e)
        {
            if (log4net != null) log4net.Fatal(message, e);
        }
        public void Error(string message)
        {
            if (log4net != null) log4net.Error(message);
        }
        public void Error(string message, Exception e)
        {
            if (log4net != null) log4net.Error(message, e);
        }
        public void Warn(string message)
        {
            if (log4net != null) log4net.Warn(message);
        }
        public void Warn(string message, Exception e)
        {
            if (log4net != null) log4net.Warn(message, e);
        }
        #endregion
        #region trace utils

        static Log log = null;
        static public event EventLogEventHandler EventLogEvent;

        //static public 
        //A group of methods that output a message to the log  
        static public void LogError(string message)
        {
            fullProcessError(message, null, true, false, false);
        }
        static public void LogError(string message, Exception e = null)
        {
            fullProcessError(message, e, true, false, false);
        }
        static public void LogError(Exception e)
        {
            fullProcessError(null, e, true, false, false);
        }

        //A group of methods that output a message to the log and to the event system  
        static public void EventError(string message)
        {
            fullProcessError(message, null, true, false, true);
        }
        static public void EventError(string message, Exception e)
        {
            fullProcessError(message, e, true, false, true);
        }
        static public void EventError(Exception e)
        {
            fullProcessError(null, e, true, false, true);
        }

        //A group of methods that output a message to the log and to the messagebox 
        static public void ShowError(string message)
        {
            fullProcessError(message, null, true, true, false);
        }
        static public void ShowError(string message, Exception e)
        {
            fullProcessError(message, e, true, true, false);
        }
        static public void ShowError(Exception e)
        {
            fullProcessError(null, e, true, true, false);
        }

        static private void fullProcessError(string message, Exception e, bool writeToLog, bool showMessageBox, bool writeToEventLog)
        {
            if (message == null)
            {
                message = "";
            }
            if (e != null)
            {
                if (e is System.Runtime.InteropServices.COMException)
                {
                    int hr = ((System.Runtime.InteropServices.COMException)e).ErrorCode;
                    message = message + String.Format(" COM error.\nHRESULT = {0}\n{1}", hr, e.Message);
                }
                else
                {
                    message = message + e.Message;
                }
            }
            if (writeToLog)
            {
                if (log == null)
                {
                    log = Log.GetLogger();
                }
                log.Error(message, e);
            }
            if (writeToEventLog)
            {
                if (EventLogEvent != null)
                {
                    EventLogEvent(null, new EventLogEventArgs() { Message = message, Exception = e });
                }
            }
            if (showMessageBox)
            {
                MessageBox.Show(message, FrwUtilsRes.Log_Error_In_Work,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static public void ProcessDebug(string message)
        {
            if (log == null)
            {
                log = Log.GetLogger();
            }
            log.Debug(message);
        }

        public static string PropertyList(object obj)
        {
            var props = obj.GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                sb.AppendLine(p.Name + ": " + p.GetValue(obj, null));
            }
            return sb.ToString();
        }

        #endregion



    }
}
