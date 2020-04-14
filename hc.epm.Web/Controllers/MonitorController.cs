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
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class MonitorController : BaseWebController
    {
        /// <summary>
        /// 安全检查列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="title"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Browse)]
        public ActionResult IndexAQ(string projectName = "", string title = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.title = title;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;


            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();

            if (!string.IsNullOrEmpty(title))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + title + "%";
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

            if (!string.IsNullOrEmpty(state))
            {
                int statu = (int)Enum.Parse(typeof(RectificationState), state);
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_Monitor>> result = new Result<List<Epm_Monitor>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //检查状态下拉数据
                ViewBag.CheckState = typeof(RectificationState).AsSelectList(true);
            }

            ViewBag.CurCompanyId = CurrentUser.CompanyId;

            return View(result.Data);
        }

        /// <summary>
        /// 检查列表（新）
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string gainLossCompanyName = "", string gainLossUserName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.projectName = projectName;
            ViewBag.gainLossCompanyName = gainLossCompanyName;
            ViewBag.gainLossUserName = gainLossUserName;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = projectName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(gainLossCompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "GainLossCompanyName";
                ce.ExpValue = gainLossCompanyName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(gainLossUserName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "GainLossUserName";
                ce.ExpValue = gainLossUserName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<InspectView>> result = new Result<List<InspectView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetInspectList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }


        /// <summary>
        /// 检查明细列表
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Browse)]
        public ActionResult IndexItem(string inspectItemId = "", long gainLossCompanyId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();
            if (!string.IsNullOrEmpty(inspectItemId))
            {
                ce = new ConditionExpression();
                ce.ExpName = "InspectItemId";
                ce.ExpValue = inspectItemId;
                ce.ExpOperater = eConditionOperator.In;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (gainLossCompanyId > 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "GainLossCompanyId";
                ce.ExpValue = gainLossCompanyId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            ViewBag.inspectItemId = inspectItemId;
            ViewBag.gainLossCompanyId = gainLossCompanyId;
            Result<List<InspectView>> result = new Result<List<InspectView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetInspectItemListByQc(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            string value = ConfigurationManager.AppSettings["FCGZY"];
            if (string.IsNullOrWhiteSpace(value))
            {
                value = "";
            }

            ViewBag.FCGZY = value;
            return View(result.Data);
        }

        /// <summary>
        /// 获取非常规作业和复查、复核列表
        /// </summary>
        /// <param name="inspectItemId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Browse)]
        public ActionResult IndexWorkItem(long checkId = 0, long InspectId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();
            if (InspectId > 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "InspectId";
                ce.ExpValue = InspectId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (checkId > 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "CheckParentId";
                ce.ExpValue = checkId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo.isAllowPage = false;
            Result<List<EPM_UnconventionalWork>> resultList = new Result<List<EPM_UnconventionalWork>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetWorkList(qc);

                List<long> ids = result.Data.Select(t => t.CheckId.Value).ToList();

                string str = string.Join(",", ids);
                QueryCondition qc1 = new QueryCondition();
                ConditionExpression ce1 = new ConditionExpression();

                ce1 = new ConditionExpression();
                ce1.ExpName = "CheckParentId";
                ce1.ExpValue = str;
                ce1.ExpOperater = eConditionOperator.In;
                ce1.ExpLogical = eLogicalOperator.And;
                qc1.ConditionList.Add(ce1);

                ce1 = new ConditionExpression();
                ce1.ExpName = "InspectId";
                ce1.ExpValue = InspectId;
                ce1.ExpOperater = eConditionOperator.Equal;
                ce1.ExpLogical = eLogicalOperator.And;
                qc1.ConditionList.Add(ce1);

                //qc1.PageInfo = GetPageInfo(pageIndex, pageSize);
                qc1.PageInfo.isAllowPage = false;

                resultList = proxy.GetWorkList(qc1);

                //ViewBag.Total = resultList.AllRowsCount;
                //ViewBag.TotalPage = Math.Ceiling((decimal)resultList.AllRowsCount / pageSize);
            }
            return View(resultList.Data);
        }

        public ActionResult SelectItem()
        {
            return View();
        }

        public ActionResult UploadTest()
        {
            return View();
        }

        public ActionResult hsw_upload()
        {
            return View();
        }

        /// <summary>
        /// 新增检查
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Add)]
        public ActionResult AddAQ()
        {
            //ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            //ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            //ViewBag.UserID = ApplicationContext.Current.UserID;
            //ViewBag.UserName = ApplicationContext.Current.UserName;
            //ViewBag.CheckId = ApplicationContext.Current.CompanyId;
            //ViewBag.CheckName = ApplicationContext.Current.CompanyName;

            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    var CheckItem = proxy.GetCheckItem();
            //    if (CheckItem.Flag == EResultFlag.Success && CheckItem.Data != null && CheckItem.Data.Any())
            //    {
            //        ViewBag.Work = CheckItem.Data;
            //    }
            //}
            return View();
        }

        /// <summary>
        /// 添加检查项弹出层
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSelectData(long projectId)
        {
            //ViewBag.ProjectId = "";
            //ViewBag.ProjectName = "";
            //ViewBag.UserID = ApplicationContext.Current.UserID;
            //ViewBag.UserName = ApplicationContext.Current.UserName;
            //ViewBag.CheckId = ApplicationContext.Current.CompanyId;
            //ViewBag.CheckName = ApplicationContext.Current.CompanyName;

            //ViewBag.CompanyId = new List<Epm_ProjectCompany>().ToSelectList("CompanyName", "CompanyId", true);

            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    var CheckItem = proxy.GetCheckItem();
            //    if (CheckItem.Flag == EResultFlag.Success && CheckItem.Data != null && CheckItem.Data.Any())
            //    {
            //        ViewBag.Work = CheckItem.Data;
            //    }

            //    var companyList = proxy.GetProjectCompanyList(projectId).Data;
            //    if (companyList != null && companyList.Any())
            //    {
            //        companyList = companyList.Where(t => t.CompanyId.HasValue && t.IsSupervisor == 0).ToList();
            //        companyList.ForEach(p =>
            //        {
            //            p.Id = p.CompanyId.Value;
            //        });
            //        ViewBag.CompanyId = companyList.ToSelectList("CompanyName", "SId", true);
            //    }
            //}
            return View();
        }
        /// <summary>
        /// 新增安全检查（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Add)]
        public ActionResult AddAQ(Epm_Monitor model)
        {
            MonitorView monitorList = new MonitorView();
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "检查标题不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //问题信息
            string Work = Request.Form["Work"];
            //if (string.IsNullOrWhiteSpace(Work))
            //{
            //    view.Flag = false;
            //    view.Message = "请选择整改问题！";
            //    return Json(view);
            //}

            List<Epm_MonitorDetails> detailList = JsonConvert.DeserializeObject<List<Epm_MonitorDetails>>(Work);
            //if (detailList == null || !detailList.Any())
            //{
            //    view.Flag = false;
            //    view.Message = "请选择整改问题！";
            //    return Json(view);
            //}
            List<MonitorView> list = new List<MonitorView>();

            if (detailList != null)
            {
                var companyIds = detailList.Select(p => new { p.RectifCompanyId, p.RectifCompanyName }).Distinct().ToList();

                foreach (var item in companyIds)
                {
                    Epm_Monitor monitor = new Epm_Monitor();
                    monitor.Title = model.Title;
                    monitor.ProjectId = model.ProjectId;
                    monitor.ProjectName = model.ProjectName;
                    monitor.RectifCompanyId = item.RectifCompanyId;
                    monitor.RectifCompanyName = item.RectifCompanyName;
                    monitor.Deadline = detailList.Where(p => p.RectifCompanyId == item.RectifCompanyId).Max(p => p.Deadline);
                    monitor.MonitorUserId = model.MonitorUserId;
                    monitor.MonitorUserName = model.MonitorUserName;
                    monitor.MonitorCompanyId = model.MonitorCompanyId;
                    monitor.MonitorCompanyName = model.MonitorCompanyName;
                    monitor.MonitorTime = model.MonitorTime;
                    monitor.State = model.State;
                    monitor.Remark = model.Remark;

                    MonitorView monitorView = new MonitorView();
                    monitorView.Monitor = monitor;
                    monitorView.MonitorDetails = detailList.Where(p => p.RectifCompanyId == item.RectifCompanyId).ToList();
                    monitorView.MonitorDetails.ForEach(p =>
                    {
                        p.MonitorId = monitor.Id;
                    });

                    monitorView.FileList = fileListFile;
                    list.Add(monitorView);
                }
            }
            else
            {
                Epm_Monitor monitor = new Epm_Monitor();
                monitor.Title = model.Title;
                monitor.ProjectId = model.ProjectId;
                monitor.ProjectName = model.ProjectName;
                monitor.RectifCompanyId = 0;
                monitor.RectifCompanyName = "";
                monitor.Deadline = DateTime.Now;
                monitor.MonitorUserId = model.MonitorUserId;
                monitor.MonitorUserName = model.MonitorUserName;
                monitor.MonitorCompanyId = model.MonitorCompanyId;
                monitor.MonitorCompanyName = model.MonitorCompanyName;
                monitor.MonitorTime = model.MonitorTime;
                monitor.State = model.State;
                monitor.Remark = model.Remark;

                MonitorView monitorView = new MonitorView();
                monitorView.Monitor = monitor;
                monitorView.MonitorDetails = null;
                monitorView.FileList = fileListFile;
                list.Add(monitorView);
            }

            Result<bool> result = new Result<bool>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                monitorList.Monitor = model;
                monitorList.FileList = fileListFile;

                //result = proxy.AddMonitorNew(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改安全检查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            ViewBag.CompanyId = new List<Epm_ProjectCompany>().ToSelectList("CompanyName", "CompanyId", true);
            Result<MonitorView> result = new Result<MonitorView>();
            Result<List<Base_Company>> comresult = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);

                var companyResult = proxy.GetProjectCompanyByProjectId(result.Data.Monitor.ProjectId.Value);
                if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null && companyResult.Data.Any())
                {
                    ViewBag.CompanyId = companyResult.Data.ToSelectList("CompanyName", "CompanyId", true);
                }
                //var CheckItem = proxy.GetCheckItem();
                //if (CheckItem.Flag == EResultFlag.Success && CheckItem.Data != null && CheckItem.Data.Any())
                //{
                //    ViewBag.Work = CheckItem.Data;
                //}
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改安全检查（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Monitor model)
        {
            MonitorView monitorList = new MonitorView();
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "检查标题不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //详情信息
            string Work = Request.Form["Work"];
            if (!string.IsNullOrWhiteSpace(Work))
            {
                monitorList.MonitorDetails = JsonConvert.DeserializeObject<List<Epm_MonitorDetails>>(Work);
            }

            Result<int> result = new Result<int>();
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    monitorList.Monitor = model;
            //    monitorList.FileList = fileListFile;
            //    result = proxy.UpdateMonitor(monitorList);
            //}
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 安全检查详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Info)]
        public ActionResult DetailAQ(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);
                ViewBag.IsEqualId = CurrentUser.UserId == result.Data.Monitor.CreateUserId;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 确认整改（提交数据）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="Remark"></param>
        [HttpPost]
        public ActionResult ChangeState(long id, RectificationState state, string Remark)
        {
            //if (state == RectificationState.RectificationSuccess)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.SecurityCheck.ToString(), SystemRight.Check.ToString(), true);
            //}
            //else if (state == RectificationState.RectificationOk)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.SecurityCheck.ToString(), SystemRight.UnCheck.ToString(), true);
            //}

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMonitorState(id, state, Remark);
            }
            return Json(result.ToResultView());
        }

        /// 上传整改结果页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
       // [AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Rectif)]
        public ActionResult UploadRectifyResult(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 上传整改结果（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Rectif)]
        public ActionResult UploadRectifyResult(Epm_MonitorRectifRecord model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "整改内容不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.AddMonitorRectifRecord(model, fileListFile);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 确认整改结果
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
       // [AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.AuditRectif)]
        public ActionResult ConfirmRectifyResult(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);
            }
            return View(result.Data);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<bool> result = new Result<bool>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.DeleteMonitorByIdNew(id);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 确认整改结果（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityCheck, Right = SystemRight.AuditRectif)]
        public ActionResult ConfirmRectifyResult(long id, RectificationState state, string Remark)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMonitorState(id, state, Remark);
            }
            return Json(result.ToResultView());
        }
    }
}