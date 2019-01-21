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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace FrwSoftware
{
    public class NotificationEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);
    public delegate void RequestForExitEventHandler(object sender, EventArgs e);

    public class DocContentRegistredEventArgs : EventArgs
    {
        public IContent Content { get; set; }
    }
    public class DocContentShowEventArgs : EventArgs
    {
        public IContent Content { get; set; }
    }
    public delegate void DocContentRegistredEventHandler(object sender, DocContentRegistredEventArgs e);
    public delegate void DocContentShowEventHandler(object sender, DocContentShowEventArgs e);

    public class AppManager
    {
        /*
         * This dialogue is heavy. Therefore, when calling it from the list cell, looping occurs. 
         * The intention is to create a dialogue in advance. 
         * In general, it is impossible to do with a single dialog for the entire application. 
         * But in our case, this dialogue has so far never been called up in several copies at the same time. 
         * Therefore, in advance, we create a dialogue in this class.
         */
        SimpleHtmlTextEditDialog simpleHtmlTextEditDialog = new SimpleHtmlTextEditDialog();

        public event RequestForExitEventHandler RequestForExitEvent;
        public event NotificationEventHandler NotificationEvent;

        public AppManager()
        {

            ListCellTruncatedMaxItemCount = Dm.TRUNCATED_VALUE_MAX_ITEM_COUNT;
            TooltipTruncatedMaxItemCount = Dm.TRUNCATED_VALUE_MAX_ITEM_COUNT * 2;
            PropertyWindowTruncatedMaxItemCount = Dm.TRUNCATED_VALUE_MAX_ITEM_COUNT;
            ListCellTruncatedMaxLength = Dm.TRUNCATED_VALUE_MAX_STRING_LENGTH;
            TooltipTruncatedMaxLength = Dm.TRUNCATED_VALUE_MAX_STRING_LENGTH * 5;
            PropertyWindowTruncatedMaxLength = Dm.TRUNCATED_VALUE_MAX_STRING_LENGTH;
        }

        private static AppManager instance;
        public static AppManager Instance
        {
            get { return instance ?? (instance = new AppManager()); }
            set
            {
                if (instance != null) throw new InvalidOperationException("Custom instance can be set only once and before first getting");
                instance = value;
            }
        }
        private FrwClipboard clipboard = new FrwClipboard();
        public FrwClipboard FrwClipboard
        {
            get
            {
                return clipboard;
            }
        }

        public WebEntryInfo CurrentWebEntryInfo { get; set; }//for Hot Key operations

        private Type defaultViewWindowType = typeof(BaseViewWindow);
        public Type DefaultViewWindowType
        {
            get { return defaultViewWindowType; }
            set { defaultViewWindowType = value; }
        }

        public Type MainAppFormType { get; set; }

        public const int TRUNCATED_VALUE_MAX_ITEM_COUNT = 10;
        public const int TRUNCATED_VALUE_MAX_STRING_LENGTH = 300;

        public int ListCellTruncatedMaxItemCount { get; set; }
        public int TooltipTruncatedMaxItemCount { get; set; }
        public int PropertyWindowTruncatedMaxItemCount { get; set; }
        public int ListCellTruncatedMaxLength { get; set; }
        public int TooltipTruncatedMaxLength { get; set; }
        public int PropertyWindowTruncatedMaxLength { get; set; }

        public event DocContentRegistredEventHandler OnDocContentRegistredEvent;
        public event DocContentShowEventHandler OnDocContentShowEvent;

        private List<IContent> docContents = new List<IContent>();
        private List<IContentContainer> docContentContainers = new List<IContentContainer>();
        XmlHelper xml = new XmlHelper();//config 

        public JDocPanelLayout CurrentLayout { get; set; }

        public void LoadLayout(JDocPanelLayout layout)
        {
            CurrentLayout = layout;

            //todo load container layout 
            LoadDocPanelContainersStateLocal(layout.Containers, true);
            //todo open new  docContentContainer if need
            int g = docContents.Count;
            int i = 0;
            foreach (var c in docContentContainers)
            {
                if (layout.Layout.Length > i)
                {
                    c.LoadLayout(layout.Layout[i]);
                }
                i++;
            }

        }


        public void SaveLayout(JDocPanelLayout layout)
        {
            CurrentLayout = layout;

            layout.Layout = new string[docContentContainers.Count];
            int i = 0;
            foreach (var c in docContentContainers)
            {
                layout.Layout[i] = c.SaveLayout();
                i++;
            }

            FillDocPanelContainersState();
            layout.Containers = xml.GetDocumentString(Encoding.GetEncoding("UTF-8"));
        }
        public void CreateNewLayout(string name)
        {
            JDocPanelLayout layout = Dm.Instance.EmptyObject<JDocPanelLayout>(null);
            layout.Name = name;
            SaveLayout(layout);
            Dm.Instance.SaveObject(layout);
        }

        // the main function of creating a list object
        virtual protected IListProcessor GetListWindowForType(Type type)
        {
            BaseListWindow w = null;
            w = new SimpleListWindow();
            w.SourceObjectType = type;
            return w;
        }
        // the main function of creating a property form object 
        virtual protected IPropertyProcessor GetPropertyWindowForType(Type type)
        {
            BasePropertyWindow w = null;
            w = new SimplePropertyWindow();
            w.SourceObjectType = type;
            return w;
        }

        virtual public void MakeContextMenuBlock(Type type, List<ToolStripItem> menuItemList, object selectedObject, IContentContainer container)
        {
            if (selectedObject is JDocPanelLayout)//to base
            {
                JDocPanelLayout item = selectedObject as JDocPanelLayout;
                JDocPanelLayoutUtils.MakeContextMenuBlock(menuItemList, item, container);
            }
            else if (selectedObject is JJobType)
            {
                menuItemList.Add(new ToolStripSeparator());
                JJobType job = (JJobType)selectedObject;
                if (job != null)
                {
                    JobManager.MakeContextMenuForRunningJobBatch(job, menuItemList, container);
                }
            }
            else if (selectedObject is JRunningJob)
            {
                menuItemList.Add(new ToolStripSeparator());
                JRunningJob job = (JRunningJob)selectedObject;
                if (job != null)
                {
                    JobManager.MakeContextMenuForRunningJob(job, menuItemList, container);
                }
            }
        }


        // is called after the window is created
        // Used to set custom event links between windows
        virtual protected void PostCreateContent(IContentContainer docPanelContainer, IContent c)
        {
        }

        virtual protected BaseMainAppForm GetMainForm()
        {
            return new BaseMainAppForm();
        }

        #region baseoperation


        public IContent CreateContentAndProcessView(IContentContainer docPanelContainer, Type contType, IDictionary<string, object> pars)
        {
            IContent cc = CreateContent(docPanelContainer, contType, null, pars, true, null, null);
            if (cc is IViewProcessor) (cc as IViewProcessor).ProcessView();
            return cc;
        }
        public IContent CreateContent(IContentContainer docPanelContainer, Type contType, IDictionary<string, object> pars)
        {
            IContent cc = CreateContent(docPanelContainer, contType, null, pars, true, null, null);
            return cc;
        }
        public void CreateAndProcessListContentForModelType(Type modelType, IContentContainer docPanelContainer)
        {
            IContent c = CreateListContentForModelType(docPanelContainer, null, modelType, null);
            if (c is IViewProcessor) (c as IViewProcessor).ProcessView();
        }
        public IContent CreateListContentForModelType(IContentContainer docPanelContainer, IParentView parentProcessor, Type modelType, IDictionary<string, object> pars)
        {
            return CreateContent(docPanelContainer, typeof(IListProcessor), modelType, pars, true, null, parentProcessor != null ? parentProcessor.PaneUID : null);
        }
        public IPropertyProcessor CreatePropertyContentForModelType(IContentContainer docPanelContainer, IParentView parentProcessor, Type modelType, IDictionary<string, object> pars)
        {
            return (IPropertyProcessor)CreateContent(docPanelContainer,
                typeof(IPropertyProcessor), modelType, pars, true, null, parentProcessor != null ? parentProcessor.PaneUID : null);
        }
        public IContent CreateStoredContent(IContentContainer docPanelContainer, IDictionary<string, object> pars)
        {
            string typeName = DictHelper.GetString(pars, FrwBaseViewControl.PersistStringTypeParameter);
            if (typeName == null) throw new InvalidOperationException(FrwBaseViewControl.PersistStringTypeParameter + " not found in persistent parameters.");
            Type contType = TypeHelper.FindType(typeName);
            string paneUID = DictHelper.GetString(pars, FrwBaseViewControl.PersistStringPaneUIDParameter);
            string relPaneUID = DictHelper.GetString(pars, FrwBaseViewControl.PersistStringRelPaneUIDParameter);
            return CreateContent(docPanelContainer, contType, null, pars, false, paneUID, relPaneUID);
        }
        private IContent CreateContent(IContentContainer docPanelContainer, Type contType,
            Type modelType, IDictionary<string, object> pars, bool forceRegistredEvent, string paneUID, string relPaneUID)
        {
            IContent c = localFindContent(contType, modelType, pars);
            if (c == null)
            {

                if (docPanelContainer == null) docPanelContainer = GetDefaultDocPanelContainer();
                c = CreateNewContentInstance(contType, modelType, pars);
                if (docContents.Contains(c) == false) docContents.Add(c);
                //common
                c.ContentContainer = docPanelContainer;

                if (c is IParentView)
                {
                    if (paneUID == null) paneUID = DataUtils.genKey(null);
                    (c as IParentView).PaneUID = paneUID;
                }
                if (c is IChildView)
                {
                    IChildView child = c as IChildView;
                    child.RelPaneUID = relPaneUID;
                    foreach (IContent s1 in docContents)
                    {
                        if (s1 is IParentView)
                        {
                            IParentView parent = s1 as IParentView;
                            if (parent.PaneUID == child.RelPaneUID && parent.ContainsChildView(child) == false)
                            {
                                parent.AddChildView(child);
                            }
                        }
                    }
                }
                PostCreateContent(docPanelContainer, c);
                if (forceRegistredEvent)// registration is not required when calling from a saved state that is already registered
                {
                    if (OnDocContentRegistredEvent != null)
                    {
                        OnDocContentRegistredEvent(null, new DocContentRegistredEventArgs()
                        {
                            Content = c
                        });
                    }
                }
            }
            if (OnDocContentShowEvent != null)//null при начальной загрузке 
            {
                OnDocContentShowEvent(null, new DocContentShowEventArgs()
                {
                    Content = c
                });
            }
            return c;
        }

        public T FindContent<T>(Type modelType = null, IDictionary<string, object> pars = null)
        {
            IContent c = localFindContent(typeof(T), modelType, pars);
            return (T)c;
        }

        public IContent FindContent(Type contType, Type modelType, IDictionary<string, object> pars = null)
        {
            IContent c = localFindContent(contType, modelType, pars);
            return c;
        }


        // search for a window, does not create a window
        // the search is always done on indirect attributes, but not on the object itself
        //т.к. There are objects reconstructed from the configuration
        private IContent localFindContent(Type contType, Type modelType, IDictionary<string, object> pars = null)
        {
            if (contType == null && modelType == null) throw new InvalidOperationException();
            foreach (var ss in docContents)
            {
                if (contType != null && AttrHelper.IsSameOrSubclass(contType, ss.GetType()) == false) continue;
                else if (modelType != null && (!(ss is IObjectViewProcessor) || (modelType != (ss as IObjectViewProcessor).SourceObjectType))) continue;
                if (pars == null || ss.CompareKeyParams(pars) == true) return ss;
            }
            return null;
        }
        // used in lists to create the properties window as a window in the container
        // used in the lists to create a selection list window as a dialog
        // used in the lists to create the property window as a dialog
        public IContent CreateNewContentInstance(Type contType, Type modelType, IDictionary<string, object> pars = null)
        {
            IContent c = null;
            if (modelType != null)
            {
                if (contType == typeof(IListProcessor))
                    c = GetListWindowForType(modelType);
                else if (contType == typeof(IPropertyProcessor))
                    c = GetPropertyWindowForType(modelType);
                else throw new InvalidOperationException();

                if (c is IObjectViewProcessor) ((IObjectViewProcessor)c).SourceObjectType = modelType;
            }
            else if (contType != null)
            {
                c = (IContent)Activator.CreateInstance(contType);
            }
            else throw new InvalidOperationException();
            c.SetKeyParams(pars);
            if (c is IViewProcessor) (c as IViewProcessor).CreateView();
            return c;
        }


        // is called after the application is downloaded
        // done because DocPanel itself creates a cone inside and needs to call ProcessView after
        public void ProcessViewForAllCreatedDocContents(IContentContainer docPanelContainer)
        {
            foreach (IContent s in docContents)
            {
                if (docPanelContainer == s.ContentContainer)
                {
                    if (s is IChildView)
                    {
                        IChildView child = s as IChildView;
                        if (child.RelPaneUID != null)
                        {
                            foreach (IContent s1 in docContents)
                            {
                                if (s1 is IParentView)
                                {
                                    IParentView parent = s1 as IParentView;
                                    if (parent.PaneUID == child.RelPaneUID && parent.ContainsChildView(child) == false)
                                    {
                                        parent.AddChildView(child);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (IContent s in docContents)
            {
                if (s is IViewProcessor) (s as IViewProcessor).ProcessView();
            }
        }


        public void RegisterDocPanelContainer(IContentContainer dc)
        {
            docContentContainers.Add(dc);
            dc.DocPanelIndex = docContentContainers.Count - 1;
            //if (dc is Form) (dc as Form).FormClosed += AppManager_FormClosed;
        }
        public void UnRegisterDocPanelContainer(IContentContainer dc)
        {
            docContentContainers.Remove(dc);
        }
        /*
        private void AppManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is IContentContainer)
                docContentContainers.Remove(sender as IContentContainer);
        }
        */
        public void ActivateDocPanelContainers()
        {
            foreach (var c in docContentContainers)
            {
                if (c is Form)
                {
                    (c as Form).Show();
                    (c as Form).Activate();
                    if ((c as Form).WindowState == FormWindowState.Minimized)
                        (c as Form).WindowState = FormWindowState.Normal;
                }
            }
        }
        public void ActivateNotificationPanel()
        {
            foreach (var c in docContentContainers)
            {
                c.ActivateNotificationPanel();
                break;//onle first container 
            }
        }

        private IContentContainer GetDefaultDocPanelContainer()
        {
            return docContentContainers[0];
        }




        // is called when the main window closes the document
        public void RemoveDocContent(IContent c)
        {
            bool res = docContents.Remove(c);
            //remove from child views 
            if (c is IChildView)
            {
                IChildView child = c as IChildView;
                foreach (IContent s1 in docContents)
                {
                    if (s1 is IParentView)
                    {
                        IParentView parent = s1 as IParentView;
                        if (parent.ContainsChildView(child) == true)
                        {
                            parent.RemoveChildView(child);
                            break;
                        }
                    }
                }
            }
        }


        #endregion
        #region saverestorestate
        public void SaveAndClose(Form initiator)
        {
            FillDocPanelContainersState();
            string configFilePath = GetDockPanelContainersXmlPath();
            xml.SaveDocument(configFilePath);

            //close other containers 
            List<IContentContainer> tmp = new List<IContentContainer>();
            tmp.AddRange(docContentContainers);
            foreach (var c in tmp)
            {
                if (c != initiator)
                {
                    c.CloseDocPanelContainer();
                }
            }
        }

        private void FillDocPanelContainersState()
        {
            RectangleConverter rectConverter = new RectangleConverter();
            if (xml.RootNode == null)
            {
                xml.CreateRoot("DockPanelContainers");
            }
            XmlNode screenSystem = xml.RootNode.SelectSingleNode("//DockPanelContainers/ScreenSystem[@ScreenCount = \'" + Screen.AllScreens.Length.ToString() + "\']");
            if (screenSystem == null)
            {
                screenSystem = xml.Doc.CreateElement("ScreenSystem");
                xml.setAttrValue(screenSystem, "ScreenCount", Screen.AllScreens.Length.ToString());
                xml.RootNode.AppendChild(screenSystem);
            }
            else
            {
                screenSystem.RemoveAll();
                xml.setAttrValue(screenSystem, "ScreenCount", Screen.AllScreens.Length.ToString());//RemoveAll() also remove attributes
            }

            foreach (var c in docContentContainers)
            {
                Form cf = c as Form;
                XmlNode dc = xml.Doc.CreateElement("DockPanelContainer");
                screenSystem.AppendChild(dc);

                //xmlOut.WriteAttributeString("ID", c.DocPanelIndex.ToString(CultureInfo.InvariantCulture));
                xml.setAttrValue(dc, "Bounds", rectConverter.ConvertToInvariantString(c.DocPanelBounds));
                xml.setAttrValue(dc, "Left", cf.Location.X.ToString());
                xml.setAttrValue(dc, "Top", cf.Location.Y.ToString());
                xml.setAttrValue(dc, "Width", cf.Size.Width.ToString());
                xml.setAttrValue(dc, "Height", cf.Size.Height.ToString());
                xml.setAttrValue(dc, "WindowState", cf.WindowState.ToString());
                xml.setAttrValue(dc, "Type", c.GetType().FullName);
                //xmlOut.WriteAttributeString("ZOrderIndex", fw.DockPanel.FloatWindows.IndexOf(fw).ToString(CultureInfo.InvariantCulture));
            }

        }


        private void TestIfLocationInScreens(int x, int y, int width, int height, int zapas, out bool locationInScreen, out bool rightBottomInScreen)
        {
            locationInScreen = false;
            rightBottomInScreen = false;
            int xCenter = x + width / 2;
            int yCenter = y + height / 2;
            foreach (var s in Screen.AllScreens)
            {

                if ((xCenter >= s.Bounds.X && xCenter < (s.Bounds.X + s.Bounds.Width - zapas))
                    &&
                    (yCenter >= s.Bounds.Y && yCenter < (s.Bounds.Y + s.Bounds.Height - zapas))
                    )
                {
                    locationInScreen = true;
                    if ((x + width) <= (s.Bounds.X + s.Bounds.Width)
                       &&
                       (y + height) <= (s.Bounds.Y + s.Bounds.Height))
                    {
                        rightBottomInScreen = true;
                    }

                    break;
                }
            }

        }
        public Form LoadDocPanelContainersState(bool visible)
        {
            if (MainAppFormType == null) throw new Exception("MainAppFormType must be set before  call of this method");

            string configFilePath = GetDockPanelContainersXmlPath();
            FileInfo fi = new FileInfo(configFilePath);
            if (fi.Exists == true) LoadDocPanelContainersStateLocal(File.ReadAllText(fi.FullName), visible);
            else LoadDocPanelContainersStateLocal(null, visible);

            object mainForm = docContentContainers.Count > 0 ? docContentContainers[0] : null;
            if (mainForm == null)
            {
                //in case: 1) context mode - all forms closed in previous session 
                //2) config file lost
                if (BaseApplicationContext.IsContextMode == true)
                    BaseMainAppForm.CreateWindowNotClosable = true;
                else
                    BaseMainAppForm.CreateWindowNotClosable = false;
                mainForm = Activator.CreateInstance(MainAppFormType);
            }
            return (Form)mainForm;
        }
        private string GetDockPanelContainersXmlPath()
        {
            return Path.Combine(FrwConfig.Instance.ProfileConfigDir, "DockPanelContainersXml.xml");
        }
        private void LoadDocPanelContainersStateLocal(string xmlStr, bool visible)
        {
            bool loaded = false;
            bool firstContainer = true;
            try
            {
                if (xmlStr == null) xml.CreateRoot("DockPanelContainers");
                else xml.LoadDocumentFromString(xmlStr);

                XmlNode screenSystem = xml.RootNode.SelectSingleNode("//DockPanelContainers/ScreenSystem[@ScreenCount = \'" + Screen.AllScreens.Length.ToString() + "\']");
                if (screenSystem == null) screenSystem = xml.RootNode.SelectSingleNode("//DockPanelContainers/ScreenSystem[@ScreenCount = \'" + 1 + "\']");
                if (screenSystem != null)
                {
                    XmlNodeList dcs = screenSystem.SelectNodes("DockPanelContainer");
                    int numberInXml = 0;
                    foreach (XmlNode dc in dcs)
                    {
                        int numberInList = 0;
                        IContentContainer form = null;
                        foreach (var c in docContentContainers)
                        {
                            if (numberInList == numberInXml)
                            {
                                form = c;
                                break;
                            }
                            numberInList++;
                        }
                        if (form == null)
                        {
                            if (firstContainer && BaseApplicationContext.IsContextMode == true)
                                BaseMainAppForm.CreateWindowNotClosable = true;
                            else
                                BaseMainAppForm.CreateWindowNotClosable = false;
                            if (firstContainer) firstContainer = false;
                            string typeStr = xml.getAttrValue(dc, "Type");
                            if (typeStr != null)
                            {
                                form = (IContentContainer)TypeHelper.FindTypeAddCreateNewInstance(typeStr);
                            }
                            else
                            {
                                form = (IContentContainer)Activator.CreateInstance(MainAppFormType);
                            }
                        }
                        //Form form = GetMainForm();
                        //? So that the size setting works, you must also install the WindowState and StartPosition in the form designer
                        FormWindowState windowState = xml.getAttrValue(dc, "WindowState").ToEnum(FormWindowState.Normal);
                        //form.WindowState = FormWindowState.Normal;
                        ((Form)form).WindowState = windowState;
                        bool locationInScreen = false;
                        bool rightBottomInScreen = false;
                        Point p = new Point(int.Parse(xml.getAttrValue(dc, "Left")), int.Parse(xml.getAttrValue(dc, "Top")));
                        Size s = new Size(int.Parse(xml.getAttrValue(dc, "Width")), int.Parse(xml.getAttrValue(dc, "Height")));
                        //Rectangle rect = (Rectangle)rectConverter.ConvertFromInvariantString(xmlIn.GetAttribute("Bounds"));
                        TestIfLocationInScreens(p.X, p.Y, s.Width, s.Height, 20, out locationInScreen, out rightBottomInScreen);
                        if (locationInScreen)//если попали в экраны 
                        {
                            ((Form)form).StartPosition = FormStartPosition.Manual;
                            //form.Bounds = new Rectangle(p.X, p.Y, s.Width, s.Height); - same as Size, Location
                            ((Form)form).Location = p;
                            if (windowState == FormWindowState.Normal && rightBottomInScreen)
                            {
                                ((Form)form).Size = s;
                                // you should keep in mind that if this.AutoScaleMode = Font That setting a new font will change the size of the window
                            }
                            else
                            { // if the size is out of bounds, we maximize the window
                                ((Form)form).WindowState = FormWindowState.Maximized;
                            }
                        }
                        else
                        {
                            // if not hit the screen maximize at the zero point (this is the main screen)
                            ((Form)form).StartPosition = FormStartPosition.Manual;
                            ((Form)form).Location = new Point(0, 0);
                            ((Form)form).WindowState = FormWindowState.Maximized;
                        }
                        numberInXml++;
                    }
                    loaded = true;
                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
            if (!loaded || docContentContainers.Count == 0)
            {
                /*
                //todo clear and create default
                docContentContainers.Clear();
                Form form = GetMainForm();
                form.StartPosition = FormStartPosition.WindowsDefaultLocation;
                form.WindowState = FormWindowState.Maximized;
                */
            }
            //show
            if (visible)
            {
                foreach (var c in docContentContainers)
                {
                    if (c is Form) (c as Form).Show();
                }
            }
        }

        public IContentContainer GetMainContainer()
        {
            return docContentContainers.Count > 0 ? docContentContainers[0] : null;
        }

        #endregion

        #region CustomPropertyEditor

        virtual public bool IsCustomEditProperty(Type sourceObjectType, string aspectName)
        {
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, aspectName);
            PropertyInfo propInfo = sourceObjectType.GetProperty(aspectName);
            if (AttrHelper.GetAttribute<JOneToMany>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JPrimaryKey>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JManyToMany>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JOneToOne>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JManyToOne>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JDictProp>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JImageName>(propInfo) != null) return true;
            else if (AttrHelper.GetAttribute<JImageRef>(propInfo) != null) return true;
            else if (pType == typeof(DateTime) || pType == typeof(DateTimeOffset) || pType == typeof(DateTime?) || pType == typeof(DateTimeOffset?)) return true;
            else if (AttrHelper.GetAttribute<JText>(propInfo) != null) return true;
            else if (pType == typeof(JAttachment)) return true;
            else if (AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment))) return true;
            else if (AttrHelper.IsGenericList(pType)) return true;// AttrHelper.IsSameOrSubclass(typeof(IList), pType))//must be last check 

            else return false;

        }

        // in the column of the match, do not show the string from the field, and show only the picture
        virtual public bool IsOnlyImageColumnProperty(Type sourceObjectType, string aspectName)
        {
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, aspectName);
            PropertyInfo propInfo = sourceObjectType.GetProperty(aspectName);
            if (AttrHelper.GetAttribute<JText>(sourceObjectType, aspectName) != null) return true;
            else if (pType == typeof(JAttachment)) return true;
            else if (AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment))) return true;
            else if (AttrHelper.GetAttribute<JDictProp>(propInfo) != null)
            {
                JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(propInfo);
                if (dictAttr.DictPropertyStyle == DisplyPropertyStyle.ImageOnly) return true;
                else return false;
            }
            else return false;
        }




        public object ProcessEditCustomPropertyValueAndSave(object rowObject, string aspectName, out bool complated, IWin32Window owner)
        {
            Type sourceObjectType = rowObject.GetType();
            PropertyInfo p = sourceObjectType.GetProperty(aspectName);
            object oldValue = null;
            if (p.PropertyType is IList)
            {
                IList l = (IList)p.GetValue(rowObject);
                oldValue = new List<object>();
                foreach (var o in l)
                {
                    ((IList)oldValue).Add(o);
                }
            }
            else
            {
                oldValue = p.GetValue(rowObject);
            }
            try
            {
                object newValue = EditCustomPropertyValue(rowObject, aspectName, out complated, owner);
                Dm.Instance.SaveObject(rowObject);
                if (complated)
                {
                    return newValue;
                }
                else return oldValue;
            }
            catch (JValidationException ex)
            {
                AppManager.ShowValidationErrorMessage(ex.ValidationResult);
                if (p.PropertyType is IList)
                {
                    IList l = (IList)p.GetValue(rowObject);
                    l.Clear();
                    foreach (var o in (IList)oldValue)
                    {
                        l.Add(o);
                    }
                }
                else
                {
                    p.SetValue(rowObject, oldValue);
                }
                complated = true;
                return oldValue;
            }
        }
        public object ProcessEditCustomPropertyValue(object rowObject, string aspectName, out bool complated, IWin32Window owner)
        {
            return EditCustomPropertyValue(rowObject, aspectName, out complated, owner);
        }
        virtual public object EditCustomSettingValue(JSetting setting, out bool complated, IWin32Window owner)
        {
            complated = false;
            Type propertyType = setting.ValueType;
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(propertyType);
            if (setting.DictId != null)
            {
                if (setting.AllowMultiValues)
                {
                    SimpleMultivalueDictFieldItemListDialog listDialog = new SimpleMultivalueDictFieldItemListDialog(setting.DictId);
                    IList list = setting.Value as IList;
                    List<JDictItem> listd = new List<JDictItem>();
                    if (list != null)
                    {
                        foreach (var l in list)
                        {
                            listd.Add(Dm.Instance.GetDictText(setting.DictId, l.ToString()));
                        }
                    }
                    listDialog.SourceObjects = listd;
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK)
                    {
                        IList newObjects = listDialog.SourceObjects;//SourceObjects

                        if (propertyType == typeof(int))
                        {
                            List<int> listkeys = new List<int>();
                            foreach (var newObject in newObjects)
                            {
                                listkeys.Add(int.Parse(((JDictItem)newObject).Key));
                            }
                            setting.Value = listkeys;
                        }
                        if (propertyType == typeof(long))
                        {
                            List<long> listkeys = new List<long>();
                            foreach (var newObject in newObjects)
                            {
                                listkeys.Add(long.Parse(((JDictItem)newObject).Key));
                            }
                            setting.Value = listkeys;
                        }
                        else
                        {
                            List<string> listkeys = new List<string>();
                            foreach (var newObject in newObjects)
                            {
                                listkeys.Add(((JDictItem)newObject).Key);
                            }
                            setting.Value = listkeys;
                        }
                        complated = true;
                    }
                }
                else
                {
                    SimpleDictListDialog listDialog = new SimpleDictListDialog(setting.DictId, false);
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK && listDialog.SelectedObjects != null)
                    {
                        IList newObjects = listDialog.SelectedObjects;
                        if (newObjects.Count > 0)
                        {
                            JDictItem d = (JDictItem)newObjects[0];
                            if (propertyType == typeof(int))
                                setting.Value = int.Parse(d.Key);
                            else if (propertyType == typeof(long))
                                setting.Value = long.Parse(d.Key);
                            else
                                setting.Value = d.Key;
                        }
                        else setting.Value = null;
                        complated = true;
                    }
                }

            }
            else if (entityAttr != null)
            {
                if (setting.AllowMultiValues)
                {
                    SimpleMultivalueFieldItemListDialog listDialog = new SimpleMultivalueFieldItemListDialog(propertyType);
                    IList list = (IList)setting.Value;
                    IList tempLIst = new List<object>();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            tempLIst.Add(item);
                        }
                    }
                    listDialog.SourceObjects = tempLIst;
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK && listDialog.SourceObjects != null)
                    {
                        IList newList = listDialog.SourceObjects;
                        IList newListToSave = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyType));
                        if (newList != null)
                        {
                            foreach (var l in newList)
                            {
                                newListToSave.Add(l);
                            }
                        }
                        setting.Value = newListToSave;
                        complated = true;
                    }
                }
                else
                {
                    SimpleListDialog listDialog = new SimpleListDialog(propertyType);
                    listDialog.EnableSetNull = true;
                    //todo select value in dialog
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK && listDialog.SelectedObjects != null && listDialog.SelectedObjects.Count > 0)
                    {
                        IList newObjects = listDialog.SelectedObjects;
                        object value = newObjects[0];
                        setting.Value = value;
                        complated = true;
                    }
                    else if (res == DialogResult.Abort)
                    {
                        setting.Value = null;
                        complated = true;
                    }
                }
            }
            object newStrValue = null;
            if (complated)
            {
                newStrValue = Dm.Instance.GetCustomSettingValue(setting);
            }
            return newStrValue;
        }
        virtual public object EditCustomPropertyValue(object rowObject, string aspectName, out bool complated, IWin32Window owner)
        {
            Type sourceObjectType = rowObject.GetType();
            Type pType = AttrHelper.GetPropertyType(sourceObjectType, aspectName);
            PropertyInfo p = sourceObjectType.GetProperty(aspectName);
            JOneToOne oneToOneAttr = AttrHelper.GetAttribute<JOneToOne>(sourceObjectType, aspectName);
            JManyToOne manyToOneAttr = AttrHelper.GetAttribute<JManyToOne>(sourceObjectType, aspectName);
            JOneToMany oneToManyAttr = AttrHelper.GetAttribute<JOneToMany>(sourceObjectType, aspectName);
            JManyToMany manyToManyAttr = AttrHelper.GetAttribute<JManyToMany>(sourceObjectType, aspectName);
            JDictProp dictAttr = AttrHelper.GetAttribute<JDictProp>(sourceObjectType, aspectName);
            //todo readonly mode in dialogs 
            JReadOnly readOnlyAttr = AttrHelper.GetAttribute<JReadOnly>(sourceObjectType, aspectName);
            complated = false;
            if (oneToOneAttr != null || manyToOneAttr != null)
            {
                SimpleListDialog listDialog = new SimpleListDialog(p.PropertyType);
                listDialog.EnableSetNull = true;
                //todo select value in dialog
                DialogResult res = listDialog.ShowDialog(owner);
                if (res == DialogResult.OK && listDialog.SelectedObjects != null && listDialog.SelectedObjects.Count > 0)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        IList newObjects = listDialog.SelectedObjects;
                        object value = newObjects[0];
                        AttrHelper.SetPropertyValue(rowObject, aspectName, value);
                        complated = true;
                    }
                }
                else if (res == DialogResult.Abort)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        AttrHelper.SetPropertyValue(rowObject, aspectName, null);
                        complated = true;
                    }
                }
            }
            else if (dictAttr != null)
            {
                if (dictAttr.AllowMultiValues)
                {
                    SimpleMultivalueDictFieldItemListDialog listDialog = new SimpleMultivalueDictFieldItemListDialog(dictAttr.Id);
                    IList list = AttrHelper.GetPropertyValue(rowObject, aspectName) as IList;
                    List<JDictItem> listd = new List<JDictItem>();
                    if (list != null)
                    {
                        foreach (var l in list)
                        {
                            listd.Add(Dm.Instance.GetDictText(dictAttr.Id, l.ToString()));
                        }
                    }
                    listDialog.SourceObjects = listd;
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK)
                    {
                        if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                             FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                        {
                            IList newObjects = listDialog.SourceObjects;//SourceObjects

                            if (AttrHelper.IsGenericListTypeOf(typeof(int), p.PropertyType))
                            {
                                List<int> listkeys = new List<int>();
                                foreach (var newObject in newObjects)
                                {
                                    listkeys.Add(int.Parse(((JDictItem)newObject).Key));
                                }
                                AttrHelper.SetPropertyValue(rowObject, aspectName, listkeys);
                            }
                            else if (AttrHelper.IsGenericListTypeOf(typeof(long), p.PropertyType))
                            {
                                List<long> listkeys = new List<long>();
                                foreach (var newObject in newObjects)
                                {
                                    listkeys.Add(long.Parse(((JDictItem)newObject).Key));
                                }
                                AttrHelper.SetPropertyValue(rowObject, aspectName, listkeys);
                            }
                            else
                            {
                                List<string> listkeys = new List<string>();
                                foreach (var newObject in newObjects)
                                {
                                    listkeys.Add(((JDictItem)newObject).Key);
                                }
                                AttrHelper.SetPropertyValue(rowObject, aspectName, listkeys);
                            }
                            complated = true;
                        }
                    }
                }
                else
                {
                    SimpleDictListDialog listDialog = new SimpleDictListDialog(dictAttr.Id, false);
                    DialogResult res = listDialog.ShowDialog(owner);
                    if (res == DialogResult.OK && listDialog.SelectedObjects != null)
                    {
                        if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                             FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        else
                        {
                            IList newObjects = listDialog.SelectedObjects;
                            if (newObjects.Count > 0)
                            {
                                JDictItem d = (JDictItem)newObjects[0];
                                if (p.PropertyType == typeof(int))
                                    AttrHelper.SetPropertyValue(rowObject, aspectName, int.Parse(d.Key));
                                else if (p.PropertyType == typeof(long))
                                    AttrHelper.SetPropertyValue(rowObject, aspectName, long.Parse(d.Key));
                                else
                                    AttrHelper.SetPropertyValue(rowObject, aspectName, d.Key);
                            }
                            else AttrHelper.SetPropertyValue(rowObject, aspectName, null);
                            complated = true;
                        }
                    }
                }
            }
            else if (oneToManyAttr != null)
            {
                SimpleMultivalueFieldItemListDialog listDialog = new SimpleMultivalueFieldItemListDialog(AttrHelper.GetGenericListArgType(p.PropertyType));
                IList list = (IList)AttrHelper.GetPropertyValue(rowObject, aspectName);
                IList tempLIst = new List<object>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        tempLIst.Add(item);
                    }
                }
                listDialog.SourceObjects = tempLIst;
                DialogResult res = listDialog.ShowDialog(owner);
                if (res == DialogResult.OK && listDialog.SourceObjects != null)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        IList newList = listDialog.SourceObjects;
                        IList newListToSave = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(AttrHelper.GetGenericListArgType(pType)));
                        if (newList != null)
                        {
                            foreach (var l in newList)
                            {
                                newListToSave.Add(l);
                            }
                        }
                        AttrHelper.SetPropertyValue(rowObject, aspectName, newListToSave);
                        complated = true;
                    }
                }
            }
            else if (manyToManyAttr != null)
            {
                SimpleMultivalueFieldItemListDialog listDialog = new SimpleMultivalueFieldItemListDialog(AttrHelper.GetGenericListArgType(p.PropertyType));
                IList list = (IList)AttrHelper.GetPropertyValue(rowObject, aspectName);
                IList tempLIst = new List<object>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        tempLIst.Add(item);
                    }
                }
                listDialog.SourceObjects = tempLIst;
                DialogResult res = listDialog.ShowDialog(owner);
                if (res == DialogResult.OK && listDialog.SourceObjects != null)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        IList newList = listDialog.SourceObjects;
                        IList newListToSave = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(AttrHelper.GetGenericListArgType(pType)));
                        if (newList != null)
                        {
                            foreach (var l in newList)
                            {
                                newListToSave.Add(l);
                            }
                        }
                        AttrHelper.SetPropertyValue(rowObject, aspectName, newListToSave);
                        complated = true;
                    }
                }
            }
            else if (pType == typeof(JAttachment) || AttrHelper.IsGenericListTypeOf(pType, typeof(JAttachment)))
            {
                //todo make clone 
                List<JAttachment> attachments = null;
                if (pType == typeof(JAttachment))
                {
                    attachments = new List<JAttachment>();
                    JAttachment s = AttrHelper.GetPropertyValue(rowObject, aspectName) as JAttachment;
                    if (s != null) attachments.Add(s);
                }
                else
                {
                    attachments = AttrHelper.GetPropertyValue(rowObject, aspectName) as List<JAttachment>;
                }
                SimpleAttachmentsDialog dialog = new SimpleAttachmentsDialog();

                dialog.SourceObject = rowObject;
                dialog.SourceObjects = attachments;//!!! after CommonStoragePath and StoragePrefixPath
                                                   //dialog.EditedText = s;
                DialogResult res = dialog.ShowDialog(owner);
                if (res == DialogResult.OK)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        AttrHelper.SetPropertyValue(rowObject, aspectName, dialog.SourceObjects);
                        complated = true;
                    }
                }
            }
            else if (AttrHelper.GetAttribute<JText>(sourceObjectType, aspectName) != null)
            {
                string s = AttrHelper.GetPropertyValue(rowObject, aspectName) as string;
                //SimpleTextEditDialog dialog = new SimpleTextEditDialog();
                simpleHtmlTextEditDialog.EditedText = s;
                DialogResult res = simpleHtmlTextEditDialog.ShowDialog(owner);
                if (res == DialogResult.OK)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        s = simpleHtmlTextEditDialog.EditedText;
                        AttrHelper.SetPropertyValue(rowObject, aspectName, s);
                        complated = true;
                    }
                }
            }
            else if (pType == typeof(DateTime) || pType == typeof(DateTimeOffset) || pType == typeof(DateTime?) || pType == typeof(DateTimeOffset?))
            {
                SimpleDateTimeDialog dialog = new SimpleDateTimeDialog();
                DateTime? date = AttrHelper.GetPropertyValue(rowObject, aspectName) as DateTime?;
                if (date != null)
                    dialog.Date = (DateTime)date;
                DialogResult res = dialog.ShowDialog(owner);
                if (res == DialogResult.OK)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        if (pType == typeof(DateTime) || pType == typeof(DateTime?))
                        {
                            if (dialog.IsDateTimeNull)
                            {
                                if (pType == typeof(DateTime)) AttrHelper.SetPropertyValue(rowObject, aspectName, DateTime.MinValue);
                                else if (pType == typeof(DateTime?)) AttrHelper.SetPropertyValue(rowObject, aspectName, null);
                            }
                            else
                            {
                                AttrHelper.SetPropertyValue(rowObject, aspectName, dialog.Date);
                            }
                        }
                        else if (pType == typeof(DateTimeOffset) || pType == typeof(DateTimeOffset?))
                        {
                            if (dialog.IsDateTimeNull)
                            {
                                if (pType == typeof(DateTimeOffset)) AttrHelper.SetPropertyValue(rowObject, aspectName, DateTimeOffset.MinValue);
                                else if (pType == typeof(DateTimeOffset?)) AttrHelper.SetPropertyValue(rowObject, aspectName, null);
                            }
                            else
                            {
                                DateTimeOffset d = (DateTimeOffset)dialog.Date;
                                AttrHelper.SetPropertyValue(rowObject, aspectName, d);
                            }
                        }
                        complated = true;
                    }
                }//cancel
            }
            else if (AttrHelper.IsGenericList(pType))// AttrHelper.IsSameOrSubclass(typeof(IList), pType))//must be last check 
            {
                //todo make clone 
                Type argType = AttrHelper.GetGenericListArgType(pType);
                IList s = AttrHelper.GetPropertyValue(rowObject, p) as IList;
                SimpleGenericListFieldItemListDialog dialog = new SimpleGenericListFieldItemListDialog(argType);
                dialog.SourceObjects = s;
                //dialog.SourceObjects = attachments;//!!! after CommonStoragePath and StoragePrefixPath
                                                   //dialog.EditedText = s;
                DialogResult res = dialog.ShowDialog(owner);
                if (res == DialogResult.OK)
                {
                    if (readOnlyAttr != null) MessageBox.Show(null, FrwCRUDRes.This_field_is_readonly,
                                         FrwCRUDRes.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        AttrHelper.SetPropertyValue(rowObject, aspectName, dialog.SourceObjects);
                        complated = true;
                    }
                }
            }

            object newStrValue = null;
            if (complated)
            {
                newStrValue = Dm.Instance.GetCustomPropertyValue(rowObject, aspectName);
            }
            return newStrValue;
        }
        public static void ShowValidationErrorMessage(JValidationResult res, IWin32Window owner = null)
        {
            MessageBox.Show(owner, res.GetFullErrorString(), FrwConstants.WARNING, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion
        public void ProcessNotification(string message)
        {
            if (NotificationEvent != null)
            {
                NotificationEvent(null, new NotificationEventArgs() { Message = message });
            }
        }
        public void ProcessNotification(NotificationEventArgs e)
        {
            if (NotificationEvent != null)
            {
                NotificationEvent(null, e);
            }
        }
        public void ExitApplication()
        {
            if (RequestForExitEvent != null)
            {
                RequestForExitEvent(null, new EventArgs());
            }
        }
        virtual public List<ToolStripItem> CreateOpenInBrowserContextMenu(WebEntryInfo webEntryInfo, IContentContainer contentContainer, object selectedObject, string webEntityInfoPropertyName = null)
        {
            return new List<ToolStripItem>();

        }
        virtual public List<ToolStripItem> CreateGetPasswordContextMenu(Action<string> setNewPassword)
        {
            return new List<ToolStripItem>();
        }
    }

    public class FrwOlvDataObject : DataObject
    {
        public IList SelectedObjects { get; set; }
    }
    public class FrwTreeDataObject : DataObject
    {
        public TreeNode SelectedTreeNode { get; set; }
    }

    public class FrwClipboard {
        public IDataObject DataObject { get; set; }
        public bool IsCutOperation { get; set; }
        public void Clear()
        {
            DataObject = null;
        }
    }
}
