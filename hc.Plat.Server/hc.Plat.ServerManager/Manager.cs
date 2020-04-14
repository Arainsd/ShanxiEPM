/**------------------------------------------------------------------------------------
** 创建人:			王金强
** 创建日期:		2015.09.18 
** 描 述:			服务管理器。 
** 版 本:           1.0.0.0
**------------------------------------------------------------------------------------
**
**************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Configuration;
using System.Linq;

namespace hc.Plat.ServerManager
{
    public partial class ManagerForm : Form
    {
        private bool RequireCloseForm = false;

        private string svcPath = AppDomain.CurrentDomain.BaseDirectory + "hc.Plat.WindowsServer.exe";
        private string svcName;
        private string svcDispName;
        private string svcDescript;
        private ServiceController sc;

        //private DateTime ServiceEndDate;
        //private int ServiceEndDays;

        public ManagerForm()
        {
            InitializeComponent();
            timer.Stop();

            tbMsg.Text = "正在刷新服务状态，请稍后......";

            string FileName = AppDomain.CurrentDomain.BaseDirectory + "Down.ico";
            if (System.IO.File.Exists(FileName))
            {
                MainnotifyIcon.Icon = Icon.ExtractAssociatedIcon("Down.ico");
                this.Icon = Icon.ExtractAssociatedIcon("Down.ico");
            }

            
            if (!System.IO.File.Exists(svcPath))
            {
                return;
            }

            Configuration cf = ConfigurationManager.OpenExeConfiguration(svcPath);
            svcName = cf.AppSettings.Settings["ServiceName"] != null ? cf.AppSettings.Settings["ServiceName"].Value : "hcPlatWindowsServer";
            svcDispName = cf.AppSettings.Settings["ServiceDispName"] != null ? cf.AppSettings.Settings["ServiceDispName"].Value : "hcPlatWindowsServer Service";
            svcDescript = cf.AppSettings.Settings["ServiceDescript"] != null ? cf.AppSettings.Settings["ServiceDescript"].Value : "hcPlatWindowsServer Service For UserUI";
             

            labServiceName.Text = svcDispName;
            MainnotifyIcon.Text = svcDispName;

            SetButtonState();

            timer.Start();

            
        }

        private bool HasResetIcon = false;
        private bool InRefreshStateing = false;
        private bool HasGetRegistry = true;

        private void SetButtonState()
        {
            if (InRefreshStateing) { return; }

            InRefreshStateing = true;
            Application.DoEvents();

            try
            {
                tsmClose.Enabled = true;
                cmsMain.Enabled = true;
                this.tsmShow.Enabled = !this.Visible;
                this.tsmHide.Enabled = this.Visible;
                try
                {
                    if (!this.HasGetRegistry)
                    {
                        this.btnStart.Enabled = false;
                        tbMsg.Text = "服务未注册！";
                    }
                    else
                    {
                        sc = new ServiceController(svcName);
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            tbMsg.Text = "服务已经启动，正在运行。";
                            StateProgressBar.Style = ProgressBarStyle.Marquee;
                            this.btnInstall.Enabled = false;
                            this.btnUnInstall.Enabled = false;
                            this.btnStart.Enabled = false;
                            this.tsmStart.Enabled = false;
                            this.btnStop.Enabled = true;
                            this.tsmStop.Enabled = true;
                            btnImportKey.Enabled = false;
                            btnDBTest.Enabled = false;
                            if (!HasResetIcon)
                            {
                                string FileName = AppDomain.CurrentDomain.BaseDirectory + "Run.ico";
                                if (System.IO.File.Exists(FileName))
                                {
                                    MainnotifyIcon.Icon = Icon.ExtractAssociatedIcon("Run.ico");
                                    this.Icon = Icon.ExtractAssociatedIcon("Run.ico"); 
                                }
                                HasResetIcon = true;
                            }
                        }
                        else
                        {
                            tbMsg.Text = "服务未启动";
                            StateProgressBar.Style = ProgressBarStyle.Continuous;
                            this.btnInstall.Enabled = false;
                            this.btnUnInstall.Enabled = true;
                            this.btnStart.Enabled = true;
                            this.tsmStart.Enabled = true;
                            this.btnStop.Enabled = false;
                            this.tsmStop.Enabled = false;
                            btnImportKey.Enabled = true; 
                            btnDBTest.Enabled = true;
                        }
                        tsmClose.Enabled = true;
                        this.cbAutoStart.Enabled = true;

                        ServiceInstaller si = new ServiceInstaller();
                        this.cbAutoStart.Checked = si.GetServiceStart_Type(svcName);
                    }

                }
                catch (Exception ex)
                {
                    //if (ex.InnerException.Message == "指定的服务未安装。")
                    //{
                    this.btnInstall.Enabled = true;
                    this.cbAutoStart.Enabled = false;
                    this.btnUnInstall.Enabled = false;
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = false;
                    this.tsmStart.Enabled = false;
                    this.tsmStop.Enabled = false;
                    btnDBTest.Enabled = false;
                    btnImportKey.Enabled = false;
                    //}

                    this.MainnotifyIcon.BalloonTipText = ex.Message;
                    this.MainnotifyIcon.ShowBalloonTip(1000);
                }
            }
            finally
            {
                InRefreshStateing = false;
            }
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void StartService()
        {
            MainnotifyIcon.BalloonTipText = "正在启动服务......";
            this.MainnotifyIcon.ShowBalloonTip(1000);
            this.Cursor = Cursors.WaitCursor;
            StateProgressBar.Style = ProgressBarStyle.Marquee;
            if (sc.Status != ServiceControllerStatus.Running)
            {
                try
                {
                    sc.Start();

                    string FileName = AppDomain.CurrentDomain.BaseDirectory + "Run.ico";
                    if (System.IO.File.Exists(FileName))
                    {
                        MainnotifyIcon.Icon = Icon.ExtractAssociatedIcon("Run.ico");
                        this.Icon = Icon.ExtractAssociatedIcon("Run.ico");
                    }
                    HasResetIcon = true;

                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));
                    MainnotifyIcon.BalloonTipText = "服务运行正常！";
                    this.MainnotifyIcon.ShowBalloonTip(1000);
                }
                catch (Exception exp)
                {
                    MainnotifyIcon.BalloonTipText = "服务运行失败！";
                    MessageBox.Show(exp.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.MainnotifyIcon.ShowBalloonTip(1000);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void StopService()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();

                    string FileName = AppDomain.CurrentDomain.BaseDirectory + "Down.ico";
                    if (System.IO.File.Exists(FileName))
                    {
                        MainnotifyIcon.Icon = Icon.ExtractAssociatedIcon("Down.ico");
                        this.Icon = Icon.ExtractAssociatedIcon("Down.ico"); 
                    } 
                    HasResetIcon = false;

                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    StateProgressBar.Style = ProgressBarStyle.Continuous;
                    MainnotifyIcon.BalloonTipText = "服务已经关闭！";
                    this.MainnotifyIcon.ShowBalloonTip(1000);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        private void btnInstall_Click(object sender, EventArgs e)
        { 
            ServiceInstaller si = new ServiceInstaller();
            if (si.InstallService(svcPath, svcName, svcDispName, null, null, svcDescript)) //sName, sPwd))
            {
                //SetButtonState();
            }
            else
            {
                MessageBox.Show("服务安装不成功，请检查管理员权限！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            ServiceInstaller si = new ServiceInstaller();
            if (si.UnInstallService(svcName))
            {
                sc = null;
                MainnotifyIcon.BalloonTipText = "服务已经关闭！";
                this.MainnotifyIcon.ShowBalloonTip(1000);
                MessageBox.Show("服务卸载完成，窗体将关闭！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.RequireCloseForm = true;
                Close();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SetButtonState();
        }

        private void tsmStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void tsmStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void tsmClose_Click(object sender, EventArgs e)
        {
            RequireCloseForm = true;
            Close();
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!RequireCloseForm)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    this.Visible = false;
                }
            }
        }

        private void tsmShow_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Activate();
        }

        private void tsmHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

         
        private void cbAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            ServiceInstaller si = new ServiceInstaller();
            si.SetServiceStart_Type(svcName, cbAutoStart.Checked);
        }
         
        private void MainnotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.Activate();
        }

        private void btnImportKey_Click(object sender, EventArgs e)
        {
             
        } 
    }
}