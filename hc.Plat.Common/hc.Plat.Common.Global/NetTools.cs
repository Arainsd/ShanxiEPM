using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace hc.Plat.Common.Global
{
    public class NetTools
    {
        /// <summary>
        /// 获取IP4的地址
        /// </summary>
        /// <returns></returns>
        static public string GetLocalMachineIP4()
        {
            IPAddress ipAddr = null;
            //if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            //{
            //    System.Net.NetworkInformation.NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            //    foreach (System.Net.NetworkInformation.NetworkInterface ni in interfaces)
            //    {
            //        //联网的IP
            //        if (ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up & ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel & ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            //        {
            //            System.Net.NetworkInformation.IPInterfaceProperties property = ni.GetIPProperties();
            //            foreach (System.Net.NetworkInformation.UnicastIPAddressInformation ip in
            //                property.UnicastAddresses)
            //            {
            //                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //                {
            //                    ipAddr = ip.Address;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
            if (ipAddr != null)
            {
                return ipAddr.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
