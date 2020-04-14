using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.Plat.Common.Global;
using hc.Plat.FileServer.Data;
using Newtonsoft.Json;
using hc.Plat.Cache.Helper;

namespace hc.Plat.FileServer.Server
{
    public class FileService : IFileService
    {
        private HCPlat_FileServerEntities dbContext = new HCPlat_FileServerEntities();
        IDataCache cacheHelper = new MemCached();
        /// <summary>
        /// 获取应用程序上传配置信息
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public Result<List<FileConfig>> GetConfig(string app)
        {
            Result<List<FileConfig>> result = new Result<List<FileConfig>>();
            try
            {
                var configPath = AppDomain.CurrentDomain.BaseDirectory + "FileConfig.json";
                string strJson = File.ReadAllText(configPath);
                var list = JsonConvert.DeserializeObject<List<FileConfig>>(strJson);

                var resultList = list.FindAll(i => i.App == app);
                result.Flag = EResultFlag.Success;
                result.Data = resultList;
                result.Exception = null;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConfig");
            }
            return result;

        }
        /// <summary>
        /// 文件存储入库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Files> AddFile(Files model)
        {
            Result<Files> result = new Result<Files>();
            try
            {
                dbContext.Files.Add(model);
                dbContext.SaveChanges();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddFile");
            }
            return result;
        }
        /// <summary>
        /// 根据id批量删除文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<string> DeleteFiles(List<int> ids)
        {
            Result<string> result = new Result<string>();
            try
            {
                var list = dbContext.Files.Where(i => ids.Contains(i.Id));
                foreach (var model in list)
                {
                    model.IsDelete = true;
                    dbContext.Entry(model).State = EntityState.Modified;

                }
                dbContext.SaveChanges();
                result.Data = "删除成功";
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteFiles");
            }
            return result;
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Files> GetFile(int id)
        {
            Result<Files> result = new Result<Files>();
            try
            {
                var model = dbContext.Files.FirstOrDefault(i => i.Id == id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFile");
            }
            return result;
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Files> DownloadFile(string guid)
        {
            Result<Files> result = new Result<Files>();
            try
            {
                var model = dbContext.Files.FirstOrDefault(i => i.Guid == guid);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DownloadFile");
            }
            return result;
        }
        /// <summary>
        /// 分块临时文件存储
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<TempFiles> AddTempFile(TempFiles model)
        {
            Result<TempFiles> result = new Result<TempFiles>();
            try
            {
                dbContext.TempFiles.Add(model);
                dbContext.SaveChanges();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTempFile");
            }
            return result;
        }
        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<string> DeleteTempFile(List<int> ids)
        {
            Result<string> result = new Result<string>();
            try
            {
                var list = dbContext.TempFiles.Where(i => ids.Contains(i.Id));
                dbContext.TempFiles.RemoveRange(list);
                dbContext.SaveChanges();
                result.Data = "删除成功";
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteFiles");
            }
            return result;
        }
        /// <summary>
        /// 获取临时文件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="fileName"></param>
        /// <param name="fileLastModifiedDate"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public Result<List<TempFiles>> GetTempFiles(string app, string fileName, string fileLastModifiedDate, long fileSize)
        {
            Result<List<TempFiles>> result = new Result<List<TempFiles>>();
            try
            {
                var list = dbContext.TempFiles.Where(i => i.App == app && i.FileName == fileName && i.FileLastModifiedDate == fileLastModifiedDate && i.FileSize == fileSize).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTempFiles");
            }
            return result;
        }
        /// <summary>
        /// 添加文件对比信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<FileGuid> AddFileGuid(FileGuid model)
        {
            Result<FileGuid> result = new Result<FileGuid>();
            try
            {
                dbContext = new HCPlat_FileServerEntities();
                dbContext.FileGuid.Add(model);
                dbContext.SaveChanges();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddFileGuid");
            }
            return result;
        }
        /// <summary>
        /// 删除文件对比信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<string> DeleteFileGuid(FileGuid model)
        {
            Result<string> result = new Result<string>();
            try
            {
                var m = dbContext.FileGuid.Where(i => i.Id == model.Id);
                dbContext.FileGuid.RemoveRange(m);
                dbContext.SaveChanges();
                result.Data = "删除成功";
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteFileGuid");
            }
            return result;
        }
        /// <summary>
        /// 获取文件对比信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<FileGuid> GetFileGuid(int id)
        {
            Result<FileGuid> result = new Result<FileGuid>();
            try
            {
                var model = dbContext.FileGuid.FirstOrDefault(i => i.Id == id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFileGuid");
            }
            return result;
        }

        /// <summary>
        /// 添加文件下载记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<DownloadLog> AddDownloadLog(DownloadLog model)
        {
            Result<DownloadLog> result = new Result<DownloadLog>();
            try
            {
                dbContext.DownloadLog.Add(model);
                dbContext.SaveChanges();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddDownloadLog");
            }
            return result;
        }
        /// <summary>
        /// 静态资源文件上传
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<StaticResource> AddStaticResource(StaticResource model)
        {
            Result<StaticResource> result = new Result<StaticResource>();
            try
            {
                dbContext.StaticResource.Add(model);
                dbContext.SaveChanges();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddStaticResource");
            }
            return result;
        }

        /// <summary>
        /// 检查token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Result<string> CheckToken(string token)
        {
            Result<string> result = new Result<string>();
            try
            {
                string code = GetCache<string>(token);
                result.Data = code;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CheckToken");
            }
            return result;
        }

        /// <summary>
        /// <summary>
        /// 获取缓存,不加入CacheKeyAll管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        protected T GetCache<T>(string cacheKey)
        {
            T obj = default(T);
            string val = "";
            try
            {
                obj = cacheHelper.Get<T>(cacheKey);
                val = obj.ToString();
                return obj;
            }
            catch (Exception ex)
            {
                LogHelper.Info(this.GetType(), "缓存错误,出错Key:" + cacheKey + ";出错value;" + val + "|" + ex.Message);
            }
            return default(T);
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="isStartsWith"></param>
        protected void DeleteCache(string cacheKey)
        {
            cacheHelper.Delete(cacheKey);

        }
        
    }
}
