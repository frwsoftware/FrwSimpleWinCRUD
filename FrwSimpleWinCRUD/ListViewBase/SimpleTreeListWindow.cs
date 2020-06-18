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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace FrwSoftware
{
    public class RootGroupTreeFolder
    {
        public Type EntityType = null;
    }
    public class BranchGroupTreeFolder
    {
        public object ParentObject = null;
        public RefEntityInfo RefEntityInfo = null;
    }
    public partial class SimpleTreeListWindow : BaseTreeListWindow 
    {
        protected List<Type> rootEntites = new List<Type>();
        //protected bool showGroupsFolder = true;
        protected List<Type> notShowGroupsFolderTypes = new List<Type>();

        public SimpleTreeListWindow()
        {
            InitializeComponent();
            treeControl.LabelEdit = true;
            //treeControl.ChangeNodeImageOnExpand = true;
            treeControl.AllowDrop = true;
            treeControl.ShowNodeToolTips = true;

            treeControl.OnTreeNodeSelectEvent += TreeControl_OnTreeNodeSelectEvent;

            this.treeControl.CanExpandGetter += delegate (TreeNode currentNode)
            {
                if (currentNode == null) return true;
                //currentNode may be not object node (may by branch node), so find real object node to view object 
                TreeNode currentObjectNode = (currentNode.Tag is BranchGroupTreeFolder) ? currentNode.Parent : currentNode;
                TreeNode parentObjectNode = (currentObjectNode != null && currentObjectNode.Parent != null)
                    ? ((currentObjectNode.Parent.Tag is BranchGroupTreeFolder) ? currentObjectNode.Parent.Parent : currentObjectNode.Parent) : null;

                object curentObject = (currentNode.Tag is TreeObjectWrap) ? (currentNode.Tag as TreeObjectWrap).Tag : currentNode.Tag;
                object parentObject = (parentObjectNode != null) ? ((parentObjectNode.Tag is TreeObjectWrap)? (parentObjectNode.Tag as TreeObjectWrap).Tag : parentObjectNode.Tag) : null;

                RefEntityInfo currentNodeRel = (currentObjectNode != null && currentObjectNode.Tag is TreeObjectWrap) ? (currentObjectNode.Tag as TreeObjectWrap).Rel : null;
                RefEntityInfo parentNodeRel = (parentObjectNode != null && parentObjectNode.Tag is TreeObjectWrap) ? (parentObjectNode.Tag as TreeObjectWrap).Rel : null;

                if (curentObject == null) return false;
                if (curentObject is string) return true;//folder 
                Type type = curentObject.GetType();
                if (curentObject is RootGroupTreeFolder)
                {
                    Type etype = (curentObject as RootGroupTreeFolder).EntityType;
                    if (ModelHelper.IsSingleHierEntity(etype))
                    {
                        return (Dm.Instance.FindRootList(etype).Count > 0);
                    }
                    else return (Dm.Instance.FindAll(etype).Count > 0);
                }
                else if (curentObject is BranchGroupTreeFolder)
                {
                    BranchGroupTreeFolder bf = (curentObject as BranchGroupTreeFolder);
                    if (bf.RefEntityInfo.PropertyInForeign != null)
                    {
                        if (AttrHelper.GetAttribute<JManyToMany>(bf.RefEntityInfo.PropertyInForeign) != null)
                            return Dm.Instance.ResolveManyToManyRelation(bf.ParentObject, bf.RefEntityInfo.ForeignEntity).Count > 0;
                        else if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.PropertyInForeign) != null)
                            return Dm.Instance.ResolveOneToManyRelation(bf.ParentObject, bf.RefEntityInfo.ForeignEntity,
                                 bf.RefEntityInfo.PropertyInForeign.Name).Count > 0;
                        else if (AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.PropertyInForeign) != null)
                            return (Dm.Instance.ResolveOneToOneRelationReverse(bf.ParentObject, bf.RefEntityInfo.ForeignEntity,
                                 bf.RefEntityInfo.PropertyInForeign.Name) != null);
                    }
                    else if (bf.RefEntityInfo.PropertyInSource != null)
                    {
                        if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.PropertyInSource) != null || AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.PropertyInSource) != null)
                        {
                            object o = AttrHelper.GetPropertyValue(bf.ParentObject, bf.RefEntityInfo.PropertyInSource);
                            return (o != null); 
                        }
                    }

                }
                else
                {
                    JEntity entityAttr = curentObject.GetType().GetCustomAttribute<JEntity>();
                    if (entityAttr != null)
                    {
                        //if (showGroupsFolder)
                        //{
                         //   List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(parentObject, false);
                         //   return rels.Count(s => (s.RefFromProperty != null || s.RefToProperty != null)) > 0;
                        //}
                        //else
                        //{
                            //find all ManyToOne relations rels to this entity type 
                            List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(curentObject);
                            foreach (var rt in rels)
                            {
                                HashSet<object> refs = rt.Records;
                                if (refs.Count > 0)
                                {
                                    return true;
                                }
                            }
                        //}
                    }
                }
                return false;
            };
            treeControl.ChildrenGetter += delegate (TreeNode currentNode)
            {
                IList lo = new List<object>();
                if (currentNode == null)
                {
                    //root
                    foreach (var e in rootEntites)
                    {
                        RootGroupTreeFolder ef = new RootGroupTreeFolder();
                        ef.EntityType = e;
                        lo.Add(ef);
                    }
                }
                else
                {

                    TreeNode currentObjectNode = (currentNode.Tag is BranchGroupTreeFolder) ? currentNode.Parent : currentNode;
                    TreeNode parentObjectNode = (currentObjectNode != null && currentObjectNode.Parent != null) 
                        ? ((currentObjectNode.Parent.Tag is BranchGroupTreeFolder) ? currentObjectNode.Parent.Parent : currentObjectNode.Parent) : null;

                    object currentObject = (currentNode.Tag is TreeObjectWrap) ? (currentNode.Tag as TreeObjectWrap).Tag : currentNode.Tag;
                    object parentObject = (parentObjectNode != null) ? ((parentObjectNode.Tag is TreeObjectWrap) ? (parentObjectNode.Tag as TreeObjectWrap).Tag : parentObjectNode.Tag) : null;

                    RefEntityInfo currentNodeRel = (currentObjectNode != null && currentObjectNode.Tag is TreeObjectWrap) ?  (currentObjectNode.Tag as TreeObjectWrap).Rel : null;
                    RefEntityInfo parentNodeRel = (parentObjectNode != null && parentObjectNode.Tag is TreeObjectWrap) ? (parentObjectNode.Tag as TreeObjectWrap).Rel : null;

                    if (currentObject == null) return new List<object>();
                    if (currentObject is RootGroupTreeFolder)
                    {
                        Type type = (currentObject as RootGroupTreeFolder).EntityType;
                        if (ModelHelper.IsSingleHierEntity(type))
                        {
                            return WrapList(Dm.Instance.FindRootList(type), null);
                        }
                        else return WrapList(Dm.Instance.FindAll(type), null);
                    }
                    else if (currentObject is BranchGroupTreeFolder)
                    {
                        BranchGroupTreeFolder bf = (currentObject as BranchGroupTreeFolder);

                        if (savedTreeState.IsRelationVisible(bf.RefEntityInfo.SourceEntity, bf.RefEntityInfo.Name))
                        {

                            if (bf.RefEntityInfo.PropertyInForeign != null)
                            {
                                if (AttrHelper.GetAttribute<JManyToMany>(bf.RefEntityInfo.PropertyInForeign) != null)
                                    return WrapList(Dm.Instance.ResolveManyToManyRelation(bf.ParentObject, bf.RefEntityInfo.ForeignEntity), bf.RefEntityInfo);
                                else if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.PropertyInForeign) != null)
                                    return WrapList(Dm.Instance.ResolveOneToManyRelation(bf.ParentObject, bf.RefEntityInfo.ForeignEntity,
                                         bf.RefEntityInfo.PropertyInForeign.Name), bf.RefEntityInfo);
                                else if (AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.PropertyInForeign) != null)
                                    lo.Add(new TreeObjectWrap()
                                    {
                                        Tag = Dm.Instance.ResolveOneToOneRelationReverse(bf.ParentObject, bf.RefEntityInfo.ForeignEntity,
                                         bf.RefEntityInfo.PropertyInForeign.Name),
                                        Rel = bf.RefEntityInfo
                                    });
                            }
                            else if (bf.RefEntityInfo.PropertyInSource != null)
                            {
                                if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.PropertyInSource) != null || AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.PropertyInSource) != null)
                                {
                                    object o = AttrHelper.GetPropertyValue(bf.ParentObject, bf.RefEntityInfo.PropertyInSource);

                                    if (o != null)
                                    {
                                        if (parentObject != null && parentObject.Equals(o)
                                            && currentNodeRel != null && currentNodeRel.PropertyInForeign != null && bf.RefEntityInfo.PropertyInSource != null
                                            && currentNodeRel.PropertyInForeign == bf.RefEntityInfo.PropertyInSource)
                                        {
                                        }
                                        else
                                        {
                                            lo.Add(new TreeObjectWrap() { Tag = o, Rel = bf.RefEntityInfo });
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        bool isSingleHierEntity = ModelHelper.IsSingleHierEntity(currentObject.GetType());
                        JEntity entityAttr = currentObject.GetType().GetCustomAttribute<JEntity>();
                        if (entityAttr != null)
                        {
                            List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(currentObject);
                            foreach (var rt in rels)
                            {
                                if (savedTreeState.IsRelationVisible(rt.SourceEntity, rt.Name))
                                {

                                    bool isSelfRelation = rt.IsSelfRelation();
                                    if ((notShowGroupsFolderTypes.Contains(rt.ForeignEntity) == false && rt.PropertyInForeign != null) && !(isSelfRelation && isSingleHierEntity))
                                    {
                                        if (rt.Records.Count > 0)
                                        {
                                            BranchGroupTreeFolder bf = new BranchGroupTreeFolder();
                                            bf.ParentObject = currentObject;
                                            bf.RefEntityInfo = rt;
                                            lo.Add(bf);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var l in rt.Records)
                                        {
                                            if (!(parentObject != null && parentObject.Equals(l)
                                                && currentNodeRel != null && currentNodeRel.PropertyInForeign != null && rt.PropertyInSource != null
                                                && currentNodeRel.PropertyInForeign == rt.PropertyInSource))
                                            {
                                                lo.Add(new TreeObjectWrap() { Tag = l, Rel = rt });
                                            }
                                        }
                                    }
                                }
                            }//foreach (var rt in rels)
                        }
                        return lo;
                    }
                }
                return lo;
            };
            treeControl.AfterEditTreeNodeLabel += delegate (object model, string labelText)
            {
                if (model == null) return false;
                Type t = model.GetType();
                JEntity entityAttr = t.GetCustomAttribute<JEntity>();
                if (entityAttr != null)
                {
                    PropertyInfo pName = AttrHelper.GetProperty<JNameProperty>(t);
                    if (pName != null)
                    {
                        object oldValue = pName.GetValue(model);
                        try
                        {
                            pName.SetValue(model, labelText);//force exception if no set method 
                            Dm.Instance.SaveObject(model);
                            return true;
                        }
                        catch (JValidationException ex)
                        {
                            AppManager.ShowValidationErrorMessage(ex.ValidationResult);
                            pName.SetValue(model, oldValue);
                        }
                    }
                }
                return false;
            };
        }

        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);

            TreeNode currentNode = selectedListItem as TreeNode;

            TreeNode currentObjectNode = (currentNode.Tag is BranchGroupTreeFolder) ? currentNode.Parent : currentNode;
            //TreeNode parentObjectNode = (currentObjectNode != null && currentObjectNode.Parent != null)
              //  ? ((currentObjectNode.Parent.Tag is BranchGroupTreeFolder) ? currentObjectNode.Parent.Parent : currentObjectNode.Parent) : null;

            object currentObject = (currentNode.Tag is TreeObjectWrap) ? (currentNode.Tag as TreeObjectWrap).Tag : currentNode.Tag;
            //object parentObject = (parentObjectNode != null) ? ((parentObjectNode.Tag is TreeObjectWrap) ? (parentObjectNode.Tag as TreeObjectWrap).Tag : parentObjectNode.Tag) : null;

            RefEntityInfo currentNodeRel = (currentObjectNode != null && currentObjectNode.Tag is TreeObjectWrap) ? (currentObjectNode.Tag as TreeObjectWrap).Rel : null;
            //RefEntityInfo parentNodeRel = (parentObjectNode != null && parentObjectNode.Tag is TreeObjectWrap) ? (parentObjectNode.Tag as TreeObjectWrap).Rel : null;

            if (currentNodeRel != null)
            {
                menuItemList.Add(new ToolStripSeparator());
                ToolStripMenuItem menuItem = null;
                menuItem = new ToolStripMenuItem();
                menuItem.Text = FrwCRUDRes.Do_not_show_this_dependency;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        savedTreeState.InvisibleField(currentNodeRel.SourceEntity, currentNodeRel.Name);
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemList.Add(menuItem);

            }

        }
        private IList WrapList(IList list, RefEntityInfo rt)
        {
            IList lo = new List<object>();
            foreach (var l in list)
            {
                lo.Add(new TreeObjectWrap() { Tag = l, Rel = rt });
            }
            return lo;
        }
        override protected void ComplateNodeFromObject(TreeNode node, object wo)
        {
            base.ComplateNodeFromObject(node, wo);

            object wx = node.Tag;
            if (wx == null) return;

            object x;
            if (wx is TreeObjectWrap) x = (wx as TreeObjectWrap).Tag;
            else x = wx;

            Type type = x.GetType();
            if (x is RootGroupTreeFolder)
            {
                Type etype = (x as RootGroupTreeFolder).EntityType;
                string name = ModelHelper.GetEntityJDescriptionOrName(etype);
                node.Name = name;
                node.Text = name;
                node.ToolTipText = name;
            }
            else if (x is BranchGroupTreeFolder)
            {
                Type etype =  (x as BranchGroupTreeFolder).RefEntityInfo.ForeignEntity;
                string name = (x as BranchGroupTreeFolder).RefEntityInfo.GetRelDescription();
                node.Name = name;
                node.Text = name;
                node.ToolTipText = name;
            }
            else
            {
                JEntity entityAttr = type.GetCustomAttribute<JEntity>();
                if (entityAttr != null && entityAttr.ImageName != null)
                {
                    if (treeControl.ImageList == null) treeControl.ImageList = new ImageList();
                    if (!treeControl.ImageList.Images.ContainsKey(entityAttr.ImageName))
                    {
                        Image smallImage = null;
                        if (smallImage == null && entityAttr.Resource != null)
                            smallImage = TypeHelper.LookupImageResource(entityAttr.Resource, entityAttr.ImageName);
                        if (smallImage == null)
                            smallImage = (Image)Properties.Resources.ResourceManager.GetObject(entityAttr.ImageName);
                        //if not found in current assembly do advanced search 
                        if (smallImage == null) smallImage = TypeHelper.FindImageInAllDiskStorages(entityAttr.ImageName);
                        if (smallImage != null)
                        {
                            treeControl.ImageList.Images.Add(entityAttr.ImageName, smallImage);
                        }
                    }
                    node.ImageKey = entityAttr.ImageName;
                    node.SelectedImageKey = entityAttr.ImageName;
                }
              
            }
        }

        protected void SetImageForNode(TreeNode node, Image image, string imageName)
        {
            if (treeControl.ImageList == null) treeControl.ImageList = new ImageList();
            if (!treeControl.ImageList.Images.ContainsKey(imageName))
            {
                treeControl.ImageList.Images.Add(imageName, image);
            }
            node.ImageKey = imageName;
            node.SelectedImageKey = imageName;
        }


        /*
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
            menuItemList.Add(new ToolStripSeparator());

            if (selectedObject != null)
            {
                if (selectedObject is EntityTreeFolder)
                {
                    //no to add 
                }
                else
                {
                    //JCompDeviceListWindow.MakeContextMenuBlock(menuItemList, item, this.ContentContainer);
                }
            }
        }
        */


   
        override protected void MakeContextMenuAddBlock(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, JRights rights)
        {

            TreeNode selectedNode = selectedListItem as TreeNode;

            if (selectedObject != null)
            {
                if (selectedObject is RootGroupTreeFolder)
                {
                    Type type = (selectedObject as RootGroupTreeFolder).EntityType;
                    menuItemList.Add(CreateAddNode(selectedListItem, selectedObject, rights, type, null));
                }
                else if (selectedObject is BranchGroupTreeFolder)
                {
                    BranchGroupTreeFolder bf = (selectedObject as BranchGroupTreeFolder);
                    menuItemList.Add(CreateAddNode(selectedListItem, bf.ParentObject, rights, bf.RefEntityInfo.ForeignEntity, bf.RefEntityInfo));
                }
                else
                {
                    List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(selectedObject, false);
                    foreach (var rt in rels)
                    {
                        if (rt.PropertyInForeign != null)
                            menuItemList.Add(CreateAddNode(selectedListItem, selectedObject, rights, rt.ForeignEntity, rt));
                    }
                }
            }
        }
        private void TreeControl_OnTreeNodeSelectEvent(object sender, TreeNodeSelectEventArgs e)
        {
            try
            {
                OnSelectObject(e.SelectedOblect);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

    }
}
