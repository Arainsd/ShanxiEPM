using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzSupMatManagement(Epm_TzSupMatManagement model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCreateUser(model);
                SetCurrentUser(model);
                //新增物资默认状态0（0：已启用，1：已停用）
                model.State = 0;

                var isexist = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetList(t => !t.IsDelete && t.State == 0 && t.SupplierId == model.SupplierId && t.MaterialCategory == model.MaterialCategory && t.ProductName == model.ProductName && t.Specification == model.Specification);
                if (isexist.Any())
                {
                    throw new Exception("此供应商已存在相同种类、品名和规格的物资！");
                }
                else
                {
                    var rows = DataOperateBusiness<Epm_TzSupMatManagement>.Get().Add(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                }
                //WriteLog(AdminModule.TzSupMatManagement.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzSupMatManagement");
            }
            return result;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRangeTzSupMatManagement(List<Epm_TzSupMatManagement> models)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (models != null)
                {
                    foreach (var item in models)
                    {
                        SetCreateUser(item);
                        SetCurrentUser(item);
                        //新增物资默认状态0（0：已启用，1：已停用）
                        item.State = 0;
                    }
                }
                var rows = DataOperateBusiness<Epm_TzSupMatManagement>.Get().AddRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupMatManagement.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRangeTzSupMatManagement");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzSupMatManagement(Epm_TzSupMatManagement model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);
                var rows = DataOperateBusiness<Epm_TzSupMatManagement>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupMatManagement.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSupMatManagement");
            }
            return result;
        }
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="ids">数据集合</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public Result<int> UpdateTzSupMatManagementState(List<long> ids, int state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var item in models)
                {
                    item.State = state;
                    SetCurrentUser(item);
                }
                var rows = DataOperateBusiness<Epm_TzSupMatManagement>.Get().UpdateRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupMatManagement.GetText(), SystemRight.Delete.GetText(), "批量更新: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSupMatManagementState");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzSupMatManagementByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzSupMatManagement>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupMatManagement.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzSupMatManagementByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSupMatManagement>> result = new Result<List<Epm_TzSupMatManagement>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSupMatManagement>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupMatManagementList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzSupMatManagement> GetTzSupMatManagementModel(long id)
        {
            Result<Epm_TzSupMatManagement> result = new Result<Epm_TzSupMatManagement>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupMatManagementModel");
            }
            return result;
        }

        /// <summary>
        /// 获取已启用的甲供物资申请数据
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <returns></returns>
        public Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementListBy(long SupplierId, string name, string productName)
        {
            Result<List<Epm_TzSupMatManagement>> result = new Result<List<Epm_TzSupMatManagement>>();
            try
            {
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(productName))
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
                else {
                    var model = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetList(t => !t.IsDelete && t.State == 0 && t.SupplierId == SupplierId
                                && (string.IsNullOrEmpty(name) || t.MaterialCategory == name)
                                 && (string.IsNullOrEmpty(productName) || t.ProductName == productName)
                                ).ToList();
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupMatManagementListBy");
            }
            return result;
        }

        /// <summary>
        /// 根据物资种类、品名、规格获取物资管理详细信息
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <param name="specification">规格</param>
        /// <returns></returns>
        public Result<Epm_TzSupMatManagement> GetTzSupMatManagementModelBy(long SupplierId, string name, string productName, string specification)
        {
            Result<Epm_TzSupMatManagement> result = new Result<Epm_TzSupMatManagement>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSupMatManagement>.Get().GetList(t => !t.IsDelete && t.State == 0 && t.SupplierId == SupplierId && t.MaterialCategory == name && t.ProductName == productName && t.Specification == specification).FirstOrDefault();
                if (model != null)
                {
                    result.Data = model;
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
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupMatManagementModelBy");
            }
            return result;
        }
    }
}
