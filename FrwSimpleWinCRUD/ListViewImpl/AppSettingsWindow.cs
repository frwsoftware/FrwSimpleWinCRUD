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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrwSoftware;

namespace FrwSoftware
{
    public partial class AppSettingsWindow : FrwBaseViewControl
    {
        public AppSettingsWindow()
        {
            InitializeComponent();
           
            Text = FrwCRUDRes.AppSettingsWindow_Title;
        }
        override public void SaveConfig()
        {
            Log.ProcessDebug("@@@@@ Saved config for AppSettingsWindow  ");

        }

        public void setObjects(object o)
        {
            this.appSettingsPropertyGrid1.SelectedObjects = new object[] { o};
        }

        private void AppSettingsWindow_Load(object sender, EventArgs e)
        {
            this.appSettingsPropertyGrid1.SetAppSettings();
        }

    }
}
