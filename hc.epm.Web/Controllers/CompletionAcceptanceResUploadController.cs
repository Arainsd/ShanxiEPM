using hc.epm.Common;
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
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class CompletionAcceptanceResUploadController : BaseWebController
    {
        // GET: CompletionAcceptanceResUpload
        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string title = "", string state = "", int pageIndex = 1, string startTime = "", string endTime = "", int pageSize = 10)
        {
            ViewBag.title = title;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
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
                int statu = int.Parse(((PreProjectApprovalState)(Enum.Parse(typeof(PreProjectApprovalState), state))).GetValue().ToString());
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "RecTime",
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

                    ExpName = "RecTime",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_CompletionAcceptanceResUpload>> result = new Result<List<Epm_CompletionAcceptanceResUpload>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceResUploadList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.ApprovalState = typeof(PreProjectApprovalState).AsSelectList(true).Where(p => p.Value != "Closed");
            //审批状态
            ViewBag.State = typeof(PreProjectApprovalState).AsSelectList(true).Where(p => p.Value != "Closed");
            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            return View(result.Data);
        }
        /// <summary>
        /// 修改竣工验收
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<CompletionAcceptanceResUploadView> result = new Result<CompletionAcceptanceResUploadView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceResUploadModel(id);

                var project = proxy.GetProjectModelByTzId(result.Data.CompletionAcceptanceResUpload.ProjectId.Value);

                //项目数据
                ViewBag.project = project.Data;

                var itemResult = proxy.GetCompletionItem(result.Data.CompletionAcceptanceResUpload.ProjectId.Value);
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();
                if (itemResult.Flag == EResultFlag.Success && itemResult.Data != null)
                {
                    list = itemResult.Data;
                }

                ViewBag.CompletionAcceptanceItemList = list;
                ViewBag.isAdd = false;

                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.AcceptanceCheckType, DictionaryType.ConclusionCode };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.AcceptanceCheckType = subjects[DictionaryType.AcceptanceCheckType].ToList().ToSelectList("Name", "No", true);

                ViewBag.isAdd = false;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改竣工验收（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_CompletionAcceptanceResUpload model)
        {
            ResultView<int> view = new ResultView<int>();
            Epm_CompletionAcceptanceResUpload complete = new Epm_CompletionAcceptanceResUpload();
            //complete.CompletionAcceptance = model;
            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "验收标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "验收内容不能为空";
                return Json(view);
            }
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> subjectsList = new List<DictionaryType>() {DictionaryType.AcceptanceCheckType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.acceptanceCheckType = subjects[DictionaryType.AcceptanceCheckType].ToList().ToSelectList("Name", "No", true);

                result = proxy.UpdateCompletionAcceptanceResUpload(model);

             
            }
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<CompletionAcceptanceResUploadView> result = new Result<CompletionAcceptanceResUploadView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceResUploadModel(id);

                var project = proxy.GetProjectModelByTzId(result.Data.CompletionAcceptanceResUpload.ProjectId.Value);

                //项目数据
                ViewBag.project = project.Data;

                var itemResult = proxy.GetCompletionItem(result.Data.CompletionAcceptanceResUpload.ProjectId.Value);
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();
                if (itemResult.Flag == EResultFlag.Success && itemResult.Data != null)
                {
                    list = itemResult.Data;
                }

                ViewBag.CompletionAcceptanceItemList = list;
                ViewBag.isAdd = false;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionAcceptance, Right = SystemRight.Check)]
        public ActionResult UpdateState(string ids, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(ids))
            {
                view.Flag = false;
                view.Message = "请选择要删除的数据";
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
                 result = proxy.UpdateCompletionAcceptanceResUploadState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}