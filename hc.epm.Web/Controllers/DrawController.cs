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
    public class DrawController : BaseWebController
    {
        /// <summary>
        /// 查询图纸列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string name = "", string state = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            //ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            //if (!string.IsNullOrEmpty(name))
            //{
            //    ce = new ConditionExpression();
            //    ce.ExpName = "Name";
            //    ce.ExpValue = "%" + name + "%";
            //    ce.ExpOperater = eConditionOperator.Like;
            //    ce.ExpLogical = eLogicalOperator.And;
            //    qc.ConditionList.Add(ce);
            //}
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + projectName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SubmitDate",
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

                    ExpName = "SubmitDate",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }

            if (!string.IsNullOrEmpty(state))
            {
                int statu = int.Parse(((ApprovalState)(Enum.Parse(typeof(ApprovalState), state))).GetValue().ToString());
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_Draw>> result = new Result<List<Epm_Draw>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDrawList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.ApprovalState = typeof(ApprovalState).AsSelectList(true, "", new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });
            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            return View(result.Data);
        }

        /// <summary>
        /// 上传图纸
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Draw };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.VersionNo = subjects[DictionaryType.Draw].ToSelectList("Name", "No", false, "FirstVersion");
            }

            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            ViewBag.CompanyId = ApplicationContext.Current.CompanyId;
            ViewBag.CompanyName = ApplicationContext.Current.CompanyName;
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            return View();
        }

        /// <summary>
        /// 上传图纸
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Add)]
        public ActionResult Add(Epm_Draw model)
        {
            ResultView<int> view = new ResultView<int>();

            //上传图纸
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            //if (string.IsNullOrEmpty(model.Name))
            //{
            //    view.Flag = false;
            //    view.Message = "名称不能为空";
            //    return Json(view);
            //}
            //if (string.IsNullOrEmpty(model.VersionOrder))
            //{
            //    view.Flag = false;
            //    view.Message = "版本号不能为空";
            //    return Json(view);
            //}
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddDraw(model, fileListFile);

                //var project = proxy.GetProject(model.ProjectId.Value).Data;
                //DateTime time = DateTime.Now;
                //if (fileListFile != null)
                //{
                //    foreach (var item in fileListFile)
                //    {
                //        if (string.IsNullOrEmpty(item.ImageType))
                //        {
                //            Bp_SendDate send = new Bp_SendDate();
                //            send.IsSend = false;
                //            send.Key = "2002040005";
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
        /// 修改图纸
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_Draw> result = new Result<Epm_Draw>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDrawModel(id);

                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Draw };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.VersionNo = subjects[DictionaryType.Draw].ToSelectList("Name", "No", false, result.Data.VersionNo);

            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改图纸（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Draw model)
        {
            ResultView<int> view = new ResultView<int>();

            //上传图纸
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            ////表单校验
            //if (string.IsNullOrEmpty(model.Name))
            //{
            //    view.Flag = false;
            //    view.Message = "名称不能为空";
            //    return Json(view);
            //}
            //if (string.IsNullOrEmpty(model.VersionOrder))
            //{
            //    view.Flag = false;
            //    view.Message = "版本号不能为空";
            //    return Json(view);
            //}
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDraw(model, fileListFile);

                //var project = proxy.GetProject(model.ProjectId.Value).Data;
                //DateTime time = DateTime.Now;
                //foreach (var item in fileListFile)
                //{
                //    if (string.IsNullOrEmpty(item.ImageType))
                //    {
                //        Bp_SendDate send = new Bp_SendDate();
                //        send.IsSend = false;
                //        send.Key = "2002040005";
                //        send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                //        send.Type = "12";
                //        send.Project = "BIM";
                //        send.KeyValue = project.ObjeId;
                //        send.UserName = CurrentUser.UserCode;
                //        send.CreateTime = time;
                //        send.OperateTime = time;
                //        send.OperateUserId = CurrentUser.UserId;
                //        send.OperateUserName = CurrentUser.UserName;
                //        send.CreateUserId = CurrentUser.UserId;
                //        send.CreateUserName = CurrentUser.UserName;
                //        proxy.AddSendDate(send);
                //    }
                //}
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看图纸详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Epm_Draw> result = new Result<Epm_Draw>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDrawModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 废弃图纸
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Invalid)]
        public ActionResult Archive(long id, string state)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeDrawState(id, state);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 审核/驳回图纸
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reject(long id, string state, string reason = "")
        {
            //权限检查
            if ((ApprovalState)(Enum.Parse(typeof(ApprovalState), state)) == ApprovalState.ApprFailure)
            {
                Helper.IsCheck(HttpContext, WebCategory.Draw.ToString(), SystemRight.UnCheck.ToString(), true);
            }
            else if ((ApprovalState)(Enum.Parse(typeof(ApprovalState), state)) == ApprovalState.ApprSuccess)
            {
                Helper.IsCheck(HttpContext, WebCategory.Draw.ToString(), SystemRight.Check.ToString(), true);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.RejectDraw(id, state, reason);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除（废弃/草稿）状态图纸
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.Draw, Right = SystemRight.Delete)]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            List<long> list = id.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteDrawByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 根据项目获取图纸数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByProjectId(long projectId)
        {
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (projectId != 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "projectId";
                ce.ExpValue = projectId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo.isAllowPage = false;

            Result<List<Epm_Draw>> result = new Result<List<Epm_Draw>>();
            int falg = 0;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDrawList(qc);
                if (result.Data.Count > 0)
                {
                    if (result.Data.Where(t => t.VersionNo == "BlueprintVersion").Count() > 0)
                    {
                        falg = 2;
                    }
                    else if (result.Data.Where(t => t.VersionNo == "FirstVersion").Count() > 0)
                    {
                        falg = 1;
                    }
                }
            }
            return Json(falg);
        }
    }
}