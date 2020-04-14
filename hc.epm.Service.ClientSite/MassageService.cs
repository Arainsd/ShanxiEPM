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
using Newtonsoft.Json;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddMassage(Epm_Massage model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_Massage>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(WebModule.Massage.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMassage");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateMassage(Epm_Massage model)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Massage oldmodel = DataOperateBusiness<Epm_Massage>.Get().GetModel(model.Id);
                oldmodel.OperateUserId = CurrentUserID.ToLongReq();
                oldmodel.OperateUserName = CurrentUserName;
                oldmodel.OperateTime = DateTime.Now;
                oldmodel.IsRead = model.IsRead;
                oldmodel.ReadTime = model.ReadTime;

                var rows = DataOperateBusiness<Epm_Massage>.Get().Update(oldmodel);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMassage");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteMassageByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Massage>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Massage>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(WebModule.Massage.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMassageByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Massage>> GetMassageList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Massage>> result = new Result<List<Epm_Massage>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Massage>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMassageList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Massage> GetMassageModel(long id)
        {
            Result<Epm_Massage> result = new Result<Epm_Massage>();
            try
            {
                var model = DataOperateBusiness<Epm_Massage>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMassageModel");
            }
            return result;
        }

        /// <summary>
        /// 更新所有消息状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateAllMassageState(long recId,bool state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var list = DataOperateBusiness<Epm_Massage>.Get().GetList(p => p.RecId == recId && p.IsRead != state).ToList();
                foreach (var item in list)
                {
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.IsRead = state;
                    item.ReadTime = DateTime.Now;
                }

                var rows = DataOperateBusiness<Epm_Massage>.Get().UpdateRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateAllMassageState");
            }
            return result;
        }

    }
}
