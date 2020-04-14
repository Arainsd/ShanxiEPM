using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Top.Api.Util
{
    public class SpiUtils
    {
        private const string TOP_SIGN_LIST = "top-sign-list";
        private const string TOP_FIELD_SIGN = "sign";
        private const string TOP_FIELD_TS = "timestamp";
        private static readonly string[] HEADER_FIELDS_IP = {"X-Real-IP", "X-Forwarded-For", "Proxy-Client-IP",
		"WL-Proxy-Client-IP", "HTTP_CLIENT_IP", "HTTP_X_FORWARDED_FOR"};

        /// <summary>
        /// 校验SPI请求签名，适用于Content-Type为application/x-www-form-urlencoded或multipart/form-data的GET或POST请求。
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="secret">app对应的secret</param>
        /// <returns>true：校验通过；false：校验不通过</returns>
        public static bool CheckSign4FormRequest(HttpRequest request, string secret)
        {
            return CheckSign(request, null, secret);
        }

        /// <summary>
        /// 校验SPI请求签名，适用于Content-Type为text/xml或text/json的POST请求。
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="body">请求体的文本内容</param>
        /// <param name="secret">app对应的secret</param>
        /// <returns>true：校验通过；false：校验不通过</returns>
        public static bool CheckSign4TextRequest(HttpRequest request, string body, string secret)
        {
            return CheckSign(request, body, secret);
        }

        private static bool CheckSign(HttpRequest request, string body, string secret)
        {
            IDictionary<string, string> parameters = new SortedDictionary<string, string>(StringComparer.Ordinal);
            string charset = GetRequestCharset(request.ContentType);

            // 1. 获取header参数
            AddAll(parameters, GetHeaderMap(request, charset));

            // 2. 获取url参数
            Dictionary<string, string> queryMap = GetQueryMap(request, charset);
            AddAll(parameters, queryMap);

            // 3. 获取form参数
            AddAll(parameters, GetFormMap(request));

            // 4. 生成签名并校验
            string remoteSign = null;
            if (queryMap.ContainsKey(TOP_FIELD_SIGN))
            {
                remoteSign = queryMap[TOP_FIELD_SIGN];
            }
            string localSign = Sign(parameters, body, secret, charset);
            return localSign.Equals(remoteSign);
        }

        private static void AddAll(IDictionary<string, string> dest, IDictionary<string, string> from)
        {
            if (from != null && from.Count > 0)
            {
                IEnumerator<KeyValuePair<string, string>> em = from.GetEnumerator();
                while (em.MoveNext())
                {
                    KeyValuePair<string, string> kvp = em.Current;
                    dest.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// 签名规则：hex(md5(secret+sorted(header_params+url_params+form_params)+body)+secret)
        /// </summary>
        private static string Sign(IDictionary<string, string> parameters, string body, string secret, string charset)
        {
            IEnumerator<KeyValuePair<string, string>> em = parameters.GetEnumerator();

            // 第1步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder(secret);
            while (em.MoveNext())
            {
                string key = em.Current.Key;
                if (!TOP_FIELD_SIGN.Equals(key))
                {
                    string value = em.Current.Value;
                    query.Append(key).Append(value);
                }
            }
            if (body != null)
            {
                query.Append(body);
            }

            query.Append(secret);

            // 第2步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(query.ToString()));

            // 第3步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }

        private static string GetRequestCharset(string ctype)
        {
            string charset = "utf-8";
            if (!string.IsNullOrEmpty(ctype))
            {
                string[] entires = ctype.Split(';');
                foreach (string entry in entires)
                {
                    string _entry = entry.Trim();
                    if (_entry.StartsWith("charset"))
                    {
                        string[] pair = _entry.Split('=');
                        if (pair.Length == 2)
                        {
                            if (!string.IsNullOrEmpty(pair[1]))
                            {
                                charset = pair[1].Trim();
                            }
                        }
                        break;
                    }
                }
            }
            return charset;
        }

        public static Dictionary<string, string> GetHeaderMap(HttpRequest request, string charset)
        {
            Dictionary<string, string> headerMap = new Dictionary<string, string>();
            string signList = request.Headers[TOP_SIGN_LIST];
            if (!string.IsNullOrEmpty(signList))
            {
                string[] keys = signList.Split(',');
                foreach (string key in keys)
                {
                    string value = request.Headers[key];
                    if (string.IsNullOrEmpty(value))
                    {
                        headerMap.Add(key, "");
                    }
                    else
                    {
                        headerMap.Add(key, HttpUtility.UrlDecode(value, Encoding.GetEncoding(charset)));
                    }
                }
            }
            return headerMap;
        }

        public static Dictionary<string, string> GetQueryMap(HttpRequest request, string charset)
        {
            Dictionary<string, string> queryMap = new Dictionary<string, string>();
            string queryString = request.Url.Query;
            if (!string.IsNullOrEmpty(queryString))
            {
                queryString = queryString.Substring(1); // 忽略?号
                string[] parameters = queryString.Split('&');
                foreach (string parameter in parameters)
                {
                    string[] kv = parameter.Split('=');
                    if (kv.Length == 2)
                    {
                        string key = HttpUtility.UrlDecode(kv[0], Encoding.GetEncoding(charset));
                        string value = HttpUtility.UrlDecode(kv[1], Encoding.GetEncoding(charset));
                        queryMap.Add(key, value);
                    }
                    else if (kv.Length == 1)
                    {
                        string key = HttpUtility.UrlDecode(kv[0], Encoding.GetEncoding(charset));
                        queryMap.Add(key, "");
                    }
                }
            }
            return queryMap;
        }

        public static Dictionary<string, string> GetFormMap(HttpRequest request)
        {
            Dictionary<string, string> formMap = new Dictionary<string, string>();
            NameValueCollection form = request.Form;
            string[] keys = form.AllKeys;
            foreach (string key in keys)
            {
                string value = request.Form[key];
                if (string.IsNullOrEmpty(value))
                {
                    formMap.Add(key, "");
                }
                else
                {
                    formMap.Add(key, value);
                }
            }
            return formMap;
        }

        public static string GetStreamAsString(HttpRequest request, string charset)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP请求体
                stream = request.InputStream;
                reader = new StreamReader(stream, Encoding.GetEncoding(charset));
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
            }
        }

        /// <summary>
        /// 检查SPI请求到达服务器端是否已经超过5分钟，如果超过则拒绝请求。
        /// </summary>
        /// <returns>true代表不超过5分钟，false代表超过5分钟。</returns>
        public static bool CheckTimestamp(HttpRequest request)
        {
            string ts = request.QueryString[TOP_FIELD_TS];
            if (!string.IsNullOrEmpty(ts))
            {
                DateTime remote = DateTime.ParseExact(ts, Constants.DATE_TIME_FORMAT, null);
                DateTime local = DateTime.Now;
                return remote.AddMinutes(5).CompareTo(local) > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查发起SPI请求的来源IP是否是TOP机房的出口IP。
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true表达IP来源合法，false代表IP来源不合法</returns>
        public static bool CheckRemoteIp(HttpRequest request)
        {
            string ip = request.UserHostAddress;
            foreach (string ipHeader in HEADER_FIELDS_IP)
            {
                string realIp = request.Headers[ipHeader];
                if (!string.IsNullOrEmpty(realIp) && !"unknown".Equals(realIp))
                {
                    ip = realIp;
                    break;
                }
            }
            return ip.StartsWith("140.205.144.") || ip.StartsWith("140.205.145.");
        }
    }
}
