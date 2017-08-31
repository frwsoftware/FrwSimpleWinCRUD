using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using FrwSoftware.Model.Chinook;

namespace FrwSoftware
{
    public partial class InvoiceListWindow : SimpleListWindow
    {
        private ToolStripComboBox filterComboBox = null;

        public InvoiceListWindow()
        {
            InitializeComponent();
            filterComboBox = new ToolStripComboBox();
            filterComboBox.Items.AddRange(new object[] {
                new  CustomItem("all", "All"),
                new  CustomItem("currentDay", "Current day"),
                new  CustomItem("lastWeek", "Last week"),
                new  CustomItem("lastMonth", "Last month"),
                new  CustomItem("2009", "Archive 2009"),
                new  CustomItem("2010", "Archive 2010")
            });
            filterComboBox.Name = "dateComboBox";
            filterComboBox.Size = new Size(121, 38);
            filterComboBox.SelectedIndexChanged += (s, em) =>
            {
                listView.UpdateColumnFiltering();
            };
            AddToolStripItem(filterComboBox);
        }

        override protected void MakeListColumns()
        {
            base.MakeListColumns();

            //make button column
            OLVColumn column = MakeButtonColumn("SendInvoice", "Send Invoice");
            column.AspectGetter = delegate (Object rowObject)
            {
                if (((Invoice)rowObject).BillingPostalCode != null)  return "Send";
                else return null;
            };
            AddColumnToList(column);

            //handler for all column buttons
            listView.ButtonClick += (s, em) =>
            {
                Invoice item = em.Model as Invoice;
                if (em.Column.Name == "SendInvoice") SendInvoive(item);
            };

            //make additional filter (combobox on list toolbar)
            listView.AdditionalFilter = new ModelFilter(delegate (object x)
            {
                CustomItem item = filterComboBox.SelectedItem as CustomItem;
                if (item != null)
                {
                    Invoice invoice = (Invoice)x;
                    if ("currentDay".Equals(item.Key))
                    {
                        if (invoice.InvoiceDate != null && invoice.InvoiceDate.Date >= DateTime.Now.Date)
                            return true;
                        else return false;
                    }
                    else if ("lastWeek".Equals(item.Key))
                    {
                        if (invoice.InvoiceDate != null && 
                            invoice.InvoiceDate.Date >= DateTime.Now.Date.AddDays(-7))
                            return true;
                        else return false;
                    }
                    else if ("lastMonth".Equals(item.Key))
                    {
                        if (invoice.InvoiceDate != null &&
                            invoice.InvoiceDate.Date >= DateTime.Now.Date.AddMonths(-1))
                            return true;
                        else return false;
                    }
                    else if ("2009".Equals(item.Key))
                    {
                        if (invoice.InvoiceDate != null &&
                            invoice.InvoiceDate.Date.Year == 2009)
                            return true;
                        else return false;
                    }
                    else if ("2010".Equals(item.Key))
                    {
                        if (invoice.InvoiceDate != null &&
                            invoice.InvoiceDate.Date.Year == 2010)
                            return true;
                        else return false;
                    }
                }
                return true;
            });

        }
        override protected void MakeContextMenu(List<ToolStripItem> menuItemList, object selectedListItem, object selectedObject, string aspectName)
        {
            base.MakeContextMenu(menuItemList, selectedListItem, selectedObject, aspectName);
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
