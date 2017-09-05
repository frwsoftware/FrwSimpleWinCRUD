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
        private static string APPLICATION_FONT = "ApplicationFont";

        override protected void CreateProperties()
        {
            base.CreateProperties();
            JSetting s = null;
            s = new JSetting()
            {
                Name = "SetProcessDPIAware",
                Description = "DPI Awareness Support for Windows Fonts (>= Windows 8). Application restart required.",
                IsUser = true,
                IsAttachedToComputer = true,
                Value = true
            };
            s.ValueChanged += SetProcessDPIAware_ValueChanged;
            FrwConfig.Instance.SetProperty(s);
            //special storage, 
            //we need this prorerty at the start ogf program 
            FrwConfig.Instance.SetPropertyValue(s.Name, Properties.Settings.Default.SetProcessDPIAware);


            s = new JSetting()
            {
                Name = APPLICATION_FONT,
                Description = "Application Font",
                IsUser = true,
                IsAttachedToComputer = true,
            };
            FrwConfig.Instance.SetProperty(s);

        }

        static public JSetting GetApplicationFontProperty()
        {
            return Instance.GetProperty(APPLICATION_FONT);
        }
        static public Font GetApplicationFont()
        {
            return (Font)Instance.GetProperty(APPLICATION_FONT).Value;
        }
        static public void SetApplicationFont(Font font)
        {
            Instance.GetProperty(APPLICATION_FONT).Value = font;
        }

        private static void SetProcessDPIAware_ValueChanged(object sender, JSettingChangedEventArgs e)
        {
            Properties.Settings.Default.SetProcessDPIAware = (bool)e.Setting.Value;
            Properties.Settings.Default.Save();

        }
    }
}
