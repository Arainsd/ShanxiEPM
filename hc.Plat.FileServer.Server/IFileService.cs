using hc.Plat.Common.Global;
using hc.Plat.FileServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.FileServer.Server
{
    [ServiceContract]
    [ServiceKnownType(typeof(FileConfig))]
    [ServiceKnownType(typeof(Files))]
    [ServiceKnownType(typeof(TempFiles))]
    [ServiceKnownType(typeof(DownloadLog))]
    [ServiceKnownType(typeof(StaticResource))]
    public interface IFileService
    {
        [OperationContract]
        Result<List<FileConfig>> GetConfig(string app);

        [OperationContract]
        Result<Files> AddFile(Files model);

        [OperationContract]
        Result<string> DeleteFiles(List<int> ids);

        [OperationContract]
        Result<Files> GetFile(int id);
        [OperationContract]
        Result<Files> DownloadFile(string guid);

        [OperationContract]
        Result<TempFiles> AddTempFile(TempFiles model);

        [OperationContract]
        Result<string> DeleteTempFile(List<int> ids);


        [OperationContract]
        Result<List<TempFiles>> GetTempFiles(string app, string fileName, string fileLastModifiedDate, long fileSize);

        [OperationContract]
        Result<FileGuid> AddFileGuid(FileGuid model);

        [OperationContract]
        Result<FileGuid> GetFileGuid(int id);

        [OperationContract]
        Result<string> DeleteFileGuid(FileGuid model);


        [OperationContract]
        Result<DownloadLog> AddDownloadLog(DownloadLog model);

        [OperationContract]
        Result<StaticResource> AddStaticResource(StaticResource model);

        /// <summary>
        /// 检查token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result<string> CheckToken(string token);
        

    }
}
