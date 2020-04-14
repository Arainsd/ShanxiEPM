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
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class ProjectOperateApplyController : BaseWebController
    {
        /// <summary>
        /// 加油站试运行申请列表
        /// </summary>
        /// <param name="applyName"></param>
        /// <param name="CompanyName"></param>
        /// <param name="userName"></param>
        /// <param name="time"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string companyIds = "", string userName = "", string startTime = "", string endTime = "", string state = "-1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.companyIds = companyIds;
            ViewBag.userName = userName;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.pageIndex = pageIndex;
            ViewBag.state = state;

            Result<List<Epm_TzProjectPolit>> result = new Result<List<Epm_TzProjectPolit>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
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

                if (!string.IsNullOrEmpty(companyIds))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = companyIds;
                    ce.ExpOperater = eConditionOperator.In;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CreateUserName";
                    ce.ExpValue = "%" + userName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
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
                    DateTime etime = Convert.ToDateTime(endTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "CreateTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                if (!string.IsNullOrEmpty(state) && state != "-1")
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = state;
                    ce.ExpOperater = eConditionOperator.In;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                result = proxy.GetProjectApprovalList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                var compamyList = proxy.GetAreaCompanyList();

                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                //ViewBag.BranchCompany = JsonConvert.SerializeObject(compamyList.Data);
            }
            
            return View(result.Data);
        }


        private List<Base_Company> SelectBranchCompany()
        {
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Owner",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "PreCode",
                ExpValue = "1133272570590793728",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "OrgType",
                ExpValue = "2",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "OrgState",
                ExpValue = "1",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
            }
            if (result.Flag == EResultFlag.Success && result.Data != null)
            {
                return result.Data;
            }
            else
            {
                return new List<Base_Company>();
            }
        }

        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Add)]
        public ActionResult AddApply()
        {
            return View();
        }

        /// <summary>
        /// 添加项目试运行申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzProjectPolit model)
        {
            ProjectApprovalView view = new ProjectApprovalView();
            view.ProjectPolit = model;
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            if (!string.IsNullOrEmpty(fileDataJson))
            {
                view.FileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddProjectApproval(view);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<ProjectApprovalView> result = new Result<ProjectApprovalView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectApprovalModel(id);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<ProjectApprovalView> result = new Result<ProjectApprovalView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.PolitFileType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.PolitFileType = subjects[DictionaryType.PolitFileType].ToList().ToSelectList("Name", "No", true);

                result = proxy.GetProjectApprovalModel(id);
                ViewBag.CompanyName = CurrentUser.CompanyName;
                ViewBag.CompanyId = CurrentUser.CompanyId;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 试运行申请编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.ProjectOperateApply, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzProjectPolit model)
        {
            ResultView<int> view = new ResultView<int>();
          
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
           

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.PolitFileType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //附件类型
                ViewBag.PolitFileType = subjects[DictionaryType.PolitFileType].ToList().ToSelectList("Name", "No", true);
                if (!string.IsNullOrEmpty(fileDataJson))
                {
                    model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
                }
              

                result = proxy.UpdateTzProjectPolit(model);


            }
            return Json(result.ToResultView());
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
                result = proxy.UpdateTzProjectPolitState(idList, state);
            }
            return Json(result.ToResultView());
        }

    }
}