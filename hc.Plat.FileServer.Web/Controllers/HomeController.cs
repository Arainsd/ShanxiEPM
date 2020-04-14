using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.FileServer.ClientProxy;
using hc.Plat.FileServer.Data;
using System.Security.Cryptography;
using hc.Plat.FileServer.Web.Models;
using Newtonsoft.Json;
using Com.Itrus.CryptoRole;
using Com.Itrus.Util;
using Com.Itrus.Svm;
using Org.BouncyCastle.Utilities.Encoders;

namespace hc.Plat.FileServer.Web.Controllers
{
    /// <summary>
    /// 对应客户端需要在js和服务端分别配置文件服务器url常量
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult TestUpload()
        {
            return View();
        }
        private static string deleteHour = System.Configuration.ConfigurationManager.AppSettings["FileDeleteHour"];
        public ActionResult Index()
        {
            //var d = DataCache.InstanceRedis.Set("myredis", "hello World");
            //using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
            //{
            //    var config = proxy.GetConfig("epm");
            //    DataCache.InstanceRedis.Set("aa", config);
            //}
            //var s = DataCache.InstanceRedis.Get<List<string>>("aa");
            //return RedirectToAction("Index", "Files");
            return View();
        }
        //[Authorize(Roles = "admin")]
        public ActionResult Test()
        {
            return View();
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public JsonResult GetConfigList(string app = "epm")
        {
            using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
            {
                var config = proxy.GetConfig(app);
                return Json(config, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 代理信息获取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private ClientProxyExType ProxyEx(HttpRequestBase request)
        {
            ClientProxyExType cpet = Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null)
            {
                cpet = new ClientProxyExType();
                cpet.UserID = "admin";
                cpet.IP_Client = request.UserHostAddress;
                cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
                Session[ConstStr_Session.CurrentProxyExType] = cpet;

            }
            return cpet;

        }
        /// <summary>
        /// 是否是断点续传
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileSize"></param>
        /// <param name="lastModifiedDate"></param>
        /// <returns></returns>
        public JsonResult IsContinue(string name, long fileSize, string lastModifiedDate, string app = "epm")
        {
            using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
            {
                //存储文件信息以便对比
                FileGuid fg = new FileGuid();
                fg.App = app;
                fg.FId = Guid.NewGuid().ToString();
                fg.FileSize = fileSize;
                fg.LastModifiedDate = lastModifiedDate;
                fg.Name = name;
                fg.RecordTime = DateTime.Now;
                var result = proxy.AddFileGuid(fg);
                fg = result.Data;


                var tempFiles = proxy.GetTempFiles(app, name, lastModifiedDate, fileSize);
                //存在临时文件，代表可以断点续传
                if (tempFiles.Data != null && tempFiles.Data.Count() > 0)
                {
                    //依据块顺序
                    var fileList = tempFiles.Data.OrderBy(i => i.Chunk);
                    var tempModel = tempFiles.Data.FirstOrDefault();
                    //如果超过两天，清除临时文件，重新上传
                    var ts = DateTime.Now - tempModel.UploadTime;

                    if (ts.Hours > deleteHour.ToInt32Req())
                    {
                        //删除临时块文件信息
                        proxy.DeleteTempFile(fileList.Select(i => i.Id).ToList());
                        return Json(new
                        {
                            size = 0,
                            flag = true,
                            fid = fg.Id
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var tempPath = tempModel.ChunkPath.Replace(tempModel.ChunkName, "") + "Temp_" + tempModel.FileName;

                    using (FileStream fsw = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        BinaryWriter bw = new BinaryWriter(fsw);
                        //合并文件，计算临时文件大小
                        foreach (var temp in fileList)
                        {
                            bw.Write(System.IO.File.ReadAllBytes(temp.ChunkPath));//打开一个文件读取流信息，将其写入新文件
                            bw.Flush(); //清理缓冲区
                        }

                    }
                    FileInfo file = new FileInfo(tempPath);
                    return Json(new
                    {
                        size = (int)file.Length,
                        flag = true,
                        fid = fg.Id
                    }, JsonRequestBehavior.AllowGet);
                    //清空临时为了计算大小合并的文件
                }
                return Json(new
                {
                    size = 0,
                    flag = true,
                    fid = fg.Id
                }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// 分块断点续传,多文件
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private object UploadFileByChunk(HttpRequestBase request)
        {
            //断点续传
            string app = Request["app"];
            string name = Request["name"];
            int fId = Request["fId"].ToInt32Req();
            int chunk = Request["chunk"].ToInt32Req(); //当前分块
            int chunks = Request["chunks"].ToInt32Req();//总的分块数量

            // 是否上传模型
            string bim = request["isbim"];
            bool isbim = "true".Equals((string.IsNullOrWhiteSpace(bim) ? "false" : bim).ToLower());

            foreach (string file in Request.Files)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
                    {
                        //取得文件对比信息
                        FileGuid fg = proxy.GetFileGuid(fId).Data;
                        HttpPostedFileBase postedFile = Request.Files[file]; //获取客户端上载文件的集合
                        string fileName = Path.GetFileNameWithoutExtension(name); //获取客户端上传文件的名称
                        string extensionName = Path.GetExtension(name).ToLower(); //获取客户端上传文件的后缀

                        string newFileName = name;
                        //获取文件存储目录
                        var configList = proxy.GetConfig(app);
                        FileConfig config = null;
                        if (isbim)
                        {
                            config = configList.Data.FirstOrDefault(i => i.FileTypeDirectory == "model");
                        }
                        else {
                            config = configList.Data.FirstOrDefault(i => i.FileTypeExtension.Contains(extensionName) && i.FileTypeDirectory != "model");
                        }

                        if (config == null)
                        {
                            config = configList.Data.FirstOrDefault(i => i.FileTypeExtension.Contains(".*"));
                            //return (new { flag = false, result = "不允许上传此类型的文件" });
                        }
                        var pathList = CreatePath(config);
                        string tempPath = pathList[0];
                        string filePath = pathList[1];

                        //分块
                        if (chunks > 1)
                        {
                            newFileName = chunk + "_" + GetRamCode() + "_" + fileName + extensionName; //按文件块重命名块文件
                        }
                        string chunkPath = tempPath + "\\" + newFileName; //将块文件和临时文件夹路径绑定
                        postedFile.SaveAs(chunkPath); //保存临时块上载文件内容

                        //临时块文件入库
                        FileInfo chunkFile = new FileInfo(chunkPath);
                        TempFiles tf = new TempFiles();
                        tf.App = app;
                        tf.Chunk = chunk;
                        tf.ChunkName = newFileName;
                        tf.ChunkPath = chunkPath;
                        tf.Chunks = chunks;
                        tf.ChunkSize = (int)chunkFile.Length;
                        tf.FileGuid = Guid.NewGuid().ToString();
                        tf.FileLastModifiedDate = fg.LastModifiedDate;
                        tf.FileName = name;
                        tf.FileSize = fg.FileSize;
                        tf.UploadTime = DateTime.Now;
                        tf.RequetURL = Request.UrlReferrer.ToString();
                        tf.IP = request.UserHostAddress;
                        tf.Browser = request.Browser.Browser;
                        //TODO:可以在此处获取每一个块文件的md5在检测是否可以续传时返回少一个块文件的长度，然后对比续传的第一个块的md5来实现去重
                        tf = proxy.AddTempFile(tf).Data;

                        //最后一个块，执行合并
                        if (chunks > 1 && chunk + 1 == chunks)
                        {

                            var tempFiles = proxy.GetTempFiles(app, name, fg.LastModifiedDate, fg.FileSize);
                            var fileList = tempFiles.Data.OrderBy(i => i.Chunk);
                            string reName = Guid.NewGuid().ToString() + name;
                            string fullPath = filePath + "\\" + reName;
                            using (FileStream fsw = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                            {
                                BinaryWriter bw = new BinaryWriter(fsw);
                                // 遍历文件合并 
                                foreach (var temp in fileList)
                                {
                                    //打开一个文件读取流信息，将其写入新文件
                                    bw.Write(System.IO.File.ReadAllBytes(temp.ChunkPath));
                                    System.IO.File.Delete(temp.ChunkPath); //删除临时块文件信息，以避免临时文件越来越大
                                    bw.Flush(); //清理缓冲区
                                }
                            }
                            //正式文件入库
                            Files sf = new Files();
                            sf.App = app;
                            sf.Day = DateTime.Now.ToString("yyyyMMdd").ToInt32Req(); ;
                            sf.Extension = extensionName;
                            sf.FilePath = fullPath;
                            sf.FileSize = fg.FileSize;
                            sf.FileType = config.FileTypeName;
                            sf.Guid = fg.FId;
                            sf.IsDelete = false;
                            sf.LastModifiedDate = fg.LastModifiedDate;
                            sf.Month = DateTime.Now.ToString("yyyyMM").ToInt32Req(); ;
                            sf.Name = name;
                            sf.ReName = reName;
                            sf.RequetURL = Request.UrlReferrer.ToString();
                            sf.UploadTime = DateTime.Now;
                            sf.Year = DateTime.Now.Year;
                            sf.UserDescription = "";
                            sf.IP = request.UserHostAddress;
                            sf.Browser = request.Browser.Browser;
                            string md5 = GetMD5HashFromFile(fullPath);
                            sf.MD5 = md5;
                            sf.VirtualURL = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + reName);
                            sf = proxy.AddFile(sf).Data;

                            //删除临时块文件信息
                            proxy.DeleteTempFile(fileList.Select(i => i.Id).ToList());
                            //删除fileguid对比信息
                            proxy.DeleteFileGuid(fg);
                            return (new { flag = true, type = "file", file = sf });
                        }
                        return (new { flag = true, type = "chunk", chunk = tf });
                    }
                }
            }
            return (new { flag = false, result = "文件上传失败" });
        }


        /// <summary>
        /// 一次性上传
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private object UploadFile(HttpRequestBase request)
        {
            //断点续传
            string app = Request["app"] ?? "epm";
            string name = Request["name"];

            // 是否生成压缩图片
            string crop = request["crop"];
            bool cropValue = "true".Equals((string.IsNullOrWhiteSpace(crop) ? "false" : crop).ToLower());

            // 是否上传模型
            string bim = request["isbim"];
            bool isbim = "true".Equals((string.IsNullOrWhiteSpace(bim) ? "false" : bim).ToLower());


            //不支持分片上传的情况处理
            foreach (string file in Request.Files)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
                    {

                        HttpPostedFileBase postedFile = Request.Files[file]; //获取客户端上载文件的集合
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            name = postedFile.FileName;
                        }
                        string extensionName = Path.GetExtension(name); //获取客户端上传文件的名称

                        //获取文件存储目录
                        FileConfig config = null;
                        var configList = proxy.GetConfig(app);
                        if (isbim)
                        {
                            config = configList.Data.FirstOrDefault(i => i.FileTypeDirectory == "model");
                        }
                        else {
                            config = configList.Data.FirstOrDefault(i => i.FileTypeExtension.Contains(extensionName) && i.FileTypeDirectory != "model");
                        }

                        if (config == null)
                        {
                            return (new { flag = false, result = "不允许上传此类型的文件" });
                        }

                        var pathList = CreatePath(config);
                        string filePath = pathList[1];

                        string reName = Guid.NewGuid().ToString() + name;
                        if (isbim)
                        {
                            reName = name;
                        }
                        string fullPath = filePath + "\\" + reName;
                        postedFile.SaveAs(fullPath); //保存文件内容

                        FileInfo fileInfo = new FileInfo(fullPath);
                        //正式文件入库
                        Files sf = new Files();
                        sf.App = app;
                        sf.Day = DateTime.Now.ToString("yyyyMMdd").ToInt32Req();
                        sf.Extension = extensionName;
                        sf.FilePath = fullPath;
                        sf.FileSize = fileInfo.Length;
                        sf.FileType = config.FileTypeName;
                        sf.Guid = Guid.NewGuid().ToString();
                        sf.IsDelete = false;
                        sf.LastModifiedDate = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                        sf.Month = DateTime.Now.ToString("yyyyMM").ToInt32Req(); ;
                        sf.Name = name;
                        sf.ReName = reName;
                        sf.RequetURL = Request.UrlReferrer?.ToString();
                        sf.UploadTime = DateTime.Now;
                        sf.Year = DateTime.Now.Year;
                        sf.UserDescription = "";
                        sf.IP = request.UserHostAddress;
                        sf.Browser = request.Browser.Browser;
                        string md5 = GetMD5HashFromFile(fullPath);
                        sf.MD5 = md5;
                        sf.VirtualURL = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + reName);
                        sf = proxy.AddFile(sf).Data;

                        List<Files> list = new List<Files>();

                        list.Add(sf);

                        if (ImageHelper.IsImage(extensionName) && cropValue)
                        {
                            string bigReName = Guid.NewGuid() + name;
                            ImageHelper.CompressImage(fullPath, filePath + "\\" + bigReName);
                            var virTualUrl = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + bigReName);
                            Files files = CreateFile(filePath, "epmbig" + name, bigReName, virTualUrl, app, extensionName, config.FileTypeName, request);
                            files = proxy.AddFile(files).Data;
                            list.Add(files);


                            string smallReName = Guid.NewGuid() + name;
                            ImageHelper.MakeThumbnailImage(fullPath, filePath + "\\" + smallReName, 300, 300, ImageHelper.ImageCutMode.Cut);
                            var virTualUrl2 = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + smallReName);
                            Files smallFiles = CreateFile(filePath, "epmsmall" + name, smallReName, virTualUrl2, app, extensionName, config.FileTypeName, request);
                            smallFiles = proxy.AddFile(smallFiles).Data;
                            list.Add(smallFiles);
                        }

                        //return (new { flag = true, type = "file", file = sf });
                        return (new { flag = true, type = "file", file = list });
                    }
                }
            }

            return (new { flag = false, result = "文件上传失败" });
        }

        private Files CreateFile(string filePath, string name, string reName, string virTualUrl, string app, string extensionName, string fileTypeName, HttpRequestBase request)
        {
            FileInfo fileInfo = new FileInfo(filePath + "\\" + reName);
            //正式文件入库
            Files sf = new Files();
            sf.App = app;
            sf.Day = DateTime.Now.ToString("yyyyMMdd").ToInt32Req();
            sf.Extension = extensionName;
            sf.FilePath = filePath + "\\" + reName;
            sf.FileSize = fileInfo.Length;
            sf.FileType = fileTypeName;
            sf.Guid = Guid.NewGuid().ToString();
            sf.IsDelete = false;
            sf.LastModifiedDate = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
            sf.Month = DateTime.Now.ToString("yyyyMM").ToInt32Req(); ;
            sf.Name = name;
            sf.ReName = reName;
            sf.RequetURL = Request.UrlReferrer?.ToString();
            sf.UploadTime = DateTime.Now;
            sf.Year = DateTime.Now.Year;
            sf.UserDescription = "";
            sf.IP = request.UserHostAddress;
            sf.Browser = request.Browser.Browser;
            string md5 = GetMD5HashFromFile(filePath + "\\" + reName);
            sf.MD5 = md5;
            sf.VirtualURL = virTualUrl;
            return sf;
        }

        private StaticResource CreateStaticResource(string fullPath, string app, string extensionName, string fileName, string reName, string filePath, FileConfig config)
        {
            FileInfo fileInfo = new FileInfo(fullPath);
            //正式文件入库
            StaticResource sr = new StaticResource();
            sr.App = app;
            sr.Extension = extensionName;
            sr.FilePath = fullPath;
            sr.FileSize = fileInfo.Length;
            sr.Guid = Guid.NewGuid().ToString();
            sr.UploadTime = DateTime.Now;
            sr.HttpURL = "";
            sr.Name = fileName;
            sr.VirtualURL = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + reName);

            return sr;
        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload()
        {
            object result = new { flag = false, result = "文件上传失败" };
            string fId = Request["fId"];
            string chunk = Request["chunk"];

            try
            {
                //一次性上传
                if (string.IsNullOrEmpty(fId) || string.IsNullOrEmpty(chunk))
                {
                    result = UploadFile(Request);
                }
                else
                {
                    int chunks = Request["chunks"].ToInt32Req();//总的分块数量
                    if (chunks == 1)
                    {
                        result = UploadFile(Request);
                    }
                    else
                    {
                        result = UploadFileByChunk(Request);
                    }

                }
            }
            catch (Exception e)
            {
                result = new { flag = false, result = e.Message };
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 上传静态文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadResource()
        {
            string app = Request["app"] ?? "epm";
            // 是否生成压缩图片
            string crop = Request["crop"];
            bool cropValue = "true".Equals((string.IsNullOrWhiteSpace(crop) ? "false" : crop).ToLower());

            try
            {
                foreach (string file in Request.Files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
                        {

                            HttpPostedFileBase postedFile = Request.Files[file]; //获取客户端上载文件的集合
                            string extensionName = Path.GetExtension(postedFile.FileName); //获取客户端上传文件的名称

                            //获取文件存储目录
                            var configList = proxy.GetConfig(app);
                            var config = configList.Data.FirstOrDefault(i => i.FileTypeName == "static" && i.FileTypeExtension.Contains(extensionName));
                            if (config == null)
                            {
                                return Json(new { flag = false, result = "不允许上传此类型的文件" });
                            }

                            var pathList = CreatePath(config);
                            string filePath = pathList[1];
                            string reName = Guid.NewGuid().ToString() + extensionName;
                            string fullPath = filePath + "\\" + reName;
                            postedFile.SaveAs(fullPath); //保存文件内容


                            FileInfo fileInfo = new FileInfo(fullPath);
                            //正式文件入库
                            StaticResource sr = new StaticResource();
                            sr.App = app;
                            sr.Extension = extensionName;
                            sr.FilePath = fullPath;
                            sr.FileSize = fileInfo.Length;
                            sr.Guid = Guid.NewGuid().ToString();
                            sr.UploadTime = DateTime.Now;
                            sr.HttpURL = "";
                            sr.Name = postedFile.FileName;
                            sr.VirtualURL = (filePath.Replace(config.ParentPath, "/").Replace("\\", "/") + "/" + reName);
                            sr = proxy.AddStaticResource(sr).Data;

                            List<StaticResource> list = new List<StaticResource>();

                            list.Add(sr);

                            if (ImageHelper.IsImage(extensionName) && cropValue)
                            {
                                string bigReName = Guid.NewGuid() + extensionName;
                                string bigCropmageFilePath = filePath + "\\" + bigReName;

                                string smallReName = Guid.NewGuid() + postedFile.FileName;
                                string smallCropmageFilePath = filePath + "\\" + smallReName;

                                ImageHelper.CompressImage(fullPath, bigCropmageFilePath);

                                // TODO： 此处指定压缩大小可改为系统配置项
                                ImageHelper.MakeThumbnailImage(fullPath, smallCropmageFilePath, 300, 300, ImageHelper.ImageCutMode.Cut);

                                StaticResource files = CreateStaticResource(bigCropmageFilePath, app, extensionName, "epmbig" + postedFile.FileName, bigReName, filePath, config);
                                files = proxy.AddStaticResource(files).Data;
                                list.Add(files);

                                StaticResource smallFiles = CreateStaticResource(smallCropmageFilePath, app, extensionName, "epmsmall" + postedFile.FileName, smallReName, filePath, config);
                                smallFiles = proxy.AddStaticResource(smallFiles).Data;
                                list.Add(smallFiles);

                            }

                            //return Json(new { flag = true, type = "static", file = sr }, JsonRequestBehavior.AllowGet);
                            return Json(new { flag = true, type = "static", file = list }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { flag = false, result = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { flag = false, result = "文件上传失败" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private FilePathResult Download(int id)
        {
            using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
            {
                var model = proxy.GetFile(id);
                string contentType = GetContentTypeByFileName(model.Data.FilePath);
                string name = model.Data.Name;
                //无后缀的文件处理
                string ext = Path.GetExtension(name);
                if (string.IsNullOrEmpty(ext))
                {
                    ext = Path.GetExtension(model.Data.FilePath);
                    name = name + ext;
                }

                return File(model.Data.FilePath, contentType, name);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Download(string guid, string token = "")
        {
            using (ClientProxyFileServer proxy = new ClientProxyFileServer(ProxyEx(Request)))
            {
                //var tokenResult = proxy.CheckToken(token);
                //if (string.IsNullOrEmpty(tokenResult.Data))
                //{
                //    return Content("文件下载授权验证失败，请勿非法操作");
                //}
                var model = proxy.DownloadFile(guid);
                if (model.Data == null)
                {
                    return Content("没有找到该文件");
                }
                DownloadLog log = new DownloadLog();

                log.IP = Request.UserHostAddress;
                log.Browser = Request.Browser.Browser;
                log.FileGuid = guid;
                log.FileName = model.Data.Name;
                log.RecordTime = DateTime.Now;
                proxy.AddDownloadLog(log);
                //执行下载
                string contentType = GetContentTypeByFileName(model.Data.FilePath);
                string name = model.Data.Name;
                //无后缀的文件处理
                string ext = Path.GetExtension(name);
                if (string.IsNullOrEmpty(ext))
                {
                    ext = Path.GetExtension(model.Data.FilePath);
                    name = name + ext;
                }
                return File(model.Data.FilePath, contentType, name);
            }
        }

        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        private string GetRamCode()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }
        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取文件ContentType
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetContentTypeByFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".doc":
                    return "application/msword";

                case ".docx":
                    return "application/msword";

                case "xls":
                    return "application/x-excel";

                case "xlsx":
                    return "application/x-excel";

                case "rtf":
                    return "application/rtf";

                case "ppt":
                    return "application/ms-powerpoint";

                case "pptx":
                    return "application/ms-powerpoint";

                case "pdf":
                    return "application/pdf";

                case "zip":
                    return "application/zip";
            }
            return "application/octet-stream";
        }

        /// <summary>
        /// 根据配置规则创建存储目录
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private List<string> CreatePath(FileConfig config)
        {
            // 是否上传模型
            string bim = Request["isbim"];
            bool isbim = "true".Equals((string.IsNullOrWhiteSpace(bim) ? "false" : bim).ToLower());

            List<string> result = new List<string>();
            string path = "";
            if (isbim)
            {
                path = config.ParentPath;
            }
            else
            {
                path = config.ParentPath + config.App;
            }
            if (!Directory.Exists(path)) //判断给定的路径上是否存在该目录
            {
                Directory.CreateDirectory(path); //不存在则创建该目录
            }
            //根据配置规则创建文件存储目录
            string format = config.DirectoryFormat;//app/temp or file/type/time
            string[] formatArray = format.Split('/');
            string tempPath = path;
            string filePath = path;
            foreach (var f in formatArray)
            {
                switch (f)
                {
                    case "file":
                        filePath += "\\" + config.SaveFilePath;
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        tempPath = filePath.Replace(config.SaveFilePath, config.SaveTempPath);
                        if (!Directory.Exists(tempPath))
                        {
                            Directory.CreateDirectory(tempPath);
                        }
                        break;
                    case "type":
                        filePath += "\\" + config.FileTypeDirectory;
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        break;
                    case "time":
                        var date = DateTime.Now;
                        filePath += "\\" + date.Year + "\\" + date.Month + "\\" + date.Day;
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        break;
                }
            }
            result.Add(tempPath);
            result.Add(filePath);
            return result;
        }
    }
}