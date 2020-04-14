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
    /// <summary>
    /// 质量检查
    /// </summary>
    public class QualityMonitorController : BaseWebController
    {
        /// <summary>
        /// 质量检查列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Browse)]
        public ActionResult IndexZL(string projectName = "", string title = "", string startTime = "", string endTime = "", string state = "", int pageIndex = 1, int pageSize = 10, string monitorTypeNo = "QualityCheck")
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.title = title;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "MonitorTypeNo";
            ce.ExpValue = "QualityCheck";
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            if (!string.IsNullOrEmpty(title))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + title + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(monitorTypeNo))
            {
                ce = new ConditionExpression();
                ce.ExpName = "MonitorTypeNo";
                ce.ExpValue = monitorTypeNo;
                ce.ExpOperater = eConditionOperator.Equal;
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
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "MonitorTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime etime = Convert.ToDateTime(endTime);
                qc.ConditionList.Add(new ConditionExpression()
                {

                    ExpName = "MonitorTime",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            //if (!string.IsNullOrEmpty(state))
            //{
            //    int statu = int.Parse(((CheckState)(Enum.Parse(typeof(CheckState), state))).GetValue().ToString());
            //    ce = new ConditionExpression();
            //    ce.ExpName = "State";
            //    ce.ExpValue = statu;
            //    ce.ExpOperater = eConditionOperator.Equal;
            //    ce.ExpLogical = eLogicalOperator.And;
            //    qc.ConditionList.Add(ce);
            //}
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_Monitor>> result = new Result<List<Epm_Monitor>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //检查状态下拉数据
                //ViewBag.CheckState = typeof(CheckState).AsSelectList(true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 新增质量检查
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Add)]
        public ActionResult AddZL()
        {
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();

            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            ViewBag.CompanyId = ApplicationContext.Current.CompanyId;
            ViewBag.CompanyName = ApplicationContext.Current.CompanyName;
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;

            return View();
        }

        /// <summary>
        /// 新增质量检查（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Add)]
        public ActionResult AddZL(Epm_Monitor model)
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
            if (string.IsNullOrWhiteSpace(model.MonitorTypeNo) || string.IsNullOrWhiteSpace(model.MonitorTypeName))
            {
                view.Flag = false;
                view.Message = "检查类型不能为空";
                return Json(view);
            }

            //if (model.Result == 0)
            //{
            //    model.State = (int)CheckState.WaitCheck;
            //}
            //else if (model.Result == 1)
            //{
            //    model.State = (int)CheckState.CheckSuccess;
            //}
            //else if (model.Result == 2)
            //{
            //    model.State = (int)CheckState.Rectification;
            //}
            string fileDataJson = Request.Form["fileDataJson"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //详情信息
            string monitorDetails = Request.Form["MonitorDetails"];
            if (!string.IsNullOrWhiteSpace(monitorDetails))
            {
                monitorList.MonitorDetails = JsonConvert.DeserializeObject<List<Epm_MonitorDetails>>(monitorDetails);
            }

            //关联组件
            string monitorDetailBIM = Request.Form["ComponentIds"];
            if (!string.IsNullOrWhiteSpace(monitorDetailBIM))
            {
                monitorList.MonitorDetailBIM = JsonConvert.DeserializeObject<List<Epm_MonitorDetailBIM>>(monitorDetailBIM);
            }

            if (model.Result == 2)
            {
                string Deadline = Request.Form["Deadline"];
                if (!string.IsNullOrWhiteSpace(Deadline))
                {
                    monitorList.MonitorRectifRecord.Deadline = Convert.ToDateTime(Deadline);
                    monitorList.MonitorRectifRecord.Remark = model.Rectification;
                }
            }

            Result<int> result = new Result<int>();
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    monitorList.Monitor = model;
            //    monitorList.FileList = fileListFile;
            //    result = proxy.AddMonitor(monitorList);
            //}
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改质量检查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Modify)]
        public ActionResult EditZL(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();
            Result<List<Base_Company>> comresult = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);

                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                comresult = proxy.GetCompanyList(qc);
                ViewBag.CompanyList = comresult.Data.ToSelectList("Name", "Id", true);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改质量检查（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Modify)]
        public ActionResult EditZL(Epm_Monitor model)
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
            if (string.IsNullOrWhiteSpace(model.MonitorTypeNo) || string.IsNullOrWhiteSpace(model.MonitorTypeName))
            {
                view.Flag = false;
                view.Message = "检查类型不能为空";
                return Json(view);
            }
            //if (model.Result == 0)
            //{
            //    model.State = (int)CheckState.WaitCheck;
            //}
            //else if (model.Result == 1)
            //{
            //    model.State = (int)CheckState.CheckSuccess;
            //}
            //else if (model.Result == 2)
            //{
            //    model.State = (int)CheckState.Rectification;
            //}
            string fileDataJson = Request.Form["fileDataJson"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //详情信息
            string monitorDetails = Request.Form["MonitorDetails"];
            if (!string.IsNullOrWhiteSpace(monitorDetails))
            {
                monitorList.MonitorDetails = JsonConvert.DeserializeObject<List<Epm_MonitorDetails>>(monitorDetails);
            }

            //关联组件
            string monitorDetailBIM = Request.Form["ComponentIds"];
            if (!string.IsNullOrWhiteSpace(monitorDetailBIM))
            {
                monitorList.MonitorDetailBIM = JsonConvert.DeserializeObject<List<Epm_MonitorDetailBIM>>(monitorDetailBIM);
            }

            if (model.Result == 2)
            {
                string Deadline = Request.Form["Deadline"];
                if (!string.IsNullOrWhiteSpace(Deadline))
                {
                    monitorList.MonitorRectifRecord.Deadline = Convert.ToDateTime(Deadline);
                    monitorList.MonitorRectifRecord.Remark = model.Rectification;
                }
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
        /// 质量检查详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Info)]
        public ActionResult DetailZL(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 上传整改结果页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Rectif)]
        public ActionResult UploadRectifyResultZL(long id)
        {
            Result<MonitorView> result = new Result<MonitorView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetMonitorModel(id);
            }

            //检查状态下拉数据
            //ViewBag.RectificationState = typeof(RectificationState).AsSelectList(true);
            List<string> removeValues = new List<string>();
            removeValues.Add(RectificationState.RectificationSuccess.ToString());
            removeValues.Add(RectificationState.RectificationOk.ToString());
            ViewBag.RectificationState = typeof(RectificationState).AsSelectList(false, RectificationState.WaitRectification.ToString(), removeValues);
            return View(result.Data);
        }

        /// <summary>
        /// 上传整改结果（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Rectif)]
        public ActionResult UploadRectifyResultZL(Epm_MonitorRectifRecord model)
        {
            ResultView<int> view = new ResultView<int>();

            string statu = Request.Form["State"];

            model.State = int.Parse(statu.ToEnum<RectificationState>().GetValue().ToString());

            List<Base_Files> fileListFile = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传图片json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            //表单校验
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "整改内容不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.AddMonitorRectifRecord(model, fileListFile);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 确认整改（提交数据）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="Remark"></param>
        [HttpPost]
        public ActionResult ChangeState(long id, string state, string Remark)
        {
            var statu = state.ToEnum<RectificationState>().Value;

            if (statu == RectificationState.RectificationSuccess)
            {
                Helper.IsCheck(HttpContext, WebCategory.QualityCheck.ToString(), SystemRight.Check.ToString(), true);
            }
            else if (statu == RectificationState.RectificationOk)
            {
                Helper.IsCheck(HttpContext, WebCategory.QualityCheck.ToString(), SystemRight.UnCheck.ToString(), true);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMonitorState(id, statu, Remark);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看BIM模型
        /// </summary>
        /// <param name="bimId"></param>
        /// <returns></returns>
        public ActionResult RelationBIM(long bimId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var bimModelResult = proxy.GetBimModel(bimId);
                if (bimModelResult.Flag == EResultFlag.Success && bimModelResult.Data != null)
                {
                    ViewBag.BIMAddress = bimModelResult.Data.BIMAddress;
                }
            }
            return View();
        }

        /// <summary>
        /// 根据检查id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="detailId"></param>
        /// <param name="bimId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMonitorComponentList(long detailId, long bimId)
        {
            Result<List<Epm_MonitorDetailBIM>> result = new Result<List<Epm_MonitorDetailBIM>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetComponentListByMonitorDetailId(detailId, bimId);
            }
            return Json(result.Data);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.QualityCheck, Right = SystemRight.Delete)]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            List<long> list = id.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //result = proxy.DeleteMonitorByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}