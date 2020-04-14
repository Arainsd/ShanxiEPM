/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.Plat.Common.Global
 * 文件名：  HttpHelper
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/6/20 18:05:59
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace hc.Plat.Common.Global
{
    /// <summary>
    /// Http 请求帮助类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static string Get(string requestUrl)
        {
            string result = string.Empty;

            try
            {
                //创建一个客户端的Http请求实例
                //HttpWebRequest request = WebRequest.Create(new Uri(requestUrl, true)) as HttpWebRequest;

                Uri uri = new Uri(requestUrl, true);
                //var url = HttpUtility.UrlEncode(requestUrl);
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 30 * 1000;
                request.ProtocolVersion = HttpVersion.Version11;
                //request.TransferEncoding = "chunked";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:63.0) Gecko/20100101 Firefox/63.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.KeepAlive = false;
                //request.ContentType = "application/json;charset=UTF-8";
                request.ContentType = "text/html;charset=UTF-8";

                //CookieContainer cc = new CookieContainer();
                //cc.SetCookies(request.RequestUri, cookie);
                //request.CookieContainer = cc;
                //获取当前Http请求的响应实例
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream respStream = response.GetResponseStream();

                using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding("UTF-8")))
                {
                    result = reader.ReadToEnd();
                }

                response.Close();
                respStream.Close();
            }
            catch (Exception ex)
            { }
            finally
            {

            }
            return result;
        }

        public static string Get(string url, string param)
        {
            url = url + param;
            return Get(url);
        }

        public static string Post(string requestUrl, string data)
        {
            return Post(requestUrl, Encoding.GetEncoding("UTF-8").GetBytes(data));
        }

        public static string Post(string url, byte[] data)
        {
            //创建一个客户端的Http请求实例
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            //获取当前Http请求的响应实例
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
            {
                result = reader.ReadToEnd();
            }
            responseStream.Close();

            return result;
        }
    }
}
