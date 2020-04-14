using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.UI.Common;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Global;
using hc.epm.Web.ClientProxy;
using hc.epm.ViewModel;
using hc.epm.DataModel.Basic;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class InvestManageController : BaseWebController
    {
        // GET: InvestManage
        public ActionResult Index(string projectName = "", string companyName = "", string companyId = "", string areaCompanyCode = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.companyName = companyName;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.CompanyName = companyName;

            Result<List<Epm_ReformRecord>> result = new Result<List<Epm_ReformRecord>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //判断是否省公司
                //bool IsAgencyUser = proxy.IsAgencyUser(CurrentUser.UserId);
                //ViewBag.IsAgencyUser = IsAgencyUser;
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
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = "%" + projectName + "%";
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
                if (!string.IsNullOrEmpty(areaCompanyCode))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "AreaCompanyCode";
                    ce.ExpValue = "%" + areaCompanyCode + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "RemarkStartTime",
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

                        ExpName = "RemarkEndTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);

                result = proxy.GetReformRecordList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }


        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ReformType, DictionaryType.FileType, DictionaryType.Limit, DictionaryType.CapitalSource };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //地市公司
                ViewBag.CompanyName = "";
                ViewBag.CompanyId = "";

                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    ViewBag.CompanyName = companyInfo.Name ?? "";
                    ViewBag.CompanyId = CurrentUser.CompanyId.ToString();
                }

                //改造类型
                ViewBag.RemarkType = subjects[DictionaryType.ReformType].ToSelectList("Name", "No", false);

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.FileType].Where(T => T.No == "gzfj1" || T.No == "gzfj2" || T.No == "gzfj3").ToList().ToSelectList("Name", "No", false);

                //资金来源
                ViewBag.SourceFund = subjects[DictionaryType.CapitalSource].ToList();

                //限上限下
                ViewBag.LimitType = subjects[DictionaryType.Limit].ToList();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add(Epm_ReformRecord model)
        {
            ReformRecordView view = new ReformRecordView();

            view.ReformRecord = model;
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                view.Attachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddReformRecord(view);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            Result<ReformRecordView> result = new Result<ReformRecordView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ReformType, DictionaryType.FileType, DictionaryType.Limit, DictionaryType.CapitalSource };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetReformRecordModel(id);

                ViewBag.RemarkType = "";
                if (result.Data != null)
                {
                    var code = result.Data.ReformRecord.RemarkType;

                    var list = subjects[DictionaryType.ReformType].ToList();
                    if (!string.IsNullOrEmpty(code))
                    {
                        var dicModel = list.Where(t => t.No == code).FirstOrDefault();
                        if (dicModel != null)
                        {
                            //改造类型
                            ViewBag.RemarkType = dicModel.Name;
                        }
                        else
                        {
                            ViewBag.RemarkType = code;
                        }
                    }
                }
            }
            return View(result.Data);
        }
    }
}