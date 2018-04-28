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
 *   This file is from Dave Kerr ConsoleControl Project 
 *   https://github.com/dwmkerr/consolecontrol   
 *   MIT License
 *   See also http://www.codeproject.com/Articles/335909/Embedding-a-Console-in-a-C-Application
 **********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConsoleControlSample
{
  /// <summary>
  /// The new process form.
  /// </summary>
  public partial class FormNewProcess : Form
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FormNewProcess"/> class.
    /// </summary>
    public FormNewProcess()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the name of the file.
    /// </summary>
    /// <value>
    /// The name of the file.
    /// </value>
    public string FileName
    {
      get { return textBoxFileName.Text; }
    }

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public string Arguments
    {
      get { return textBoxArguments.Text; }
    }
  }
}
