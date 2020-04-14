using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Common
{
    public class EmailHelper
    {
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="toEmail">接收人地址</param>
        /// <param name="smtp">smtp协议地址</param>
        /// <param name="fromAccount">发送人地址</param>
        /// <param name="pwd">发送人账户密码</param>
        /// <param name="displayAccount">发送人显示名称</param>
        /// <returns></returns>
        public static Result<bool> SendEmail(string subject, string body, string toEmail, string smtp, string fromAccount, string pwd, string displayAccount)
        {
            Result<bool> result = new Result<bool>();
            try
            {

                MailAddress from = new MailAddress(fromAccount, displayAccount);
                MailAddress to = new MailAddress(toEmail);

                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.Subject = subject;//主题
                mailMessage.Body = body;//内容
                mailMessage.BodyEncoding = Encoding.Default;//正文编码
                mailMessage.IsBodyHtml = true;//设置为HTML格式
                mailMessage.Priority = MailPriority.Normal;//优先级
                //SMTP smtp.mxhichina.com 25 465
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
                    smtpClient.Host = smtp;//指定SMTP服务器
                    //smtpClient.Port = 465;
                    //smtpClient.UseDefaultCredentials = true;
                    // smtpClient.EnableSsl = true;
                    //var ac = fromAccount.Substring(0, fromAccount.IndexOf("@"));
                    smtpClient.Credentials = new NetworkCredential(fromAccount, pwd);//用户名和密码

                    smtpClient.Send(mailMessage);

                    result.Data = true;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (SmtpException ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SendEmail");
            }
            return result;
        }
    }
}
