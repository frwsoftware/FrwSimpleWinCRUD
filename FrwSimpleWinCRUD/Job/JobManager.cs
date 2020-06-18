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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FrwSoftware
{
    public class JobManager
    {
        public static JobManager Instance
        {
            get { return JobManager.instance ?? (JobManager.instance = new JobManager()); }
            set
            {
                if (JobManager.instance != null) throw new InvalidOperationException("Custom instance can be set only once and before first getting");
                JobManager.instance = value;
            }
        }
        private static JobManager instance;

        /// <summary>
        /// Creates new job and job type (if needs)
        /// </summary>
        /// <param name="jjobTypeId"></param>
        /// <param name="jobTypeName"></param>
        /// <param name="additionalJobName"></param>
        /// <param name="concurType"></param>
        /// <returns></returns>
        public JRunningJob CreateJob(string jjobTypeId, string jobTypeName, string additionalJobName, JobConcurrentTypeEnum concurType = JobConcurrentTypeEnum.Wait)
        {
            DateTime time = DateTime.Now;
            if (jjobTypeId == null) throw new ArgumentException();
            JJobType jobType = (JJobType)Dm.Instance.Find(typeof(JJobType), jjobTypeId);
            bool created = false;
            if (jobType == null)
            {
                jobType = (JJobType)Dm.Instance.EmptyObject(typeof(JJobType), null);
                jobType.JJobTypeId = jjobTypeId;
                jobType.ConcurrentType = concurType.ToString();
                if (JobConcurrentTypeEnum.Allow.ToString().Equals(jobType.ConcurrentType))
                {
                    jobType.MaxThreadCount = JJobType.DEFAULT_MAX_THREAD_COUNT;
                }
                if (jobTypeName != null)
                {
                    jobType.Name = jobTypeName;
                }
                else
                {
                   jobType.Name = jjobTypeId;
                }
                Dm.Instance.SaveObject(jobType);
                created = true;
            }
            if (jobType.JobBatchLog == null)
            {
                jobType.JobBatchLog = new JobLog();
                jobType.JobBatchLog.LogFileName = GetJobTypeLogFileName(jobType);
                JobTypeConsoleWindow consoleWindow = (JobTypeConsoleWindow)AppManager.Instance.FindContent(typeof(JobTypeConsoleWindow), null, new Dictionary<string, object> { { "JobType", jobType } });
                if (consoleWindow != null)
                {
                    if (jobType.JobBatchLog != null) jobType.JobBatchLog.ExternalWriter = consoleWindow.ConsoleWriter;

                }
            }
            if (created)  jobType.JobBatchLog.Debug("Job type" + jobType.JJobTypeId + " created new");
            else jobType.JobBatchLog.Debug("Job type" + jobType.JJobTypeId + " found");

            JRunningJob job = null;
            job = (JRunningJob)Dm.Instance.EmptyObject(typeof(JRunningJob), null);
            job.Stage = RunningJobStageEnum.initial.ToString();

            job.CreateDate = time;
            if (additionalJobName != null) job.Name = (jobType.Name) + " " + additionalJobName;
            else job.Name = (jobType.Name) + " " + time.ToString("yyyy/MM/dd HH:mm:ss"); ;
            job.JJobType = jobType;
            job.JobLog = new JobLog();
            job.JobLog.ParentLog = jobType.JobBatchLog;//paren ref 
            job.JobLog.LogFileName = GetJobLogFileName(job);
            Dm.Instance.SaveObject(job);
            job.JobLog.Debug("Job " + job.JRunningJobId + " created");
            return job;
        }

        public string GetJobLogFileName(JRunningJob job)
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, Path.Combine("jobLogs", Path.Combine(job.JJobType.JJobTypeId, "jobLog_" + job.JRunningJobId)));
        }
        public string GetJobTypeLogFileName(JJobType jobType)
        {
            return Path.Combine(FrwConfig.Instance.ProfileDir, Path.Combine("jobLogs", Path.Combine(jobType.JJobTypeId, "jobTypeLog")));
        }

        static public bool ScheduleJobToQueue(JRunningJob job)
        {
            if (JobConcurrentTypeEnum.Cancel.ToString().Equals(job.JJobType.ConcurrentType)
                && job.JJobType.JobBatchQueueCount > 0)
            {
                job.JobLog.Info("Job " + job.JRunningJobId + " canceled due to concurence");
                job.Stage = RunningJobStageEnum.concurrent.ToString();
                Dm.Instance.SaveObject(job);
                return false;
            }
            job.JJobType.EnqueueJob(job);
            job.JobLog.Info("Job " + job.JRunningJobId + " scheduled  To Queue");
            return true;
        }
        static public void StartProcessingJobBatch(JJobType jobType)
        {
            if (jobType.JobBatchObject != null) throw new InvalidOperationException();//todo

            BackgroundWorker mainWorker = new AbortableBackgroundWorker();
            mainWorker.WorkerSupportsCancellation = true;
            mainWorker.WorkerReportsProgress = true;
            mainWorker.DoWork += (sd, ew) =>
            {
                BackgroundWorker wk = sd as BackgroundWorker;
                if (wk.CancellationPending)
                {
                    ew.Cancel = true;
                    return;
                }
                try
                {
                    if (jobType.JobBatchQueueSemaphore == null)
                    {

                        //first start
                        int maxTreadCount = 1;
                        if (JobConcurrentTypeEnum.Cancel.ToString().Equals(jobType.ConcurrentType)) maxTreadCount = 1;
                        else if (JobConcurrentTypeEnum.Wait.ToString().Equals(jobType.ConcurrentType)) maxTreadCount = 1;
                        else if (JobConcurrentTypeEnum.Allow.ToString().Equals(jobType.ConcurrentType)) maxTreadCount = jobType.MaxThreadCount;

                        jobType.JobBatchRunningDict.Clear();
                        jobType.JobBatchQueueSemaphore = new Semaphore(maxTreadCount, maxTreadCount);
                        jobType.JobBatchLog.Debug("JobType Semaphore created maxTreadCount: " + maxTreadCount);

                        jobType.JobBatchLastRunDate = DateTime.Now;
                        jobType.JobBatchStage = RunningJobStageEnum.running.ToString();
                        Dm.Instance.SaveObject(jobType);
                    }

                    List<AutoResetEvent> endHandlers = new List<AutoResetEvent>();
                    int initialQueueCount = jobType.JobBatchQueueCount;
                    int startedCount = 0;
                    while (true)
                    {
                        if (wk.CancellationPending)
                        {
                            ew.Cancel = true;
                            break;
                        }
                        if (jobType.JobBatchQueueCount == 0) break;

                        jobType.JobBatchQueueSemaphore.WaitOne();

                        JRunningJob job = jobType.DequeueJob();
                        job.QueueSemaphore = jobType.JobBatchQueueSemaphore;
                        job.EndHandle = new AutoResetEvent(false);
                        endHandlers.Add(job.EndHandle);
                        
                        ScheduleJobBackgroundLocal(job);

                        startedCount++;
                        int progress = (int)(((double)startedCount / (double)initialQueueCount) * 100.0);
                        wk.ReportProgress(progress);
                      
                    }
                    if (endHandlers.Count > 0) WaitHandle.WaitAll(endHandlers.ToArray<AutoResetEvent>());

                    PostLastJobRunEventArgs arg1 = new PostLastJobRunEventArgs();
                    arg1.Canceled = ew.Cancel;
                    jobType.DoPostLJobBatch(arg1);
                    if (ew.Cancel == false)
                        ew.Result = RunningJobStageEnum.complated;
                }
                catch (Exception ex)
                {
                    jobType.JobBatchLog.Error("Job Batch fatal error", ex);
                    ew.Result = RunningJobStageEnum.exception;
                    JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.exception);
                }
            };
            mainWorker.RunWorkerCompleted += (sd, ek) =>
            {
                ResetJobBatch(jobType);
                if (ek.Cancelled == true)
                {
                    JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.aborted);
                }
                else
                {
                    if (RunningJobStageEnum.exception.Equals(ek.Result))
                    {
                        //do nothing
                    }
                    else if (RunningJobStageEnum.warning.Equals(ek.Result))
                        JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.warning);
                    else if (RunningJobStageEnum.error.Equals(ek.Result))
                        JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.error);
                    else
                        JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.complated);
                }
            };
            mainWorker.ProgressChanged += (sd, pc) =>
            {
                localReportProgresJobType(jobType, pc.ProgressPercentage);
            };
            //run worker
            jobType.JobBatchObject = mainWorker;
            mainWorker.RunWorkerAsync();
        }


        static public void ResetJobBatch(JJobType jobType)
        {
            //reset
            jobType.JobBatchObject = null;
            jobType.JobBatchQueueSemaphore = null;
        }

        static public void ScheduleJobBackground(JRunningJob job)
        {
            ScheduleJobToQueue(job);
            StartProcessingJobBatch(job.JJobType);
        }

        static private void ScheduleJobBackgroundLocal(JRunningJob job)
        {
            BackgroundWorker worker = new AbortableBackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sd, ew) =>
            {
                BackgroundWorker wk = sd as BackgroundWorker;
                if (wk.CancellationPending)
                {
                    ew.Cancel = true;
                    return;
                }
                try
                {
                    JobDoWorkEventArgs arg = new JobDoWorkEventArgs();
                    job.JobLog.Info("Job started " + job.Name);
                    arg.StageResult = RunningJobResultEnum.ok;
                    job.Stage = RunningJobStageEnum.running.ToString();
                    job.JJobType.JobBatchRunningDict.TryAdd(job.JRunningJobId, job);
                    Dm.Instance.SaveObject(job);
                    job.RunJob(arg);
                    if (arg.StageResult == RunningJobResultEnum.ok) ew.Result = RunningJobStageEnum.complated;
                    else if (arg.StageResult == RunningJobResultEnum.warning) ew.Result = RunningJobStageEnum.warning;
                    else if (arg.StageResult == RunningJobResultEnum.error) ew.Result = RunningJobStageEnum.error;
                    if (job.CancellationPending) ew.Cancel = true;
                            
                }
                catch (Exception ex)
                {
                    if (ex is System.Threading.ThreadAbortException)
                    {
                        job.JobLog.Warn("Job aborted");
                    }
                    else
                    {
                        job.JobLog.Error("Job fatal error", ex);
                    }
                    ew.Result = RunningJobStageEnum.exception;
                    JobManager.Instance.ComplateJob(job, RunningJobStageEnum.exception);
                }
                finally
                {
                    ResetJob(job);
                }
            };
            worker.RunWorkerCompleted += (sd, ek) =>
            {
                if (ek.Cancelled == true)
                {
                    job.JobLog.Debug("Job aborted");
                    JobManager.Instance.ComplateJob(job, RunningJobStageEnum.aborted);
                }
                else
                {
                    if (RunningJobStageEnum.exception.Equals(ek.Result))
                    {
                        //do nothing
                    }
                    else if (RunningJobStageEnum.warning.Equals(ek.Result))
                        JobManager.Instance.ComplateJob(job, RunningJobStageEnum.warning);
                    else if (RunningJobStageEnum.error.Equals(ek.Result))
                        JobManager.Instance.ComplateJob(job, RunningJobStageEnum.error);
                    else
                        JobManager.Instance.ComplateJob(job, RunningJobStageEnum.complated);
                }
            };
            worker.ProgressChanged += (sd, pc) =>
            {
                localReportProgresJob(job, pc.ProgressPercentage);
            };
            //run worker
            job.JobObject = worker;
            (job.JobObject as BackgroundWorker).RunWorkerAsync();
            if (job.JJobType != null)
            {
                job.JJobType.LastRunDate = DateTimeOffset.Now;
                Dm.Instance.SaveObject(job.JJobType);
            }

        }

        static private void ResetJob(JRunningJob job)
        {
            if (job.QueueSemaphore != null) job.QueueSemaphore.Release();
            if (job.EndHandle != null) job.EndHandle.Set();
            if (job.JJobType.JobBatchRunningDict != null)
            {
                JRunningJob tmp;
                job.JJobType.JobBatchRunningDict.TryRemove(job.JRunningJobId, out tmp);
            }
        }

        public void CancelAsyncJob(JRunningJob job)
        {
            try
            {
                if (job.JobObject != null && job.JobObject is BackgroundWorker)
                {
                    (job.JobObject as BackgroundWorker).CancelAsync();
                }
                job.CancellationPending = true;
            }
            catch (Exception ex)
            {
                Log.LogError("Error CancelAsync job: " + job.JRunningJobId, ex);
            }
        }

        public void AbortJob(JRunningJob job, int pauseBeforeAbort = 1000)
        {
            try
            {
                if (job.JobObject != null)
                {
                    CancelAsyncJob(job);
                    Thread.Sleep(pauseBeforeAbort);
                    if (job.JobObject is AbortableBackgroundWorker)
                    {
                        AbortableBackgroundWorker backgroundWorker1 = (job.JobObject as AbortableBackgroundWorker);
                        if (backgroundWorker1.IsBusy == true)
                        {
                            backgroundWorker1.Abort();
                            backgroundWorker1.Dispose();
                        }
                    }
                    ResetJob(job);
                    ComplateJob(job, RunningJobStageEnum.aborted);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error aborting job: " + job.JRunningJobId, ex);
            }
        }

        public void AbortAllJobsAndJobBatches()
        {
            IList jobTypes = Dm.Instance.FindAll(typeof(JJobType));
            foreach (var j in jobTypes)
            {
                JJobType jobType = j as JJobType;
                if (jobType.IsJobBatchWorking())
                {
                    AbortJobBatch(jobType);
                }
            }

            IList jobs = Dm.Instance.FindAll(typeof(JRunningJob));
            foreach(var j in jobs)
            {
                JRunningJob job = j as JRunningJob;
                if (job.IsWorking())
                {
                    AbortJob(job);
                }
            }
        }

        public void CancelAsyncJobBatch(JJobType jobType)
        {
            try
            {
                if (jobType.JobBatchObject != null && jobType.JobBatchObject is BackgroundWorker)
                {
                    (jobType.JobBatchObject as BackgroundWorker).CancelAsync();
                }
                jobType.JobBatchCancellationPending = true;
            }
            catch (Exception ex)
            {
                Log.LogError("Error CancelAsync job: " + jobType.JJobTypeId, ex);
            }
        }

        public void AbortJobBatch(JJobType jobType, int pauseBeforeAbort = 1000)
        {
            try
            {
                if (jobType.JobBatchObject != null)
                {
                    CancelAsyncJobBatch(jobType);
                    Thread.Sleep(pauseBeforeAbort);
                    //
                    List<JRunningJob> jobs = new List<JRunningJob>();
                    jobs.AddRange(jobType.JobBatchRunningDict.Values);//copy for prevent deadlock
                    foreach (var job in jobs)
                    {
                        AbortJob(job);
                    }
                    //
                    if (jobType.JobBatchObject is AbortableBackgroundWorker)
                    {
                        AbortableBackgroundWorker backgroundWorker1 = (jobType.JobBatchObject as AbortableBackgroundWorker);
                        if (backgroundWorker1.IsBusy == true)
                        {
                            backgroundWorker1.Abort();
                            backgroundWorker1.Dispose();
                        }
                    }
                    ResetJobBatch(jobType);
                    JobManager.Instance.ComplateJobBatch(jobType, RunningJobStageEnum.aborted);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error aborting job: " + jobType.JJobTypeId, ex);
            }
        }



        public void ComplateJob(JRunningJob job, RunningJobStageEnum stage = RunningJobStageEnum.complated)
        {
            try
            {
                job.JobLog.Info("Job complate with result: " + stage);
                job.Stage = stage.ToString();
                if (job.Progress == 0) job.Progress = 100;
                Dm.Instance.SaveObject(job);
                //save log to file
                job.JobLog.SaveLogToFile();
                if (job.JJobType != null)
                {
                    job.JJobType.LastStage = job.Stage;
                    Dm.Instance.SaveObject(job.JJobType);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error complating job: " + job.JRunningJobId, ex);
            }

        }
        public void ComplateJobBatch(JJobType jobType, RunningJobStageEnum stage = RunningJobStageEnum.complated)
        {
            try
            {
                jobType.JobBatchLog.Info("JobBatch complate with result: " + stage);
                jobType.JobBatchStage = stage.ToString();
                if (jobType.JobBatchProgress == 0) jobType.JobBatchProgress = 100;
                Dm.Instance.SaveObject(jobType);
                //save log to file
                jobType.JobBatchLog.SaveLogToFile();
            }
            catch (Exception ex)
            {
                Log.LogError("Error complating job: " + jobType.JJobTypeId, ex);
            }

        }
        public void ReportProgresJob(JRunningJob job, int progressPercentage)
        {
            try
            {
                if (job.JobObject != null && job.JobObject is BackgroundWorker)
                {
                    (job.JobObject as BackgroundWorker).ReportProgress(progressPercentage);
                }
                else
                {
                    localReportProgresJob(job, progressPercentage);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error reporting progress job: " + job.JRunningJobId, ex);
            }
        }
        private static void localReportProgresJob(JRunningJob job, int progressPercentage)
        {
            try
            {
                if (job.Progress != progressPercentage)
                {
                    job.JobLog.Debug("Out Progress job " + job.JRunningJobId + " Progress: " + progressPercentage.ToString() + "%");
                    job.Progress = progressPercentage;
                    Dm.Instance.SaveObject(job);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error reporting progress job: " + job.JRunningJobId, ex);
            }
        }
        private static void localReportProgresJobType(JJobType jobType, int progressPercentage)
        {
            try
            {
                if (jobType.JobBatchProgress != progressPercentage)
                {
                    jobType.JobBatchLog.Debug("Out Progress jobType " + jobType.JJobTypeId + " Progress: " + progressPercentage.ToString() + "%");
                    jobType.JobBatchProgress = progressPercentage;
                    Dm.Instance.SaveObject(jobType);
                }
            }
            catch (Exception ex)
            {
                Log.LogError("Error reporting progress jobType: " + jobType.JJobTypeId, ex);
            }
        }

        public static void MakeTitleContextMenuForRunningJob(JRunningJob job, List<ToolStripItem> menuItemList, IContentContainer docPanelContainer)
        {
            ToolStripMenuItem menuItem = null;
            if (job != null)
            {
                List<ToolStripItem> menuItemList1 = new List<ToolStripItem>();
                MakeContextMenuForRunningJob(job, menuItemList1, docPanelContainer);
                if (menuItemList1.Count > 0)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Job__ + job.Name  +"\"" +" (" + job.Stage + ")";
                    menuItemList.Add(menuItem);
                    menuItem.DropDownItems.AddRange(menuItemList1.ToArray<ToolStripItem>());
                }
            }
        }
        public static void MakeTitleContextMenuForRunningJobTypeQueue(JJobType jobType, List<ToolStripItem> menuItemList, IContentContainer docPanelContainer)
        {
            ToolStripMenuItem menuItem = null;
            if (jobType != null)
            {
                List<ToolStripItem> menuItemList1 = new List<ToolStripItem>();
                MakeContextMenuForRunningJobBatch(jobType, menuItemList1, docPanelContainer);
                if (menuItemList1.Count > 0)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Job_batch__ + jobType.Name + "\"";// + " (" + job.Stage + ")";
                    menuItemList.Add(menuItem);
                    menuItem.DropDownItems.AddRange(menuItemList1.ToArray<ToolStripItem>());
                }
            }
        }
        public static void MakeContextMenuForRunningJob(JRunningJob job, List<ToolStripItem> menuItemList, IContentContainer docPanelContainer)
        {

            ToolStripMenuItem menuItem = null;
            if (job != null)
            {
                if (RunningJobStageEnum.running.ToString().Equals(job.Stage) && job.JobObject != null)
                {
                    if (job.JJobType.IsCancelable)
                    {
                        menuItem = new ToolStripMenuItem();
                        menuItem.Text = FrwCRUDRes.Try_to_cancel_job;
                        menuItem.Click += (s, em) =>
                        {
                            try
                            {
                                JobManager.Instance.CancelAsyncJob(job);
                            }
                            catch (Exception ex)
                            {
                                Log.ShowError(ex);
                            }
                        };
                        menuItemList.Add(menuItem);
                    }

                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Kill_job_thread;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            JobManager.Instance.AbortJob(job);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);

                }

                string logFileName = JobManager.Instance.GetJobLogFileName(job);
                FileInfo logFile = new FileInfo(logFileName);
                if (job.JobLog != null || logFile.Exists)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Show_job_console;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            JobConsoleWindow consoleWindow = (JobConsoleWindow)AppManager.Instance.CreateContent(docPanelContainer, typeof(JobConsoleWindow),
                                new Dictionary<string, object> { { "RunningJob", job } });
                            if (consoleWindow != null)
                            {
                                if (job.JobLog != null) job.JobLog.ExternalWriter = consoleWindow.ConsoleWriter;
                                else if (logFile.Exists) consoleWindow.ConsoleWriter.Write(File.ReadAllText(logFile.FullName));
                                consoleWindow.ProcessView();
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                }
            }
        }
        public static void MakeContextMenuForRunningJobBatch(JJobType jobType, List<ToolStripItem> menuItemList, IContentContainer docPanelContainer)
        {

            ToolStripMenuItem menuItem = null;
            if (jobType != null)
            {
                if (RunningJobStageEnum.running.ToString().Equals(jobType.JobBatchStage) && jobType.JobBatchObject != null)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Try_to_cancel_job_batch;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            JobManager.Instance.CancelAsyncJobBatch(jobType);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);

                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Kill_job_batch_thread;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            JobManager.Instance.AbortJobBatch(jobType);
                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);

                }

                string logFileName = JobManager.Instance.GetJobTypeLogFileName(jobType);
                FileInfo logFile = new FileInfo(logFileName);
                if (jobType.JobBatchLog != null || logFile.Exists)
                {
                    menuItem = new ToolStripMenuItem();
                    menuItem.Text = FrwCRUDRes.Show_job_batch_console;
                    menuItem.Click += (s, em) =>
                    {
                        try
                        {
                            JobTypeConsoleWindow consoleWindow = (JobTypeConsoleWindow)AppManager.Instance.CreateContent(docPanelContainer, typeof(JobTypeConsoleWindow),
                                new Dictionary<string, object> { { "JobType", jobType } });
                            if (consoleWindow != null)
                            {
                                if (jobType.JobBatchLog != null) jobType.JobBatchLog.ExternalWriter = consoleWindow.ConsoleWriter;
                                else if (logFile.Exists) consoleWindow.ConsoleWriter.Write(File.ReadAllText(logFile.FullName));
                                consoleWindow.ProcessView();
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.ShowError(ex);
                        }
                    };
                    menuItemList.Add(menuItem);
                }
            }
        }
    }
}