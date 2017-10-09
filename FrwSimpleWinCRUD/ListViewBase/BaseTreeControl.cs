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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FrwSoftware;

namespace FrwSoftware
{
    public class TreeNodeSelectEventArgs : EventArgs
    {
        public object SelectedOblect { get; set; }
        public bool DbClick { get; set; }
        public bool ShowProperty { get; set; }
        public ViewMode ViewMode { get; set; }

    }
    public delegate void TreeNodeSelectEventHandler(object sender, TreeNodeSelectEventArgs e);
   
    public delegate bool CanExpandGetterDelegate(Object model);
    public delegate IEnumerable ChildrenGetterDelegate(TreeNode parentNode);
    public delegate void AfterEditTreeNodeLabelDelegate(object model, string labelText);
    public delegate void InitialCreateTreeNodeRootDelegate();
    public delegate void ComplateNodeFromObjectDelegate(TreeNode node, object o);

    public class TreeContextMenuSelectEventArgs : EventArgs
    {
        public TreeNode SelectedNode { get; set; }
        public List<ToolStripItem> MenuItemList { get; set; }

    }
    public delegate void TreeContextMenuEventHandler(object sender, TreeContextMenuSelectEventArgs e);

    public class BaseTreeControl : TreeView
    {
        static public string TREE_FOLDER_OPENED = "tree_folder_opened";
        static public string TREE_FOLDER_CLOSED = "tree_folder_closed";
        static public string TREE_FOLDER_OPENED_SELECTED = "tree_folder_opened_selected";
        static public string TREE_FOLDER_CLOSED_SELECTED = "tree_folder_closed_selected";

        public TreeNode RootNode { get; set; }

        public bool ChangeNodeImageOnExpand { get; set; }
        static public string PSEVDO_NODE_TEXT = FrwCRUDRes.TreeList_Loading;
        protected TreeNode m_OldSelectNode;
      
        protected ContextMenuStrip treeContextMenu = null;
        protected System.ComponentModel.IContainer components = null;
        public event TreeContextMenuEventHandler OnTreeContextMenuEvent;
        public event TreeNodeSelectEventHandler OnTreeNodeSelectEvent;

        public IViewProcessor ParentViewProcessor { get; set; }

        public BaseTreeControl()
        {
            components = new System.ComponentModel.Container();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MouseDoubleClick += TreeControl_MouseDoubleClick;
            this.MouseUp += TreeControl_MouseUp;
            this.BeforeExpand += tree_BeforeExpand;
            this.BeforeCollapse += tree_BeforeCollapse;
            this.AfterLabelEdit += new NodeLabelEditEventHandler(tree_AfterLabelEdit);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void FullRefresh()
        {
            this.BeginUpdate();
            RecourseRemove(this.Nodes);
            InitialCreateTreeNodeRoot();
            this.EndUpdate();
        }

        /// <summary>
        /// Load child nodes 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="levelStep">Used for expand until level</param>
        /// <param name="maxLevelStep">Used for expand until level</param>
        /// <param name="savedExpansionState">Used for expand saved state level</param>
        public void LoadBrahch(TreeNode parentNode, int levelStep, int maxLevelStep, IList<string> savedExpansionState)
        {
            levelStep++;
            if (parentNode.Nodes.Count > 0 &&
                parentNode.Nodes[0].Text == PSEVDO_NODE_TEXT)
            {
                parentNode.Nodes[0].Remove();
            }
            if (ChildrenGetter != null)
            {
                IEnumerable oList = ChildrenGetter(parentNode);
                List<TreeNode> nodes = new List<TreeNode>();
                foreach(var o in oList)
                {
                    TreeNode node = new TreeNode();
                    if (ComplateNodeFromObject != null)
                    {
                        ComplateNodeFromObject(node, o);
                    }
                    nodes.Add(node);
                }
                foreach (var node in nodes)
                {
                    if (node.ImageKey == null) node.ImageKey = TREE_FOLDER_CLOSED;
                    if (node.SelectedImageKey == null) node.SelectedImageKey = TREE_FOLDER_CLOSED_SELECTED;
                    // add a pseudo-node (it is necessary to display a plus sign), which is then deleted when the branch is actually loaded
                    if (CanExpandGetter(node.Tag) == true)
                    {
                        node.Nodes.Add(PSEVDO_NODE_TEXT);
                    }
                    parentNode.Nodes.Add(node);
                    if (savedExpansionState != null)
                    {
                        if (savedExpansionState.Contains(node.FullPath))
                        {
                            node.Expand();
                            LoadBrahch(node, levelStep, maxLevelStep, savedExpansionState);
                        }
                    }
                    else if (levelStep < maxLevelStep)
                    {
                        LoadBrahch(node, levelStep, maxLevelStep, savedExpansionState);
                    }
                }
            }
        }


        /// <summary>
        /// This is the delegate that will be used to decide if a model object can be expanded.
        /// </summary>
        public CanExpandGetterDelegate CanExpandGetter
        {
            get { return canExpandGetter; }
            set { canExpandGetter = value; }
        }
        private CanExpandGetterDelegate canExpandGetter;

        public AfterEditTreeNodeLabelDelegate AfterEditTreeNodeLabel
        {
            get { return afterEditTreeNodeLabel; }
            set { afterEditTreeNodeLabel = value; }
        }
        private AfterEditTreeNodeLabelDelegate afterEditTreeNodeLabel;

        public InitialCreateTreeNodeRootDelegate InitialCreateTreeNodeRoot
        {
            get { return initialCreateTreeNodeRoot; }
            set { initialCreateTreeNodeRoot = value; }
        }
        private InitialCreateTreeNodeRootDelegate initialCreateTreeNodeRoot;

        


        /// <summary>
        /// This is the delegate that will be used to fetch the children of a model object
        /// </summary>
        /// <remarks>This delegate will only be called if the CanExpand delegate has 
        /// returned true for the model object.</remarks>
        public ChildrenGetterDelegate ChildrenGetter
        {
            get { return childrenGetter; }
            set { childrenGetter = value; }
        }
        private ChildrenGetterDelegate childrenGetter;

        public ComplateNodeFromObjectDelegate ComplateNodeFromObject
        {
            get { return complateNodeFromObject; }
            set { complateNodeFromObject = value; }
        }
        private ComplateNodeFromObjectDelegate complateNodeFromObject;

        

        //remove node presentation and all childs 
        protected void RecourseRemove(TreeNodeCollection parent)
        {

            List<TreeNode> rem = new List<TreeNode>();
            foreach (TreeNode node in parent)
            {
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    RecourseRemove(node.Nodes);
                    rem.Add(node);
                }
                else
                {
                    rem.Add(node);
                }
            }
            foreach (TreeNode node in rem)
            {
                node.Remove();
            }
        }

        protected void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                if (ChangeNodeImageOnExpand) e.Node.ImageKey = TREE_FOLDER_OPENED;
                if (e.Node.Nodes[0].Text == PSEVDO_NODE_TEXT) //alt if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == null) {
                {
                    this.LoadBrahch(e.Node, 0, 1, null);
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        protected void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            try { 
               if (ChangeNodeImageOnExpand) e.Node.ImageKey = TREE_FOLDER_CLOSED;
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        protected void TreeControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                // Show menu only if the right mouse button is clicked.
                if (e.Button == MouseButtons.Left)
                {
                    // Point where the mouse is clicked.
                    Point p = new Point(e.X, e.Y);
                    // Get the node that the user has clicked.
                    TreeNode node = GetNodeAt(p);
                    if (node != null)
                    {
                        if (node.Bounds.Contains(p))
                        {
                            //"Click on Node";
                            // Select the node the user has clicked.
                            // The node appears selected until the menu is displayed on the screen.
                            m_OldSelectNode = SelectedNode;
                            SelectedNode = node;
                            OnItemActivated();

                        }
                        else
                        {
                            //"Click on plus box or image";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        virtual protected void OnItemActivated()
        {
            if (SelectedNode != null && SelectedNode.Tag != null)
            {

                TreeNodeSelectEventArgs e = new TreeNodeSelectEventArgs();
                e.SelectedOblect = SelectedNode.Tag;
        
                e.DbClick = true;

                this.OnTreeNodeSelectEvent(this, e);

            }
        }
        virtual protected void OnItemSelected()
        {
            if (SelectedNode != null)
            {
                TreeNodeSelectEventArgs e = new TreeNodeSelectEventArgs();

                if (SelectedNode.Tag != null)
                {
                    e.SelectedOblect = SelectedNode.Tag;
                }

                if (OnTreeNodeSelectEvent != null) this.OnTreeNodeSelectEvent(this, e);

            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref TVHITTESTINFO lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct TVHITTESTINFO
        {
            public Point pt;
            public int flags;
            public IntPtr hItem;
        }

        private const int TVM_HITTEST = 0x1111;


        protected void TreeControl_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                // Show menu only if the right mouse button is clicked.
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);
                // Get the node that the user has clicked.
                TreeNode node = GetNodeAt(p);
                if (node != null)
                {

                    TVHITTESTINFO ht = new TVHITTESTINFO();
                    ht.pt = e.Location;
                    SendMessage(this.Handle, TVM_HITTEST, IntPtr.Zero, ref ht);
               
                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = SelectedNode;
        
                    SelectedNode = node;
                   
              
                    if (e.Button == MouseButtons.Right)
                    {
                        IEnumerable<ToolStripItem> items = MakeContextMenu(node);
                        if (items != null)
                        {
                            treeContextMenu.Items.Clear();
                            foreach (var i in items)
                            {
                                treeContextMenu.Items.Add(i);
                            }
                            treeContextMenu.Show(this, p);
                        }
                    }
                    else
                    {
                        if (ht.flags == 16)//todo
                        {
                            //Clicked on a ExpandOrCollapse button 
                            //- срабатывает не всегда
                            //todo итем всё равно выделяется 
                        }
                        else
                            OnItemSelected();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private IEnumerable<ToolStripItem> MakeContextMenu(TreeNode node)
        {
            List<ToolStripItem> menuItemList = new List<ToolStripItem>();
            if (OnTreeContextMenuEvent != null) OnTreeContextMenuEvent(this, new TreeContextMenuSelectEventArgs()
                { MenuItemList = menuItemList, SelectedNode = node });
    
            return menuItemList;
        }

        //for drag
        public TreeNode GetHoveringNode(int screen_x, int screen_y)
        {
            Point pt = PointToClient(new Point(screen_x, screen_y));
            TreeViewHitTestInfo hitInfo = HitTest(pt);
            return hitInfo.Node;
        }


        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            try
            {
                if (e.Label != null)
                {
                    if (e.Label.Length > 0)
                    {
                        if (e.Node.Tag != null)
                        {
                            AfterEditTreeNodeLabel(e.Node.Tag, e.Label);
                        }
                        e.Node.EndEdit(false);
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show(FrwCRUDRes.Record_Can_Not_Be_Empty,
                           FrwCRUDRes.Wrong_Operation);
                        e.Node.BeginEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


    }
}
