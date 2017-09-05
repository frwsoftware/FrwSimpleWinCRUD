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

namespace FrwSoftware
{
    public partial class SimpleDateTimeDialog : BaseDialogForm
    {
        public DateTime Date
        {
            get
            {
                return dateTimePicker1.Value;
            }
            set
            {
                if (value != null && value != DateTime.MinValue)
                {
                    isDateTimeNull = false;
                    dateTimePicker1.Value = value;
                    this.monthCalendar1.SetDate(value);
                }
                else
                {
                    isDateTimeNull = true;
                }
            }
        }
        private bool isDateTimeNull = false;
        public bool IsDateTimeNull
        {
            get
            {
                return isDateTimeNull;
            }
            set
            {
                isDateTimeNull = value;
            }
        }

        public bool IsDateTimeNullable
        {
            get
            {
                return isDateTimeNullable;
            }

            set
            {
                isDateTimeNullable = value;
                nullButton.Visible = value;
            }
        }

        private bool isDateTimeNullable = true;

        public SimpleDateTimeDialog()
        {
            MonthCalendar calendar = new MonthCalendar();
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleDateTimeDialog_Date_And_Time;
            this.monthCalendar1.MaxSelectionCount = 1;
            //dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";

            //dateTimePicker1.Format = DateTimePickerFormat.Time;
            //dateTimePicker1.ShowUpDown = true;
            //dateTimePicker1.ShowCheckBox = true;

        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            dateTimePicker1.Value = new DateTime(monthCalendar1.SelectionStart.Year, monthCalendar1.SelectionStart.Month, monthCalendar1.SelectionStart.Day, dateTimePicker1.Value.Hour, dateTimePicker1.Value.Minute, dateTimePicker1.Value.Second, dateTimePicker1.Value.Millisecond) ;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            isDateTimeNull = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButtion_Click(object sender, EventArgs e)
        {

        }

        private void clearTimeButton_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, 0, 0, 0, 0);
        }

        private void nullButton_Click(object sender, EventArgs e)
        {
            isDateTimeNull = true;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
