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
using System.Windows.Forms;

namespace FrwSoftware
{
    public class WinFormsUtils
    {
        static public  void SetNewControlFont(Control control, Font newFont)
        {
            if (newFont != null && newFont.Equals(control.Font) == false)
            {
                List<Control> allControls = new List<Control>();
                GetAllChildControls(control, allControls);
                allControls.ForEach(k => k.Font = newFont);
            }
        }

        static public void GetAllChildControls(Control c, List<Control> list)
        {
            if (list.Contains(c) == false)
                list.Add(c);
            if (c.Controls.Count > 0)
            {
                foreach (Control chid in c.Controls)
                {
                    GetAllChildControls(chid, list);
                }
            }

        }

    }
}
