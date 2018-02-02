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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class FrwSimpleWinCRUDConfig : FrwConfig
    {
        public static string APPLICATION_FONT = "ApplicationFont";

        override protected void CreateProperties()
        {
            base.CreateProperties();
            JSetting setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = "SetProcessDPIAware",
                Description = FrwCRUDRes.DPI_Awareness_Support_for_Windows_Fonts,
                Help = FrwCRUDRes.DPI_Awareness_Support_for_Windows_Fonts_____Windows_8___Application_restart_required_,
                Value = true,
                IsUser = true,
                IsAttachedToComputer = true
            });
            setting.ValueChanged += SetProcessDPIAware_ValueChanged;
            //special storage, 
            //we need this prorerty at the start ogf program 
            FrwConfig.Instance.SetPropertyValue(setting.Name, Properties.Settings.Default.SetProcessDPIAware);

            setting = FrwConfig.Instance.CreatePropertyIfNotExist(new JSetting()
            {
                Name = APPLICATION_FONT,
                Description = FrwCRUDRes.Application_Font,
                ValueType = typeof(Font),
                IsUser = true,
                IsAttachedToComputer = true,
            });

        }


        private static void SetProcessDPIAware_ValueChanged(object sender, JSettingChangedEventArgs e)
        {
            Properties.Settings.Default.SetProcessDPIAware = (bool)e.Setting.Value;
            Properties.Settings.Default.Save();

        }
    }
}
