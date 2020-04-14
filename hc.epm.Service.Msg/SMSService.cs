using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Msg;
using hc.epm.Service.Base;
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
        /// 发送短信
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result<int> AddSMS(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters)
        {
            Result<int> result = new Result<int>();
            try
            {
                using (MsgDataContext db = new MsgDataContext())
                {
                    //查找模板
                    var templete = db.Msg_SMSTemplete.FirstOrDefault(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                    if (templete != null)
                    {
                        Msg_SMS model = new Msg_SMS();
                        string con = templete.TemplateCon;
                        if (parameters != null && parameters.Any())
                        {
                            //替换内容参数
                            foreach (var item in parameters)
                            {
                                con = con.Replace("{" + item.Key + "}", item.Value);
                            }
                        }
                        BasicDataContext basicData = new BasicDataContext();
                        var receive = basicData.Base_User.FirstOrDefault(i => i.Id == receiveId);
                        model.ReceivePhone = receive.Phone;
                        model.ReceiveId = receiveId;
                        model.SenderId = sendId;
                        model.SendeCompanyId = sendCompanyId;
                        model.ReceiveCompanyId = receiveCompanyId;
                        model.SenderPhone = "";
                        model.SenderTime = null;
                        model.SmsCon = con;
                        model.State = false;
                        model.SubmissionTime = DateTime.Now;
                        model.Step = step.ToString();
                        model.TemplateId = templete.Id;
                        model.ServerNo = templete.ServerNo;
                        model.Params = UtilitySendMessage.CreateSmsParam(parameters).Data;
                        model.SignName = templete.SignName;


                        db.Msg_SMS.Add(model);
                        var rows = db.SaveChanges();
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        //WriteLog(AdminModule.SMSHistory.GetText(), SystemRight.Add.GetText(), "新增短信:" + model.Id + ":" + model.SmsCon);
                    }
                    else
                    {
                        throw new Exception("未查找到短信模板");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSMS");
            }
            return result;
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
            //生成验证码
            string code = ConstString.CreateRandomNum(ConstString.RANDOMNUMCOUNT);
            parameters.Add("Code", code);

            Result<int> result = new Result<int>();
            try
            {
                //查找模板
                var templete = DataOperateMsg<Msg_SMSTemplete>.Get().Single(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                if (templete != null)
                {
                    Msg_SMS model = new Msg_SMS();
                    string con = templete.TemplateCon;
                    if (parameters != null && parameters.Any())
                    {
                        //替换内容参数
                        foreach (var item in parameters)
                        {
                            con = con.Replace("{" + item.Key + "}", item.Value);

                        }
                    }
                    var receive = DataOperateBasic<Base_User>.Get().Single(i => i.Phone == phone);
                    long receiveId = 0;
                    if (receive != null && receive.Id > 0)
                    {
                        receiveId = receive.Id;
                    }
                    model.ReceivePhone = phone;
                    model.ReceiveId = receiveId;
                    model.SendeCompanyId = 0;
                    model.ReceiveCompanyId = 0;
                    model.SenderId = 0;
                    model.SenderPhone = "";
                    model.SenderTime = null;
                    model.SmsCon = con;
                    model.State = false;
                    model.SubmissionTime = DateTime.Now;
                    model.Step = step.ToString();
                    model.TemplateId = templete.Id;
                    model.ServerNo = templete.ServerNo;
                    model.Params = UtilitySendMessage.CreateSmsParam(parameters).Data;
                    model.SignName = templete.SignName;
                    var splitTime = (LoadSettingsByKeys(Settings.SendRegisterCodeTime));
                    //验证是否频繁发送
                    var last = DataOperateMsg<Msg_SMSValidate>.Get().GetList(i => i.ReceivePhone == phone).OrderByDescending(i => i.Id).FirstOrDefault();
                    if (last != null && (DateTime.Now - last.RecordTime).TotalSeconds < splitTime.Value.ToInt32Req())
                    {
                        throw new Exception("不能频繁发送验证码");
                    }
                    var rows = DataOperateMsg<Msg_SMS>.Get().Add(model);

                    //添加验证码数据
                    Msg_SMSValidate valModel = new Msg_SMSValidate();
                    valModel.Code = code;
                    valModel.SMSId = model.Id;
                    valModel.ReceivePhone = model.ReceivePhone;
                    valModel.ReceiveId = model.ReceiveId;
                    valModel.SendId = model.SenderId;
                    valModel.SendTime = model.SubmissionTime;
                    valModel.State = ValCodeState.UNUse.ToString();
                    valModel.ValidateType = step.ToString();
                    var seconds = LoadSettingsByKeys(Settings.SMSCodeDuration).Value.ToInt32Req();
                    valModel.ExpiredTime = DateTime.Now.AddSeconds(seconds);
                    rows = DataOperateMsg<Msg_SMSValidate>.Get().Add(valModel);


                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(AdminModule.SMSHistory.GetText(), SystemRight.Add.GetText(), "新增短信验证码:" + model.Id + ":" + model.SmsCon);
                }
                else
                {
                    throw new Exception("未查找到对应短信模板");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSMSValCode");
            }
            return result;
        }


        /// <summary>
        /// 验证码校验，接收人和电话二者需要传入一个
        /// </summary>
        /// <param name="code"></param>
        /// <param name="receiveId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Result<Msg_SMSValidate> ValidateSMSCode(string code, MessageStep step, string phone, long receiveId = -1)
        {
            Result<Msg_SMSValidate> result = new Result<Msg_SMSValidate>();
            try
            {
                string strStep = step.ToString();
                var model = DataOperateMsg<Msg_SMSValidate>.Get().Single(i => (i.ReceiveId == receiveId || i.ReceivePhone == phone) && i.Code == code && i.ValidateType == strStep);
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
                    DataOperateMsg<Msg_SMSValidate>.Get().Update(model);
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ValidateSMSCode");
            }
            return result;
        }
        ///<summary>
        ///删除:短信验证码
        ///</summary>
        public Result<int> DeleteSMSValidateByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_SMSValidate>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_SMSValidate>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(MessageType.SMS.GetText(), SystemRight.Add.GetText(), "批量删除短信验证码: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSMSValidateByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:短信验证码
        ///</summary>
        public Result<List<Msg_SMSValidate>> GetSMSValidateList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_SMSValidate>> result = new Result<List<Msg_SMSValidate>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_SMSValidate>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSValidateList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:短信验证码
        ///</summary>
        public Result<Msg_SMSValidate> GetSMSValidateModel(long id)
        {
            Result<Msg_SMSValidate> result = new Result<Msg_SMSValidate>();
            try
            {
                var model = DataOperateMsg<Msg_SMSValidate>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSValidateModel");
            }
            return result;
        }

        ///<summary>
        ///修改:短信记录
        ///</summary>
        public Result<int> UpdateSMS(Msg_SMS model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMS>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(MessageType.SMS.GetText(), SystemRight.Add.GetText(), "修改短信记录: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSMS");
            }
            return result;
        }
        ///<summary>
        ///删除:短信记录
        ///</summary>
        public Result<int> DeleteSMSByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_SMS>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_SMS>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(MessageType.SMS.GetText(), SystemRight.Add.GetText(), "批量删除短信记录: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSMSByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:短信记录
        ///</summary>
        public Result<List<Msg_SMS>> GetSMSList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_SMS>> result = new Result<List<Msg_SMS>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_SMS>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:短信记录
        ///</summary>
        public Result<Msg_SMS> GetSMSModel(long id)
        {
            Result<Msg_SMS> result = new Result<Msg_SMS>();
            try
            {
                var model = DataOperateMsg<Msg_SMS>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSModel");
            }
            return result;
        }
        ///<summary>
        ///添加:历史短信
        ///</summary>
        public Result<int> AddSMSHistory(Msg_SMSHistory model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMSHistory>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSHistory.GetText(), SystemRight.Add.GetText(), "新增历史短信: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSMSHistory");
            }
            return result;
        }


        ///<summary>
        ///获取列表:历史短信
        ///</summary>
        public Result<List<Msg_SMSHistory>> GetSMSHistoryList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_SMSHistory>> result = new Result<List<Msg_SMSHistory>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_SMSHistory>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSHistoryList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:历史短信
        ///</summary>
        public Result<Msg_SMSHistory> GetSMSHistoryModel(long id)
        {
            Result<Msg_SMSHistory> result = new Result<Msg_SMSHistory>();
            try
            {
                var model = DataOperateMsg<Msg_SMSHistory>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSHistoryModel");
            }
            return result;
        }
        ///<summary>
        ///添加:短信接口设置
        ///</summary>
        public Result<int> AddSMSSetting(Msg_SMSSetting model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMSSetting>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSPlatform.GetText(), SystemRight.Add.GetText(), "新增短信接口设置: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSMSSetting");
            }
            return result;
        }
        ///<summary>
        ///修改:短信接口设置
        ///</summary>
        public Result<int> UpdateSMSSetting(Msg_SMSSetting model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMSSetting>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSPlatform.GetText(), SystemRight.Add.GetText(), "修改短信接口设置: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSMSSetting");
            }
            return result;
        }
        /// <summary>
        /// 审核短信设置
        /// </summary>
        /// <param name="smsSettingId">短信设置Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSMSSetting(long smsSettingId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_SMSSetting>.Get().GetModel(smsSettingId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_SMSSetting>.Get().Update(model);
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
                result.Exception = new ExceptionEx(ex, "AuditSMSSetting");
            }
            return result;
        }

        ///<summary>
        ///删除:短信接口设置
        ///</summary>
        public Result<int> DeleteSMSSettingByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_SMSSetting>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_SMSSetting>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSPlatform.GetText(), SystemRight.Add.GetText(), "批量删除短信接口设置: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSMSSettingByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:短信接口设置
        ///</summary>
        public Result<List<Msg_SMSSetting>> GetSMSSettingList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_SMSSetting>> result = new Result<List<Msg_SMSSetting>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_SMSSetting>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSSettingList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:短信接口设置
        ///</summary>
        public Result<Msg_SMSSetting> GetSMSSettingModel(long id)
        {
            Result<Msg_SMSSetting> result = new Result<Msg_SMSSetting>();
            try
            {
                var model = DataOperateMsg<Msg_SMSSetting>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSSettingModel");
            }
            return result;
        }
        ///<summary>
        ///添加:短信模板
        ///</summary>
        public Result<int> AddSMSTemplete(Msg_SMSTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMSTemplete>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSTemplete.GetText(), SystemRight.Add.GetText(), "新增短信模板: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSMSTemplete");
            }
            return result;
        }
        ///<summary>
        ///修改:短信模板
        ///</summary>
        public Result<int> UpdateSMSTemplete(Msg_SMSTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_SMSTemplete>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSTemplete.GetText(), SystemRight.Add.GetText(), "修改短信模板: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSMSTemplete");
            }
            return result;
        }

        /// <summary>
        /// 审核短信模板
        /// </summary>
        /// <param name="smsTempleteId">短信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSMSTemplete(long smsTempleteId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_SMSTemplete>.Get().GetModel(smsTempleteId);
                if (type == 2)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 3)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_SMSTemplete>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

               if (type == 2)
                {
                    WriteStateLog(model, (!model.IsEnable).ToString(), (model.IsEnable).ToString());
                }
                else if (type == 3)
                {
                    WriteStateLog(model, (!model.IsConfirm).ToString(), (model.IsConfirm).ToString());
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditSMSTemplete");
            }
            return result;
        }

        ///<summary>
        ///删除:短信模板
        ///</summary>
        public Result<int> DeleteSMSTempleteByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_SMSTemplete>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_SMSTemplete>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSTemplete.GetText(), SystemRight.Add.GetText(), "批量删除短信模板: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSMSTempleteByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:短信模板
        ///</summary>
        public Result<List<Msg_SMSTemplete>> GetSMSTempleteList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_SMSTemplete>> result = new Result<List<Msg_SMSTemplete>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_SMSTemplete>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSTempleteList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:短信模板
        ///</summary>
        public Result<Msg_SMSTemplete> GetSMSTempleteModel(long id)
        {
            Result<Msg_SMSTemplete> result = new Result<Msg_SMSTemplete>();
            try
            {
                var model = DataOperateMsg<Msg_SMSTemplete>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSMSTempleteModel");
            }
            return result;
        }

        ///<summary>
        ///删除:历史短信
        ///</summary>
        public Result<int> DeleteSMSHistoryByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_SMSHistory>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateMsg<Msg_SMSHistory>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SMSHistory.GetText(), SystemRight.Add.GetText(), "批量删除历史短信: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSMSHistoryByIds");
            }
            return result;
        }
    }
}
