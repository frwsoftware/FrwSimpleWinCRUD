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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace FrwSoftware
{
    public enum JobConcurrentTypeEnum
    {
        /// <summary>
        /// You can run jobs in parallel threads
        /// </summary>
        Allow,
        /// <summary>
        /// It is not allowed to run jobs in parallel threads. The job added to the queue must wait for the current job to be completed. 
        /// After that it will automatically start.
        /// </summary>
        Wait,
        /// <summary>
        /// It is not allowed to run jobs in parallel threads. In case if one job of this type is already running, 
        /// the new task added to the queue does not start, it is translated into the status "canceled".
        /// </summary>
        Cancel
    }
    public class PostLastJobRunEventArgs : EventArgs
    {
        public bool Canceled = false;
    }

    /// <summary>
    /// The event generated after the last job in the package is completed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PostLJobBatchEventHandler(object sender, PostLastJobRunEventArgs e);

    [JDisplayName(typeof(FrwUtilsRes), "JJobType")]
    [JEntity]
    public class JJobType
    {
        static public int DEFAULT_MAX_THREAD_COUNT = 10;

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JJobTypeId")]
        [JPrimaryKey]
        public string JJobTypeId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_LastRunDate")]
        public DateTimeOffset LastRunDate { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JobBatchLastRunDate")]
        public DateTimeOffset JobBatchLastRunDate { get; set; }

        private string concurrentType = null;

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_ConcurrentType")]
        [JDictProp(DictNames.JobConcurrentType, false, DisplyPropertyStyle.TextOnly)]
        public string ConcurrentType {
            get
            {
                return concurrentType;
            }
            set
            {
                concurrentType = value;
                if (JobConcurrentTypeEnum.Cancel.ToString().Equals(concurrentType) || JobConcurrentTypeEnum.Wait.ToString().Equals(concurrentType))
                {
                    if (MaxThreadCount != 1) MaxThreadCount = 1;
                }
            }
        }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_IsCancelable")]
        public bool IsCancelable { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_MaxThreadCount")]
        public int MaxThreadCount { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_LastStage")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        public string LastStage { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JobBatchStage")]
        [JDictProp(DictNames.RunningJobStage, false, DisplyPropertyStyle.ImageOnly)]
        [JsonIgnore]
        public string JobBatchStage { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JobBatchProgress")]
        [JsonIgnore]
        public int JobBatchProgress { get; set; }


        public bool IsJobBatchWorking()
        {
            if (RunningJobStageEnum.running.ToString().Equals(JobBatchStage) || RunningJobStageEnum.waiting.ToString().Equals(JobBatchStage)
                || RunningJobStageEnum.paused.ToString().Equals(JobBatchStage)) return true;
            else return false;
        }

        [JsonIgnore]
        private Queue<JRunningJob> jobBatchQueue = new Queue<JRunningJob>();

        public void EnqueueJob(JRunningJob job)
        {
            jobBatchQueue.Enqueue(job);
        }
        public JRunningJob DequeueJob()
        {
            JRunningJob job = jobBatchQueue.Dequeue();
            
            return job;
        }
        public void DoPostLJobBatch(PostLastJobRunEventArgs arg)
        {
            if (PostJobBatch != null) PostJobBatch(null, arg);
        }
        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JobBatchQueueCount")]
        [JsonIgnore]
        public int JobBatchQueueCount
        {
            get
            {
                return jobBatchQueue.Count;
            }
        }

        [JDisplayName(typeof(FrwUtilsRes), "JJobType_JobBatchRunningCount")]
        [JsonIgnore]
        public int JobBatchRunningCount
        {
            get
            {
                return JobBatchRunningDict.Count;
            }
        }
        //event 
        public event PostLJobBatchEventHandler PostJobBatch;
        public void RemoveAllPostJobBatchEventHandlers()
        {
            if (PostJobBatch != null && PostJobBatch.GetInvocationList() != null)
            {
                foreach (Delegate d in PostJobBatch.GetInvocationList())
                {
                    PostJobBatch -= (PostLJobBatchEventHandler)d;
                }
            }

        }
 
        [JIgnore, JsonIgnore]
        public object JobBatchObject { get; set; }

        [JIgnore, JsonIgnore]
        public JobLog JobBatchLog { get; set; }

        [JIgnore, JsonIgnore]
        public bool JobBatchCancellationPending { get; set; }

        [JsonIgnore]
        public Semaphore JobBatchQueueSemaphore = null;

        /// <summary>
        /// Tasks from the package that are currently running
        /// </summary>
        [JsonIgnore]
        public ConcurrentDictionary<string, JRunningJob> JobBatchRunningDict = new ConcurrentDictionary<string, JRunningJob>();

        public  void StandartPostLJobBatchEventHandler(object sender, PostLastJobRunEventArgs ew)
        {
            MessageBox.Show((ew.Canceled ? FrwUtilsRes.Job_batch_canceled : FrwUtilsRes.Job_batch_completed), FrwUtilsRes.Job_batch, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
