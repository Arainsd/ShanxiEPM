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
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///获取列表:加油站信息表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_OilStation>> result = new Result<List<Epm_OilStation>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_OilStation>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetOilStationList");
            }
            return result;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Epm_OilStation>> GetOilStationAllList()
        {
            Result<List<Epm_OilStation>> result = new Result<List<Epm_OilStation>>();
            try
            {
                var model = DataOperateBusiness<Epm_OilStation>.Get().GetList(t => t.IsDelete == false).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetOilStationModel");
            }
            return result;
        }
    }
}
