using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBConnectionDesTools
{
    public partial class NETDB : Form
    {
        public NETDB()
        {
            InitializeComponent();
        }
        private void NETDB_Load(object sender, EventArgs e)
        {

        }

        public enum CmdType
        {
            /// <summary>
            /// 创建秘钥容器
            /// </summary>
            CreateKeyContainer = 1,
            /// <summary>
            /// 加密配置节
            /// </summary>
            EncryptSection = 2,
            /// <summary>
            /// 解密config
            /// </summary>
            DecryptConfig = 3,
            /// <summary>
            /// 导出秘钥
            /// </summary>
            ExportKeyContainer = 4,
            /// <summary>
            /// 删除秘钥容器
            /// </summary>
            DeleteKeyContainer = 5,
            /// <summary>
            /// 导入秘钥容器
            /// </summary>
            ImportKeyContainer = 6

        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public string GetCmd(CmdType cmdType, params string[] paramArray)
        {
            var result = string.Format(CmdList[cmdType], paramArray);
            return result;

        }
        /// <summary>
        /// 命令列表
        /// </summary>
        public Dictionary<CmdType, string> CmdList
        {
            get
            {
                Dictionary<CmdType, string> dictionary = new Dictionary<CmdType, string>();
                dictionary.Add(CmdType.CreateKeyContainer, "aspnet_regiis -pc \"{0}\" -exp");
                dictionary.Add(CmdType.EncryptSection, "aspnet_regiis -pef \"{0}\" \"{1}\" -prov \"{2}\"");
                dictionary.Add(CmdType.DecryptConfig, "aspnet_regiis -pdf \"{0}\" \"{1}\"");
                dictionary.Add(CmdType.ExportKeyContainer, "aspnet_regiis -px \"{0}\" \"{1}\" -pri");
                dictionary.Add(CmdType.DeleteKeyContainer, "aspnet_regiis -pz \"{0}\"");
                dictionary.Add(CmdType.ImportKeyContainer, "aspnet_regiis -pi \"{0}\" \"{1}\"");
                return dictionary;
            }
        }


        /// <summary>
        /// 创建秘钥容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateKeyContainer_Click(object sender, EventArgs e)
        {
            string key = txtKeyName.Text.Trim();
            string cmd = GetCmd(CmdType.CreateKeyContainer, key);
            RunCmd(cmd);
        }
        /// <summary>
        /// 加密连接字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            string providerName = txtKeyName.Text.Trim() + "Provider";
            string path = txtConfigFile.Text;
            string oldPath = path + "/App.config";
            string newPath = path + "/Web.config";
            //将app.config修改为web.config
            MoveOrRename(oldPath, newPath);
            string cmd = GetCmd(CmdType.EncryptSection, "connectionStrings", path, providerName);
            RunCmd(cmd);
            //将web.config修改为app.config
            MoveOrRename(newPath, oldPath);

        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string path = txtConfigFile.Text;
            string oldPath = path + "/App.config";
            string newPath = path + "/Web.config";
            //将app.config修改为web.config
            MoveOrRename(oldPath, newPath);
            string cmd = GetCmd(CmdType.DecryptConfig, "connectionStrings", path);
            RunCmd(cmd);
            //将web.config修改为app.config
            MoveOrRename(newPath, oldPath);
        }
        /// <summary>
        /// 删除秘钥容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteKeyContainer_Click(object sender, EventArgs e)
        {
            string key = txtKeyName.Text.Trim();
            string cmd = GetCmd(CmdType.DeleteKeyContainer, key);
            RunCmd(cmd);
        }
        public void MoveOrRename(string oldPath, string targetPath)
        {
            System.IO.File.Move(oldPath, targetPath);//2个文件在不同目录则是移动,如果在相同目录下则是重命名
        }
        /// <summary>
        /// 运行cmd命令
        /// </summary>
        /// <param name="cmdStr">执行命令行参数</param>
        private void RunCmd(string cmdStr)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            string vsTools = txtToolsFile.Text;
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(vsTools + "&exit");

            Thread.Sleep(5000);
            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(cmdStr + "&exit");

            p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
            //获取cmd窗口的输出信息
            string output = p.StandardOutput.ReadToEnd();


            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            txtLog.Text = output;
        }
        /// <summary>
        /// 导出秘钥容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportKeyContainer_Click(object sender, EventArgs e)
        {
            string key = txtKeyName.Text.Trim();
            string exportPath = AppDomain.CurrentDomain.BaseDirectory + "/" + key + ".xml";
            string cmd = GetCmd(CmdType.ExportKeyContainer, key, exportPath);
            RunCmd(cmd);
            txtLog.Text += "\r\n秘钥文件导出成功：" + exportPath;
        }
        /// <summary>
        /// 导入秘钥
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportKeyContainer_Click(object sender, EventArgs e)
        {
            string key = txtKeyName.Text.Trim();
            string importPath = AppDomain.CurrentDomain.BaseDirectory  + key + ".xml";
            if (!System.IO.File.Exists(importPath))
            {
                MessageBox.Show("程序根目录下不存在秘钥文件");
            }
            else
            {
                string cmd = GetCmd(CmdType.ImportKeyContainer, key, importPath);
                RunCmd(cmd);
            }

        }
       
    }
}
