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
    public partial class ApplicationConsoleWindow : ConsoleAdvancedWindow
    {
        public ApplicationConsoleWindow()
        {
            InitializeComponent();
            SetCatchConsoleMode();
        }
        
    }
}
