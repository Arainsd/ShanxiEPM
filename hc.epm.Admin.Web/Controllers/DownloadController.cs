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
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;
using System.Drawing;

namespace hc.epm.Admin.Web.Controllers
{
    public class DownloadController : Controller
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
        /// 资源服务器上传到DMZ地址，内网站点配置此项，配置的是dmz区站点的/Download/ReceiveDMZ
        /// </summary>
        private static string UploadUrlDMZ
        {
            get
            {
                string value = ConfigurationManager.AppSettings["UploadUrlDMZ"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置DMZ区资源服务器上传地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 资源服务器下载地址,如果在DMZ区，配置内网文件处理服务器地址，即：/Download/DownLoadHBHandle；如果在内网，配置基础平台文件服务器地址，即http://10.206.129.206
        /// </summary>
        private static string DownloadUrl
        {
            get
            {
                string value = ConfigurationManager.AppSettings["DownloadUrl"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置内网（185）资源服务器下载地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 基础平台资源服务器下载地址
        /// </summary>
        private static string DownloadUrlBase
        {
            get
            {
                string value = ConfigurationManager.AppSettings["DownloadUrlBase"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置基础平台资源服务器下载地址！");
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
        /// 下载地址
        /// </summary>
        /// <param name="group"></param>
        /// <param name="path">相对路径</param>
        [HttpGet]
        public void DownLoadHB(string group, string path, string fileName)
        {
            HttpWebRequest request = null;
            try
            {
                if (IsOpenHbUpload == "1")
                {
                    ////外网下载文件处理
                    //if (IsNetwork)
                    //{
                    //    string url = DownloadUrl + "?fileName=" + Server.UrlEncode(fileName) + "&RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + UCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";
                    //    request = (HttpWebRequest)HttpWebRequest.Create(url);
                    //    request.Method = "POST";
                    //    request.Timeout = 600000;
                    //    request.ContentLength = 0;
                    //    request.ContentType = "application/json";
                    //    string responseStr = string.Empty;
                    //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    //    {
                    //        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    //        responseStr = reader.ReadToEnd();
                    //        reader.Close();
                    //        if (!string.IsNullOrEmpty(responseStr))
                    //        {
                    //            responseStr = responseStr.TrimEnd('"').TrimStart('"');
                    //            FileInfo file = new FileInfo(responseStr);//创建一个文件对象
                    //            FileStream stream = file.OpenRead();
                    //            var bufferLength = stream.Length;
                    //            byte[] bytes = new byte[bufferLength];
                    //            stream.Read(bytes, 0, Convert.ToInt32(bufferLength));
                    //            stream.Flush();
                    //            stream.Close();

                    //            Response.Clear();
                    //            // 设置缓冲输出为true,后台编辑的文件写到内存流中了
                    //            Response.Buffer = true;
                    //            // 设置编码格式 ContentEncoding是管字节流到文本的，而Charset是管在浏览器中显示的
                    //            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                    //            Response.Charset = "GBK";
                    //            // 将HTTP头添加到输出流，指定默认名
                    //            Response.AddHeader("Content-Disposition", string.Format(@"attachment;filename=""{0}""", HttpUtility.UrlEncode(fileName)));
                    //            Response.ContentEncoding = System.Text.Encoding.UTF8;
                    //            // 设置输出流的HTTP MIME类型
                    //            Response.ContentType = "application/octet-stream;charset=gbk";
                    //            Response.AddHeader("Content-Length", bytes.Length.ToString());
                    //            // 将指定的文件写入HTTP内容输出流
                    //            //Response.OutputStream.Write(bytes, 0, bytes.Length);

                    //            //防止文件名含中文出现乱码而进行编码
                    //            //Response.BinaryWrite(bytes);
                    //            System.IO.Stream fs = this.Response.OutputStream;
                    //            fs.Write(bytes, 0, bytes.Length);
                    //            fs.Close();

                    //            // 向客户端发送当前所有缓冲的输出
                    //            Response.Flush();
                    //            // 将当前所有缓冲的输出关闭
                    //            Response.Close();

                    //            // 下载成功后， 删除临时文件
                    //            System.IO.File.Delete(responseStr);
                    //        }
                    //    }
                    //}
                    //else//内网处理逻辑
                    //{
                    string url = DownloadUrlBase + "?RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + UCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";
                    request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "POST";
                    request.Timeout = 600000;
                    request.ContentType = "multipart/form-data";
                    string responseStr = string.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                        responseStr = reader.ReadToEnd().ToString();
                        reader.Close();

                        if (!string.IsNullOrEmpty(responseStr))
                        {
                            ResultDownUpload result = JsonConvert.DeserializeObject<ResultDownUpload>(responseStr);//将文件信息json字符
                            byte[] bytes = JavaBytesToNetBytes(result.responseMap.FILE_DATA);

                            Response.ClearHeaders();
                            Response.Clear();
                            Response.Expires = 0;
                            Response.Buffer = true;
                            Response.AddHeader("Accept-Language", "zh-cn");
                            Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                            Response.ContentType = "application/octet-stream;charset=gbk";
                            Response.BinaryWrite(bytes);
                            Response.Flush();
                            Response.End();
                        }
                    }
                    //}
                }
                else
                {
                    FileInfo file = new FileInfo(path);//创建一个文件对象
                    FileStream stream = file.OpenRead();
                    var bufferLength = stream.Length;
                    byte[] bytes = new byte[bufferLength];
                    stream.Read(bytes, 0, Convert.ToInt32(bufferLength));
                    stream.Flush();
                    stream.Close();

                    Response.Clear();
                    // 设置缓冲输出为true,后台编辑的文件写到内存流中了
                    Response.Buffer = true;
                    // 设置编码格式 ContentEncoding是管字节流到文本的，而Charset是管在浏览器中显示的
                    //Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                    Response.Charset = "GBK";
                    // 将HTTP头添加到输出流，指定默认名
                    Response.AddHeader("Content-Disposition", string.Format(@"attachment;filename=""{0}""", HttpUtility.UrlEncode(fileName)));
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    // 设置输出流的HTTP MIME类型
                    Response.ContentType = "application/octet-stream;charset=gbk";
                    Response.AddHeader("Content-Length", bytes.Length.ToString());
                    // 将指定的文件写入HTTP内容输出流
                    //Response.OutputStream.Write(bytes, 0, bytes.Length);

                    //防止文件名含中文出现乱码而进行编码
                    //Response.BinaryWrite(bytes);
                    System.IO.Stream fs = this.Response.OutputStream;
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                    // 向客户端发送当前所有缓冲的输出
                    Response.Flush();
                    // 将当前所有缓冲的输出关闭
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        /// <summary>
        /// 处理dmz区转发的文件下载请求
        /// </summary>
        public ActionResult DownLoadHBHandle(string fileName)
        {
            string resultUpload = "";
            try
            {
                string url = DownloadUrlBase + "?RequestParam=" + Request.Params["RequestParam"];
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 600000;
                request.ContentType = "application/octet-stream";

                string responseStr = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    responseStr = reader.ReadToEnd().ToString();
                    reader.Close();

                    if (!string.IsNullOrEmpty(responseStr))
                    {
                        ResultDownUpload result = JsonConvert.DeserializeObject<ResultDownUpload>(responseStr);//将文件信息json字符
                        byte[] bytesBasePlat = JavaBytesToNetBytes(result.responseMap.FILE_DATA);
                        MemoryStream ms = new MemoryStream(bytesBasePlat);

                        //本地存储路径
                        if (!Directory.Exists(TepmoraryPath))
                        {
                            Directory.CreateDirectory(TepmoraryPath);
                        }
                        string pathFile = TepmoraryPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                        FileStream fs = new FileStream(pathFile, FileMode.OpenOrCreate);
                        ms.WriteTo(fs);
                        ms.Close();
                        fs.Close();

                        //响应外网请求
                        resultUpload = UploadDMZ(pathFile, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(resultUpload);
        }

        /// <summary>
        /// 上传至DMZ
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string UploadDMZ(string pathFile, string fileName)
        {
            string result = "";
            try
            {
                #region 附件上传到DMZ自有站点下资源服务器

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(UploadUrlDMZ + "?fileName=" + fileName + "&RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + UCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FILE_NAME%22:%22" + fileName + "%22}}}");
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

                string responseContent;
                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    responseContent = httpStreamReader.ReadToEnd();
                }
                fileStream.Close();
                httpWebResponse.Close();
                webRequest.Abort();

                result = responseContent;
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// dmz接收文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReceiveDMZ(string fileName)
        {
            HttpPostedFileBase postedFileBase = Request.Files[0];
            string pathFile = TepmoraryPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            postedFileBase.SaveAs(pathFile);

            var responseStr = pathFile;
            return Content(responseStr);
        }


        private int[] NetBytesToJavaBytes(byte[] arr)
        {
            int[] newByte = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > 127)
                    newByte[i] = (int)arr[i] - 256;
                else
                    newByte[i] = (int)arr[i];
            }
            return newByte;
        }

        private byte[] JavaBytesToNetBytes(int[] arr)
        {
            byte[] newByte = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < 0)
                    newByte[i] = (byte)(arr[i] + 256);
                else
                    newByte[i] = (byte)arr[i];
            }
            return newByte;
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