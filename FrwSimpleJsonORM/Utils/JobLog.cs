using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware;

namespace FrwSoftware
{
    public class JobLog
    {
        //TextWriter w = null;
        //Stream stream = null;

        StringBuilder sb = null;
        StringWriter w = null;
        public bool WriteToConsole { get; set; }
        public string LogFileName { get; set; }


        private TextWriter externalWriter = null; 
        public TextWriter ExternalWriter
        {
            get {
                return externalWriter;
            }
            set {
                //todo sinxr
                value.Write(LogString);
                externalWriter = value;
            }
        }

        public JobLog()
        {
           //stream = new MemoryStream();
            //w = new StreamWriter(stream);
            sb = new StringBuilder();
            w = new StringWriter(sb);
            WriteToConsole = true; //tmp 
        }

        public string LogString
        {
            get
            {
                return sb.ToString();
                /*
                w.Flush();

                // If we dispose the StreamWriter now, it will close 
                // the BaseStream (which is our MemoryStream) which 
                // will prevent us from reading from our MemoryStream
                //DON'T DO THIS - sw.Dispose();

                // The StreamReader will read from the current 
                // position of the MemoryStream which is currently 
                // set at the end of the string we just wrote to it. 
                // We need to set the position to 0 in order to read 
                // from the beginning.
                stream.Position = 0;
                var sr = new StreamReader(stream);
                var myStr = sr.ReadToEnd();
                return myStr;
                */
            }
        }

        public void SaveLogToFile()
        {
            if (LogFileName != null)
            {
                FileInfo logFile = new FileInfo(LogFileName);
                if (logFile.Directory.Exists == false)
                    FileUtils.CreateDirectory(logFile.Directory.FullName);
                File.WriteAllText(LogFileName, LogString);
            }
        }

        private void WriteLine(string message, Exception e = null)
        {
            if (message == null) message = "";
            if (e != null) message = message + e;//todo stack
            w.WriteLine(message);
            if (externalWriter != null) externalWriter.WriteLine(message);
            if (WriteToConsole) Console.WriteLine(message);
        }

        public void Info(string message)
        {
            WriteLine(message);
        }
        public void Debug(string message)
        {
            WriteLine(message);
        }
        public void Fatal(string message)
        {
            WriteLine(message);
        }
        public void Fatal(string message, Exception e)
        {
            WriteLine(message, e);
        }
        public void Error(string message)
        {
            WriteLine(message);
        }
        public void Error(string message, Exception e)
        {
            WriteLine(message, e);
        }
        public void Error(Exception e)
        {
            WriteLine(null, e);
        }
        public void Warn(string message)
        {
            WriteLine(message);
        }
        public void Warn(string message, Exception e)
        {
            WriteLine(message, e);
        }

    }
}
