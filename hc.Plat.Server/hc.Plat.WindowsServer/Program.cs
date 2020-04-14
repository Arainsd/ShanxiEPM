using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Net;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading; 
 
namespace hc.Plat.WindowsServer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new hcPlatWindowsServer()
            };
            ServiceBase.Run(ServicesToRun);           
        }  
        
    }
}
