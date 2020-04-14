using System;
using System.Collections.Generic;
using hc.epm.UI.Common;
using System.Web.Mvc;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Global;
using hc.epm.Web.ClientProxy;
using hc.epm.Common;
using System.Linq;
using Newtonsoft.Json;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 项目提出申请
    /// </summary>
    public class TzProjectProposalController : BaseWebController
    {
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string projectNature = "", string stationName = "", string companyId = "", string startTime = "", string projectState = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.stationName = stationName;
            ViewBag.companyId = companyId;
            ViewBag.projectNature = projectNature;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                ViewBag.IsAgencyUser = false;
                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    //是省公司
                    if (companyInfo.OrgType == "1" || (companyInfo.PId == 10 && companyInfo.OrgType == "3"))
                    {
                        ViewBag.IsAgencyUser = true;
                        companyId = "";
                    }
                    else if (companyInfo.OrgType == "2" || (companyInfo.PId != 10 && companyInfo.OrgType == "3"))
                    {
                        companyId = CurrentUser.CompanyId.ToString();
                        ViewBag.CompanyName = CurrentUser.CompanyName;
                    }
                }

                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;

                if (!string.IsNullOrEmpty(projectState))
                {
                    int d = (int)(PreProjectState)Enum.Parse(typeof(PreProjectState), projectState);

                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = d;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }

                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = "%" + projectName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(projectNature))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Nature";
                    ce.ExpValue = projectNature;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(stationName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "StationName";
                    ce.ExpValue = "%" + stationName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = Convert.ToInt64(companyId);
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ApplyTime",
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

                        ExpName = "ApplyTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);


                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetSingleTzProjectProposalList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();

                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToList().ToSelectList("Name", "No", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreProjectApprovalState).AsSelectList(true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.GasStationType, DictionaryType.GeographicDosition, DictionaryType.FileType, DictionaryType.ProjectType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //地市公司
                ViewBag.CompanyName = "";
                ViewBag.CompanyId = "";
                //地区公司
                ViewBag.ProvinceName = "";
                ViewBag.ProvinceId = "";

                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    ViewBag.CompanyName = companyInfo.Name ?? "";
                    ViewBag.CompanyId = CurrentUser.CompanyId.ToString();

                    ViewBag.ProvinceName = companyInfo.PreName ?? "";
                    ViewBag.ProvinceCode = companyInfo.PreCode ?? "";
                }

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].Where(t => t.No != "HZHZ").ToList().ToSelectList("Name", "No", true);

                // 加油站类别
                ViewBag.GasStationType = subjects[DictionaryType.GasStationType].ToSelectList("Name", "No", true);

                //地理位置
                ViewBag.GeographicDosition = subjects[DictionaryType.GeographicDosition].ToSelectList("Name", "No", true);

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.FileType].ToList().ToSelectList("Name", "No", true);

                ViewBag.ProjectType = subjects[DictionaryType.ProjectType].ToSelectList("Name", "No", false);
                return View();
            }
        }

        [HttpPost]
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzProjectProposal model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzProjectProposal(model);
            }
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.GasStationType, DictionaryType.GeographicDosition, DictionaryType.FileType, DictionaryType.ProjectType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //地市公司
                ViewBag.CompanyName = "";
                ViewBag.CompanyId = "";
                //地区公司
                ViewBag.ProvinceName = "";
                ViewBag.ProvinceId = "";

                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    ViewBag.CompanyName = companyInfo.Name ?? "";
                    ViewBag.CompanyId = CurrentUser.CompanyId.ToString();

                    ViewBag.ProvinceName = companyInfo.PreName ?? "";
                    ViewBag.ProvinceCode = companyInfo.PreCode ?? "";
                }

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.FileType].ToList().ToSelectList("Name", "No", true);

                var result = proxy.GetTzProjectProposalModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    var date = result.Data;
                    // 项目性质
                    ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].Where(t => t.No != "HZHZ").ToList().ToSelectList("Name", "No", true, date.Nature);

                    // 加油站类别
                    ViewBag.GasStationType = subjects[DictionaryType.GasStationType].ToSelectList("Name", "No", true, date.StationType);

                    //地理位置
                    ViewBag.GeographicDosition = subjects[DictionaryType.GeographicDosition].ToSelectList("Name", "No", true, date.PositionType);

                    ViewBag.ProjectType = subjects[DictionaryType.ProjectType].ToSelectList("Name", "No", false, date.ProjectType);

                    return View(result.Data);
                }
                return View();
            }
        }

        [HttpPost]
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzProjectProposal model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzProjectProposal(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Info)]
        public ActionResult Detail(long id, string param)
        {
            ViewBag.param = param;
            ViewBag.id = id;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzProjectProposalALL(id);
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
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Check)]
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
                result = proxy.UpdateTzProjectProposalState(idList, state);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.TzProjectProposal, Right = SystemRight.Close)]
        public ActionResult CloseProject(long projectId)
        {
            ResultView<int> view = new ResultView<int>();
            if (projectId == 0)
            {
                view.Flag = false;
                view.Message = "请选择要关闭的项目";
                return Json(view);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.CloseTzProjectProposal(projectId);
            }
            return Json(result.ToResultView());
        }
    }
}