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
 *   This file is  from Dave Kerr ConsoleControl Project 
 *   https://github.com/dwmkerr/consolecontrol   
 *   MIT License
 *   See also http://www.codeproject.com/Articles/335909/Embedding-a-Console-in-a-C-Application
 **********************************************************************************/

 using System;

namespace ConsoleControl
{
  /// <summary>
  /// The ConsoleEventArgs are arguments for a console event.
  /// </summary>
  public class ConsoleEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    public ConsoleEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public ConsoleEventArgs(string content)
    {
      //  Set the content.
      Content = content;
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Content
    {
      get;
      private set;
    }
  }
}
