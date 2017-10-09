﻿/**********************************************************************************
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
                isUrl = ! u.IsFile;
            }
            catch (Exception)
            {
                isFile = false;
                isUrl = false;
            }
        }
    }
}
