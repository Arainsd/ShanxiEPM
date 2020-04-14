using hc.epm.Common;
using hc.epm.DataModel.Basic;
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
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> GetRegionList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Region>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRegionList");
            }
            return result;
        }
        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Base_Region> GetRegionModel(string code)
        {

            Result<Base_Region> result = new Result<Base_Region>();
            try
            {
                var model = DataOperateBasic<Base_Region>.Get().Single(i => i.RegionCode == code);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRegionModel");
            }
            return result;
        }
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> LoadRegionList(string parentCode = "")
        {
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            try
            {
                List<Base_Region> list = new List<Base_Region>();
                if (!string.IsNullOrEmpty(parentCode))
                {
                    list = DataOperateBasic<Base_Region>.Get().GetList(i => i.ParentCode == parentCode).ToList();
                }
                else
                {
                    list = DataOperateBasic<Base_Region>.Get().GetList().ToList();
                }

                result.Data = list.OrderBy(i => i.Id).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadRegionList");
            }
            return result;
        }

        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types)
        {
            Dictionary<DictionaryType, List<Base_TypeDictionary>> dictionary = new Dictionary<DictionaryType, List<Base_TypeDictionary>>();
            Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> result = new Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>>();
            try
            {
                var strTypes = types.Select(i => i.ToString());
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => strTypes.Contains(i.Type)).ToList();
                list = list.OrderBy(t => t.Id).ToList();
                dictionary = list.GroupBy(i => i.Type).ToDictionary(i => i.Key.ToEnumReq<DictionaryType>(), j => j.ToList());
                result.Data = dictionary;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeListByType");
            }
            return result;
        }
    }
}
