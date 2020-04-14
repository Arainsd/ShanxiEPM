using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 土地协议谈判
    /// </summary>
    public class TzLandNegotiationController : BaseWebController
    {
        // GET: TzLandNegotiation
        [AuthCheck(Module = WebCategory.TzLandNegotiation, Right = SystemRight.Browse)]
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

                result = proxy.GetTzLandNegotiationList(qc);
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
        /// 土地协议谈判信息录入页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzLandNegotiation, Right = SystemRight.Modify)]
        public ActionResult Add(long projectId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.NegotiateResult, DictionaryType.NegotiationFileType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                var result = proxy.GetTzLandNegotiationModel(projectId);

                // 谈判结果
                ViewBag.LandNatureCode = subjects[DictionaryType.NegotiateResult].ToSelectList("Name", "No", false, result.Data.TzLandNegotiation.TalkResultType ?? "");

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.NegotiationFileType].ToList().ToSelectList("Name", "No", false);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 土地协议谈判
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.TzLandNegotiation, Right = SystemRight.Modify)]
        public ActionResult Add(Epm_TzLandNegotiation model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzLandNegotiation(model);
            }
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = WebCategory.TzLandNegotiation, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzLandTalkModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
    }
}