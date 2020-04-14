using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 现场调研
    /// </summary>
    public class TzResearchController : BaseWebController
    {
        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="projectNature">项目性质</param>
        /// <param name="companyName">地市公司名称</param>
        /// <param name="startTime">提出时间-开始时间</param>
        /// <param name="endTime">提出时间-结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzResearch, Right = SystemRight.Browse)]
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

                #region 查询条件
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
                    ce.ExpValue = projectName;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(projectNature))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectNature";
                    ce.ExpValue = projectNature;
                    ce.ExpOperater = eConditionOperator.Equal;
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
                if (!string.IsNullOrEmpty(stationName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "StationName";
                    ce.ExpValue = stationName;
                    ce.ExpOperater = eConditionOperator.Equal;
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
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzResearchList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToList().ToSelectList("Name", "No", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreProjectSubmitState).AsSelectList(true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 现场勘查添加页面
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzResearch, Right = SystemRight.Modify)]
        public ActionResult Add(long projectId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Environment, DictionaryType.NatureLand, DictionaryType.LandUse, DictionaryType.ResearchFileType, DictionaryType.Job, DictionaryType.VanityProject, DictionaryType.PermitType, DictionaryType.ReformNeed };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                var result = proxy.GetTzSiteSurveyProject(projectId);

                // 周边环境
                string envi = result.Data.TzResearchOfInvestment.EnvironmentTypeCode == null ? "" : result.Data.TzResearchOfInvestment.EnvironmentTypeCode;
                ViewBag.EnvironmentTypeCode = subjects[DictionaryType.Environment].ToSelectList("Name", "No", true, result.Data.TzResearchOfInvestment.EnvironmentTypeCode ?? "");

                // 土地性质
                ViewBag.LandNatureCode = subjects[DictionaryType.NatureLand].ToSelectList("Name", "No", true, result.Data.TzResearchOfInvestment.LandNatureCode ?? "");

                // 土地用途
                ViewBag.LandUseCode = subjects[DictionaryType.LandUse].ToSelectList("Name", "No", true, result.Data.TzResearchOfInvestment.LandUseCode ?? "");

                var job = subjects[DictionaryType.Job].ToList();
                // 职务-现场工程方面调研
                ViewBag.JobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfEngineering.JobCode ?? "");

                // 职务-信息方面调研
                ViewBag.InfoJobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfInformation.JobCode ?? "");

                // 职务-现场投资调研
                ViewBag.InvestJobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfInvestment.JobCode ?? "");

                // 职务-现场法律调研
                ViewBag.LawJobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfLaw.JobCode ?? "");

                // 职务-经营方面调研
                ViewBag.ManageJobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfManagement.JobCode ?? "");

                // 职务-安全方面调研
                ViewBag.SafeJobCode = job.ToSelectList("Name", "No", true, result.Data.TzResearchOfSecurity.JobCode ?? "");


                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.ResearchFileType].ToList().ToSelectList("Name", "No", true);

                //形象工程符合行业规划
                ViewBag.IndustryPlanning = subjects[DictionaryType.VanityProject].ToList();

                //证照类型
                ViewBag.License = subjects[DictionaryType.PermitType].ToList();

                //改造必要性
                ViewBag.ReformCode = subjects[DictionaryType.ReformNeed].ToList();

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        [HttpPost]
        [AuthCheck(Module = WebCategory.TzResearch, Right = SystemRight.Modify)]
        public ActionResult Add(TzResearchView model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzSiteSurvey(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取详情                                                              
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzResearch, Right = SystemRight.Info)]
        public ActionResult Detail(long id = 0)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzSiteSurveyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
    }
}