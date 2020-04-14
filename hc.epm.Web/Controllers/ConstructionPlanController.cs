using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
//using Microsoft.Office.Interop.MSProject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class ConstructionPlanController : BaseWebController
    {
        #region 计划
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Browse)]
        public ActionResult Index(long ProjectId = 0, string ProjectName = "")
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            if (ProjectId != 0)
            {
                ViewBag.ProjectId = ProjectId;
                ViewBag.ProjectName = ProjectName;
            }

            Result<List<PlanView>> result = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (ViewBag.ProjectId != null && !string.IsNullOrEmpty(ViewBag.ProjectId.ToString()))
                {
                    long id = Convert.ToInt64(ViewBag.ProjectId.ToString());
                    result = proxy.GetMilepostPlan(id);
                }
            }
            return View(result.Data);
        }
        /// <summary>
        /// 施工计划添加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(long projectId = 0)
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            ViewBag.parentId = "";
            Result<List<PlanView>> result = new Result<List<PlanView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (projectId != 0)
                {
                    result = proxy.GetMilepostPlan(projectId);
                }
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                ce = new ConditionExpression();
                ce.ExpName = "ParentId";
                ce.ExpValue = 0;
                ce.ExpOperater = eConditionOperator.Equal;
                qc.ConditionList.Add(ce);
                qc.PageInfo.isAllowPage = false;
                Result<List<Epm_Milepost>> data = proxy.GetMilepostList(qc);
                ViewBag.parentId = data.Data.ToSelectList("Name", "Id", true);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 生成施工计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConstructionPlan(long projectId, DateTime planStart, long mileType)
        {
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.CreateMilepostPlan(projectId, planStart, mileType, 2);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 保存施工计划
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePlan(Epm_Plan model)
        {
            string ConstructionPlans = Request.Form["ConstructionPlan"];
            List<Epm_Plan> list = JsonConvert.DeserializeObject<List<Epm_Plan>>(ConstructionPlans);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepostPlan(list, 1);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 提交施工计划
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitPlan(Epm_Plan model)
        {
            string ConstructionPlans = Request.Form["ConstructionPlan"];
            List<Epm_Plan> list = JsonConvert.DeserializeObject<List<Epm_Plan>>(ConstructionPlans);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepostPlan(list, 2);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 查看施工计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult Detail(long projectId)
        {
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostPlan(projectId);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 编辑施工计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult Edit(long projectId)
        {
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostPlan(projectId);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 编辑保存施工计划
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePlanEdit(Epm_Plan model)
        {
            string ConstructionPlans = Request.Form["ConstructionPlan"];
            List<Epm_Plan> list = JsonConvert.DeserializeObject<List<Epm_Plan>>(ConstructionPlans);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepostPlan(list, 1);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 编辑提交施工计划
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitPlanEdit(Epm_Plan model)
        {
            string ConstructionPlans = Request.Form["ConstructionPlan"];
            List<Epm_Plan> list = JsonConvert.DeserializeObject<List<Epm_Plan>>(ConstructionPlans);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepostPlan(list, 2);
            }
            return Json(result.ToResultView());
        }
        #endregion
        #region 进度
        public ActionResult ScheduleIndex(long ProjectId = 0, string ProjectName = "")
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            if (ProjectId != 0)
            {
                ViewBag.ProjectId = ProjectId;
                ViewBag.ProjectName = ProjectName;
            }
            Result<List<PlanView>> planViewList = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (ViewBag.ProjectId != null && !string.IsNullOrEmpty(ViewBag.ProjectId.ToString()))
                {
                    long id = Convert.ToInt64(ViewBag.ProjectId.ToString());
                    planViewList = proxy.GetPlanViewList(id);
                }
            }
            //施工计划树形列表
            ViewBag.planViewList = JsonConvert.SerializeObject(planViewList.Data);

            return View();
        }
        //生成甘特图
        [HttpGet]
        public ActionResult GetGantt(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetProjectGantt(id);
                return Json(result.ToResultView(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
