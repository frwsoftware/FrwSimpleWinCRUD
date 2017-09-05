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
using FrwSoftware;

namespace FrwSoftware
{
    public partial class SimpleJDictItemDialog : BaseDialogForm
    {
        public JDictItem DictItem { get;  }
        public SimpleJDictItemDialog(JDictItem dictItem)
        {
            InitializeComponent();
            this.Text = FrwCRUDRes.SimpleJDictItemDialog_Title;
            this.label3.Text = FrwCRUDRes.SimpleJDictItemDialog_Image;
            this.label2.Text = FrwCRUDRes.SimpleJDictItemDialog_Text;
            this.label1.Text = FrwCRUDRes.SimpleJDictItemDialog_Key;

            DictItem = DictItem;
            textTextBox.Text = DictItem.Text;
            keyTextBox.Text = DictItem.Key;
            //imageTextBox.Text = DictItem.Image;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DictItem.Text = textTextBox.Text;
            DictItem.Key = keyTextBox.Text;
            //DictItem.Image = imageTextBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
