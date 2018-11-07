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
    public class FileUtils
    {
        static public string FILE_URL_PREFIX = "file:///";


        static public bool CheckPathIsDirectory(string path)
        {
            FileInfo pFinfo = new FileInfo(path);
            if (pFinfo.Exists)
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directo‌​ry))
                {
                    return true;
                }
            }
            return false;
        }

        static public string GetDirectorNameForPath(string path)
        {
            return Path.GetFullPath(Path.GetDirectoryName(path));
        }
        static public void IsFilePath(string path, out bool isFile, out bool isUrl)
        {
            Uri u;
            try
            {
                u = new Uri(path);
                isFile = u.IsFile;
                isUrl = !u.IsFile;
            }
            catch (Exception)
            {
                isFile = false;
                isUrl = false;
            }
        }
        /*
        static public bool IsHtmlFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            string ext = fi.Extension;
            if (".htm".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else if (".html".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
        */
        static public bool IsHtmlFileWithBookmark(string path)
        {
            int i = path.IndexOf(".htm#");
            if (i > -1) return true;
            i = path.IndexOf(".html#");
            if (i > -1) return true;
            else return false;
        }
        public static string FilePathWithBookmarkToFileUrl(string filePath)
        {
            int i = filePath.IndexOf("#");
            if (i > -1)
            {
                return (new Uri(filePath.Substring(0, i))).AbsoluteUri + filePath.Substring(i);
            }
            else return new Uri(filePath).AbsoluteUri;
        }

        public static string FileUrlToFilePath(string url) {
            //todo decode
            if (url.StartsWith(FILE_URL_PREFIX)) return url.Substring(FILE_URL_PREFIX.Length).Replace("/", "\\");
            else return url;
        }


        //not in use 
        //from https://stackoverflow.com/questions/1546419/convert-file-path-to-a-file-uri
        //
        public static string FilePathToFileUrl(string filePath)
        {
            StringBuilder uri = new StringBuilder();
            foreach (char v in filePath)
            {
                if ((v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9') ||
                  v == '+' || v == '/' || v == ':' || v == '.' || v == '-' || v == '_' || v == '~' ||
                  v > '\xFF')
                {
                    uri.Append(v);
                }
                else if (v == Path.DirectorySeparatorChar || v == Path.AltDirectorySeparatorChar)
                {
                    uri.Append('/');
                }
                else
                {
                    uri.Append(String.Format("%{0:X2}", (int)v));
                }
            }
            if (uri.Length >= 2 && uri[0] == '/' && uri[1] == '/') // UNC path
                uri.Insert(0, "file:");
            else
                uri.Insert(0, "file:///");
            return uri.ToString();
        }
        //delete directory content and directory
        public static void DeleteDirectory(string dirName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);
            if (dirInfo.Exists == true)
            {
                Directory.Delete(dirName, true);
            }
            int count = 0;
            while (Directory.Exists(dirName) == true && count < 1000)
            {
                //Had this problem under Windows 7, .Net 4.0, VS2010. It would appear Directory.Delete() is not synchronous so the issue is timing. I'm guessing Directory.CreateDirectory() does not fail because the existing folder is marked for deletion. The new folder is then dropped as Directory.Delete() finishes.
                //http://stackoverflow.com/questions/35069311/why-sometimes-directory-createdirectory-fails
                System.Threading.Thread.Sleep(10);
                count++;
            }
        }
        public static void CreateDirectory(string dirName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);
            if (dirInfo.Exists == false) Directory.CreateDirectory(dirInfo.FullName);
            int count = 0;
            while (Directory.Exists(dirName) == false && count < 1000)
            {
                //Had this problem under Windows 7, .Net 4.0, VS2010. It would appear Directory.Delete() is not synchronous so the issue is timing. I'm guessing Directory.CreateDirectory() does not fail because the existing folder is marked for deletion. The new folder is then dropped as Directory.Delete() finishes.
                //http://stackoverflow.com/questions/35069311/why-sometimes-directory-createdirectory-fails
                System.Threading.Thread.Sleep(10);
                count++;
            }
        }

        //remove directory content only
        public static void DeleteDirectoryContent(string dirName)
        {
            if (Directory.Exists(dirName))
            {
                DirectoryInfo di = new DirectoryInfo(dirName);

                foreach (FileInfo file in di.GetFiles())
                    file.Delete();

                foreach (DirectoryInfo dir in di.GetDirectories())
                    DeleteDirectory(dir.FullName);
            }

        }
        public static void CreateOrClearDirectory(string dirName)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);
            if (dirInfo.Exists == false)
            {
                CreateDirectory(dirName);
            }
            else
            {
                DeleteDirectoryContent(dirName);
            }

        }

    }
}
