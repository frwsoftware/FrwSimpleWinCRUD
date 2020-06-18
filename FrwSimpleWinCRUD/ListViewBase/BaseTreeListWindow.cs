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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FrwSoftware
{
    public class TreeObjectWrap
    {
        public object Tag { get; set; }
        public RefEntityInfo Rel { get; set; }
    }
    public class BaseBranchTreeFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public partial class BaseTreeListWindow : BaseListWindow
    {
        protected BaseTreeControl treeControl;

        public BaseTreeListWindow()
        {
            InitializeComponent();

            // treeControl
            // 
            this.treeControl = new BaseTreeControl();
            this.treeControl.AfterEditTreeNodeLabel = null;
          
            this.treeControl.CanExpandGetter = null;
            this.treeControl.ChangeNodeImageOnExpand = false;
            this.treeControl.ChildrenGetter = null;
            this.treeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeControl.Location = new System.Drawing.Point(0, 0);
            this.treeControl.Name = "treeControl";
            this.treeControl.ParentViewProcessor = null;
            //this.treeControl.RootNode = null;
            this.treeControl.Size = new System.Drawing.Size(150, 125);
            this.treeControl.TabIndex = 0;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.treeControl);

            treeControl.ParentViewProcessor = this;
            treeControl.ShowNodeToolTips = true;

            treeControl.ImageList = new ImageList();
            treeControl.ImageList.Images.Add(BaseTreeControl.TREE_FOLDER_CLOSED, Properties.Resources.tree_closed);
            treeControl.ImageList.Images.Add(BaseTreeControl.TREE_FOLDER_OPENED, Properties.Resources.tree_open);
            treeControl.ImageList.Images.Add(BaseTreeControl.TREE_FOLDER_OPENED_SELECTED, Properties.Resources.tree_open_arrow);
            treeControl.ImageList.Images.Add(BaseTreeControl.TREE_FOLDER_CLOSED_SELECTED, Properties.Resources.tree_closed_arrow);

            treeControl.OnTreeContextMenuEvent += TreeControl_OnTreeContextMenuEvent;
            treeControl.DragDrop += TreeControl_DragDrop;
            treeControl.DragOver += TreeControl_DragOver;
            treeControl.DragLeave += TreeControl_DragLeave;
            treeControl.DragEnter += TreeControl_DragEnter;
            treeControl.ItemDrag += TreeControl_ItemDrag;

            treeControl.ComplateNode += delegate (TreeNode node, object x)
            {
                ComplateNodeFromObject(node, x);
            };

        }

        override public void CreateView()
        {
            base.CreateView();
            LoadConfig();
            treeControl.BeginUpdate();
            this.treeControl.InitCreateRootNodes();
            if (savedTreeState != null)
            {
                //We can not use the same algorithm as in the Reload method. 
                //Because child records are actually loaded asynchronously.
                //We need to call a recursive load directly.
                foreach (TreeNode n in treeControl.Nodes)
                {
                    if (savedTreeState.NodeStates.Contains(n.FullPath))
                    {
                        n.Expand();
                        treeControl.LoadBrahch(n, 0, 1, savedTreeState.NodeStates);
                    }
                }
            }
            treeControl.EndUpdate();
        }

        virtual protected object AddChildTreeNode(object selectedObject)
        {
            return AddObjectLocal(selectedObject, SourceObjectType);

        }
        private void TreeControl_OnTreeContextMenuEvent(object sender, TreeContextMenuSelectEventArgs e)
        {
            try
            {
                List<ToolStripItem> menuItemList = new List<ToolStripItem>();
                MakeContextMenu(menuItemList, e.SelectedNode, (e.SelectedNode.Tag is TreeObjectWrap) ? (e.SelectedNode.Tag as TreeObjectWrap).Tag : e.SelectedNode.Tag, null);//todo aspectName
                e.MenuItemList.AddRange(menuItemList);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
}

        virtual protected void ComplateNodeFromObject(TreeNode node, object wo)
        {
            object o;
            RefEntityInfo rel = null; 
            if (wo is TreeObjectWrap)
            {
                o = (wo as TreeObjectWrap).Tag;
                rel = (wo as TreeObjectWrap).Rel;
            }
            else o = wo;

            node.Tag = wo;
            string name;
            if (o is string) name = o as string;
            if (o is BaseBranchTreeFolder)
            {
                name = (o as BaseBranchTreeFolder).Name;
            }
            else name = ModelHelper.GetShortNameForObjectAdv(o);
            string shortName = (name.Length > 70) ? (name.Substring(0, 70) + "...") : name;
            node.Name = shortName;
            node.Text = shortName;
            if (rel != null)
                if (rel.PropertyInForeign != null)
                    node.ToolTipText = name + " (" + rel.GetRelDescription() + ")";
                else if (rel.PropertyInSource != null)
                    node.ToolTipText = name + " (" + rel.GetRelDescription() + ")";
            else 
                node.ToolTipText = name;
        }

        override protected void AddObject(object selectedListItem, object selectedObject, Type sourceObjectType, IDictionary<string, object> extraParams)
        {
            object newObject = AddObjectLocal(selectedObject, sourceObjectType, extraParams);
            if (newObject != null)
            {
                TreeNode selectedNode = selectedListItem as TreeNode;
                TreeNode node = AddNodeTo(selectedNode, newObject);
                if (selectedNode.IsExpanded == false) selectedNode.Expand();
            }
        }
        override protected void CloneObject(object selectedListItem, object selectedObject,  IDictionary<string, object> extraParams)
        {
            object newObject = CloneObjectLocal(selectedObject,  extraParams);
            if (newObject != null)
            {
                TreeNode selectedNode = selectedListItem as TreeNode;
                TreeNode node = AddNodeTo(selectedNode, newObject);
                if (selectedNode.IsExpanded == false) selectedNode.Expand();
            }
        }

        protected TreeNode AddNodeTo(TreeNode parent, object tagObject)
        {
            TreeNode node = new TreeNode();
            ComplateNodeFromObject(node, tagObject);
            treeControl.PostCreateNode(node);

            if (parent != null)
            {
                if (parent.Nodes.Count > 0 &&
                parent.Nodes[0].Text == BaseTreeControl.PSEVDO_NODE_TEXT)
                {
                    parent.Nodes[0].Remove();
                }
                parent.Nodes.Add(node);
            }
            else
            {
                treeControl.Nodes.Add(node);
            }

            return node;
        }

        public TreeNode GetFirstNode()
        {
            if (treeControl.Nodes.Count > 0) return treeControl.Nodes[0];
            else return null;
        }

        override protected void DeleteObject(object selectedListItem, object[] selectedObjects)
        {
            DialogResult res = MessageBox.Show(null, FrwCRUDRes.List_Delete_Record_Confirmation,
                          FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            TreeNode selectedNode = selectedListItem as TreeNode;
            if (res == DialogResult.Yes)
            {
                if (!(selectedNode.Nodes == null || selectedNode.Nodes.Count == 0))
                {
                    MessageBox.Show(FrwCRUDRes.List_Can_Not_Delete_Children);
                }
                else
                {
                    if (DeleteObjectLocal(selectedObjects))
                    {
                        selectedNode.Remove();
                    }
                }

            }
        }
        override protected void DeleteAllObjects()
        {
            DialogResult res = MessageBox.Show(null, FrwCRUDRes.List_Delete_All_records_Confirmation,
                               FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {

                if (DeleteAllObjectsLocal())
                {
                    this.treeControl.Nodes.Clear(); ;//todo test
                }
            }
        }
        override protected void ReloadList()
        {
            if (savedTreeState != null)
            {
                savedTreeState.NodeStates = treeControl.Nodes.GetExpansionState();
            }
            treeControl.FullRefresh();
            if (savedTreeState != null)
            {
                treeControl.BeginUpdate();
                treeControl.Nodes.SetExpansionState(savedTreeState.NodeStates);
                treeControl.EndUpdate();
            }
        }
        /*
        virtual public void MoveNode(TreeNode destNode, TreeNode sourceNode, bool cut)
        {
            CopySelectedObjectToClipBoard(sourceNode, sourceNode.Tag, cut);
            PasteSelectedObjectFromAppClipBoard(destNode, destNode.Tag);
        }
        */
        override protected void CopySelectedObjectToClipBoard(IList sourceObjects, bool cut)
        {
            if (sourceObjects != null && sourceObjects.Count > 0)
            {
                //windows
                object x = ((sourceObjects[0] as TreeNode).Tag is TreeObjectWrap) ? ((sourceObjects[0] as TreeNode).Tag as TreeObjectWrap).Tag : (sourceObjects[0] as TreeNode).Tag;
                string str = ModelHelper.ModelPropertyList(x, "\n", null, null);
                Clipboard.SetText(str);
                //app
                FrwTreeDataObject dataObject = new FrwTreeDataObject();
                dataObject.SelectedTreeNode = sourceObjects[0] as TreeNode;
                CopyAdditionalObjectsToAppClipboard(sourceObjects, cut, dataObject);
                AppManager.Instance.FrwClipboard.DataObject = dataObject;
                AppManager.Instance.FrwClipboard.IsCutOperation = cut;

            }
        }
    
        //DragDrop: The last event to handle is the DragDrop event of the destination TreeView 
        //control.This event occurs when the TreeNode object that is dragged has been dropped 
        //on the destination TreeView control.To handle this event, retrieve the TreeNode object,
        //and add the object to the destination TreeView control.
        private void TreeControl_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string formats = string.Join("\n", e.Data.GetFormats(false));
                bool cut;
                if (e.Effect == DragDropEffects.None) return;
                else if (e.Effect == DragDropEffects.Move)
                {
                    cut = true;
                }
                else if (e.Effect == DragDropEffects.Copy)
                {
                    cut = false;
                }
                else throw new Exception("");

                TreeNode destNode = treeControl.GetHoveringNode(e.X, e.Y);
                bool res = PasteSelectedObjectsFromIDataObject(destNode, e.Data, cut);
                if (res) MessageBox.Show(FrwCRUDRes.Drag_Record_Successfully + (cut ? FrwCRUDRes.Drage_Moved : FrwCRUDRes.Drag_Copied));

            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }


        //The DragOver event is raised when the mouse cursor moves within the bounds of the control during a drag-and-drop operation.
        //If the mouse moves but stays within the same control, the DragOver event is raised.
        protected void TreeControl_DragOver(object sender, DragEventArgs e)
        {
            //Console.WriteLine("DragOver");
            try
            {
                TreeNode hoveringNode = treeControl.GetHoveringNode(e.X, e.Y);
                TreeNode draggingNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;
     
                if (hoveringNode != null && hoveringNode != draggingNode)
                {
                    //по умолчанию переносим 
                    if (((e.KeyState & 4) == 4 || (e.KeyState & 8) == 8) &&
                        (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {
                        //4 - SHIFT KeyState.
                        //8 - CTRL KeyState.
                        e.Effect = DragDropEffects.Copy;
                    }
                    else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    hoveringNode.TreeView.SelectedNode = hoveringNode;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        //If the mouse enters another control, the DragEnter for that control is raised
        protected void TreeControl_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        //If the user moves out of a window, the DragLeave event is raised.
        protected void TreeControl_DragLeave(object sender, System.EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }

        }
        //ItemDrag: This event is raised from the source TreeView control as soon as the user 
        //starts to drag the tree node. When this occurs, call the DoDragDrop method to 
        //initiate the drag-and-drop procedure.
        private void TreeControl_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                DragDropEffects dragDropEffects = DoDragDrop( e.Item, DragDropEffects.All);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private  TreeNode FindNodeByTagObject(object o, TreeNode parent)
        {
            if (parent == null) {
                foreach(TreeNode n in treeControl.Nodes)
                {
                    var ret = FindNodeByTagObject(o, n);
                    if (ret != null) return ret;
                }
            }
            else
            {
                if ((parent.Tag is TreeObjectWrap && o.Equals((parent.Tag as TreeObjectWrap).Tag))
                    ||
                    (!(parent.Tag is TreeObjectWrap) && o.Equals(parent.Tag))
                    )
                {
                    return parent;
                }
                foreach (TreeNode n in parent.Nodes)
                {
                    var ret = FindNodeByTagObject(o, n);
                    if (ret != null) return ret;
                }
            }
            return null;
        }

        
        override public void RefreshObject(object o)
        {
            TreeNode n = FindNodeByTagObject(o, null);
            if (n != null)
            {
                bool expanded = n.IsExpanded;
                ComplateNodeFromObject(n, o);
                treeControl.PostCreateNode(n);
                if (expanded)
                {
                    //n.Expand();
                }
            }
        }

        /*
        override protected object GetSelectedObject()
        {
            return treeControl.SelectedNode.Tag;
        }
        */
        override protected object GetSelectedListItem()
        {
            return treeControl.SelectedNode;
        }
        override protected IList GetSelectedForCopyPaste()
        {
            return new List<object>() { GetSelectedListItem() };
        }
        override public IList GetSelectedObjects()
        {

            if (treeControl.SelectedNode != null && treeControl.SelectedNode.Tag != null)
                return new List<object>() {(treeControl.SelectedNode.Tag is TreeObjectWrap)? (treeControl.SelectedNode.Tag as TreeObjectWrap).Tag : treeControl.SelectedNode.Tag };
            else return null;
        }

        override protected void MakeContextMenuAddBlock(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject,JRights rights)
        {
            TreeNode parentNode = selectedListItem as TreeNode;

            ToolStripMenuItem menuItem = null;

            menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.List_Add_TopLevel_Record;
            menuItem.Enabled = rights.CanAdd;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    AddObject(selectedListItem, selectedObject, SourceObjectType, null);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemList.Add(menuItem);

            if (parentNode != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.List_Add_Child_Record;
                menuItem.Enabled = rights.CanAddChild;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        object x = (parentNode.Tag is TreeObjectWrap) ? (parentNode.Tag as TreeObjectWrap).Tag : parentNode.Tag;
                        var pars = new Dictionary<string, object> { { "Parent", x }, { "ParentNode", parentNode } };
                        AddObject(selectedListItem, selectedObject, SourceObjectType, pars);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);
            }
        }
        #region save restore config
        private string configFileStr = null;
        protected TreeListState savedTreeState = null;
        private void LoadConfig()
        {
            try
            {
                string filename = null;
                if (DlgMode)
                {
                    filename = GetStateConfigFileName(true);
                    if (!File.Exists(filename)) filename = GetStateConfigFileName(false);
                }
                else
                {
                    filename = GetStateConfigFileName(false);
                }
                if (File.Exists(filename))
                {
                    configFileStr = File.ReadAllText(filename, Encoding.UTF8);
                    savedTreeState = JsonSerializeHelper.LoadFromString<TreeListState>(configFileStr);
                }
                else savedTreeState = new TreeListState();
                LoadUserSettings(savedTreeState.UserSettings);
            }
            catch (Exception ex)
            {
                Log.LogError("Can not read config for " + GetType().ToString(), ex);
            }

        }
        override public void SaveConfig()
        {
            try
            {
                string filename = GetStateConfigFileName(DlgMode);
                SaveUserSettings(savedTreeState.UserSettings);
                savedTreeState.NodeStates = treeControl.Nodes.GetExpansionState();
                string newConfigStr = JsonSerializeHelper.SaveToString(savedTreeState);
                if (newConfigStr.Equals(configFileStr) == false)
                {
                    File.WriteAllText(filename, newConfigStr, Encoding.UTF8);
                    configFileStr = newConfigStr;
                }
                
            }
            catch (Exception ex)
            {
                Log.LogError("Can not write config for " + GetType().ToString(), ex);
            }

        }
        #endregion
        override protected void EnsureAddedObjectVisible(ChildObjectUpdateEventArgs e)
        {
            //todo reload only spec TreeNode
            ReloadList();
        }
    }

    public class RelationVisibility
    {
        public string Name { get; set; }
    }
    public class EntityRelationVisibility
    {
        public string EntityTypeFullName { get; set; }//do not use Type? use string
        public List<RelationVisibility> InvisileRelations { get; set; } = new List<RelationVisibility>();
        public bool IsRelationVisible(string name)
        {
            foreach(var f in InvisileRelations)
            {
                if (f.Name.Equals(name)) return false;
            }
            return true;
        }
        public void InvisibleRelation(string name)
        {
            RelationVisibility fv = null;
            foreach (var f in InvisileRelations)
            {
                if (f.Name.Equals(name)) fv = f;
                break;
            }
            if (fv == null)
            {
                fv = new RelationVisibility() { Name = name};
                InvisileRelations.Add(fv);
            }
        }
    }

    public class TreeListState
    {
        private IDictionary<string, object> userSettings = new Dictionary<string, object>();
        public IDictionary<string, object> UserSettings { get { return userSettings; } set { userSettings = value; } }

        private  IList<string> nodeStates = new List<string>();
        public IList<string> NodeStates { get { return nodeStates; } set { nodeStates = value; } }

        private IList<EntityRelationVisibility> relationVisibilities = new List<EntityRelationVisibility>();
        public IList<EntityRelationVisibility> RelationVisibilities { get { return relationVisibilities; } set { relationVisibilities = value; } }

        public bool IsRelationVisible(Type entityType, string name)
        {
            foreach (var f in RelationVisibilities)
            {
                if (entityType.FullName.Equals(f.EntityTypeFullName))
                {
                    return f.IsRelationVisible(name);
                }
            }
            return true;
        }
        public void InvisibleField(Type entityType, string name)
        {
            EntityRelationVisibility fv = null;
            foreach (var f in RelationVisibilities)
            {
                if (entityType.FullName.Equals(f.EntityTypeFullName))
                {
                    fv = f;
                    break;
                }
            }
            if (fv == null) {
                fv = new EntityRelationVisibility() { EntityTypeFullName = entityType.FullName };
                RelationVisibilities.Add(fv);
            }
            fv.InvisibleRelation(name);
        }
    }

    public static class TreeViewExtensions
    {
        public static IList<string> GetExpansionState(this TreeNodeCollection nodes)
        {
            return nodes.Descendants()
                        .Where(n => n.IsExpanded)
                        .Select(n => n.FullPath)
                        .ToList();
        }
        //We can use this algorithm onle in ReloadList method.
        public static void SetExpansionState(this TreeNodeCollection nodes, IList<string> savedExpansionState)
        {
            foreach (var node in nodes.Descendants()
                                      .Where(n => savedExpansionState.Contains(n.FullPath)))
            {
                node.Expand();
            }
        }

        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }
}
