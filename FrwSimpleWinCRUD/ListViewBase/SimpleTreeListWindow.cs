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
        protected bool showGroupsFolder = true;
        public SimpleTreeListWindow()
        {
            InitializeComponent();
            treeControl.LabelEdit = true;
            //treeControl.ChangeNodeImageOnExpand = true;
            treeControl.AllowDrop = true;
            treeControl.ShowNodeToolTips = true;

            treeControl.OnTreeNodeSelectEvent += TreeControl_OnTreeNodeSelectEvent;

            this.treeControl.CanExpandGetter += delegate (TreeNode parentNode)
            {
                if (parentNode == null) return true;
                object x = parentNode.Tag;
                if (x == null) return false;
                if (x is string) return true;//folder 
                Type type = x.GetType();
                if (x is RootGroupTreeFolder)
                {
                    Type etype = (x as RootGroupTreeFolder).EntityType;
                    if (ModelHelper.IsSingleHierEntity(etype))
                    {
                        return (Dm.Instance.FindRootList(etype).Count > 0);
                    }
                    else return (Dm.Instance.FindAll(etype).Count > 0);
                }
                else if (x is BranchGroupTreeFolder)
                {
                    BranchGroupTreeFolder bf = (x as BranchGroupTreeFolder);
                    if (AttrHelper.GetAttribute<JManyToMany>(bf.RefEntityInfo.foreinProperty) != null)
                        return Dm.Instance.ResolveManyToManyRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity).Count > 0;
                    else if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.foreinProperty) != null)
                        return Dm.Instance.ResolveOneToManyRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity,
                             bf.RefEntityInfo.foreinProperty.Name).Count > 0;
                    else if (AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.foreinProperty) != null)
                        return (Dm.Instance.ResolveOneToOneRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity,
                             bf.RefEntityInfo.foreinProperty.Name) != null);

                }
                else
                {
                    JEntity entityAttr = x.GetType().GetCustomAttribute<JEntity>();
                    if (entityAttr != null)
                    {
                        if (showGroupsFolder)
                        {
                            List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(x, false);
                            return rels.Count(s => (s.foreinProperty != null)) > 0;
                            /*
                            foreach (var rt in rels)
                            {
                                HashSet<object> refs = rt.Records;
                                if (refs.Count > 0)
                                {
                                    return true;
                                }
                            }
                            */
                        }
                        else
                        {
                            //find all ManyToOne relations rels to this entity type 
                            List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(x);
                            foreach (var rt in rels)
                            {
                                HashSet<object> refs = rt.Records;
                                if (refs.Count > 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            };
            treeControl.ChildrenGetter += delegate (TreeNode parentNode)
            {
                IList lo = new List<object>();
                if (parentNode == null)
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
                    object x = parentNode.Tag;
                    if (x == null) return new List<object>();
                    if (x is RootGroupTreeFolder)
                    {
                        Type type = (x as RootGroupTreeFolder).EntityType;
                        if (ModelHelper.IsSingleHierEntity(type))
                        {
                            return (Dm.Instance.FindRootList(type));
                        }
                        else return Dm.Instance.FindAll(type);
                    }
                    else if (x is BranchGroupTreeFolder)
                    {
                        BranchGroupTreeFolder bf = (x as BranchGroupTreeFolder);
                        if (AttrHelper.GetAttribute<JManyToMany>(bf.RefEntityInfo.foreinProperty) != null)
                            return Dm.Instance.ResolveManyToManyRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity);
                        else if (AttrHelper.GetAttribute<JManyToOne>(bf.RefEntityInfo.foreinProperty) != null)
                            return Dm.Instance.ResolveOneToManyRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity,
                                 bf.RefEntityInfo.foreinProperty.Name);
                        else if (AttrHelper.GetAttribute<JOneToOne>(bf.RefEntityInfo.foreinProperty) != null)
                            lo.Add(Dm.Instance.ResolveOneToOneRelation(bf.ParentObject, bf.RefEntityInfo.RefEntity,
                                 bf.RefEntityInfo.foreinProperty.Name));
                    }
                    else
                    {
                        bool isSingleHierEntity = ModelHelper.IsSingleHierEntity(x.GetType());
                        JEntity entityAttr = x.GetType().GetCustomAttribute<JEntity>();
                        if (entityAttr != null)
                        {
                            if (showGroupsFolder)
                            {
                                //List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(x, false);
                                List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(x);
                                foreach (var rt in rels)
                                {
                                    if (rt.foreinProperty != null)
                                    {
                                        if (rt.IsSelfRelation() && isSingleHierEntity)
                                        {
                                            //IList lll = Dm.Instance.ResolveOneToManyRelation(x, rt.RefEntity,
                                            //   rt.foreinProperty.Name);
                                            //foreach (var l1 in lll)
                                            //  lo.Add(l1);
                                            foreach (var l1 in rt.Records)
                                              lo.Add(l1);

                                        }
                                        else
                                        {
                                            if (rt.Records.Count > 0)
                                            {
                                                BranchGroupTreeFolder bf = new BranchGroupTreeFolder();
                                                bf.ParentObject = x;
                                                bf.RefEntityInfo = rt;
                                                lo.Add(bf);
                                            }
                                        }
                                    }
                                }
                            }
                            else { 
                                List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(x);

                                foreach (var rt in rels)
                                {
                                    HashSet<object> refs = rt.Records;
                                    if (refs.Count > 0)
                                    {
                                        foreach (var l in refs)
                                        {
                                            lo.Add(l);
                                        }
                                    }
                                }
                            }
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
        override protected void ComplateNodeFromObject(TreeNode node, object o)
        {
            base.ComplateNodeFromObject(node, o);

            object x = node.Tag;
            if (x == null) return;
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
                Type etype =  (x as BranchGroupTreeFolder).RefEntityInfo.RefEntity;
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
            ToolStripMenuItem menuItem = null;

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
                    menuItemList.Add(CreateAddNode(selectedListItem, bf.ParentObject, rights, bf.RefEntityInfo.RefEntity, bf.RefEntityInfo));
                }
                else
                {
                    List<RefEntityInfo> rels = Dm.Instance.GetAllReferencedToEntity(selectedObject, false);
                    foreach (var rt in rels)
                    {
                        if (rt.foreinProperty != null)
                            menuItemList.Add(CreateAddNode(selectedListItem, selectedObject, rights, rt.RefEntity, rt));
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
