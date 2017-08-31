using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FrwSoftware
{
  

    public class ShellApi
    {
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
            public RECT(Int32 left, Int32 right, Int32 top, Int32 bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }

        public enum ShowWindowCommands
        {
            SW_HIDE = 0,    // Hides the window and activates another window.
            SW_SHOWNORMAL = 1,  // Sets the show state based on the SW_ flag specified in the STARTUPINFO 
            SW_NORMAL = 1,  // structure passed to the CreateProcess function by the program that started 
                            // the application.
            SW_SHOWMINIMIZED = 2,   // Activates the window and displays it as a minimized window.
            SW_SHOWMAXIMIZED = 3,   // Maximizes the specified window.
            SW_MAXIMIZE = 3,    // Activates the window and displays it as a maximized window.
            SW_SHOWNOACTIVATE = 4,  // Displays a window in its most recent size and position. The active window remains active.
            SW_SHOW = 5,    // Activates the window and displays it in its current size and position.
            SW_MINIMIZE = 6,    // Minimizes the specified window and activates the next top-level window in the z-order.
            SW_SHOWMINNOACTIVE = 7, // Displays the window as a minimized window. The active window remains active.
            SW_SHOWNA = 8,  // Displays the window in its current state. The active window remains active.
            SW_RESTORE = 9, // Activates and displays the window.
            SW_SHOWDEFAULT = 10,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        private const int MAX_PATH = 260;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes,
            out SHFILEINFO psfi, int cbFileInfo, int uFlags);
        #region icons and file types in OLV file list

        private const int SHGFI_ICON = 0x00100;     // get icon
        private const int SHGFI_DISPLAYNAME = 0x00200;     // get display name
        private const int SHGFI_TYPENAME = 0x00400;     // get type name
        private const int SHGFI_ATTRIBUTES = 0x00800;     // get attributes
        private const int SHGFI_ICONLOCATION = 0x01000;     // get icon location
        private const int SHGFI_EXETYPE = 0x02000;     // return exe type
        private const int SHGFI_SYSICONINDEX = 0x04000;     // get system icon index
        private const int SHGFI_LINKOVERLAY = 0x08000;     // put a link overlay on icon
        private const int SHGFI_SELECTED = 0x10000;     // show icon in selected state
        private const int SHGFI_ATTR_SPECIFIED = 0x20000;     // get only specified attributes
        private const int SHGFI_LARGEICON = 0x00000;     // get large icon
        private const int SHGFI_SMALLICON = 0x00001;     // get small icon
        private const int SHGFI_OPENICON = 0x00002;     // get open icon
        private const int SHGFI_SHELLICONSIZE = 0x00004;     // get shell size icon
        private const int SHGFI_PIDL = 0x00008;     // pszPath is a pidl
        private const int SHGFI_USEFILEATTRIBUTES = 0x00010;     // use passed dwFileAttribute
        //if (_WIN32_IE >= 0x0500)
        private const int SHGFI_ADDOVERLAYS = 0x00020;     // apply the appropriate overlays
        private const int SHGFI_OVERLAYINDEX = 0x00040;     // Get the index of the overlay

        private const int FILE_ATTRIBUTE_NORMAL = 0x00080;     // Normal file
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x00010;     // Directory





        #endregion

        //This Function is used to set active window in Maximized state.
        [DllImport("user32.dll")]
        public static extern int SetWindowPlacement(IntPtr hWnd,
        [In] ref WINDOWPLACEMENT lpwndpl);

        //This Function is used to set a window as active one.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool SetWindowsPlacementMaximized(IntPtr handle)
        {
            ShellApi.WINDOWPLACEMENT pc = new ShellApi.WINDOWPLACEMENT();
            pc.length = Marshal.SizeOf(typeof(ShellApi.WINDOWPLACEMENT));
            pc.showCmd = (int)ShellApi.ShowWindowCommands.SW_SHOWMAXIMIZED;
            ShellApi.SetWindowPlacement(handle, ref pc);
            return true;
        }

        /// <summary>
        /// Get the string that describes the file's type.
        /// </summary>
        /// <param name="path">The file or directory whose type is to be fetched</param>
        /// <returns>A string describing the type of the file, or an empty string if something goes wrong.</returns>
        public static String GetFileType(string path)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            int flags = SHGFI_TYPENAME;
            IntPtr result = SHGetFileInfo(path, 0, out shfi, Marshal.SizeOf(shfi), flags);
            if (result.ToInt32() == 0)
                return String.Empty;
            else
                return shfi.szTypeName;
        }

        /// <summary>
        /// Return the icon for the given file/directory.
        /// </summary>
        /// <param name="path">The full path to the file whose icon is to be returned</param>
        /// <param name="isSmallImage">True if the small (16x16) icon is required, otherwise the 32x32 icon will be returned</param>
        /// <param name="useFileType">If this is true, only the file extension will be considered</param>
        /// <returns>The icon of the given file, or null if something goes wrong</returns>
        public static Icon GetFileIcon(string path, bool isSmallImage, bool useFileType)
        {
            int flags = SHGFI_ICON;
            if (isSmallImage)
                flags |= SHGFI_SMALLICON;

            int fileAttributes = 0;
            if (useFileType)
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
                if (System.IO.Directory.Exists(path))
                    fileAttributes = FILE_ATTRIBUTE_DIRECTORY;
                else
                    fileAttributes = FILE_ATTRIBUTE_NORMAL;
            }

            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr result = SHGetFileInfo(path, fileAttributes, out shfi, Marshal.SizeOf(shfi), flags);
            if (result.ToInt32() == 0)
                return null;
            else
                return Icon.FromHandle(shfi.hIcon);
        }




    }
}
