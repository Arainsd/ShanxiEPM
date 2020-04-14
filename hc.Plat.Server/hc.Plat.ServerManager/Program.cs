using System;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace hc.Plat.ServerManager
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll ", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public const int SW_RESTORE = 9;
        public static IntPtr formhwnd;

        private static string svcPath = AppDomain.CurrentDomain.BaseDirectory + @"\hc.Plat.WindowsServer.exe";
        private static string svcName;
        private static string svcDispName;
        private static string svcDescript;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!System.IO.File.Exists(svcPath))
            {
                MessageBox.Show("当前目录下没有找到应用服务‘hcPlatWindowsServer.exe’！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string proc = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);
            if (processes.Length <= 1)
            {

                try
                {
                    Configuration cf = ConfigurationManager.OpenExeConfiguration(svcPath);
                    svcName = cf.AppSettings.Settings["ServiceName"] != null ? cf.AppSettings.Settings["ServiceName"].Value : "hcPlatWindowsServer";
                    svcDispName = cf.AppSettings.Settings["ServiceDispName"] != null ? cf.AppSettings.Settings["ServiceDispName"].Value : "hcPlatWindowsServer";
                    svcDescript = cf.AppSettings.Settings["ServiceDescript"] != null ? cf.AppSettings.Settings["ServiceDescript"].Value : "hcPlatWindowsServer";
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Application.Run(new ManagerForm());
            }
            else
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].Id != Process.GetCurrentProcess().Id)
                    {
                        if (processes[i].MainWindowHandle.ToInt64() == 0)
                        {
                            formhwnd = FindWindow(null, "ManagerForm");
                            ShowWindow(formhwnd, 1);
                            SwitchToThisWindow(formhwnd, true);
                        }
                        else
                        {
                            SwitchToThisWindow(processes[i].MainWindowHandle, true);
                            MessageBox.Show("服务管理器已经运行，请从右下角图标启动！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

        }
    }
}
 