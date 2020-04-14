using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.Plat.Common.Service;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddQuestion(QuestionView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                List<int> statelist = new List<int>() { (int)ApprovalState.WaitAppr, (int)ApprovalState.ApprSuccess };
                try
                {
                    var completion = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetList(t => t.ProjectId == model.ProjectId && statelist.Contains(t.State.Value)).FirstOrDefault();

                    if (completion != null)
                    {
                        throw new Exception("沟通项目已发起完工验收，不可操作！");
                    }
                    if (model == null)
                    {
                        throw new Exception("请填写问题相关内容！");
                    }
                    if (model.ProjectId <= 0)
                    {
                        throw new Exception("请选择问题项目名称！");
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "字符串截取位置1: " + model.ProjectId);
                }
                model.BusinessTypeNo = GetString(model.BusinessTypeNo);

                var BusinessTypeName = ((BusinessType)Enum.Parse(typeof(BusinessType), model.BusinessTypeNo)).GetText();

                Epm_Question question = new Epm_Question()
                {
                    ProjectId = model.ProjectId,
                    ProjectName = model.ProjectName,
                    Title = model.Title,
                    Description = model.Description,
                    Proposal = model.Proposal,
                    IsAccident = model.IsAccident,
                    BusinessTypeNo = model.BusinessTypeNo,
                    BusinessTypeName = BusinessTypeName,
                    BusinessId = model.BusinessId,
                    SubmitUserId = CurrentUserID.ToLongReq(),
                    SubmitUserName = CurrentUserName,
                    SubmitCompanyId = CurrentCompanyID.ToLongReq(),
                    SubmitCompanyName = CurrentCompanyName,
                    SubmitTime = DateTime.Now,
                    RecCompanyId = model.RecCompanyId,
                    RecCompanyName = model.RecCompanyName,
                    RecUserId = model.RecUserId,
                    RecUserName = model.RecUserName,
                    State = 1,
                    CrtCompanyId = CurrentCompanyID.ToLongReq(),
                    CrtCompanyName = CurrentCompanyName,
                    CreateTime = DateTime.Now,
                    IsDelete = false,
                    ProblemTypeName = model.ProblemTypeName,
                    ProblemTypeNo = model.ProblemTypeNo
                };
                question = SetCurrentUser(question);

                if (question == null)
                {
                    throw new Exception("问题沟通！");
                }
                #region 问题关联模型

                if (model.QuestionBims != null && model.QuestionBims.Any())
                {
                    model.QuestionBims.ForEach(p =>
                    {
                        p.Sort = p.Sort ?? 0;
                        p.State = p.State ?? 0;
                        p.CrtCompanyId = question.CrtCompanyId;
                        p.CrtCompanyName = question.CrtCompanyName;
                        p.CreateTime = DateTime.Now;
                        p.CreateUserId = question.CreateUserId;
                        p.CreateUserName = question.CreateUserName;
                        p.OperateUserId = question.OperateUserId;
                        p.OperateUserName = question.OperateUserName;
                        p.OperateTime = DateTime.Now;
                        p.IsDelete = false;
                    });
                }


                #endregion

                #region 问题协助人员

                if (model.QuestionUsers != null && model.QuestionUsers.Any())
                {
                    model.QuestionUsers.ForEach(p =>
                    {
                        p.State = p.State ?? 0;
                        p.CrtCompanyId = question.CrtCompanyId;
                        p.CrtCompanyName = question.CrtCompanyName;
                        p.CreateTime = DateTime.Now;
                        p.CreateUserId = question.CreateUserId;
                        p.CreateUserName = question.CreateUserName;
                        p.OperateUserId = question.OperateUserId;
                        p.OperateUserName = question.OperateUserName;
                        p.OperateTime = DateTime.Now;
                        p.IsDelete = false;
                    });
                }

                #endregion
                try
                {
                    int rows = DataOperateBusiness<Epm_Question>.Get().Add(question);
                    if (model.QuestionBims != null && model.QuestionBims.Any())
                    {
                        try
                        {
                            model.QuestionBims.ForEach(p => { p.QuestionId = question.Id; p.BIMId = p.ComponentId.Split('_')[0].ToLongReq(); });

                            DataOperateBusiness<Epm_QuestionBIM>.Get().AddRange(model.QuestionBims);
                        }
                        catch (Exception ex)
                        {
                            WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "字符串截取位置2: " + model.QuestionBims[0].ComponentId.Split('_')[0].ToLongReq());
                        }

                    }
                    if (model.QuestionUsers != null && model.QuestionUsers.Any())
                    {
                        model.QuestionUsers.ForEach(p => p.QuestionId = question.Id);
                        DataOperateBusiness<Epm_QuestionUser>.Get().AddRange(model.QuestionUsers);
                    }

                    //transaction.Commit();

                    if (model.Attachs.Any())
                    {
                        AddFilesByTable(question, model.Attachs);
                    }

                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);


                    // TODO：需要处理替换消息模板以及消息类型
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    try
                    {
                        var recCompany = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == model.ProjectId && t.CompanyId == question.RecCompanyId).FirstOrDefault();

                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        if (recCompany == null)
                        {
                            if (project != null)
                            {
                                app.ApproverId = project.ContactUserId;
                                app.ApproverName = project.ContactUserName;
                            }
                        }
                        else {
                            if (recCompany != null && recCompany.LinkManId.HasValue)
                            {
                                app.ApproverId = recCompany.LinkManId;
                                app.ApproverName = recCompany.LinkMan;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "字符串截取位置3: " + model.ProjectId);
                    }

                    app.Title = CurrentUserName + "对" + app.ApproverName + "发起了问题沟通";
                    app.Content = CurrentUserName + "对" + app.ApproverName + "发起了问题沟通";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Question.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Question.GetText();
                    app.BusinessState = (int)(EnumState.Normal);
                    app.BusinessId = question.Id;
                    app.ProjectId = question.ProjectId;
                    app.ProjectName = question.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "提交问题生成待办: " + question.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = CurrentUserName + "对" + app.ApproverName + "发起了问题沟通";
                        modelMsg.Content = CurrentUserName + "对" + app.ApproverName + "发起了问题沟通";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = question.Id;
                        modelMsg.BussinesType = BusinessType.Question.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddQuestion");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateQuestion(Epm_Question model)
        {
            Result<int> result = new Result<int>();
            try
            {
                throw new Exception("该功能不实现！");
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateQuestion");
            }
            return result;
        }

        /// <summary>
        /// 根据问题 ID 删除问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> DeleteQuestion(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (id <= 0)
                {
                    throw new Exception("请选择要删除的问题！");
                }

                var model = DataOperateBusiness<Epm_Question>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("要删除的问题不存在或已被删除！");
                }
                var track = DataOperateBusiness<Epm_QuestionTrack>.Get().Single(p => p.QuestionId == id);
                if (track != null)
                {
                    throw new Exception("当前问题已有人进行回复，不能删除！");
                }

                var bimList = DataOperateBusiness<Epm_QuestionBIM>.Get().GetList(p => p.QuestionId == id);
                var userList = DataOperateBusiness<Epm_QuestionUser>.Get().GetList(p => p.QuestionId == id);

                string tableName = model.GetType().Name;
                var fileList = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableId == id && p.TableName == tableName);

                DbContextTransaction transaction = context.Database.BeginTransaction();
                try
                {
                    DataOperateBusiness<Epm_Question>.Get().Delete(model);
                    if (bimList.Any())
                    {
                        DataOperateBusiness<Epm_QuestionBIM>.Get().DeleteRange(bimList);
                    }
                    if (userList.Any())
                    {
                        DataOperateBusiness<Epm_QuestionUser>.Get().DeleteRange(userList);
                    }
                    transaction.Commit();
                    if (fileList.Any())
                    {
                        DataOperateBasic<Base_Files>.Get().DeleteRange(fileList);
                    }

                    result.Data = true;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Question.GetText(), SystemRight.Delete.GetText(), "删除: " + model.Id);

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = model.CreateUserName + "发起的问题沟通已删除";
                        modelMsg.Content = model.CreateUserName + "发起的问题沟通已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Question.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    transaction.Dispose();
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteQuestion");
            }
            finally
            {

            }
            return result;
        }

        ///<summary>
        ///获取列表: 
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Question>> GetQuestionList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_Question>> result = new Result<List<Epm_Question>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Question>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetQuestionList");
            }
            return result;
        }
        public Result<List<Base_VideoManage>> GetBaseVideoManageLists(QueryCondition qc)
        {
            //qc = AddDefaultWeb(qc);
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_VideoManage>(basicContext, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseVideoManageLists");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<QuestionView> GetQuestionModel(long id)
        {
            Result<QuestionView> result = new Result<QuestionView>();
            try
            {
                var model = DataOperateBusiness<Epm_Question>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("问题不存在或已被删除！");
                }

                var tracks = DataOperateBusiness<Epm_QuestionTrack>.Get().GetList(p => p.QuestionId == id).ToList();
                var bims = DataOperateBusiness<Epm_QuestionBIM>.Get().GetList(p => p.QuestionId == id).ToList();
                var users = DataOperateBusiness<Epm_QuestionUser>.Get().GetList(p => p.QuestionId == id).ToList();
                var attachs = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableId == id).ToList();

                QuestionView view = new QuestionView
                {
                    Id = model.Id,
                    ProjectId = model.ProjectId,
                    ProjectName = model.ProjectName,
                    Title = model.Title,
                    Description = model.Description,
                    Proposal = model.Proposal,
                    IsAccident = model.IsAccident ?? false,
                    BusinessTypeNo = model.BusinessTypeNo,
                    BusinessTypeName = model.BusinessTypeName,
                    BusinessId = model.BusinessId,
                    SubmitUserId = model.SubmitUserId ?? 0,
                    SubmitUserName = model.SubmitUserName,
                    SubmitCompanyId = model.SubmitCompanyId ?? 0,
                    SubmitCompanyName = model.SubmitCompanyName,
                    SubmitTime = model.SubmitTime,
                    RecCompanyId = model.RecCompanyId,
                    RecCompanyName = model.RecCompanyName,
                    RecUserName = model.RecUserName,
                    State = model.State ?? 0,
                    CrtCompanyId = model.CrtCompanyId ?? 0,
                    CrtCompanyName = model.CrtCompanyName,
                    CloseTime = model.CloseTime,
                    CreateTime = model.CreateTime,
                    ProblemTypeName = model.ProblemTypeName,
                    ProblemTypeNo = model.ProblemTypeNo,
                    QuestionTracks = tracks,
                    QuestionBims = bims,
                    QuestionUsers = users,
                    CreateUserId = model.CreateUserId,
                    CreateUserName = model.CreateUserName,
                    Attachs = attachs
                };

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetQuestionModel");
            }
            return result;
        }

        /// <summary>
        /// 关闭问题
        /// </summary>
        /// <param name="id">问题 ID</param>
        /// <param name="isAccident">是否重大事故</param>
        /// <returns></returns>
        public Result<bool> CloseQuestion(long id, bool isAccident = false)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (id <= 0)
                {
                    throw new Exception("请选择要关闭的问题！");
                }
                Epm_Question model = DataOperateBusiness<Epm_Question>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("问题不存在或已被删除！");
                }
                if (model.State == 2)
                {
                    throw new Exception("该问题已关闭，请勿重复操作！");
                }

                model.CloseTime = DateTime.Now;
                model.IsAccident = isAccident;
                model.State = 2;
                model = SetCurrentUser(model);

                DataOperateBusiness<Epm_Question>.Get().Update(model);
                result.Data = true;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "关闭: " + model.Id);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.ApproverId = CurrentUserID.ToLongReq();
                    app.ApproverName = CurrentUserName;
                    app.Title = model.CreateUserName + "发起了问题沟通，已关闭";
                    app.Content = model.CreateUserName + "发起了问题沟通，已关闭";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Question.ToString();
                    app.Action = SystemRight.Close.ToString();
                    app.BusinessTypeName = BusinessType.Question.GetText();
                    app.BusinessState = (int)(EnumState.Close);
                    app.BusinessId = model.Id;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "提交问题生成待办: " + model.Id);
                    #endregion

                }
                WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "生成消息: " + model.Id);

                #region 消息
                var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = model.CreateUserName + "发起的问题沟通，已关闭";
                    modelMsg.Content = model.CreateUserName + "发起的问题沟通，已关闭";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Question.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "QuestionClose");
            }
            return result;
        }

        /// <summary>
        /// 回复问题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ReplyQuestion(Epm_QuestionTrack model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                Epm_Question question = DataOperateBusiness<Epm_Question>.Get().GetModel(model.QuestionId ?? 0);
                if (question == null)
                {
                    throw new Exception("问题不存在或已被删除！");
                }
                if (model.State == 2)
                {
                    throw new Exception("该问题已关闭，无法在进行回复操作！");
                }

                model = SetCurrentUser(model);

                DataOperateBusiness<Epm_QuestionTrack>.Get().Add(model);
                result.Data = true;
                result.Flag = EResultFlag.Success;

                WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "回复问题: " + model.Id);


                #region 处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == question.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.ApproverId = CurrentUserID.ToLongReq();
                    app.ApproverName = CurrentUserName;
                    app.Title = question.CreateUserName + "发起的问题沟通，已有回复";
                    app.Content = question.CreateUserName + "发起的问题沟通，已有回复";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Question.ToString();
                    app.Action = SystemRight.Reply.ToString();
                    app.BusinessTypeName = BusinessType.Question.GetText();
                    app.BusinessState = (int)(EnumState.Normal);
                    app.BusinessId = question.Id;
                    app.ProjectId = question.ProjectId;
                    app.ProjectName = question.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Question.GetText(), SystemRight.Add.GetText(), "提交问题生成待办: " + question.Id);
                    #endregion
                }
                #endregion

                #region 消息
                var waitSend = GetWaitSendMessageList(question.ProjectId.Value);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = question.CreateUserName + "发起了问题沟通，" + CurrentUserName + "有回复";
                    modelMsg.Content = question.CreateUserName + "发起了问题沟通，" + CurrentUserName + "有回复";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Question.ToString();
                    modelMsg.ProjectId = question.ProjectId.Value;
                    modelMsg.ProjectName = question.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "QuestionClose");
            }
            return result;
        }

        /// <summary>
        /// 获取当前登录人问题
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Question>> GetCurrUserQuestion(QueryCondition qc)
        {
            qc = AddDefault(qc);
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "RecUserId",
                ExpValue = CurrentUser.Id,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = 0,
                ExpOperater = eConditionOperator.NotEqual,
                ExpLogical = eLogicalOperator.And
            });

            qc.SortList.Add(new SortExpression()
            {
                SortName = "SubmitTime",
                SortType = eSortType.Desc
            });

            Result<List<Epm_Question>> result = new Result<List<Epm_Question>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Question>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetQuestionList");
            }
            return result;
        }

        /// <summary>
        /// 根据问题id获取关联组件列表
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Result<List<Epm_QuestionBIM>> GetComponentListByQuestionId(long questionId)
        {
            Result<List<Epm_QuestionBIM>> result = new Result<List<Epm_QuestionBIM>>();
            try
            {
                var model = DataOperateBusiness<Epm_QuestionBIM>.Get().GetList(p => p.QuestionId == questionId).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetComponentListByQuestionId");
            }
            return result;
        }

        /// <summary>
        /// 根据模型ID获取关联组件列表
        /// </summary>
        /// <param name="bimId"></param>
        /// <returns></returns>
        public Result<List<Epm_QuestionBIM>> GetComponentListByBimId(long bimId)
        {
            Result<List<Epm_QuestionBIM>> result = new Result<List<Epm_QuestionBIM>>();
            try
            {
                var model = DataOperateBusiness<Epm_QuestionBIM>.Get().GetList(p => p.BIMId == bimId).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetComponentListByBimId");
            }
            return result;
        }

        /// <summary>
        /// 添加问题关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="componentIds"></param>
        /// <returns></returns>
        public Result<int> AddQuestionBIM(Epm_QuestionBIM model, string componentIds)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (!string.IsNullOrEmpty(componentIds))
                {
                    //首先删除同一模型同一检查详情下的所有模型组件
                    var componentList = DataOperateBusiness<Epm_QuestionBIM>.Get().GetList(i => i.QuestionId == model.QuestionId && i.BIMId == model.BIMId).ToList();
                    if (componentList.Count > 0)
                    {
                        foreach (var item in componentList)
                        {
                            item.OperateUserId = CurrentUserID.ToLongReq();
                            item.OperateUserName = CurrentUserName;
                            item.OperateTime = DateTime.Now;
                            item.DeleteTime = DateTime.Now;
                        }
                        DataOperateBusiness<Epm_QuestionBIM>.Get().DeleteRange(componentList);
                    }

                    //获取组件ID集合
                    var componentIdList = componentIds.SplitString(",");
                    List<Epm_QuestionBIM> dataList = new List<Epm_QuestionBIM>();
                    foreach (var item in componentIdList)
                    {
                        Epm_QuestionBIM temp = new Epm_QuestionBIM();
                        temp = base.SetCurrentUser(temp);
                        temp.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        temp.CrtCompanyName = CurrentCompanyName;

                        temp.QuestionId = model.QuestionId;
                        temp.BIMId = model.BIMId;
                        temp.BIMName = model.BIMName;
                        temp.ComponentId = item;
                        dataList.Add(temp);
                    }
                    //批量添加问题管理关联模型
                    var rows = DataOperateBusiness<Epm_QuestionBIM>.Get().AddRange(dataList);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Plan.GetText(), SystemRight.Add.GetText(), "新增:添加问题关联模型 " + model.Id);
                }
                else
                {
                    throw new Exception("没有选择关联组件，请选择要关联的组件！");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddQuestionBIM");
            }
            return result;
        }


        /// <summary>
        /// 获取问题回复列表
        /// </summary>
        /// <param name="qc">查询条件</param>
        /// <returns></returns>
        public Result<List<Epm_QuestionTrack>> GetQuestionTrack(QueryCondition qc)
        {
            Result<List<Epm_QuestionTrack>> result = new Result<List<Epm_QuestionTrack>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_QuestionTrack>(context, qc);
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "");
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.AllRowsCount = 0;
            }
            return result;
        }

        public string GetString(string type)
        {
            switch (type.ToLower())
            {
                case "approver":
                    return "Approver";
                case "plan":
                    return "Plan";
                case "schedule":
                    return "Schedule";
                case "securitytrain":
                    return "SecurityTrain";
                case "dangerous":
                    return "Dangerous";
                case "securitycheck":
                    return "SecurityCheck";
                case "qualitytrain":
                    return "QualityTrain";
                case "qualitycheck":
                    return "QualityCheck";
                case "log":
                    return "Log";
                case "change":
                    return "Change";
                case "visa":
                    return "Visa";
                case "draw":
                    return "Draw";
                case "model":
                    return "Model";
                case "equipment":
                    return "Equipment";
                case "track":
                    return "Track";
                case "special":
                    return "Special";
                case "completed":
                    return "Completed";
                case "delayapply":
                    return "DelayApply";
                case "project":
                    return "Project";
                case "contract":
                    return "Contract";
                case "question":
                    return "Question";
                case "rectification":
                    return "Rectification";
                default:
                    break;
            }
            return "";
        }
    }
}
