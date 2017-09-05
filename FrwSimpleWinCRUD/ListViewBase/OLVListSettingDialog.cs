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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace FrwSoftware
{
    public partial class OLVListSettingDialog : BaseDialogForm
    {
        ObjectListView listView = null;
        BaseOLVListWindow listWindow = null; 
        public OLVListSettingDialog(ObjectListView listView, BaseOLVListWindow listWindow)
        {
            InitializeComponent();

            this.labelViewType.Text = FrwCRUDRes.ListSettingDialogcs_View;
            this.labelEditable.Text = FrwCRUDRes.ListSettingDialogcs_Editable;
            this.labelHotItemStyle.Text = FrwCRUDRes.ListSettingDialogcs_HotItem;
            this.labelRowHeight.Text = FrwCRUDRes.ListSettingDialogcs_RowHeight;
            this.Text = FrwCRUDRes.ListSettingDialogcs_ListSettings;



            this.listView = listView;
            this.listWindow = listWindow;
            //
            checkBoxGroups.Checked = listView.ShowGroups;
            //
            if (listView.RowHeight == -1)
            {
                rowHeightUpDown.Value = listView.RowHeightEffective;
                rowHeightUpDown.Enabled = false;
                autoCalcHeightCheckBox.Checked = true;
            }
            else
            {
                rowHeightUpDown.Enabled = true;
                autoCalcHeightCheckBox.Checked = false;
                rowHeightUpDown.Value = listView.RowHeight;
                rowHeightUpDown.ValueChanged += rowHeightUpDown_ValueChanged;
            }

            //
            string comboText = null;
            if (listView.CellEditActivation == ObjectListView.CellEditActivateMode.None)
                comboText = FrwCRUDRes.ListSettingDialogcs_No;
            else if (listView.CellEditActivation == ObjectListView.CellEditActivateMode.SingleClick)
                comboText = FrwCRUDRes.ListSettingDialogcs_SingleClick;
            else if (listView.CellEditActivation == ObjectListView.CellEditActivateMode.DoubleClick)
                comboText = FrwCRUDRes.ListSettingDialogcs_DoubleClick;
            else if (listView.CellEditActivation == ObjectListView.CellEditActivateMode.F2Only)
                comboText = FrwCRUDRes.ListSettingDialogcs_F2Only;
            int i = comboBoxEditable.Items.IndexOf(comboText);
            if (i != -1) comboBoxEditable.SelectedIndex = i;
            //
            comboBoxHotItemStyle.SelectedIndex = (int)listWindow.HotItemStyle;
            //
            if (listView.View == View.SmallIcon) comboBoxView.SelectedIndex = 0;
            else if (listView.View == View.LargeIcon) comboBoxView.SelectedIndex = 1;
            else if ( listView.View == View.List) comboBoxView.SelectedIndex = 2;
            else if (listView.View == View.Tile) comboBoxView.SelectedIndex = 3;
            else if (listView.View == View.Details) comboBoxView.SelectedIndex = 4;
        }

        private void checkBoxGroups_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked && listView.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show(FrwCRUDRes.ListSettingDialogcs_ListViewSCannotShowGroupsWhenInListView);
            }
            else
            {
                listView.ShowGroups = cb.Checked;
                listView.BuildList();
            }
        }

        private void comboBoxEditable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.Text == FrwCRUDRes.ListSettingDialogcs_No)
                listView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
            else if (comboBox.Text == FrwCRUDRes.ListSettingDialogcs_SingleClick)
                listView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;
            else if (comboBox.Text == FrwCRUDRes.ListSettingDialogcs_DoubleClick)
                listView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            else
                listView.CellEditActivation = ObjectListView.CellEditActivateMode.F2Only;
        }

        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            // Handle restrictions on Tile view
            if (comboBox.SelectedIndex == 3)
            {
                if (listView.VirtualMode)
                {
                    MessageBox.Show(FrwCRUDRes.ListSettingDialogcs_SorryMicrosoftSaysThatVirtualListsCanTUseTileView);
                    return;
                }
                if (listView.CheckBoxes)
                {
                    MessageBox.Show(FrwCRUDRes.ListSettingDialogcs_MicrosoftSaysThatTileViewCanTHaveCheckboxesSoCheckBoxesHaveBeenTurnedOffOnThisList);
                    listView.CheckBoxes = false;
                }
            }

            switch (comboBox.SelectedIndex)
            {
                case 0:
                    listView.View = View.SmallIcon;
                    break;
                case 1:
                    listView.View = View.LargeIcon;
                    break;
                case 2:
                    listView.View = View.List;
                    break;
                case 3:
                    listView.View = View.Tile;
                    break;
                case 4:
                    listView.View = View.Details;
                    break;
            }


            // Make the hot item show an overlay when it changes
            if (this.listView.UseTranslucentHotItem)
            {
                //this.listView.HotItemStyle.Overlay = new BusinessCardOverlay();
                this.listView.HotItemStyle = this.listView.HotItemStyle;
            }

            this.listView.UseTranslucentSelection = this.listView.UseTranslucentHotItem;

            this.listView.Invalidate();
        }

        private void comboBoxHotItemStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            listWindow.HotItemStyle = (OLVHotItemStyle)cb.SelectedIndex;
            OLVHelper.SetHotItemStyle(listView, (OLVHotItemStyle)cb.SelectedIndex);
        }

        private void rowHeightUpDown_ValueChanged(object sender, EventArgs e)
        {
            listView.RowHeight = Convert.ToInt32(rowHeightUpDown.Value);
        }

        private void autoCalcHeightCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                rowHeightUpDown.ValueChanged -= rowHeightUpDown_ValueChanged;
                listView.RowHeight = -1;
                listView.RowHeight = listView.RowHeightEffective; //xak
                rowHeightUpDown.Value = listView.RowHeightEffective;
                listView.RowHeight = -1;
                rowHeightUpDown.Enabled = false;
            }
            else
            {
                rowHeightUpDown.Enabled = true;
                rowHeightUpDown.Value = listView.RowHeightEffective;
                rowHeightUpDown.ValueChanged += rowHeightUpDown_ValueChanged;
            }
        }
    }
}
