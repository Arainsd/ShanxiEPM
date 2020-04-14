using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
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
    /// 人员变更
    /// </summary>
    public class TzPeopleChgApplyController : BaseWebController
    {
        /// <summary>
        /// 建设工程项目管理人员变更申请流程
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="leader">负责人</param>
        /// <param name="companyName">申请单位</param>
        /// <param name="companyId"></param>
        /// <param name="workUnit">施工单位</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Browse)]
        public ActionResult Index(string title = "", string projectName = "", string leader = "", string companyName = "", string companyId = "", string workUnit = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.title = title;
            ViewBag.leader = leader;
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.workUnit = workUnit;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            Result<List<Epm_TzPeopleChgApply>> result = new Result<List<Epm_TzPeopleChgApply>>();

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
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = Convert.ToInt64(companyId);
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(leader))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Leader";
                    ce.ExpValue = "%" + leader + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }

                if (!string.IsNullOrEmpty(workUnit))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "WorkUnit";
                    ce.ExpValue = "%" + workUnit + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ApplyDate",
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

                        ExpName = "ApplyDate",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzPeopleChgApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        [AuthCheck(Module = WebModule.PersonnelChangeApply, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.Applicant = CurrentUser.RealName;
                ViewBag.ApplicantID = CurrentUser.UserId;
                ViewBag.CompanyName = CurrentUser.CompanyName;
                ViewBag.CompanyId = CurrentUser.CompanyId;
                ViewBag.ApplyDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.Title = "建设工程项目管理人员变更申请流程" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.DepartmentID = "";
                ViewBag.Department = "";
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
                                ViewBag.DepartmentID = companyInfo.Data.Id;
                                ViewBag.Department = companyInfo.Data.Name;
                            }
                        }
                    }
                }

            }
            return View();
        }

        /// <summary>
        /// 添加人员变更申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebModule.PersonnelChangeApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzPeopleChgApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            string tzPeopleChgApplyItem = Request.Form["tzPeopleChgApplyItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            if (!string.IsNullOrEmpty(tzPeopleChgApplyItem))
            {
                model.TzPeopleChgApplyItem = JsonConvert.DeserializeObject<List<Epm_TzPeopleChgApplyItem>>(tzPeopleChgApplyItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzPeopleChgApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改人员变更信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.PersonnelChangeApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzPeopleChgApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        /// <summary>
        /// 添加人员变更申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = WebModule.PersonnelChangeApply, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzPeopleChgApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            string tzPeopleChgApplyItem = Request.Form["tzPeopleChgApplyItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            if (!string.IsNullOrEmpty(tzPeopleChgApplyItem))
            {
                model.TzPeopleChgApplyItem = JsonConvert.DeserializeObject<List<Epm_TzPeopleChgApplyItem>>(tzPeopleChgApplyItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzPeopleChgApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取人员变更详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.PersonnelChangeApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzPeopleChgApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        /// <summary>
        /// 根据单位Id获取岗位信息列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPostById(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ResultView<int> view = new ResultView<int>();
                try
                {
                    var result = proxy.GetUserPostList(id);
                    if (result.Data != null)
                    {
                        var list = result.Data.Where(t => !string.IsNullOrEmpty(t.Post))
                       .Select(t => new { name = t.PostValue, no = t.Post })
                       .Distinct().ToList();

                        return Json(list);
                    }
                    else
                    {
                        view.Flag = false;
                        view.Message = "单位没有岗位信息";
                        return Json(view);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 根据单位Id和岗位信息获取人员信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUserListByPost(long id, string postName)
        {
            Result<List<Base_User>> result = new Result<List<Base_User>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserListByPost(id, postName);
            }

            return Json(result.Data);
        }
    }
}