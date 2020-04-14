using hc.epm.Common;
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
        private MsgDataContext context = new MsgDataContext();
        /// <summary>
        /// 消息发送通用方法
        /// </summary>
        /// <param name="sendId"></param>
        /// <param name="receiveId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <param name="linkURL"></param>
        /// <returns></returns>
        public async void SendMsg(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, Dictionary<string, string> parameters, string linkURL = "")
        {
            await Task.Run(() =>
            {
            Result<int> result = new Result<int>();
            int rows = 0;
                //try
                //{
                using (MsgDataContext db = new MsgDataContext())
                {
                    //parameters.Add("Phone", LoadSettingsByKeys(Settings.WebPhone).Value);
                    //parameters.Add("WebURL", LoadSettingsByKeys(Settings.WebUrl).Value);
                    //查询消息环节配置
                    var section = db.Msg_MessageSection.FirstOrDefault(i => i.IsConfirm && i.IsEnable && i.Name == step.ToString());
                    if (section != null)
                    {
                        var types = section.MsgTypes.SplitString(",");
                        foreach (var item in types)
                        {
                            var type = item.ToEnumReq<MessageType>();
                            switch (type)
                            {
                                case MessageType.Message:
                                    rows += AddMessage(sendId, sendCompanyId, receiveId, receiveCompanyId, step, string.Empty, parameters, linkURL).Data;
                                    break;
                                //case MessageType.Email:
                                //    rows += AddEmail(sendId, sendCompanyId, receiveId, receiveCompanyId, step, parameters).Data;
                                //    break;
                                //case MessageType.SMS:
                                //    rows += AddSMS(sendId, sendCompanyId, receiveId, receiveCompanyId, step, parameters).Data;
                                //    break;
                                default:
                                    break;
                            }
                        }
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                }
                //        else
                //        {
                //            throw new Exception("未查询到消息环节配置");
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    result.Data = -1;
                //    result.Flag = EResultFlag.Failure;
                //    result.Exception = new ExceptionEx(ex, "SendMsg");
                //}

            });


        }
        /// <summary>
        /// 新增站内信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMessage(long sendId, long sendCompanyId, long receiveId, long receiveCompanyId, MessageStep step, string businessUrl, Dictionary<string, string> parameters, string linkURL = "")
        {
            Result<int> result = new Result<int>();
            try
            {
                using (MsgDataContext db = new MsgDataContext())
                {
                    //查找模板
                    var templete = db.Msg_MessageTemplete.FirstOrDefault(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                    if (templete != null)
                    {
                        Msg_Message model = new Msg_Message();
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
                        model.Contents = con;
                        model.LinkURL = string.IsNullOrEmpty(linkURL) ? templete.LinkURL : linkURL;
                        model.ReadTime = null;
                        model.ReceiveId = receiveId;
                        model.SenderId = sendId;
                        model.SendeCompanyId = sendCompanyId;
                        model.ReceiveCompanyId = receiveCompanyId;
                        model.State = false;
                        model.Step = step.ToString();
                        model.TemplateId = templete.Id;
                        model.Title = title;
                        model.SendTime = DateTime.Now;
                        model.BusinessUrl = businessUrl;
                        db.Msg_Message.Add(model);
                        var rows = db.SaveChanges();
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        //WriteLog(AdminModule.MessageHistory.GetText(), SystemRight.Add.GetText(), "新增站内信:" + model.Id + ":" + model.Title);
                    }
                    else
                    {
                        throw new Exception("未查找到对应站内信模板");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMessage");
            }
            return result;
        }
        /// <summary>
        /// 新增站内信
        /// </summary>
        /// <param name="sendId"></param>
        /// <param name="sendCompanyId"></param>
        /// <param name="recDic">接收信息字典类型 key：接收人Id；value：接收人单位Id</param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <param name="linkURL"></param>
        /// <returns></returns>
        public Result<int> AddMessageBatch(long sendId, long sendCompanyId, Dictionary<long, long> recDic, MessageStep step, Dictionary<string, string> parameters, string linkURL = "")
        {
            Result<int> result = new Result<int>();
            try
            {
                using (MsgDataContext db = new MsgDataContext())
                {
                    //查找模板
                    var templete = db.Msg_MessageTemplete.FirstOrDefault(i => i.Step == step.ToString() && i.IsConfirm && i.IsEnable);
                    if (templete != null)
                    {
                        if (recDic != null && recDic.Any())
                        {
                            List<Msg_Message> list = new List<Msg_Message>();
                            Msg_Message model = null;
                            foreach (var dic in recDic)
                            {
                                model = new Msg_Message();

                                string con = templete.TemplateCon;
                                string title = templete.TitleCon;
                                if (parameters != null && parameters.Any())
                                {
                                    foreach (var item in parameters)
                                    {
                                        con = con.Replace("{" + item.Key + "}", item.Value);
                                        title = title.Replace("{" + item.Key + "}", item.Value);
                                    }
                                }
                                model.Contents = con;
                                model.LinkURL = string.IsNullOrEmpty(linkURL) ? templete.LinkURL : linkURL;
                                model.ReadTime = null;
                                model.ReceiveId = dic.Key;
                                model.SenderId = sendId;
                                model.SendeCompanyId = sendCompanyId;
                                model.ReceiveCompanyId = dic.Value;
                                model.State = false;
                                model.Step = step.ToString();
                                model.TemplateId = templete.Id;
                                model.Title = title;
                                model.SendTime = DateTime.Now;
                                list.Add(model);
                            }
                            db.Msg_Message.AddRange(list);
                            var rows = db.SaveChanges();
                            result.Data = rows;
                            result.Flag = EResultFlag.Success;
                        }
                        else
                        {
                            throw new Exception("未找到接收人");
                        }
                    }
                    else
                    {
                        throw new Exception("未查找到对应站内信模板");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMessage");
            }
            return result;
        }
        /// <summary>
        /// 添加站内信模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMessageTemplete(Msg_MessageTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageTemplete>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageTemplete.GetText(), SystemRight.Add.GetText(), "新增站内信模板:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMessageTemplete");
            }
            return result;
        }
        /// <summary>
        /// 审核站内信模板
        /// </summary>
        /// <param name="messageTempleteId">站内信模板Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditMessageTemplete(long messageTempleteId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_MessageTemplete>.Get().GetModel(messageTempleteId);
                if (type == 2)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 3)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_MessageTemplete>.Get().Update(model);
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
                result.Exception = new ExceptionEx(ex, "AuditMessageTemplete");
            }
            return result;
        }

        /// <summary>
        /// 添加消息环节
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSection(Msg_MessageSection model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageSection>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageSection.GetText(), SystemRight.Add.GetText(), "新增消息环节:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSection");
            }
            return result;
        }
        /// <summary>
        /// 审核消息环节配置
        /// </summary>
        /// <param name="sectionId">消息环节Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditSection(long sectionId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_MessageSection>.Get().GetModel(sectionId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_MessageSection>.Get().Update(model);
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
                result.Exception = new ExceptionEx(ex, "AuditSection");
            }
            return result;
        }

        /// <summary>
        /// 新增消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddStrategy(Msg_MessageStrategy model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageStrategy>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageStrategy.GetText(), SystemRight.Add.GetText(), "新增消息发送策略:" + model.Id + ":" + model.Type);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddStrategy");
            }
            return result;
        }
        /// <summary>
        /// 审核消息发送策略
        /// </summary>
        /// <param name="strategyId">消息策略Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditStrategy(long strategyId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateMsg<Msg_MessageStrategy>.Get().GetModel(strategyId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateMsg<Msg_MessageStrategy>.Get().Update(model);
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
                result.Exception = new ExceptionEx(ex, "AuditStrategy");
            }
            return result;
        }
        /// <summary>
        /// 删除站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_Message>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateMsg<Msg_Message>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.MessageList.GetText(), SystemRight.Delete.GetText(), "批量删除站内信:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMessageByIds");
            }
            return result;
        }
        /// <summary>
        /// 删除历史站内信
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageHistoryByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_MessageHistory>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateMsg<Msg_MessageHistory>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.MessageList.GetText(), SystemRight.Delete.GetText(), "批量删除站内信:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMessageHistoryByIds");
            }
            return result;
        }
        /// <summary>
        /// 删除站内信模板
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMessageTempleteByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_MessageTemplete>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateMsg<Msg_MessageTemplete>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.MessageTemplete.GetText(), SystemRight.Delete.GetText(), "批量删除站内信模板:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMessageTempleteByIds");
            }
            return result;
        }
        /// <summary>
        /// 删除消息环节配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteSectionByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_MessageSection>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateMsg<Msg_MessageSection>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.MessageSection.GetText(), SystemRight.Delete.GetText(), "批量删除消息环节配置:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSectionByIds");
            }
            return result;
        }
        /// <summary>
        /// 批量删除消息发送策略
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteStrategyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateMsg<Msg_MessageStrategy>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateMsg<Msg_MessageStrategy>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.MessageStrategy.GetText(), SystemRight.Delete.GetText(), "批量删除消息发送策略:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteStrategyByIds");
            }
            return result;
        }
        /// <summary>
        /// 获取历史消息列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageHistory>> GetMessageHistoryList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_MessageHistory>> result = new Result<List<Msg_MessageHistory>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_MessageHistory>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageHistoryList");
            }
            return result;
        }
        /// <summary>
        /// 获取历史消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageHistory> GetMessageHistoryModel(long id)
        {
            Result<Msg_MessageHistory> result = new Result<Msg_MessageHistory>();
            try
            {
                var model = DataOperateMsg<Msg_MessageHistory>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageHistoryModel");
            }
            return result;
        }

        /// <summary>
        /// 获取站内信列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_Message>> GetMessageList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_Message>> result = new Result<List<Msg_Message>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_Message>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProtocolList");
            }
            return result;
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
            Result<List<Msg_Message>> result = new Result<List<Msg_Message>>();
            try
            {
                var list = DataOperateMsg<Msg_Message>.Get().GetList(i => i.ReceiveId == userId && i.ReceiveCompanyId == companyId && i.State == false).Take(top).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageNoReadList");
            }
            return result;
        }
        /// <summary>
        /// 获取站内信详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_Message> GetMessageModel(long id)
        {
            Result<Msg_Message> result = new Result<Msg_Message>();
            try
            {
                var model = DataOperateMsg<Msg_Message>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageModel");
            }
            return result;
        }
        /// <summary>
        /// 读取站内信
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_Message> ReadMessage(long id)
        {
            Result<Msg_Message> result = new Result<Msg_Message>();
            try
            {
                var model = DataOperateMsg<Msg_Message>.Get().GetModel(id);
                model.State = true;
                model.ReadTime = DateTime.Now;
                DataOperateMsg<Msg_Message>.Get().Update(model);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ReadMessage");
            }
            return result;
        }

        /// <summary>
        /// 获取站内信模板列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageTemplete>> GetMessageTempleteList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_MessageTemplete>> result = new Result<List<Msg_MessageTemplete>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_MessageTemplete>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageTempleteList");
            }
            return result;
        }
        /// <summary>
        /// 获取站内信模板详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageTemplete> GetMessageTempleteModel(long id)
        {
            Result<Msg_MessageTemplete> result = new Result<Msg_MessageTemplete>();
            try
            {
                var model = DataOperateMsg<Msg_MessageTemplete>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMessageTempleteModel");
            }
            return result;
        }
        /// <summary>
        /// 获取消息配置列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageSection>> GetSectionList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_MessageSection>> result = new Result<List<Msg_MessageSection>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_MessageSection>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSectionList");
            }
            return result;
        }
        /// <summary>
        /// 获取消息环节配置详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageSection> GetSectionModel(long id)
        {
            Result<Msg_MessageSection> result = new Result<Msg_MessageSection>();
            try
            {
                var model = DataOperateMsg<Msg_MessageSection>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSectionModel");
            }
            return result;
        }
        /// <summary>
        /// 获取消息发送策略列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_MessageStrategy>> GetStrategyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Msg_MessageStrategy>> result = new Result<List<Msg_MessageStrategy>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Msg_MessageStrategy>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetStrategyList");
            }
            return result;
        }
        /// <summary>
        /// 获取消息发送策略详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Msg_MessageStrategy> GetStrategyModel(long id)
        {
            Result<Msg_MessageStrategy> result = new Result<Msg_MessageStrategy>();
            try
            {
                var model = DataOperateMsg<Msg_MessageStrategy>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetStrategyModel");
            }
            return result;
        }

        /// <summary>
        /// 修改站内消息模板
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateMessageTemplete(Msg_MessageTemplete model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageTemplete>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageTemplete.GetText(), SystemRight.Add.GetText(), "修改站内消息模板:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMessageTemplete");
            }
            return result;
        }
        /// <summary>
        /// 修改消息环节配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateSection(Msg_MessageSection model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageSection>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageSection.GetText(), SystemRight.Add.GetText(), "修改消息环节配置:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSection");
            }
            return result;
        }
        /// <summary>
        /// 修改消息发送策略
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateStrategy(Msg_MessageStrategy model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateMsg<Msg_MessageStrategy>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MessageStrategy.GetText(), SystemRight.Add.GetText(), "修改消息发送策略:" + model.Id);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateStrategy");
            }
            return result;
        }


    }
}
