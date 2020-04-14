using hc.Plat.FileServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using hc.Plat.Common.Global;
using hc.Plat.FileServer.Data;

namespace hc.Plat.FileServer.ClientProxy
{
    /// <summary>
    /// 文件入库代理
    /// </summary>
    public class ClientProxyFileServer : ClientBase<IFileService>, IFileService
    {
        public ClientProxyFileServer(hc.Plat.Common.Global.ClientProxyExType cpet)
        {

            //传输当前用户的信息；
            ApplicationContext.Current.UserID = cpet.UserID;
            ApplicationContext.Current.WebIP = cpet.IP_WebServer;
            ApplicationContext.Current.ClientIP = cpet.IP_Client;


            /*以下密码是用作在应用服务器中使用程序验证密码的作用*/
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            string user = "";
            string pass = "";
            string msg = DesTool.LoadCertUserPass(FilePath, out user, out pass);
            if (msg != "")
            {
                throw new Exception(msg);
            }
            ClientCredentials.UserName.UserName = user;
            ClientCredentials.UserName.Password = pass;
            /*OK*/
        }

        public Result<List<FileConfig>> GetConfig(string app)
        {
            return base.Channel.GetConfig(app);
        }

        public Result<Files> AddFile(Files model)
        {
            return base.Channel.AddFile(model);
        }

        public Result<string> DeleteFiles(List<int> ids)
        {
            return base.Channel.DeleteFiles(ids);
        }

        public Result<Files> GetFile(int id)
        {
            return base.Channel.GetFile(id);
        }
        public Result<Files> DownloadFile(string guid)
        {
            return base.Channel.DownloadFile(guid);
        }
        public Result<TempFiles> AddTempFile(TempFiles model)
        {
            return base.Channel.AddTempFile(model);
        }

        public Result<string> DeleteTempFile(List<int> ids)
        {
            return base.Channel.DeleteTempFile(ids);
        }

        public Result<List<TempFiles>> GetTempFiles(string app, string fileName, string fileLastModifiedDate, long fileSize)
        {
            return base.Channel.GetTempFiles(app, fileName, fileLastModifiedDate, fileSize);
        }

        public Result<FileGuid> AddFileGuid(FileGuid model)
        {
            return base.Channel.AddFileGuid(model);
        }

        public Result<string> DeleteFileGuid(FileGuid model)
        {
            return base.Channel.DeleteFileGuid(model);
        }

        public Result<FileGuid> GetFileGuid(int id)
        {
            return base.Channel.GetFileGuid(id);
        }

        public Result<DownloadLog> AddDownloadLog(DownloadLog model)
        {
            return base.Channel.AddDownloadLog(model);
        }

        public Result<StaticResource> AddStaticResource(StaticResource model)
        {
            return base.Channel.AddStaticResource(model);
        }

        /// <summary>
        /// 检查token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Result<string> CheckToken(string token)
        {
            return base.Channel.CheckToken(token);
        }
    }
}
