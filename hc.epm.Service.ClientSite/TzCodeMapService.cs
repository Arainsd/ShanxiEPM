/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzCodeMapService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/21 14:52:17
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

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
        #region 编码映射

        /// <summary>
        /// 获取编码映射结果
        /// </summary>
        /// <param name="codeType">编码映射类型</param>
        /// <param name="mapType">映射系统</param>
        /// <param name="fromCode">源编码</param>
        /// <returns></returns>
        public Result<Epm_TzCodeMap> GetCodeMap(string codeType, string mapType, string fromCode)
        {
            Result<Epm_TzCodeMap> result = new Result<Epm_TzCodeMap>();
            try
            {
                if(string.IsNullOrWhiteSpace(codeType) || string.IsNullOrWhiteSpace(mapType) || string.IsNullOrWhiteSpace(fromCode))
                {
                    throw new Exception("");
                }
                var model = DataOperateBusiness<Epm_TzCodeMap>.Get().Single(p => !p.IsDelete && p.CodeType == codeType && p.MapType == mapType && p.From_Code == fromCode);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCodeMap");
            }
            return result;
        }

        #endregion
    }
}
