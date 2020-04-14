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
    /// 招标申请
    /// </summary>
    public class TzTenderingApplyController : BaseWebController
    {
        // GET: TzTenderingApply
        /// <summary>
        /// 招标申请列表
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="userName">联系人</param>
        /// <param name="phone">联系电话</param>
        /// <param name="bidType">招标方式</param>
        /// <param name="state">状态</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Browse)]
        public ActionResult Index(string tenderingName = "", string userName = "", string phone = "", string bidType = "", string state = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.userName = userName;
            ViewBag.phone = phone;
            ViewBag.tenderingName = tenderingName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            Result<List<Epm_TzTenderingApply>> result = new Result<List<Epm_TzTenderingApply>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(tenderingName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "TenderingName";
                    ce.ExpValue = "%" + tenderingName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "UndertakeContacts";
                    ce.ExpValue = "%" + userName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "UndertakeTel";
                    ce.ExpValue = "%" + phone.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(bidType))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "BidType";
                    ce.ExpValue = bidType;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(state))
                {
                    int d = (int)(PreProjectState)Enum.Parse(typeof(PreProjectState), state);
                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = d;
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
                #endregion

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TenderingFileType, DictionaryType.BiddingMethod };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzTenderingApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //招标方式
                ViewBag.BidType = subjects[DictionaryType.BiddingMethod].ToList().ToSelectList("Name", "No", true);
                //审批状态
                ViewBag.State = typeof(PreProjectApprovalState).AsSelectList(true).Where(p => p.Value != "Closed");
            }

            return View(result.Data);
        }

        /// <summary>
        /// 招标申请录入页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TenderingFileType, DictionaryType.BiddingMethod, DictionaryType.TenderingType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.TenderingFileType].ToList().ToSelectList("Name", "No", true);

                //招标方式
                ViewBag.BidType = subjects[DictionaryType.BiddingMethod].ToList().ToSelectList("Name", "No", true);

                //招标类型
                ViewBag.TenderingType = subjects[DictionaryType.TenderingType].ToList().ToSelectList("Name", "No", true);

                return View();
            }
        }

        /// <summary>
        /// 招标申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzTenderingApply model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzTenderingApply(model);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 招标申请修改页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_TzTenderingApply> result = new Result<Epm_TzTenderingApply>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TenderingFileType, DictionaryType.BiddingMethod, DictionaryType.TenderingType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.AnnexType = subjects[DictionaryType.TenderingFileType].ToList().ToSelectList("Name", "No", true);

                result = proxy.GetTzTenderingApplyModel(id);

                //招标方式
                ViewBag.BidType = subjects[DictionaryType.BiddingMethod].ToList().ToSelectList("Name", "No", true, result.Data.BidType);

                //招标类型
                ViewBag.TenderingType = subjects[DictionaryType.TenderingType].ToList().ToSelectList("Name", "No", true, result.Data.TenderingType);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 招标申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzTenderingApply model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzTenderingApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 招标申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Epm_TzTenderingApply> result = new Result<Epm_TzTenderingApply>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzTenderingApplyModel(id);
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
        [AuthCheck(Module = WebModule.TenderingApply, Right = SystemRight.Check)]
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
                result = proxy.UpdateTzTenderingApplyState(idList, state);
            }
            return Json(result.ToResultView());
        }
    }
}