using hc.epm.Common;
using hc.epm.DataModel.Msg;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.MsgTimer
{
    public class MessageHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setting"></param>
        /// <param name="strategy"></param>
        /// <param name="displayName"></param>
        public static async void SendEmailAsync(Msg_Email model, Msg_EmailSetting setting, Msg_MessageStrategy strategy, string displayName = "")
        {

            await Task.Run(() =>
             {
                 Result<bool> result = null;
                 result = EmailHelper.SendEmail(model.Title, model.EmailCon, model.ReceiveEmaile, setting.MailProtocol, setting.UserName, setting.PassWord, displayName);
                 //以下为回调
                 //更新状态
                 model.SendCount += 1;
                 model.State = result.Data;
                 //超出重发次数
                 if (model.SendCount > strategy.ResendStra)
                 {
                     model.State = true;
                 }
                 model.SenderTime = DateTime.Now;
                 //执行更新
                 MsgDataContext db = new MsgDataContext();
                 var entry = db.Entry(model);
                 db.Entry<Msg_Email>(model).State = EntityState.Modified;
                 db.SaveChanges();
             });



        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="model"></param>
        /// <param name="setting"></param>
        /// <param name="strategy"></param>
        public static async void SendSMSAsync(Msg_SMS model, Msg_SMSSetting setting, Msg_MessageStrategy strategy)
        {

            await Task.Run(() =>
            {
                Result<int> result = null;
                //阿里大鱼
                result = UtilitySendMessage.SendMessageByAlidayuSms(setting.URL,
                    setting.AppKey, setting.AppSecret, ConstString.SMSFORMAT, ConstString.SMSTYPE,
                    model.SignName, model.Params, model.ReceivePhone, model.ServerNo);
                //以下为回调
                //更新状态
                model.SendCount += 1;
                model.State = result.Data == 1;
                //超出重发次数
                if (model.SendCount > strategy.ResendStra)
                {
                    model.State = true;
                }
                model.SenderTime = DateTime.Now;

                //执行更新
                MsgDataContext db = new MsgDataContext();
                var entry = db.Entry(model);
                db.Entry<Msg_SMS>(model).State = EntityState.Modified;
                db.SaveChanges();
            });
        }


        public static async void HBSendSMSAsync(Msg_SMS model)
        {
            await Task.Run(() =>
            {
                Result<bool> result = null;
                //阿里大鱼
                result = UtilitySendMessage.SendMessage(model.ReceivePhone, model.SmsCon);
                //以下为回调
                //更新状态
                model.State = result.Data;
                model.SenderTime = DateTime.Now;

                //执行更新
                MsgDataContext db = new MsgDataContext();
                var entry = db.Entry(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            });
        }
    }
}
