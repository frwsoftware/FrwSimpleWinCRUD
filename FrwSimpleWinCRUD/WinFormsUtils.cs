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
