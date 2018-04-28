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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware
{
    public class JobLog
    {
        static private string LOG_DATETIME_PATTERN = "yyyy/MM/dd HH:mm:ss ";

        public JobLog ParentLog { get; set; }
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
            sb = new StringBuilder();
            w = new StringWriter(sb);
            WriteToConsole = true; //tmp 
        }

        public string LogString
        {
            get
            {
                return sb.ToString();
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
            DateTime time = DateTime.Now;
            if (e != null) message = message + e;
            string messageWithDate = time.ToString(LOG_DATETIME_PATTERN) + message; 
            w.WriteLine(messageWithDate);
            if (externalWriter != null) externalWriter.WriteLine(messageWithDate);
            if (WriteToConsole) Console.WriteLine(message);
            if (ParentLog != null) ParentLog.WriteLine(message);
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
