using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using hc.Plat.Common.Service;
using hc.epm.ViewModel;
using hc.epm.DataModel.Basic;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        /// 添加专项验收
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddSpecialAcceptance(SpecialAcceptanceView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写专项验收内容！");
                }
                if (!model.AcceptanceDetailList.Any() && (int)ApprovalState.WaitAppr == model.State)
                {
                    throw new Exception("请添加验收项目后在提交审核！");
                }

                DbContextTransaction transaction = context.Database.BeginTransaction();

                try
                {
                    Epm_SpecialAcceptance specialAcceptance = new Epm_SpecialAcceptance();
                    specialAcceptance.ProjectId = model.ProjectId;
                    specialAcceptance.ProjectName = model.ProjectName;
                    specialAcceptance.TypeId = model.TypeId;
                    specialAcceptance.TypeName = model.TypeName;
                    specialAcceptance.Title = model.Title;

                    specialAcceptance.Content = model.Content;
                    specialAcceptance.Num = model.Num;
                    specialAcceptance.RecCompanyId = CurrentUser.CompanyId;
                    specialAcceptance.RecCompanyName = CurrentCompanyName;
                    specialAcceptance.RecUserId = CurrentUser.Id;
                    specialAcceptance.RecTime = model.RecTime;

                    specialAcceptance.RecUserName = CurrentUserName;
                    specialAcceptance.AcceptanceResult = model.AcceptanceResult;
                    specialAcceptance.State = model.State;
                    specialAcceptance.Remark = model.Remark;
                    specialAcceptance.CrtCompanyId = CurrentUser.CompanyId;
                    specialAcceptance.CrtCompanyName = CurrentCompanyName;

                    specialAcceptance.Num = specialAcceptance.Num ?? 1;
                    specialAcceptance.IsDelete = false;
                    specialAcceptance = SetCurrentUser(specialAcceptance);
                    specialAcceptance = SetCreateUser(specialAcceptance);

                    DataOperateBusiness<Epm_SpecialAcceptance>.Get().Add(specialAcceptance);
                    model.AcceptanceDetailList.ForEach(p =>
                    {
                        p.State = 0;
                        p.SpecialCheckId = specialAcceptance.Id;
                        p.CrtCompanyId = CurrentUser.CompanyId;
                        p.CrtCompanyName = CurrentCompanyName;
                        p = SetCurrentUser(p);
                        p = SetCreateUser(p);
                    });
                    DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get().AddRange(model.AcceptanceDetailList);

                    transaction.Commit();

                    if (model.AttachList.Any())
                    {
                        AddFilesByTable(specialAcceptance, model.AttachList);
                    }

                    result.Data = 1;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Special.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSpecialAcceptance");
            }
            return result;
        }

        ///<summary>
        /// 修改专项验收
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateSpecialAcceptance(SpecialAcceptanceView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null || model.Id <= 0)
                {
                    throw new Exception("请选择要修改的专项验收！");
                }

                var data = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetModel(model.Id);
                if (data == null)
                {
                    throw new Exception("要修改的专项验收信息不存在或已被删除！");
                }
                if (!model.AcceptanceDetailList.Any() && (int)ApprovalState.WaitAppr == model.State)
                {
                    throw new Exception("请添加验收项目后在提交审核！");
                }

                data.Title = model.Title;
                data.Content = model.Content;
                data.Num = model.Num;
                data.RecTime = model.RecTime;
                data.State = model.State;
                data.Remark = model.Remark;
                data.AcceptanceResult = model.AcceptanceResult;

                data = SetCurrentUser(data);

                var oldAcceptanceDetail = DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get().GetList(p => p.SpecialCheckId == model.Id);

                DbContextTransaction transaction = context.Database.BeginTransaction();
                try
                {
                    DataOperateBusiness<Epm_SpecialAcceptance>.Get().Update(data);
                    if (model.AcceptanceDetailList.Any())
                    {
                        model.AcceptanceDetailList.ForEach(p => p.SpecialCheckId = data.Id);

                        if (oldAcceptanceDetail.Any())
                        {
                            DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get().DeleteRange(oldAcceptanceDetail);
                        }
                        DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get().AddRange(model.AcceptanceDetailList);
                    }
                    transaction.Commit();

                    if (model.AttachList != null)
                    {
                        //删除之前的附件
                        DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });

                        AddFilesByTable(data, model.AttachList);
                    }
                    result.Data = 1;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Special.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);

                    if ((int)ApprovalState.WaitAppr == model.State)
                    {
                        #region 生成待办
                        #endregion
                    }
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
                result.Exception = new ExceptionEx(ex, "UpdateSpecialAcceptance");
            }
            return result;
        }

        ///<summary>
        ///删除专项验收
        ///</summary>
        /// <param name="id">要删除的专项验收 ID</param>
        /// <returns>受影响的行数</returns>
        public Result<bool> DeleteSpecialAcceptanceById(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetModel(id);

                if (model == null)
                {
                    throw new Exception("要删除的专项验收不存在！");
                }
                if (model.State != (int)ApprovalState.Enabled && model.State != (int)ApprovalState.Discarded)
                {
                    throw new Exception("只要未提交审核或已废弃的专项验收才能进行删除操作！");
                }
                DbContextTransaction transaction = context.Database.BeginTransaction();
                try
                {

                    string tableName = model.GetType().Name;
                    var rows = DataOperateBusiness<Epm_SpecialAcceptance>.Get().Delete(id);
                    DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get().Delete(p => p.SpecialCheckId == id);

                    transaction.Commit();

                    DataOperateBasic<Base_Files>.Get().Delete(p => p.TableName == tableName && p.TableId == id);
                    result.Data = true;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Special.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSpecialAcceptanceById");
            }
            return result;
        }

        ///<summary>
        ///获取专项验收列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SpecialAcceptance>> GetSpecialAcceptanceList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_SpecialAcceptance>> result = new Result<List<Epm_SpecialAcceptance>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_SpecialAcceptance>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSpecialAcceptanceList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<SpecialAcceptanceView> GetSpecialAcceptanceModel(long id)
        {
            Result<SpecialAcceptanceView> result = new Result<SpecialAcceptanceView>();
            try
            {
                if (id <= 0)
                {
                    throw new Exception("请选择要查看的专项验收！");
                }

                var specialAcceptance = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetModel(id);

                SpecialAcceptanceView model = new SpecialAcceptanceView()
                {
                    Id = specialAcceptance.Id,
                    ProjectId = specialAcceptance.ProjectId,
                    ProjectName = specialAcceptance.ProjectName,
                    TypeId = specialAcceptance.TypeId,
                    TypeName = specialAcceptance.TypeName,
                    Title = specialAcceptance.Title,

                    Content = specialAcceptance.Content,
                    Num = specialAcceptance.Num,
                    RecCompanyId = specialAcceptance.RecCompanyId,
                    RecCompanyName = specialAcceptance.RecCompanyName,
                    RecUserId = specialAcceptance.RecUserId,
                    RecUserName = specialAcceptance.RecUserName,
                    RecTime = specialAcceptance.RecTime,
                    AcceptanceResult = specialAcceptance.AcceptanceResult,
                    State = specialAcceptance.State,
                    Remark = specialAcceptance.Remark,
                    CrtCompanyId = CurrentUser.CompanyId,
                    CrtCompanyName = CurrentCompanyName,
                    IsDelete = false
                };
                if (model != null)
                {
                    model.AcceptanceDetailList = DataOperateBusiness<Epm_SpecialAcceptanceDetails>.Get()
                        .GetList(p => p.SpecialCheckId == id).ToList();

                    string tableName = model.GetType().Name;
                    model.AttachList = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableId == id && p.TableName == tableName).ToList();
                }

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSpecialAcceptanceModel");
            }
            return result;
        }


        /// <summary>
        /// 专项验收审核
        /// </summary>
        /// <returns></returns>
        public Result<bool> AuditSpecialAccptance(SpecialAcceptanceView model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (model == null || model.Id <= 0)
                {
                    throw new Exception("请选择要审核的专项验收！");
                }
                var data = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetModel(model.Id);
                if (data == null)
                {
                    throw new Exception("要审核的专项验收不存在或已被删除！");
                }
                if ((int)ApprovalState.WaitAppr != data.State)
                {
                    throw new Exception("当前专项验收暂不能进行审核！");
                }
                if ((int)ApprovalState.ApprFailure == model.State && string.IsNullOrWhiteSpace(model.AuditRemark))
                {
                    throw new Exception("请填写审核不通过的具体原因！");
                }

                data.State = model.State;
                // todo: 将审核备注写入数据库
                if(model.State== (int)ApprovalState.ApprSuccess)
                {
                    data.AcceptanceResult = 1;
                }else if (model.State == (int)ApprovalState.ApprFailure)
                {
                    data.AcceptanceResult = 2;
                }
                DataOperateBusiness<Epm_SpecialAcceptance>.Get().Update(data);

                WriteLog(BusinessType.Special.GetText(), SystemRight.Check.GetText(), "修改: " + model.Id);

                Func<int, MessageStep> getMessageStep = delegate (int state)
                {
                    switch (model.State)
                    {
                        case (int)ApprovalState.ApprSuccess:
                            return MessageStep.SpecialAudit;
                        case (int)ApprovalState.ApprFailure:
                            return MessageStep.SpecialReject;
                        default:
                            return MessageStep.SpecialReject;
                    }
                };
                
                if (model.State == (int)ApprovalState.ApprFailure)
                {
                    #region 生成待办
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditSpecialAccptance");
                result.Data = false;
            }
            return result;
        }

        /// <summary>
        /// 废弃专项验收
        /// </summary>
        ///<param name="id">要废弃的专项验收 ID</param>
        /// <returns></returns>
        public Result<bool> DiscardSpecialAccptance(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (id <= 0)
                {
                    throw new Exception("请选择要废弃的专项验收！");
                }
                var data = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetModel(id);
                if (data == null)
                {
                    throw new Exception("要废弃的专项验收不存在或已被删除！");
                }
                if ((int)ApprovalState.Discarded == data.State)
                {
                    throw new Exception("当前专项验收已废弃，请勿重复操作！");
                }
                if ((int)ApprovalState.ApprFailure != data.State)
                {
                    throw new Exception("只有审核被驳回的专项验收才可以进行废弃操作！");
                }

                data.State = (int)ApprovalState.Discarded;
                DataOperateBusiness<Epm_SpecialAcceptance>.Get().Update(data);

                WriteLog(BusinessType.Special.GetText(), SystemRight.Check.GetText(), "废弃: " + data.Id);
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DiscardSpecialAccptance");
                result.Data = false;
            }
            return result;
        }
    }
}
