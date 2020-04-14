using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Business;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.BaseCore;
using hc.Plat.Common.Service;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:新闻、资讯表
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddNews(Epm_News model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_News>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.News.GetText(), SystemRight.Add.GetText(), "新增新闻、资讯表: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddNews");
            }
            return result;
        }
        ///<summary>
        ///修改:新闻、资讯表
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateNews(Epm_News model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_News>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.News.GetText(), SystemRight.Modify.GetText(), "修改新闻、资讯表: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateNews");
            }
            return result;
        }
        ///<summary>
        ///删除:新闻、资讯表
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteNewsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_News>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_News>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.News.GetText(), SystemRight.Delete.GetText(), "批量删除新闻、资讯表: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteNewsByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:新闻、资讯表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_News>> GetNewsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_News>> result = new Result<List<Epm_News>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_News>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewsList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:新闻、资讯表
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_News> GetNewsModel(long id)
        {
            Result<Epm_News> result = new Result<Epm_News>();
            try
            {
                var model = DataOperateBusiness<Epm_News>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewsModel");
            }
            return result;
        }
    }
}
