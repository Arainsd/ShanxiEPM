using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class VideoManageController : BaseWebController
    {
        /// <summary>
        /// 视频管理列表
        /// </summary>
        /// <param name="companyName">分公司名称</param>
        /// <param name="count">页数</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(long? companyId = 0, string count = "", int pageIndex = 1, int pageSize = 6)
        {
            //ViewBag.companyName = companyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.count = count;
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (companyId != 0)
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Companyid";
                    ce.ExpValue = companyId;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                ce = new ConditionExpression();
                ce.ExpName = "CameraState";
                ce.ExpValue = "1";
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);

                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion
                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);
                ViewBag.Date = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                result = proxy.GetBaseVideoManageLists(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

            }

            return View(result.Data);
        }
        public ActionResult VideoFailure(long? companyId = 0, string count = "", int pageIndex = 1, int pageSize = 10)
        {
            //ViewBag.companyName = companyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.count = count;
            Result<List<ProjectView>> result = new Result<List<ProjectView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (companyId != 0)
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = companyId;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }

                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion
                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);
                ViewBag.Date = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                result = proxy.GetProjectListInfo(pageIndex, pageSize, "5", "", "", "", "");
                var proinfo = proxy.GetBaseVideoManageLists(qc);
                proinfo.Data = proinfo.Data.Where(p => p.CameraState == "1").ToList();
                if (proinfo.Data.Count > 0)
                {
                    result.Data = result.Data.Where(p => p.CompanyId == companyId && p.Id != proinfo.Data.FirstOrDefault().Projectid).ToList();
                }
                else
                {
                    result.Data = result.Data.Where(p => p.CompanyId == companyId).ToList();
                }
                ViewBag.Total = result.Data.Count;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.Data.Count / pageSize);

            }

            return View(result.Data);
        }
    }
}