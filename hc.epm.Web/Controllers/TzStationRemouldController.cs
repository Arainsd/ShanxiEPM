using hc.epm.Common;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.Plat.Common.Extend;

namespace hc.epm.Web.Controllers
{
    public class TzStationRemouldController : BaseWebController
    {
        //  private Result<List<Epm_ReformRecord>> result;

        // GET: TzStationRemould
        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string projectNature = "", string stationName = "", string companyId = "",string remarkType="", string startTime = "", string projectState = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {

            ViewBag.stationName = stationName; //站库名称
            ViewBag.companyId = companyId;//地市公司
            ViewBag.projectNature = projectNature;
            ViewBag.projectName = projectName;//项目名称
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;//时间起
            ViewBag.endTime = endTime;//时间止
            ViewBag.remarkType = remarkType;//改造类型

            Result<List<Epm_ReformRecord>> result = new Result<List<Epm_ReformRecord>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request))) {

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
                if (!string.IsNullOrEmpty(remarkType))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "RemarkType";//改造类型
                    ce.ExpValue = "%" + remarkType.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
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
                        ExpName = "CreateTime",
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

                        ExpName = "CreateTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.StationReformType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetReformRecordList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();

                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToList().ToSelectList("Name", "No", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreProjectApprovalState).AsSelectList(true).Where(p => p.Value != "Closed");

                //库站改造改造类型
                ViewBag.StationReformType = subjects[DictionaryType.StationReformType].ToList().ToSelectList("Name", "No", true);
                
            }

            return View(result.Data);
        }

        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.StationReformType, DictionaryType.StationReformFileType,DictionaryType.CapitalSource,DictionaryType.Limit };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //库站改造改造类型
                ViewBag.StationReformType = subjects[DictionaryType.StationReformType].ToList().ToSelectList("Name", "No", false);
                //附件类型
                ViewBag.StationReformFileType = subjects[DictionaryType.StationReformFileType].ToList().ToSelectList("Name", "No", true);

                //资金来源
                ViewBag.CapitalSource = subjects[DictionaryType.CapitalSource].ToList();
                //上限下限
                ViewBag.Limit = subjects[DictionaryType.Limit].ToList();

                //地市公司
                ViewBag.CompanyName = CurrentUser.CompanyName;
                ViewBag.CompanyId = CurrentUser.CompanyId;

            }
            return View();
        }
        [HttpPost]

        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Add)]
        public ActionResult Add(Epm_ReformRecord model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddReformRecordeEtity(model);
            }
            
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.StationReformType, DictionaryType.StationReformFileType, DictionaryType.CapitalSource, DictionaryType.Limit };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //库站改造改造类型
                ViewBag.StationReformType = subjects[DictionaryType.StationReformType].ToList().ToSelectList("Name", "No", true);
                //附件类型
                ViewBag.StationReformFileType = subjects[DictionaryType.StationReformFileType].ToList().ToSelectList("Name", "No", true);

                //资金来源
                ViewBag.CapitalSource = subjects[DictionaryType.CapitalSource].ToList();
                //上限下限
                ViewBag.Limit = subjects[DictionaryType.Limit].ToList();

                var result = proxy.GetReformRecordEntity(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
            }
            return View();
        }

        /// <summary>
        /// 设计方案修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_ReformRecord model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateReformRecord(model);
            }
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = WebCategory.ReformRecord, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.StationReformFileType, DictionaryType.StationReformType, DictionaryType.CapitalSource, DictionaryType.Limit };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //库站改造改造类型
                ViewBag.StationReformType = subjects[DictionaryType.StationReformType].ToList().ToSelectList("Name", "No", true);
                //附件类型
                ViewBag.StationReformFileType = subjects[DictionaryType.StationReformFileType].ToList().ToSelectList("Name", "No", true);

                //资金来源
                ViewBag.CapitalSource = subjects[DictionaryType.CapitalSource].ToList();
                //上限下限
                ViewBag.Limit = subjects[DictionaryType.Limit].ToList();

                var result = proxy.GetReformRecordEntity(id);
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
                result = proxy.UpdateReformRecordState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}