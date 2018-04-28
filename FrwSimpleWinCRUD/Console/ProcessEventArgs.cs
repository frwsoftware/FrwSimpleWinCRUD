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
using System;

namespace ConsoleControlAPI
{
    /// <summary>
    /// The ProcessEventArgs are arguments for a console event.
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        public ProcessEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public ProcessEventArgs(string content)
        {
            //  Set the content and code.
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public ProcessEventArgs(int code)
        {
            //  Set the content and code.
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="code">The code.</param>
        public ProcessEventArgs(string content, int code)
        {
            //  Set the content and code.
            Content = content;
            Code = code;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int? Code { get; private set; }
    }
}