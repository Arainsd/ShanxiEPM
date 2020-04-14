/**------------------------------------------------------------------------------------
** 创建人:			王金强
** 创建日期:		2015.09.18 
** 描 述:			服务安装与卸载。 
** 版 本:           1.0.0.0
**------------------------------------------------------------------------------------
**
**************************************************************************************/
using System;
using System.Runtime.InteropServices;

namespace hc.Plat.ServerManager
{

    public class ServiceInstaller
    {

        #region DLLImport
        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);
        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();
        #endregion DLLImport


        #region Constants declaration.
        static int SC_MANAGER_CREATE_SERVICE = 0x0002;
        static int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        //int SERVICE_DEMAND_START = 0x00000003;
        static int SERVICE_ERROR_NORMAL = 0x00000001;
        static int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        static int SERVICE_QUERY_CONFIG = 0x0001;
        static int SERVICE_CHANGE_CONFIG = 0x0002;
        static int SERVICE_QUERY_STATUS = 0x0004;
        static int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
        static int SERVICE_START = 0x0010;
        static int SERVICE_STOP = 0x0020;
        static int SERVICE_PAUSE_CONTINUE = 0x0040;
        static int SERVICE_INTERROGATE = 0x0080;
        static int SERVICE_USER_DEFINED_CONTROL = 0x0100;
        static int SERVICE_ALL_ACCESS = (   STANDARD_RIGHTS_REQUIRED |
                                            SERVICE_QUERY_CONFIG |
                                            SERVICE_CHANGE_CONFIG |
                                            SERVICE_QUERY_STATUS |
                                            SERVICE_ENUMERATE_DEPENDENTS |
                                            SERVICE_START |
                                            SERVICE_STOP |
                                            SERVICE_PAUSE_CONTINUE |
                                            SERVICE_INTERROGATE |
                                            SERVICE_USER_DEFINED_CONTROL);
        static int SERVICE_AUTO_START = 0x00000002;
        static int SERVICE_USER_START = 0x00000003;
        #endregion Constants declaration.


        public bool GetServiceStart_Type(string svcName)
        {
            //写入注册表服务描述
            Microsoft.Win32.RegistryKey service;
            try
            {
                service = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + svcName, true);

                object obj = service.GetValue("Start");

                if (obj == null)
                {
                    return false;
                }
                if ((int)obj == SERVICE_AUTO_START)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SetServiceStart_Type(string svcName, bool AutoStart)
        {
            //写入注册表服务描述
            Microsoft.Win32.RegistryKey service;
            try
            {
                service = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + svcName, true);

                if (AutoStart)
                {
                    service.SetValue("Start", SERVICE_AUTO_START);
                }
                else
                {
                    service.SetValue("Start", SERVICE_USER_START);
                } 
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ///<summary>
        /// 安装和运行
        ///</summary>
        ///<param name="svcPath">程序路径.</param>
        ///<param name="svcName">服务名</param>
        ///<param name="svcDispName">服务显示名称.</param>
        ///<param name="svcDescription">服务描述.</param>
        ///<returns>服务安装是否成功.</returns>
        public bool InstallService(string svcPath, string svcName, string svcDispName, string svcUser, string svcPassword, string svcDescription)
        { 
            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt64() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, svcUser, svcPassword);
                    if (sv_handle.ToInt64() == 0)
                    {
                        CloseServiceHandle(sc_handle);
                        return false;
                    }
                    else
                    {
                        CloseServiceHandle(sc_handle);

                        //写入注册表服务描述
                        Microsoft.Win32.RegistryKey service ;
                        try
                        {
                            service = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + svcName,true);
                            service.SetValue("Description", svcDescription);
                        }
                        catch (Exception e)
                        {
                            throw e; 
                        }

                        return true;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        ///<summary>
        /// 反安装服务.
        ///</summary>
        ///<param name="svcName">服务名.</param>
        public bool UnInstallService(string svcName)
        {
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt64() != 0)
            {
                int DELETE = 0x10000;
                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                if (svc_hndl.ToInt64() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        CloseServiceHandle(sc_hndl);
                        return true;
                    }
                    else
                    {
                        CloseServiceHandle(sc_hndl);
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }
    }
}
