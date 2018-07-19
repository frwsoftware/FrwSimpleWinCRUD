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

using System.IO;

namespace FrwSoftware
{
    public partial class ConsoleAdvancedWindow : FrwBaseViewControl
    {
        public ConsoleAdvancedWindow()
        {
            InitializeComponent();
            Text = FrwCRUDRes.Console;
        }

        public TextWriter ConsoleWriter
        {
            get
            {
                return consoleControlAdvanced1.ConsoleWriter;
            }
        }

        public void SetCatchConsoleMode()
        {
            consoleControlAdvanced1.SetCatchConsoleMode();
        }
    }
}
