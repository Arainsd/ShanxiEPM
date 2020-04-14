using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace hc.Plat.Common.Global
{
    public class UtilitySendMessage
    {
        private static string Url = "http://api.ums86.com:8899/sms/Api//Send.do";//短信接口地址
        private static string SMSSPCODE = "207985";//短信接口企业编号 固定值
        private static string SMSlOGINNAME = "hb_zgsy";//短信接口用户名称 固定值
        private static string SMSPWD = "gs2002";//短信接口用户密码 固定值

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="serverUrl">短信服务url</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="format">数据格式，一般是json</param>
        /// <param name="smsType">短信类型，传入值请填写normal</param>
        /// <param name="smsFreeSignName">签名</param>
        /// <param name="smsParam">短信模板参数变量  :  {\"code\":\"1234\",\"product\":\"alidayu\"}</param>
        /// <param name="recNum">短信接收号码。支持单个或多个手机号码，传入号码为11位手机号码，不能加0或+86。群发短信需传入多个号码，以英文逗号分隔，一次调用最多传入200个号码。示例：18600000000,13911111111,13322222222</param>
        /// <param name="smsTemplateCode">短信模板ID</param>
        /// <returns></returns>
        public static Result<int> SendMessageByAlidayuSms(string serverUrl, string appKey, string appSecret, string format, string smsType, string smsFreeSignName, string smsParam, string recNum, string smsTemplateCode)
        {
            Result<int> result = new Result<int>();
            try
            {
                ITopClient client = new DefaultTopClient(serverUrl, appKey, appSecret, format);
                AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();
                //req.Extend = "";
                req.SmsType = smsType;
                req.SmsFreeSignName = smsFreeSignName;
                req.SmsParam = smsParam;
                req.RecNum = recNum;
                req.SmsTemplateCode = smsTemplateCode;
                AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
                try
                {
                    if (rsp.Result != null && rsp.Result.Success)
                    {
                        result.Data = 1;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception(rsp.Body);
                    }
                }
                catch (Exception rspEx)
                {
                    result.Data = -1;
                    result.Flag = EResultFlag.Failure;
                    if (rsp.SubErrCode != null)
                        throw new Exception(GetErrorMesage(rsp.SubErrCode));
                    else
                    {
                        throw new Exception(string.Format("发送短信失败,错误信息:{0}", rspEx.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SendMessageByAlidayu");
            }

            return result;
        }

        /// <summary>
        /// 生成短信模板参数
        /// </summary>
        /// <param name="paraDic"></param>
        /// <returns></returns>
        public static Result<string> CreateSmsParam(IDictionary<string, string> paraDic)
        {
            Result<string> result = new Result<string>();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                result.Data = js.Serialize(paraDic);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CreateSmsParam");
            }

            return result;
        }

        public static Result<int> SendMessageByAlidayuVoice(string serverUrl, string appKey, string appSecret, string format, string calledNum, string calledShowNum, string voiceCode)
        {
            Result<int> result = new Result<int>();
            try
            {
                ITopClient client = new DefaultTopClient(serverUrl, appKey, appSecret, format);
                AlibabaAliqinFcVoiceNumSinglecallRequest req = new AlibabaAliqinFcVoiceNumSinglecallRequest();
                //req.Extend = "";
                req.CalledNum = calledNum;
                req.CalledShowNum = calledShowNum;
                req.VoiceCode = voiceCode;
                AlibabaAliqinFcVoiceNumSinglecallResponse rsp = client.Execute(req);
                if (rsp.Result.Success)
                {
                    result.Data = 1;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = -1;
                    result.Flag = EResultFlag.Failure;
                    throw new Exception(rsp.SubErrCode + ":" + rsp.SubErrMsg);
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SendMessageByAlidayuVoice");
            }

            return result;
        }

        /// <summary>
        /// 错误提示
        /// </summary>
        /// <param name="errCode"></param>
        /// <returns></returns>
        private static string GetErrorMesage(string errCode)
        {
            string errMessage = "";
            switch (errCode)
            {
                case "isv.BUSINESS_LIMIT_CONTROL":
                    errMessage = "您的操作过于频繁,请稍后再试!";
                    break;
                default:
                    errMessage = "发送异常,请联系客服!";
                    break;
            }
            return errMessage;
        }

        public static Result<bool> SendMessage(string phone, string content)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                string keystr = KeyCreate.SmsKeyStr();
                HttpWebRequest request = null;
                Encoding myEncoding = Encoding.GetEncoding("gb2312");
                //content = HttpUtility.UrlEncode(content, myEncoding);//http方式提交，短信内容转化为GB2312编码 
                string param =
                      string.Format(
                          "SpCode={0}&LoginName={1}&Password={2}&MessageContent={3}&UserNumber={4}&SerialNumber={5}&ScheduleTime={6}&f={7}",
                          SMSSPCODE, SMSlOGINNAME, SMSPWD, content, phone, keystr, "", "1");//定义参数内容

                request = (HttpWebRequest)WebRequest.Create(string.Format(Url + "?{0}", param));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=gb2312";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != reader.Peek())
                {
                    strBuilder.Append(reader.ReadLine());
                }
                string strResult = strBuilder.ToString();
                string[] sendResult = strResult.Split('&');
                if (strResult.IndexOf("result=0") != -1)
                {
                    result.Data = true;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    string msg = "短信发送失败!";
                    if (sendResult.Length > 1)
                    {
                        string[] msgs = sendResult[1].Split('=');
                        msg = msgs[1];
                    }
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "");
            }
            return result;
        }


    }
}
