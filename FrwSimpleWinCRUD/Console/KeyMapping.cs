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
 *   
 *   This file is impordted from Dave Kerr ConsoleControl Project 
 *   https://github.com/dwmkerr/consolecontrol   
 *   MIT License
 *   See also http://www.codeproject.com/Articles/335909/Embedding-a-Console-in-a-C-Application
 **********************************************************************************/

 using System.Windows.Forms;

namespace ConsoleControl
{
    /// <summary>
    /// A KeyMapping defines how a key combination should
    /// be mapped to a SendKeys message.
    /// </summary>
    public class KeyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapping"/> class.
        /// </summary>
        public KeyMapping()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMapping"/> class.
        /// </summary>
        /// <param name="control">if set to <c>true</c> [control].</param>
        /// <param name="alt">if set to <c>true</c> [alt].</param>
        /// <param name="shift">if set to <c>true</c> [shift].</param>
        /// <param name="keyCode">The key code.</param>
        /// <param name="sendKeysMapping">The send keys mapping.</param>
        /// <param name="streamMapping">The stream mapping.</param>
        public KeyMapping(bool control, bool alt, bool shift, Keys keyCode, string sendKeysMapping, string streamMapping)
        {
            //  Set the member variables.
            IsControlPressed = control;
            IsAltPressed = alt;
            IsShiftPressed = shift;
            KeyCode = keyCode;
            SendKeysMapping = sendKeysMapping;
            StreamMapping = streamMapping;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is control pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is control pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsControlPressed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether alt is pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is alt pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsAltPressed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is shift pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shift pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsShiftPressed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key code.
        /// </summary>
        /// <value>
        /// The key code.
        /// </value>
        public Keys KeyCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the send keys mapping.
        /// </summary>
        /// <value>
        /// The send keys mapping.
        /// </value>
        public string SendKeysMapping
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stream mapping.
        /// </summary>
        /// <value>
        /// The stream mapping.
        /// </value>
        public string StreamMapping
        {
            get;
            set;
        }
    }
}