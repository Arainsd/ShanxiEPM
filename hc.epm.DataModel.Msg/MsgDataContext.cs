using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Msg
{
    public class MsgDataContext : DbContext
    {
        public MsgDataContext() : base("msgConnectionString")
        {
        }

        ///<summary>
        ///短信验证码
        ///</summary>
        public DbSet<Msg_SMSValidate> Msg_SMSValidate { get; set; }
        ///<summary>
        ///邮件验证码
        ///</summary>
        public DbSet<Msg_EmailValidate> Msg_EmailValidate { get; set; }
        ///<summary>
        ///邮件模板
        ///</summary>
        public DbSet<Msg_EmailTemplete> Msg_EmailTemplete { get; set; }
        ///<summary>
        ///历史邮件
        ///</summary>
        public DbSet<Msg_EmailHistory> Msg_EmailHistory { get; set; }
        ///<summary>
        ///邮件接口设置
        ///</summary>
        public DbSet<Msg_EmailSetting> Msg_EmailSetting { get; set; }
        ///<summary>
        ///站内信
        ///</summary>
        public DbSet<Msg_Message> Msg_Message { get; set; }
        ///<summary>
        ///历史站内信
        ///</summary>
        public DbSet<Msg_MessageHistory> Msg_MessageHistory { get; set; }
        ///<summary>
        ///发送日志
        ///</summary>
        public DbSet<Msg_MsgLog> Msg_MsgLog { get; set; }
        ///<summary>
        ///消息环节配置
        ///</summary>
        public DbSet<Msg_MessageSection> Msg_MessageSection { get; set; }
        ///<summary>
        ///消息发送策略
        ///</summary>
        public DbSet<Msg_MessageStrategy> Msg_MessageStrategy { get; set; }
        ///<summary>
        ///站内信模板
        ///</summary>
        public DbSet<Msg_MessageTemplete> Msg_MessageTemplete { get; set; }
        ///<summary>
        ///邮件发送记录
        ///</summary>
        public DbSet<Msg_Email> Msg_Email { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<Msg_SMS> Msg_SMS { get; set; }
        ///<summary>
        ///历史短信
        ///</summary>
        public DbSet<Msg_SMSHistory> Msg_SMSHistory { get; set; }
        ///<summary>
        ///短信接口设置
        ///</summary>
        public DbSet<Msg_SMSSetting> Msg_SMSSetting { get; set; }
        ///<summary>
        ///短信模板
        ///</summary>
        public DbSet<Msg_SMSTemplete> Msg_SMSTemplete { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}

