using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
//using Microsoft.Office.Interop.MSProject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class PlanController : BaseWebController
    {
        // GET: Plan
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Browse)]
        public ActionResult Index(long projectId = 0)
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;

            if (projectId == 0)
            {
                projectId = Convert.ToInt64(ViewBag.ProjectId);
            }
            List<Epm_Plan> list = new List<Epm_Plan>();
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            Result<List<PlanView>> planViewList = new Result<List<PlanView>>();
            QueryCondition qc = new QueryCondition();
            if (projectId > 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "projectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And,
                });
            }
            qc.PageInfo.isAllowPage = false;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanList(qc);

                planViewList = proxy.GetPlanViewList(projectId);
            }
            if (list.Any())
            {
                //获取批次号下拉数据
                list = result.Data.GroupBy(p => new
                {
                    p.BatchNo
                }).Select(p => p.First()).ToList();
                ViewBag.BatchNoList = list.ToSelectList("BatchNo", "Id", true);
            }
            //施工计划树形列表
            ViewBag.planViewList = JsonConvert.SerializeObject(planViewList.Data);

            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;

            return View(result.Data);
        }

        /// <summary>
        /// 根据项目名称查询批次号下拉列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public ActionResult GetBatchNoList(string projectName)
        {
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.PageInfo.isAllowPage = false;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanList(qc);
            }
            var list = result.Data.GroupBy(p => new { p.BatchNo }).Select(p => p.First()).ToList();
            if (list.Count == 0)
            {
                list = new List<Epm_Plan>();
            }
            //获取批次号下拉数据

            var BatchNoList = list.ToSelectList("BatchNo", "Id", true);

            return Json(BatchNoList);
        }

        /// <summary>
        /// 添加计划
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            return View();
        }

        /// <summary>
        /// 添加计划（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Add)]
        public ActionResult Add(Epm_Plan model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "计划名称不能为空";
                return Json(view);
            }
            if (!model.StartTime.HasValue || model.StartTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "开始时间不能为空";
                return Json(view);
            }
            if (!model.EndTime.HasValue || model.EndTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "结束时间不能为空";
                return Json(view);
            }
            if (model.StartTime.Value > model.EndTime.Value)
            {
                view.Flag = false;
                view.Message = "开始时间不能大于结束时间";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.BatchNo))
            {
                view.Flag = false;
                view.Message = "批次号不能为空";
                return Json(view);
            }
            if (model.MilepostId == 0 || string.IsNullOrEmpty(model.MilepostName))
            {
                view.Flag = false;
                view.Message = "工程节点不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.PlanContent))
            {
                view.Flag = false;
                view.Message = "计划说明不能为空";
                return Json(view);
            }

            List<Epm_PlanComponent> planComponentIds = new List<Epm_PlanComponent>();
            string dataComponentIds = Request.Form["PlanBim"];
            if (!string.IsNullOrWhiteSpace(dataComponentIds))
            {
                planComponentIds = JsonConvert.DeserializeObject<List<Epm_PlanComponent>>(dataComponentIds);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                DateTime startTime = Convert.ToDateTime(model.StartTime.Value.ToString("yyyy-MM-dd") + " 00:00:00");
                DateTime endTime = Convert.ToDateTime(model.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                TimeSpan ts = endTime - startTime;

                model.State = (int)(ApprovalState.WaitAppr);
                model.BuildDays = ts.Days;

                result = proxy.AddPlan(model, planComponentIds);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<PlanView> result = new Result<PlanView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanModel(id);

                if (result.Data.Plan.ParentId.Value == 0)
                {
                    ViewBag.ParName = "";
                }
                else {
                    //所属父级名称
                    ViewBag.ParName = proxy.GetPlanById(result.Data.ParentId.Value).Data.Name;
                }
            }

            return View(result.Data);
        }

        /// <summary>
        /// 修改计划（提交数据）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Plan model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "计划名称不能为空";
                return Json(view);
            }
            if (!model.StartTime.HasValue || model.StartTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "开始时间不能为空";
                return Json(view);
            }
            if (!model.EndTime.HasValue || model.EndTime.Value == DateTime.MinValue)
            {
                view.Flag = false;
                view.Message = "结束时间不能为空";
                return Json(view);
            }
            if (model.StartTime.Value > model.EndTime.Value)
            {
                view.Flag = false;
                view.Message = "开始时间不能大于结束时间";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.BatchNo))
            {
                view.Flag = false;
                view.Message = "批次号不能为空";
                return Json(view);
            }
            if (model.MilepostId == 0 || string.IsNullOrEmpty(model.MilepostName))
            {
                view.Flag = false;
                view.Message = "工程节点不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.PlanContent))
            {
                view.Flag = false;
                view.Message = "计划说明不能为空";
                return Json(view);
            }
            List<Epm_PlanComponent> planComponentIds = new List<Epm_PlanComponent>();
            string dataComponentIds = Request.Form["PlanBim"];
            if (!string.IsNullOrWhiteSpace(dataComponentIds))
            {
                planComponentIds = JsonConvert.DeserializeObject<List<Epm_PlanComponent>>(dataComponentIds);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                DateTime startTime = Convert.ToDateTime(model.StartTime.Value.ToString("yyyy-MM-dd") + " 00:00:00");
                DateTime endTime = Convert.ToDateTime(model.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                TimeSpan ts = endTime - startTime;
                model.BuildDays = ts.Days;
                model.State = (int)(ApprovalState.WaitAppr);

                result = proxy.UpdatePlan(model, planComponentIds);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 施工计划详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<PlanView> result = new Result<PlanView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanModel(id);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 选择父级计划
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectParent()
        {
            return View();
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmAudit(string batchNo)
        {
            Result<int> result = new Result<int>();
            ApprovalState state = ApprovalState.WaitAppr;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangePlanState(batchNo, state, "");
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除施工计划（审核通过的计划，不能删除）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Delete)]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            List<long> list = id.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //如果已审核通过的计划，不能删除
                result = proxy.DeletePlanByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃施工计划
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Invalid)]
        public ActionResult Archive(string batchNo)
        {
            Result<int> result = new Result<int>();
            ApprovalState state = ApprovalState.Discarded;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangePlanState(batchNo, state, "");
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 驳回施工计划
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.UnCheck)]
        public ActionResult Reject(string batchNo, string reason)
        {
            Result<int> result = new Result<int>();
            ApprovalState state = ApprovalState.ApprFailure;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangePlanState(batchNo, state, reason);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 审核施工计划
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Plan, Right = SystemRight.Check)]
        public ActionResult Audit(string batchNo)
        {
            Result<int> result = new Result<int>();
            ApprovalState state = ApprovalState.ApprSuccess;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangePlanState(batchNo, state, "");
            }
            return Json(result.ToResultView());
        }

        #region 获取计划名称树形列表

        public ActionResult GetPlanDetailsTree(long pId = 0, bool last = true, string chk = "", long projectId = 0)
        {
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;

            if (projectId > 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "projectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And,
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanList(qc);
            }
            var list = result.Data;
            list.Insert(0, new Epm_Plan() { Id = 0, ParentId = -1, Name = "根节点" });

            var first = list.FirstOrDefault(i => i.Id == pId);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = chk == first.Id.ToString();
            rootTree.data = new { id = first.Id.ToString() };
            rootTree.name = first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list);
            if (!last)
            {
                tree = createTreeNoLast(first.Id, list, chk);
            }
            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet);
        }
        private List<RightNode> createTree(long parentId, List<Epm_Plan> allList)
        {
            List<RightNode> list = new List<RightNode>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    RightNode node = new RightNode();
                    node.checkboxValue = item.Id.ToString();
                    node.@checked = false;
                    node.data = new { id = item.Id.ToString() };
                    node.name = item.Name;
                    node.spread = true;

                    var iteratorList = createTree(item.Id, allList);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }
        /// <summary>
        /// 不包含 末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <param name="chk"></param>
        /// <returns></returns>
        private List<RightNode> createTreeNoLast(long parentId, List<Epm_Plan> allList, string chk = "")
        {
            List<RightNode> list = new List<RightNode>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    RightNode node = new RightNode();
                    node.checkboxValue = item.Id.ToString();
                    node.@checked = false;
                    if (item.Id.ToString() == chk)
                    {
                        node.@checked = true;
                    }
                    node.data = new { id = item.Id.ToString() };
                    node.name = item.Name;
                    node.spread = true;

                    var iteratorList = createTreeNoLast(item.Id, allList, chk);
                    node.children = iteratorList;
                    var ccp = allList.Where(i => i.ParentId == item.Id).ToList();
                    if (ccp.Count() < 1)
                    {
                        continue;
                    }
                    list.Add(node);
                }
            }
            return list;
        }
        #endregion

        /// <summary>
        /// 导入页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            return View();
        }

        /// <summary>
        /// project文件读取
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ProjectId"></param>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectImport(FormCollection fc)
        {
            //string ProjectId = fc["ProjectId"];
            //string ProjectName = fc["ProjectName"];

            ////上传文件保存地址
            //string upLoadUrl = ConfigurationManager.AppSettings["upLoadUrl"];
            //if (Request.Files.Count < 1)
            //{
            //    return Json(new ResultView<bool>
            //    {
            //        Flag = false,
            //        Data = false,
            //        Message = "请上传附件！"
            //    });
            //}
            //HttpPostedFileBase postedFileBase = Request.Files[0];

            //string path = postedFileBase.FileName;
            //string str = Path.GetExtension(path);

            //if (!Directory.Exists(upLoadUrl))
            //{
            //    Directory.CreateDirectory(upLoadUrl);
            //}

            //string pathStr = Server.MapPath(upLoadUrl + path);
            //postedFileBase.SaveAs(pathStr);
            //// TODO:将文件保存到站点临时目录下，并读取文件并解析

            //if (str == ".mpp") //读取project文件
            //{
            //    List<PlanView> list = new List<PlanView>();
            //    Result<int> result = new Result<int>();
            //    ApplicationClass prj = new ApplicationClass();
            //    #region 读取Project文件内容
            //    prj.FileOpen(pathStr, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, PjPoolOpen.pjPoolReadOnly, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            //    foreach (Project proj in prj.Projects)
            //    {
            //        foreach (Task task in proj.Tasks)
            //        {
            //            PlanView planView = new PlanView();
            //            if (task != null)
            //            {
            //                DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(task.Start).ToString("yyyy-MM-dd") + " 00:00:00");
            //                DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(task.Finish).ToString("yyyy-MM-dd") + " 23:59:59");
            //                TimeSpan ts = endTime - startTime;

            //                planView.Name = task.Name;
            //                planView.StartTime = Convert.ToDateTime(task.Start);
            //                planView.EndTime = Convert.ToDateTime(task.Finish);
            //                planView.State = int.Parse((ApprovalState.Enabled).GetValue().ToString());
            //                planView.BuildDays = ts.Days;
            //                planView.WBS = task.WBS.ToString();
            //                planView.OutlineParent = task.OutlineParent.WBS.ToString();
            //                planView.iTaskLevel = task.OutlineLevel;
            //                list.Add(planView);
            //            }
            //        }
            //    }
            //    #endregion

            //    if (list.Count > 0)
            //    {
            //        //生成批次号
            //        string batchNo = DateTime.Now.ToString("yyyyMMdd") + Utils.Number(3, true);
            //        Epm_Plan temp = null;

            //        #region  1、保存一级任务到数据库
            //        //查询一级任务
            //        var listlevelOne = list.Where(t => t.iTaskLevel == 1).ToList();
            //        if (listlevelOne.Count > 0)
            //        {
            //            for (int i = 0; i < listlevelOne.Count; i++)
            //            {
            //                temp = new Epm_Plan();
            //                temp.Name = listlevelOne[i].Name;
            //                temp.StartTime = listlevelOne[i].StartTime;
            //                temp.EndTime = listlevelOne[i].EndTime;
            //                temp.State = int.Parse((ApprovalState.Enabled).GetValue().ToString());
            //                temp.ParentId = 0;
            //                temp.BuildDays = listlevelOne[i].BuildDays;
            //                temp.ProjectId = ProjectId.ToLongReq();
            //                temp.ProjectName = ProjectName;
            //                temp.BatchNo = batchNo;
            //                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //                {
            //                    result = proxy.AddPlan(temp, null);
            //                }
            //            }
            //        }
            //        #endregion

            //        #region 2、保存其他级别任务到数据库
            //        //查询其他级别任务
            //        var listlevelNotOne = list.Where(t => t.iTaskLevel != 1).ToList();
            //        if (listlevelNotOne.Count > 0)
            //        {
            //            for (int i = 0; i < listlevelNotOne.Count; i++)
            //            {
            //                temp = new Epm_Plan();
            //                temp.Name = listlevelNotOne[i].Name;
            //                temp.StartTime = listlevelNotOne[i].StartTime;
            //                temp.EndTime = listlevelNotOne[i].EndTime;
            //                temp.State = int.Parse((ApprovalState.Enabled).GetValue().ToString());
            //                temp.BuildDays = listlevelNotOne[i].BuildDays;
            //                temp.ProjectId = ProjectId.ToLongReq();
            //                temp.ProjectName = ProjectName;
            //                temp.BatchNo = batchNo;

            //                string ParentOutLine = listlevelNotOne[i].OutlineParent;

            //                //查询父级任务
            //                var plist = list.Where(t => t.WBS == ParentOutLine).ToList().FirstOrDefault();
            //                //父级任务名称
            //                string planName = plist == null ? "" : plist.Name;
            //                #region 查询条件
            //                QueryCondition qc = new QueryCondition();
            //                ConditionExpression ce = null;
            //                if (!string.IsNullOrEmpty(planName))
            //                {
            //                    ce = new ConditionExpression();
            //                    ce.ExpName = "Name";
            //                    ce.ExpValue = "%" + planName + "%";
            //                    ce.ExpOperater = eConditionOperator.Like;
            //                    ce.ExpLogical = eLogicalOperator.And;
            //                    qc.ConditionList.Add(ce);
            //                }
            //                if (!string.IsNullOrEmpty(ProjectName))
            //                {
            //                    ce = new ConditionExpression();
            //                    ce.ExpName = "ProjectName";
            //                    ce.ExpValue = "%" + ProjectName + "%";
            //                    ce.ExpOperater = eConditionOperator.Like;
            //                    ce.ExpLogical = eLogicalOperator.And;
            //                    qc.ConditionList.Add(ce);
            //                }
            //                #endregion
            //                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //                {
            //                    Epm_Plan model = proxy.GetPlanList(qc).Data.FirstOrDefault();
            //                    temp.ParentId = model.Id;
            //                    result = proxy.AddPlan(temp, null);
            //                }
            //            }
            //        }
            //        #endregion
            //    }
            //    //读取完成后删除文件
            //    //System.IO.File.Delete(pathStr);
            //}
            //else if (str == ".xls" || str == ".xlsx") //读取excel文件
            //{
            //    //ToDo:读取excel文件添加到数据库
            //}
            //else
            //{
            //    throw new System.Exception("不能读取该类型文件，请重新上传");
            //}
            return View();
        }


        /// <summary>
        /// 关联BIM模型
        /// </summary>
        /// <returns></returns>
        public ActionResult RelationBIM(long id, long projectId)
        {
            Result<List<Epm_Bim>> list = new Result<List<Epm_Bim>>();
            Result<PlanView> result = new Result<PlanView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据ProjectId获取BIM模型列表
                list = proxy.GetBimModelListByProjectId(projectId);
                result = proxy.GetPlanModel(id);
            }

            ViewBag.BimList = list.Data.ToSelectList("Name", "Id", false, list.Data.Any() ? list.Data.FirstOrDefault().Id.ToString() : "");
            ViewBag.BIMAddress = list.Data.Any() ? list.Data.FirstOrDefault().BIMAddress : "";
            return View(result.Data);
        }

        /// <summary>
        /// 查看关联模型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult SelectRelationBIM(long id, long projectId)
        {
            Result<List<Epm_Bim>> list = new Result<List<Epm_Bim>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据ProjectId获取BIM模型列表
                list = proxy.GetBimModelListByProjectId(projectId);
            }
            ViewBag.BimList = list.Data.ToSelectList("Name", "Id", false, list.Data.Count == 0 ? "" : list.Data.FirstOrDefault().Id.ToString());
            ViewBag.BIMAddress = list.Data.Count == 0 ? "" : list.Data.FirstOrDefault().BIMAddress;

            return View();
        }

        /// <summary>
        /// 获取BIM模型地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBimUrl(long id)
        {
            Result<Epm_Bim> result = new Result<Epm_Bim>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimModel(id);
            }

            string bimAddress = result.Data.BIMAddress ?? "";
            return Json(bimAddress);
        }

        /// <summary>
        /// 根据计划id获取关联组件列表
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="bimId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPlanComponentList(long planId, long bimId)
        {
            Result<List<Epm_PlanComponent>> result = new Result<List<Epm_PlanComponent>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetComponentListByPlanId(planId, bimId);
            }
            return Json(result.Data);
        }

        /// <summary>
        /// 添加计划关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPlanComponent(Epm_PlanComponent model, string planComponentIds = "")
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddPlanComponent(model, planComponentIds);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 甘特图页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GanttChart(string projectName = "")
        {
            List<Epm_Plan> list = new List<Epm_Plan>();
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.PageInfo.isAllowPage = false;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetPlanList(qc);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                var data = result.Data.Select(p => new
                {
                    name = p.Name,
                    values = result.Data.Where(x => x.Id == p.Id).Select(x => new
                    {
                        from = "/Date(" + (p.StartTime.Value.Ticks - startTime.Ticks) / 10000 + ")/",
                        to = "/Date(" + (p.EndTime.Value.Ticks - startTime.Ticks) / 10000 + ")/",
                        label = "",
                        customClass = "red"
                    })
                }).ToList();
                ViewBag.ganData = JsonConvert.SerializeObject(data);
                return View();
            }
        }

        /// <summary>
        /// 进度跟踪列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleIndex(long ProjectId = 0, string ProjectName = "")
        {
            ViewBag.ProjectId = ProjectId;
            ViewBag.ProjectName = ProjectName;
            Result<List<PlanView>> planViewList = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                planViewList = proxy.GetPlanViewList(ProjectId);
            }
            //施工计划树形列表
            ViewBag.planViewList = JsonConvert.SerializeObject(planViewList.Data);

            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetGantt(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetProjectGantt(id);
                return Json(result.ToResultView(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}