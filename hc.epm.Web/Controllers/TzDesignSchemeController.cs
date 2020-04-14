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
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 设计方案信息
    /// </summary>
    public class TzDesignSchemeController : BaseWebController
    {
        //设计方案
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "",string ProjectState = "", string ProjectNatureType = "",string StationName="", string companyName = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.companyName = companyName;
            ViewBag.projectNature = ProjectNatureType;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime; 
            ViewBag.StationName = StationName;

            List<int> strState = new List<int> {(int)PreProjectApprovalState.Closed };
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;


            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + projectName.Trim() + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(ProjectNatureType))//项目性质
            {
                ce = new ConditionExpression();
                ce.ExpName = "Nature";
                ce.ExpValue = ProjectNatureType;
                ce.ExpOperater = eConditionOperator.Equal;
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
            //站库名称StationName
            if (!string.IsNullOrEmpty(StationName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "StationName";
                ce.ExpValue = "%" + StationName.Trim() + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_TzDesignScheme>> result = new Result<List<Epm_TzDesignScheme>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据-ProjectNature项目性质
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzDesignSchemeList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();

                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToList().ToSelectList("Name", "No", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreProjectApprovalState).AsSelectList(true).Where(p=>p.Value!= "Closed");
                return View(result.Data);
            }
        }

        #region 设计方案录入
        /// <summary>
        /// 新增设计方案
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据(standardType-示范/标准类别，EnclosureType-附件类型)
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.StandardType, DictionaryType.EnclosureType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //示范/标准类别
                ViewBag.standType = subjects[DictionaryType.StandardType].ToList().ToSelectList("Name", "No", true);

                //附件类型
                ViewBag.enclosureType = subjects[DictionaryType.EnclosureType].ToList().ToSelectList("Name", "No", true);

                return View();
            }

        }
        /// <summary>
        /// 新增设计方案
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzDesignScheme model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzDesignScheme(model);
            }
            return Json(result.ToResultView());
        }
        #endregion


        #region 设计方案修改
        /// <summary>
        /// 设计方案修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据(standardType-示范/标准类别，EnclosureType-附件类型)
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.StandardType, DictionaryType.EnclosureType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //示范/标准类别
                ViewBag.standType = subjects[DictionaryType.StandardType].ToList().ToSelectList("Name", "No", true);

                //附件类型
                ViewBag.enclosureType = subjects[DictionaryType.EnclosureType].ToList().ToSelectList("Name", "No", true);

                var result = proxy.GetTzDesignSchemeModel(id);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 设计方案修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzDesignScheme model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzDesignScheme(model);
            }
            return Json(result.ToResultView());
        }
        #endregion


        #region 查看设计方案详情
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzDesignSchemeModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
        #endregion


        #region 根据项目ID获取项目批复信息
        //public ActionResult getProjectApprovalInfoById(long id)
        //{
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        //根据项目ID获取项目批复信息
        //        var ProjectApprova = proxy.GetTzProjectApprovalInfoModel(id);
        //        if (ProjectApprova.Data != null)
        //        {
        //            return Json(new { type = "1", data = ProjectApprova.Data });
        //        }

        //    }
        //    return Json(new { type = "0", data = "error" });
        //}
        #endregion

        #region 新增，编辑？选择项目时加载项目批复信息(批复信息表：Epm_TzProjectApproval 项目信息表：Epm_TzProjectProposal）
        //[HttpPost]
        //public ActionResult getTzProjectPropasalInfoList(int pageIndex = 1, int pageSize = 5)
        //{
        //    ViewBag.pageIndex = pageIndex;

        // //   List<int> strState = new List<int> { (int)PreProjectState.Discarded, (int)PreProjectState.Closed };
        //    QueryCondition qc = new QueryCondition();
        //   // ConditionExpression ce = null;

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

        #region 选择项目批复信息加载项目基础信息
        [HttpPost]
        public ActionResult getTzProjectProposalInfoById(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据项目ID获取项目基础信息
                var ProjectProposal = proxy.GetTzProjectProposalModel(id);
                if (ProjectProposal.Data != null)
                {
                    return Json(new { type = "1", data = ProjectProposal.Data });
                }

            }
            return Json(new { type = "0", data = "error" });
        }
        #endregion

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.DesignScheme, Right = SystemRight.Check)]
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
                result = proxy.UpdateTzDesignSchemeState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}