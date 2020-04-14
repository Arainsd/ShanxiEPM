using hc.epm.Common;
using hc.epm.DataModel.Msg;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.Msg
{
    [ServiceContract]
    [ServiceKnownType(typeof(Msg_SMSValidate))]
    [ServiceKnownType(typeof(Msg_EmailValidate))]
    [ServiceKnownType(typeof(Msg_EmailTemplete))]
    [ServiceKnownType(typeof(Msg_EmailHistory))]
    [ServiceKnownType(typeof(Msg_EmailSetting))]
    [ServiceKnownType(typeof(Msg_Message))]
    [ServiceKnownType(typeof(Msg_MessageHistory))]
    [ServiceKnownType(typeof(Msg_MsgLog))]
    [ServiceKnownType(typeof(Msg_MessageSection))]
    [ServiceKnownType(typeof(Msg_MessageStrategy))]
    [ServiceKnownType(typeof(Msg_MessageTemplete))]
    [ServiceKnownType(typeof(Msg_Email))]
    [ServiceKnownType(typeof(Msg_SMS))]
    [ServiceKnownType(typeof(Msg_SMSHistory))]
    [ServiceKnownType(typeof(Msg_SMSSetting))]
    [ServiceKnownType(typeof(Msg_SMSTemplete))]

    public interface IMsgService
    {
        ///// <summary>
        ///// 发送消息公用接口，会根据步骤自动检测可用模板，适用的消息类型，关于重发策略由消息定时服务规定
        ///// </summary>
        ///// <param name="sendId"></param>
        ///// <param name="receiveId"></param>
        ///// <param name="step"></param>
        ///// <param name="parameters"></param>
        ///// <param name="typeList"></param>
        ///// <param name="linkURL"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<int> SendMsg(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters, string linkURL = "");

        #region 站内信及公用消息配置

        /// <summary>
        /// 添加站内信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMessage(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, string businessUrl, Dictionary<string, string> parameters, string linkURL = "");
        /// <summary>
        /// 添加站内信
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMessageBatch(long sendId, long sendCompanyId, Dictionary<long, long> recDic, MessageStep step, Dictionary<string, string> parameters, string linkURL = "");
        /// <summary>
        /// 获取站内信详情
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_Message> GetMessageModel(long id);

        /// <summary>
        /// 获取站内信列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_Message>> GetMessageList(QueryCondition qc);

        /// <summary>
        /// 删除站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteMessageByIds(List<long> ids);


        /// <summary>
        /// 获取站内信历史详情
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_MessageHistory> GetMessageHistoryModel(long id);

        /// <summary>
        /// 获取站内信历史列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_MessageHistory>> GetMessageHistoryList(QueryCondition qc);

        /// <summary>
        /// 删除历史站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteMessageHistoryByIds(List<long> ids);

        /// <summary>
        /// 添加消息环节配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSection(Msg_MessageSection model);

        /// <summary>
        /// 修改消息环节配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateSection(Msg_MessageSection model);

        /// <summary>
        /// 获取消息环节配置详情
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_MessageSection> GetSectionModel(long id);

        /// <summary>
        /// 获取消息环节配置列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_MessageSection>> GetSectionList(QueryCondition qc);

        /// <summary>
        /// 删除消息环节配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteSectionByIds(List<long> ids);


        /// <summary>
        /// 添加消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddStrategy(Msg_MessageStrategy model);

        /// <summary>
        /// 修改消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateStrategy(Msg_MessageStrategy model);

        /// <summary>
        /// 获取消息发送策略
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_MessageStrategy> GetStrategyModel(long id);

        /// <summary>
        /// 获取消息发送策略列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_MessageStrategy>> GetStrategyList(QueryCondition qc);

        /// <summary>
        /// 删除消息发送策略
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteStrategyByIds(List<long> ids);

        /// <summary>
        /// 添加站内信模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMessageTemplete(Msg_MessageTemplete model);

        /// <summary>
        /// 修改站内信模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMessageTemplete(Msg_MessageTemplete model);

        /// <summary>
        /// 获取站内信模板
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_MessageTemplete> GetMessageTempleteModel(long id);

        /// <summary>
        /// 获取站内信模板列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_MessageTemplete>> GetMessageTempleteList(QueryCondition qc);

        /// <summary>
        /// 删除站内信模板
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteMessageTempleteByIds(List<long> ids);
        #endregion

        #region  邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddEmail(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters);

        /// <summary>
        /// 邮件验证码
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddEmailValCode(string email, MessageStep step, Dictionary<string, string> parameters);
        /// <summary>
        /// 邮件链接
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddEmailValLink(string email, MessageStep step, Dictionary<string, string> parameters);

        /// <summary>
        /// 验证邮件验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="step"></param>
        /// <param name="email"></param>
        /// <param name="receiveId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_EmailValidate> ValidateEmailCode(string code, MessageStep step, string email, long receiveId = 0);

        /// <summary>
        /// 验证邮件链接
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_EmailValidate> ValidateEmailCodeByLink(string code);
        ///<summary>
        ///删除:邮件验证码
        ///</summary>
        [OperationContract]
        Result<int> DeleteEmailValidateByIds(List<long> ids);
        ///<summary>
        ///获取列表:邮件验证码
        ///</summary>
        [OperationContract]
        Result<List<Msg_EmailValidate>> GetEmailValidateList(QueryCondition qc);
        ///<summary>
        ///获取详情:邮件验证码
        ///</summary>
        [OperationContract]
        Result<Msg_EmailValidate> GetEmailValidateModel(long id);
        ///<summary>
        ///添加:邮件模板
        ///</summary>
        [OperationContract]
        Result<int> AddEmailTemplete(Msg_EmailTemplete model);
        ///<summary>
        ///修改:邮件模板
        ///</summary>
        [OperationContract]
        Result<int> UpdateEmailTemplete(Msg_EmailTemplete model);
        ///<summary>
        ///删除:邮件模板
        ///</summary>
        [OperationContract]
        Result<int> DeleteEmailTempleteByIds(List<long> ids);
        ///<summary>
        ///获取列表:邮件模板
        ///</summary>
        [OperationContract]
        Result<List<Msg_EmailTemplete>> GetEmailTempleteList(QueryCondition qc);
        ///<summary>
        ///获取详情:邮件模板
        ///</summary>
        [OperationContract]
        Result<Msg_EmailTemplete> GetEmailTempleteModel(long id);
        ///<summary>
        ///添加:历史邮件
        ///</summary>
        [OperationContract]
        Result<int> AddEmailHistory(Msg_EmailHistory model);

        ///<summary>
        ///删除:历史邮件
        ///</summary>
        [OperationContract]
        Result<int> DeleteEmailHistoryByIds(List<long> ids);
        ///<summary>
        ///获取列表:历史邮件
        ///</summary>
        [OperationContract]
        Result<List<Msg_EmailHistory>> GetEmailHistoryList(QueryCondition qc);
        ///<summary>
        ///获取详情:历史邮件
        ///</summary>
        [OperationContract]
        Result<Msg_EmailHistory> GetEmailHistoryModel(long id);
        ///<summary>
        ///添加:邮件接口设置
        ///</summary>
        [OperationContract]
        Result<int> AddEmailSetting(Msg_EmailSetting model);
        ///<summary>
        ///修改:邮件接口设置
        ///</summary>
        [OperationContract]
        Result<int> UpdateEmailSetting(Msg_EmailSetting model);
        ///<summary>
        ///删除:邮件接口设置
        ///</summary>
        [OperationContract]
        Result<int> DeleteEmailSettingByIds(List<long> ids);
        ///<summary>
        ///获取列表:邮件接口设置
        ///</summary>
        [OperationContract]
        Result<List<Msg_EmailSetting>> GetEmailSettingList(QueryCondition qc);
        ///<summary>
        ///获取详情:邮件接口设置
        ///</summary>
        [OperationContract]
        Result<Msg_EmailSetting> GetEmailSettingModel(long id);


        ///<summary>
        ///删除:邮件发送记录
        ///</summary>
        [OperationContract]
        Result<int> DeleteEmailByIds(List<long> ids);
        ///<summary>
        ///获取列表:邮件发送记录
        ///</summary>
        [OperationContract]
        Result<List<Msg_Email>> GetEmailList(QueryCondition qc);
        ///<summary>
        ///获取详情:邮件发送记录
        ///</summary>
        [OperationContract]
        Result<Msg_Email> GetEmailModel(long id);

        #endregion

        #region 短信

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSMS(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters);

        /// <summary>
        /// 短信验证码
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSMSValCode(string phone, MessageStep step, Dictionary<string, string> parameters);
        /// <summary>
        /// 短信验证码校验，接收人和电话二者需要传入一个
        /// </summary>
        /// <param name="code"></param>
        /// <param name="step"></param>
        /// <param name="phone"></param>
        /// <param name="receiveId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_SMSValidate> ValidateSMSCode(string code, MessageStep step, string phone, long receiveId = 0);
        ///<summary>
        ///删除:短信验证码
        ///</summary>
        [OperationContract]
        Result<int> DeleteSMSValidateByIds(List<long> ids);
        ///<summary>
        ///获取列表:短信验证码
        ///</summary>
        [OperationContract]
        Result<List<Msg_SMSValidate>> GetSMSValidateList(QueryCondition qc);
        ///<summary>
        ///获取详情:短信验证码
        ///</summary>
        [OperationContract]
        Result<Msg_SMSValidate> GetSMSValidateModel(long id);
        ///<summary>
        ///删除:短信记录
        ///</summary>
        [OperationContract]
        Result<int> DeleteSMSByIds(List<long> ids);
        ///<summary>
        ///获取列表:短信记录
        ///</summary>
        [OperationContract]
        Result<List<Msg_SMS>> GetSMSList(QueryCondition qc);
        ///<summary>
        ///获取详情:短信记录
        ///</summary>
        [OperationContract]
        Result<Msg_SMS> GetSMSModel(long id);
        /// <summary>
        /// 添加历史短信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSMSHistory(Msg_SMSHistory model);
        ///<summary>
        ///删除:历史短信
        ///</summary>
        [OperationContract]
        Result<int> DeleteSMSHistoryByIds(List<long> ids);
        ///<summary>
        ///获取列表:历史短信
        ///</summary>
        [OperationContract]
        Result<List<Msg_SMSHistory>> GetSMSHistoryList(QueryCondition qc);
        ///<summary>
        ///获取详情:历史短信
        ///</summary>
        [OperationContract]
        Result<Msg_SMSHistory> GetSMSHistoryModel(long id);
        ///<summary>
        ///添加:短信接口设置
        ///</summary>
        [OperationContract]
        Result<int> AddSMSSetting(Msg_SMSSetting model);
        ///<summary>
        ///修改:短信接口设置
        ///</summary>
        [OperationContract]
        Result<int> UpdateSMSSetting(Msg_SMSSetting model);
        ///<summary>
        ///删除:短信接口设置
        ///</summary>
        [OperationContract]
        Result<int> DeleteSMSSettingByIds(List<long> ids);
        ///<summary>
        ///获取列表:短信接口设置
        ///</summary>
        [OperationContract]
        Result<List<Msg_SMSSetting>> GetSMSSettingList(QueryCondition qc);
        ///<summary>
        ///获取详情:短信接口设置
        ///</summary>
        [OperationContract]
        Result<Msg_SMSSetting> GetSMSSettingModel(long id);
        ///<summary>
        ///添加:短信模板
        ///</summary>
        [OperationContract]
        Result<int> AddSMSTemplete(Msg_SMSTemplete model);
        ///<summary>
        ///修改:短信模板
        ///</summary>
        [OperationContract]
        Result<int> UpdateSMSTemplete(Msg_SMSTemplete model);
        ///<summary>
        ///删除:短信模板
        ///</summary>
        [OperationContract]
        Result<int> DeleteSMSTempleteByIds(List<long> ids);
        ///<summary>
        ///获取列表:短信模板
        ///</summary>
        [OperationContract]
        Result<List<Msg_SMSTemplete>> GetSMSTempleteList(QueryCondition qc);
        ///<summary>
        ///获取详情:短信模板
        ///</summary>
        [OperationContract]
        Result<Msg_SMSTemplete> GetSMSTempleteModel(long id);
        #endregion



        /// <summary>
        /// 审核消息发送策略
        /// </summary>
        /// <param name="strategyId">消息策略Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditStrategy(long strategyId, int type);

        /// <summary>
        /// 审核站内信模板
        /// </summary>
        /// <param name="messageTempleteId">站内信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditMessageTemplete(long messageTempleteId, int type);

        /// <summary>
        /// 审核消息环节配置
        /// </summary>
        /// <param name="sectionId">消息环节Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditSection(long sectionId, int type);

        /// <summary>
        /// 审核邮件模板
        /// </summary>
        /// <param name="emailTempleteId">邮件模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditEmailTemplete(long emailTempleteId, int type);

        /// <summary>
        /// 审核邮件设置
        /// </summary>
        /// <param name="emailTempleteId">邮件设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditEmailSetting(long emailSettingId, int type);


        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="smsTempleteId">短信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditSMSTemplete(long smsTempleteId, int type);

        /// <summary>
        /// 审核短信设置
        /// </summary>
        /// <param name="smsSettingId">短信设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditSMSSetting(long smsSettingId, int type);

        /// <summary>
        /// 读取站内信
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Msg_Message> ReadMessage(long id);

        /// <summary>
        /// 获取未读站内信
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="companyId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_Message>> GetMessageNoReadList(long userId, long companyId, int top);
    }
}
