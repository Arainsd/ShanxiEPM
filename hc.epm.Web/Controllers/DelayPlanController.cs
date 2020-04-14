using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
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
    public class DelayPlanController : BaseWebController
    {
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Browse)]
        public ActionResult Index(string planName = "", string state = "", string projectName = "", string applyCompanyName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "监理日志列表";
            ViewBag.planName = planName;
            ViewBag.projectName = projectName;
            ViewBag.applyCompanyName = applyCompanyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.state = state;
            QueryCondition qc = new QueryCondition()
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            ConditionExpression ce = null;

            if (!string.IsNullOrEmpty(planName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "PlanName";
                ce.ExpValue = "%" + planName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + projectName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(applyCompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ApplyCompanyName";
                ce.ExpValue = "%" + applyCompanyName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(state))
            {
                int statu = (int)Enum.Parse(typeof(ApprovalState), state);
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            Result<List<Epm_PlanDelay>> result = new Result<List<Epm_PlanDelay>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanDelayList(qc);

                //long projectId = result.Data[0].ProjectId;
                //bool IsSupervisor = proxy.IsSupervisor(projectId, CurrentUser.UserId);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                ViewBag.UserID = ApplicationContext.Current.UserID;
                ViewBag.UserName = ApplicationContext.Current.UserName;
                ViewBag.State = typeof(ApprovalState).AsSelectList(true, state, new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });

                //ViewBag.IsSupervisor = IsSupervisor;
            }
            return View(result.Data);
        }
        /// <summary>
        /// 新增延期申请
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Add)]
        [HttpGet]
        public ActionResult Add(long projectId = 0, string projectName = "", long PlanId = 0)
        {
            ViewBag.Title = "新增监理日志";
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            if (projectId != 0)
            {
                ViewBag.ProjectId = projectId;
                ViewBag.ProjectName = projectName;
            }
            ViewBag.PlanId = new List<Epm_Plan>().ToSelectList("Name", "Id", true, PlanId.ToString());
            ViewBag.CompanyId = new List<Epm_ProjectCompany>().ToSelectList("CompanyName", "CompanyId", true);

            if (ViewBag.ProjectId != null && !string.IsNullOrEmpty(ViewBag.ProjectId.ToString()))
            {
                projectId = Convert.ToInt64(ViewBag.ProjectId.ToString());
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                // 施工计划
                var planResult = proxy.GetPlanList(qc);      //获取施工计划表中的所有数据
                if (planResult.Flag == EResultFlag.Success && planResult.Data != null && planResult.Data.Any())
                {
                    ViewBag.PlanId = planResult.Data.OrderBy(t => t.StartTime).ToList().ToSelectList("Name", "Id", true);    // 获取项目对应的里程碑
                }
                // 施工单位
                var companyResult = proxy.GetProjectCompanyList(projectId);
                if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null && companyResult.Data.Any())
                {
                    companyResult.Data = companyResult.Data.Where(t => t.CompanyId.HasValue).ToList();
                    var comList = companyResult.Data.Select(p => new
                    {
                        CompanyId = p.CompanyId,
                        CompanyName = p.CompanyName
                    }).Distinct().ToList();
                    ViewBag.CompanyId = comList.ToSelectList("CompanyName", "CompanyId", true);
                }
            }
            return View();
        }


        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Add)]
        [HttpPost]        /// <summary>
                          /// 根据里程碑ID获取开始时间和结束时间
                          /// </summary>
                          /// <param name="PlanId"></param>
                          /// <returns></returns>
        public ActionResult GetPlanTime(long PlanId)
        {
            Result<PlanView> result = new Result<PlanView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanModel(PlanId);
            }
            return Json(result.Data);
        }

        /// <summary>
        /// 新增延期申请（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(PlanDelayView model)
        {
            ResultView<int> view = new ResultView<int>();

            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "请选择延期申请所属项目！";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.PlanName))
            {
                view.Flag = false;
                view.Message = "请选择延期工程节点！";
                return Json(view);
            }

            ////施工单位
            //string builder = Request.Form["Builder"];
            //if (string.IsNullOrWhiteSpace(builder))
            //{
            //    view.Flag = false;
            //    view.Message = "请填写相关责任单位情况！";
            //    return Json(view);
            //}
            //model.PlanDelayCompanys = JsonConvert.DeserializeObject<List<Epm_PlanDelayCompany>>(builder);

            var companyIds = Request.Form["CompanyId"];
            var companyNames = Request.Form["CompanyName"];
            for (int i = 0; i < companyIds.Split('、').Length; i++)
            {
                var planDelayview = new Epm_PlanDelayCompany();
                planDelayview.ProjectId = model.ProjectId;
                planDelayview.ProjectName = model.ProjectName;
                planDelayview.CompanyId = Convert.ToInt64(companyIds.Split('、')[i]);
                planDelayview.CompanyName = companyNames.Split('、')[i];
                planDelayview.DelayDay = model.DelayDay;
                planDelayview.State = model.State;
                planDelayview.DelayId = model.Id;
                model.PlanDelayCompanys.Add(planDelayview);
            }
            if (model.PlanDelayCompanys == null || !model.PlanDelayCompanys.Any())
            {
                view.Flag = false;
                view.Message = "请填写相关责任单位情况！";
                return Json(view);
            }

            Result<bool> result = new Result<bool>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                result = proxy.AddPlanDelay(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 延期申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            ViewBag.Title = "查看延期申请";

            Result<PlanDelayView> result = new Result<PlanDelayView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanDelayInfo(id);
                return View(result.Data);
            }
        }
        /// <summary>
        /// 修改延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Modify)]
        [HttpGet]
        public ActionResult Edit(long id)
        {
            ViewBag.Title = "修改延期申请";

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                Result<Epm_PlanDelayCompany> company = new Result<Epm_PlanDelayCompany>();
                QueryCondition qc = new QueryCondition();

                var planDelay = proxy.GetPlanDelayInfo(id);
                ViewBag.ProjectId = planDelay.Data.ProjectId;
                ViewBag.ProjectName = planDelay.Data.ProjectName;
                ViewBag.PlanId = new List<Epm_Plan>().ToSelectList("Name", "Id", true);
                ViewBag.CompanyId = new List<Epm_ProjectCompany>().ToSelectList("CompanyName", "CompanyId", true);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = planDelay.Data.ProjectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                if (planDelay.Flag == EResultFlag.Success && planDelay.Data != null)
                {
                    var planResult = proxy.GetPlanList(qc);      //获取施工计划表中的所有数据
                    if (planResult.Flag == EResultFlag.Success && planResult.Data != null && planResult.Data.Any())
                    {
                        ViewBag.PlanId = planResult.Data.OrderBy(t => t.StartTime).ToList().ToSelectList("Name", "Id", false, planDelay.Data.PlanName);    // 获取项目对应的里程碑                        
                    }
                    // 施工单位
                    var companyResult = proxy.GetProjectCompanyList(planDelay.Data.ProjectId);
                    if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null && companyResult.Data.Any())
                    {
                        companyResult.Data = companyResult.Data.Where(t => t.CompanyId.HasValue).ToList();
                        ViewBag.CompanyId = companyResult.Data.ToSelectList("CompanyName", "CompanyId", true);
                    }
                    return View(planDelay.Data);
                }
                else
                {
                    return View();
                }
            }
        }
        /// <summary>
        /// 修改延期申请（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(PlanDelayView model)
        {
            ResultView<int> view = new ResultView<int>();

            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "请选择延期申请所属项目！";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.PlanName))
            {
                view.Flag = false;
                view.Message = "请选择延期工程节点！";
                return Json(view);
            }

            //施工单位
            //string builder = Request.Form["Builder"];
            //if (string.IsNullOrWhiteSpace(builder))
            //{
            //    view.Flag = false;
            //    view.Message = "请填写相关责任单位情况！";
            //    return Json(view);
            //}
            //model.PlanDelayCompanys = JsonConvert.DeserializeObject<List<Epm_PlanDelayCompany>>(builder);
            var companyIds = Request.Form["CompanyId"];
            var companyNames = Request.Form["CompanyName"];
            for (int i = 0; i < companyIds.Split('、').Length; i++)
            {
                var planDelayview = new Epm_PlanDelayCompany();
                planDelayview.ProjectId = model.ProjectId;
                planDelayview.ProjectName = model.ProjectName;
                planDelayview.CompanyId = Convert.ToInt64(companyIds.Split('、')[i]);
                planDelayview.CompanyName = companyNames.Split('、')[i];
                planDelayview.DelayDay = model.DelayDay;
                planDelayview.State = model.State;
                planDelayview.DelayId = model.Id;
                model.PlanDelayCompanys.Add(planDelayview);
            }

            if (model.PlanDelayCompanys == null || !model.PlanDelayCompanys.Any())
            {
                view.Flag = false;
                view.Message = "请填写现场施工单位情况！";
                return Json(view);
            }

            Result<bool> result = new Result<bool>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                result = proxy.ModifyPlanDelay(model);

            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<bool> result = new Result<bool>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeletePlanDelay(id);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Check)]
        [HttpPost]
        public ActionResult Audit(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_PlanDelay model = new Epm_PlanDelay();
                model.Id = id;
                model.State = (int)ApprovalState.ApprSuccess;
                result = proxy.AuditPlanDelay(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.UnCheck)]
        [HttpPost]
        public ActionResult Reject(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_PlanDelay model = new Epm_PlanDelay();
                model.Id = id;
                model.State = (int)ApprovalState.ApprFailure;
                result = proxy.AuditPlanDelay(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DelayApply, Right = SystemRight.Invalid)]
        [HttpPost]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_PlanDelay model = new Epm_PlanDelay();
                model.Id = id;
                model.State = (int)ApprovalState.Discarded;
                result = proxy.AuditPlanDelay(model);
            }
            return Json(result.ToResultView());
        }
    }
}