using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FrwSoftware
{
    public partial class NotifAndErrorConsoleWindow : ConsoleAdvancedWindow, IViewProcessor
    {
        public NotifAndErrorConsoleWindow()
        {
            InitializeComponent();
        }
        #region IViewProcessor
        virtual public void CreateView()
        {
        }
        public void ProcessView()
        {
            SetNewCaption(FrwCRUDRes.Error_and_notification_console);
        }
        #endregion

    }
}
