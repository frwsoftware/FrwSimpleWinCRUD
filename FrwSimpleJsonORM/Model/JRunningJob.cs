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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;

namespace FrwSoftware
{
    public enum RunningJobStageEnum
    {
        initial,
        waiting,
        concurrent,//rejected due to DisallowConcurrentExecution
        running,
        paused,
        aborted,
        error,
        warning,//complated with warning 
        complated,
        exception
    }
    public enum RunningJobResultEnum
    {
        ok,
        error,
        warning
    }

    //
    // Summary:
    //     Provides data for the System.ComponentModel.BackgroundWorker.DoWork event handler.
    public class JobDoWorkEventArgs : CancelEventArgs
    {
        public object Argument { get; set; }
        public object Result { get; set; }
        public RunningJobResultEnum StageResult { get; set; }
    }

    public delegate void DoJobEventHandler(object sender, JobDoWorkEventArgs e);

    [JDisplayName(typeof(FrwUtilsRes), "JRunningJob")]
    [JEntity]
    public class JRunningJob
    {

        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_JRunningJobId")]
        [JPrimaryKey, JAutoIncrement]
        public string JRunningJobId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_JJobType")]
        [JManyToOne]
        public JJobType JJobType { get; set; }


        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_CreateDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_Stage")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        public string Stage { get; set; }

        public bool IsWorking()
        {
            if (RunningJobStageEnum.running.ToString().Equals(Stage) || RunningJobStageEnum.waiting.ToString().Equals(Stage)
                || RunningJobStageEnum.paused.ToString().Equals(Stage)) return true;
            else return false;
        }

        [JDisplayName(typeof(FrwUtilsRes), "JRunningJob_Progress")]
        public int Progress { get; set; }

        [JIgnore, JsonIgnore]
        public object JobObject { get; set; }

        [JIgnore, JsonIgnore]
        public JobLog JobLog { get; set; }

        [JIgnore, JsonIgnore]
        public bool CancellationPending { get; set; }

        //event 
        public event DoJobEventHandler DoJob;
        public void RunJob(JobDoWorkEventArgs arg)
        {
            DoJob(null, arg);
        }
        [JsonIgnore]
        public Semaphore QueueSemaphore = null;
        [JsonIgnore]
        public AutoResetEvent EndHandle = null;
    }

}
