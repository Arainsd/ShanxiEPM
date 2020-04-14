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
    public class TzConDrawingController : BaseWebController
    {
        // GET: 施工图纸
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "",string ProjectState = "", string projectNature = "", string StationName = "", string companyName = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.companyName = companyName;
            ViewBag.projectNature = projectNature;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            List<int> strState = new List<int> {(int)PreProjectApprovalState.Closed };
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;

            #region 查询条件
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue ="%"+ projectName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(projectNature))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Nature";
                ce.ExpValue = projectNature;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(companyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyId";
                ce.ExpValue = companyName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "startTime",
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

                    ExpName = "endTime",
                    ExpValue = etime.AddDays(1),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            //站库名称StationName
            if (!string.IsNullOrEmpty(StationName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "StationName";
                ce.ExpValue = "%"+StationName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(ProjectState))
            {
                int d = (int)(PreProjectApprovalState)Enum.Parse(typeof(PreProjectApprovalState), ProjectState);

                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = d;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            #endregion

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_TzConDrawing>> result = new Result<List<Epm_TzConDrawing>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据-项目性质
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzConDrawingList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();

                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToList().ToSelectList("Name", "No", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreProjectApprovalState).AsSelectList(true).Where(p => p.Value != "Closed");

               // result = proxy.GetTzConDrawingList(qc);
                return View(result.Data);
            }
        }

        #region 施工图纸上传
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy=new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.DrawingType, DictionaryType.ConclusionCode };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //评审结论
                ViewBag.conclusionCode = subjects[DictionaryType.ConclusionCode].ToList();

                //施工图纸附件类型
                ViewBag.drawingType = subjects[DictionaryType.DrawingType].ToList().ToSelectList("Name", "No", true);
                return View();
            }
        }

        //保存
        [HttpPost]
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzConDrawing model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                    model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }
            if (model.TzAttachs!=null&& model.TzAttachs.Count!=0)
            {
                var res = model.TzAttachs.Where(p => p.TypeNo.Contains("SGTZSHTZ")).FirstOrDefault();
                if (res != null)
                {
                    fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符---上传到图纸管理

                }
            }
           

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzConDrawing(model, fileListFile);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 施工图纸编辑
        /// <summary>
        /// 施工图纸修改
        /// </summary>
        /// <param name="id">施工图纸id</param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.DrawingType, DictionaryType.ConclusionCode };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //评审结论
                ViewBag.conclusionCode = subjects[DictionaryType.ConclusionCode].ToList();

                //施工图纸附件类型
                ViewBag.drawingType = subjects[DictionaryType.DrawingType].ToList().ToSelectList("Name", "No", true);

                var result = proxy.GetTzConDrawingModel(id);//根据施工图纸id获取施工图纸详情

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 施工图纸修改
        /// </summary>
        /// <param name="model">施工图纸实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzConDrawing model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }
            if (model.TzAttachs != null && model.TzAttachs.Count != 0)
            {
                var res = model.TzAttachs.Where(p => p.TypeNo.Contains("SGTZSHTZ")).FirstOrDefault();
                if (res != null)
                {
                    fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符---上传到图纸管理

                }
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzConDrawing(model, fileListFile);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 施工图纸删除
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            //string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            //if (!string.IsNullOrEmpty(fileDataJson))
            //{
            //    model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            //}
            List<long> deRes = new List<long>();
            deRes.Add(id);
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteTzConDrawingByIds(deRes);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 查看施工图纸详情
        /// <summary>
        /// 根据ID查看施工图纸详情
        /// </summary>
        /// <param name="id">施工图纸ID</param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzConDrawingModel(id);//根据施工图纸id获取施工图纸详情
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
        #endregion

        #region 新增，编辑？选择项目时加载项目批复信息(批复信息表：Epm_TzProjectApproval 项目信息表：Epm_TzProjectProposal）
        //[HttpPost]
        //public ActionResult getTzProjectPropasalInfoList(int pageIndex = 1, int pageSize = 5)
        //{
        //    ViewBag.pageIndex = pageIndex;

        //    //   List<int> strState = new List<int> { (int)PreProjectState.Discarded, (int)PreProjectState.Closed };
        //    QueryCondition qc = new QueryCondition();
        //    // ConditionExpression ce = null;

        //    qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
        //    qc.PageInfo = GetPageInfo(pageIndex, pageSize);

        //    Result<List<Epm_TzProjectApproval>> result = new Result<List<Epm_TzProjectApproval>>();

        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {

        //        result = proxy.GetTzProjectApprovalList_Choice(qc);
        //        ViewBag.Total = result.AllRowsCount;
        //        ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

        //        return View();
        //    }
        //}
        #endregion

        #region 选择项目批复信息加载项目基础信息-GetTzProjectProposalModel
        [HttpPost]
        public ActionResult getTzProjectProposalInfoById(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据项目ID获取项目基础信息
                var ProjectProposal = proxy.GetProjectBasicInfoByID(id);
                if (ProjectProposal.Data != null)
                {
                    return Json(new { type = "1", data = ProjectProposal.Data });
                }
                else
                {
                    return Json( new { Type="0", data = "没有查到任何信息"});
                }
              
            }
        }
        #endregion

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.ConstructionDrawings, Right = SystemRight.Check)]
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
                result = proxy.UpdateTzConDrawingState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}