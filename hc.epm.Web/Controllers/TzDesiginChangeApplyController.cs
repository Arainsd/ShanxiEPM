using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.epm.Web.Models;
using hc.Plat.Common.Extend;
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
    /// 建设工程设计变更申请
    /// </summary>
    public class TzDesiginChangeApplyController : BaseWebController
    {
        // GET: TzDesiginChangeApply
        /// <summary>
        /// 查询建设工程设计变更列表
        /// </summary>
        /// <param name="projectName">工程名称</param>
        /// <param name="constructionUnit">建设单位</param>
        /// <param name="workUnit">施工单位</param>
        /// <param name="supervisionUnit">监理单位</param>
        /// <param name="designUnit">设计单位</param>
        /// <param name="startTime">申请日期</param>
        /// <param name="endTime">申请日期</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string projectName = "", string constructionUnit = "", string workUnit = "", string supervisionUnit = "", string designUnit = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.constructionUnit = constructionUnit;
            ViewBag.workUnit = workUnit;
            ViewBag.supervisionUnit = supervisionUnit;
            ViewBag.designUnit = designUnit;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            Result<List<Epm_TzDesiginChangeApply>> result = new Result<List<Epm_TzDesiginChangeApply>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //ViewBag.IsAgencyUser = false;
                //var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                //if (companyInfo != null)
                //{
                //    //是省公司
                //    if (companyInfo.OrgType == "1")
                //    {
                //        companyId = "";
                //    }
                //    else if (companyInfo.OrgType == "2")
                //    {
                //        companyId = CurrentUser.CompanyId.ToString();
                //        ViewBag.CompanyName = CurrentUser.CompanyName;
                //    }
                //    ViewBag.IsAgencyUser = companyInfo.OrgType == "1" ? true : false;
                //}

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = "%" + projectName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(constructionUnit))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ConstructionUnit";
                    ce.ExpValue = "%" + constructionUnit + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(workUnit))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "WorkUnit";
                    ce.ExpValue = "%" + workUnit + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(supervisionUnit))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "SupervisionUnit";
                    ce.ExpValue = "%" + supervisionUnit + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(designUnit))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "DesignUnit";
                    ce.ExpValue = "%" + designUnit + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ApplyDate",
                        ExpValue = stime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    DateTime etime = Convert.ToDateTime(endTime + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "ApplyDate",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzDesiginChangeApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 建设工程设计变更添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.Applicant = CurrentUser.RealName;
                ViewBag.ApplicantID = CurrentUser.UserId;
                ViewBag.Unit = CurrentUser.CompanyName;
                ViewBag.UnitID = CurrentUser.CompanyId;
                ViewBag.ApplyDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.DepartmentID = "";
                ViewBag.Department = "";
                //获取用户信息
                var userInfo = proxy.GetUserModel(CurrentUser.UserId);
                if (userInfo.Data != null)
                {
                    if (userInfo.Data.DepartmentId != null)
                    {
                        long dempId = userInfo.Data.DepartmentId.Value;
                        if (dempId != 0)
                        {
                            var companyInfo = proxy.GetCompanyModel(dempId);

                            if (companyInfo.Data != null)
                            {
                                ViewBag.DepartmentID = companyInfo.Data.Id;
                                ViewBag.Department = companyInfo.Data.Name;
                            }
                        }
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 建设工程设计变更添加方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Epm_TzDesiginChangeApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            model.Title = model.Title;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzDesiginChangeApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 建设工程设计变更修改页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzDesiginChangeApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 建设工程设计变更修改方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Epm_TzDesiginChangeApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzDesiginChangeApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看建设工程设计变更详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzDesiginChangeApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateState(string ids, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(ids))
            {
                view.Flag = false;
                view.Message = "请选择要操作的数据";
                return Json(view);
            }
            if (string.IsNullOrEmpty(state))
            {
                view.Flag = false;
                view.Message = "状态不能为空";
                return Json(view);
            }
            List<long> idList = ids.SplitString(",").ToLongList();

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzDesiginChangeApplyState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}