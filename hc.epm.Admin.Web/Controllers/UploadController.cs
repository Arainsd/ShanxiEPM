using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

using hc.epm.ViewModel;
using hc.Plat.Common.Global;

namespace hc.epm.Admin.Web.Controllers
{
    public class UploadController : Controller
    {
        /// <summary>
        /// 是否启用本省上传、下载
        /// </summary>
        private static string IsOpenHbUpload
        {
            get
            {
                string value = ConfigurationManager.AppSettings["IsOpenHbUpload"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }
                return value;
            }
        }

        /// <summary>
        /// 是否是外网
        /// </summary>
        private static bool IsNetwork
        {
            get
            {
                string value = ConfigurationManager.AppSettings["IsNetwork"];
                return value == "1";
            }
        }

        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        private static string MakeThumbnail
        {
            get
            {
                string value = ConfigurationManager.AppSettings["MakeThumbnail"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }
                return value;
            }
        }

        /// <summary>
        /// 缩略图存放地址
        /// </summary>
        private static string ThumbnailPath
        {
            get
            {
                string value = ConfigurationManager.AppSettings["ThumbnailPath"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置缩略图存放路径！");
                }
                return value;
            }
        }

        /// <summary>
        /// DMZ区资源服务器上传地址
        /// </summary>
        private static string UploadUrlThumbnail
        {
            get
            {
                string value = ConfigurationManager.AppSettings["UploadUrlThumbnail"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置DMZ区资源服务器上传地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 附件转存路径
        /// </summary>
        private static string TepmoraryPath
        {
            get
            {
                string value = ConfigurationManager.AppSettings["TepmoraryPath"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置资源存放路径！");
                }
                return value;
            }
        }

        /// <summary>
        /// 缩略图附件标识
        /// </summary>
        private static string Small
        {
            get { return "small"; }
        }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string UCode
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadHB()
        {
            List<ResultUpload> result = new List<ResultUpload>();
            try
            {
                HttpPostedFileBase postedFileBase = Request.Files[0];
                //2.png
                string fileName = postedFileBase.FileName;

                // 获取指定目录绝对路径，如果不存在，则创建
                if (!Directory.Exists(TepmoraryPath))
                {
                    Directory.CreateDirectory(TepmoraryPath);
                }

                string pathFile = TepmoraryPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                postedFileBase.SaveAs(pathFile);

                string guid = Guid.NewGuid().ToString();

                if (IsOpenHbUpload == "1")
                {
                    string responseContent = UploadFile(pathFile, fileName);
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        ResultUpload upload = JsonConvert.DeserializeObject<ResultUpload>(responseContent);//将文件信息json字符

                        //获取文件大小
                        var file = new FileInfo(pathFile);
                        var size = GetLength(file.Length);

                        upload.GuidId = guid;
                        upload.errorCode = "0";
                        upload.errorString = "";
                        upload.Name = fileName;
                        upload.Size = size;
                        upload.UploadName = UCode;
                        upload.UploadDate = DateTime.Now;
                        ResponseObject obj = new ResponseObject();
                        obj.FDFS_GROUP = upload.ResponseObject.responseObject[0];
                        obj.FDFS_NAME = upload.ResponseObject.responseObject[1];
                        upload.ResponseObject = obj;
                        result.Add(upload);
                    }
                }
                else
                {
                    //获取文件大小
                    var file = new FileInfo(pathFile);
                    var size = GetLength(file.Length);

                    ResultUpload upload = new ResultUpload();
                    upload.GuidId = guid;
                    upload.errorCode = "0";
                    upload.errorString = "";
                    upload.Name = fileName;
                    upload.Size = size;
                    upload.UploadName = UCode;
                    upload.UploadDate = DateTime.Now;
                    ResponseObject obj = new ResponseObject();
                    obj.FDFS_GROUP = string.Empty;
                    obj.FDFS_NAME = pathFile;
                    upload.ResponseObject = obj;
                    result.Add(upload);
                }

                #region 生成缩略图
                if (MakeThumbnail == "1" && ImageHelper.IsImage(Path.GetExtension(fileName)))
                {
                    string thumbnailPath = ConfigurationManager.AppSettings["ThumbnailPath"];
                    if (!string.IsNullOrWhiteSpace(thumbnailPath))
                    {
                        if (!thumbnailPath.EndsWith("\\"))
                        {
                            thumbnailPath += "\\";
                        }

                        //拼接相对路径
                        string RelativePath = thumbnailPath.Substring(0, thumbnailPath.Length - 1);
                        RelativePath = string.Format("{0}\\{1}\\{2}\\", RelativePath.Substring(RelativePath.LastIndexOf("\\") + 1), DateTime.Today.Year, DateTime.Today.Month);

                        // 缩略图存储文件夹按年月格式生成
                        thumbnailPath = string.Format("{0}{1}\\{2}\\", thumbnailPath, DateTime.Today.Year, DateTime.Today.Month);
                        if (!Directory.Exists(thumbnailPath))
                        {
                            Directory.CreateDirectory(thumbnailPath);
                        }

                        string smallReName = string.Format("{0}{1}", Small, Guid.NewGuid() + fileName);

                        //ImageHelper.MakeThumbnailImage(pathFile, thumbnailPath + smallReName, 300, 300, ImageHelper.ImageCutMode.Cut);
                        ImageHelper.CompressImage(pathFile, thumbnailPath + smallReName);

                        //获取文件大小
                        var file = new FileInfo(thumbnailPath.Replace("\\", "/") + smallReName);
                        var size = GetLength(file.Length);

                        ResultUpload upload = new ResultUpload();
                        upload.errorCode = "0";
                        upload.errorString = "";
                        upload.GuidId = guid;
                        upload.Name = smallReName;
                        upload.Size = size;
                        upload.UploadName = UCode;
                        upload.UploadDate = DateTime.Now;
                        upload.ImageType = Small;
                        ResponseObject obj = new ResponseObject();
                        obj.FDFS_GROUP = "";
                        obj.FDFS_NAME = RelativePath.Replace("\\", "/") + smallReName;
                        upload.ResponseObject = obj;
                        result.Add(upload);

                        #region 内网上传图片，缩略图发送给DMZ区服务器
                        if (IsOpenHbUpload == "1" && !IsNetwork)
                        {
                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(UploadUrlThumbnail + "?fileName=" + string.Format("{0}\\{1}\\{2}", DateTime.Today.Year, DateTime.Today.Month, smallReName));
                            // 边界符
                            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

                            // 设置属性
                            webRequest.Method = "POST";
                            webRequest.Timeout = 600000;
                            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                            // Header
                            const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
                            var header = string.Format(filePartHeader, "filepath", thumbnailPath + smallReName);
                            var headerbytes = Encoding.UTF8.GetBytes(header);

                            // 写入文件
                            var fileStream = new FileStream(thumbnailPath + smallReName, FileMode.Open, FileAccess.Read);

                            var memStream = new MemoryStream();
                            memStream.Write(beginBoundary, 0, beginBoundary.Length);
                            memStream.Write(headerbytes, 0, headerbytes.Length);

                            var buffer = new byte[fileStream.Length];
                            int bytesRead;
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                memStream.Write(buffer, 0, bytesRead);
                            }

                            // 写入字符串的Key
                            Dictionary<string, string> dicr = new Dictionary<string, string>();
                            dicr.Add("status", "1");
                            var stringKeyHeader = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n";
                            foreach (byte[] formitembytes in from string key in dicr.Keys select string.Format(stringKeyHeader, key, dicr[key]) into formitem select Encoding.UTF8.GetBytes(formitem))
                            {
                                memStream.Write(formitembytes, 0, formitembytes.Length);
                            }

                            // 最后的结束符
                            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                            memStream.Write(endBoundary, 0, endBoundary.Length);
                            memStream.Position = 0;

                            webRequest.ContentLength = memStream.Length;
                            var requestStream = webRequest.GetRequestStream();

                            var tempBuffer = new byte[memStream.Length];

                            memStream.Read(tempBuffer, 0, tempBuffer.Length);
                            memStream.Close();

                            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                            requestStream.Close();
                            fileStream.Close();
                            webRequest.Abort();
                        }
                        #endregion
                    }
                }
                #endregion

                if (IsOpenHbUpload == "1")
                {
                    // 上传成功后， 删除临时文件
                    System.IO.File.Delete(pathFile);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(result);
        }

        /// <summary>
        /// 附件上传跳转
        /// </summary>
        /// <param name="ucode">用户编码</param>
        /// <param name="pathFile">文件路径</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        private string UploadFile(string pathFile, string fileName)
        {
            #region 附件上传到指定资源服务器
            string UploadUrl = string.Empty;
            //if (IsNetwork)
            //{
            //    UploadUrl = ConfigurationManager.AppSettings["UploadUrl"];
            //    if (string.IsNullOrWhiteSpace(UploadUrl))
            //    {
            //        throw new Exception("未配置内网（185）资源服务器上传地址！");
            //    }
            //}
            //else
            //{
            UploadUrl = ConfigurationManager.AppSettings["UploadUrlBase"];
            if (string.IsNullOrWhiteSpace(UploadUrl))
            {
                throw new Exception("未配置基础平台资源服务器上传地址！");
            }
            //}
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(UploadUrl + "?fileName=" + fileName + "&RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + UCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FILE_NAME%22:%22" + fileName + "%22}}}");
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = 600000;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            // Header
            const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
            var header = string.Format(filePartHeader, "filepath", pathFile);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            // 写入文件
            var fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

            var memStream = new MemoryStream();
            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[fileStream.Length];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            // 写入字符串的Key
            Dictionary<string, string> dicr = new Dictionary<string, string>();
            dicr.Add("status", "1");
            var stringKeyHeader = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n";
            foreach (byte[] formitembytes in from string key in dicr.Keys select string.Format(stringKeyHeader, key, dicr[key]) into formitem select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
            memStream.Write(endBoundary, 0, endBoundary.Length);
            memStream.Position = 0;

            webRequest.ContentLength = memStream.Length;
            var requestStream = webRequest.GetRequestStream();

            var tempBuffer = new byte[memStream.Length];

            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            string responseContent = string.Empty;
            var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            {
                responseContent = httpStreamReader.ReadToEnd();
            }
            fileStream.Close();
            httpWebResponse.Close();
            webRequest.Abort();

            return responseContent;
            #endregion
        }

        /// <summary>
        /// 处理dmz区转发的文件上传请求
        /// </summary>
        /// <param name="ucode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public string UploadJump(string fileName)
        {
            HttpPostedFileBase postedFileBase = Request.Files[0];
            string pathFile = TepmoraryPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            postedFileBase.SaveAs(pathFile);

            #region 上传服务器
            if (IsOpenHbUpload == "1")
            {
                return UploadFile(pathFile, fileName);
            }
            #endregion

            return string.Empty;
        }

        /// <summary>
        /// dmz接收文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ThumbnailDMZ(string fileName)
        {
            HttpPostedFileBase postedFileBase = Request.Files[0];
            string pathFile = ThumbnailPath + fileName;
            postedFileBase.SaveAs(pathFile);

            var responseStr = pathFile;
            return Content(responseStr);
        }

        /// <summary>
        /// 因为C#提供的文件的大小是以B为单位的，所以显示文件大小的时候会出现一大串数字很不方便
        /// 所以，该函数为了方便地显示文件大小而出现
        /// 函数说明，
        ///     如果文件大小是0-1024B 以内的   显示以B为单位
        ///     如果文件大小是1KB-1024KB之间的 显示以KB为单位
        ///     如果文件大小是1M-1024M之间的   显示以M为单位
        ///     如果文件大小是1024M以上的      显示以GB为单位
        /// </summary>
        /// <param name="lengthOfDocument"> 文件的大小 单位：B 类型：long</param>
        /// <returns></returns>
        private string GetLength(long lengthOfDocument)
        {
            if (lengthOfDocument < 1024)
            {
                return string.Format(Math.Round((lengthOfDocument / 1.0), 2).ToString() + "B");
            }
            else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0), 2).ToString() + "KB");
            }
            else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0), 2).ToString() + "M");
            }
            else
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0 / 1024.0), 2).ToString() + "GB");
            }
        }
    }
}