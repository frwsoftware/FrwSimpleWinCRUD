using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public interface IContentContainer
    {
        int DocPanelIndex { get; set; }
        Rectangle DocPanelBounds { get; set; }
        void CloseDocPanelContainer();
        void ActivateNotificationPanel();
        void LoadLayout(string xml);
        string SaveLayout();
    }
}
