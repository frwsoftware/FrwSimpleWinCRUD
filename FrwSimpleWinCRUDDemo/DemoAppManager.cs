﻿/**********************************************************************************
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware.Model.Chinook;

namespace FrwSoftware
{
    public class DemoAppManager : AppManager
    {
        override protected BaseMainAppForm GetMainForm()
        {
            return new FrwMainAppForm();
        }
        override protected IListProcessor GetListWindowForType(Type type)
        {
            IListProcessor w = null;
            if (type == typeof(Invoice)) w = new InvoiceListWindow();
            //else if (type == typeof(JDocPanelLayout)) w = new JDocPanelLayoutListWindow();
            else w = base.GetListWindowForType(type);
            return w;
        }
        override protected IPropertyProcessor GetPropertyWindowForType(Type type)
        {
            return base.GetPropertyWindowForType(type);
        }

    }
}
