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
using System.Dynamic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace FrwSoftware
{
    public partial class BaseMainAppForm : Form, IContentContainer
    {
        protected Queue<string> localNotificationQueue = new Queue<string>();
        protected JobLog localNotificationLog = new JobLog();
        protected System.Windows.Forms.Timer notificationTimer;
        protected System.Windows.Forms.ToolStripSplitButton eventWarrningButton;

        static public bool CreateWindowNotClosable = false;//for create this instance (changed when before next instance creation)
        protected bool NotClosable { get; set; }//for this instance (persistent)

        protected DeserializeDockContent m_deserializeDockContent;

        public int DocPanelIndex { get; set; }
        public Rectangle DocPanelBounds
        {
            get
            {
                return this.Bounds;
            }
            set
            {
                this.Bounds = value;
            }
        }
        public BaseMainAppForm()
        {
            InitializeComponent();
            //if (CreateWindowNotClosable) NotClosable = true;
            // 
            // eventWarrningButton
            // 
            this.eventWarrningButton = new System.Windows.Forms.ToolStripSplitButton();
            this.eventWarrningButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            //this.eventWarrningButton.Enabled = false;
            this.eventWarrningButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eventWarrningButton.Name = "eventWarrningButton";
            this.eventWarrningButton.Size = new System.Drawing.Size(19, 23);
            this.eventWarrningButton.Text = "toolStripSplitButton1";
            this.eventWarrningButton.Click += eventWarrningButton_ButtonClick;
            setEventWarrningButton(false);
            ToolStripItem item = new ToolStripMenuItem();
            item.Text = "Notifications";
            item.Click += (s1, em) =>
            {
                try
                {
                    CreateList(typeof(JNotification));
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            eventWarrningButton.DropDownItems.Add(item);


            this.notificationTimer = new System.Windows.Forms.Timer(this.components);
            this.notificationTimer.Interval = 3000;
            this.notificationTimer.Tick += new System.EventHandler(this.notificationTimer_Tick);

            AppManager.Instance.RegisterDocPanelContainer(this);

            Text = Text + ((DocPanelIndex > 0) ? (" " + DocPanelIndex) : "");//index exists after registration in AppManager

            this.Load += BaseMainAppForm_Load;
            this.FormClosing += BaseMainAppForm_FormClosing;

            JSetting s = FrwConfig.Instance.GetProperty(FrwSimpleWinCRUDConfig.APPLICATION_FONT);
            
            if (s != null)
            {
                if (s.Value == null) s.Value = this.Font;
                s.ValueChanged += FontSetting_ValueChanged;
            }

            //create panes
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
        }

  

        private void BaseMainAppForm_Load(object sender, EventArgs e)
        {
            LoadConfig();
            AppManager.Instance.ProcessViewForAllCreatedDocContents(this);
            AppManager.Instance.OnDocContentRegistredEvent += AppManager_OnDocContentRegistredEvent;
            AppManager.Instance.OnDocContentShowEvent += AppManager_OnDocContentShowEvent;
            AppManager.Instance.NotificationEvent += Instance_NotificationEvent;
            Log.EventLogEvent += Log_EventLogEvent;

            notificationTimer.Enabled = true;

        }

        private void setEventWarrningButton(bool events, string tooltips = null)
        {
            if (events)
            {
                this.eventWarrningButton.ToolTipText = tooltips != null ? tooltips : FrwCRUDRes.There_are_new_notifications_and_events;
                this.eventWarrningButton.Image = FrwSoftware.Properties.Resources.if_info_b_86249;
            }
            else
            {
                this.eventWarrningButton.Image = FrwSoftware.Properties.Resources.if_info_172483;
                eventWarrningButton.ToolTipText = FrwCRUDRes.Notifications_and_events;
            }
        }

        private void eventWarrningButton_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                setEventWarrningButton(false);
                /*
                //eventWarrningButton.DropDownItems.Clear();//- очистка приводит к тому, что меню появляется по 0.0 координатам 
                ToolStripItem sep = new ToolStripSeparator();
                eventWarrningButton.DropDownItems.Add(sep);
                List<ToolStripItem> tmp = new List<ToolStripItem>();
                foreach (ToolStripItem c in eventWarrningButton.DropDownItems)
                {
                    if (c != sep) tmp.Add(c);
                }
                foreach (var c in tmp)
                {
                    eventWarrningButton.DropDownItems.Remove(c);
                }
                ToolStripItem item = new ToolStripMenuItem();
                item.Text = "Notifications";
                item.Click += (s, em) =>
                {
                    try
                    {
                        CreateList(typeof(JNotification));
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                eventWarrningButton.DropDownItems.Add(item);
                */


            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void notificationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ProcessNotificationTimer();
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }

        }
        virtual protected void ProcessNotificationTimer()
        {
            ProcessNotificationQueueCheck();
        }
        protected void ProcessNotificationQueueCheck()
        {
            try
            {
                if (localNotificationQueue.Count > 0)
                {
                    bool first = true;
                    while (localNotificationQueue.Count > 0)
                    {
                        string notif = localNotificationQueue.Dequeue();
                        localNotificationLog.Error(notif);
                        if (first)
                        {
                            setEventWarrningButton(true, notif);
                            first = false;
                        }
                    }
                    //localNotificationQueue.Clear();//todo
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
            }

        }
        private void Log_EventLogEvent(object sender, EventLogEventArgs e)
        {
            localNotificationQueue.Enqueue(e.Message);
        }

        private void Instance_NotificationEvent(object sender, NotificationEventArgs e)
        {
            localNotificationQueue.Enqueue(e.Message);
        }


        private void FontSetting_ValueChanged(object sender, JSettingChangedEventArgs e)
        {
            WinFormsUtils.SetNewControlFont(this, (Font)e.Setting.Value);
        }
        private void BaseMainAppForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SaveConfig();
                if (e.CloseReason == CloseReason.UserClosing)
                {

                }

                //close 
                List<IDockContent> contents = new List<IDockContent>();
                foreach (IDockContent document in dockPanel.Contents)//copy to new collection to prevent exception 
                {
                    if (document.DockHandler.DockState != DockState.Document)
                    {
                        contents.Add(document);
                    }
                }
                foreach (IDockContent content in contents)
                {
                    if (content is Form) (content as Form).Close();
                    //CloseContent(content);
                }

                if (!(BaseApplicationContext.IsContextMode == false && AppManager.Instance.GetMainContainer() == this))//do not unregister main form in notray app
                    AppManager.Instance.UnRegisterDocPanelContainer(this);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        private void Content_FormClosed(object sender, FormClosedEventArgs e)
        {
            //!! this event does not occurs for HideOnClose windows because it not closed 
            if (sender is FrwDocContent)
            {
                AppManager.Instance.RemoveDocContent(((FrwDocContent)sender).ContentControl);
                ((FrwDocContent)sender).DockHandler.DockPanel = null;
            }
        }

        public void CloseDocPanelContainer()
        {
            this.Close();
        }

        public virtual void ActivateNotificationPanel()
        {

        }

        protected void AppManager_OnDocContentRegistredEvent(object sender, DocContentRegistredEventArgs e)
        {
            if (e.Content.ContentContainer == null) throw new Exception("");
            if (e.Content.ContentContainer == this)
            {
                FrwDocContent cc = FindJustDocContentByIContent(e.Content);
                if (cc == null)
                {
                    cc = CreateDocContentByIContent(e.Content);

                }
                cc.Show(dockPanel);//, DockState.Document);
            }
        }
        protected void AppManager_OnDocContentShowEvent(object sender, DocContentShowEventArgs e)
        {
            if (e.Content.ContentContainer == null) throw new Exception("");
            if (e.Content.ContentContainer == this)
            {
                FrwDocContent cc = FindJustDocContentByIContent(e.Content);
                if (cc != null)
                {
                    cc.Show(dockPanel);//, DockState.Document);
                }
            }
        }


        private FrwDocContent CreateDocContentByIContent(IContent c)
        {
            FrwDocContent cc = new FrwDocContent(c);
            c.ContentContainer = this;
            cc.FormClosed += Content_FormClosed;
            OnDocContentCreated(cc.ContentControl);
            return cc;
        }
        //Used to establish additional links between the window and the main window
        protected virtual void OnDocContentCreated(IContent contentControl)
        {

        }

        private FrwDocContent FindJustDocContentByIContent(IContent c)
        {
            FrwDocContent cc = null;
            foreach (var v in dockPanel.Contents)
            {
                if (v is FrwDocContent && (v as FrwDocContent).ContentControl == c)
                {
                    cc = (v as FrwDocContent);
                }
            }
            return cc;
        }


   


        protected IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == null) return null;
            string[] parsedStrings = persistString.Split(new string[] { FrwBaseViewControl.PersistStringSeparator }, StringSplitOptions.None);
            Dictionary<string, object> pars = new Dictionary<string, object>();
            foreach (var kv in parsedStrings)
            {
                string[] kvp = kv.Split(new string[] { FrwBaseViewControl.PersistStringSeparatorKeyValue }, StringSplitOptions.None);
                if (kvp.Length >= 2)
                {
                    pars.Add(kvp[0], kvp[1]);
                }
                else
                {
                    throw new InvalidOperationException("Pair string not parsed as key and value, String: " + kv);
                }
            }
            try
            {
                IContent c = AppManager.Instance.CreateStoredContent(this, pars);
                return CreateDocContentByIContent(c);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
                return null;
            }
        }

     

   
        protected void CloseAllContents(bool doNotCloseActiveDoc, bool closeOnlyDocuments)
        {
            IDockContent ad = dockPanel.ActiveDocument;//You must temporarily save the index, because After Close () ActiveDocument will change
            List<IDockContent> contents = new List<IDockContent>();
            foreach (IDockContent document in dockPanel.Contents)//copy to new collection to prevent exception 
            {
                if (!(doNotCloseActiveDoc && document == ad))
                {
                    if (closeOnlyDocuments)
                    {
                        if (document.DockHandler.DockState == DockState.Document)
                        {
                            contents.Add(document);
                        }
                    }
                    else contents.Add(document);
                }
            }
            foreach (IDockContent content in contents)
            {
                CloseContent(content);
            }
        }
        //You must close the hidden windows before saving the configuration
        protected void CloseAllHiddenContents()
        {
            List<IDockContent> contents = new List<IDockContent>();
            foreach (IDockContent document in dockPanel.Contents)//copy to new collection to prevent exception 
            {
                if (document.DockHandler.IsHidden == true)
                {
                    contents.Add(document);
                }
            }
            foreach (IDockContent content in contents)
            {
                CloseContent(content);
            }
        }
        protected void CloseActiveDocument()
        {
            IDockContent content = dockPanel.ActiveDocument;
            if (content != null)
            {
                CloseContent(content);
            }
        }

        protected void CloseContent(IDockContent content)
        {
            content.DockHandler.Close();
            if (content is FrwDocContent)
            {
                AppManager.Instance.RemoveDocContent((content as FrwDocContent).ContentControl);
                ((FrwDocContent)content).DockHandler.DockPanel = null;
            }
        }

        protected void CreateMainMenuItemsForAllEntityTypes(ToolStripMenuItem parentItem)
        {
            var entityTypes = AttrHelper.GetTypesWithAttribute<JEntity>(true);
            foreach (var t in entityTypes)
            {
                CreateMainMenuItemForEntityType(parentItem, t);
            }
        }

        protected ToolStripMenuItem CreateMainMenuItemForEntityType(ToolStripMenuItem parentItem, Type t)
        {
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Name = "menuItem_" + t.FullName;
            menuItem.Text = ModelHelper.GetEntityJDescriptionOrFullName(t);
            menuItem.Click += (s, em) =>
            {
                CreateList(t);
            };
            parentItem.DropDownItems.AddRange(new ToolStripItem[] {
                    menuItem});
            return menuItem;
        }
        protected ToolStripMenuItem CreateMainMenuItemForWindowType(ToolStripMenuItem parentItem, string name, Type t)
        {
            JEntity entityAttr = AttrHelper.GetClassAttribute<JEntity>(t);
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Name = "menuItem_" + t.FullName;
            menuItem.Text = name != null ? name : ModelHelper.GetEntityJDescriptionOrFullName(t);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    AppManager.Instance.CreateContentAndProcessView(this, t, null);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            parentItem.DropDownItems.AddRange(new ToolStripItem[] {
                    menuItem});
            return menuItem;
        }
        protected void CreateList(Type modelType)
        {
            try
            {
                AppManager.Instance.CreateAndProcessListContentForModelType(modelType, this);
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }
        protected void CreateHelpMenuItems(ToolStripMenuItem menuItemHelp, Form mainForm)
        {
            ToolStripMenuItem menuItem = null;
            menuItem = new ToolStripMenuItem(FrwCRUDRes.About);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    BaseAboutDialog aboutDialog = new BaseAboutDialog(this);
                    aboutDialog.ShowDialog(this);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemHelp.DropDownItems.Add(menuItem);

        }

        protected void CreateFileMenuItems(ToolStripMenuItem menuItemFile)
        {
            ToolStripMenuItem menuItem = null;
            ToolStripMenuItem groupItem = null;

            groupItem = new ToolStripMenuItem(FrwCRUDRes.Close);
            menuItemFile.DropDownItems.Add(groupItem);



            menuItem = new ToolStripMenuItem(FrwCRUDRes.ActiveDocumentWindow);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    CloseActiveDocument();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.AllDocumentsExpectActive);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    CloseAllContents(true, true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.AllDocuments);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    CloseAllContents(false, true);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.AllWindows);
            menuItem.Click += (s, em) =>
            {
                try
                {
                    CloseAllContents(false, false);
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            groupItem.DropDownItems.Add(menuItem);


            groupItem = new ToolStripMenuItem(FrwCRUDRes.WidgetSConfiguration);
            menuItemFile.DropDownItems.Add(groupItem);

            menuItem = new ToolStripMenuItem(FrwCRUDRes.LoadConfiguration);
            menuItem.Click += (s, em) =>
            {

                SimpleListDialog listDialog = new SimpleListDialog(typeof(JDocPanelLayout));
                DialogResult res = listDialog.ShowDialog();
                if (res == DialogResult.OK && listDialog.SelectedObjects != null && listDialog.SelectedObjects.Count > 0)
                {
                    IList layouts = listDialog.SelectedObjects;

                    foreach (var layout in layouts)
                    {
                        //JDocPanelLayout layout = Dm.Instance.FindAll<JDocPanelLayout>().FirstOrDefault<JDocPanelLayout>(c => c.JDocPanelLayoutId == "1");
                        AppManager.Instance.LoadLayout((JDocPanelLayout)layout);
                        break;
                    }
                }

            };
            groupItem.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.SaveConfiguration);
            menuItem.Click += (s, em) =>
            {
                JDocPanelLayout layout = Dm.Instance.EmptyObject<JDocPanelLayout>(null);
                SimpleTextDialog dlg = new SimpleTextDialog();
                if (dlg.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dlg.TextToEdit))
                {
                    layout.Name = dlg.TextToEdit;
                    AppManager.Instance.SaveLayout(layout);
                    Dm.Instance.SaveObject(layout);
                }
            };
            groupItem.DropDownItems.Add(menuItem);

            menuItem = new ToolStripMenuItem(FrwCRUDRes.UpdateCurrentConfiguration);
            menuItem.Click += (s, em) =>
            {
                AppManager.Instance.SaveLayout(AppManager.Instance.CurrentLayout);
            };
            groupItem.DropDownItems.Add(menuItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JDocPanelLayout));

            menuItem = new ToolStripMenuItem(FrwCRUDRes.BaseMainAppForm_OpenNewContainerWindow);
            menuItem.Click += (s, em) =>
            {
                Cursor cursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    BaseMainAppForm.CreateWindowNotClosable = false;
                    Form newWindow = (Form)Activator.CreateInstance(this.GetType());
                    newWindow.Show();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
                finally
                {
                    Cursor.Current = cursor;
                }
            };
            menuItemFile.DropDownItems.Add(menuItem);

            if (NotClosable && DocPanelIndex == 0)
            {
                menuItem = new ToolStripMenuItem(FrwCRUDRes.BaseMainAppForm_HideMainApplicationWindow);//
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        FrwConfig.Instance.SetPropertyValue(BaseApplicationContext.SETTING_showMainFormOnStartup, false);
                        Hide();
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemFile.DropDownItems.Add(menuItem);
            }
            else
            {
                menuItemFile.DropDownItems.Add(menuItem);
                menuItem = new ToolStripMenuItem(DocPanelIndex == 0 ? FrwCRUDRes.CloseApplication : FrwCRUDRes.BaseMainAppForm_CloseContainerWindow);
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        Close();
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemFile.DropDownItems.Add(menuItem);
            }
        }



        protected void CreateToolsMenuItems(ToolStripMenuItem menuItemTools, ToolStrip toolBar, StatusStrip statusBar)
        {
            ToolStripMenuItem menuItem = null;
            ////////////////////////////////////////
            ToolStripMenuItem groupItem = new ToolStripMenuItem(FrwCRUDRes.Jobs_and_notifications);
            menuItemTools.DropDownItems.Add(groupItem);
            CreateMainMenuItemForEntityType(groupItem, typeof(JJobType));
            CreateMainMenuItemForEntityType(groupItem, typeof(JRunningJob));
            CreateMainMenuItemForEntityType(groupItem, typeof(JNotification));
            CreateMainMenuItemForWindowType(menuItemTools, FrwCRUDRes.Application_settings, typeof(AppSettingsWindow));
        }

        protected void CreateViewMenuItems(ToolStripMenuItem menuItemView, ToolStrip toolBar, StatusStrip statusBar)
        {
            ToolStripMenuItem menuItem = null;
           

            if (toolBar != null)
            {
                menuItem = new ToolStripMenuItem(FrwCRUDRes.Toolbar);
                //toolBar.Visible = menuItem.Checked = FrwConfig.Instance.getPropValueAsBool(PropGroup + ".toolBar.Visible", true);
                menuItem.Checked = toolBar.Visible; // isToolBarVisible;
                menuItem.CheckState = menuItem.Checked ? CheckState.Checked : CheckState.Unchecked;
                menuItem.Click += (s, em) =>
                {

                    try
                    {
                        ToolStripMenuItem thisMenuItem = (ToolStripMenuItem)s;
                        toolBar.Visible =  thisMenuItem.Checked = !thisMenuItem.Checked;
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemView.DropDownItems.Add(menuItem);
            }
            if (statusBar != null)
            {
                menuItem = new ToolStripMenuItem(FrwCRUDRes.Statusbar);
                //statusBar.Visible = menuItem.Checked = FrwConfig.Instance.getPropValueAsBool(PropGroup + ".statusBar.Visible", true);
                menuItem.Checked = statusBar.Visible; // isStatusBarVisible;
                menuItem.CheckState = menuItem.Checked ? CheckState.Checked : CheckState.Unchecked;
                menuItem.Click += (s, em) =>
                {
                    try
                    {
                        ToolStripMenuItem thisMenuItem = (ToolStripMenuItem)s;
                        statusBar.Visible =  thisMenuItem.Checked = !thisMenuItem.Checked;
                    }
                    catch (Exception ex)
                    {
                        Log.ShowError(ex);
                    }
                };
                menuItemView.DropDownItems.Add(menuItem);
            }

            menuItemView.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.EnableFloating);
            menuItem.Checked = dockPanel.AllowEndUserDocking;
            menuItem.CheckState = menuItem.Checked ? CheckState.Checked : CheckState.Unchecked;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ToolStripMenuItem thisMenuItem = (ToolStripMenuItem)s;
                    dockPanel.AllowEndUserDocking = thisMenuItem.Checked = !thisMenuItem.Checked;
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemView.DropDownItems.Add(menuItem);

            menuItemView.DropDownItems.Add(menuItem);
            menuItem = new ToolStripMenuItem(FrwCRUDRes.ShowDocumentsIcons);
            menuItem.Checked = dockPanel.ShowDocumentIcon; // false;
            menuItem.CheckState = menuItem.Checked ? CheckState.Checked : CheckState.Unchecked;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ToolStripMenuItem thisMenuItem = (ToolStripMenuItem)s;
                    dockPanel.ShowDocumentIcon = thisMenuItem.Checked = !thisMenuItem.Checked;
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            menuItemView.DropDownItems.Add(menuItem);
        }
        #region saverestorestate

        public void LoadLayout(string xml)
        {
            dockPanel.SuspendLayout(true);

            // In order to load layout from XML, we need to close all the DockContents
            CloseAllContents(false, false);
            Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
            xmlStream.Close();
            AppManager.Instance.ProcessViewForAllCreatedDocContents(this);
            dockPanel.ResumeLayout(true, true);
        }
        public string SaveLayout()
        {
            CloseAllHiddenContents();
            MemoryStream xmlStream = new MemoryStream();
            dockPanel.SaveAsXml(xmlStream, Encoding.UTF8);
            return Encoding.UTF8.GetString(xmlStream.ToArray());
        }

        protected string GetStateConfigFileName()
        {
            string filename = Path.Combine(FrwConfig.Instance.ProfileConfigDir, "DockPanel" + DocPanelIndex + "_state.config");
            return filename;
        }
        protected string GetContentConfigFileName()
        {
            string configFile = Path.Combine(FrwConfig.Instance.ProfileConfigDir, "DockPanel" + DocPanelIndex + ".config");
            return configFile;
        }



        private string configFileStr = null;
        private string configLayoutStr = null;

        protected void LoadConfig()
        {
            /////////////////////////////////////////////////
            try
            {
                string filename = GetStateConfigFileName();
                if (File.Exists(filename))
                {
                    configFileStr = File.ReadAllText(filename, Encoding.UTF8);
                    DocPanelState state = JsonSerializeHelper.LoadFromString<DocPanelState>(configFileStr);
                    LoadUserSettings(state.UserSettings);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Can not read config for " + GetType().ToString(), ex);
            }


            string configFile = GetContentConfigFileName();

            if (File.Exists(configFile))
            {
                configLayoutStr = File.ReadAllText(configFile);
                Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(configLayoutStr));
                dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
                //xmlStream.Close();
            }
            try
            {
                // Font installation for all forms
                WinFormsUtils.SetNewControlFont(this, (Font)FrwConfig.Instance.GetPropertyValue(FrwSimpleWinCRUDConfig.APPLICATION_FONT));//if this.AutoScaleMode = Font - Setting the font will change the size of the window 
            }
            catch (Exception ex)
            {
                Log.LogError("Can not set font ", ex);
            }

        }
        protected void SaveConfig()
        {
            //todo - do not save if not modified
            CloseAllHiddenContents();
            FrwConfig.Instance.SetPropertyValue( FrwSimpleWinCRUDConfig.APPLICATION_FONT, Font);
            string configFile = GetContentConfigFileName();

            MemoryStream xmlStream = new MemoryStream();
            dockPanel.SaveAsXml(xmlStream, Encoding.UTF8);
            string newConfigLayoutStr = Encoding.UTF8.GetString(xmlStream.ToArray());
            if (newConfigLayoutStr.Equals(configLayoutStr) == false)
            {
                File.WriteAllText(configFile, newConfigLayoutStr);
                configLayoutStr = newConfigLayoutStr;
                //Log.ProcessDebug("@@@@@ Saved config for main window " + DocPanelIndex);
            }

            ////////////////////////////////////////////////
            try
            {
                string filename = GetStateConfigFileName();
                DocPanelState state = new DocPanelState();
                SaveUserSettings(state.UserSettings);

                string newConfigStr = JsonSerializeHelper.SaveToString(state);
                if (newConfigStr.Equals(configFileStr) == false)
                {
                    File.WriteAllText(filename, newConfigStr, Encoding.UTF8);
                    configFileStr = newConfigStr;
                    //Log.ProcessDebug("@@@@@ Saved json config for main window " + DocPanelIndex);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Can not write config for " + GetType().ToString(), ex);
            }
        }
        virtual protected void LoadUserSettings(IDictionary<string, object> userSettings)
        {
            dockPanel.ShowDocumentIcon = DictHelper.GetValueAsBool(userSettings, "ShowDocumentIcon");
            dockPanel.AllowEndUserDocking = DictHelper.GetValueAsBool(userSettings, "AllowEndUserDocking");

        }
        virtual protected void SaveUserSettings(IDictionary<string, object> userSettings)
        {
            userSettings.Add("ShowDocumentIcon", dockPanel.ShowDocumentIcon);
            userSettings.Add("AllowEndUserDocking", dockPanel.AllowEndUserDocking);
        }

        #endregion

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                if (BaseMainAppForm.CreateWindowNotClosable) {
                    NotClosable = true;
                    CreateParams myCp = base.CreateParams;
                    myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                    return myCp;
                }
                else return base.CreateParams; 
            }
        }
    }

    public class DocPanelState
    {
        private IDictionary<string, object> userSettings = new Dictionary<string, object>();
        public IDictionary<string, object> UserSettings { get { return userSettings; } set { userSettings = value; } }
    }
}
