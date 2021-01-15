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
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace FrwSoftware
{
    [JListViewForEntityPlugin(typeof(JCountry))]
    public partial class DemoListWindowPluginForJCountry : SimpleListWindow
    {
        private ToolStripComboBox filterComboBox = null;

        public DemoListWindowPluginForJCountry()
        {
            InitializeComponent();
            filterComboBox = new ToolStripComboBox();
            filterComboBox.Items.AddRange(new object[] {
                new  CustomItem("all", "All"),
                new  CustomItem("independedOnly", "Independed only"),
            });
            filterComboBox.Name = "independedComboBox";
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
            OLVColumn column = MakeButtonColumn("ShowCapital", "Show Capital");
            column.AspectGetter = delegate (Object rowObject)
            {
                if (((JCountry)rowObject).Capital != null)  return "Show";
                else return null;
            };
            AddColumnToList(column);

            //handler for all column buttons
            listView.ButtonClick += (s, em) =>
            {
                JCountry item = em.Model as JCountry;
                if (em.Column.Name == "ShowCapital")
                {
                    MessageBox.Show(item.Capital);
                }
            };

            //make additional filter (combobox on list toolbar)
            listView.AdditionalFilter = new ModelFilter(delegate (object x)
            {
                CustomItem item = filterComboBox.SelectedItem as CustomItem;
                if (item != null)
                {
                    JCountry invoice = (JCountry)x;
                    if ("independedOnly".Equals(item.Key))
                    {
                        if ("Yes".Equals(invoice.Is_independent))
                            return true;
                        else return false;
                    }
                }
                return true;
            });

        }

    }
}
