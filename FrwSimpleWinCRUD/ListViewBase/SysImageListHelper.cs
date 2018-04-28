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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;


namespace FrwSoftware
{
    //from OLV demo project 
    public class SysImageListHelper
    {
        private SysImageListHelper()
        {
        }

        protected ImageList.ImageCollection SmallImageCollection
        {
            get
            {
                if (this.listView != null)
                    return this.listView.SmallImageList.Images;
                if (this.treeView != null)
                    return this.treeView.ImageList.Images;
                return null;
            }
        }

        protected ImageList.ImageCollection LargeImageCollection
        {
            get
            {
                if (this.listView != null)
                    return this.listView.LargeImageList.Images;
                return null;
            }
        }

        protected ImageList SmallImageList
        {
            get
            {
                if (this.listView != null)
                    return this.listView.SmallImageList;
                if (this.treeView != null)
                    return this.treeView.ImageList;
                return null;
            }
        }

        protected ImageList LargeImageList
        {
            get
            {
                if (this.listView != null)
                    return this.listView.LargeImageList;
                return null;
            }
        }


        /// <summary>
        /// Create a SysImageListHelper that will fetch images for the given tree control
        /// </summary>
        /// <param name="treeView">The tree view that will use the images</param>
        public SysImageListHelper(TreeView treeView)
        {
            if (treeView.ImageList == null)
            {
                treeView.ImageList = new ImageList();
                treeView.ImageList.ImageSize = new Size(16, 16);
            }
            this.treeView = treeView;
        }
        protected TreeView treeView;

        /// <summary>
        /// Create a SysImageListHelper that will fetch images for the given listview control.
        /// </summary>
        /// <param name="listView">The listview that will use the images</param>
        /// <remarks>Listviews manage two image lists, but each item can only have one image index.
        /// This means that the image for an item must occur at the same index in the two lists. 
        /// SysImageListHelper instances handle this requirement. However, if the listview already
        /// has image lists installed, they <b>must</b> be of the same length.</remarks>
        public SysImageListHelper(ObjectListView listView)
        {
            if (listView.SmallImageList == null)
            {
                listView.SmallImageList = new ImageList();
                listView.SmallImageList.ColorDepth = ColorDepth.Depth32Bit;
                listView.SmallImageList.ImageSize = new Size(16, 16);
            }

            if (listView.LargeImageList == null)
            {
                listView.LargeImageList = new ImageList();
                listView.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
                listView.LargeImageList.ImageSize = new Size(32, 32);
            }

            //if (listView.SmallImageList.Images.Count != listView.LargeImageList.Images.Count)
            //    throw new ArgumentException("Small and large image lists must have the same number of items.");

            this.listView = listView;
        }
        protected ObjectListView listView;

        /// <summary>
        /// Return the index of the image that has the Shell Icon for the given file/directory.
        /// </summary>
        /// <param name="path">The full path to the file/directory</param>
        /// <returns>The index of the image or -1 if something goes wrong.</returns>
        public int GetImageIndex(string path)
        {
            if (System.IO.Directory.Exists(path))
                path = System.Environment.SystemDirectory; // optimization! give all directories the same image
            else
                if (System.IO.Path.HasExtension(path))
                path = System.IO.Path.GetExtension(path);

            if (this.SmallImageCollection.ContainsKey(path))
                return this.SmallImageCollection.IndexOfKey(path);

            try
            {
                this.AddImageToCollection(path, this.SmallImageList, ShellApi.GetFileIcon(path, true, true));
                this.AddImageToCollection(path, this.LargeImageList, ShellApi.GetFileIcon(path, false, true));
            }
            catch (ArgumentNullException)
            {
                return -1;
            }

            return this.SmallImageCollection.IndexOfKey(path);
        }

        private void AddImageToCollection(string key, ImageList imageList, Icon image)
        {
            if (imageList == null)
                return;

            if (imageList.ImageSize == image.Size)
            {
                imageList.Images.Add(key, image);
                return;
            }

            using (Bitmap imageAsBitmap = image.ToBitmap())
            {
                Bitmap bm = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height);
                Graphics g = Graphics.FromImage(bm);
                g.Clear(imageList.TransparentColor);
                Size size = imageAsBitmap.Size;
                int x = Math.Max(0, (bm.Size.Width - size.Width) / 2);
                int y = Math.Max(0, (bm.Size.Height - size.Height) / 2);
                g.DrawImage(imageAsBitmap, x, y, size.Width, size.Height);
                imageList.Images.Add(key, bm);
            }
        }

    }
}
