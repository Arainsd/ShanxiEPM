using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 招标申请统计
    /// </summary>
    public class TzTenderingCountController : BaseWebController
    {
        // GET: TzTenderingCount
        /// <summary>
        /// 招标申请统计列表
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="count">联系人</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string projectName = "",string count="", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.count = count;
            Result<List<TzTenderingCountView>> result = new Result<List<TzTenderingCountView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = projectName.Trim();
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(count))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "counts";
                    ce.ExpValue =Convert.ToInt32(count);
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                //qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                //根据字典类型集合获取字典数据
                //List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TenderingFileType, DictionaryType.BiddingMethod };
                //var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzTenderingCountList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                
                ////招标方式
                //ViewBag.BidType = subjects[DictionaryType.BiddingMethod].ToList().ToSelectList("Name", "No", true);
                ////审批状态
                //ViewBag.State = typeof(PreProjectState).AsSelectList(true);
            }

            return View(result.Data);
        }

    }
}