using System;
using System.Collections.Generic;
using System.Linq;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using hc.epm.DataModel.Business;
using hc.epm.Service.Msg;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel.AppView;
using hc.Plat.Common.Extend;
using hc.epm.Common;
using hc.epm.DataModel.BaseCore;
using hc.epm.ViewModel;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        #region  APP 专用接口

        /// <summary>
        /// 获取沟通列表集合
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<QuestionModel>> GetQuestions(QueryCondition qc)
        {
            Result<List<QuestionModel>> result = new Result<List<QuestionModel>>();
            try
            {
                if (qc == null)
                {
                    qc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = true
                        }
                    };
                }
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "IsDelete",
                    ExpValue = false,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                qc.SortList.Add(new SortExpression()
                {
                    SortName = "OperateTime",
                    SortType = eSortType.Desc
                });

                var questionResult = DataOperate.QueryListSimple<Epm_Approver>(context, qc);
                if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Any())
                {
                    List<QuestionModel> list = questionResult.Data.Select(p => new QuestionModel()
                    {
                        id = p.Id,
                        name = p.Title ?? "",
                        submitTime = p.SendTime,
                        type = p.BusinessTypeNo ?? "",
                        state = p.State ?? 0,
                        createUserId = p.SendUserId ?? 0,
                        submitUserName = p.SendUserName ?? "",
                        workContent = p.Content ?? "",
                        businessId = p.BusinessId ?? 0,
                        businessState = p.BusinessState ?? 0,
                        projectId = p.ProjectId ?? 0,
                        projectName = p.ProjectName ?? "",
                        businessChild = p.BussinesChild,
                        action = p.Action
                    }).ToList();

                    var businessIds = list.Select(p => p.businessId).Distinct().ToList();
                    if (businessIds.Any())
                    {
                        // 业务问题数
                        var questions = DataOperateBusiness<Epm_Question>.Get().GetList(p => businessIds.Contains(p.BusinessId.Value));
                        var questionCount = questions.GroupBy(p => p.BusinessId).Select(p => new
                        {
                            key = p.Key,
                            count = p.Count(x => x.BusinessId == p.Key)
                        }).ToDictionary(p => p.key, p => p.count);

                        // 问题回复数
                        var questionTracks = DataOperateBusiness<Epm_QuestionTrack>.Get().GetList(p => businessIds.Contains(p.QuestionId.Value));
                        var questionTrackCount = questionTracks.GroupBy(p => p.QuestionId).Select(p => new
                        {
                            key = p.Key,
                            count = p.Count(x => x.QuestionId == p.Key)
                        }).ToDictionary(p => p.key, p => p.count);

                        list.ForEach(p =>
                        {
                            p.answerCount = questionCount.ContainsKey(p.businessId) ? questionCount[p.businessId] : (questionTrackCount.ContainsKey(p.businessId) ? questionTrackCount[p.businessId] : 0);
                        });
                    }

                    result.Data = list;
                    result.AllRowsCount = questionResult.AllRowsCount;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    string msg = "暂无相关数据！";
                    if (result.Exception != null)
                    {
                        msg = result.Exception.Decription;
                    }
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.AllRowsCount = 0;
                result.Flag = EResultFlag.Success;
                result.Exception = new ExceptionEx(ex, "GetQuestions");
            }
            return result;
        }

        /// <summary>
        /// 获取登录用户待办事项总数和未读消息总数
        /// </summary>
        /// <returns></returns>
        public Result<Dictionary<string, int>> GetApproverCount(long UserId)
        {
            Result<Dictionary<string, int>> result = new Result<Dictionary<string, int>>();
            int approvalCount = 0;
            int unreadMsg = 0;
            Dictionary<string, int> dic = new Dictionary<string, int>();
            try
            {
                QueryCondition approverQc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };
                AddDefault(approverQc);
                approverQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ApproverId",
                    ExpValue = UserId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                approverQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "IsApprover",
                    ExpValue = false,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                var approverResult = DataOperate.QueryListSimple<Epm_Approver>(context, approverQc);
                approvalCount = approverResult.AllRowsCount;

                QueryCondition msgQc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };
                AddDefault(msgQc);
                msgQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SendId",
                    ExpValue = UserId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                msgQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "IsRead",
                    ExpValue = false,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                var msgResult = DataOperate.QueryListSimple<Epm_Approver>(context, msgQc);
                unreadMsg = msgResult.Data.Count;
            }
            finally
            {
                dic.Add("approvalCount", approvalCount);
                dic.Add("unreadMsg", unreadMsg);

                result.Data = dic;
                result.Flag = EResultFlag.Success;
            }
            return result;
        }

        /// <summary>
        /// 获取可展示广告列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdPutRecord>> GetAdShowList(QueryCondition qc)
        {
            Result<List<Epm_AdPutRecord>> result = new Result<List<Epm_AdPutRecord>>();
            try
            {
                AddDefault(qc);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = 1,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "StartTime",
                    ExpValue = DateTime.Now,
                    ExpOperater = eConditionOperator.LessThanOrEqual,
                    ExpLogical = eLogicalOperator.And
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "EndTime",
                    ExpValue = DateTime.Now,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual,
                    ExpLogical = eLogicalOperator.And
                });

                result = DataOperate.QueryListSimple<Epm_AdPutRecord>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdList");
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// 获取指定数据的附件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetBaseFiles(List<long> tableId)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var data = DataOperateBasic<Base_Files>.Get().GetList(p => tableId.Contains(p.TableId)).ToList();

                result.Data = data;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseFiles");
            }
            return result;
        }

        public Result<Base_Files> GetBaseFile(long id)
        {
            Result<Base_Files> result = new Result<Base_Files>();
            try
            {
                var data = DataOperateBasic<Base_Files>.Get().GetModel(id);

                result.Data = data;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseFile");
            }
            return result;
        }

        /// <summary>
        /// 根据guid查询图片附件
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetBaseFileByGuid(string guid)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var data = DataOperateBasic<Base_Files>.Get().GetList(p => p.GuidId == guid).ToList();

                result.Data = data;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseFiles");
            }
            return result;
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> AddFilesByTables(string model, List<Base_Files> fileList, bool isdelete)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            if (fileList != null)
            {
                long id = 0;
                //新增附件
                List<Base_Files> fileModels = new List<Base_Files>();
                foreach (var item in fileList)
                {
                    SetCurrentUser(item);
                    item.TableId = id;
                    item.TableName = model;
                    fileModels.Add(item);
                }
                int rows = 0;
                rows = DataOperateBasic<Base_Files>.Get().AddRange(fileModels);

                if (rows > 0)
                {
                    result.Data = fileModels;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
            }
            return result;
        }


        /// <summary>
        /// 获取用户头像链接
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Dictionary<long, string>> GetUserProfilePhoto(List<long> userId)
        {
            Result<Dictionary<long, string>> result = new Result<Dictionary<long, string>>();
            try
            {
                var list = from a in basicContext.Base_User.Where(p => userId.Contains(p.Id))
                           join b in basicContext.Base_Files.Where(p => p.IsDelete == false && p.TableColumn == "GYSZP" && p.ImageType == "small") on a.Id equals b.TableId into bref
                           from b in bref.DefaultIfEmpty()
                           select new
                           {
                               a.Id,
                               b.Url
                           };
                result.Data = list.Distinct().ToDictionary(p => p.Id, p => p.Url);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.Exception = new ExceptionEx(ex, "GetUserProfilePhoto");
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// 获取热门问题
        /// </summary>
        /// <returns></returns>
        public Result<QuestionModel> GetHotQuestion()
        {
            Result<QuestionModel> result = new Result<QuestionModel>();
            try
            {
                // 去除未删除且未关闭的问题，在这些问题中找热门问题
                var questionIds = context.Epm_Question.Where(p => p.IsDelete == false && p.State == 1).Select(p => p.Id).ToList();

                long questionId = 0;
                int answerCount = 0;
                if (questionIds.Any())
                {
                    var tempQuestion = context.Epm_QuestionTrack
                        .Where(p => p.IsDelete == false && questionIds.Contains(p.QuestionId.Value))
                        .GroupBy(p => p.QuestionId).Select(
                            p => new
                            {
                                questionId = p.Key,
                                questionCount = p.Count()
                            }).OrderByDescending(p => p.questionCount).FirstOrDefault();
                    if (tempQuestion != null)
                    {
                        questionId = tempQuestion.questionId ?? 0;
                        answerCount = tempQuestion.questionCount;
                    }
                }

                Epm_Question question = null;

                if (questionId == 0)
                {
                    question = context.Epm_Question.Where(p => p.IsDelete == false && p.State == 1).OrderByDescending(p => p.SubmitTime).FirstOrDefault();
                    if (question != null)
                    {
                        answerCount = context.Epm_QuestionTrack.Count(p => p.QuestionId == question.Id);
                    }
                }
                else
                {
                    question = context.Epm_Question.FirstOrDefault(p => p.Id == questionId);
                }

                if (question != null)
                {
                    QuestionModel view = QuestionModel.QuestionToView(question);
                    view.answerCount = answerCount;

                    result.Data = view;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Exception = new ExceptionEx(ex, "GetHotQuestion");
                result.Flag = EResultFlag.Failure;
            }
            finally
            {

            }
            return result;
        }

        /// <summary>
        /// 获取问题回复数
        /// </summary>
        /// <param name="questionIds">问题 ID 集合</param>
        /// <returns></returns>
        public Result<Dictionary<long, int>> GetQuestionTrackCount(List<long> questionIds)
        {
            Result<Dictionary<long, int>> result = new Result<Dictionary<long, int>>();
            try
            {
                var data = DataOperateBusiness<Epm_QuestionTrack>.Get().GetList(p => questionIds.Contains(p.QuestionId.Value)).GroupBy(p => p.QuestionId.Value).Select(p => new
                {
                    key = p.Key,
                    count = p.Count()
                }).ToDictionary(p => p.key, p => p.count);

                result.Data = data;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.Exception = new ExceptionEx(ex, "GetQuestionTrackCount");
            }
            return result;
        }

        /// <summary>
        /// 获取业务相关问题数
        /// </summary>
        /// <param name="businessIds">业务 ID</param>
        /// <returns></returns>
        public Result<Dictionary<long, int>> GetQuestionCount(List<long> businessIds)
        {
            Result<Dictionary<long, int>> result = new Result<Dictionary<long, int>>();
            try
            {
                var data = DataOperateBusiness<Epm_Question>.Get().GetList(p => businessIds.Contains(p.BusinessId.Value)).GroupBy(p => p.BusinessId.Value).Select(p => new
                {
                    key = p.Key,
                    count = p.Count()
                }).ToDictionary(p => p.key, p => p.count);

                result.Data = data;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.Exception = new ExceptionEx(ex, "GetQuestionCount");
            }
            return result;
        }

        /// <summary>
        /// 根据项目 ID 获取项目相关变更
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        public Result<List<Epm_Change>> GetChangeByProjectId(long projectId)
        {
            Result<List<Epm_Change>> result = new Result<List<Epm_Change>>();
            try
            {
                if (CurrentProjectIds.Contains(projectId.ToString()))
                {
                    var list = DataOperateBusiness<Epm_Change>.Get().GetList(p => p.ProjectId == projectId).ToList();
                    result.Data = list;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = new List<Epm_Change>();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "");
            }
            return result;
        }

        /// <summary>
        /// 获取签证列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Visa>> GetVisaListByQc(QueryCondition qc)
        {
            Result<List<Epm_Visa>> result = new Result<List<Epm_Visa>>();
            try
            {
                AddDefaultWeb(qc);
                result = DataOperate.QueryListSimple<Epm_Visa>(context, qc);
                return result;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVisaListByQc");
                result.AllRowsCount = -1;
                result.Data = null;
            }
            return result;
        }

        /// <summary>
        /// 获取 APP 最新版本
        /// </summary>
        /// <param name="appNum">APP 包名</param>
        /// <returns></returns>
        public Result<Epm_AppVersion> GetAppVersion(string appNum)
        {
            Result<Epm_AppVersion> result = new Result<Epm_AppVersion>();
            try
            {
                var version = DataOperateBusiness<Epm_AppVersion>.Get().Single(p => p.AppNum == appNum && p.State == 1);
                result.Data = version;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVersion");
            }
            return result;
        }

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public Result<bool> AddProjectWorkPoint(Epm_ProjectWorkMainPoints model)
        //{
        //    Result<bool> result = new Result<bool>();
        //    try
        //    {
        //        var data = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().Single(p => p.ProjectId == model.ProjectId && p.WorkMainPointsValue == model.WorkMainPointsValue);
        //        if (data != null)
        //        {
        //            throw new Exception(string.Format("该项目下已存在同名【{0}】工程内容要点！", model.WorkMainPointsValue));
        //        }

        //        SetCreateUser(model);
        //        SetCurrentUser(model);

        //        DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().Add(model);

        //        result.Data = true;
        //        result.Flag = EResultFlag.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = false;
        //        result.Flag = EResultFlag.Failure;
        //        result.Exception = new ExceptionEx(ex, "AddProjectWorkPoint");
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 根据 ID 修改工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public Result<bool> UpdateProjectWorkPointById(Epm_ProjectWorkMainPoints model)
        //{
        //    Result<bool> result = new Result<bool>();
        //    try
        //    {
        //        var data = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().GetModel(model.Id);
        //        if (data == null)
        //        {
        //            throw new Exception("内容不存在或已被删除！");
        //        }

        //        Epm_ProjectWorkMainPointsHistory historyModel = new Epm_ProjectWorkMainPointsHistory();
        //        //historyModel.Id = data.Id;
        //        historyModel.ProjectId = data.ProjectId;
        //        historyModel.WorkMainPointsKey = data.WorkMainPointsKey;
        //        historyModel.WorkMainPointsValue = data.WorkMainPointsValue;
        //        historyModel.IsCharging = data.IsCharging;
        //        historyModel.CompanyId = data.CompanyId;
        //        historyModel.CompanyName = data.CompanyName;
        //        historyModel.Qty = data.Qty;
        //        historyModel.Description = data.Description;
        //        historyModel.Sort = data.Sort;
        //        historyModel.CreateTime = data.CreateTime;
        //        historyModel.CreateUserId = data.CreateUserId;
        //        historyModel.CreateTime = data.CreateTime;
        //        historyModel.OperateUserId = data.OperateUserId;
        //        historyModel.OperateUserName = data.OperateUserName;
        //        historyModel.OperateTime = data.OperateTime;
        //        historyModel.IsDelete = false;

        //        DataOperateBusiness<Epm_ProjectWorkMainPointsHistory>.Get().Add(historyModel);

        //        data.ProjectId = model.ProjectId;
        //        data.WorkMainPointsValue = model.WorkMainPointsValue;
        //        data.CompanyName = model.CompanyName;
        //        data.Qty = model.Qty;
        //        data.Description = model.Description;

        //        SetCurrentUser(data);

        //        DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().Update(data);

        //        #region 消息
        //        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(data.ProjectId.Value);

        //        var waitSend = GetWaitSendMessageList(project.Id);
        //        foreach (var send in waitSend)
        //        {
        //            Epm_Massage modelMsg = new Epm_Massage();
        //            modelMsg.ReadTime = null;
        //            modelMsg.RecId = send.Key;
        //            modelMsg.RecName = send.Value;
        //            modelMsg.RecTime = DateTime.Now;
        //            modelMsg.SendId = CurrentUserID.ToLongReq();
        //            modelMsg.SendName = CurrentUserName;
        //            modelMsg.SendTime = DateTime.Now;
        //            modelMsg.Title = CurrentUserName + "更新工程内容要点信息";
        //            modelMsg.Content = CurrentUserName + "更新工程内容要点信息";
        //            modelMsg.Type = 2;
        //            modelMsg.IsRead = false;
        //            modelMsg.BussinessId = project.Id;
        //            modelMsg.BussinesType = BusinessType.Project.ToString();
        //            modelMsg.BussinesChild = "WorkPoints";
        //            modelMsg.ProjectId = project.Id;
        //            modelMsg.ProjectName = project.Name;
        //            modelMsg = base.SetCurrentUser(modelMsg);
        //            modelMsg = base.SetCreateUser(modelMsg);
        //            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
        //        }
        //        #endregion

        //        result.Data = true;
        //        result.Flag = EResultFlag.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = false;
        //        result.Flag = EResultFlag.Failure;
        //        result.Exception = new ExceptionEx(ex, "UpdateProjectWorkPointById");
        //    }
        //    return result;
        //}

        /// <summary>
        /// 修改项目供应商负责人及项目经理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> UpdateProjectCompanyPmInfo(Epm_ProjectCompany projectCompany, long userId)
        {
            Result<bool> result = new Result<bool>();
            int rows = -1;
            try
            {
                Epm_ProjectCompany pc = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(projectCompany.Id);
                if (pc == null)
                {
                    throw new Exception("供应商信息不存在或已被删除！");
                }
                #region 更新项目经理信息
                if (IsBranchCompanyUser(userId))//是否分公司
                {
                    //分公司修改不需要审核
                    pc.PMId_New = projectCompany.PMId_New;
                    pc.PM_New = projectCompany.PM_New;
                    pc.PMPhone_New = projectCompany.PMPhone_New;
                    pc.PMId = projectCompany.PMId_New;
                    pc.PM = projectCompany.PM_New;
                    pc.PMPhone = projectCompany.PMPhone_New;

                    pc.LinkManId_New = projectCompany.LinkManId_New;
                    pc.LinkMan_New = projectCompany.LinkMan_New;
                    pc.LinkPhone_New = projectCompany.LinkPhone_New;
                    pc.LinkManId = projectCompany.LinkManId_New;
                    pc.LinkMan = projectCompany.LinkMan_New;
                    pc.LinkPhone = projectCompany.LinkPhone_New;

                    pc.SafeManId = projectCompany.SafeManId_New;
                    pc.SafeMan = projectCompany.SafeMan_New;
                    pc.SafePhone = projectCompany.SafePhone_New;
                    pc.SafeManId_New = projectCompany.SafeManId_New;
                    pc.SafeMan_New = projectCompany.SafeMan_New;
                    pc.SafePhone_New = projectCompany.SafePhone_New;

                    pc.State = (int)ApprovalState.ApprSuccess;
                    pc = base.FiterUpdate(pc, pc);

                    pc.State = (int)ApprovalState.ApprSuccess;
                    rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);
                }
                else
                {
                    pc.PMId_New = projectCompany.PMId_New;
                    pc.PM_New = projectCompany.PM_New;
                    pc.PMPhone_New = projectCompany.PMPhone_New;

                    pc.LinkManId_New = projectCompany.LinkManId_New;
                    pc.LinkMan_New = projectCompany.LinkMan_New;
                    pc.LinkPhone_New = projectCompany.LinkPhone_New;

                    pc.SafeManId_New = projectCompany.SafeManId_New;
                    pc.SafeMan_New = projectCompany.SafeMan_New;
                    pc.SafePhone_New = projectCompany.SafePhone_New;

                    pc.State = (int)ApprovalState.WaitAppr;
                    pc = base.FiterUpdate(pc, pc);
                    rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "更新了服务商用户信息，待审核";
                    app.Content = CurrentUserName + "更新了服务商用户信息，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Project.ToString();
                    app.BussinesChild = "ISP";
                    app.Action = SystemRight.Modify.ToString();
                    app.BusinessTypeName = BusinessType.Project.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = pc.Id;
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = pc.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "服务商用户信息更新生成待办: " + pc.ProjectId);
                    #endregion

                }
                #endregion

                result.Flag = EResultFlag.Success;
                result.Data = true;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "UpdateProjectCompanyPmInfo");
            }
            return result;
        }

        /// <summary>
        /// 审核、驳回服务商PM和负责人
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<bool> AuditProjectCompanyPmAndLink(long Id, ApprovalState state)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                Epm_ProjectCompany pc = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(Id);

                if (state == ApprovalState.ApprSuccess)
                {
                    pc.PMId = pc.PMId_New;
                    pc.PM = pc.PM_New;
                    pc.PMPhone = pc.PMPhone_New;
                    pc.LinkManId = pc.LinkManId_New;
                    pc.LinkMan = pc.LinkMan_New;
                    pc.LinkPhone = pc.LinkPhone_New;
                    pc.SafeManId = pc.SafeManId_New;
                    pc.SafeMan = pc.SafeMan_New;
                    pc.SafePhone = pc.SafePhone_New;
                    pc = base.FiterUpdate(pc, pc);
                }
                pc.State = (int)state;
                pc.LinkState = (int)state;
                DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);

                #region 处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == pc.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }
                #endregion

                #region 消息
                var waitSend = GetWaitSendMessageList(pc.ProjectId.Value);
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
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
                    if (state == ApprovalState.ApprSuccess)
                    {
                        modelMsg.Title = CurrentUserName + "审核通过了你提交的服务商用户修改信息";
                        modelMsg.Content = CurrentUserName + "审核通过了你提交的服务商用户修改信息";
                    }
                    else if (state == ApprovalState.ApprFailure)
                    {
                        modelMsg.Title = CurrentUserName + "驳回了你提交的服务商用户修改信息";
                        modelMsg.Content = CurrentUserName + "驳回了你提交的服务商用户修改信息";
                    }
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Project.ToString();
                    modelMsg.BussinesChild = "ISP";
                    modelMsg.ProjectId = model.Id;
                    modelMsg.ProjectName = model.Name;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Flag = EResultFlag.Success;
                result.Data = true;
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "AuditProjectCompanyPmAndLink");
            }
            return result;
        }

        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        public Result<AttendanceView> GetAttendanceModel()
        {
            Result<AttendanceView> result = new Result<AttendanceView>();
            try
            {
                AttendanceView model = new AttendanceView();
                var list = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(t => t.ProjectId == 0).ToList();

                if (list.Any())
                {
                    model.AttendanceList = list.Select(t => t.AttendanceType).Distinct().ToList();
                    model.MarginError = list.FirstOrDefault().MarginError;
                    var time = list.Select(t => t.AttendanceTime).ToList().Distinct();
                    model.AttendanceTimeList = new List<string>();
                    foreach (var item in time)
                    {
                        model.AttendanceTimeList.Add(item);
                    }
                    model.Num = time.Count();
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAttendanceModel");
            }
            return result;
        }
        #endregion
    }
}
