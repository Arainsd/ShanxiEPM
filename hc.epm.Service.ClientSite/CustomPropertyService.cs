using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
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
        public Result<int> AddCustomProperty(EPM_CustomProperty model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = base.SetCurrentUser(model);
                var rows = DataOperateBusiness<EPM_CustomProperty>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(WebModule.CustomProperty.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCustomProperty");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCustomProperty(EPM_CustomProperty model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<EPM_CustomProperty>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(WebModule.CustomProperty.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCustomProperty");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteCustomPropertyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<EPM_CustomProperty>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<EPM_CustomProperty>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
               //WriteLog(WebModule.CustomProperty.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCustomPropertyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<EPM_CustomProperty>> GetCustomPropertyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<EPM_CustomProperty>> result = new Result<List<EPM_CustomProperty>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<EPM_CustomProperty>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCustomPropertyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<EPM_CustomProperty> GetCustomPropertyModel(long id)
        {
            Result<EPM_CustomProperty> result = new Result<EPM_CustomProperty>();
            try
            {
                var model = DataOperateBusiness<EPM_CustomProperty>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCustomPropertyModel");
            }
            return result;
        }

    }
}
