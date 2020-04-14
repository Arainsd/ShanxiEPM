using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{
    [Serializable]
    public class ClientProxyExType
    {
        /// <summary>
        /// 当前账户ID；
        /// </summary>
        public string UserID { get; set; }
        
        /// <summary>
        /// 当前账户ID；
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 当前客户端（IE or SpecialApp）IP；
        /// 需要通过js获取，并且通过Context传输到Web的Server Handler；
        /// 专用客户端IP；需要调用段复制；
        /// </summary>
        public string IP_Client { get; set; }

        /// <summary>
        /// Web服务器地址；【自动获取，并且只能在Web服务器上使用此IP】
        /// </summary>
        public string IP_WebServer { get; set; }


        public ClientProxyExType()
        {
            IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
            IP_Client = IP_WebServer;
        }

        public ClientProxyExType(string aUserID, string aClientIP)
        {
            UserID = aUserID;
            IP_Client = aClientIP;
            IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
        }

        public ClientProxyExType(string aUserID)
        {
            UserID = aUserID;
            IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
            IP_Client = IP_WebServer;
        }
        
        /// <summary>
        /// 当前登录用户信息，默认为空
        /// </summary>
        public object CurrentUser { get; set; } = null;
    }
}
