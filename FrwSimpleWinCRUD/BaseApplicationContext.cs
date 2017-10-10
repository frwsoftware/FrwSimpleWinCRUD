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
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using FrwSoftware;
using System.Collections.Generic;

/// <summary>
/// idea from  https://www.simple-talk.com/dotnet/.net-framework/creating-tray-applications-in-.net-a-practical-guide/ 
/// </summary>
namespace FrwSoftware
{
    /// <summary>
    /// Framework for running application as a tray app.
    /// </summary>
    /// <remarks>
    /// Tray app code adapted from "Creating Applications with NotifyIcon in Windows Forms", Jessica Fosler,
    /// http://windowsclient.net/articles/notifyiconapplications.aspx
    /// </remarks>
    public class BaseApplicationContext : ApplicationContext
    {
        public string IconFileName { get; set; }
        public string DefaultTooltip  { get; set; }
        protected static  int BalloonTimeout = 3000; // preferred timeout (msecs) though .NET enforces 10-sec minimum
        protected static  int MaxTooltipLength = 63; // framework constraint
        protected Timer notificationTimer = new Timer();
        protected Queue<string> localNotificationQueue = new Queue<string>();

        //
        protected System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        protected NotifyIcon notifyIcon;                            // the icon that sits in the system tray

        /// <summary>
        /// This class should be created and passed into Application.Run( ... )
        /// </summary>
        public BaseApplicationContext()
        {
        }

        public void Load()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            //notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

            JSetting setting = null;
            setting = new JSetting()
            {
                Name = "MainApp.showMainFormOnStartup",
                Description = FrwCRUDRes.BaseApplicationContext_Show_Main_Window_OnStartup,
                IsUser = true,
                Value = true
            };
            FrwConfig.Instance.SetProperty(setting);
            if (FrwConfig.Instance.GetPropertyValueAsBool("MainApp.showMainFormOnStartup", true))
            {
                ShowDetailsForm();
            }
            //timer for balloon tooltips and change systray icon
            notificationTimer.Interval = 3000;
            notificationTimer.Tick += NotificationTimer_Tick;
            notificationTimer.Enabled = true;
            Log.EventLogEvent += Log_EventLogEvent;
            AppManager.Instance.NotificationEvent += Instance_NotificationEvent;

        }


        private void Instance_NotificationEvent(object sender, NotificationEventArgs e)
        {
            localNotificationQueue.Enqueue(e.Message);
        }

        private void Log_EventLogEvent(object sender, EventLogEventArgs e)
        {
            localNotificationQueue.Enqueue(e.Message);
        }


        private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            if (localNotificationQueue.Count > 0)
            {
                string notif = localNotificationQueue.Dequeue();
                localNotificationQueue.Clear();//todo
                ShowBalloonTip(notif);
            }

   
        }

        private void ShowBalloonTip(string text)
        {
            notifyIcon.ShowBalloonTip(BalloonTimeout, FrwCRUDRes.BaseApplicationContext_Notification, text, ToolTipIcon.Info);
        }
        private void SetNotifyIconToolTip(string toolTipText)
        {
            notifyIcon.Text = toolTipText.Length >= MaxTooltipLength ?
                toolTipText.Substring(0, MaxTooltipLength - 3) + "..." : toolTipText;
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            notifyIcon.ContextMenuStrip.Items.Clear();
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.BaseApplicationContext_ShowMainWindow;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    ShowDetailsForm();
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            notifyIcon.ContextMenuStrip.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = FrwCRUDRes.BaseApplicationContext_Exit;
            menuItem.Click += (s, em) =>
            {
                try
                {
                    long tstart = DateTime.Now.Ticks;
                    ExitThread();
                    long tend = DateTime.Now.Ticks;
                    Log.ProcessDebug("Application tread anded whith time:  " + (tend - tstart) / 10000 + " ms. ");
                }
                catch (Exception ex)
                {
                    Log.ShowError(ex);
                }
            };
            notifyIcon.ContextMenuStrip.Items.Add(menuItem);
        }
        

        private void ShowDetailsForm()
        {
            if (AppManager.Instance.GetMainContainer() == null)
            {
                AppManager.Instance.LoadDocPanelContainersState();
            }
            else
            {
                AppManager.Instance.ActivateDocPanelContainers();
            }
        }

        // From http://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }
        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            ShowDetailsForm();
            AppManager.Instance.ActivateNotificationPanel();
        }

        /// <summary>
        /// When the application context is disposed, dispose things like the notify icon.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            AppManager.Instance.SaveAndClose(null);
            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }

    }
}
