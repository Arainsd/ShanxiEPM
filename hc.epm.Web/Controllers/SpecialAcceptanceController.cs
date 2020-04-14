using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using hc.epm.ViewModel;
using hc.epm.UI.Common;
using hc.epm.Common;
using hc.Plat.Common.Global;
using hc.epm.Web.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class SpecialAcceptanceController : BaseWebController
    {

        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Browse)]
        public ActionResult Index(string title, string projectTitle, string startTime, string endTime, string state, long projectId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "专项验收列表";

            ViewBag.projectTitle = projectTitle;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.specialtitle = title;
            ViewBag.projectId = projectId;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            ViewBag.ApprovalState = typeof(ApprovalState).AsSelectList(true, state);

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };

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

            if (!string.IsNullOrWhiteSpace(state))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)state.ToEnumReq<ApprovalState>(),
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime startTimeValue;

                if (DateTime.TryParse(startTime, out startTimeValue))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "RecTime",
                        ExpValue = startTimeValue,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual,
                        ExpLogical = eLogicalOperator.And
                    });
                }
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime endTimeValue;

                if (DateTime.TryParse(endTime, out endTimeValue))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "RecTime",
                        ExpValue = endTimeValue.AddDays(1),
                        ExpOperater = eConditionOperator.LessThan,
                        ExpLogical = eLogicalOperator.And
                    });
                }
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetSpecialAcceptanceList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        [HttpGet]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.Title = "新增专项验收";
            ViewBag.CompanyName = CurrentUser.CompanyName;
            ViewBag.UserName = CurrentUser.UserName;

            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;

            GetMonitorType();
            GetSpecialAccepType();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Monitor };
                //var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                //ViewBag.TypeId = subjects[DictionaryType.Monitor].ToSelectList("Name", "Id", true);
            }

            return View();
        }

        [HttpPost]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Add)]
        public ActionResult Add(SpecialAcceptanceView model)
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

            // 获取上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = new List<Base_Files>();
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);
            }
            if (fileList.Any())
            {
                model.AttachList.AddRange(fileList);
            }

            string acceptanceDetailList = Request.Form["AcceptanceDetailList"];
            if (!string.IsNullOrWhiteSpace(acceptanceDetailList))
            {
                model.AcceptanceDetailList = JsonConvert.DeserializeObject<List<Epm_SpecialAcceptanceDetails>>(acceptanceDetailList);
            }
            if (!model.AcceptanceDetailList.Any())
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请填写专项验收要验收的项目明细！"
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.AddSpecialAcceptance(model);
                return Json(result.ToResultView());
            }
        }

        [HttpGet]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            GetMonitorType();
            GetSpecialAccepType();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetSpecialAcceptanceModel(id);

                //List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Monitor };
                //var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                //ViewBag.TypeId = subjects[DictionaryType.Monitor].ToSelectList("Name", "Id", true, result.Data == null ? "" : result.Data.TypeId.ToString());

                return View(result.Data);
            }
        }

        /// <summary>
        /// 修改专项验收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Modify)]
        public ActionResult Edit(SpecialAcceptanceView model)
        {
            if (model.Id <= 0)
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要修改的专项验收！"
                });
            }

            // 获取上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            List<Base_Files> fileList = new List<Base_Files>();
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);
            }

            if (fileList.Any())
            {
                model.AttachList.AddRange(fileList);
            }

            string acceptanceDetailList = Request.Form["AcceptanceDetailList"];
            if (!string.IsNullOrWhiteSpace(acceptanceDetailList))
            {
                model.AcceptanceDetailList = JsonConvert.DeserializeObject<List<Epm_SpecialAcceptanceDetails>>(acceptanceDetailList);
            }
            if (!model.AcceptanceDetailList.Any())
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请填写专项验收要验收的项目明细！"
                });
            }


            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.UpdateSpecialAcceptance(model);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 专项验收审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Check)]
        public ActionResult Audit(SpecialAcceptanceView model)
        {
            if (model.Id <= 0)
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要删除的专项验收！"
                });
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.AuditSpecialAccptance(model);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 废弃专项验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Invalid)]
        public ActionResult Discard(long id)
        {
            if (id <= 0)
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要废弃的专项验收！"
                });
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.DiscardSpecialAccptance(id);
                return Json(result.ToResultView());
            }
        }

        /// <summary>
        /// 删除专项验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            if (id <= 0)
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请选择要删除的专项验收！"
                });
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.DeleteSpecialAcceptanceById(id);
                return Json(result.ToResultView());
            }
        }


        [HttpGet]
       // [AuthCheck(Module = WebModule.SpecialAcceptance, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            ViewBag.Title = "查看专项验收";
            GetMonitorType();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetSpecialAcceptanceModel(id);
                //List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Monitor };
                //var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                //ViewBag.TypeId = subjects[DictionaryType.Monitor].ToSelectList("Name", "Id", true, result.Data == null ? "" : result.Data.TypeId.ToString());
                
                return View(result.Data);
            }
        }

        /// <summary>
        /// 获取专项验收项类型
        /// </summary>
        private void GetMonitorType()
        {
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.MonitorType };
            //    var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
            //    ViewBag.MonitorType = subjects[DictionaryType.MonitorType].ToSelectList("Name", "No", false);
            //}
        }

        /// <summary>
        /// 获取专项验收模板类型
        /// </summary>
        private void GetSpecialAccepType()
        {
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.TempleteType };
            //    var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
            //    ViewBag.TempleteType = subjects[DictionaryType.TempleteType].ToSelectList("Name", "No", false);
            //}
        }
    }
}