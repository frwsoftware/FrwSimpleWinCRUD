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
using System.Drawing;
using System.Windows.Forms;
using Flobbster.Windows.Forms;
using System.Collections;
using FrwSoftware;

namespace FrwSoftware
{
    public partial class AppSettingsPropertyGrid : PropertyGrid
    {
        PropertyBag bag1 = null;
        public AppSettingsPropertyGrid()
        {
            InitializeComponent();
        }
        public void SetAppSettings()
        {
            bag1 = new PropertyBag();
            bag1.GetValue += new PropertySpecEventHandler(this.bag1_GetValue);
            bag1.SetValue += new PropertySpecEventHandler(this.bag1_SetValue);
            PropertySpec props = null;
            string group = "Settings";
            foreach (JSetting itemdata in FrwConfig.Instance.Settings)
            {
                if (itemdata.IsUser)
                {
                    props = new PropertySpec(itemdata.Name, itemdata.Value != null ? itemdata.Value.GetType() : typeof(string), group,
                        itemdata.Description);
                    bag1.Properties.Add(props);
                }
            }
            ArrayList objs = new ArrayList();
            objs.Add(bag1);
            this.SelectedObjects = objs.ToArray();
            
        }

        private void bag1_GetValue(object sender, PropertySpecEventArgs e)
        {
            bool found = false;
            foreach (JSetting s in FrwConfig.Instance.Settings)
            {
                if (e.Property.Name.Equals(s.Name))
                {
                    e.Value = s.Value;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                e.Value = "";
            }
           
        }

        private void bag1_SetValue(object sender, PropertySpecEventArgs e)
        {
            foreach (JSetting s in FrwConfig.Instance.Settings)
            {
                if (e.Property.Name.Equals(s.Name))
                {

                    s.Value = e.Value;
                    s.ForceValueChangedEvent(this);
                    break;
                }
            }
     
        }
    }
}
