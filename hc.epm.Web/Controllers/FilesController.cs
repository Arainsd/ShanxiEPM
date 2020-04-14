using hc.epm.DataModel.Basic;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.Common;
using hc.epm.UI.Common;
using hc.Plat.Common.Extend;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.epm.Web.ClientProxy;
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;

namespace hc.epm.Web.Controllers
{
    public class FilesController : BaseController
    {
        // GET: Files
        public ActionResult Index(string tableName, long id, string column)
        {
            ViewBag.tableName = tableName;
            ViewBag.id = id;

            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetFilesByTable(tableName, id);
                if (!string.IsNullOrEmpty(column))
                {
                    var list = result.Data;
                    list = list.Where(i => i.TableColumn == column).ToList();
                    result.Data = list;
                }

            }
            return View(result.Data);
        }

        /// <summary>
        /// 根据表名与数据Id获取附件列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="id">数据Id</param>
        /// <param name="tableCol"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFileList(string tableName, long id, string tableCol = "")
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (id != 0)
                {
                    result = proxy.GetFilesByTable(tableName, id);

                    if (!string.IsNullOrEmpty(tableCol))
                    {
                        var list = result.Data;
                        list = list.Where(i => i.TableColumn == tableCol).ToList();
                        result.Data = list;
                    }
                }
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 根据表名与数据Id获取附件列表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="ids">数据Ids</param>
        /// <param name="tableCol"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFileListByTableIds(string tableName, string ids, string tableCol = "")
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    List<long> idlist = ids.Split(',').ToLongList();
                    result = proxy.GetFileListByTableIds(tableName, idlist);

                    var list = result.Data;
                    foreach (var item in list)
                    {
                        item.Id = item.TableId;
                    }

                    if (!string.IsNullOrEmpty(tableCol))
                    {
                        list = list.Where(i => i.TableColumn == tableCol).ToList();
                    }
                    result.Data = list;
                }
            }

            return Json(result.ToResultView());
        }

        public ActionResult GetFileName(string tableName, string name)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetFilesByTableName(tableName, name);
            }
            return Json(result.ToResultView());
        }

        #region 上传、下载 方法废弃2018.10.21
        ///// <summary>
        ///// 是否启用本省上传、下载
        ///// </summary>
        //private static string IsOpenHbUpload
        //{
        //    get
        //    {
        //        string value = ConfigurationManager.AppSettings["IsOpenHbUpload"];
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            value = "0";
        //        }
        //        return value;
        //    }
        //}

        ///// <summary>
        ///// 是否生成缩略图
        ///// </summary>
        //private static string MakeThumbnail
        //{
        //    get
        //    {
        //        string value = ConfigurationManager.AppSettings["MakeThumbnail"];
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            value = "0";
        //        }
        //        return value;
        //    }
        //}

        ///// <summary>
        ///// 附件上传路径
        ///// </summary>
        //private static string UploadUrl
        //{
        //    get
        //    {
        //        string value = ConfigurationManager.AppSettings["UploadUrl"];
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            throw new Exception("未配置资源服务器上传地址！");
        //        }
        //        return value;
        //    }
        //}

        ///// <summary>
        ///// 附件下载路径
        ///// </summary>
        //private static string DownloadUrl
        //{
        //    get
        //    {
        //        string value = ConfigurationManager.AppSettings["DownloadUrl"];
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            throw new Exception("未配置资源服务器下载地址！");
        //        }
        //        return value;
        //    }
        //}

        ///// <summary>
        ///// 缩略图附件标识
        ///// </summary>
        //private static string Small
        //{
        //    get { return "small"; }
        //}

        ///// <summary>
        ///// 上传
        ///// </summary>
        ///// <param name="path">文件路径</param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult UploadHB()
        //{
        //    List<ResultUpload> result = new List<ResultUpload>();
        //    try
        //    {
        //        HttpPostedFileBase postedFileBase = Request.Files[0];
        //        //2.png
        //        string fileName = postedFileBase.FileName;
        //        //.png
        //        string extension = Path.GetExtension(fileName);

        //        string fullPath = ConfigurationManager.AppSettings["TepmoraryPath"];
        //        // 获取指定目录绝对路径，如果不存在，则创建
        //        if (!Directory.Exists(fullPath))
        //        {
        //            Directory.CreateDirectory(fullPath);
        //        }

        //        string pathFile = fullPath + Guid.NewGuid().ToString() + extension;
        //        postedFileBase.SaveAs(pathFile);

        //        //用户编码
        //        string ucode = CurrentUser.UserCode;

        //        //Guid
        //        string GuidId = Guid.NewGuid().ToString();

        //        #region 上传服务器
        //        if (IsOpenHbUpload == "1")
        //        {
        //            #region 附件上传到本省资源服务器

        //            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(UploadUrl + "?RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + ucode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FILE_NAME%22:%22" + fileName + "%22}}}");
        //            // 边界符
        //            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        //            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

        //            // 设置属性
        //            webRequest.Method = "POST";
        //            webRequest.Timeout = 600000;
        //            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

        //            // Header
        //            const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
        //            var header = string.Format(filePartHeader, "filepath", pathFile);
        //            var headerbytes = Encoding.UTF8.GetBytes(header);

        //            // 写入文件
        //            var fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

        //            var memStream = new MemoryStream();
        //            memStream.Write(beginBoundary, 0, beginBoundary.Length);
        //            memStream.Write(headerbytes, 0, headerbytes.Length);
        //            var buffer = new byte[1024];
        //            int bytesRead;
        //            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //            {
        //                memStream.Write(buffer, 0, bytesRead);
        //            }

        //            // 写入字符串的Key
        //            Dictionary<string, string> dicr = new Dictionary<string, string>();
        //            dicr.Add("status", "1");
        //            var stringKeyHeader = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n";
        //            foreach (byte[] formitembytes in from string key in dicr.Keys select string.Format(stringKeyHeader, key, dicr[key]) into formitem select Encoding.UTF8.GetBytes(formitem))
        //            {
        //                memStream.Write(formitembytes, 0, formitembytes.Length);
        //            }

        //            // 最后的结束符
        //            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
        //            memStream.Write(endBoundary, 0, endBoundary.Length);
        //            memStream.Position = 0;

        //            webRequest.ContentLength = memStream.Length;
        //            var requestStream = webRequest.GetRequestStream();

        //            var tempBuffer = new byte[memStream.Length];

        //            memStream.Read(tempBuffer, 0, tempBuffer.Length);
        //            memStream.Close();

        //            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
        //            requestStream.Close();

        //            string responseContent;
        //            var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
        //            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
        //            {
        //                responseContent = httpStreamReader.ReadToEnd();
        //            }
        //            fileStream.Close();
        //            httpWebResponse.Close();
        //            webRequest.Abort();

        //            //responseStr = "{\"addinObject\":null,\"errorCode\":0,\"errorString\":\"上传成功！\",\"exceptoin\":null,\"responseMap\":{\"osplicense\":\"TRUE\"},\"responseObject\":{\"addinObject\":null,\"errorCode\":0,\"errorString\":\"上传成功！\",\"exceptoin\":null,\"responseMap\":{\"osplicense\":\"TRUE\"},\"responseObject\":[\"group1\",\"M00/00/6E/Cs6Bz1ufFDeAc58CAAAIXkl8ijg320.jpg\"]}}";
        //            //responseStr = "{\"addinObject\":null,\"errorCode\":0,\"errorString\":\"上传成功！\",\"exceptoin\":null,\"responseMap\":{\"osplicense\":\"TRUE\"},\"responseObject\":{\"addinObject\":null,\"errorCode\":0,\"errorString\":\"上传成功！\",\"exceptoin\":null,\"responseMap\":{\"osplicense\":\"TRUE\"},\"responseObject\":[\"group1\",\"M00/00/6E/Cs6Bz1ufE3mAbEoHAAAHDJ7WfBM330.txt\"]}}";
        //            if (!string.IsNullOrEmpty(responseContent))
        //            {
        //                ResultUpload upload = JsonConvert.DeserializeObject<ResultUpload>(responseContent);//将文件信息json字符

        //                //获取文件大小
        //                var file = new FileInfo(pathFile);
        //                var size = GetLength(file.Length);

        //                upload.GuidId = Guid.NewGuid().ToString();
        //                upload.errorCode = "0";
        //                upload.errorString = "";
        //                upload.Name = fileName;
        //                upload.Size = size;
        //                upload.UploadName = CurrentUser.UserName;
        //                upload.UploadDate = DateTime.Now;
        //                ResponseObject obj = new ResponseObject();
        //                obj.FDFS_GROUP = upload.ResponseObject.responseObject[0];
        //                obj.FDFS_NAME = upload.ResponseObject.responseObject[1];
        //                upload.ResponseObject = obj;
        //                result.Add(upload);
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            //获取文件大小
        //            var file = new FileInfo(pathFile);
        //            var size = GetLength(file.Length);

        //            ResultUpload upload = new ResultUpload();
        //            upload.errorCode = "0";
        //            upload.errorString = "";
        //            upload.GuidId = GuidId;
        //            upload.Name = fileName;
        //            upload.Size = size;
        //            upload.UploadName = CurrentUser.UserName;
        //            upload.UploadDate = DateTime.Now;
        //            ResponseObject obj = new ResponseObject();
        //            obj.FDFS_GROUP = "epm";
        //            obj.FDFS_NAME = pathFile;
        //            upload.ResponseObject = obj;
        //            result.Add(upload);
        //        }
        //        #endregion

        //        #region 生成缩略图
        //        if (MakeThumbnail == "1" && ImageHelper.IsImage(extension))
        //        {
        //            string thumbnailPath = ConfigurationManager.AppSettings["ThumbnailPath"];
        //            if (!string.IsNullOrWhiteSpace(thumbnailPath))
        //            {
        //                if (!thumbnailPath.EndsWith("\\"))
        //                {
        //                    thumbnailPath += "\\";
        //                }

        //                //拼接相对路径
        //                string RelativePath = thumbnailPath.Substring(0, thumbnailPath.Length - 1);
        //                RelativePath = string.Format("{0}\\{1}\\{2}\\", RelativePath.Substring(RelativePath.LastIndexOf("\\") + 1), DateTime.Today.Year, DateTime.Today.Month);

        //                // 缩略图存储文件夹按年月格式生成
        //                thumbnailPath = string.Format("{0}{1}\\{2}\\", thumbnailPath, DateTime.Today.Year, DateTime.Today.Month);
        //                if (!Directory.Exists(thumbnailPath))
        //                {
        //                    Directory.CreateDirectory(thumbnailPath);
        //                }

        //                string smallReName = string.Format("{0}{1}", Small, Guid.NewGuid() + fileName);

        //                //ImageHelper.MakeThumbnailImage(pathFile, thumbnailPath + smallReName, 300, 300, ImageHelper.ImageCutMode.Cut);
        //                ImageHelper.CompressImage(pathFile, thumbnailPath + smallReName);

        //                //获取文件大小
        //                var file = new FileInfo(thumbnailPath.Replace("\\", "/") + smallReName);
        //                var size = GetLength(file.Length);

        //                ResultUpload upload = new ResultUpload();
        //                upload.errorCode = "0";
        //                upload.errorString = "";
        //                upload.GuidId = GuidId;
        //                upload.Name = smallReName;
        //                upload.Size = size;
        //                upload.UploadName = CurrentUser.UserName;
        //                upload.UploadDate = DateTime.Now;
        //                upload.ImageType = Small;
        //                ResponseObject obj = new ResponseObject();
        //                obj.FDFS_GROUP = "";
        //                obj.FDFS_NAME = RelativePath.Replace("\\", "/") + smallReName;
        //                upload.ResponseObject = obj;
        //                result.Add(upload);
        //            }
        //        }
        //        #endregion

        //        if (IsOpenHbUpload == "1")
        //        {
        //            // 上传成功后， 删除临时文件
        //            System.IO.File.Delete(pathFile);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return Json(result);
        //}

        ///// <summary>
        ///// 下载
        ///// </summary>
        ///// <param name="group"></param>
        ///// <param name="path">相对路径</param>
        //[HttpGet]
        //public void DownLoadHB(string group, string path, string fileName)
        //{
        //    HttpWebRequest request = null;
        //    try
        //    {
        //        if (IsOpenHbUpload == "0")
        //        {
        //            throw new Exception("下载失败，服务连接资源服务器");
        //        }

        //        string ucode = CurrentUser.UserCode;

        //        string url = DownloadUrl + "?RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + ucode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";
        //        request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Method = "POST";
        //        request.ContentType = "multipart/form-data";

        //        string responseStr = string.Empty;
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
        //            responseStr = reader.ReadToEnd().ToString();
        //            reader.Close();

        //            if (!string.IsNullOrEmpty(responseStr))
        //            {
        //                ResultDownUpload result = JsonConvert.DeserializeObject<ResultDownUpload>(responseStr);//将文件信息json字符
        //                byte[] bytes = JavaBytesToNetBytes(result.responseMap.FILE_DATA);

        //                Response.ClearHeaders();
        //                Response.Clear();
        //                Response.Expires = 0;
        //                Response.Buffer = true;
        //                Response.AddHeader("Accept-Language", "zh-cn");
        //                Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
        //                Response.ContentType = "application/octet-stream;charset=gbk";
        //                Response.BinaryWrite(bytes);
        //                Response.Flush();
        //                Response.End();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取下载图片
        ///// </summary>
        ///// <param name="group"></param>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public string GetDownLoadImageHB(string group, string path, string ucode)
        //{
        //    HttpWebRequest request = null;
        //    try
        //    {
        //        if (IsOpenHbUpload == "0")
        //        {
        //            throw new Exception("下载失败，服务连接资源服务器");
        //        }

        //        string url = DownloadUrl + "?RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + ucode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";
        //        request = (HttpWebRequest)HttpWebRequest.Create(url);
        //        request.Method = "GET";
        //        request.ContentType = "multipart/form-data";

        //        string responseStr = string.Empty;
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
        //            responseStr = reader.ReadToEnd().ToString();
        //            reader.Close();

        //            if (!string.IsNullOrEmpty(responseStr))
        //            {
        //                ResultDownUpload result = JsonConvert.DeserializeObject<ResultDownUpload>(responseStr);//将文件信息json字符
        //                byte[] bytes = JavaBytesToNetBytes(result.responseMap.FILE_DATA);

        //                return "data:image/" + path.Substring(path.LastIndexOf('.') + 1) + ";base64," + Convert.ToBase64String(bytes);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //    }
        //    return string.Empty;
        //}

        //private int[] NetBytesToJavaBytes(byte[] arr)
        //{
        //    int[] newByte = new int[arr.Length];
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (arr[i] > 127)
        //            newByte[i] = (int)arr[i] - 256;
        //        else
        //            newByte[i] = (int)arr[i];
        //    }
        //    return newByte;
        //}

        //private byte[] JavaBytesToNetBytes(int[] arr)
        //{
        //    byte[] newByte = new byte[arr.Length];
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (arr[i] < 0)
        //            newByte[i] = (byte)(arr[i] + 256);
        //        else
        //            newByte[i] = (byte)arr[i];
        //    }
        //    return newByte;
        //}

        ///// <summary>
        ///// 因为C#提供的文件的大小是以B为单位的，所以显示文件大小的时候会出现一大串数字很不方便
        ///// 所以，该函数为了方便地显示文件大小而出现
        ///// 函数说明，
        /////     如果文件大小是0-1024B 以内的   显示以B为单位
        /////     如果文件大小是1KB-1024KB之间的 显示以KB为单位
        /////     如果文件大小是1M-1024M之间的   显示以M为单位
        /////     如果文件大小是1024M以上的      显示以GB为单位
        ///// </summary>
        ///// <param name="lengthOfDocument"> 文件的大小 单位：B 类型：long</param>
        ///// <returns></returns>
        //private string GetLength(long lengthOfDocument)
        //{
        //    if (lengthOfDocument < 1024)
        //    {
        //        return string.Format(Math.Round((lengthOfDocument / 1.0), 2).ToString() + "B");
        //    }
        //    else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
        //    {
        //        return string.Format(Math.Round((lengthOfDocument / 1024.0), 2).ToString() + "KB");
        //    }
        //    else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
        //    {
        //        return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0), 2).ToString() + "M");
        //    }
        //    else
        //    {
        //        return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0 / 1024.0), 2).ToString() + "GB");
        //    }
        //}

        #endregion
    }
}