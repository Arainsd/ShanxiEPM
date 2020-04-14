using FluentScheduler;
using hc.epm.Common;
using hc.epm.Helper.Timer;
using hc.Plat.Cache.Helper;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hc.epm.TimerManage
{
    public partial class MessageForm : Form
    {
        private string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        public MessageForm()
        {
            InitializeComponent();
        }

        private void MessageForm_Load(object sender, EventArgs e)
        {
            LogHelper.SetConfig(path + "timerlog.config");
            TaskManage.Start(JobStart, JobEnd, JobEx);
            BindTable();
        }
        /// <summary>
        /// 任务开始事件
        /// </summary>
        /// <param name="startInfo"></param>
        public void JobStart(JobStartInfo startInfo)
        {
            string content = string.Format("任务开始执行事件:方法名称：{0}；开始执行时间:{1}。", startInfo.Name, startInfo.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            Log(content);
        }
        /// <summary>
        /// 任务执行结束事件
        /// </summary>
        /// <param name="endInfo"></param>
        public void JobEnd(JobEndInfo endInfo)
        {
            string content = string.Format("任务结束执行事件:方法名称：{0};结束执行时间:{1};运行持续时间:{2};下次运行时间:{3}。", endInfo.Name, endInfo.StartTime.ToString("yyyy-MM-dd HH:mm:ss"), endInfo.Duration.TotalMilliseconds, (endInfo.NextRun.HasValue ? endInfo.NextRun.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
            Log(content);
        }
        /// <summary>
        /// 执行异常事件
        /// </summary>
        /// <param name="exInfo"></param>
        public void JobEx(JobExceptionInfo exInfo)
        {
            string content = string.Format("任务执行异常:方法名称：{0}；异常消息:{1}；发生时间:{2}。", exInfo.Name, exInfo.Exception.Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Log(content);
        }



        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BindTable();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopAll_Click(object sender, EventArgs e)
        {
            TaskManage.Stop();
            BindTable();
        }
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            TaskManage.Start(JobStart, JobEnd, JobEx);
            BindTable();
        }
        /// <summary>
        /// 绑定列表
        /// </summary>
        private void BindTable()
        {
            List<TaskConfig> list = TaskManage.LoadConfig();
            List<TaskView> result = new List<TaskView>();
            var runningTask = TaskManage.GetAllRunningTask();
            foreach (var item in list)
            {
                TaskView m = new TaskView();
                m.AssemblyPath = item.AssemblyPath;
                m.ClassPath = item.ClassPath;
                m.ExcuteTime = item.ExcuteTime;
                m.Id = item.Id;
                m.IntervalTime = item.IntervalTime;
                m.MethodName = item.MethodName;
                TaskType type = (TaskType)item.TaskType;
                m.TaskType = type.GetText();
                m.State = "运行停止";
                if (runningTask.Select(i => i.Name).Contains(m.MethodName))
                {
                    m.State = "运行中";
                }
                result.Add(m);
            }
            gvTables.DataSource = result;
        }
        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"D:\timerlog\");
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="content"></param>
        private void Log(string content)
        {


            //SaveLogs(content);
            SetText(content);
            LogHelper.Info(this.GetType(), content);

        }
        private void SetText(string text)
        {
            if (this.tbLog.InvokeRequired)
            {
                Action<string> callBack = SetText;
                this.Invoke(callBack, new object[] { text });
            }
            else
            {
                int count = this.tbLog.Lines.Count();
                if (count > 400)
                {
                    this.tbLog.Text = "";
                }
                this.tbLog.Text += text + "\r\n";
            }
        }

        private void btnCleanLog_Click(object sender, EventArgs e)
        {
            this.tbLog.Text = "";
        }
    }
}
