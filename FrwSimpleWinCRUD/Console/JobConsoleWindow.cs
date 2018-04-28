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
    public partial class JobConsoleWindow : ConsoleAdvancedWindow, IViewProcessor
    {
        public JRunningJob RunningJob { get; set; }

        public JobConsoleWindow()
        {
            InitializeComponent();
        }

        #region IViewProcessor
        virtual public void CreateView()
        {
        }
        public void ProcessView()
        {
            if (RunningJob != null) SetNewCaption("Консоль - " + RunningJob.Name);
        }
        #endregion

        #region saveconfig


        public override IDictionary<string, object> GetKeyParams()
        {
            IDictionary<string, object> pars = base.GetKeyParams();
            if (RunningJob != null) pars.Add("RunningJob", RunningJob.JRunningJobId);
            return pars;
        }
        public override void SetKeyParams(IDictionary<string, object> pars)
        {
            if (pars == null) return;

            object t = DictHelper.Get(pars, "RunningJob");
            if (t != null && t is JRunningJob) RunningJob = t as JRunningJob;
            else if (t != null && t is string)
            {
                RunningJob = Dm.Instance.Find<JRunningJob>(t as string);
                //todo load log 
            }
            else if (t != null) throw new ArgumentException();

        }
        public override bool CompareKeyParams(IDictionary<string, object> pars)
        {
            //keys в данном методе являются только объектами, но не стрингами 
            if (!compareJobKey(DictHelper.Get(pars, "RunningJob"), RunningJob)) return false;
            return true;
        }

        private bool compareJobKey(object key, JRunningJob item)
        {
            //если ключ не задан сравнение по нему не производится 
            if (key != null)
            {
                if (key is JRunningJob) return ((JRunningJob)key).Equals(item);
                if (key is string) return (item != null) ? ((string)key).Equals(RunningJob.JRunningJobId) : false;
                else throw new ArgumentException();
            }
            else return true;
        }
        #endregion


    }
}
