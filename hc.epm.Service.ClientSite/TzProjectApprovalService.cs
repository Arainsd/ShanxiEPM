/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzProjectApprovalService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/23 16:50:56
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.DataModel.Business;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {///<summary>
     ///添加:
     ///</summary>
     /// <param name="model">要添加的model</param>
     /// <returns>受影响的行数</returns>
        public Result<int> AddTzProjectApproval(Epm_TzProjectApproval model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzProjectApproval>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApproval.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzProjectApproval");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectApproval(Epm_TzProjectApproval model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzProjectApproval>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApproval.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectApproval");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectApprovalByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectApproval>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectApproval>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApproval.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzProjectApprovalByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzProjectApproval>> result = new Result<List<Epm_TzProjectApproval>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzProjectApproval>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzProjectApproval> GetTzProjectApprovalModel(long id)
        {
            Result<Epm_TzProjectApproval> result = new Result<Epm_TzProjectApproval>();
            try
            {
                var model = DataOperateBusiness<Epm_TzProjectApproval>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzSecondTakl(Epm_TzSecondTakl model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzSecondTakl>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTakl.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzSecondTakl");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzSecondTakl(Epm_TzSecondTakl model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzSecondTakl>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTakl.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSecondTakl");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzSecondTaklByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSecondTakl>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzSecondTakl>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTakl.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzSecondTaklByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSecondTakl>> GetTzSecondTaklList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSecondTakl>> result = new Result<List<Epm_TzSecondTakl>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSecondTakl>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSecondTaklList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzSecondTakl> GetTzSecondTaklModel(long id)
        {
            Result<Epm_TzSecondTakl> result = new Result<Epm_TzSecondTakl>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSecondTakl>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSecondTaklModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzSecondTalkAudit(Epm_TzSecondTalkAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTalkAudit.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzSecondTalkAudit");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzSecondTalkAudit(Epm_TzSecondTalkAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTalkAudit.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSecondTalkAudit");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzSecondTalkAuditByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSecondTalkAudit.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzSecondTalkAuditByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSecondTalkAudit>> GetTzSecondTalkAuditList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSecondTalkAudit>> result = new Result<List<Epm_TzSecondTalkAudit>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSecondTalkAudit>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSecondTalkAuditList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzSecondTalkAudit> GetTzSecondTalkAuditModel(long id)
        {
            Result<Epm_TzSecondTalkAudit> result = new Result<Epm_TzSecondTalkAudit>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSecondTalkAuditModel");
            }
            return result;
        }

    }
}
