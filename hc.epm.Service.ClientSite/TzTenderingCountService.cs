using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 招标申请统计
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<TzTenderingCountView>> GetTzTenderingCountList(QueryCondition qc)
        {
            Result<List<TzTenderingCountView>> result = new Result<List<TzTenderingCountView>>();
            try
            {
                var query = from pp in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false)
                            select new
                            {
                                //pp.Id,
                                //pp.ProjectName,
                                //pp.NatureName,
                                //pp.StationName
                                pp
                            };

                var projectList = query.ToList().Select(c => new TzTenderingCountView()
                //query.ToList().OrderByDescending(p => p.pp.ProjectName).Skip(skip).Take(take).AsEnumerable().Select(c => new TzTenderingCountView()
                {
                    Id = c.pp.Id,
                    ProjectName = c.pp.ProjectName,
                    StationName = c.pp.StationName,
                    NatureName = c.pp.NatureName,
                    OperateTime=c.pp.OperateTime,
                    counts = GetCounts(c.pp.Id.ToString())
                }).ToList();
               

                if (qc != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "ProjectName":
                                    {
                                        projectList = projectList.Where(t => t.ProjectName.Contains(value)).ToList();
                                        break;
                                    }
                                case "counts":
                                    {
                                        projectList = projectList.Where(t => t.counts == Convert.ToInt32(value)).ToList();
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                //int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                //int take = qc.PageInfo.PageRowCount;
                //int total = projectList.Count();
                result.AllRowsCount = projectList.Count();
                projectList = projectList.OrderByDescending(t => t.OperateTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount).ToList();
                result.Data = projectList;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTenderingCountList");
            }
            return result;
        }
        /// <summary>
        /// 获取招标次数
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        private int GetCounts(string ProjectId)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzTenderingApply.Where(p => p.IsDelete == false)
                        select new
                        {
                            ta.ProjectId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.ProjectId != null)
                {
                    var strId = item.ProjectId.Split('、');
                    for (int i = 0; i < strId.Length; i++)
                    {
                        if (strId[i] == ProjectId)
                        {
                            ii++;
                        }
                    }
                }
            }
            return ii;

        }
    }
}
