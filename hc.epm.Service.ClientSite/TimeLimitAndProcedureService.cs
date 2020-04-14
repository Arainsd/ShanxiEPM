using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
     partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
               // WriteLog(AdminModule.TimeLimitAndCrossings.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTimeLimitAndCrossings");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model)
        {
            Result<int> result = new Result<int>();
            try
            {

                int rows = 0;
                bool isAdd = false;
                var TimeLimitAndCrossings = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();//判断是否存在

                if (TimeLimitAndCrossings == null)
                {
                    isAdd = true;
                    TimeLimitAndCrossings = new Epm_TimeLimitAndProcedure();
                    //当前创建人
                    SetCreateUser(TimeLimitAndCrossings);
                }
                TimeLimitAndCrossings.ProjectId = model.ProjectId;//项目id
                TimeLimitAndCrossings.IsCrossings = model.IsCrossings;//
                TimeLimitAndCrossings.ShutdownTime = model.ShutdownTime;//
                TimeLimitAndCrossings.PlanWorkStartTime = model.PlanWorkStartTime;//
                TimeLimitAndCrossings.PlanWorkEndTime = model.PlanWorkEndTime;//
                TimeLimitAndCrossings.TimeLimit = model.TimeLimit;//
                TimeLimitAndCrossings.PlanOpeningTime = model.PlanOpeningTime;//
                TimeLimitAndCrossings.PlanShutdowLimit = model.PlanShutdowLimit;//

                if (model.typeSub == 2)//提交审核，如果是提交审核，要修改开工申请的状态为已提交
                {
                    var ProjectStart = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Single(p => p.ProjectId == model.ProjectId);
                    if (ProjectStart != null)
                    {
                        ProjectStart.State = (int)PreProjectState.Submitted;//修改状态为已提交

                        rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Update(ProjectStart);
                    }

                }
                //当前操作人
                SetCurrentUser(TimeLimitAndCrossings);

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Add(TimeLimitAndCrossings);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Update(TimeLimitAndCrossings);
                }

                //上传附件
                AddFilesBytzTable(TimeLimitAndCrossings, model.TzAttachs);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzConDrawing.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTimeLimitAndCrossings");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTimeLimitAndCrossingsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
               // WriteLog(AdminModule.TimeLimitAndCrossings.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTimeLimitAndCrossingsByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TimeLimitAndProcedure>> GetTimeLimitAndCrossingsList(QueryCondition qc)
        {
            //qc = AddDefault(qc);
            Result<List<Epm_TimeLimitAndProcedure>> result = new Result<List<Epm_TimeLimitAndProcedure>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TimeLimitAndProcedure>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTimeLimitAndCrossingsList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndCrossingsModel(long id)
        {
            Result<Epm_TimeLimitAndProcedure> result = new Result<Epm_TimeLimitAndProcedure>();
            try
            {
                var model = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTimeLimitAndCrossingsModel");
            }
            return result;
        }

    }
}
