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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FrwSoftware 
{
    public partial class BaseViewWindow : FrwBaseViewControl, IViewProcessor
    {
        protected ViewType viewType = ViewType.NONE;
        protected UserControl viewControl = null;
        public ViewType ViewType { get { return viewType; } set { viewType = value; } }
        public string FileFullPath { get; set; }
        public string WebEntityInfoPropertyName { get; set; }
        public BrowserPrivateType BrowserPrivateType { get; set; }
        public object LinkedObject { get; set; }
        protected bool viewProcessed = false;

        public BaseViewWindow()
        {
            InitializeComponent();
            Text = "View";
        }
        public void CreateView()
        {
        }
        virtual public void ProcessView()
        {
            if (viewControl != null) throw new InvalidOperationException();
            if (viewProcessed) throw new InvalidOperationException();
            else viewProcessed = true;
            WebEntryInfo webEntryInfo = WebEntryInfo.GetWebEntryInfoFromObject(LinkedObject);
            if (viewType == ViewType.Simple)
                viewControl = new SimpleViewControl();
            else
                throw new InvalidOperationException();
            AddSpecialTask();
            viewControl.Dock = DockStyle.Fill;
            this.Controls.Add(viewControl);
            string cap = null;
            if (LinkedObject != null) cap = ModelHelper.GetNameForObject( LinkedObject);
            else if (FileFullPath != null) cap = FileFullPath;
            if (cap != null && cap.Length > 200) cap = cap.Substring(0, 200) + "...";
            if (cap != null) SetNewCaption(cap);
        }

        protected virtual void AddSpecialTask()
        {
        }
  

        #region saverestore
        //for restore DockContent
        public override IDictionary<string, object> GetKeyParams()
        {
            IDictionary<string, object> pars = base.GetKeyParams();
            if (LinkedObject != null)
            {
                pars.Add("Item", ModelHelper.GetPKValue(LinkedObject));
                pars.Add("ItemType", LinkedObject.GetType().FullName);
            }
            pars.Add("ViewType", ViewType);
            if (FileFullPath != null) pars.Add("FileFullPath", FileFullPath);
            if (WebEntityInfoPropertyName != null) pars.Add("WebEntityInfoPropertyName", WebEntityInfoPropertyName);
            pars.Add("BrowserPrivateType", BrowserPrivateType);
            
            return pars;
        }
        public override void SetKeyParams(IDictionary<string, object> pars)
        {
            base.SetKeyParams(pars);

            if (pars == null) return;
            object t = DictHelper.Get(pars, "Item");
            if (t != null) {
                if (t != null && t is string) {
                    object typeStr = DictHelper.Get(pars, "ItemType");
                    if (typeStr != null && typeStr is string)
                    {
                        LinkedObject = Dm.Instance.FindTypeAddFindInstance(typeStr as string, t as string);
                    }
                    else throw new ArgumentException();
                }
                else LinkedObject = t;
            }

            t = DictHelper.Get(pars, "ViewType");
            if (t != null && t is ViewType) ViewType = (ViewType)t;
            else if (t != null && t is string) ViewType = (t as string).ToEnum(ViewType.NONE);
            else if (t != null) throw new ArgumentException();

            t = DictHelper.Get(pars, "FileFullPath");
            if (t != null) FileFullPath = t as string;

            t = DictHelper.Get(pars, "WebEntityInfoPropertyName");
            if (t != null) WebEntityInfoPropertyName = t as string;

            t = DictHelper.Get(pars, "BrowserPrivateType");
            if (t != null && t is BrowserPrivateType) BrowserPrivateType = (BrowserPrivateType)t;
            else if (t != null && t is string) BrowserPrivateType = (t as string).ToEnum(BrowserPrivateType.COMMON_CACHE);
            else if (t != null) throw new ArgumentException();


        }
        public override bool CompareKeyParams(IDictionary<string, object> pars)
        {
            if (!compareItemKey(DictHelper.Get(pars, "Item"), LinkedObject)) return false;
            if (!compareObjectKey(DictHelper.Get(pars, "ViewType"), ViewType)) return false;
            if (!compareStringKey(DictHelper.Get(pars, "FileFullPath"), FileFullPath)) return false;
            if (!compareStringKey(DictHelper.Get(pars, "WebEntityInfoPropertyName"), WebEntityInfoPropertyName)) return false;
            if (!compareObjectKey(DictHelper.Get(pars, "BrowserPrivateType"), BrowserPrivateType)) return false;
            
            return true;
        }
        private bool compareItemKey(object key, object linkedObject)
        {
            if (key != null)
            {
                if (linkedObject == null) return false;
                object pkValue = ModelHelper.GetPKValue(linkedObject);
                if (pkValue == null) return false;
                if (key is string) return ((string)key).Equals(pkValue);
                else
                {
                    object pkValue1 = ModelHelper.GetPKValue(key);
                    if (pkValue1 == null) return false;
                    else  return (pkValue1).Equals(pkValue);
                }
            }
            else return true;
        }
        #endregion
    }
}
