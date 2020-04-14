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
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class ProjectController : BaseWebController
    {
        #region 查询
        /// <summary>
        /// 在建项目列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.ProjectInfoManage, Right = SystemRight.Browse)]
        public ActionResult Index(string state = "5", string first = "0", string pmName = "", string name = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.state = state;
            ViewBag.str = "";

            ViewBag.RealName = CurrentUser.RealName;

            if (!string.IsNullOrWhiteSpace(state))
            {
                var s = state.Split(',');
                var str = "";
                if (s.Count() > 0)
                {
                    for (int i = 0; i < s.Count(); i++)
                    {
                        str = str + ((ProjectState)(Convert.ToInt32(s[i]))).GetText() + ',';
                    }
                }
                ViewBag.str = str.Substring(0, str.Length - 1);
            }

            //pmName = (first == "1" && pmName == "") ? CurrentUser.UserId.ToString() : pmName;
            ViewBag.pmName = pmName;
            ViewBag.UserName = "";

            Result<List<ProjectView>> result = new Result<List<ProjectView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (!string.IsNullOrEmpty(pmName))
                {
                    var pname = pmName.Split(',');
                    var strPname = "";
                    if (pmName != "youke")
                    {
                        for (int i = 0; i < pname.Count(); i++)
                        {
                            long usId = Convert.ToInt64(pname[i]);

                            var userModel = proxy.GetUserModel(usId);
                            if (userModel.Data != null)
                            {
                                strPname = strPname + userModel.Data.UserName + ',';
                            }
                        }
                        ViewBag.UserName = strPname.Substring(0, strPname.Length - 1);
                    }
                    else
                    {
                        ViewBag.UserName = "";
                    }
                }

                result = proxy.GetProjectListInfo(pageIndex, pageSize, state, pmName, name, startTime, endTime);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //项目经理数据
                Result<List<Base_User>> allUser = proxy.GetAgencyPMList(string.Empty, 1, 100);
                ViewBag.pmList = allUser.Data;
            }

            //项目状态下拉数据
            ViewBag.ProjectState = typeof(ProjectState).AsSelectList(false);
            return View(result.Data);
        }

        #endregion

        #region 新增
        /// <summary>
        /// 新增在建项目
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.ProjectInfoManage, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.CurrentCompanyId = CurrentUser.CompanyId;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //返回项目主体列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectSubject, DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.SubjectNo = subjects[DictionaryType.ProjectSubject].ToSelectList("Name", "No", false);

                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].ToSelectList("Name", "No", false);

                //返回项目性质列表   
                //根据字典类型集合获取字典数据
                //Result<List<Epm_ProjectNature>> data = proxy.GetProjectNature();
                //ViewBag.ProjectNature = data.Data.ToSelectList("NATURE_MC", "NATURE_BH", false);
            }
            return View();
        }

        /// <summary>
        /// 添加在建项目（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.ProjectInfoManage, Right = SystemRight.Add)]
        public ActionResult Add(Epm_Project model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编码不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddProject(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 考勤设置
        /// </summary>
        /// <returns></returns>
        public ActionResult AttendanceSet()
        {
            return View();
        }
        #endregion

        #region 修改
        /// <summary>
        /// 编辑在建项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.ProjectInfoManage, Right = SystemRight.Modify)]
        public ActionResult Edit(long projectId)
        {
            ViewBag.CurrentCompanyId = CurrentUser.CompanyId;
            Result<Epm_Project> result = new Result<Epm_Project>();
            
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
                //var resultTz = proxy.GetProjectApprovalInfos(result.Data.TzProjectId);
                //result.Data.Amount = resultTz.Data.TotalInvestment;
                List<DictionaryType> SchemeTypeList = new List<DictionaryType>() { DictionaryType.SchemeType };
                var SchemeTypes = proxy.GetTypeListByTypes(SchemeTypeList).Data;
                ViewBag.BluePrintKey = SchemeTypes[DictionaryType.SchemeType].ToSelectList("Name", "No", true, result.Data.BluePrintKey);

                //是否分公司部门主任
                ViewBag.IsDirector = proxy.IsBranchCompanyDirector(CurrentUser.UserId) ? 1 : 0;
            }
            ViewBag.ProjectId = projectId;
            //ViewBag.TzProjectId = result.Data.TzProjectId;

            return View(result.Data);
        }
        /// <summary>
        /// 编辑在建项目（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.ProjectInfoManage, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Project model)
        {
            //上传附件
            List<Base_Files> attachs = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                attachs = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateProject(model, attachs);

                if (!string.IsNullOrEmpty(fileDataJson))
                {
                    var project = proxy.GetProject(model.Id).Data;
                    DateTime time = DateTime.Now;
                    foreach (var item in attachs)
                    {
                        if (string.IsNullOrEmpty(item.ImageType))
                        {
                            Bp_SendDate send = new Bp_SendDate();
                            send.IsSend = false;
                            send.Key = "2001020014";
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
        /// 总批复及构成
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ApprovalConstitute(long projectId)
        {
            Result<List<Epm_ProjectConstitute>> result = new Result<List<Epm_ProjectConstitute>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProject(projectId).Data;
                ViewBag.Amount = project.Amount;
                ViewBag.AProvideAmount = project.AProvideAmount;
                ViewBag.BalanceAmount = project.BalanceAmount;
                ViewBag.IsCrossings = project.IsCrossings;
                ViewBag.ProjectId = projectId;

                result = proxy.GetProjectConstituteByProjectId(projectId);

                List<DictionaryType> SchemeTypeList = new List<DictionaryType>() { DictionaryType.SchemeType };
                var SchemeTypes = proxy.GetTypeListByTypes(SchemeTypeList).Data;
                ViewBag.BluePrintKey = SchemeTypes[DictionaryType.SchemeType].ToSelectList("Name", "No", false, project.BluePrintKey);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 编辑在建项目（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApprovalConstitute(Epm_Project model, long projectId)
        {
            ViewBag.ProjectId = projectId;
            model.Id = projectId;
            string constituteLists = Request.Form["ConstituteLists"];
            List<Epm_ProjectConstitute> list = JsonConvert.DeserializeObject<List<Epm_ProjectConstitute>>(constituteLists);

            //上传附件
            List<Base_Files> attachs = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                attachs = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateProjectConstitute(model, list, attachs);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 历史总批复及构成
        /// </summary>
        /// <returns></returns>
        public ActionResult HApprovalConstitute(long projectId)
        {
            DataTable dt = new DataTable();
            Result<List<Epm_ProjectConstituteHistory>> result = new Result<List<Epm_ProjectConstituteHistory>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectConstituteHistoryByProjectId(projectId);
            }
            result.Data = result.Data.Where(t => !string.IsNullOrEmpty(t.ConstituteValue)).ToList();


            if (result.Data.Count() > 0)
            {
                dt.Columns.Add("总批复及构成", System.Type.GetType("System.String"));
                var Times = result.Data.GroupBy(t => t.CreateTime).OrderByDescending(t => t.Key).Select(t => t.Key).ToList();
                for (int i = 0; i < Times.Count(); i++)
                {
                    dt.Columns.Add(Times[i].ToString("yyyy-MM-dd HH:mm:ss"), System.Type.GetType("System.String"));
                }

                var data = result.Data.OrderBy(t => t.IsAProvide).OrderBy(t => t.Sort).ToList();
                var cols = data.GroupBy(t => t.ConstituteValue).Select(t => t.Key).ToList();
                for (int j = 0; j < cols.Count(); j++)
                {
                    var rows = result.Data.Where(t => t.ConstituteValue == cols[j]).ToList();
                    if (rows.Count() > 0)
                    {
                        DataRow dr = null;
                        dr = dt.NewRow();
                        dr["总批复及构成"] = cols[j];
                        for (int i = 0; i < rows.Count(); i++)
                        {
                            var time = rows[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                            dr[time] = rows[i].Amount.ToString("0.######");
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return View(dt);
        }

        /// <summary>
        /// 编辑工程内容要点
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ProjectMainPoint(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPoints>> result = new Result<List<Epm_ProjectWorkMainPoints>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProject(projectId).Data;
                ViewBag.ProjectId = projectId;
                result = proxy.GetProjectPointsByProjectId(projectId);
            }
            return View(result.Data);
        }

        public ActionResult MainPointTemp(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPoints>> result = new Result<List<Epm_ProjectWorkMainPoints>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProject(projectId).Data;
                ViewBag.ProjectId = projectId;
                result = proxy.GetProjectPointsByProjectId(projectId);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 编辑工程内容要点（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectMainPointPost(long projectId)
        {
            string ProjectMainPoints = Request.Form["ProjectMainPoints"];
            List<Epm_ProjectWorkMainPoints> list = JsonConvert.DeserializeObject<List<Epm_ProjectWorkMainPoints>>(ProjectMainPoints);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateProjectPoints(list, projectId);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 历史工程内容要点
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult HProjectMainPoint(long projectId)
        {
            DataTable dt = new DataTable();
            //Result<List<Epm_ProjectWorkMainPointsHistory>> result = new Result<List<Epm_ProjectWorkMainPointsHistory>>();
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            //{
            //    result = proxy.GetProjectPointsHistoryByProjectId(projectId);
            //}
            //result.Data = result.Data.Where(t => !string.IsNullOrEmpty(t.WorkMainPointsValue)).ToList();

            //if (result.Data.Count() > 0)
            //{
            //    dt.Columns.Add("工程内容要点", System.Type.GetType("System.String"));
            //    dt.Columns.Add("类型", System.Type.GetType("System.String"));
            //    var Times = result.Data.GroupBy(t => t.CreateTime).OrderByDescending(t => t.Key).Select(t => t.Key).ToList();
            //    for (int i = 0; i < Times.Count(); i++)
            //    {
            //        dt.Columns.Add(Times[i].ToString("yyyy-MM-dd HH:mm:ss"), System.Type.GetType("System.String"));
            //    }

            //    var data = result.Data.OrderBy(t => t.Sort).ToList();
            //    var cols = data.GroupBy(t => t.WorkMainPointsValue).Select(t => t.Key).ToList();
            //    for (int j = 0; j < cols.Count(); j++)
            //    {
            //        var rows = result.Data.Where(t => t.WorkMainPointsValue == cols[j]).ToList();
            //        if (rows.Count() > 0)
            //        {
            //            DataRow drQty = dt.NewRow();
            //            drQty["工程内容要点"] = cols[j];
            //            drQty["类型"] = "数量";

            //            DataRow drRemark = dt.NewRow();
            //            drRemark["工程内容要点"] = cols[j];
            //            drRemark["类型"] = "备注";

            //            for (int i = 0; i < rows.Count(); i++)
            //            {
            //                var time = rows[i].CreateTime.ToString("yyyy-MM-dd HH:mm:ss");

            //                drQty[time] = rows[i].Qty.ToString("0.######");
            //                drRemark[time] = rows[i].Description;
            //            }
            //            dt.Rows.Add(drQty);
            //            dt.Rows.Add(drRemark);
            //        }
            //    }
            //}
            return View(dt);
        }

        /// <summary>
        /// 工期管理
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult TimeManage(long projectId)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
            }
            ViewBag.ProjectId = projectId;
            // ViewBag.TzProjectId = result.Data.TzProjectId;
            return View(result.Data);
        }
        /// <summary>
        /// 工期管理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TimeManage(Epm_Project model)
        {
            //上传附件
            List<Base_Files> attachs = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                var attach = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
                attachs.AddRange(attach);
            }
            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJsonFile))
            {
                var attach = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符
                attachs.AddRange(attach);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTimelimit(model, attachs);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 根据项目id获取工期
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TimeLimitDetail(long projectId)
        {
            Result<Epm_TimeLimitAndProcedure> result = new Result<Epm_TimeLimitAndProcedure>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var proje = proxy.GetProject(projectId);
                if (proje.Data != null)
                {
                    result = proxy.GetTimeLimitAndProcedure(proje.Data.TzProjectId);
                }


                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
                ViewBag.ProjectId = projectId;
            }

            return View(result.Data);
        }

        /// <summary>
        /// 根据项目id获取手续
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProcedureDetail(long projectId)
        {
            Result<Epm_TimeLimitAndProcedure> result = new Result<Epm_TimeLimitAndProcedure>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var proje = proxy.GetProject(projectId);
                if (proje.Data != null)
                {
                    result = proxy.GetTimeLimitAndProcedure(proje.Data.TzProjectId);
                }

                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
                ViewBag.ProjectId = projectId;
            }

            return View(result.Data);
        }

        /// <summary>
        /// 根据项目id获取工期
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CKTimeLimitDetail(long projectId)
        {
            Result<Epm_TimeLimitAndProcedure> result = new Result<Epm_TimeLimitAndProcedure>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var proje = proxy.GetProject(projectId);
                if (proje.Data != null)
                {
                    result = proxy.GetTimeLimitAndProcedure(proje.Data.TzProjectId);
                }


                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
                ViewBag.ProjectId = projectId;
            }

            return View(result.Data);
        }

        /// <summary>
        /// 根据项目id获取手续
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CKProcedureDetail(long projectId)
        {
            Result<Epm_TimeLimitAndProcedure> result = new Result<Epm_TimeLimitAndProcedure>();
            ResultView<int> view = new ResultView<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource, DictionaryType.TimeAndCrossingsType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var proje = proxy.GetProject(projectId);
                if (proje.Data != null)
                {
                    result = proxy.GetTimeLimitAndProcedure(proje.Data.TzProjectId);
                }

                ViewBag.timeAndCrossingsType = subjects[DictionaryType.TimeAndCrossingsType].ToList().ToSelectList("Name", "No", true);//附件类型
                ViewBag.ProjectId = projectId;
            }

            return View(result.Data);
        }

        /// <summary>
        /// 获取里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult MilestonePlan(long projectId)
        {
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostPlan(projectId);
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                ce = new ConditionExpression();
                ce.ExpName = "ParentId";
                ce.ExpValue = 0;
                ce.ExpOperater = eConditionOperator.Equal;
                qc.ConditionList.Add(ce);
                qc.PageInfo.isAllowPage = false;
                Result<List<Epm_Milepost>> data = proxy.GetMilepostList(qc);
                ViewBag.parentId = data.Data.ToSelectList("Name", "Id", true);

            }
            ViewBag.Limit = 0;
            if (result.Flag == EResultFlag.Success && result.Data.Any())
            {
                DateTime startTime = result.Data.Where(t => t.Plan.StartTime.HasValue).OrderBy(t => t.Plan.StartTime).FirstOrDefault().Plan.StartTime.Value;
                DateTime endTime = result.Data.Where(t => t.Plan.EndTime.HasValue).OrderByDescending(t => t.Plan.EndTime).FirstOrDefault().Plan.EndTime.Value;
                DateTime? delayTime = null;
                if (result.Data.Where(t => t.Plan.DelayTime.HasValue).OrderByDescending(t => t.Plan.DelayTime).Any())
                {
                    delayTime = result.Data.Where(t => t.Plan.DelayTime.HasValue).OrderByDescending(t => t.Plan.DelayTime).FirstOrDefault().Plan.DelayTime;
                }
                endTime = (delayTime.HasValue && delayTime.Value > endTime) ? delayTime.Value : endTime;

                TimeSpan sp = endTime.Subtract(startTime);

                ViewBag.Limit = sp.Days + 1;
            }

            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 生成里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MilestonePlan(long projectId, DateTime planStart, long mileType)
        {
            Result<List<Epm_Plan>> result = new Result<List<Epm_Plan>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.CreateMilepostPlan(projectId, planStart, mileType, 1);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 审核里程碑计划（支持修改）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditMilepostPlan(Epm_Project model)
        {
            string MilepostPlans = Request.Form["MilepostPlan"];
            string inputId = Request.Form["HidinputId"];
            string state = Request.Form["State"];
            List<Epm_Plan> list = JsonConvert.DeserializeObject<List<Epm_Plan>>(MilepostPlans);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (!string.IsNullOrEmpty(inputId))
                {
                    var str = inputId.Substring(0, inputId.Length - 1).Split(',');
                    List<long> ids = new List<long>();
                    if (str.Length > 0)
                    {
                        foreach (var item in str)
                        {
                            ids.Add(item.ToLongReq());
                        }
                    }
                    //删除页面也删除的里程碑
                    proxy.DeletePlanByIds(ids);
                }
                if (list.Any())
                {
                    list.ForEach(p =>
                    {
                        if ("1".Equals(state))
                        {
                            p.State = (int)ApprovalState.ApprSuccess;
                        }
                    });
                    result = proxy.AuditMilepostPlan(list);
                }
                return Json(result.ToResultView());
            }
        }
        /// <summary>
        /// 驳回里程碑计划
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectMilepostPlan(long projectId)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.RejectMilepostPlan(projectId);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 计划关联构件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindComponents(long planId, string param)
        {
            Result<int> result = new Result<int>();
            List<Epm_PlanComponent> pcList = new List<Epm_PlanComponent>();
            if (!string.IsNullOrWhiteSpace(param))
            {
                pcList = JsonConvert.DeserializeObject<List<Epm_PlanComponent>>(param);
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.BindComponents(planId, pcList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工程服务商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ProjectISP(long projectId)
        {
            Result<List<NewProjectCompanyContractView>> result = new Result<List<NewProjectCompanyContractView>>();
            List<NewProjectCompanyContractView> list = new List<NewProjectCompanyContractView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var resultProject = proxy.GetProjectCompanyByProjectId(projectId);

                if (resultProject.Data.Count > 0)
                {
                    foreach (var item in resultProject.Data)
                    {
                        NewProjectCompanyContractView view = new NewProjectCompanyContractView();
                        view.Epm_ProjectCompany = item;
                        if (item.ContractId.HasValue && item.ContractId.Value != 0)
                        {
                            var contractResult = proxy.GetContractModel(item.ContractId.Value);
                            if (contractResult.Data != null)
                            {
                                view.ContractAmount = contractResult.Data.Amount;
                                view.ContractStartTime = contractResult.Data.StartTime;
                                view.ContractEndTime = contractResult.Data.EndTime;
                                view.ContractCode = contractResult.Data.Code;
                                view.ContractSignTime = contractResult.Data.SignTime;
                            }
                        }
                        if (item.OrderId.HasValue && item.OrderId.Value != 0)
                        {
                            var orderResult = proxy.GetContractModel(item.OrderId.Value);
                            if (orderResult.Data != null)
                            {
                                view.OrderAmount = orderResult.Data.Amount;
                                view.OrderStartTime = orderResult.Data.StartTime;
                                view.OrderEndTime = orderResult.Data.EndTime;
                                view.OrderCode = orderResult.Data.Code;
                                view.OrderSignTime = orderResult.Data.SignTime;
                            }
                        }

                        list.Add(view);
                    }

                    result.Data = list;
                }
                bool isService = proxy.IsServiceUser(CurrentUser.UserId);
                ViewBag.isService = isService;
                bool isSupervisor = proxy.IsSupervisor(projectId, CurrentUser.UserId);
                ViewBag.isSupervisor = isSupervisor;
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 工程服务商数据提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectISP()
        {
            string ISPValues = Request.Form["ISPValue"];
            List<ProjectCompanyContractView> list = JsonConvert.DeserializeObject<List<ProjectCompanyContractView>>(ISPValues);

            List<Epm_ProjectCompany> comList = new List<Epm_ProjectCompany>();
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (list.Any())
                {
                    List<string> randonList = new List<string>();
                    Epm_Project project = proxy.GetProject(list[0].ProjectId.Value).Data;
                    DateTime time = DateTime.Now;
                    for (int i = 0; i < list.Count(); i++)
                    {
                        Epm_ProjectCompany proCom = new Epm_ProjectCompany();
                        proCom.Id = list[i].Id;
                        proCom.ProjectId = list[i].ProjectId;
                        proCom.CompanyId = list[i].CompanyId;
                        proCom.CompanyName = list[i].CompanyName;
                        proCom.Type = list[i].Type;
                        proCom.ContractId = list[i].ContractId;
                        proCom.ContractName = list[i].ContractName;
                        proCom.OrderId = list[i].OrderId;
                        proCom.OrderName = list[i].OrderName;
                        proCom.ContractCode = list[i].ContractCode;

                        //Epm_Contract contract = null;

                        #region 委托书
                        //if (list[i].IsOrderAdd == "Add")
                        //{
                        //    var oldProjectCompany = proxy.GetProjectCompany(list[i].Id).Data;

                            //if (list[i].OrderId.HasValue)
                            //{
                            //    contract = new Epm_Contract();
                            //    contract.FirstPartyId = project.CompanyId;
                            //    contract.FirstPartyName = project.CompanyName;
                            //    contract.SecondPartyId = list[i].CompanyId;
                            //    contract.SecondPartyName = list[i].CompanyName;
                            //    contract.StartTime = list[i].OrderStartTime;
                            //    contract.EndTime = list[i].OrderEndTime;
                            //    contract.BuildDays = (contract.EndTime - contract.StartTime).Value.Days + 1;
                            //    contract.SignTime = list[i].OrderSignTime;
                            //    contract.ProjectId = list[i].ProjectId;
                            //    contract.ProjectName = project.Name;
                            //    contract.ContractType = (int)ContractType.Order;

                            //    var selContract = proxy.GetContractModel(list[i].OrderId.Value).Data;
                            //    List<Base_Files> files = proxy.GetFilesByTable("Epm_Contract", selContract.Id).Data;
                            //    foreach (var file in files)
                            //    {
                            //        file.Id = SnowflakeHelper.GetID;
                            //        file.TableId = contract.Id;
                            //    }

                            //    contract.Name = files.Where(t => t.ImageType != "small").FirstOrDefault().Name.Split('.')[0];
                            //    contract.Code = list[i].OrderCode;
                            //    contract.Amount = list[i].OrderAmount;
                            //    contract.BuildDays = (contract.EndTime.Value - contract.StartTime.Value).Days + 1;
                            //    contract.State = (int)ApprovalState.ApprSuccess;
                            //    proxy.AddContract(contract, files);

                            //    foreach (var item in files)
                            //    {
                            //        if (string.IsNullOrEmpty(item.ImageType))
                            //        {
                            //            Bp_SendDate send = new Bp_SendDate();
                            //            send.IsSend = false;
                            //            send.Key = "2002040003";
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

                            //    proCom.OrderId = contract.Id;
                            //    proCom.OrderName = files.Where(t => t.ImageType != "small").Select(t => t.Name).JoinToString(",");

                            //    if (selContract.Code == contract.Code)
                            //    {
                            //        proxy.DeleteContractByIds(new List<long>() { selContract.Id });
                            //    }
                            //}
                            //else
                            //{
                        //    if (!string.IsNullOrEmpty(list[i].OrderFiles))
                        //    {
                        //        contract = new Epm_Contract();
                        //        contract.FirstPartyId = project.CompanyId;
                        //        contract.FirstPartyName = project.CompanyName;
                        //        contract.SecondPartyId = list[i].CompanyId;
                        //        contract.SecondPartyName = list[i].CompanyName;
                        //        contract.StartTime = list[i].OrderStartTime;
                        //        contract.EndTime = list[i].OrderEndTime;
                        //        contract.SignTime = list[i].OrderSignTime;
                        //        contract.BuildDays = (contract.EndTime - contract.StartTime).Value.Days + 1;
                        //        contract.ProjectId = list[i].ProjectId;
                        //        contract.ProjectName = project.Name;
                        //        contract.ContractType = (int)ContractType.Order;
                        //        List<Base_Files> orderFileList = JsonConvert.DeserializeObject<List<Base_Files>>(list[i].OrderFiles);
                        //        contract.Name = orderFileList[0].Name.Split('.')[0];
                        //        contract.Code = list[i].OrderCode;
                        //        contract.Amount = list[i].OrderAmount;
                        //        contract.State = (int)ApprovalState.ApprSuccess;
                        //        contract.BuildDays = (contract.EndTime.Value - contract.StartTime.Value).Days + 1;
                        //        proxy.AddContract(contract, orderFileList);

                        //        foreach (var item in orderFileList)
                        //        {
                        //            if (string.IsNullOrEmpty(item.ImageType))
                        //            {
                        //                Bp_SendDate send = new Bp_SendDate();
                        //                send.IsSend = false;
                        //                send.Key = "2002040003";
                        //                send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                        //                send.Type = "12";
                        //                send.Project = "BIM";
                        //                send.KeyValue = project.ObjeId;
                        //                send.UserName = CurrentUser.UserCode;
                        //                send.CreateTime = time;
                        //                send.OperateTime = time;
                        //                send.OperateUserId = CurrentUser.UserId;
                        //                send.OperateUserName = CurrentUser.UserName;
                        //                send.CreateUserId = CurrentUser.UserId;
                        //                send.CreateUserName = CurrentUser.UserName;
                        //                proxy.AddSendDate(send);
                        //            }
                        //        }

                        //        proCom.OrderId = contract.Id;
                        //        //proCom.OrderName = orderFileList.Where(t => t.ImageType != "small").Select(t => t.Name).JoinToString(",");
                        //        proCom.OrderName = contract.Name;
                        //    }
                        //    //}

                        //    //删除之前数据
                        //    if (oldProjectCompany != null && oldProjectCompany.OrderId.HasValue && oldProjectCompany.ContractId != 0)
                        //    {
                        //        var oldContract = proxy.GetContractModel(oldProjectCompany.OrderId.Value).Data;
                        //        if (oldContract != null)
                        //        {
                        //            List<Base_Files> oldFiles = proxy.GetFilesByTable("Epm_Contract", oldContract.Id).Data;

                        //            proxy.DeleteContractModel(oldContract.Id);
                        //            proxy.DeleteFilesByTableIds("Epm_Contract", oldFiles.Select(p => p.Id).ToList());
                        //        }
                        //    }
                        //}

                        #endregion

                        #region 合同
                        //if (list[i].IsContractAdd == "Add")
                        //{
                            #region 选择合同修改为手输，不需要去表中取合同名字，再次需要删除下面的if，打开这块就好
                            //var oldProjectCompany = proxy.GetProjectCompany(list[i].Id).Data;

                            //if (list[i].ContractId.HasValue)
                            //{
                            //    //contract = new Epm_Contract();
                            //    //contract.FirstPartyId = project.CompanyId;
                            //    //contract.FirstPartyName = project.CompanyName;
                            //    //contract.SecondPartyId = list[i].CompanyId;
                            //    //contract.SecondPartyName = list[i].CompanyName;
                            //    //contract.StartTime = list[i].ContractStartTime;
                            //    //contract.EndTime = list[i].ContractEndTime;
                            //    //contract.BuildDays = (contract.EndTime - contract.StartTime).Value.Days + 1;
                            //    //contract.SignTime = list[i].ContractSignTime;
                            //    //contract.ProjectId = list[i].ProjectId;
                            //    //contract.ProjectName = project.Name;
                            //    //contract.ContractType = (int)ContractType.Contract;
                            //    //contract.BuildDays = (contract.EndTime.Value - contract.StartTime.Value).Days + 1;

                            //    var selContract = proxy.GetContractModel(list[i].ContractId.Value).Data;
                            //    List<Base_Files> files = proxy.GetFilesByTable("Epm_Contract", selContract.Id).Data;
                            //    //foreach (var file in files)
                            //    //{
                            //    //    file.Id = SnowflakeHelper.GetID;
                            //    //    file.TableId = contract.Id;
                            //    //}

                            //    //contract.Name = files.Where(t => t.ImageType != "small").FirstOrDefault().Name.Split('.')[0];
                            //    //contract.Code = list[i].ContractCode;
                            //    //contract.Amount = list[i].ContractAmount;
                            //    //contract.State = (int)ApprovalState.ApprSuccess;
                            //    //proxy.AddContract(contract, files);

                            //    foreach (var item in files)
                            //    {
                            //        if (string.IsNullOrEmpty(item.ImageType))
                            //        {
                            //            Bp_SendDate send = new Bp_SendDate();
                            //            send.IsSend = false;
                            //            send.Key = "2002040003";
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

                            //    //proCom.ContractId = contract.Id;
                            //    //proCom.ContractName = files.Where(t => t.ImageType != "small").Select(t => t.Name).JoinToString(",");
                            //    proCom.ContractId = selContract.Id;
                            //    proCom.ContractName = selContract.Name;

                            //    //if (selContract.Code == contract.Code)
                            //    //{
                            //    //    proxy.DeleteContractByIds(new List<long>() { selContract.Id });
                            //    //}
                            #endregion
                            //var oldProjectCompany = proxy.GetProjectCompany(list[i].Id).Data;

                            //if (!string.IsNullOrEmpty(list[i].ContractCode))
                            //{
                            //    proCom.ContractCode = list[i].ContractCode;//合同编码
                            //}

                            //if (list[i].ContractId.HasValue)
                            //{

                            //    proCom.ContractId = list[i].ContractId;

                            //}
                            //else
                            //{
                            //    if (!string.IsNullOrEmpty(list[i].ContractFiles))
                            //    {
                            //        contract = new Epm_Contract();
                            //        contract.FirstPartyId = project.CompanyId;
                            //        contract.FirstPartyName = project.CompanyName;
                            //        contract.SecondPartyId = list[i].CompanyId;
                            //        contract.SecondPartyName = list[i].CompanyName;
                            //        contract.StartTime = list[i].ContractStartTime;
                            //        contract.EndTime = list[i].ContractEndTime;
                            //        contract.BuildDays = (contract.EndTime - contract.StartTime).Value.Days + 1;
                            //        contract.SignTime = list[i].ContractSignTime;
                            //        contract.ProjectId = list[i].ProjectId;
                            //        contract.ProjectName = project.Name;
                            //        contract.ContractType = (int)ContractType.Contract;
                            //        List<Base_Files> contractFileList = JsonConvert.DeserializeObject<List<Base_Files>>(list[i].ContractFiles);
                            //        contract.Name = contractFileList[0].Name.Split('.')[0];
                            //        contract.Code = list[i].ContractCode;
                            //        contract.Amount = list[i].ContractAmount;
                            //        contract.State = (int)ApprovalState.ApprSuccess;
                            //        contract.BuildDays = (contract.EndTime.Value - contract.StartTime.Value).Days + 1;
                            //        proxy.AddContract(contract, contractFileList);

                            //        foreach (var item in contractFileList)
                            //        {
                            //            if (string.IsNullOrEmpty(item.ImageType))
                            //            {
                            //                Bp_SendDate send = new Bp_SendDate();
                            //                send.IsSend = false;
                            //                send.Key = "2002040003";
                            //                send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                            //                send.Type = "12";
                            //                send.Project = "BIM";
                            //                send.KeyValue = project.ObjeId;
                            //                send.UserName = CurrentUser.UserCode;
                            //                send.CreateTime = time;
                            //                send.OperateTime = time;
                            //                send.OperateUserId = CurrentUser.UserId;
                            //                send.OperateUserName = CurrentUser.UserName;
                            //                send.CreateUserId = CurrentUser.UserId;
                            //                send.CreateUserName = CurrentUser.UserName;
                            //                proxy.AddSendDate(send);
                            //            }
                            //        }

                            //        proCom.ContractId = contract.Id;
                            //        proCom.ContractName = contractFileList.Where(t => t.ImageType != "small").Select(t => t.Name).JoinToString(",");
                            //    }
                            //}

                            ////删除之前数据
                            //if (oldProjectCompany != null && oldProjectCompany.ContractId.HasValue && oldProjectCompany.ContractId != 0)
                            //{
                            //    var oldContract = proxy.GetContractModel(oldProjectCompany.ContractId.Value).Data;
                            //    if (oldContract != null)
                            //    {
                            //        List<Base_Files> oldFiles = proxy.GetFilesByTable("Epm_Contract", oldContract.Id).Data;

                            //        proxy.DeleteContractModel(oldContract.Id);
                            //        proxy.DeleteFilesByTableIds("Epm_Contract", oldFiles.Select(p => p.Id).ToList());
                            //    }
                            //}
                        //}

                        #endregion

                        comList.Add(proCom);
                    }
                }
                result = proxy.UpdateProjectCompany(comList);
            }
            return Json(result.ToResultView());
        }

        public ActionResult DetailCrossingsManage(long projectId)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
            }
            ViewBag.ProjectId = projectId;
            ViewBag.IsCrossings = result.Data.IsCrossings;
            return View(result.Data);
        }


        /// <summary>
        /// 外部手续
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult CrossingsManage(long projectId)
        {
            // 分成两个Action
            // 服务商问题：1、合同、委托书上传、选择未去掉，2、修改时需带出之前数据
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
            }
            ViewBag.ProjectId = projectId;
            ViewBag.IsCrossings = result.Data.IsCrossings;
            return View(result.Data);
        }

        /// <summary>
        /// 外部手续
        /// </summary>
        /// <param name="model"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CrossingsManage(Epm_Project model, long projectId)
        {
            ViewBag.ProjectId = projectId;
            model.Id = projectId;
            //上传附件
            List<Base_Files> attachs = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                attachs = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            List<Epm_ProjectConstitute> list = new List<Epm_ProjectConstitute>();
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateProjectConstitute(model, list, attachs);

                if (!string.IsNullOrEmpty(fileDataJson))
                {
                    var project = proxy.GetProject(projectId).Data;
                    DateTime time = DateTime.Now;
                    foreach (var item in attachs)
                    {
                        if (string.IsNullOrEmpty(item.ImageType))
                        {
                            Bp_SendDate send = new Bp_SendDate();
                            send.IsSend = false;
                            send.Key = "2003020006";
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

        public ActionResult InputContract()
        {
            return View();
        }
        /// <summary>
        /// 新增修改项目经理
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AddPerson(long Id, string isUpdate, string type)
        {
            ViewBag.Id = Id;
            ViewBag.isUpdate = isUpdate;
            ViewBag.type = type;

            Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
            Result<Epm_Project> resultZc = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectCompany(Id);

                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CompanyId",
                    ExpValue = result.Data.CompanyId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                List<Base_User> lis = proxy.GetUserList(qc).Data;

                QueryCondition qY = new QueryCondition();
                qY.PageInfo.isAllowPage = false;
                qY.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Card",
                    ExpValue = "QY",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                List<Base_User> lisqy = proxy.GetUserList(qY).Data;
                lis.AddRange(lisqy);
                string selValue = result.Data.PMId.HasValue ? result.Data.PMId.ToString() : "";
                string selLinkValue = result.Data.LinkManId.HasValue ? result.Data.LinkManId.ToString() : "";

                if (type == "监理")
                {
                    ViewBag.UserList = lis.Where(t => (t.Post != null && t.Post.Contains("总监")) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selValue);
                    ViewBag.pmPhone = "";
                    if (!string.IsNullOrEmpty(selValue) && selValue != "0")
                    {
                        ViewBag.pmPhone = proxy.GetUserModel(selValue.ToLongReq()).Data.Phone;
                    }

                    ViewBag.LinkUserList = lis.ToSelectList("UserName", "Id", true, selLinkValue);
                    ViewBag.linkPhone = "";
                    if (!string.IsNullOrEmpty(selLinkValue) && selLinkValue != "0")
                    {
                        ViewBag.linkPhone = proxy.GetUserModel(selLinkValue.ToLongReq()).Data.Phone;
                    }
                }
                else if (type == "设计费" || type == "地勘" || type == "危废处置")
                {
                    ViewBag.UserList = lis.Where(t => (t.Post != null && t.Post.Contains("本省地区负责人")) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selValue);
                    ViewBag.pmPhone = "";
                    if (!string.IsNullOrEmpty(selValue) && selValue != "0")
                    {
                        ViewBag.pmPhone = proxy.GetUserModel(selValue.ToLongReq()).Data.Phone;
                    }

                    ViewBag.LinkUserList = lis.ToSelectList("UserName", "Id", true, "");
                    ViewBag.linkPhone = "";
                    if (!string.IsNullOrEmpty(selLinkValue) && selLinkValue != "0")
                    {
                        ViewBag.linkPhone = proxy.GetUserModel(selLinkValue.ToLongReq()).Data.Phone;
                    }
                }
                else if (type == "土建" || type == "内衬")
                {
                    var postUser = "项目经理（" + (type == "油罐清洗费" ? "清罐" : type) + "）";
                    ViewBag.UserList = lis.Where(t => (t.Post != null && t.Post.StartsWith(postUser)) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selValue);
                    ViewBag.pmPhone = "";
                    if (!string.IsNullOrEmpty(selValue) && selValue != "0")
                    {
                        ViewBag.pmPhone = proxy.GetUserModel(selValue.ToLongReq()).Data.Phone;
                    }

                    var post = "现场负责人（" + (type == "油罐清洗费" ? "清罐" : type) + "）";

                    ViewBag.LinkUserList = lis.Where(t => (t.Post != null && t.Post.StartsWith(post)) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selLinkValue);

                    ViewBag.linkPhone = "";
                    if (!string.IsNullOrEmpty(selLinkValue) && selLinkValue != "0")
                    {
                        ViewBag.linkPhone = proxy.GetUserModel(selLinkValue.ToLongReq()).Data.Phone;
                    }
                }
                else if (type == "安装" || type == "包装" || type == "加固" || type == "油罐清洗费")
                {
                    ViewBag.UserList = lis.Where(t => (t.Post != null && t.Post.Contains("本省地区负责人")) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selValue);
                    ViewBag.pmPhone = "";
                    if (!string.IsNullOrEmpty(selValue) && selValue != "0")
                    {
                        ViewBag.pmPhone = proxy.GetUserModel(selValue.ToLongReq()).Data.Phone;
                    }

                    var post = "现场负责人（" + (type == "油罐清洗费" ? "清罐" : type) + "）";
                    ViewBag.LinkUserList = lis.Where(t => (t.Post != null && t.Post.Contains(post)) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selLinkValue);
                    ViewBag.linkPhone = "";
                    if (!string.IsNullOrEmpty(selLinkValue) && selLinkValue != "0")
                    {
                        ViewBag.linkPhone = proxy.GetUserModel(selLinkValue.ToLongReq()).Data.Phone;
                    }
                }
                else //if (type == "网架" || type == "油罐" || type == "管线" || type == "发电机" || type == "液位仪" || type == "加油机" || type == "配电柜")
                {
                    ViewBag.UserList = lis.Where(t => (t.Post != null && t.Post.Contains("本省地区负责人")) || t.Card == "QY").ToList().ToSelectList("UserName", "Id", true, selValue);
                    ViewBag.pmPhone = "";
                    if (!string.IsNullOrEmpty(selValue) && selValue != "0")
                    {
                        ViewBag.pmPhone = proxy.GetUserModel(selValue.ToLongReq()).Data.Phone;
                    }

                    ViewBag.LinkUserList = lis.ToSelectList("UserName", "Id", true, "");
                    ViewBag.linkPhone = "";
                    if (!string.IsNullOrEmpty(selLinkValue) && selLinkValue != "0")
                    {
                        ViewBag.linkPhone = proxy.GetUserModel(selLinkValue.ToLongReq()).Data.Phone;
                    }
                }
                //else
                //{
                //    ViewBag.UserList = lis.ToSelectList("UserName", "Id", true, "");
                //    ViewBag.pmPhone = "";

                //    ViewBag.LinkUserList = lis.ToSelectList("UserName", "Id", true, "");
                //    ViewBag.linkPhone = "";
                //}
            }
            return View(result.Data);
        }

        public ActionResult GetUserInfo(long id)
        {
            Result<Base_User> list = new Result<Base_User>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                list = proxy.GetUserModel(id);
            }
            return Json(list);
        }
        public ActionResult GetUserByCompanyId(long companyId)
        {
            Result<List<Base_User>> list = new Result<List<Base_User>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                list = proxy.GetUserByCompanyId(companyId);
            }
            return Json(list);
        }
        /// <summary>
        /// 更新项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectISPUpdate(Epm_ProjectCompany ProjectCompany)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdatePMAndPhone(ProjectCompany);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 获取项目经理信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult AuditePerson(long Id)
        {
            Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectCompany(Id);
            }
            ViewBag.Id = Id;
            return View(result.Data);
        }
        /// <summary>
        /// 审核项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditePerson(Epm_ProjectCompany projectCompany)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditPMAndPhone(projectCompany);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 驳回项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectPerson(Epm_ProjectCompany projectCompany)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.RejectPMManAndPhone(projectCompany);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 新增修改项目负责人
        /// </summary>
        /// <returns></returns>
        //public ActionResult AddLinkMan(long Id)
        //{
        //    Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetProjectCompany(Id);
        //    }
        //    ViewBag.Id = Id;
        //    return View(result.Data);
        //}
        //public ActionResult AddLinkMan(long Id)
        //{
        //    Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetProjectCompany(Id);

        //        QueryCondition qc = new QueryCondition();
        //        qc.PageInfo.isAllowPage = false;
        //        qc.ConditionList.Add(new ConditionExpression()
        //        {
        //            ExpName = "CompanyId",
        //            ExpValue = result.Data.CompanyId,
        //            ExpLogical = eLogicalOperator.And,
        //            ExpOperater = eConditionOperator.Equal
        //        });
        //        List<Base_User> lis = proxy.GetUserList(qc).Data;

        //        string selValue = result.Data.LinkManId.HasValue ? result.Data.LinkManId.ToString() : "";
        //        ViewBag.UserList = lis.ToSelectList("UserName", "Id", false, selValue);
        //    }
        //    ViewBag.Id = Id;
        //    return View(result.Data);
        //}
        ///// <summary>
        ///// 更新项目负责人信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult ProjectISPLInkManUpdate(Epm_ProjectCompany ProjectCompany)
        //{

        //    Result<int> result = new Result<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.UpdateLinkManAndPhone(ProjectCompany);
        //    }
        //    return Json(result.ToResultView());
        //}
        ///// <summary>
        ///// 审核项目负责人信息
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult AuditeLinkMan(Epm_ProjectCompany projectCompany)
        //{
        //    Result<int> result = new Result<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.AuditLinkManAndPhone(projectCompany);
        //    }
        //    return Json(result.ToResultView());
        //}
        ///// <summary>
        ///// 驳回项目负责人信息
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult RejectLinkMan(Epm_ProjectCompany projectCompany)
        //{
        //    Result<int> result = new Result<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.RejectLinkManAndPhone(projectCompany);
        //    }
        //    return Json(result.ToResultView());
        //}
        ///// <summary>
        ///// 获取项目负责人信息
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetLinkMan(long Id)
        //{
        //    Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetProjectCompany(Id);
        //    }
        //    ViewBag.Id = Id;
        //    return View(result.Data);
        //}

        /// <summary>
        /// 项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ProjectData(long projectId)
        {
            Result<List<Epm_ProjectDataSubmit>> result = new Result<List<Epm_ProjectDataSubmit>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectSubmitByProjectId(projectId);
            }
            ViewBag.TableIds = (result.Data == null || result.Data.Count == 0) ? "" : result.Data.Select(i => i.SId).JoinToString(",");
            ViewBag.ProjectId = projectId;
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 项目资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectData()
        {
            //上传附件
            List<Base_Files> attachs = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                var attach = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
                attachs.AddRange(attach);
            }

            var projectId = Request["ProjectId"].ToString();

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                proxy.UpdateProjectSubmit(Convert.ToInt64(projectId), attachs);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 查看
        /// <summary>
        /// 查看在建项目详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult Detail(long projectId)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 总批复及构成详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailApprovalConstitute(long projectId)
        {
            Result<List<Epm_ProjectConstitute>> result = new Result<List<Epm_ProjectConstitute>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProject(projectId).Data;
                ViewBag.Amount = project.Amount;
                ViewBag.AProvideAmount = project.AProvideAmount;
                ViewBag.BalanceAmount = project.BalanceAmount;
                ViewBag.IsCrossings = project.IsCrossings;
                ViewBag.BluePrintValue = project.BluePrintValue;
                ViewBag.ProjectId = projectId;

                result = proxy.GetProjectConstituteByProjectId(projectId);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 工程内容要点详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailProjectMainPoint(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPoints>> result = new Result<List<Epm_ProjectWorkMainPoints>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProject(projectId).Data;
                ViewBag.ProjectId = projectId;
                result = proxy.GetProjectPointsByProjectId(projectId);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 工期管理详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailTimeManage(long projectId)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 里程碑计划详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailMilestonePlan(long projectId)
        {
            Result<List<PlanView>> result = new Result<List<PlanView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostPlan(projectId);
            }
            ViewBag.ProjectId = projectId;
            ViewBag.Limit = 0;
            if (result.Flag == EResultFlag.Success && result.Data.Any())
            {
                DateTime startTime = result.Data.Where(t => t.Plan.StartTime.HasValue).OrderBy(t => t.Plan.StartTime).FirstOrDefault().Plan.StartTime.Value;
                DateTime endTime = result.Data.Where(t => t.Plan.EndTime.HasValue).OrderByDescending(t => t.Plan.EndTime).FirstOrDefault().Plan.EndTime.Value;
                DateTime? delayTime = null;
                if (result.Data.Where(t => t.Plan.DelayTime.HasValue).OrderByDescending(t => t.Plan.DelayTime).Any())
                {
                    delayTime = result.Data.Where(t => t.Plan.DelayTime.HasValue).OrderByDescending(t => t.Plan.DelayTime).FirstOrDefault().Plan.DelayTime;
                }

                endTime = (delayTime.HasValue && delayTime.Value > endTime) ? delayTime.Value : endTime;

                TimeSpan sp = endTime.Subtract(startTime);

                ViewBag.Limit = sp.Days + 1;
            }

            return View(result.Data);
        }
        /// <summary>
        /// 工程服务商详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailProjectISP(long projectId)
        {
            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectCompanyByProjectId(projectId);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 项目资料详情
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult DetailProjectData(long projectId)
        {
            Result<List<Epm_ProjectDataSubmit>> result = new Result<List<Epm_ProjectDataSubmit>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectSubmitByProjectId(projectId);
            }
            ViewBag.TableIds = (result.Data == null || result.Data.Count == 0) ? "" : result.Data.Select(i => i.SId).JoinToString(",");
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 合同
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ContractInfo(long projectId)
        {
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ProjectId",
                ExpValue = projectId,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            Result<List<Epm_Contract>> result = new Result<List<Epm_Contract>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractList(qc);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 变更信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult ChangeInfo(long projectId)
        {
            Result<List<ChangeView>> result = new Result<List<ChangeView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var projectName = proxy.GetProject(projectId).Data.Name;
                result = proxy.GetChangeList(projectName, string.Empty, -1, 1, int.MaxValue);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 变更信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult VisaInfo(long projectId)
        {
            Result<List<Epm_Visa>> result = new Result<List<Epm_Visa>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var projectName = proxy.GetProject(projectId).Data.Name;
                result = proxy.GetVisaList(projectName, string.Empty, -1, string.Empty, 1, int.MaxValue);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        /// 问题信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>schedule
        public ActionResult QuestionInfo(long projectId)
        {
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ProjectId",
                ExpValue = projectId,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            Result<List<Epm_Question>> result = new Result<List<Epm_Question>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetQuestionList(qc);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        /// <summary>
        ///进度信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScheduleInfo(long projectId)
        {
            ViewBag.ProjectId = projectId;
            return View();
        }

        /// <summary>
        /// 视频集成
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult Video(long projectId)
        {
            ViewBag.ProjectId = projectId;
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ProjectId",
                ExpValue = projectId,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBaseVideoManageLists(qc);
            }
            ViewBag.ProjectId = projectId;
            return View(result.Data);
        }
        #endregion

        #region 更新状态
        /// <summary>
        /// 修改在建项目状态（终结）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult End(long id, string state = "")
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeProjectState(id, state);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 结项（结项前判断是否施工验收）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Archive(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.CheckAcceptance(id);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除无效项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            List<long> list = id.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteProjectByIds(list);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 获取地区
        /// <summary>
        /// 根据parentCode获取地区列表
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegionList(string parentCode)
        {
            ViewBag.parentCode = parentCode;
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRegionList(parentCode);
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 导出
        [HttpPost]
        public void OutputProjectToExcel()
        {
            string fileName = HttpContext.Request.Form["fileName"];

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<ProjectView>> result = proxy.GetProjectListInfo(0, 100, "", "", "", "");
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        { "Name" , "项目名称"},
                        { "Code" , "项目编码"},
                        {"Address","地址" },
                        {"StartDate","开始日期" },
                        {"EndDate","截止日期" },
                        {"ProjectTypeName","项目类型" },
                        {"Amount","总金额(万元)" },
                        {"State","状态" },
                    };


                    var data = result.Data.Select(p => new
                    {
                        p.Name,
                        p.Code,
                        p.Address,
                        StartDate = string.Format("{0:yyyy-MM-dd}", p.StartDate),
                        EndDate = string.Format("{0:yyyy-MM-dd}", p.EndDate),
                        p.Amount,
                        State = (p.State ?? 0).ToString().ToEnum<ProjectState>().GetText()
                    });

                    hc.epm.UI.Common.ExcelHelper.ExportExcel(fileName, dic, data.Cast<object>().ToList(), HttpContext);
                }
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取服务商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProjectCompany(long projectId)
        {
            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectCompanyList(projectId);
                foreach (var item in result.Data)
                {
                    item.CompanyName = item.CompanyId.ToString();
                }
            }
            return Json(result.ToResultView());
        }

        #endregion


        /// <summary>
        /// 获取防渗改造/项目提出前数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetProjectInfo(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetReformRecordModel(id);
                if (result.Data != null && result.Data.ReformRecord != null)
                {
                    return Json(new { type = "1", data = result.Data });
                }
                else
                {
                    var starApppList = proxy.GetTzProjectApprovalInfoModel(id);
                    if (starApppList.Data != null)
                    {
                        return Json(new { type = "2", data = starApppList.Data });
                    }
                }
            }
            return Json(new { type = "0", data = "error" });
        }

        public ActionResult SetCompany(string type)
        {
            ViewBag.Type = type;
            return View();
        }

        public ActionResult GetContract(string code, string id)
        {
            Result<List<Epm_Contract>> result = new Result<List<Epm_Contract>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Code",
                    ExpValue = code,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                if (!string.IsNullOrEmpty(id))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Id",
                        ExpValue = id.ToLongReq(),
                        ExpOperater = eConditionOperator.NotEqual,
                        ExpLogical = eLogicalOperator.And
                    });
                }
                result = proxy.GetContractList(qc);
            }
            return Json(result.Data);
        }

        /// <summary>
        /// 油站是否存在编码
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsStationCode(long projectId)
        {
            bool IsStationCode = false;
            Result<Epm_Project> result = new Result<Epm_Project>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProject(projectId);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    if (!string.IsNullOrEmpty(result.Data.ProjectSubjectCode))
                    {
                        IsStationCode = true;
                    }
                }
            }
            return Json(IsStationCode);
        }
    }
}