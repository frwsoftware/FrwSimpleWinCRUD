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
    public partial class JobTypeConsoleWindow : ConsoleAdvancedWindow, IViewProcessor
    {

        public JJobType JobType { get; set; }

        public JobTypeConsoleWindow()
        {
            InitializeComponent();
        }

        #region IViewProcessor
        virtual public void CreateView()
        {
        }
        public void ProcessView()
        {
            if (JobType != null) SetNewCaption(FrwCRUDRes.Console + " - " + JobType.Name);
        }
        #endregion

        #region saveconfig


        public override IDictionary<string, object> GetKeyParams()
        {
            IDictionary<string, object> pars = base.GetKeyParams();
            if (JobType != null) pars.Add("JobType", JobType.JJobTypeId);
            return pars;
        }
        public override void SetKeyParams(IDictionary<string, object> pars)
        {
            if (pars == null) return;

            object t = DictHelper.Get(pars, "JobType");
            if (t != null && t is JJobType) JobType = t as JJobType;
            else if (t != null && t is string)
            {
                JobType = Dm.Instance.Find<JJobType>(t as string);
                //todo load log 
            }
            else if (t != null) throw new ArgumentException();

        }
        public override bool CompareKeyParams(IDictionary<string, object> pars)
        {
            if (!compareJobTypeKey(DictHelper.Get(pars, "JobType"), JobType)) return false;
            return true;
        }

        private bool compareJobTypeKey(object key, JJobType item)
        {
            if (key != null)
            {
                if (key is JJobType) return ((JJobType)key).Equals(item);
                if (key is string) return (item != null) ? ((string)key).Equals(JobType.JJobTypeId) : false;
                else throw new ArgumentException();
            }
            else return true;
        }
        #endregion


    }
}
