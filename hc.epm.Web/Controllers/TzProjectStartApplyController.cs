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
    /// 开工申请
    /// </summary>
    public class TzProjectStartApplyController : BaseWebController
    {
        // GET: TzProjectStartApply
        /// <summary>
        /// 开工申请列表
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="companyId">分公司</param>
        /// <param name="startTime">计划开始时间</param>
        /// <param name="endTime">计划开始时间</param>
        /// <param name="startTime2">计划结束时间</param>
        /// <param name="endTime2">计划结束时间</param>
        /// <param name="applyTime1">申请时间</param>
        /// <param name="applyTime2">申请时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Browse)]
        public ActionResult Index(string title = "", string companyId = "", string startTime = "", string endTime = "", string startTime2 = "", string endTime2 = "", string applyTime1 = "", string applyTime2 = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageSize = pageSize;
            ViewBag.pageIndex = pageIndex;

            ViewBag.title = title;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.startTime2 = startTime2;
            ViewBag.endTime2 = endTime2;
            ViewBag.applyTime1 = applyTime1;
            ViewBag.applyTime2 = applyTime2;
            Result<List<Epm_TzProjectStartApply>> result = new Result<List<Epm_TzProjectStartApply>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.IsAgencyUser = false;
                //var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                //if (companyInfo != null)
                //{
                //    //是省公司
                //    if (companyInfo.OrgType == "1" || (companyInfo.PId == 10 && companyInfo.OrgType == "3"))
                //    {
                //        ViewBag.IsAgencyUser = true;
                //        companyId = "";
                //    }
                //    else if (companyInfo.OrgType == "2" || (companyInfo.PId != 10 && companyInfo.OrgType == "3"))
                //    {
                //        companyId = CurrentUser.CompanyId.ToString();
                //        ViewBag.CompanyName = CurrentUser.CompanyName;
                //    }
                //}

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(title))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ApplyTitle";
                    ce.ExpValue = "%" + title + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ApplyCompanyId";
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
                        ExpName = "PlanStartTime",
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

                        ExpName = "PlanStartTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                if (!string.IsNullOrWhiteSpace(startTime2))
                {
                    DateTime stime2 = Convert.ToDateTime(startTime2);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "PlanEndTime",
                        ExpValue = stime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(endTime2))
                {
                    DateTime etime2 = Convert.ToDateTime(endTime2 + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "PlanEndTime",
                        ExpValue = etime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                if (!string.IsNullOrWhiteSpace(applyTime1))
                {
                    DateTime stime2 = Convert.ToDateTime(applyTime1);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "StartApplyTime",
                        ExpValue = stime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(applyTime2))
                {
                    DateTime etime2 = Convert.ToDateTime(applyTime2 + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "StartApplyTime",
                        ExpValue = etime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                //审批状态
                ViewBag.ProjectState = typeof(PreCompletionScceptanceState).AsSelectList(true);

                result = proxy.GetTzProjectStartApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 开工申请录入页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //资金来源
                ViewBag.MoneySourceType = subjects[DictionaryType.CapitalSource].ToSelectList("Name", "No", false);
                //申请人
                ViewBag.ApplyUserName = CurrentUser.RealName;
                ViewBag.ApplyUserId = CurrentUser.UserId;
                //申请单位
                ViewBag.ApplyCompanyName = CurrentUser.CompanyName;
                ViewBag.ApplyCompanyId = CurrentUser.CompanyId;
                //申请时间
                ViewBag.StartApplyTime = DateTime.Now.ToString("yyyy-MM-dd");
                //申请部门
                ViewBag.ApplyDepartmentId = "";
                ViewBag.ApplyDepartment = "";

                //标题
                ViewBag.ApplyTitle = "项目开工申请" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");
                //获取用户信息
                var userInfo = proxy.GetUserModel(CurrentUser.UserId);
                if (userInfo.Data != null)
                {
                    if (userInfo.Data.DepartmentId != null)
                    {
                        long dempId = userInfo.Data.DepartmentId.Value;
                        if (dempId != 0)
                        {
                            var companyInfo = proxy.GetCompanyModel(dempId);

                            if (companyInfo.Data != null)
                            {
                                ViewBag.ApplyDepartmentId = companyInfo.Data.Id;
                                ViewBag.ApplyDepartment = companyInfo.Data.Name;
                            }
                        }
                    }
                }

            }
            return View();
        }

        /// <summary>
        /// 开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzProjectStartApply model)
        {
            Result<int> result = new Result<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzProjectStartApply(model);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 开工申请修改页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult Edit(long id)
        //{
        //    Result<Epm_TzProjectStartApply> result = new Result<Epm_TzProjectStartApply>();
        //    ResultView<int> view = new ResultView<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //     
        //        //根据字典类型集合获取字典数据
        //        List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
        //        var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

        //        result = proxy.GetTzProjectStartApplyModel(id);

        //      //

        //        //资金来源
        //        ViewBag.FundsSourceType = subjects[DictionaryType.CapitalSource].ToList().ToSelectList("Name", "No", false, result.Data == null ? "" : result.Data.FundsSourceType);

        //        ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型

        //        if (result.Flag == EResultFlag.Success && result.Data != null)
        //        {
        //            return View(result.Data);
        //        }
        //        return View();
        //    }
        //}

        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<TzStartTenderingAndSupplyView> result = new Result<TzStartTenderingAndSupplyView>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //申请人
                ViewBag.ApplyUserName = CurrentUser.RealName;
                ViewBag.ApplyUserId = CurrentUser.UserId;

                // 申请单位
                ViewBag.ApplyCompanyName = CurrentUser.CompanyName;
                ViewBag.ApplyCompanyId = CurrentUser.CompanyId;

                //申请时间
                ViewBag.StartApplyTime = DateTime.Now.ToString("yyyy-MM-dd");
                //申请部门
                ViewBag.ApplyDepartmentId = "";
                ViewBag.ApplyDepartment = "";
                
                //获取用户信息
                var userInfo = proxy.GetUserModel(CurrentUser.UserId);
                if (userInfo.Data != null)
                {
                    if (userInfo.Data.DepartmentId != null)
                    {
                        long dempId = userInfo.Data.DepartmentId.Value;
                        if (dempId != 0)
                        {
                            var companyInfo = proxy.GetCompanyModel(dempId);

                            if (companyInfo.Data != null)
                            {
                                ViewBag.ApplyDepartmentId = companyInfo.Data.Id;
                                ViewBag.ApplyDepartment = companyInfo.Data.Name;
                            }
                        }
                    }
                }

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzProjectStartApplyModelAndOther(id);

                //资金来源
                ViewBag.FundsSourceType = subjects[DictionaryType.CapitalSource].ToList().ToSelectList("Name", "No", false, result.Data == null ? "" : result.Data.TzProjectStartApply.FundsSourceType);

                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }
        /// <summary>
        /// 点击编辑按钮需要判断招标申请和甲供物资是否完成，只有完成招标申请和甲供物资申请才可以做开工申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult isExistTenderingAndSupply(long id)
        {
            //点击编辑按钮需要判断招标申请和甲供物资是否完成，只有完成招标申请和甲供物资申请才可以做开工申请
            Result<bool> check = new Result<bool>();
            if (id <= 0)
            {
                return Json(new ResultView<bool>()
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要编辑的内容！"
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                 check = proxy.isExistTenderingAndSupply(id);
                return Json(check.ToResultView());
            }
        }

        /// <summary>
        /// 开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Modify)]
        public ActionResult Edit(TzProjectStartApplyView model)
        {
            Result<int> result = new Result<int>();

            List<Epm_TzAttachs> attachs = new List<Epm_TzAttachs>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取开工申请信息附件
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            string fileDataJsonTimer = Request.Form["fileDataJsonFileTimer"];//获取工期管理附件
            if (!string.IsNullOrEmpty(fileDataJsonTimer))
            {
                List<Epm_TzAttachs> attachsTimer = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJsonTimer);
                attachs.AddRange(attachsTimer);
            }

            string fileDataJsonCrossings = Request.Form["fileDataJsonFileCrossings"];//获取外部手续附件
            if (!string.IsNullOrEmpty(fileDataJsonCrossings))
            {
                List<Epm_TzAttachs> attachsCrossings = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJsonCrossings);
                attachs.AddRange(attachsCrossings);
            }
            
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                model.TzAttachsTime = attachs;
                result = proxy.UpdateTzProjectStartApplyNew(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工期和手续提交保存---如果是提交-修改开工申请状态为已提交
        /// </summary>
        /// <param name="model"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCrossingsAndTimeManage(Epm_TimeLimitAndProcedure model)
        {
            if (model.ProjectId == 0)
            {
                ResultView<int> view = new ResultView<int>();
                view.Flag = false;
                view.Message = "项目id不能为空";
                return Json(view);
            }
            #region 工期和手续提交
            //上传附件
            List<Epm_TzAttachs> attachs = new List<Epm_TzAttachs>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串--注意附件类型，手续没有类型，给一个默认的类型SHOUXU
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                var attach = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
                attachs.AddRange(attach);
                model.TzAttachs = attach;
            }
          
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTimeLimitAndCrossings(model);
            }
            #endregion

            return Json(result.ToResultView());
        }

        /// <summary>
        /// 开工申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            //Result<Epm_TzProjectStartApply> result = new Result<Epm_TzProjectStartApply>();
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    result = proxy.GetTzProjectStartApplyModel(id);
            //    if (result.Flag == EResultFlag.Success && result.Data != null)
            //    {
            //        return View(result.Data);
            //    }
            //    return View();
            //}
            Result<TzStartTenderingAndSupplyView> result = new Result<TzStartTenderingAndSupplyView>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzProjectStartApplyModelAndOther(id);

                //资金来源
                ViewBag.FundsSourceType = subjects[DictionaryType.CapitalSource].ToList().ToSelectList("Name", "No", false, result.Data == null ? "" : result.Data.TzProjectStartApply.FundsSourceType);
                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
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
        [AuthCheck(Module = WebCategory.TzStartsApply, Right = SystemRight.Check)]
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
                result = proxy.UpdateTzProjectStartApplyState(idList, state);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 开工申请工期管理手续--详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DetailStartTimeAndCrossings(long id)
        {
            Result<TzStartTenderingAndSupplyView> result = new Result<TzStartTenderingAndSupplyView>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                result = proxy.GetTzProjectStartApplyModelAndOther(id);

                //资金来源
                ViewBag.FundsSourceType = subjects[DictionaryType.CapitalSource].ToList().ToSelectList("Name", "No", false, result.Data == null ? "" : result.Data.TzProjectStartApply.FundsSourceType);
                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
            }
  
            return View(result.Data);
        }

        ////修改
        //public ActionResult CrossingsAndTimeManage(long projectId)
        //{
        //    Result<Epm_Project> result = new Result<Epm_Project>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        //加载数据字典
        //        List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TimeAndCrossingsType };
        //        var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
        //        ViewBag.drawingType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型

        //        result = proxy.GetProject(projectId);
        //    }
        //    ViewBag.ProjectId = projectId;
        //    ViewBag.IsCrossings = result.Data.IsCrossings;
        //    return View(result.Data);
        //}

      

       
    }
}