using FrwSoftware.Properties;
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

using System;

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
            string defaultGroup = FrwCRUDRes.Common_settings;
            foreach (JSetting setting in FrwConfig.Instance.Settings)
            {
                if (setting.IsUser)
                {
                    bool isCustomEdit = false;
                    if (setting.IsCustomSetting())
                    {
                        isCustomEdit = true;
                    }
                    Type pType = null;
                    if (isCustomEdit) pType = typeof(string);//disabled comboboxes for list type fields 
                    else pType = setting.Value != null ? setting.Value.GetType() : typeof(string);

                    props = new PropertySpec(setting.Description, pType, setting.Group != null ? setting.Group : defaultGroup,
                        setting.Help);
                    props.PropTag = setting;
                    if (isCustomEdit)
                    {
                        props.EditorTypeName = typeof(CustomSettingEditor).ToString();
                    }

                    bag1.Properties.Add(props);
                }
            }
            ArrayList objs = new ArrayList();
            objs.Add(bag1);
            this.SelectedObjects = objs.ToArray();
            
        }

        private void bag1_GetValue(object sender, PropertySpecEventArgs e)
        {
            try
            {
                if (e.Property.PropTag != null && e.Property.PropTag is JSetting)
                {
                    JSetting setting = e.Property.PropTag as JSetting;
                    if (setting.IsCustomSetting())
                    {
                        e.Value = Dm.Instance.GetCustomSettingValue(setting, true, AppManager.Instance.PropertyWindowTruncatedMaxItemCount, AppManager.Instance.PropertyWindowTruncatedMaxLength);
                    }
                    else e.Value = setting.Value;

                }
            }
            catch (Exception ex)
            {
                Log.ShowError(ex);
            }
        }

        private void bag1_SetValue(object sender, PropertySpecEventArgs e)
        {
            try
            {
                if (e.Property.PropTag != null && e.Property.PropTag is JSetting)
                {
                    JSetting setting = e.Property.PropTag as JSetting;
                    if (setting.IsCustomSetting())
                    {
                    }
                    else
                    {
                        setting.Value = e.Value;
                        setting.ForceValueChangedEvent(this);
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
