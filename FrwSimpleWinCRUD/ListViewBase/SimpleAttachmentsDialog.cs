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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FrwSoftware;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FrwSoftware
{
    public partial class SimpleAttachmentsDialog : BaseDialogForm
    {
        public string CommonStoragePath { get; set;}
        public string StoragePrefixPath { get; set; }

        private List<JAttachment> sourceObjects;
        public List<JAttachment> SourceObjects {
            get
            {
                return sourceObjects;
            }
            set
            {
                sourceObjects = value;
                listView.SetObjects(value);
            }
        }

        public SimpleAttachmentsDialog()
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleAttachmentsDialog_Attachments;

            ((System.ComponentModel.ISupportInitialize)(listView)).BeginInit();
            //ovl settings 
            listView.EmptyListMsg = FrwCRUDRes.List_No_Records;
            listView.EmptyListMsgFont = new Font("Tahoma", 9);//todo;
            listView.FullRowSelect = true;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = System.Windows.Forms.View.Details;
            listView.UseFiltering = true; 
            listView.UseFilterIndicator = true;
            listView.AllowColumnReorder = true;
            listView.TriStateCheckBoxes = false;
            listView.CellEditUseWholeCell = false;
            listView.TintSortColumn = true;
            listView.ShowItemToolTips = true;
            listView.UseHotItem = true;
            listView.UseHyperlinks = true;
            listView.HyperlinkClicked += ListView_HyperlinkClicked;
            listView.ShowCommandMenuOnRightClick = true;
            listView.TintSortColumn = true;
            //special for selection list
            //listView.CheckBoxes = true;
            listView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
            listView.CellToolTipShowing += delegate (object sender, ToolTipShowingEventArgs e) {
               if (e.Column.Text == FrwCRUDRes.SimpleAttachmentsDialog_Path)
               {
                   JAttachment item = (JAttachment)e.Model;
                   string path = GetRealPath(item.Path);
                   FileSystemInfo x = GetFileSystemInfo(path);
                   if (x != null)
                   {
                       e.Text = x.FullName;
                   }
                   //e.Text = String.Format("Tool tip for '{0}', column '{1}'\r\nValue shown: '{2}'", e.Model, e.Column.Text, e.SubItem.Text);
               }
               //else e.Text = 
            };
            SysImageListHelper helper = new SysImageListHelper(this.listView);//todo вынести в кешируемый уровень 

            OLVColumn column = null;
            column = new OLVColumn();
            column.AspectName = "Path";
            column.Text = FrwCRUDRes.SimpleAttachmentsDialog_Path;
            column.Width = 350;
            column.Hyperlink = true;
            column.FillsFreeSpace = true;
            column.AspectGetter = delegate (object xx)
            {
                JAttachment item = (JAttachment)xx;
                if (item.Path != null && item.Path.StartsWith(Dm.STORAGE_PREFIX))
                {

                    string path = GetRealPath(item.Path);
                    FileSystemInfo x = GetFileSystemInfo(path);
                    if (x == null) return null;
                    return x.Name;
                }
                else return item.Path;



            };
            column.ImageGetter = delegate (object x)
            {
                try
                {
                    JAttachment item = (JAttachment)x;
                    string path = GetRealPath(item.Path);
                    FileSystemInfo fi = GetFileSystemInfo(path);
                    if (fi == null) return null;
                    return helper.GetImageIndex((fi).FullName);
                }
                catch(Exception)
                {
                    return null;
                }
            };
            AddColumnToList(column);

            column = new OLVColumn();
            column.AspectName = "Path";
            column.Text = FrwCRUDRes.SimpleAttachmentsDialog_Size;
            column.ButtonPadding = new Size(10, 10);
            column.HeaderTextAlign = HorizontalAlignment.Right;
            column.TextAlign = HorizontalAlignment.Right;
            column.Width = 80;
            column.AspectGetter = delegate (object xx) {
                JAttachment item = (JAttachment)xx;
                string path = GetRealPath(item.Path);
                FileSystemInfo x = GetFileSystemInfo(path);
                if (x == null) return null;

                if (x is DirectoryInfo)
                    return (long)-1;

                try
                {
                    return ((FileInfo)x).Length;
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Mono 1.2.6 throws this for hidden files
                    return (long)-2;
                }
            };
            column.AspectToStringConverter = delegate (object x) {
                if (x == null) return null; //file not exist
                long sizeInBytes = (long)x;
                if (sizeInBytes < 0) // folder or error
                    return "";
                return FormatUtils.FormatFileSize(sizeInBytes);
            };
            AddColumnToList(column);

            column = new OLVColumn();
            column.AspectName = "Path";
            column.Text = FrwCRUDRes.SimpleAttachmentsDialog_CreationDate;
            column.Width = 130;
            column.AspectGetter = delegate (object xx) {
                JAttachment item = (JAttachment)xx;
                string path = GetRealPath(item.Path);
                FileSystemInfo x = GetFileSystemInfo(path);
                if (x == null) return null;
                return x.CreationTime;

            };          
            AddColumnToList(column);

            column = new OLVColumn();
            column.AspectName = "Path";
            column.Text = FrwCRUDRes.SimpleAttachmentsDialog_ModificationDate;
            column.Width = 130;
            column.AspectGetter = delegate (object xx) {
                JAttachment item = (JAttachment)xx;
                string path = GetRealPath(item.Path);
                FileSystemInfo x = GetFileSystemInfo(path);
                if (x == null) return null;
                return x.LastWriteTime;

            };                   
            AddColumnToList(column);

            column = new OLVColumn();
            column.AspectName = "Path";
            column.Text = FrwCRUDRes.SimpleAttachmentsDialog_FileType;
            column.Width = 130;
            column.AspectGetter = delegate (object xx) {
                JAttachment item = (JAttachment)xx;
                string path = GetRealPath(item.Path);
                FileSystemInfo x = GetFileSystemInfo(path);
                if (x == null) return null;

                return ShellApi.GetFileType(((FileSystemInfo)x).FullName);
            };
            AddColumnToList(column);

            ((System.ComponentModel.ISupportInitialize)(listView)).EndInit();
        }

        private void ListView_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e)
        {
            JAttachment item = (JAttachment)e.Model;
            if (item.Path != null && item.Path.StartsWith(Dm.STORAGE_PREFIX))
            {
                string path = GetRealPath(item.Path);
                FileSystemInfo x = GetFileSystemInfo(path);
                if (x != null)
                {
                    e.Url = x.FullName;
                }
            }
        }

        private FileSystemInfo GetFileSystemInfo(string path)
        {
            FileSystemInfo fi = null;
            if (Directory.Exists(path))
                fi = new DirectoryInfo(path);
            else if (File.Exists(path))
                fi = new FileInfo(path);
            else return null;
            return fi;
        }
        protected void AddColumnToList(OLVColumn column)
        {
            listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { column });//may be only TreeList ? 
            listView.AllColumns.Add(column);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (sourceObjects == null)
                sourceObjects = new List<JAttachment>();
            sourceObjects.Clear();
            foreach (var o in listView.Objects)
            {
                sourceObjects.Add((JAttachment)o);
            }
        }


        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                //cheched
                bool checkedPresent = false;
                List<object> oToRemove = new List<object>();
                foreach (var o in listView.Objects)
                {
                    if (listView.IsChecked(o))
                    {
                        checkedPresent = true;
                        oToRemove.Add(o);

                    }
                }
                if (oToRemove.Count > 0)
                    this.listView.RemoveObjects(oToRemove);
                //selected
                if (!(listView.SelectedObjects != null && listView.SelectedObjects.Count > 0))
                {
                    if (!checkedPresent)
                    {
                        MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                    }
                    return;
                }
                listView.RemoveObjects(listView.SelectedObjects);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }

        private void addFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Install the following NuGet package to get the required assemblies (Tools->Library Package Manager->Package Manager Console): https://www.nuget.org/packages/Windows7APICodePack-Shell/
                var dlg = new CommonOpenFileDialog();
                dlg.AllowNonFileSystemItems = false;
                dlg.Multiselect = true;
                dlg.IsFolderPicker = false;
                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var f in dlg.FileNames)
                    {
                        //copy file 
                        string objectStorageDir = Path.Combine(CommonStoragePath, StoragePrefixPath);
                        string fileName = (new FileInfo(f)).Name;
                        string random = DataUtils.genKey(null);
                        string newDiskDir = Path.Combine(objectStorageDir, random);
                        string newDiskFileName = Path.Combine(newDiskDir, fileName);
                        if (!Directory.Exists(newDiskDir))
                            Directory.CreateDirectory(newDiskDir);
                        string newStoreFileName = Path.Combine(Path.Combine(Dm.STORAGE_PREFIX, random), fileName);
                        Cursor cursor = Cursor.Current;
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            File.Copy(f, newDiskFileName);
                        }
                        finally
                        {
                            Cursor.Current = cursor;
                        }
                        //new path 
                        JAttachment at = new JAttachment();
                        at.Path = newStoreFileName;
                        this.listView.AddObject(at);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }

        private void addFilePathButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Install the following NuGet package to get the required assemblies (Tools->Library Package Manager->Package Manager Console): https://www.nuget.org/packages/Windows7APICodePack-Shell/
                var dlg = new CommonOpenFileDialog();
                dlg.AllowNonFileSystemItems = false;
                dlg.Multiselect = true;
                dlg.IsFolderPicker = false;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var f in dlg.FileNames)
                    {
                        JAttachment at = new JAttachment();
                        at.Path = f;
                        this.listView.AddObject(at);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void addURLButton_Click(object sender, EventArgs e)
        {
            try
            {
                SimpleTextDialog dlg = new SimpleTextDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    JAttachment at = new JAttachment();
                    at.Path = dlg.TextToEdit;
                    this.listView.AddObject(at);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }

  
        private void addDirPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Install the following NuGet package to get the required assemblies (Tools->Library Package Manager->Package Manager Console): https://www.nuget.org/packages/Windows7APICodePack-Shell/
                var dlg = new CommonOpenFileDialog();
                dlg.AllowNonFileSystemItems = false;
                dlg.Multiselect = true;
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (var f in dlg.FileNames)
                    {
                        JAttachment at = new JAttachment();
                        at.Path = f;
                        this.listView.AddObject(at);
                    }
                }
                /* very ugly dialog
                  FolderBrowserDialog fbd = new FolderBrowserDialog();
                  DialogResult result = fbd.ShowDialog();
                  if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                  {
                      string[] files = Directory.GetFiles(fbd.SelectedPath);
                      System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                  }
                */
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFileOrFolder(false);
        }

        private void openDirButton_Click(object sender, EventArgs e)
        {
            openFileOrFolder(true);
        }

        private string GetRealPath(string path)
        {
            if (path != null && path.StartsWith(Dm.STORAGE_PREFIX))
            {
                path = path.Substring(Dm.STORAGE_PREFIX.Length);
                path = Path.Combine(Path.Combine(CommonStoragePath, StoragePrefixPath), path);
            }
            return path;
        }

        private void openFileOrFolder(bool openAsFolder)
        {
            try
            {
                if (listView.SelectedObjects != null && listView.SelectedObjects.Count > 1) MessageBox.Show(FrwCRUDRes.List_Selected_More_Than_On_record);
                else if (listView.SelectedObjects == null || listView.SelectedObjects.Count == 0) MessageBox.Show(FrwCRUDRes.List_No_Selected_Records);
                else
                {
                    JAttachment selectedObject = null;
                    if (listView.SelectedIndex > -1) selectedObject = listView.GetItem(listView.SelectedIndex).RowObject as JAttachment;
                    string path = GetRealPath(selectedObject.Path);
                    if (openAsFolder)
                    {
                        if (Directory.Exists(path) == false)
                        {
                            FileInfo fi = new FileInfo(path);
                            path = fi.Directory.FullName;
                        } 
                    }
                    ProcessUtils.OpenFile(path);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }

    }
}
