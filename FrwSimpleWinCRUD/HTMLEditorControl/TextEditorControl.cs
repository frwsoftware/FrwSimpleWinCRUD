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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrwSoftware
{
    public partial class TextEditorControl : UserControl
    {
        private bool isHtml = false;

        public string EditedText {
            get
            {
                if (isHtml)
                    return this.htmlEditorControl.InnerHtml;
                else
                    return this.simpleTextBox.Text;
            }
            set
            {
                if (value != null && HtmlUtils.CheckIsHtmlFull(value))
                {
                    this.htmlEditorControl.Visible = true;
                    this.simpleTextBox.Visible = false;
                    isHtml = true;
                    this.htmlEditorControl.InnerHtml = value;
                    this.buttonHTML.Checked = true;
                }
                else
                {
                    this.htmlEditorControl.Visible = false;
                    this.simpleTextBox.Visible = true;
                    isHtml = false;
                    this.simpleTextBox.Text = value;
                    this.buttonHTML.Checked = false;
                }
            }
        }

        public TextEditorControl()
        {
            InitializeComponent();
        }

        private void TextEditorControl_Load(object sender, EventArgs e)
        {

        }

        private void buttonHTML_Click(object sender, EventArgs e)
        {
            if (this.buttonHTML.Checked)
            {
                if (string.IsNullOrWhiteSpace(EditedText) == false)
                {
                    DialogResult res = DialogResult.Yes;
                    res = MessageBox.Show(null, FrwCRUDRes.Press__Yes__to_convert_plain_text_to_html_and_edit_it_with_html_editor_or_press__No__to_still_edit_plain_text,
                              FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.Yes)
                    {
                        this.htmlEditorControl.InnerHtml = HtmlUtils.CreateHtmlBRTextFromText(this.simpleTextBox.Text);
                        this.htmlEditorControl.Visible = true;
                        this.simpleTextBox.Visible = false;
                        isHtml = true;
                    }
                    else
                    {
                        this.buttonHTML.Checked = false;
                    }
                }
                else
                {
                    this.htmlEditorControl.Visible = true;
                    this.simpleTextBox.Visible = false;
                    isHtml = true;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(EditedText) == false) {
                    if (HtmlUtils.CheckIsHtmlFull(EditedText)) {
                        DialogResult res = DialogResult.Yes;
                        res = MessageBox.Show(null, FrwCRUDRes.Press__Yes__to_convert_html_text_to_plain_text_and_edit_it_with_plain_text_editor__all_HTML_formating_will_be_lost__or_press__No__to_still_edit_html_text,
                                  FrwCRUDRes.WARNING, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (res == DialogResult.Yes)
                        {
                            this.simpleTextBox.Text = HtmlUtils.ConvertHtmlToPlainTextRegexp(this.htmlEditorControl.InnerHtml);
                            this.htmlEditorControl.Visible = false;
                            this.simpleTextBox.Visible = true;
                            isHtml = false;
                        }
                        else
                        {
                            this.buttonHTML.Checked = true;
                        }
                    }
                    else
                    {
                        this.simpleTextBox.Text = this.htmlEditorControl.InnerHtml;
                        this.htmlEditorControl.Visible = false;
                        this.simpleTextBox.Visible = true;
                        isHtml = false;
                    }
                }
                else
                {
                    this.htmlEditorControl.Visible = false;
                    this.simpleTextBox.Visible = true;
                    isHtml = false;
                }
            }
        }
    }
}
