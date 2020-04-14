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
    public class CompletionAcceptanceController : BaseWebController
    {
        /// <summary>
        /// 竣工验收列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string title = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.title = title;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;


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
            if (!string.IsNullOrEmpty(state))
            {
                int statu = int.Parse(((PreCompletionScceptanceState)(Enum.Parse(typeof(PreCompletionScceptanceState), state))).GetValue().ToString());
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_CompletionAcceptance>> result = new Result<List<Epm_CompletionAcceptance>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.ApprovalState = typeof(PreCompletionScceptanceState).AsSelectList(true);
            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            return View(result.Data);
        }

        /// <summary>
        /// 添加竣工验收
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Add)]
        public ActionResult Add(long projectId)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectModel(projectId);

                var itemResult = proxy.GetCompletionItem(projectId);
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();
                if (itemResult.Flag == EResultFlag.Success && itemResult.Data != null)
                {
                    list = itemResult.Data;
                }

                ViewBag.CompletionAcceptanceItemList = list;
                ViewBag.isAdd = true;
            }
            //ViewBag.AcceptanceResult = Enum<EnumState>.AsEnumerable().Where(i => i == EnumState.Must || i == EnumState.UnMust).ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            ViewBag.UserID = CurrentUser.UserId;
            ViewBag.UserName = CurrentUser.RealName;
            ViewBag.CompanyId = CurrentUser.CompanyId;
            ViewBag.CompanyName = CurrentUser.CompanyName;

            //项目数据
            ViewBag.ProjectData = result.Data;
            return View();
        }
        /// <summary>
        /// 添加竣工验收（提交方法）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_CompletionAcceptance model)
        {
            ResultView<int> view = new ResultView<int>();
            CompletionAcceptanceView complete = new CompletionAcceptanceView();
            complete.CompletionAcceptance = model;
            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "验收标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "验收内容不能为空";
                return Json(view);
            }

            string dataVisaCompany = Request.Form["Unit"];
            if (!string.IsNullOrEmpty(dataVisaCompany))
            {
                complete.CompletionRectifyCompanys = JsonConvert.DeserializeObject<List<Epm_CompletionRectifyCompany>>(dataVisaCompany);
            }

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            complete.BaseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompletionAcceptanceNew(complete);
                var project = proxy.GetProject(model.ProjectId.Value).Data;
                DateTime time = DateTime.Now;
                foreach (var item in complete.BaseFiles)
                {
                    if (string.IsNullOrEmpty(item.ImageType))
                    {
                        if (item.TableColumn == "YSD" || item.TableColumn == "JGT")
                        {
                            Bp_SendDate send = new Bp_SendDate();
                            send.IsSend = false;
                            if (item.TableColumn == "YSD")
                            {
                                send.Key = "2003020004";
                            }
                            if (item.TableColumn == "JGT")
                            {
                                send.Key = "2003020002";
                            }

                            send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                            send.Type = "12";
                            send.Project = "BIM";
                            send.KeyValue = project.ObjeId;
                            send.UserName = CurrentUser.UserCode;
                            send.CreateTime = time;
                            send.OperateTime = time;
                            send.OperateUserId = CurrentUser.UserId;
                            send.OperateUserName = CurrentUser.UserName;
                            send.CreateUserId = CurrentUser.UserId;
                            send.CreateUserName = CurrentUser.UserName;
                            proxy.AddSendDate(send);
                        }
                    }
                }
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改竣工验收
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<CompletionAcceptanceView> result = new Result<CompletionAcceptanceView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceModelNew(id);

                var project = proxy.GetProjectModelByTzId(result.Data.CompletionAcceptance.ProjectId.Value);

                //项目数据
                ViewBag.project = project.Data;

                ViewBag.UserID = CurrentUser.UserId;
                ViewBag.UserName = CurrentUser.RealName;
                ViewBag.CompanyId = CurrentUser.CompanyId;
                ViewBag.CompanyName = CurrentUser.CompanyName;

                var itemResult = proxy.GetCompletionItem(result.Data.CompletionAcceptance.ProjectId.Value);
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();
                if (itemResult.Flag == EResultFlag.Success && itemResult.Data != null)
                {
                    list = itemResult.Data;
                }

                ViewBag.CompletionAcceptanceItemList = list;
                ViewBag.isAdd = false;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改竣工验收（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_CompletionAcceptance model)
        {
            ResultView<int> view = new ResultView<int>();
            CompletionAcceptanceView complete = new CompletionAcceptanceView();
            complete.CompletionAcceptance = model;
            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "验收标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "验收内容不能为空";
                return Json(view);
            }
            //List<Epm_CompletionRectifyCompany> completionRectifyCompanys = new List<Epm_CompletionRectifyCompany>();
            string dataVisaCompany = Request.Form["Unit"];
            if (!string.IsNullOrEmpty(dataVisaCompany))
            {
                complete.CompletionRectifyCompanys = JsonConvert.DeserializeObject<List<Epm_CompletionRectifyCompany>>(dataVisaCompany);
            }

            //List<Base_Files> baseFiles = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            complete.BaseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateCompletionAcceptanceNew(complete);

                var project = proxy.GetProject(model.ProjectId.Value).Data;
                DateTime time = DateTime.Now;
                //foreach (var item in complete.BaseFiles)
                //{
                //    if (string.IsNullOrEmpty(item.ImageType))
                //    {
                //        if (item.TableColumn == "YSD" || item.TableColumn == "JGT")
                //        {
                //            Bp_SendDate send = new Bp_SendDate();
                //            send.IsSend = false;
                //            if (item.TableColumn == "YSD")
                //            {
                //                send.Key = "2003020004";
                //            }
                //            if (item.TableColumn == "JGT")
                //            {
                //                send.Key = "2003020002";
                //            }

                //            send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                //            send.Type = "12";
                //            send.Project = "BIM";
                //            send.KeyValue = project.ObjeId;
                //            send.UserName = CurrentUser.UserCode;
                //            send.CreateTime = time;
                //            send.OperateTime = time;
                //            send.OperateUserId = CurrentUser.UserId;
                //            send.OperateUserName = CurrentUser.UserName;
                //            send.CreateUserId = CurrentUser.UserId;
                //            send.CreateUserName = CurrentUser.UserName;
                //            proxy.AddSendDate(send);
                //        }
                //    }
                //}
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看竣工验收详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<CompletionAcceptanceView> result = new Result<CompletionAcceptanceView>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompletionAcceptanceModelNew(id);

                var project = proxy.GetProjectModelByTzId(result.Data.CompletionAcceptance.ProjectId.Value);

                //项目数据
                ViewBag.project = project.Data;

                var itemResult = proxy.GetCompletionItem(result.Data.CompletionAcceptance.ProjectId.Value);
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();
                if (itemResult.Flag == EResultFlag.Success && itemResult.Data != null)
                {
                    list = itemResult.Data;
                }

                ViewBag.CompletionAcceptanceItemList = list;
                ViewBag.isAdd = false;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Check)]
        public ActionResult Audit(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeCompletionAcceptanceState(id, PreCompletionScceptanceState.ApprovalSuccess, "");
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.UnCheck)]
        public ActionResult Reject(long id, string reason)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeCompletionAcceptanceState(id, PreCompletionScceptanceState.ApprovalFailure, reason);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Invalid)]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeCompletionAcceptanceState(id, PreCompletionScceptanceState.ApprovalFailure, "");
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Delete)]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            if (id.Length > 0)
            {

                List<long> list = id.SplitString(",").ToLongList();
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                {
                    result = proxy.DeleteCompletionAcceptanceByIds(list);
                }
                return Json(result.ToResultView());
            }
            else
            {
                view.Flag = false;
                view.Message = "该竣工验收信息已被删除或者不存在！";
                return Json(view);
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.CompletionApply, Right = SystemRight.Check)]
        public ActionResult UpdateState(string ids, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(ids))
            {
                view.Flag = false;
                view.Message = "请选择要删除的数据";
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
                result = proxy.UpdateCompletionAcceptanceState(idList, state);
            }
            return Json(result.ToResultView());
        }
    
}
}