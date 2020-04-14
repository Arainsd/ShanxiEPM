using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Msg;
using hc.Plat.Cache.Helper;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.MsgTimer
{
    public class MessageTimer
    {

        public MessageTimer()
        {

        }
        public object objLock = new object();
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public List<Msg_Email> TimerSendEmail()
        {
            MsgDataContext db = new MsgDataContext();
            List<Msg_Email> list = new List<Msg_Email>();

            //邮件设置
            var strategy = GetStrategy(MessageType.Email);
            var setting = GetEmailSetting();
            if (strategy == null || setting == null)
            {
                return null;
            }
            //待发邮件
            list = db.Msg_Email.Where(i => i.State == false || !i.SenderTime.HasValue).ToList();
            List<Msg_Email> result = new List<Msg_Email>();

            foreach (var item in list)
            {
                //发送
                MessageHelper.SendEmailAsync(item, setting, strategy);
            }
            return list;
        }



        /// <summary>
        /// 发送短信
        /// </summary>
        /// <returns></returns>
        public List<Msg_SMS> TimerSendSMS()
        {

            MsgDataContext db = new MsgDataContext();
            List<Msg_SMS> list = new List<Msg_SMS>();

            //短信设置
            var setting = GetSMSSetting();
            var strategy = GetStrategy(MessageType.SMS);
            list = db.Msg_SMS.Where(i => i.State == false || !i.SenderTime.HasValue).ToList();
            List<Msg_SMS> result = new List<Msg_SMS>();
            foreach (var item in list)
            {
                //发送
                MessageHelper.SendSMSAsync(item, setting, strategy);
            }
            return list;
        }
        /// <summary>
        /// 获取消息发送策略
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Msg_MessageStrategy GetStrategy(MessageType type)
        {
            MsgDataContext db = new MsgDataContext();
            Msg_MessageStrategy model = null;

            var list = db.Msg_MessageStrategy.ToList();

            model = list.FirstOrDefault(i => i.IsConfirm && i.IsEnable && i.Type == type.ToString());
            return model;

        }
        /// <summary>
        /// 获取邮件设置
        /// </summary>
        /// <returns></returns>
        public Msg_EmailSetting GetEmailSetting()
        {
            MsgDataContext db = new MsgDataContext();
            Msg_EmailSetting model = null;

            model = db.Msg_EmailSetting.FirstOrDefault(i => i.IsConfirm && i.IsEnable);
            return model;
        }
        /// <summary>
        /// 获取短信设置
        /// </summary>
        /// <returns></returns>
        public Msg_SMSSetting GetSMSSetting()
        {
            MsgDataContext db = new MsgDataContext();
            Msg_SMSSetting model = null;

            model = db.Msg_SMSSetting.FirstOrDefault(i => i.IsConfirm && i.IsEnable);
            return model;

        }

        public List<Msg_SMS> TimerSendSMSHB()
        {
            MsgDataContext db = new MsgDataContext();
            List<Msg_SMS> list = new List<Msg_SMS>();
            
            list = db.Msg_SMS.Where(i => i.State == false || !i.SenderTime.HasValue).ToList();
            List<Msg_SMS> result = new List<Msg_SMS>();
            foreach (var item in list)
            {
                //发送
                MessageHelper.HBSendSMSAsync(item);
            }
            return list;
        }

    }
}
