using System;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Collections;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;

namespace hc.Plat.WindowsServer
{
    partial class hcPlatWindowsServer : ServiceBase
    {
        private System.Collections.Hashtable hosts = new System.Collections.Hashtable();
        private Thread thread;
        public hcPlatWindowsServer()
        {
            InitializeComponent();
            thread = new Thread(new ThreadStart(this.ServerInit));
        }

        private void WriteTestLog(string LogText)
        {
            string FilePath = AppDomain.CurrentDomain.BaseDirectory;
            FilePath = FilePath + "mcWindowsService.txt";
            FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(DateTime.Now.ToString() + "  "+ LogText); 
            m_streamWriter.Flush();
            m_streamWriter.Close();
            fs.Close();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                WriteTestLog("");
                WriteTestLog("开始启动服务");
                //加载所有服务的基础类型；
                Type svcType = null;
                ServiceHost defaultHost = null;
                Configuration conf = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
                ServiceModelSectionGroup svcmod = (ServiceModelSectionGroup)conf.GetSectionGroup("system.serviceModel");

                foreach (ServiceElement el in svcmod.Services.Services)
                {
                    string sSpaceName = el.Name.Remove(el.Name.LastIndexOf('.'));
                    WriteTestLog("启动服务......" + el.Name);

                    WriteTestLog("启动服务......" + el.Name + "  GetType");
                    svcType = Type.GetType(el.Name + "," + sSpaceName);
                    WriteTestLog("启动服务......" + el.Name + "  GetType OK");

                    WriteTestLog("启动服务......" + el.Name + "  GetHost");
                     
                    defaultHost = new ServiceHost(svcType); 
                    string fullpath = AppDomain.CurrentDomain.BaseDirectory + "hcSinri.pfx";
                    string password = "sinri2015";
                    X509Certificate2 certificate = new X509Certificate2(fullpath, password);
                    defaultHost.Credentials.ServiceCertificate.Certificate = certificate;
                    
                    WriteTestLog("启动服务......" + el.Name + "  GetHost OK");


                    WriteTestLog("启动服务......" + el.Name + "  AddBehavior");
                    ServiceMetadataBehavior smb = defaultHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (smb == null)
                        defaultHost.Description.Behaviors.Add(new ServiceMetadataBehavior());

                    //暴露出元数据，以便能够让SvcUtil.exe自动生成配置文件
                    defaultHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                    WriteTestLog("启动服务......" + el.Name + "  AddBehavior  OK");

                    WriteTestLog("启动服务......" + el.Name + "  Open");

                    if (defaultHost.State != CommunicationState.Opening)
                    {
                        defaultHost.Open();
                    }

                    WriteTestLog("启动服务......" + el.Name + "  Open  OK");
                    hosts.Add(el.Name, defaultHost);

                }
                //初始化服务端缓存数据

                WriteTestLog("启动服务......启动线程");
                thread.Start();
                WriteTestLog("启动服务......启动线程  OK");
            } 
            catch (Exception e)
            {
                WriteTestLog("启动服务出错：" + e.Message);
                System.Diagnostics.EventLog evtlog = new System.Diagnostics.EventLog();
                evtlog.Log = "Application";
                evtlog.Source = ConfigurationManager.AppSettings["ServiceName"] ?? "hcPlatWindowsServer";
                evtlog.WriteEntry(e.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        { 
            foreach(DictionaryEntry de in hosts )
            {
                string KeyName = de.Key.ToString();
                ServiceHost sh = de.Value as ServiceHost;
                WriteTestLog("服务停止......"+ KeyName);
                sh.Abort();
                sh.Close();
                WriteTestLog("服务停止......" + KeyName+ "  OK");
            }
            hosts.Clear();
            thread.Abort();
        }
        public void ServerInit()
        {
            
        }
    }

}
     


