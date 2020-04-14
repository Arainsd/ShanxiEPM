using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Msg;
using hc.epm.Service.Base;
using hc.epm.Service;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.Msg
{
    public partial class MsgService : BaseService, IMsgService
    {
        /// <summary>
        /// 发送邮件，TODO:扩展附件方法
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddEmail(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters)
        {
            Result<int> result = new Result<int>();
            try
            {
                using (MsgDataContext db = new MsgDataContext())
                {
                    //查找模板
                    var templete = db.Msg_EmailTemplete.FirstOrDefault(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                    if (templete != null)
                    {
                        Msg_Email model = new Msg_Email();
                        string con = templete.TemplateCon;
                        string title = templete.TitleCon;
                        if (parameters != null && parameters.Any())
                        {
                            //替换标题和内容参数
                            foreach (var item in parameters)
                            {
                                con = con.Replace("{" + item.Key + "}", item.Value);
                                title = title.Replace("{" + item.Key + "}", item.Value);
                            }
                        }
                        BasicDataContext basicData = new BasicDataContext();
                        model.EmailCon = con;
                        model.ReceiveId = receiveId;
                        model.ReceiveEmaile = basicData.Base_User.FirstOrDefault(i => i.Id == model.ReceiveId).Email;
                        model.SenderId = sendId;
                        model.SenderEmail = db.Msg_EmailSetting.FirstOrDefault(i => i.IsConfirm && i.IsEnable).UserName;
                        model.SendeCompanyId = sendCompanyId;
                        model.ReceiveCompanyId = receiveCompanyId;

                        model.State = false;
                        model.Step = step.ToString();
                        model.TemplateId = templete.Id;
                        model.Title = title;
                        model.SenderTime = null;
                        model.SubmissionTime = DateTime.Now;
                        model.SendCount = 0;
                        db.Msg_Email.Add(model);
                        var rows = db.SaveChanges();
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        //WriteLog(AdminModule.EmailHistory.GetText(), SystemRight.Add.GetText(), "新增邮件:" + model.Id + ":" + model.Title);
                    }
                    else
                    {
                        throw new Exception("未查找到对应邮件模板");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmail");
            }
            return result;
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
            //生成验证码
            string code = ConstString.CreateRandomNum(ConstString.RANDOMNUMCOUNT);
            parameters.Add("Code", code);

            Result<int> result = new Result<int>();
            try
            {
                //查找模板
                var templete = DataOperateMsg<Msg_EmailTemplete>.Get().Single(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                if (templete != null)
                {
                    Msg_Email model = new Msg_Email();
                    string con = templete.TemplateCon;
                    string title = templete.TitleCon;
                    if (parameters != null && parameters.Any())
                    {
                        //替换标题和内容参数
                        foreach (var item in parameters)
                        {
                            con = con.Replace("{" + item.Key + "}", item.Value);
                            title = title.Replace("{" + item.Key + "}", item.Value);
                        }
                    }
                    var receive = DataOperateBasic<Base_User>.Get().Single(i => i.Email == email);
                    long receiveId = 0;
                    if (receive != null && receive.Id > 0)
                    {
                        receiveId = receive.Id;
                    }

                    model.EmailCon = con;
                    model.ReceiveId = receiveId;
                    model.SendeCompanyId = 0;
                    model.ReceiveCompanyId = 0;
                    model.ReceiveEmaile = email;
                    model.SenderId = 0;//管理员
                    model.SenderEmail = DataOperateMsg<Msg_EmailSetting>.Get().Single(i => i.IsConfirm && i.IsEnable).UserName;
                    model.State = false;
                    model.Step = step.ToString();
                    model.TemplateId = templete.Id;
                    model.Title = title;
                    model.SenderTime = null;
                    model.SubmissionTime = DateTime.Now;

                    var rows = DataOperateMsg<Msg_Email>.Get().Add(model);

                    //添加验证码数据
                    Msg_EmailValidate valModel = new Msg_EmailValidate();
                    valModel.Code = code;
                    valModel.EmailId = model.Id;
                    valModel.ReceiveEmail = model.ReceiveEmaile;
                    valModel.ReceiveId = model.ReceiveId;
                    valModel.SendId = model.SenderId;
                    valModel.SendTime = model.SubmissionTime;
                    valModel.State = ValCodeState.UNUse.ToString();
                    valModel.ValidateType = step.ToString();
                    var seconds = LoadSettingsByKeys(Settings.EmailCodeDuration).Value.ToInt32Req();
                    valModel.ExpiredTime = DateTime.Now.AddSeconds(seconds);
                    rows = DataOperateMsg<Msg_EmailValidate>.Get().Add(valModel);


                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(AdminModule.EmailHistory.GetText(), SystemRight.Add.GetText(), "新增邮件验证码:" + model.Id + ":" + model.Title);
                }
                else
                {
                    throw new Exception("未查找到对应邮件模板");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmailValCode");
            }
            return result;
        }
        /// <summary>
        /// 构造验证邮件链接
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        private string buldValidateEmail(MessageStep step, string email, string code, out int expiredTime)
        {
            string url = LoadSettingsByKeys(Settings.ValidateEmailLink).Value;
            //var user = DataOperateBasic<Base_User>.Get().GetModel(receiveId);
            string sourceData = step.ToString() + "#" + email + "#" + code;
            //处理时解密校验
            string encryptData = DesTool.DesEncrypt(sourceData);
            expiredTime = 0;
            //构造链接
            switch (step)
            {
                case MessageStep.RegisterActive:
                    expiredTime = LoadSettingsByKeys(Settings.RegisterActiveUrlValidity).Value.ToInt32Req() * 60;
                    break;
                case MessageStep.CertificationValid:
                    expiredTime = LoadSettingsByKeys(Settings.CertificationValidTime).Value.ToInt32Req();
                    break;
                case MessageStep.FindPwd:
                    expiredTime = LoadSettingsByKeys(Settings.FindPwdUrlValidity).Value.ToInt32Req();
                    break;
                default:
                    throw new Exception("该消息类型不支持生成Email链接类邮件");
            }
            string strLink = string.Format("{0}?code={1}", url, encryptData);
            return strLink;
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
            //生成验证码
            string code = ConstString.CreateRandomNum(ConstString.RANDOMNUMCOUNT);
            Result<int> result = new Result<int>();
            try
            {
                //过期时间
                int expiredTime = 0;
                var receive = DataOperateBasic<Base_User>.Get().Single(i => i.Email == email);
                long receiveId = 0;
                if (receive != null && receive.Id > 0)
                {
                    receiveId = receive.Id;
                }
                var link = buldValidateEmail(step, email, code, out expiredTime);
                parameters.Add("ValidateLink", link);
                //查找模板
                var templete = DataOperateMsg<Msg_EmailTemplete>.Get().Single(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                if (templete != null)
                {
                    Msg_Email model = new Msg_Email();
                    string con = templete.TemplateCon;
                    string title = templete.TitleCon;
                    if (parameters != null && parameters.Any())
                    {
                        //替换标题和内容参数
                        foreach (var item in parameters)
                        {
                            con = con.Replace("{" + item.Key + "}", item.Value);
                            title = title.Replace("{" + item.Key + "}", item.Value);
                        }
                    }
                    model.EmailCon = con;
                    model.ReceiveId = receiveId;
                    model.ReceiveEmaile = email;
                    model.SenderId = 0;//管理员
                    model.SenderEmail = DataOperateMsg<Msg_EmailSetting>.Get().Single(i => i.IsConfirm && i.IsEnable).UserName;
                    model.State = false;
                    model.Step = step.ToString();
                    model.TemplateId = templete.Id;
                    model.Title = title;
                    model.SenderTime = null;
                    model.SubmissionTime = DateTime.Now;
                    var rows = DataOperateMsg<Msg_Email>.Get().Add(model);

                    //添加验证码数据
                    Msg_EmailValidate valModel = new Msg_EmailValidate();
                    valModel.Code = code;
                    valModel.EmailId = model.Id;
                    valModel.ReceiveEmail = model.ReceiveEmaile;
                    valModel.ReceiveId = model.ReceiveId;
                    valModel.SendId = model.SenderId;
                    valModel.SendTime = model.SubmissionTime;
                    valModel.State = ValCodeState.UNUse.ToString();
                    valModel.ValidateType = step.ToString();
                    valModel.ExpiredTime = DateTime.Now.AddMinutes(expiredTime);

                    rows = DataOperateMsg<Msg_EmailValidate>.Get().Add(valModel);

                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(AdminModule.EmailHistory.GetText(), SystemRight.Add.GetText(), "新增链接类邮件:" + model.Id + ":" + model.Title);
                }
                else
                {
                    throw new Exception("未查找到对应邮件模板");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmailValLink");
            }
            return result;
        }

        /// <summary>
        /// 验证邮件验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="step"></param>
        /// <param name="email"></param>
        /// <param name="receiveId"></param>
        /// <returns></returns>
        public Result<Msg_EmailValidate> ValidateEmailCode(string code, MessageStep step, string email, long receiveId = 0)
        {
            return ValidateEmailCode(code, step, receiveId, email);
        }
        /// <summary>
        /// 验证邮件链接
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Msg_EmailValidate> ValidateEmailCodeByLink(string code)
        {
            return ValidateEmailCode(code, MessageStep.Default);
        }
        /// <summary>
        /// 验证码校验
        /// 如果是邮件链接，步骤/接收人/邮箱无须传入
        /// 如果是验证码，则接收人和邮箱二者必须传入一个
        /// </summary>
        /// <param name="code"></param>
        /// <param name="receiveId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private Result<Msg_EmailValidate> ValidateEmailCode(string code, MessageStep step, long receiveId = 0, string email = "")
        {
            Result<Msg_EmailValidate> result = new Result<Msg_EmailValidate>();
            try
            {
                string strStep = step.ToString();
                //链接里的验证码校验
                if (receiveId == 0 && string.IsNullOrEmpty(email))
                {
                    //解密校验
                    string sourceData = DesTool.DesDecrypt(code);
                    IList<string> decryptData = sourceData.SplitString("#");
                    strStep = decryptData[0];
                    email = decryptData[1];
                    code = decryptData[2];
                }
                var model = DataOperateMsg<Msg_EmailValidate>.Get().Single(i => (i.ReceiveId == receiveId || i.ReceiveEmail == email) && i.Code == code && i.ValidateType == strStep);
                if (model == null)
                {
                    throw new Exception("验证码错误");
                }
                else
                {
                    if (model.State == ValCodeState.Used.ToString())
                    {
                        throw new Exception("验证码已经使用过");
                    }
                    if (model.ExpiredTime < DateTime.Now)
                    {
                        throw new Exception("验证码已过期");
                    }
                    //更新验证码
                    model.State = ValCodeState.Used.ToString();
                    DataOperateMsg<Msg_EmailValidate>.Get().Update(model);
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ValidateEmailCode");
            }
            return result;
        }

        ///<summary>
        ///删除:邮件验证码
        ///</summary>
        public Result<int> DeleteEmailValidateByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_EmailValidate>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_EmailValidate>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(MessageType.Email.GetText(), SystemRight.Add.GetText(), "批量删除邮件验证码: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteEmailValidateByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:邮件验证码
        ///</summary>
        public Result<List<Msg_EmailValidate>> GetEmailValidateList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_EmailValidate>> result = new Result<List<Msg_EmailValidate>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_EmailValidate>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailValidateList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:邮件验证码
        ///</summary>
        public Result<Msg_EmailValidate> GetEmailValidateModel(long id)
        {
            Result<Msg_EmailValidate> result = new Result<Msg_EmailValidate>();
            try
            {
                var model = DataOperateMsg<Msg_EmailValidate>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailValidateModel");
            }
            return result;
        }
        ///<summary>
        ///添加:邮件模板
        ///</summary>
        public Result<int> AddEmailTemplete(Msg_EmailTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_EmailTemplete>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailTemplete.GetText(), SystemRight.Add.GetText(), "新增邮件模板: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmailTemplete");
            }
            return result;
        }
        ///<summary>
        ///修改:邮件模板
        ///</summary>
        public Result<int> UpdateEmailTemplete(Msg_EmailTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_EmailTemplete>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailTemplete.GetText(), SystemRight.Add.GetText(), "修改邮件模板: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateEmailTemplete");
            }
            return result;
        }
        /// <summary>
        /// 审核邮件模板
        /// </summary>
        /// <param name="emailTempleteId">邮件模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditEmailTemplete(long emailTempleteId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_EmailTemplete>.Get().GetModel(emailTempleteId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_EmailTemplete>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                if (type == 1)
                {
                    WriteStateLog(model, (!model.IsEnable).ToString(), (model.IsEnable).ToString());
                }
                else if (type == 2)
                {
                    WriteStateLog(model, (!model.IsConfirm).ToString(), (model.IsConfirm).ToString());
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditEmailTemplete");
            }
            return result;
        }

        ///<summary>
        ///删除:邮件模板
        ///</summary>
        public Result<int> DeleteEmailTempleteByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_EmailTemplete>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_EmailTemplete>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailTemplete.GetText(), SystemRight.Add.GetText(), "批量删除邮件模板: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteEmailTempleteByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:邮件模板
        ///</summary>
        public Result<List<Msg_EmailTemplete>> GetEmailTempleteList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_EmailTemplete>> result = new Result<List<Msg_EmailTemplete>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_EmailTemplete>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailTempleteList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:邮件模板
        ///</summary>
        public Result<Msg_EmailTemplete> GetEmailTempleteModel(long id)
        {
            Result<Msg_EmailTemplete> result = new Result<Msg_EmailTemplete>();
            try
            {
                var model = DataOperateMsg<Msg_EmailTemplete>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailTempleteModel");
            }
            return result;
        }
        ///<summary>
        ///添加:历史邮件
        ///</summary>
        public Result<int> AddEmailHistory(Msg_EmailHistory model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_EmailHistory>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailHistory.GetText(), SystemRight.Add.GetText(), "新增历史邮件: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmailHistory");
            }
            return result;
        }

        ///<summary>
        ///删除:历史邮件
        ///</summary>
        public Result<int> DeleteEmailHistoryByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_EmailHistory>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_EmailHistory>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailHistory.GetText(), SystemRight.Add.GetText(), "批量删除历史邮件: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteEmailHistoryByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:历史邮件
        ///</summary>
        public Result<List<Msg_EmailHistory>> GetEmailHistoryList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_EmailHistory>> result = new Result<List<Msg_EmailHistory>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_EmailHistory>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailHistoryList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:历史邮件
        ///</summary>
        public Result<Msg_EmailHistory> GetEmailHistoryModel(long id)
        {
            Result<Msg_EmailHistory> result = new Result<Msg_EmailHistory>();
            try
            {
                var model = DataOperateMsg<Msg_EmailHistory>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailHistoryModel");
            }
            return result;
        }
        ///<summary>
        ///添加:邮件接口设置
        ///</summary>
        public Result<int> AddEmailSetting(Msg_EmailSetting model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_EmailSetting>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailSetting.GetText(), SystemRight.Add.GetText(), "新增邮件接口设置: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddEmailSetting");
            }
            return result;
        }
        ///<summary>
        ///修改:邮件接口设置
        ///</summary>
        public Result<int> UpdateEmailSetting(Msg_EmailSetting model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_EmailSetting>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailSetting.GetText(), SystemRight.Add.GetText(), "修改邮件接口设置: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateEmailSetting");
            }
            return result;
        }

        /// <summary>
        /// 审核邮件设置
        /// </summary>
        /// <param name="emailTempleteId">邮件设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditEmailSetting(long emailSettingId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_EmailSetting>.Get().GetModel(emailSettingId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_EmailSetting>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                if (type == 1)
                {
                    WriteStateLog(model, (!model.IsEnable).ToString(), (model.IsEnable).ToString());
                }
                else if (type == 2)
                {
                    WriteStateLog(model, (!model.IsConfirm).ToString(), (model.IsConfirm).ToString());
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditEmailSetting");
            }
            return result;
        }


        ///<summary>
        ///删除:邮件接口设置
        ///</summary>
        public Result<int> DeleteEmailSettingByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_EmailSetting>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_EmailSetting>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.EmailSetting.GetText(), SystemRight.Add.GetText(), "批量删除邮件接口设置: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteEmailSettingByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:邮件接口设置
        ///</summary>
        public Result<List<Msg_EmailSetting>> GetEmailSettingList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_EmailSetting>> result = new Result<List<Msg_EmailSetting>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_EmailSetting>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailSettingList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:邮件接口设置
        ///</summary>
        public Result<Msg_EmailSetting> GetEmailSettingModel(long id)
        {
            Result<Msg_EmailSetting> result = new Result<Msg_EmailSetting>();
            try
            {
                var model = DataOperateMsg<Msg_EmailSetting>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailSettingModel");
            }
            return result;
        }


        ///<summary>
        ///删除:邮件发送记录
        ///</summary>
        public Result<int> DeleteEmailByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_Email>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_Email>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(MessageType.Email.GetText(), SystemRight.Add.GetText(), "批量删除邮件发送记录: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteEmailByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:邮件发送记录
        ///</summary>
        public Result<List<Msg_Email>> GetEmailList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_Email>> result = new Result<List<Msg_Email>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_Email>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:邮件发送记录
        ///</summary>
        public Result<Msg_Email> GetEmailModel(long id)
        {
            Result<Msg_Email> result = new Result<Msg_Email>();
            try
            {
                var model = DataOperateMsg<Msg_Email>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetEmailModel");
            }
            return result;
        }

    }
}
