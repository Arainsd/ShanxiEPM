using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddPlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;
                model.ParentId = model.ParentId == null ? 0 : model.ParentId;
                model.BuildCompanyId = CurrentCompanyID.ToLongReq();
                model.BuildCompanyName = CurrentCompanyName;
                int state = (int)(ApprovalState.Enabled);
                //同一项目、同一批次下计划名称不能重复；
                bool dConfig = DataOperateBusiness<Epm_Plan>.Get().Count(i => i.Name == model.Name && i.ProjectId == model.ProjectId && i.BatchNo == model.BatchNo) > 0;
                if (dConfig)
                {
                    throw new Exception("该计划名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_Plan>.Get().Count(i => i.BatchNo == model.BatchNo && i.State != state && i.ProjectId == model.ProjectId) > 0;
                if (dConfig)
                {
                    throw new Exception("该计划批次号已经存在");
                }

                var rows = DataOperateBusiness<Epm_Plan>.Get().Add(model);

                if (planComponentIds.Any())
                {
                    List<Epm_PlanComponent> dataList = new List<Epm_PlanComponent>();
                    foreach (var item in planComponentIds)
                    {
                        Epm_PlanComponent temp = new Epm_PlanComponent();
                        temp = base.SetCurrentUser(temp);
                        temp.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        temp.CrtCompanyName = CurrentCompanyName;

                        temp.PlanId = model.Id;
                        temp.PlanName = model.Name;
                        dataList.Add(temp);
                    }
                    //批量添加计划关联模型
                    DataOperateBusiness<Epm_PlanComponent>.Get().AddRange(dataList);
                }
                else
                {
                    throw new Exception("没有选择关联组件，请选择要关联的组件！");
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Plan.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddPlan");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdatePlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds)
        {
            Result<int> result = new Result<int>();
            int state = (int)(ApprovalState.Enabled);

            try
            {
                var oldModel = DataOperateBusiness<Epm_Plan>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                bool dConfig = DataOperateBusiness<Epm_Plan>.Get().Count(i => i.Name == model.Name && i.Id != model.Id && i.ProjectId == model.ProjectId && i.BatchNo == model.BatchNo) > 0;
                if (dConfig)
                {
                    throw new Exception("该计划名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_Plan>.Get().Count(i => i.BatchNo == model.BatchNo && i.State != state && i.Id != model.Id && i.ProjectId == model.ProjectId) > 0;
                if (dConfig)
                {
                    throw new Exception("该计划批次号已经存在");
                }
                var rows = DataOperateBusiness<Epm_Plan>.Get().Update(model);

                if (planComponentIds.Any())
                {
                    //首先删除同一模型同一计划下的所有模型组件
                    var componentList = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(i => i.PlanId == model.Id).ToList();
                    if (componentList.Count > 0)
                    {
                        foreach (var item in componentList)
                        {
                            item.OperateUserId = CurrentUserID.ToLongReq();
                            item.OperateUserName = CurrentUserName;
                            item.OperateTime = DateTime.Now;
                            item.DeleteTime = DateTime.Now;
                        }
                        DataOperateBusiness<Epm_PlanComponent>.Get().DeleteRange(componentList);
                    }
                    List<Epm_PlanComponent> dataList = new List<Epm_PlanComponent>();
                    foreach (var item in planComponentIds)
                    {
                        Epm_PlanComponent temp = new Epm_PlanComponent();
                        temp = base.SetCurrentUser(temp);
                        temp.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        temp.CrtCompanyName = CurrentCompanyName;

                        temp.PlanId = model.Id;
                        temp.PlanName = model.Name;
                        dataList.Add(temp);
                    }
                    //批量添加计划关联模型
                    DataOperateBusiness<Epm_PlanComponent>.Get().AddRange(dataList);
                }
                else
                {
                    throw new Exception("没有选择关联组件，请选择要关联的组件！");
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Plan.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdatePlan");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeletePlanByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            int state = (int)(ApprovalState.ApprSuccess);
            try
            {
                if (ids.Count <= 0)
                {
                    throw new Exception("请选择要删除的问题！");
                }

                var plan = DataOperateBusiness<Epm_Plan>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (plan == null)
                {
                    throw new Exception("要删除的问题不存在或已被删除！");
                }

                //查看是否存在已经审核通过的施工计划数据
                var list = DataOperateBusiness<Epm_Plan>.Get().GetList(i => ids.Contains(i.Id) && i.State == state).ToList();
                //如果已审核通过的计划，不能删除
                if (list.Count > 0)
                {
                    throw new Exception("存在已经审核通过的施工计划，请重新选择！");
                }
                else
                {
                    var models = DataOperateBusiness<Epm_Plan>.Get().GetList(i => ids.Contains(i.Id) && i.State != state).ToList();
                    foreach (var model in models)
                    {
                        model.OperateUserId = CurrentUserID.ToLongReq();
                        model.OperateUserName = CurrentUserName;
                        model.OperateTime = DateTime.Now;
                        model.DeleteTime = DateTime.Now;
                    }

                    var rows = DataOperateBusiness<Epm_Plan>.Get().DeleteRange(models);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Plan.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeletePlanByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Plan>> GetPlanList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Plan>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetPlanList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<PlanView> GetPlanModel(long id)
        {
            Result<PlanView> result = new Result<PlanView>();
            try
            {
                PlanView list = new PlanView();
                var model = DataOperateBusiness<Epm_Plan>.Get().GetModel(id);
                if (model != null)
                {
                    list.Plan = model;
                    list.EpmPlanComponent = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(t => t.PlanId == id).ToList();
                }
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetPlanModel");
            }
            return result;
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> ChangePlanState(string batchNo, ApprovalState state, string reason)
        {
            Result<int> result = new Result<int>();
            try
            {
                List<Epm_Plan> list = null;
                int planState = 0;
                if (state == ApprovalState.WaitAppr) //提交审核数据
                {
                    planState = (int)(ApprovalState.Enabled);
                }
                else if (state == ApprovalState.ApprSuccess || state == ApprovalState.ApprFailure)  //驳回/审核（当前批次下的计划是待审核状态才可以被驳回/审核）
                {
                    planState = (int)(ApprovalState.WaitAppr);
                }
                else if (state == ApprovalState.Discarded)  //废弃
                {
                    planState = (int)(ApprovalState.ApprFailure);
                }

                list = DataOperateBusiness<Epm_Plan>.Get().GetList(i => i.BatchNo == batchNo && i.State == planState).ToList();
                if (list.Count == 0)
                {
                    throw new Exception("该计划不存在或已被删除！");
                }
                foreach (var item in list)
                {
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.State = int.Parse(state.GetValue().ToString());
                }
                var rows = DataOperateBusiness<Epm_Plan>.Get().UpdateRange(list);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangePlanState");
            }
            return result;
        }

        /// <summary>
        /// 根据计划id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="PlanId"></param>
        /// <param name="bimId"></param>
        /// <returns></returns>
        public Result<List<Epm_PlanComponent>> GetComponentListByPlanId(long PlanId, long bimId)
        {
            Result<List<Epm_PlanComponent>> result = new Result<List<Epm_PlanComponent>>();
            try
            {
                var model = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(p => p.PlanId == PlanId && p.BIMId == bimId).ToList();

                if (model.Count == 0)
                {
                    throw new Exception("该模型组件不存在或已被删除！");
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetComponentListByPlanId");
            }
            return result;
        }

        /// <summary>
        /// 添加计划关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="planComponentIds"></param>
        /// <returns></returns>
        public Result<int> AddPlanComponent(Epm_PlanComponent model, string planComponentIds)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (!string.IsNullOrEmpty(planComponentIds))
                {
                    //首先删除同一模型同一计划下的所有模型组件
                    var componentList = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(i => i.PlanId == model.PlanId && i.BIMId == model.BIMId).ToList();
                    if (componentList.Count > 0)
                    {
                        foreach (var item in componentList)
                        {
                            item.OperateUserId = CurrentUserID.ToLongReq();
                            item.OperateUserName = CurrentUserName;
                            item.OperateTime = DateTime.Now;
                            item.DeleteTime = DateTime.Now;
                        }
                        DataOperateBusiness<Epm_PlanComponent>.Get().DeleteRange(componentList);
                    }

                    //获取组件ID集合
                    var planComponentIdList = planComponentIds.SplitString(",");
                    List<Epm_PlanComponent> dataList = new List<Epm_PlanComponent>();
                    foreach (var item in planComponentIdList)
                    {
                        Epm_PlanComponent temp = new Epm_PlanComponent();
                        temp = base.SetCurrentUser(temp);
                        temp.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        temp.CrtCompanyName = CurrentCompanyName;

                        temp.PlanId = model.PlanId;
                        temp.PlanName = model.PlanName;
                        temp.BIMId = model.BIMId;
                        temp.BIMName = model.BIMName;
                        temp.ComponentId = item;
                        dataList.Add(temp);
                    }
                    //批量添加计划关联模型
                    var rows = DataOperateBusiness<Epm_PlanComponent>.Get().AddRange(dataList);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Plan.GetText(), SystemRight.Add.GetText(), "新增:添加计划关联模型 " + model.Id);
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
                result.Exception = new ExceptionEx(ex, "AddPlanComponent");
            }
            return result;
        }

        /// <summary>
        /// 根据parentId获取计划信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public Result<Epm_Plan> GetPlanById(long parentId)
        {
            Result<Epm_Plan> result = new Result<Epm_Plan>();
            try
            {
                var rows = DataOperateBusiness<Epm_Plan>.Get().GetModel(parentId);
                if (rows == null)
                {
                    throw new Exception("该计划不存在或已被删除！");
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetPlanById");
            }
            return result;
        }

        #region 获取施工计划树形列表数据
        /// <summary>
        /// 获取施工计划树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<PlanView>> GetPlanViewList(long ProjectId)
        {
            List<PlanView> list = new List<PlanView>();
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            try
            {
                var temp = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == ProjectId).ToList();

                if (temp.Count > 0)
                {
                    // 1. 筛选出所有一级计划；
                    var firstList = temp.Where(t => t.ParentId == 0 || !t.ParentId.HasValue).ToList();
                    // 2. 便利所有一级计划，查找子级；
                    if (firstList.Count > 0)
                    {
                        foreach (var item in firstList)
                        {
                            PlanView rootTree = new PlanView();
                            rootTree = GetPlanTree(item.Id, temp);
                            list.Add(rootTree);
                        }
                    }

                }
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetPlanViewList");
            }
            return result;
        }


        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public PlanView GetPlanTree(long pId, List<Epm_Plan> list)
        {
            List<Epm_Plan> result = new List<Epm_Plan>();

            //result = DataOperateBusiness<Epm_Plan>.Get().GetList().ToList();
            PlanView rootTree = new PlanView();

            var first = list.FirstOrDefault(i => i.Id == pId);

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.Name = first.Name;
            rootTree.BatchNo = first.BatchNo;
            rootTree.StartTime = first.StartTime;
            rootTree.EndTime = first.EndTime;
            rootTree.BuildDays = first.BuildDays;
            rootTree.MilepostName = first.MilepostName;
            rootTree.MilepostId = first.MilepostId == null ? "" : first.MilepostId.Value.ToString();
            rootTree.State = first.State;
            rootTree.ProjectName = first.ProjectName;
            rootTree.ProjectId = first.ProjectId.Value;
            rootTree.ParentId = first.ParentId;
            rootTree.DelayTime = first.DelayTime;
            rootTree.FactEndTime = first.EndDate;
            rootTree.FactStartTime = first.StartDate;
            rootTree.FinishScale = first.FinishScale;
            rootTree.IsFinish = first.IsFinish;
            rootTree.ToResean = first.NoFinishResean;
            rootTree.BuildCompanyId = first.BuildCompanyId;
            rootTree.BuildCompanyName = first.BuildCompanyName;
            rootTree.ContactUserName = first.ContactUserName;
            rootTree.ContactUserId = first.ContactUserId;
            rootTree.CreateUserId = first.CreateUserId;
            rootTree.CreateUserName = first.CreateUserName;

            var tree = createTrees(first.Id, list);

            rootTree.children = tree;
            return rootTree; ;
        }
        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<PlanView> createTrees(long parentId, List<Epm_Plan> allList)
        {
            List<PlanView> list = new List<PlanView>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    PlanView node = new PlanView();
                    node.checkboxValue = item.Id.ToString();
                    node.Name = item.Name ?? "";
                    node.BatchNo = item.BatchNo ?? "";
                    node.StartTime = item.StartTime;
                    node.EndTime = item.EndTime;
                    node.BuildDays = item.BuildDays ?? 0;
                    node.MilepostName = item.MilepostName;
                    node.MilepostId = item.MilepostId == null ? "0" : item.MilepostId.Value.ToString();
                    node.State = item.State ?? 0;
                    node.ProjectName = item.ProjectName ?? "";
                    node.ProjectId = item.ProjectId.Value;
                    node.ParentId = item.ParentId ?? 0;
                    node.FactEndTime = item.EndDate;
                    node.FactStartTime = item.StartDate;
                    node.FinishScale = item.FinishScale;
                    node.IsFinish = item.IsFinish;
                    node.ToResean = item.NoFinishResean;
                    node.BuildCompanyId = item.BuildCompanyId ?? 0;
                    node.BuildCompanyName = item.BuildCompanyName ?? "";
                    node.ContactUserName = item.ContactUserName ?? "";
                    node.ContactUserId = item.ContactUserId ?? 0;
                    node.CreateUserId = item.CreateUserId;
                    node.CreateUserName = item.CreateUserName ?? "";

                    var iteratorList = createTrees(item.Id, allList);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }

        #endregion

        #region 获取进度跟踪树形列表数据
        /// <summary>
        /// 获取进度跟踪树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<PlanView>> GetScheduleViewList(long ProjectId)
        {
            List<PlanView> list = new List<PlanView>();
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            try
            {
                //var temp = new List<Epm_Plan>();
                //var temp = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == ProjectId).ToList();
                var temp = (from plan in context.Epm_Plan.Where(p => p.IsDelete == false)
                            join sup in context.Epm_SupervisorLogDetails.Where(p => p.IsDelete == false) on plan.Id equals
                                sup.PlanId
                            where plan.ProjectId == ProjectId
                            select new PlanView
                            {
                                ProjectName = plan.ProjectName,
                                ParentId = plan.ParentId.Value,
                                ProjectId = plan.ProjectId.Value,
                                StartTime = plan.StartTime,
                                EndTime = plan.EndTime,
                                FactStartTime = sup.StartTime,
                                FactEndTime = sup.EndTime,
                                ToResean = sup.ToResean,
                                //FinishScale = sup.FinishScale,
                                Name = plan.Name,
                                BuildDays = plan.BuildDays,
                                BatchNo = plan.BatchNo,
                                MilepostName = plan.MilepostName,
                                IsFinish = sup.IsFinish,
                                Id = plan.Id,
                                SupervisorLogDetailsId = sup.Id,
                                State = plan.State,
                                BuildCompanyName = plan.BuildCompanyName,
                                BuildCompanyId = plan.BuildCompanyId.Value,
                                ContactUserId = plan.ContactUserId.Value,
                                ContactUserName = plan.ContactUserName
                            }).ToList();

                if (temp.Count > 0)
                {
                    // 1. 筛选出所有一级计划；
                    var firstList = temp.Where(t => t.ParentId == 0).ToList();
                    // 2. 便利所有一级计划，查找子级；
                    if (firstList.Count > 0)
                    {
                        foreach (var item in firstList)
                        {
                            PlanView rootTree = new PlanView();
                            rootTree = GetScheduleTree(item.Id, temp);
                            list.Add(rootTree);
                        }
                    }

                }
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetScheduleViewList");
            }
            return result;
        }


        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public PlanView GetScheduleTree(long pId, List<PlanView> list)
        {
            List<Epm_Plan> result = new List<Epm_Plan>();
            PlanView rootTree = new PlanView();

            var first = list.FirstOrDefault(i => i.Id == pId);

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.Name = first.Name;
            rootTree.BatchNo = first.BatchNo;
            rootTree.StartTime = first.StartTime;
            rootTree.EndTime = first.EndTime;
            rootTree.BuildDays = first.BuildDays;
            rootTree.MilepostName = first.MilepostName;
            rootTree.State = first.State;
            rootTree.ProjectName = first.ProjectName;
            rootTree.SupervisorLogDetailsId = first.SupervisorLogDetailsId;
            rootTree.ProjectId = first.ProjectId;
            rootTree.ParentId = first.ParentId;
            rootTree.FactEndTime = first.FactEndTime;
            rootTree.FactStartTime = first.FactStartTime;
            rootTree.FinishScale = first.FinishScale;
            rootTree.IsFinish = first.IsFinish;
            rootTree.ToResean = first.ToResean;
            rootTree.BuildCompanyId = first.BuildCompanyId;
            rootTree.BuildCompanyName = first.BuildCompanyName;
            rootTree.ContactUserName = first.ContactUserName;
            rootTree.ContactUserId = first.ContactUserId;

            var tree = createScheduleTrees(first.Id, list);

            rootTree.children = tree;
            return rootTree; ;
        }
        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<PlanView> createScheduleTrees(long parentId, List<PlanView> allList)
        {
            List<PlanView> list = new List<PlanView>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    PlanView node = new PlanView();
                    node.checkboxValue = item.Id.ToString();
                    node.Name = item.Name;
                    node.BatchNo = item.BatchNo;
                    node.StartTime = item.StartTime;
                    node.EndTime = item.EndTime;
                    node.BuildDays = item.BuildDays;
                    node.MilepostName = item.MilepostName;
                    node.State = item.State;
                    node.ProjectName = item.ProjectName;
                    node.SupervisorLogDetailsId = item.SupervisorLogDetailsId;
                    node.ProjectId = item.ProjectId;
                    node.ParentId = item.ParentId;
                    node.FactEndTime = item.FactEndTime;
                    node.FactStartTime = item.FactStartTime;
                    node.FinishScale = item.FinishScale;
                    node.IsFinish = item.IsFinish;
                    node.ToResean = item.ToResean;
                    node.BuildCompanyId = item.BuildCompanyId;
                    node.BuildCompanyName = item.BuildCompanyName;
                    node.ContactUserName = item.ContactUserName;
                    node.ContactUserId = item.ContactUserId;

                    var iteratorList = createScheduleTrees(item.Id, allList);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }

        #endregion


        #region 里程碑计划
        /// <summary>
        /// 获取里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<PlanView>> GetMilepostPlan(long projectId)
        {
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            try
            {
                List<PlanView> viewlist = new List<PlanView>();
                PlanView view = null;

                List<Epm_Plan> planlist = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == projectId).OrderBy(o => o.StartTime).ToList();
                foreach (var item in planlist)
                {
                    var planComponent = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(t => t.PlanId == item.Id).OrderBy(o => o.Id).ToList();
                    view = new PlanView();
                    view.Plan = item;
                    view.EpmPlanComponent = planComponent;
                    viewlist.Add(view);
                }

                result.Data = viewlist;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostPlan");
            }
            return result;
        }
        /// <summary>
        /// 生成里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="planStart"></param>
        /// <param name="mileType">工程节点类型</param>
        /// <param name="type">1监理生成，2施工生成</param>
        /// <returns></returns>
        public Result<List<Epm_Plan>> CreateMilepostPlan(long projectId, DateTime planStart, long mileType, int type = 1)
        {
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            try
            {
                var planlist = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == projectId).ToList();
                if (planlist != null && planlist.Count > 0)
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                    result.Exception = new ExceptionEx(new Exception("已存在工程节点计划"), "CreateMilepostPlan");
                }
                else
                {
                    //var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                    //var milepost = DataOperateBusiness<Epm_Milepost>.Get().GetList(t => t.ParentId == 0 && t.Code == project.ProjectNature).FirstOrDefault();
                    List<Epm_Milepost> milepostList = DataOperateBusiness<Epm_Milepost>.Get().GetList(t => t.ParentId == mileType).OrderBy(t => t.Sort).ToList();
                    if (milepostList == null || milepostList.Count == 0)
                    {
                        result.Data = null;
                        result.Flag = EResultFlag.Failure;
                        result.Exception = new ExceptionEx(new Exception("生成工程节点计划失败，无法获取里程碑"), "CreateMilepostPlan");
                    }
                    else
                    {
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                        List<Epm_Plan> list = new List<Epm_Plan>();
                        Epm_Plan plan = null;
                        foreach (var item in milepostList)
                        {
                            plan = new Epm_Plan();
                            plan.ProjectId = projectId;
                            plan.ProjectName = project.Name;
                            plan.Name = item.Name;
                            plan.MilepostId = item.Id;
                            plan.MilepostName = item.Name;
                            plan.StartTime = planStart;
                            plan.EndTime = planStart.AddDays(item.Limit - 1);
                            plan.BuildDays = item.Limit;
                            plan.State = (type == 1 ? (int)ApprovalState.WaitAppr : (int)ApprovalState.Enabled);
                            plan = base.SetCurrentUser(plan);
                            list.Add(plan);
                            planStart = planStart.AddDays(item.Limit);
                        }
                        DataOperateBusiness<Epm_Plan>.Get().AddRange(list);

                        var mil = DataOperateBusiness<Epm_Milepost>.Get().GetModel(mileType);
                        project.MilepostType = mil.Code;
                        project.MilepostName = mil.Name;
                        DataOperateBusiness<Epm_Project>.Get().Update(project);
                        result.Data = list;
                        result.Flag = EResultFlag.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CreateMilepostPlan");
            }
            return result;
        }
        /// <summary>
        /// 更新工程节点计划
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type">1保存，2提交</param>
        /// <returns></returns>
        public Result<int> UpdateMilepostPlan(List<Epm_Plan> listPlan, int type = 1)
        {
            Result<int> result = new Result<int>();
            try
            {
                var projectId = listPlan[0].ProjectId.Value;
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);

                List<Epm_Plan> oldPlan = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == projectId).ToList();
                foreach (var item in oldPlan)
                {
                    var plan = listPlan.Where(t => t.Id == item.Id).First();
                    item.StartTime = plan.StartTime;
                    item.EndTime = plan.EndTime;
                    item.BuildDays = plan.BuildDays;
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.State = (type == 1 ? (int)ApprovalState.Enabled : (int)ApprovalState.WaitAppr);

                }
                var rows = DataOperateBusiness<Epm_Plan>.Get().UpdateRange(oldPlan);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                if (type == 2)
                {
                    //处理待办
                    var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == project.Id && t.IsApprover == false && t.BusinessTypeNo == BusinessType.Plan.ToString()).FirstOrDefault();
                    if (tempApp != null)
                    {
                        ComplateApprover(tempApp.Id);
                    }

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报的工程节点计划，待审核";
                    app.Content = CurrentUserName + "提报的工程节点计划，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Plan.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Plan.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = project.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = project.Id;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Plan.GetText(), SystemRight.Modify.GetText(), "服务商项目经理信息更新生成待办: " + projectId);
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterssm = new Dictionary<string, string>();
                    //parameterssm.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.PlanAdd, parameterssm);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMilepostPlan");
            }
            return result;
        }
        /// <summary>
        /// 审核工程节点计划(审核是支持修改）
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> AuditMilepostPlan(List<Epm_Plan> listPlan)
        {
            Result<int> result = new Result<int>();
            try
            {
                List<Epm_Plan> havPlanList = new List<Epm_Plan>();
                List<Epm_Plan> planList = new List<Epm_Plan>();
                long projectId = listPlan[0].ProjectId.Value;
                foreach (var item in listPlan)
                {
                    var havPlan = DataOperateBusiness<Epm_Plan>.Get().GetModel(item.Id);
                    if (havPlan == null)
                    {
                        var prpjectInfo = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                        Epm_Plan plan = new Epm_Plan();
                        plan = base.SetCurrentUser(plan);
                        plan = base.SetCreateUser(plan);
                        plan.Name = item.Name;
                        plan.StartTime = item.StartTime;
                        plan.EndTime = item.EndTime;
                        plan.BuildDays = item.BuildDays;
                        plan.State = (int)ApprovalState.ApprSuccess;
                        plan.MilepostId = 0;
                        plan.MilepostName = item.Name;
                        plan.ProjectId = projectId;
                        plan.ProjectName = prpjectInfo == null ? "" : prpjectInfo.Name;
                        plan.IsDelete = false;
                        planList.Add(plan);
                    }
                    else
                    {
                        string states = item.State.ToString();
                        if (states == "30")
                        {
                            havPlan.Id = item.Id;
                            havPlan.Name = item.Name;
                            havPlan.StartTime = item.StartTime;
                            havPlan.EndTime = item.EndTime;
                            havPlan.BuildDays = item.BuildDays;
                            havPlan.OperateUserId = CurrentUserID.ToLongReq();
                            havPlan.OperateUserName = CurrentUserName;
                            havPlan.OperateTime = DateTime.Now;
                            havPlan.State = (int)ApprovalState.WaitAppr;
                            havPlanList.Add(havPlan);
                        }
                        else {
                            havPlan.Id = item.Id;
                            havPlan.Name = item.Name;
                            havPlan.StartTime = item.StartTime;
                            havPlan.EndTime = item.EndTime;
                            havPlan.BuildDays = item.BuildDays;
                            havPlan.OperateUserId = CurrentUserID.ToLongReq();
                            havPlan.OperateUserName = CurrentUserName;
                            havPlan.OperateTime = DateTime.Now;
                            havPlan.State = (int)ApprovalState.ApprSuccess;
                            havPlanList.Add(havPlan);
                        }
                    }
                }
                int rows = 0;
                if (havPlanList.Count > 0)
                {
                    rows += DataOperateBusiness<Epm_Plan>.Get().UpdateRange(havPlanList);
                }
                if (planList.Count > 0)
                {
                    rows += DataOperateBusiness<Epm_Plan>.Get().AddRange(planList);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == projectId && t.IsApprover == false && t.BusinessTypeNo == BusinessType.Plan.ToString()).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                #region 更新工期
                List<Epm_Plan> plans = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == projectId).ToList();
                DateTime startTime = plans.OrderBy(t => t.StartTime).First().StartTime.Value;
                DateTime endTime = plans.OrderByDescending(t => t.EndTime).First().EndTime.Value;
                Epm_Project project = DataOperateBusiness<Epm_Project>.Get().GetList(t => t.Id == projectId).First();
                project.PlanWorkStartTime = startTime;
                project.PlanWorkEndTime = endTime;
                TimeSpan sp = endTime.Subtract(startTime);
                project.Limit = sp.Days + 1;
                DataOperateBusiness<Epm_Project>.Get().Update(project);
                #endregion

                //保存需要发送的基础数据
                //AddSendDateByProjectId(projectId);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditMilepostPlan");
            }
            return result;
        }

        /// <summary>
        /// 驳回工程节点计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> RejectMilepostPlan(long projectId)
        {
            Result<int> result = new Result<int>();
            try
            {
                List<Epm_Plan> list = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.ProjectId == projectId).ToList();
                foreach (var item in list)
                {
                    item.State = (int)ApprovalState.ApprFailure;
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_Plan>.Get().UpdateRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == projectId && t.IsApprover == false && t.BusinessTypeNo == BusinessType.Plan.ToString()).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                #region 生成待办
                List<Epm_Approver> listApp = new List<Epm_Approver>();
                Epm_Approver app = new Epm_Approver();
                app.Title = list[0].CreateUserName + "提报的工程节点计划已被驳回，请处理";
                app.Content = list[0].CreateUserName + "提报的工程节点计划已被驳回，请处理";
                app.SendUserId = list[0].CreateUserId;
                app.SendUserName = list[0].CreateUserName;
                app.SendTime = list[0].CreateTime;
                app.LinkURL = string.Empty;
                app.BusinessTypeNo = BusinessType.Plan.ToString();
                app.Action = SystemRight.UnCheck.ToString();
                app.BusinessTypeName = BusinessType.Plan.GetText();
                app.BusinessState = (int)(ApprovalState.WaitAppr);
                app.BusinessId = list[0].ProjectId;
                app.ApproverId = list[0].CreateUserId;
                app.ApproverName = list[0].CreateUserName;
                app.ProjectId = list[0].ProjectId;
                app.ProjectName = list[0].ProjectName;
                listApp.Add(app);
                AddApproverBatch(listApp);
                WriteLog(BusinessType.Plan.GetText(), SystemRight.Modify.GetText(), "服务商项目经理信息更新生成待办: " + projectId);
                #endregion

                #region 发送短信
                //WriteSMS(list[0].CreateUserId, 0, MessageStep.PlanReject, null);
                #endregion
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "RejectMilepostPlan");
            }
            return result;
        }
        /// <summary>
        /// 关联构件
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="pcList"></param>
        /// <returns></returns>
        public Result<int> BindComponents(long planId, List<Epm_PlanComponent> pcList)
        {

            Result<int> result = new Result<int>();
            try
            {
                #region 删除数据
                var models = DataOperateBusiness<Epm_PlanComponent>.Get().GetList(i => i.PlanId == planId).ToList();
                var delRows = DataOperateBusiness<Epm_PlanComponent>.Get().DeleteRange(models);
                #endregion

                if (pcList != null && pcList.Any())
                {
                    pcList.ForEach(p =>
                    {
                        p.PlanId = planId;
                        p.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        p.CrtCompanyName = CurrentCompanyName;
                        p.CreateUserId = CurrentUserID.ToLongReq();
                        p.CreateUserName = CurrentUserName;
                        p.CreateTime = DateTime.Now;
                        p.OperateUserId = CurrentUserID.ToLongReq();
                        p.OperateUserName = CurrentUserName;
                        p.OperateTime = DateTime.Now;
                        p.IsDelete = false;
                    });
                    var rows = DataOperateBusiness<Epm_PlanComponent>.Get().AddRange(pcList);
                    result.Data = rows;
                }
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "工程节点计划关联构件: " + planId);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "BindComponents");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 根据项目施工计划获取项目进度甘特图
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        public Result<List<Gantt>> GetProjectGantt(long projectId)
        {
            Result<List<Gantt>> result = new Result<List<Gantt>>();
            try
            {
                var planList = DataOperateBusiness<Epm_Plan>.Get().GetList(p => p.ProjectId == projectId).OrderBy(p => p.StartTime).ToList();
                List<Gantt> list = new List<Gantt>();
                foreach (var epmPlan in planList)
                {
                    Gantt gt = new Gantt();
                    gt.name = epmPlan.Name;
                    GanttItem item = new GanttItem();
                    item.from = ToMillisecondDate(epmPlan.StartTime.Value);
                    item.desc = string.Format("计划开工：{0} 计划完工：{1}", epmPlan.StartTime.ToString("yyyy-MM-dd"), epmPlan.EndTime.ToString("yyyy-MM-dd"));
                    if (epmPlan.EndDate.HasValue)
                    {
                        var time = epmPlan.DelayTime.HasValue ? epmPlan.DelayTime : epmPlan.EndTime;
                        time = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
                        item.desc = item.desc + (epmPlan.DelayTime.HasValue ? (" 变更后计划完工：" + epmPlan.DelayTime.ToString("yyyy-MM-dd") + " 实际完工：" + epmPlan.EndDate.ToString("yyyy-MM-dd")) : (" 实际完工：" + epmPlan.EndDate.ToString("yyyy-MM-dd")));

                        epmPlan.EndDate = Convert.ToDateTime(epmPlan.EndDate.ToString("yyyy-MM-dd"));
                        if (epmPlan.EndDate < time)
                        {
                            //绿色 提前完工，已完工，完工时间 小于 计划完工时间 or 变更完工时间
                            item.to = ToMillisecondDate(epmPlan.EndDate.Value);
                            item.customClass = GanttCustomerClass.ganttGreen.ToString();
                        }
                        else if (epmPlan.EndDate > time)
                        {
                            //红色 延期完工，已完工，完工时间 大于 计划完工时间 or 变更完工时间
                            item.to = ToMillisecondDate(epmPlan.EndDate.Value);
                            item.customClass = GanttCustomerClass.ganttRed.ToString();
                        }
                        else
                        {
                            //蓝色 正常完工，已完工，完工时间 等于 计划完工时间 or 变更完工时间
                            item.to = ToMillisecondDate(epmPlan.EndDate.Value);
                            item.customClass = GanttCustomerClass.ganttBlue.ToString();
                        }
                    }
                    else
                    {
                        if (epmPlan.DelayTime.HasValue)
                        {
                            //橘色 变更工期，未完工，已变更
                            item.to = ToMillisecondDate(epmPlan.DelayTime.Value);
                            item.customClass = GanttCustomerClass.ganttOrange.ToString();
                            item.desc = item.desc + " 变更后计划完工：" + epmPlan.DelayTime.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            //灰色 计划工期，未完工，未变更
                            item.to = ToMillisecondDate(epmPlan.EndTime.Value);
                            item.customClass = GanttCustomerClass.ganttGray.ToString();
                        }
                    }
                    gt.values.Add(item);
                    list.Add(gt);
                }

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = new List<Gantt>();
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
        private string ToMillisecondDate(DateTime dt)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Date;
            return "/Date(" + ((dt.Date - startTime.Date).TotalSeconds * 1000) + ")/";
        }
    }
}