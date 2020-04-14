using hc.epm.Admin.ClientProxy;
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
    public class TrainController : BaseWebController
    {

        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Browse)]
        
        public ActionResult IndexAQ(string projectName = "", string name = "", string startTime = "", string endTime = "", string state = "",string trainCompanyName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.trainCompanyName = trainCompanyName;
            ViewBag.state = typeof(ConfirmState).AsSelectList(true, state);

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(trainCompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "TrainCompanyName";
                ce.ExpValue = "%" + trainCompanyName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "projectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "StartTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "EndTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = Enum.Parse(typeof(ConfirmState), state);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)approvalState,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "TrainTypeNo",
                ExpValue = "AQPX",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            if (base.CurrentUser.CompanyType != "Owner")//
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CompanyIds",
                    ExpValue = base.CurrentUser.CompanyId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<TrainView>> result = new Result<List<TrainView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                result = proxy.GetTrainList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);

        }
        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Add)]
        public ActionResult AddAQ()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            ViewBag.Title = "新增安全培训";
            return View();
        }
        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Add)]
        public ActionResult AddAQ(TrainView model)
        {

            #region ///校验
            ResultView<int> view = new ResultView<int>();
            if (model.ProjectId == null || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.CompanyNames) || string.IsNullOrEmpty(model.CompanyIds))
            //{
            //    view.Flag = false;
            //    view.Message = "参与培训单位不可为空";
            //    return Json(view);
            //}
            if (model.TrainUserId == null || string.IsNullOrWhiteSpace(model.TrainUserName))
            {
                view.Flag = false;
                view.Message = "培训人员不可为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null || string.IsNullOrWhiteSpace(model.TrainCompanyName))
            {
                view.Flag = false;
                view.Message = "培训单位不可为空";
                return Json(view);
            }
            if (model.StartTime == null)
            {
                view.Flag = false;
                view.Message = "培训开始时间不可为空";
                return Json(view);
            }
            if (model.EndTime == null)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可为空";
                return Json(view);
            }
            if (model.EndTime < model.StartTime)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可小于开始时间";
                return Json(view);
            }
            #endregion
            model.TrainTypeName = "安全培训";
            model.TrainTypeNo = "AQPX";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            Result<int> result = new Result<int>();
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTrain(model, fileList);
            }
            return Json(result.ToResultView());
        }
        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Info)]
        public ActionResult DetailAQ(long id)
        {
            ViewBag.Title = "查看安全培训";
            Result<TrainView> result = new Result<TrainView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTrainModel(id);
            }
            return View(result.Data);
        }
        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Modify)]
        public ActionResult EditAQ(long id)
        {
            Result<TrainView> result = new Result<TrainView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTrainModel(id);
            }
            return View(result.Data);
        }

        [HttpPost]
        //[AuthCheck(Module = WebModule.SecurityTrian, Right = SystemRight.Modify)]
        public ActionResult EditAQ(TrainView model)
        {
            model.TrainTypeName = "安全培训";
            model.TrainTypeNo = "AQPX";
            #region ///校验
            ResultView<int> view = new ResultView<int>();
            if (model.Id == 0)
            {
                view.Flag = false;
                view.Message = "Id不可为空";
                return Json(view);
            }
            if (model.ProjectId == null || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.CompanyNames) || string.IsNullOrEmpty(model.CompanyIds))
            //{
            //    view.Flag = false;
            //    view.Message = "参与培训单位不可为空";
            //    return Json(view);
            //}
            if (model.TrainUserId == null || string.IsNullOrWhiteSpace(model.TrainUserName))
            {
                view.Flag = false;
                view.Message = "培训人员不可为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null || string.IsNullOrWhiteSpace(model.TrainCompanyName))
            {
                view.Flag = false;
                view.Message = "培训单位不可为空";
                return Json(view);
            }
            if (model.StartTime == null)
            {
                view.Flag = false;
                view.Message = "培训开始时间不可为空";
                return Json(view);
            }
            if (model.EndTime == null)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可为空";
                return Json(view);
            }
            if (model.EndTime < model.StartTime)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可小于开始时间";
                return Json(view);
            }
            #endregion

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTrain(model, fileList);
            }
            return Json(result.ToResultView());
        }
 
        [HttpPost]
        public ActionResult UpdateState(long id, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(state))
            {
                view.Flag = false;
                view.Message = "状态不能为空";
                return Json(view);
            }
            ////判断权限
            //if ((ConfirmState)Enum.Parse(typeof(ConfirmState), state) == ConfirmState.ApprSuccess)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.Check.ToString(), true);
            //else if ((ConfirmState)Enum.Parse(typeof(ConfirmState), state) == ConfirmState.ApprFailure)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.UnCheck.ToString(), true);
            //else if ((ApprovalState)Enum.Parse(typeof(ConfirmState), state) == ConfirmState.Discarded)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.Invalid.ToString(), true);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTrainState(id, state);
            }
            return Json(result.ToResultView());
        }
        [HttpPost]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteTrainByIds(new List<long> { id });
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="TrainTypeName">类型名称</param>
        /// <returns></returns>
        public ActionResult GetTemplateList(string TrainTypeName,string name,int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrWhiteSpace(TrainTypeName))
            {
                view.Flag = false;
                view.Message = "类型名称不可为空";
                return Json(view);
            }
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(TrainTypeName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "TemplateTypeName";
                ce.ExpValue = "%" + TrainTypeName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Browse)]
        public ActionResult IndexZL(string projectName = "", string name = "", string startTime = "", string endTime = "", string state = "", string trainCompanyName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.trainCompanyName = trainCompanyName;
            ViewBag.state = typeof(ConfirmState).AsSelectList(true, state);

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(trainCompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "TrainCompanyName";
                ce.ExpValue = "%" + trainCompanyName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "projectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "StartTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "EndTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = Enum.Parse(typeof(ConfirmState), state);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)approvalState,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "TrainTypeNo",
                ExpValue = "ZLPX",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<TrainView>> result = new Result<List<TrainView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                result = proxy.GetTrainList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Add)]
        public ActionResult AddZL()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            return View();
        }
        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult AddZL(TrainView model)
        {

            #region ///校验
            ResultView<int> view = new ResultView<int>();
            if (model.ProjectId == null || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.CompanyNames) || string.IsNullOrEmpty(model.CompanyIds))
            //{
            //    view.Flag = false;
            //    view.Message = "参与培训单位不可为空";
            //    return Json(view);
            //}
            if (model.TrainUserId == null || string.IsNullOrWhiteSpace(model.TrainUserName))
            {
                view.Flag = false;
                view.Message = "培训人员不可为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null || string.IsNullOrWhiteSpace(model.TrainCompanyName))
            {
                view.Flag = false;
                view.Message = "培训单位不可为空";
                return Json(view);
            }
            if (model.StartTime == null)
            {
                view.Flag = false;
                view.Message = "培训开始时间不可为空";
                return Json(view);
            }
            if (model.EndTime == null)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可为空";
                return Json(view);
            }
            if (model.EndTime < model.StartTime)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可小于开始时间";
                return Json(view);
            }
            #endregion
            model.TrainTypeName = "质量培训";
            model.TrainTypeNo = "ZLPX";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            Result<int> result = new Result<int>();
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTrain(model, fileList);
            }
            return Json(result.ToResultView());
        }

        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Modify)]
        public ActionResult EditZL(long id)
        {
            Result<TrainView> result = new Result<TrainView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTrainModel(id);
            }
            return View(result.Data);
        }

        [HttpPost]
        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Modify)]
        public ActionResult EditZL(TrainView model)
        {
            model.TrainTypeName = "质量培训";
            model.TrainTypeNo = "ZLPX";
            #region ///校验
            ResultView<int> view = new ResultView<int>();
            if (model.ProjectId == null || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不可为空";
                return Json(view);
            }
            if (model.Id == 0)
            {
                view.Flag = false;
                view.Message = "Id不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "内容不可为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.CompanyNames) || string.IsNullOrEmpty(model.CompanyIds))
            //{
            //    view.Flag = false;
            //    view.Message = "参与培训单位不可为空";
            //    return Json(view);
            //}
            if (model.TrainUserId == null || string.IsNullOrWhiteSpace(model.TrainUserName))
            {
                view.Flag = false;
                view.Message = "培训人员不可为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null || string.IsNullOrWhiteSpace(model.TrainCompanyName))
            {
                view.Flag = false;
                view.Message = "培训单位不可为空";
                return Json(view);
            }
            if (model.StartTime == null)
            {
                view.Flag = false;
                view.Message = "培训开始时间不可为空";
                return Json(view);
            }
            if (model.EndTime == null)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可为空";
                return Json(view);
            }
            if (model.EndTime < model.StartTime)
            {
                view.Flag = false;
                view.Message = "培训结束时间不可小于开始时间";
                return Json(view);
            }
            #endregion

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTrain(model, fileList);
            }
            return Json(result.ToResultView());
        }
        //[AuthCheck(Module = WebModule.QualityTrain, Right = SystemRight.Info)]
        public ActionResult DetailZL(long id)
        {
            Result<TrainView> result = new Result<TrainView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTrainModel(id);
            }
            return View(result.Data);
        }
    }
}