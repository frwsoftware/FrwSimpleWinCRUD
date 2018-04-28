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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsoleControlSample
{
    /// <summary>
    /// The sample form.
    /// </summary>
    public partial class ConsoleControlAdvanced : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleControlAdvanced"/> class.
        /// </summary>
        public ConsoleControlAdvanced()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the FormConsoleControlSample control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void FormConsoleControlSample_Load(object sender, EventArgs e)
        {
            //  Update the UI state.
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonNewProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonNewProcess_Click(object sender, EventArgs e)
        {
            //  Create the new process form.
            FormNewProcess formNewProcess = new FormNewProcess();

            //  If the form is shown OK, start the process.
            if (formNewProcess.ShowDialog() == DialogResult.OK)
            {
                //  Start the proces.
                consoleControl.StartProcess(formNewProcess.FileName, formNewProcess.Arguments, null);

                //  Update the UI state.
                UpdateUIState();
            }
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonStopProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonStopProcess_Click(object sender, EventArgs e)
        {
            consoleControl.StopProcess();

            //  Update the UI state.
            UpdateUIState();
        }

        /// <summary>
        /// Updates the state of the UI.
        /// </summary>
        private void UpdateUIState()
        {
            
            //  Update the state.
            if (consoleControl.IsProcessRunning)
                toolStripStatusLabelConsoleState.Text = "Running " + System.IO.Path.GetFileName(consoleControl.ProcessInterface.ProcessFileName);
            else
                toolStripStatusLabelConsoleState.Text = "Not Running";

            //  Update toolbar buttons.
            toolStripButtonRunCMD.Enabled = !consoleControl.IsProcessRunning;
            toolStripButtonNewProcess.Enabled = !consoleControl.IsProcessRunning;
            toolStripButtonStopProcess.Enabled = consoleControl.IsProcessRunning;
            toolStripButtonShowDiagnostics.Checked = consoleControl.ShowDiagnostics;
            toolStripButtonInputEnabled.Checked = consoleControl.IsInputEnabled;
            toolStripButtonSendKeyboardCommandsToProcess.Checked = consoleControl.SendKeyboardCommandsToProcess;
            
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonShowDiagnostics control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonShowDiagnostics_Click(object sender, EventArgs e)
        {
            consoleControl.ShowDiagnostics = !consoleControl.ShowDiagnostics;
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Tick event of the timerUpdateUI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timerUpdateUI_Tick(object sender, EventArgs e)
        {
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonInputEnabled control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonInputEnabled_Click(object sender, EventArgs e)
        {
            consoleControl.IsInputEnabled = !consoleControl.IsInputEnabled;
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonRunCMD control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonRunCMD_Click(object sender, EventArgs e)
        {
            consoleControl.StartProcess("cmd", null, Encoding.GetEncoding(866));//todo i18n for not only rus  https://stackoverflow.com/questions/16803748/how-to-decode-cmd-output-correctly
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonSendKeyboardCommandsToProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonSendKeyboardCommandsToProcess_Click(object sender, EventArgs e)
        {
            consoleControl.SendKeyboardCommandsToProcess = !consoleControl.SendKeyboardCommandsToProcess;
            UpdateUIState();
        }

        /// <summary>
        /// Handles the Click event of the toolStripButtonClearOutput control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void toolStripButtonClearOutput_Click(object sender, EventArgs e)
        {
            consoleControl.ClearOutput();
        }

        private void toolStripButtonAttachConsole_Click(object sender, EventArgs e)
        {
            toolStripButtonAttachConsole.Checked = !toolStripButtonAttachConsole.Checked;
            consoleControl.CatchConsole(toolStripButtonAttachConsole.Checked);
        }

        public void SetCatchConsoleMode()
        {
            toolStripButtonAttachConsole.Checked = true;
            consoleControl.CatchConsole(toolStripButtonAttachConsole.Checked);
        }

        public TextWriter ConsoleWriter
        {
            get
            {
                return consoleControl.ConsoleWriter;
            }
        }
    }
}