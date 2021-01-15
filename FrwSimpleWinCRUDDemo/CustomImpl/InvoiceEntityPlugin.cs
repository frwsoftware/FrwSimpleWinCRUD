using FrwSoftware.Model.Chinook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    [JEntityPlugin(typeof(JCountry))]
    public class InvoiceEntityPlugin : IFormsEntityPlugin
    {
        public void MakeContextMenu(IListProcessor list, List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            menuItemList.Add(new ToolStripSeparator());

            //add additional items to list context menu
            Invoice item = (Invoice)selectedObject;
            ToolStripMenuItem menuItem = null;
            if (item.BillingPostalCode != null)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Text = "Send";
                menuItem.Click += (s, em) =>
                {
                    SendInvoive(item);
                };
                menuItemList.Add(menuItem);
            }
        }
        private void SendInvoive(Invoice invoice)
        {
            MessageBox.Show("Sent invoice with id: " + invoice.InvoiceId);
        }
    }
}
