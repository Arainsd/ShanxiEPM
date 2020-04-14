using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using hc.epm.Service.Basic;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Basic;
using hc.epm.Common;
using hc.epm.ViewModel;
using hc.epm.Service.Msg;
using hc.epm.DataModel.Msg;

namespace hc.epm.Admin.ClientProxy
{
    public class MessageClientProxy : ClientBase<IMsgService>, IMsgService
    {
        public MessageClientProxy(hc.Plat.Common.Global.ClientProxyExType cpet)
        {

            //传输当前用户的信息；
            ApplicationContext.Current.UserID = cpet.UserID;
            ApplicationContext.Current.WebIP = cpet.IP_WebServer;
            ApplicationContext.Current.ClientIP = cpet.IP_Client;


            /*以下密码是用作在应用服务器中使用程序验证密码的作用*/
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            string user = "";
            string pass = "";
            string msg = DesTool.LoadCertUserPass(FilePath, out user, out pass);
            if (msg != "")
            {
                throw new Exception(msg);
            }
            ClientCredentials.UserName.UserName = user;
            ClientCredentials.UserName.Password = pass;
            /*OK*/
        }
        /// <summary>
        /// 站内信发送
        /// </summary>
        /// <param name="sendId"></param>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <param name="linkURL"></param>
        /// <returns></returns>
        public Result<int> AddMessage(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, string businessUrl, Dictionary<string, string> parameters, string linkURL = "")
        {
            return base.Channel.AddMessage(sendId, sendCompanyId, receiveId, receiveCompanyId, step, businessUrl, parameters, linkURL);
        }
        public Result<int> AddMessageBatch(long sendId, long sendCompanyId, Dictionary<long, long> recDic, MessageStep step, Dictionary<string, string> parameters, string linkURL = "")
        {
            return base.Channel.AddMessageBatch(sendId, sendCompanyId, recDic, step, parameters, linkURL);
        }
        /// <summary>
        /// 添加消息模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMessageTemplete(Msg_MessageTemplete model)
        {
            return base.Channel.AddMessageTemplete(model);
        }
        /// <summary>
        /// 添加消息环节配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSection(Msg_MessageSection model)
        {
            return base.Channel.AddSection(model);
        }
        /// <summary>
        /// 添加消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddStrategy(Msg_MessageStrategy model)
        {
            return base.Channel.AddStrategy(model);
        }
        /// <summary>
        /// 批量删除站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageByIds(List<long> ids)
        {
            return base.Channel.DeleteMessageByIds(ids);
        }
        /// <summary>
        /// 批量删除历史站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageHistoryByIds(List<long> ids)
        {
            return base.Channel.DeleteMessageHistoryByIds(ids);
        }
        /// <summary>
        /// 删除站内信模板
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageTempleteByIds(List<long> ids)
        {
            return base.Channel.DeleteMessageTempleteByIds(ids);
        }
        /// <summary>
        /// 删除消息环节配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteSectionByIds(List<long> ids)
        {
            return base.Channel.DeleteSectionByIds(ids);
        }
        /// <summary>
        /// 删除消息发送策略
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteStrategyByIds(List<long> ids)
        {
            return base.Channel.DeleteStrategyByIds(ids);
        }
        /// <summary>
        /// 获取历史站内信列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageHistory>> GetMessageHistoryList(QueryCondition qc)
        {
            return base.Channel.GetMessageHistoryList(qc);
        }
        /// <summary>
        /// 获取历史站内信详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageHistory> GetMessageHistoryModel(long id)
        {
            return base.Channel.GetMessageHistoryModel(id);
        }
        /// <summary>
        /// 获取站内信列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_Message>> GetMessageList(QueryCondition qc)
        {
            return base.Channel.GetMessageList(qc);
        }
        /// <summary>
        /// 获取站内信详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_Message> GetMessageModel(long id)
        {
            return base.Channel.GetMessageModel(id);
        }
        /// <summary>
        /// 获取站内信模板列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageTemplete>> GetMessageTempleteList(QueryCondition qc)
        {
            return base.Channel.GetMessageTempleteList(qc);
        }
        /// <summary>
        /// 获取站内信模板详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageTemplete> GetMessageTempleteModel(long id)
        {
            return base.Channel.GetMessageTempleteModel(id);
        }
        /// <summary>
        /// 获取消息环节配置列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageSection>> GetSectionList(QueryCondition qc)
        {
            return base.Channel.GetSectionList(qc);
        }
        /// <summary>
        /// 获取消息环节配置详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageSection> GetSectionModel(long id)
        {
            return base.Channel.GetSectionModel(id);
        }
        /// <summary>
        /// 获取消息发送策略列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageStrategy>> GetStrategyList(QueryCondition qc)
        {
            return base.Channel.GetStrategyList(qc);
        }
        /// <summary>
        /// 获取消息发送策略详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageStrategy> GetStrategyModel(long id)
        {
            return base.Channel.GetStrategyModel(id);
        }
        /// <summary>
        /// 修改站内信模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateMessageTemplete(Msg_MessageTemplete model)
        {
            return base.Channel.UpdateMessageTemplete(model);
        }
        /// <summary>
        /// 修改消息环节配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateSection(Msg_MessageSection model)
        {
            return base.Channel.UpdateSection(model);
        }
        /// <summary>
        /// 修改消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateStrategy(Msg_MessageStrategy model)
        {
            return base.Channel.UpdateStrategy(model);
        }
        ///<summary>
        ///删除:短信验证码
        ///</summary>
        public Result<int> DeleteSMSValidateByIds(List<long> ids)
        {
            return base.Channel.DeleteSMSValidateByIds(ids);
        }
        ///<summary>
        ///获取列表:短信验证码
        ///</summary>
        public Result<List<Msg_SMSValidate>> GetSMSValidateList(QueryCondition qc)
        {
            return base.Channel.GetSMSValidateList(qc);
        }
        ///<summary>
        ///获取详情:短信验证码
        ///</summary>
        public Result<Msg_SMSValidate> GetSMSValidateModel(long id)
        {
            return base.Channel.GetSMSValidateModel(id);
        }
        ///<summary>
        ///添加:短信记录
        ///</summary>
        public Result<int> AddSMS(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters)
        {
            return base.Channel.AddSMS(sendId, sendCompanyId, receiveId, receiveCompanyId, step, parameters);
        }
        ///<summary>
        ///删除:短信记录
        ///</summary>
        public Result<int> DeleteSMSByIds(List<long> ids)
        {
            return base.Channel.DeleteSMSByIds(ids);
        }
        ///<summary>
        ///获取列表:短信记录
        ///</summary>
        public Result<List<Msg_SMS>> GetSMSList(QueryCondition qc)
        {
            return base.Channel.GetSMSList(qc);
        }
        ///<summary>
        ///获取详情:短信记录
        ///</summary>
        public Result<Msg_SMS> GetSMSModel(long id)
        {
            return base.Channel.GetSMSModel(id);
        }
        ///<summary>
        ///添加:历史短信
        ///</summary>
        public Result<int> AddSMSHistory(Msg_SMSHistory model)
        {
            return base.Channel.AddSMSHistory(model);
        }
        ///<summary>
        ///删除:历史短信
        ///</summary>
        public Result<int> DeleteSMSHistoryByIds(List<long> ids)
        {
            return base.Channel.DeleteSMSHistoryByIds(ids);
        }
        ///<summary>
        ///获取列表:历史短信
        ///</summary>
        public Result<List<Msg_SMSHistory>> GetSMSHistoryList(QueryCondition qc)
        {
            return base.Channel.GetSMSHistoryList(qc);
        }
        ///<summary>
        ///获取详情:历史短信
        ///</summary>
        public Result<Msg_SMSHistory> GetSMSHistoryModel(long id)
        {
            return base.Channel.GetSMSHistoryModel(id);
        }
        ///<summary>
        ///添加:短信接口设置
        ///</summary>
        public Result<int> AddSMSSetting(Msg_SMSSetting model)
        {
            return base.Channel.AddSMSSetting(model);
        }
        ///<summary>
        ///修改:短信接口设置
        ///</summary>
        public Result<int> UpdateSMSSetting(Msg_SMSSetting model)
        {
            return base.Channel.UpdateSMSSetting(model);
        }
        ///<summary>
        ///删除:短信接口设置
        ///</summary>
        public Result<int> DeleteSMSSettingByIds(List<long> ids)
        {
            return base.Channel.DeleteSMSSettingByIds(ids);
        }
        ///<summary>
        ///获取列表:短信接口设置
        ///</summary>
        public Result<List<Msg_SMSSetting>> GetSMSSettingList(QueryCondition qc)
        {
            return base.Channel.GetSMSSettingList(qc);
        }
        ///<summary>
        ///获取详情:短信接口设置
        ///</summary>
        public Result<Msg_SMSSetting> GetSMSSettingModel(long id)
        {
            return base.Channel.GetSMSSettingModel(id);
        }
        ///<summary>
        ///添加:短信模板
        ///</summary>
        public Result<int> AddSMSTemplete(Msg_SMSTemplete model)
        {
            return base.Channel.AddSMSTemplete(model);
        }
        ///<summary>
        ///修改:短信模板
        ///</summary>
        public Result<int> UpdateSMSTemplete(Msg_SMSTemplete model)
        {
            return base.Channel.UpdateSMSTemplete(model);
        }
        ///<summary>
        ///删除:短信模板
        ///</summary>
        public Result<int> DeleteSMSTempleteByIds(List<long> ids)
        {
            return base.Channel.DeleteSMSTempleteByIds(ids);
        }
        ///<summary>
        ///获取列表:短信模板
        ///</summary>
        public Result<List<Msg_SMSTemplete>> GetSMSTempleteList(QueryCondition qc)
        {
            return base.Channel.GetSMSTempleteList(qc);
        }
        ///<summary>
        ///获取详情:短信模板
        ///</summary>
        public Result<Msg_SMSTemplete> GetSMSTempleteModel(long id)
        {
            return base.Channel.GetSMSTempleteModel(id);
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendId"></param>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddEmail(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters)
        {
            return base.Channel.AddEmail(sendId, sendCompanyId, receiveId, receiveCompanyId, step, parameters);
        }
        ///<summary>
        ///删除:邮件验证码
        ///</summary>
        public Result<int> DeleteEmailValidateByIds(List<long> ids)
        {
            return base.Channel.DeleteEmailValidateByIds(ids);
        }
        ///<summary>
        ///获取列表:邮件验证码
        ///</summary>
        public Result<List<Msg_EmailValidate>> GetEmailValidateList(QueryCondition qc)
        {
            return base.Channel.GetEmailValidateList(qc);
        }
        ///<summary>
        ///获取详情:邮件验证码
        ///</summary>
        public Result<Msg_EmailValidate> GetEmailValidateModel(long id)
        {
            return base.Channel.GetEmailValidateModel(id);
        }
        ///<summary>
        ///添加:邮件模板
        ///</summary>
        public Result<int> AddEmailTemplete(Msg_EmailTemplete model)
        {
            return base.Channel.AddEmailTemplete(model);
        }
        ///<summary>
        ///修改:邮件模板
        ///</summary>
        public Result<int> UpdateEmailTemplete(Msg_EmailTemplete model)
        {
            return base.Channel.UpdateEmailTemplete(model);
        }
        ///<summary>
        ///删除:邮件模板
        ///</summary>
        public Result<int> DeleteEmailTempleteByIds(List<long> ids)
        {
            return base.Channel.DeleteEmailTempleteByIds(ids);
        }
        ///<summary>
        ///获取列表:邮件模板
        ///</summary>
        public Result<List<Msg_EmailTemplete>> GetEmailTempleteList(QueryCondition qc)
        {
            return base.Channel.GetEmailTempleteList(qc);
        }
        ///<summary>
        ///获取详情:邮件模板
        ///</summary>
        public Result<Msg_EmailTemplete> GetEmailTempleteModel(long id)
        {
            return base.Channel.GetEmailTempleteModel(id);
        }
        ///<summary>
        ///添加:历史邮件
        ///</summary>
        public Result<int> AddEmailHistory(Msg_EmailHistory model)
        {
            return base.Channel.AddEmailHistory(model);
        }
        ///<summary>
        ///删除:历史邮件
        ///</summary>
        public Result<int> DeleteEmailHistoryByIds(List<long> ids)
        {
            return base.Channel.DeleteEmailHistoryByIds(ids);
        }
        ///<summary>
        ///获取列表:历史邮件
        ///</summary>
        public Result<List<Msg_EmailHistory>> GetEmailHistoryList(QueryCondition qc)
        {
            return base.Channel.GetEmailHistoryList(qc);
        }
        ///<summary>
        ///获取详情:历史邮件
        ///</summary>
        public Result<Msg_EmailHistory> GetEmailHistoryModel(long id)
        {
            return base.Channel.GetEmailHistoryModel(id);
        }
        ///<summary>
        ///添加:邮件接口设置
        ///</summary>
        public Result<int> AddEmailSetting(Msg_EmailSetting model)
        {
            return base.Channel.AddEmailSetting(model);
        }
        ///<summary>
        ///修改:邮件接口设置
        ///</summary>
        public Result<int> UpdateEmailSetting(Msg_EmailSetting model)
        {
            return base.Channel.UpdateEmailSetting(model);
        }
        ///<summary>
        ///删除:邮件接口设置
        ///</summary>
        public Result<int> DeleteEmailSettingByIds(List<long> ids)
        {
            return base.Channel.DeleteEmailSettingByIds(ids);
        }
        ///<summary>
        ///获取列表:邮件接口设置
        ///</summary>
        public Result<List<Msg_EmailSetting>> GetEmailSettingList(QueryCondition qc)
        {
            return base.Channel.GetEmailSettingList(qc);
        }
        ///<summary>
        ///获取详情:邮件接口设置
        ///</summary>
        public Result<Msg_EmailSetting> GetEmailSettingModel(long id)
        {
            return base.Channel.GetEmailSettingModel(id);
        }
        ///<summary>
        ///删除:邮件发送记录
        ///</summary>
        public Result<int> DeleteEmailByIds(List<long> ids)
        {
            return base.Channel.DeleteEmailByIds(ids);
        }
        ///<summary>
        ///获取列表:邮件发送记录
        ///</summary>
        public Result<List<Msg_Email>> GetEmailList(QueryCondition qc)
        {
            return base.Channel.GetEmailList(qc);
        }
        ///<summary>
        ///获取详情:邮件发送记录
        ///</summary>
        public Result<Msg_Email> GetEmailModel(long id)
        {
            return base.Channel.GetEmailModel(id);
        }
        /// <summary>
        /// 审核消息发送策略
        /// </summary>
        /// <param name="strategyId">消息策略Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditStrategy(long strategyId, int type)
        {
            return base.Channel.AuditStrategy(strategyId, type);
        }
        /// <summary>
        /// 审核站内信模板
        /// </summary>
        /// <param name="messageTempleteId">站内信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditMessageTemplete(long messageTempleteId, int type)
        {
            return base.Channel.AuditMessageTemplete(messageTempleteId, type);
        }
        /// <summary>
        /// 审核消息环节配置
        /// </summary>
        /// <param name="sectionId">消息环节Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSection(long sectionId, int type)
        {
            return base.Channel.AuditSection(sectionId, type);
        }
        /// <summary>
        /// 审核邮件模板
        /// </summary>
        /// <param name="emailTempleteId">邮件模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditEmailTemplete(long emailTempleteId, int type)
        {
            return base.Channel.AuditEmailTemplete(emailTempleteId, type);
        }
        /// <summary>
        /// 审核邮件设置
        /// </summary>
        /// <param name="emailTempleteId">邮件设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditEmailSetting(long emailSettingId, int type)
        {
            return base.Channel.AuditEmailSetting(emailSettingId, type);
        }
        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="smsTempleteId">短信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSMSTemplete(long smsTempleteId, int type)
        {
            return base.Channel.AuditSMSTemplete(smsTempleteId, type);
        }
        /// <summary>
        /// 审核短信设置
        /// </summary>
        /// <param name="smsSettingId">短信设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSMSSetting(long smsSettingId, int type)
        {
            return base.Channel.AuditSMSSetting(smsSettingId, type);
        }
        /// <summary>
        /// 邮件验证码
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddEmailValCode(string email, MessageStep step, Dictionary<string, string> parameters)
        {
            return base.Channel.AddEmailValCode(email, step, parameters);
        }
        /// <summary>
        /// 邮件链接
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddEmailValLink(string email, MessageStep step, Dictionary<string, string> parameters)
        {
            return base.Channel.AddEmailValLink(email, step, parameters);
        }
        /// <summary>
        /// 邮件验证码校验
        /// </summary>
        /// <param name="code"></param>
        /// <param name="step"></param>
        /// <param name="email"></param>
        /// <param name="receiveId"></param>
        /// <returns></returns>
        public Result<Msg_EmailValidate> ValidateEmailCode(string code, MessageStep step, string email, long receiveId = 0)
        {
            return base.Channel.ValidateEmailCode(code, step, email, receiveId);
        }
        /// <summary>
        /// 邮件链接验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Msg_EmailValidate> ValidateEmailCodeByLink(string code)
        {
            return base.Channel.ValidateEmailCodeByLink(code);
        }
        /// <summary>
        /// 短信验证码
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddSMSValCode(string phone, MessageStep step, Dictionary<string, string> parameters)
        {
            return base.Channel.AddSMSValCode(phone, step, parameters);
        }
        /// <summary>
        /// 短信验证码校验
        /// </summary>
        /// <param name="code"></param>
        /// <param name="step"></param>
        /// <param name="phone"></param>
        /// <param name="receiveId"></param>
        /// <returns></returns>
        public Result<Msg_SMSValidate> ValidateSMSCode(string code, MessageStep step, string phone, long receiveId = -1)
        {
            return base.Channel.ValidateSMSCode(code, step, phone, receiveId);
        }
        /// <summary>
        /// 读取站内信
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_Message> ReadMessage(long id)
        {
            return base.Channel.ReadMessage(id);
        }
        /// <summary>
        /// 获取未读站内信
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public Result<List<Msg_Message>> GetMessageNoReadList(long userId, long companyId, int top)
        {
            return base.Channel.GetMessageNoReadList(userId, companyId, top);
        }
    }
}
