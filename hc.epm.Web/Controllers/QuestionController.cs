using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.ViewModel;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.Common;
using hc.Plat.Common.Global;
using hc.epm.Web.ClientProxy;
using hc.epm.DataModel.Basic;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class QuestionController : BaseWebController
    {
        /// <summary>
        /// 问题列表
        /// </summary>
        /// <param name="title">问题表存</param>
        /// <param name="projectTitle">所属项目</param>
        /// <param name="businessType">问题类型</param>
        /// <param name="projectId">项目名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页行数</param>
        /// <returns></returns>
        public ActionResult Index(string title, string projectTitle, string businessType, long projectId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "问题列表";
            ViewBag.questionTitle = title;
            ViewBag.projectTitle = projectTitle;
            ViewBag.businessType = businessType;
            ViewBag.projectId = projectId;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            ViewBag.BusinessType = typeof(BusinessType).AsSelectList(true, businessType, new List<string>() { BusinessType.Approver.ToString(), BusinessType.SecurityTrain.ToString(), BusinessType.QualityTrain.ToString(), BusinessType.QualityCheck.ToString(), BusinessType.Special.ToString() });
            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            if (projectId > 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
            }
            if (!string.IsNullOrWhiteSpace(projectTitle))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = string.Format("%{0}%", projectTitle),
                    ExpOperater = eConditionOperator.Like,
                    ExpLogical = eLogicalOperator.And
                });
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Title",
                    ExpValue = string.Format("%{0}%", title),
                    ExpOperater = eConditionOperator.Like,
                    ExpLogical = eLogicalOperator.And
                });
            }

            if (!string.IsNullOrWhiteSpace(businessType))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "BusinessTypeNo",
                    ExpValue = businessType,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
            }
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetQuestionList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 新增问题
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Add(string businType = "Question", long projectId = 0, long BusinessId = 0, string componentIds = "[]")
        {
            ViewBag.Title = "新增问题";
            ViewBag.BusinessType = typeof(BusinessType).AsSelectList(true, businType);
            ViewBag.BusinessId = BusinessId;
            ViewBag.ComponentIds = componentIds;

            if (projectId == 0)
            {
                ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
                ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            }
            else
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                {
                    var project = proxy.GetProjectModel(projectId).Data;

                    ViewBag.ProjectId = project.Id.ToString();
                    ViewBag.ProjectName = project.Name;
                }
            }

            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.CompanyName = CurrentUser.CompanyName;

            GetProblemType();
            return View();
        }

        /// <summary>
        /// 新增问题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(QuestionView model)
        {
            if (model.ProjectId <= 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择项目名称！"
                });
            }

            #region 附件处理

            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            List<Base_Files> fileList = new List<Base_Files>();
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);
            }
            if (fileList.Any())
            {
                model.Attachs = fileList;
            }

            #endregion

            #region 协同人员处理
            //string users = Request.Form["QuestionUser"];
            //List<Epm_QuestionUser> questionUsers = new List<Epm_QuestionUser>();
            //if (!string.IsNullOrEmpty(users))
            //{
            //    questionUsers = JsonConvert.DeserializeObject<List<Epm_QuestionUser>>(users);
            //}
            //if (questionUsers.Any())
            //{
            //    model.QuestionUsers = questionUsers;
            //}
            //else
            //{
            //    model.QuestionUsers = new List<Epm_QuestionUser>();
            //}
            #endregion

            #region 关联模型处理

            List<Epm_QuestionBIM> bimList = new List<Epm_QuestionBIM>();
            string bims = Request.Form["ComponentIds"];
            if (!string.IsNullOrWhiteSpace(bims))
            {
                bimList = JsonConvert.DeserializeObject<List<Epm_QuestionBIM>>(bims);
            }
            if (bimList.Any())
            {
                model.QuestionBims = bimList;
            }
            else
            {
                model.QuestionBims = new List<Epm_QuestionBIM>();
            }

            #endregion

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.AddQuestion(model);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long id)
        {
            if (id <= 0)
            {
                return Json(new ResultView<bool>()
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要删除的问题！"
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.DeleteQuestion(id);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 关闭问题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isAccident"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Close(long id, bool isAccident)
        {
            if (id <= 0)
            {
                return Json(new ResultView<bool>()
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要关闭的问题！"
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.CloseQuestion(id, isAccident);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 回复问题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reply(Epm_QuestionTrack model)
        {
            string errorMsg = "";
            if (model.QuestionId <= 0)
            {
                errorMsg = "请选择要回复的问题！";
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                errorMsg = "请填写回复内容！";
            }

            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                return Json(new ResultView<bool>()
                {
                    Flag = false,
                    Data = false,
                    Message = errorMsg
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.ReplyQuestion(model);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 获取接收单位下拉数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompany(long projectId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                var companyResult = proxy.GetProjectCompanyList(projectId);
                companyResult.Data = companyResult.Data.Where(t => t.CompanyId.HasValue && t.CompanyId != CurrentUser.CompanyId).ToList();
                companyResult.Data.ForEach(p =>
                {
                    p.Id = p.CompanyId.Value;
                });

                Epm_ProjectCompany comList = new Epm_ProjectCompany();

                var projectInfo = proxy.GetProject(projectId);

                comList.Id = projectInfo.Data.CompanyId.Value;
                comList.CompanyId = projectInfo.Data.CompanyId.Value;
                comList.CompanyName = projectInfo.Data.CompanyName;
                companyResult.Data.Add(comList);


                List<Epm_ProjectCompany> comLists = new List<Epm_ProjectCompany>();
                if (companyResult.Data.Count > 0)
                {
                    foreach (var item in companyResult.Data)
                    {
                        var isCom = comLists.Where(t => t.CompanyId == item.CompanyId);
                        if (isCom.Count() == 0)
                        {
                            Epm_ProjectCompany com = new Epm_ProjectCompany();
                            com.CompanyId = item.CompanyId;
                            com.CompanyName = item.CompanyName;
                            com.Id = com.CompanyId.Value;

                            comLists.Add(com);
                        }
                    }

                    companyResult.Data = comLists;
                }

                return Json(companyResult.Data);
            }
        }
        /// <summary>
        /// 获取问题详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCheck(Module = WebModule.ProblemManage, Right = SystemRight.Reply)]
        [AuthCheck(Module = WebModule.ProblemManage, Right = SystemRight.Close)]
        public ActionResult Detail(long id)
        {
            ViewBag.Title = "查看问题";
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserId = CurrentUser.UserId;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<Epm_QuestionBIM> list = proxy.GetComponentListByQuestionId(id).Data;
                ViewBag.PositionJson = JsonConvert.SerializeObject(list);
                ViewBag.bimId = (list != null && list.Count > 0) ? list[0].BIMId.ToString() : "[]";
                var result = proxy.GetQuestionModel(id);
                return View(result.Data);
            }
        }

        public ActionResult RelationBIM(long bimId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var bimModelResult = proxy.GetBimModel(bimId);
                if (bimModelResult.Flag == EResultFlag.Success && bimModelResult.Data != null)
                {
                    ViewBag.BIMAddress = bimModelResult.Data.BIMAddress;
                }
            }
            return View();
        }
        /// <summary>
        /// 根据问题id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult GetQuestionComponentList(long questionId)
        {
            Result<List<Epm_QuestionBIM>> result = new Result<List<Epm_QuestionBIM>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetComponentListByQuestionId(questionId);
                return Json(result.Data);
            }
        }

        /// <summary>
        /// 获取问题类型下拉框数据
        /// </summary>
        private void GetProblemType()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProblemType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.ProblemTypeNo = subjects[DictionaryType.ProblemType].ToSelectList("Name", "No", true);
            }
        }
    }
}