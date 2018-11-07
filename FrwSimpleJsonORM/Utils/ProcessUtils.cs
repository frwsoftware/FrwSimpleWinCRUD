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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware
{
    public class ProcessUtils
    {
        static private readonly List<Process> runningProcessesList = new List<Process>();

        public static void OpenFile(string fileName, string arguments = null)
        {
            Process prc = null;

            try
            {
                prc = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = true,//When you use the operating system shell to start processes, you can start any document (which is any registered file type associated with an executable that has a default open action) and perform operations on the file, such as printing, by using the Process object. When UseShellExecute is false, you can start only executables by using the Process object.
                        RedirectStandardOutput = false,
                        //Verb = "open",  //The action to take with the file that the process opens. The default is an empty string (""), which signifies no action. Examples of verbs are "Edit", "Open", "OpenAsReadOnly", "Print", and "Printto".
                        FileName = fileName,
                        Arguments = arguments
                    }
                };
                prc.Start();
            }
            finally
            {
                try
                {
                    if (prc != null) prc.Close();
                }
                catch (Exception)
                {
                }
            }
        }
        public static void ExecuteProgram(string fileName, string arguments, bool waitForExit = false)
        {
            Process prc = null;

            try
            {
                // Set the process startup parameters
                prc = new Process
                {
                    StartInfo =
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        FileName = fileName,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = arguments
                    }
                };

                // Start
                prc.Start();

                if (waitForExit)
                {
                    // wait for exit
                    prc.WaitForExit();
                }
            }
            finally
            {
                try
                {
                    if (prc != null) prc.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        static public bool RunExe(String path, String workingDir, String arguments, bool openMaximized, bool reopenMaximized, bool addToProcessList)
        {
            Log.ProcessDebug("RunExe. Сommand line:  " + path + " Arguments: " + arguments);
            try
            {
                if (!Path.IsPathRooted(path))
                {
                    string root = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    root = Path.GetDirectoryName(root);
                    path = Path.Combine(root, path);
                }
                FileInfo info = new FileInfo(path);
                if (info.Exists)
                {
                    // todo
                    string processName = info.Name.Substring(0, info.Name.Length - 4);
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        ProcessStartInfo pi = new ProcessStartInfo();
                        pi.Verb = "open";
                        pi.FileName = path;
                        if (openMaximized)
                            pi.WindowStyle = ProcessWindowStyle.Maximized;

                        if (workingDir != null)
                            pi.WorkingDirectory = workingDir;

                        if (arguments != null)
                            pi.Arguments = arguments;

                        Process p = new Process();
                        p.EnableRaisingEvents = true;
                        p.Exited += myProcess_Exited;
                        p.StartInfo = pi;
                        p.Start();
                        if (addToProcessList)
                        {
                            lock (runningProcessesList)
                            {
                                clearExitedProcessses();
                                runningProcessesList.Add(p);
                            }
                        }
                    }
                    else
                    {
                        ShellApi.SetForegroundWindow(processes[0].MainWindowHandle);
                        if (reopenMaximized)
                            ShellApi.SetWindowsPlacementMaximized(processes[0].MainWindowHandle);
                    }
                }
                else
                {
                    throw new ArgumentException("File not found:  " + path);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("RunExe error. Сommand line:  " + path + " Arguments: " + arguments, ex);
                return false;
            }

            return true;
        }
        // Handle Exited event and display process information.
        static private void myProcess_Exited(object sender, System.EventArgs e)
        {
            Log.ProcessDebug("myProcess_Exited sender ");// + (sender as Process));
        }

        static private void clearExitedProcessses()
        {
            for (int i = runningProcessesList.Count - 1; i >= 0; i--)
            {
                Process process = runningProcessesList[i];
                if (process.HasExited)
                {
                    runningProcessesList.RemoveAt(i);
                    try { process.Dispose(); } catch { }
                }
            }
        }

        public static void RegisterSomeDll(string dllName)
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.Verb = "open";
            pi.FileName = "cmd.exe";
            pi.Arguments = "/C regsvr32 /s " + dllName;
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.WorkingDirectory = "DLL";
            try
            {
                Process p = Process.Start(pi);
                p.WaitForExit(5000);
                if (!p.HasExited)
                {
                    p.Kill();
                }
                p.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Reset
        /// </summary>
        public static void RestartComputer()
        {
            ExecuteProgram("cmd", "/C shutdown -f -r -t 1");
        }

        /// <summary>
        /// Shutdown
        /// </summary>
        public static void ShutdownComputer()
        {
            ExecuteProgram("cmd", "/C shutdown -f -s -t 1");
        }



    }
}
