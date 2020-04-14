using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.epm.Common;
using hc.epm.Web.ClientProxy;
using hc.epm.DataModel.Business;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class WorkController : BaseWebController
    {
        /// <summary>
        /// 获取危险作业列表
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="workName">作业名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="state">状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string workName = "", string submitTime = "", string time = "", string state = "", int pageIndex = 1, int pageSize = 10, string WorkType = "")
        {
            ViewBag.pageIndex = pageIndex;
            ViewBag.projectName = projectName;
            //ViewBag.workName = workName;
            ViewBag.submitTime = submitTime;
            ViewBag.time = time;
            ViewBag.state = typeof(ApprovalState).AsSelectList(true, state);

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(WorkType))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "TaskTypeNo",
                    ExpValue = "%" + WorkType + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            //if (!string.IsNullOrWhiteSpace(workName))
            //{
            //    qc.ConditionList.Add(new ConditionExpression()
            //    {
            //        ExpName = "TaskName",
            //        ExpValue = "%" + workName + "%",
            //        ExpLogical = eLogicalOperator.And,
            //        ExpOperater = eConditionOperator.Like
            //    });
            //}
            if (!string.IsNullOrWhiteSpace(submitTime))
            {
                DateTime stime = Convert.ToDateTime(submitTime);
                DateTime stime1 = Convert.ToDateTime(Convert.ToDateTime(submitTime).ToString("yyyy-MM-dd") + "  23:59:59");
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CreateTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CreateTime",
                    ExpValue = stime1,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(time))
            {
                DateTime etime = Convert.ToDateTime(time);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "StartTime",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "EndTime",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = state.ToEnumReq<ApprovalState>();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = int.Parse(approvalState.GetValue().ToString()),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }

            Result<List<Epm_DangerousWork>> result = new Result<List<Epm_DangerousWork>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDangerousWorkList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.WorkType };
                var dic = proxy.GetTypeListByTypes(list).Data;
                ViewBag.WorkType = dic[DictionaryType.WorkType].ToSelectList("Name", "No", true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 新增危险作业
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.WorkType };
                var dic = proxy.GetTypeListByTypes(list).Data;
                ViewBag.WorkType = dic[DictionaryType.WorkType].ToSelectList("Name", "No", true);

                if (!string.IsNullOrWhiteSpace(ViewBag.ProjectId))
                {
                    //Epm_ProjectCompany company = proxy.GetProjectCompanyByProjectAndRole(long.Parse(ViewBag.ProjectId), RoleType.Constructor);
                    //ViewBag.WorkCompanyId = company.CompanyId;
                    //ViewBag.WorkCompanyName = company.CompanyName;
                }
            }
            return View();
        }

        /// <summary>
        /// 新增危险作业（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_DangerousWork model)
        {
            ResultView<int> view = new ResultView<int>();

            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.TaskName))
            //{
            //    view.Flag = false;
            //    view.Message = "作业名称不能为空";
            //    return Json(view);
            //}
            if (!model.StartTime.HasValue || model.StartTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "开始时间不能为空";
                return Json(view);
            }
            //if (!model.EndTime.HasValue || model.EndTime.Value == DateTime.MinValue)
            //{
            //    view.Flag = false;
            //    view.Message = "结束时间不能为空";
            //    return Json(view);
            //}
            //if (model.StartTime.Value > model.EndTime.Value)
            //{
            //    view.Flag = false;
            //    view.Message = "开始时间不能大于结束时间";
            //    return Json(view);
            //}
            //if (model.StartTime.Value > model.EndTime.Value)
            //{
            //    view.Flag = false;
            //    view.Message = "开始时间不能大于结束时间";
            //    return Json(view);
            //}
            if (string.IsNullOrEmpty(model.TaskTypeNo) || string.IsNullOrEmpty(model.TaskTypeName))
            {
                view.Flag = false;
                view.Message = "作业分类不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TaskContent))
            {
                view.Flag = false;
                view.Message = "作业内容不能为空";
                return Json(view);
            }

            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddDangerousWork(model, files);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改危险作业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_DangerousWork> result = new Result<Epm_DangerousWork>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDangerousWorkModel(id);

                List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.WorkType };
                var dic = proxy.GetTypeListByTypes(list).Data;
                ViewBag.WorkType = dic[DictionaryType.WorkType].ToSelectList("Name", "No", true, result.Data.TaskTypeNo);

            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改危险作业（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_DangerousWork model)
        {
            ResultView<int> view = new ResultView<int>();

            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.TaskName))
            //{
            //    view.Flag = false;
            //    view.Message = "作业名称不能为空";
            //    return Json(view);
            //}
            if (!model.StartTime.HasValue || model.StartTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "开始时间不能为空";
                return Json(view);
            }
            //if (!model.EndTime.HasValue || model.EndTime.Value == DateTime.MinValue)
            //{
            //    view.Flag = false;
            //    view.Message = "结束时间不能为空";
            //    return Json(view);
            //}
            //if (model.StartTime.Value > model.EndTime.Value)
            //{
            //    view.Flag = false;
            //    view.Message = "开始时间不能大于结束时间";
            //    return Json(view);
            //}
            //if (model.StartTime.Value > model.EndTime.Value)
            //{
            //    view.Flag = false;
            //    view.Message = "开始时间不能大于结束时间";
            //    return Json(view);
            //}
            if (string.IsNullOrEmpty(model.TaskTypeNo) || string.IsNullOrEmpty(model.TaskTypeName))
            {
                view.Flag = false;
                view.Message = "作业分类不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TaskContent))
            {
                view.Flag = false;
                view.Message = "作业内容不能为空";
                return Json(view);
            }

            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDangerousWork(model, files);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看危险作业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Info)]
        public ActionResult Detail(long id, int type = 1)
        {
            ViewBag.detail = type;
            Result<Epm_DangerousWork> result = new Result<Epm_DangerousWork>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.WorkType };
                var dic = proxy.GetTypeListByTypes(list).Data;
                ViewBag.WorkType = dic[DictionaryType.WorkType].ToSelectList("Name", "No", true);

                result = proxy.GetDangerousWorkModel(id);

                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "WorkId",
                    ExpValue = id,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                ViewBag.RealSceneList = proxy.GetWorkRealSceneList(qc).Data;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Check)]
        [HttpPost]
        public ActionResult Audit(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDangerousWorkState(id, ApprovalState.ApprSuccess);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.UnCheck)]
        [HttpPost]
        public ActionResult Reject(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDangerousWorkState(id, ApprovalState.ApprFailure);
            }
            return Json(result.ToResultView());
        }
        
        /// <summary>
        /// 审核实景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Check)]
        [HttpPost]
        public ActionResult AuditRealScene(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateWorkRealScenenState(id, ApprovalState.ApprSuccess);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回实景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.UnCheck)]
        [HttpPost]
        public ActionResult RejectRealScene(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateWorkRealScenenState(id, ApprovalState.ApprFailure);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Invalid)]
        [HttpPost]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDangerousWorkState(id, ApprovalState.Discarded);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.DangerousWork, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteDangerousWork(id);
            }
            return Json(result.ToResultView());
        }

        #region 导出
        [HttpPost]
        public void OutputWorkToExcel()
        {
            string fileName = HttpContext.Request.Form["fileName"];

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                Result<List<Epm_DangerousWork>> result = proxy.GetDangerousWorkList(qc);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        { "ProjectName" , "项目名称"},
                        //{ "TaskName" , "作业名称"},
                        {"StartTime","作业时间" },
                        //{"EndTime","截止日期" },
                        {"TaskTypeName","作业分类" },
                        //{"TaskArea","作业区域" },
                        {"TaskContent","作业内容" },
                        //{"Protective","防护设施" },
                        {"State","状态" },
                    };


                    var data = result.Data.Select(p => new
                    {
                        p.ProjectName,
                        //p.TaskName,
                        StartTime = string.Format("{0:yyyy-MM-dd}", p.StartTime),
                        //EndTime = string.Format("{0:yyyy-MM-dd}", p.EndTime),
                        p.TaskTypeName,
                        //p.TaskArea,
                        p.TaskContent,
                        //p.Protective,
                        State = (p.State ?? 0).ToString().ToEnum<ApprovalState>().GetText()
                    });

                    hc.epm.UI.Common.ExcelHelper.ExportExcel(fileName, dic, data.Cast<object>().ToList(), HttpContext);
                }
            }
        }
        #endregion

        public ActionResult SaveWorkFile(long id)
        {
            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_DangerousWork work = proxy.GetDangerousWorkModel(id).Data;

                Epm_WorkUploadRealScene model = new Epm_WorkUploadRealScene();
                model.WorkId = work.Id;
                model.WorkName = work.TaskName;
                model.ProjectId = work.ProjectId;
                model.UploadTime = DateTime.Now;
                model.State = (int)ApprovalState.WaitAppr;
                model.Remark = "";

                proxy.AddWorkRealScenen(model, files);
            }
            return Json(result.ToResultView());
        }
    }
}