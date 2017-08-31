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


    }
}
