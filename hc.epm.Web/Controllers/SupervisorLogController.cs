using System;
using System.Collections.Generic;
using System.Linq;

using hc.Plat.Common.Extend;
using System.Web.Mvc;
using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
//using Microsoft.Office.Interop.MSProject;
using Newtonsoft.Json;
using hc.epm.Common;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Configuration;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.epm.Web.Models;

namespace hc.epm.Web.Controllers
{
    public class SupervisorLogController : BaseWebController
    {
        // GET: SupervisorLog
        public ActionResult Index(string projectName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "监理日志列表";
            ViewBag.ProjectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition()
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
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

            Result<List<Epm_SupervisorLog>> result = new Result<List<Epm_SupervisorLog>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSupervisorLogListNew(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        [HttpPost]
        public ActionResult CompanyInfo(long projectId = 0, long companyId = 0)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                string[] companys = { "土建", "包装", "安装", "加固", "内衬", "油罐清洗费" };
                string[] workType = { "项目经理", "现场负责人", "安全员", "本省地区负责人" };
                string a = "'土建', '包装', '安装', '加固', '内衬', '油罐清洗费'";
                // 施工单位
                List<Epm_ProjectCompany> companyList = new List<Epm_ProjectCompany>();
                LogCompanyView logCompanyView = new LogCompanyView();
                var companyResult = proxy.GetProjectCompanyList(projectId);
                if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null && companyResult.Data.Any())
                {
                    companyList = companyResult.Data.Where(t => t.CompanyId == companyId && a.Contains(t.Type) && t.Type != "油罐").ToList();
                }
                List<LogCompanyView> viewlist = new List<LogCompanyView>();
                PeoplesView xmjg = null;
                foreach (var item in companyList)
                {
                    List<PeoplesView> namesList = new List<PeoplesView>();

                    if (item.Type == companys[0] || item.Type == companys[4])
                    {

                        xmjg = new PeoplesView();
                        xmjg.type = workType[0];
                        xmjg.id = item.PMId ?? 0;
                        xmjg.name = item.PM == "请选择" ? "" : item.PM;
                        xmjg.phone = item.PMPhone ?? "";

                    }
                    else if (item.Type == companys[1] || item.Type == companys[2] || item.Type == companys[3] || item.Type == companys[5])
                    {

                        xmjg = new PeoplesView();
                        xmjg.type = workType[3];
                        xmjg.id = item.PMId ?? 0;
                        xmjg.name = item.PM ?? "";
                        xmjg.phone = item.PMPhone ?? "";


                    }

                    PeoplesView xcfzr = new PeoplesView();
                    xcfzr.type = workType[1];
                    xcfzr.id = item.LinkManId ?? 0;
                    xcfzr.name = item.LinkMan == "请选择" ? "" : item.LinkMan;
                    xcfzr.phone = item.LinkPhone ?? "";

                    PeoplesView aqy = new PeoplesView();
                    aqy.type = workType[2];
                    aqy.id = item.SafeManId ?? 0;
                    aqy.name = item.SafeMan == "请选择" ? "" : item.SafeMan;
                    aqy.phone = item.SafePhone ?? "";

                    namesList.Add(xmjg);
                    namesList.Add(xcfzr);
                    namesList.Add(aqy);
                    logCompanyView.personnelList = namesList;
                    logCompanyView.id = item.CompanyId;
                    logCompanyView.name = item.CompanyName;
                    viewlist.Add(logCompanyView);


                };
                return Json(logCompanyView);
            }
        }
        [HttpGet]
        public ActionResult Add(long projectId = 0, string projectName = "")
        {
            ViewBag.Title = "新增监理日志";
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            if (projectId != 0)
            {
                ViewBag.ProjectId = projectId;
                ViewBag.ProjectName = projectName;
            }
            ViewBag.Day = 0;
            ViewBag.ConstructionDate = 0;
            ViewBag.PlanId = "";
            ViewBag.NextPlanId = "";
            ViewBag.jobes = "";
            ViewBag.CompanyId = new List<Epm_ProjectCompany>().ToSelectList("CompanyName", "CompanyId", true);
            ViewBag.SubmitTime = DateTime.Now.ToShortDateString();

            if (ViewBag.ProjectId != null && !string.IsNullOrEmpty(ViewBag.ProjectId.ToString()))
            {
                projectId = Convert.ToInt64(ViewBag.ProjectId.ToString());
            }
            ViewBag.Str = "";
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Weather };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.TypeNo = subjects[DictionaryType.Weather].ToSelectList("Name", "No", false);

                QueryCondition qc = new QueryCondition();

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });



                // 施工计划
                var planResult = proxy.GetPlanList(qc);      //获取施工计划表中的所有数据
                if (planResult.Flag == EResultFlag.Success && planResult.Data != null && planResult.Data.Any())
                {
                    ViewBag.PlanId = JsonConvert.SerializeObject(planResult.Data.Where(t => !t.StartDate.HasValue).OrderBy(t => t.Id).Select(t => new { id = t.Id.ToString(), text = t.Name }).ToList());

                    ViewBag.NextPlanId = JsonConvert.SerializeObject(planResult.Data.Where(t => t.StartDate.HasValue && !t.EndDate.HasValue).OrderBy(t => t.Id).Select(t => new { id = t.Id.ToString(), text = t.Name }).ToList());

                    var planModel = planResult.Data.OrderBy(p => p.StartTime).FirstOrDefault(p => p.IsFinish == null || p.IsFinish == 0);
                    if (planModel != null)
                    {
                        ViewBag.Day = (DateTime.Today - (planModel.DelayTime ?? planModel.EndTime)).Value.Days;   //延期的天数
                    }
                }

                // 项目信息
                var projectResult = proxy.GetProjectModel(projectId);
                if (projectResult.Flag == EResultFlag.Success && projectResult.Data != null)
                {
                    // 项目总工期
                    ViewBag.Total = projectResult.Data.Limit;      //总工期
                    int cd = projectResult.Data.PlanWorkStartTime.HasValue ? ((DateTime.Today - projectResult.Data.PlanWorkStartTime.Value.Date).Days + 1) : 0;
                    ViewBag.ConstructionDate = cd < 0 ? 0 : (cd > projectResult.Data.Limit ? projectResult.Data.Limit : cd);   //施工天数
                    ViewBag.Str = "";
                    string city = projectResult.Data.City;
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        WebRequest request = WebRequest.Create("http://wthrcdn.etouch.cn/WeatherApi?city=" + city);
                        WebResponse response = request.GetResponse();
                        GZipStream GStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress, true);
                        StreamReader reader = new StreamReader(GStream, Encoding.UTF8);
                        string str = reader.ReadToEnd();
                        reader.Close();
                        response.Close();

                        ViewBag.Str = str.ToString();
                    }
                }

                // 施工单位
                List<string> companys = new List<string> { "土建", "包装", "安装", "加固", "内衬", "油罐清洗费" };
                var companyResult = proxy.GetProjectCompanyList(projectId);
                if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null && companyResult.Data.Any())
                {
                    companyResult.Data = companyResult.Data.Where(t => t.CompanyId.HasValue && companys.Contains(t.Type)).ToList();

                    var comList = companyResult.Data.Select(t => new
                    {
                        t.CompanyId,
                        t.CompanyName
                    }).Distinct().ToList();
                    ViewBag.CompanyId = comList.ToSelectList("CompanyName", "CompanyId", true);
                }

                List<Base_TypeDictionary> Job_ScopesList = new List<Base_TypeDictionary>();
                List<DictionaryType> Job_Scopes = new List<DictionaryType>() { DictionaryType.Job_Scopes };
                var dicResults = proxy.GetTypeListByTypes(Job_Scopes);
                if (dicResults.Flag == EResultFlag.Success && dicResults.Data != null && dicResults.Data.ContainsKey(DictionaryType.Job_Scopes))
                {
                    Job_ScopesList = dicResults.Data[DictionaryType.Job_Scopes];
                }
                ViewBag.jobes = dicResults.Data[DictionaryType.Job_Scopes].ToSelectList("Name", "No", false);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Add(SupervisorLogView model)
        {
            ResultView<int> view = new ResultView<int>();
            model.CrtCompanyId = CurrentUser.UserId;
            model.CrtCompanyName = CurrentUser.UserName;
            var ProjectIds = model.ProjectId.Value;
            var CompanyNames = model.CrtCompanyName;
            //表单校验
            if (string.IsNullOrEmpty(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "请选择监理日志所属项目！";
                return Json(view);
            }

            //施工单位
            string builder = Request.Form["Builder"];
            if (string.IsNullOrWhiteSpace(builder))
            {
                view.Flag = false;
                view.Message = "请填写现场施工单位情况！";
                return Json(view);
            }

            string jsonData = JsonConvert.SerializeObject(null);

            List<LogAtillsoners> workCompanys = JsonConvert.DeserializeObject<List<LogAtillsoners>>(builder);

            if (workCompanys == null || !workCompanys.Any())
            {
                view.Flag = false;
                view.Message = "请填写现场施工单位情况！";
                return Json(view);
            }
            foreach (var item in workCompanys)
            {
                Epm_SupervisorLogCompany com = new Epm_SupervisorLogCompany();
                com.CompanyId = item.companyId.Value;//供应商ID
                com.CompanyName = item.companyName;//供应商姓名
                com.ManagerName = item.managerName;//姓名

                Epm_ProjectlLogName projectlLogName = new Epm_ProjectlLogName();
                try
                {
                    projectlLogName.personid = item.ManagerId.Value;
                }
                catch (Exception ex)
                { }
                projectlLogName.name = item.managerName;
                projectlLogName.type = item.workPeopleTypeName;
                com.ProjectlLogName.Add(projectlLogName);

                Epm_AttendanceList attendanceList = new Epm_AttendanceList();
                attendanceList.bepresent = item.bepresent;
                attendanceList.name = item.managerName;
                attendanceList.permit = item.permit;
                attendanceList.workPeopleTypeName = item.workPeopleTypeName;
                com.AttendanceList.Add(attendanceList);
                model.SupervisorLogCompanys.Add(com);
            }
            //上传附件
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.Attachs = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            else
            {
                view.Flag = false;
                view.Message = "请上传附件！";
                return Json(view);
            }

            string fileDataJsonFile = Request.Form["fileDataJsonWorkId"];
            if (!string.IsNullOrEmpty(fileDataJsonFile))
            {
                List<WorkView> workViewList = JsonConvert.DeserializeObject<List<WorkView>>(fileDataJsonFile);

                List<WorkUploadRealSceneView> sceneList = new List<WorkUploadRealSceneView>();
                if (workViewList.Count > 0)
                {
                    foreach (var item in workViewList)
                    {
                        if (item.Type == 2 && item.workFile == "[]")
                        {
                            view.Flag = false;
                            view.Message = "请上传作业实景！";
                            return Json(view);
                        }
                        else
                        {
                            WorkUploadRealSceneView scene = new WorkUploadRealSceneView();

                            if (item.Type == 1)
                            {
                                scene.Id = item.Id;
                                scene.Type = item.Type;
                                scene.Attachs = null;
                            }
                            else
                            {
                                scene.Id = item.Id;
                                scene.Type = item.Type;
                                scene.Attachs = JsonConvert.DeserializeObject<List<Base_Files>>(item.workFile);
                            }
                            sceneList.Add(scene);
                        }
                    }
                    model.SenceList = sceneList;
                }

            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                model.SubmitTime = DateTime.Now;
                result = proxy.AddSupervisorLogNew(model, null);

            }
            return Json(result.ToResultView());
        }

        [HttpGet]
        [AuthCheck(Module = WebModule.LogManage, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            ViewBag.Title = "查看监理日志";
            ViewBag.WebAPIURL = ConfigurationManager.AppSettings["WebAPIURL"];

            Result<SupervisorLogView> result = new Result<SupervisorLogView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSupervisorLogModelNew(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    // 项目信息
                    var projectResult = proxy.GetProjectModel(result.Data.ProjectId.Value);
                    if (projectResult.Flag == EResultFlag.Success && projectResult.Data != null)
                    {
                        // 项目总工期
                        ViewBag.Total = projectResult.Data.Limit;      //总工期
                        //int cd = (result.Data.SubmitTime.Value - projectResult.Data.PlanWorkStartTime.Value.Date).Days + 1;
                        int cd = projectResult.Data.PlanWorkStartTime.HasValue ? ((DateTime.Today - projectResult.Data.PlanWorkStartTime.Value.Date).Days + 1) : 0;
                        ViewBag.ConstructionDate = cd < 0 ? 0 : cd;   //施工天数
                    }

                    if (result.Data.Attachs.Any())
                    {
                        result.Data.Attachs = result.Data.Attachs.Where(t => t.ImageType == "small").ToList();
                    }
                }

                return View(result.Data);
            }

        }

        /// <summary>
        /// 根据projectId获取危险作业信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult GetWorkInfo(long projectId, string submitTime)
        {
            DateTime stime = Convert.ToDateTime(Convert.ToDateTime(submitTime).ToString("yyyy-MM-dd"));
            DateTime etime = Convert.ToDateTime(Convert.ToDateTime(submitTime).ToString("yyyy-MM-dd") + "  23:59:59");

            #region 查询条件 在作业时间内
            QueryCondition qc2 = new QueryCondition();
            ConditionExpression ce2 = null;
            qc2.PageInfo.isAllowPage = false;
            qc2.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
            if (projectId != 0)
            {
                ce2 = new ConditionExpression();
                ce2.ExpName = "ProjectId";
                ce2.ExpValue = projectId;
                ce2.ExpOperater = eConditionOperator.Equal;
                ce2.ExpLogical = eLogicalOperator.And;
                qc2.ConditionList.Add(ce2);
            }
            qc2.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "StartTime",
                ExpValue = etime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.LessThanOrEqual
            });

            qc2.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "EndTime",
                ExpValue = stime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.GreaterThanOrEqual
            });

            ConditionExpression ce3 = new ConditionExpression();
            ce3.ExpLogical = eLogicalOperator.And;
            ce3.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.WorkPartAppr,
                ExpOperater = eConditionOperator.Equal
            });
            ce3.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.ApprSuccess,
                ExpLogical = eLogicalOperator.Or,
                ExpOperater = eConditionOperator.Equal
            });
            qc2.ConditionList.Add(ce3);
            #endregion

            #region 查询条件 等于提交时间
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            qc.PageInfo.isAllowPage = false;
            qc.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
            if (projectId != 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectId";
                ce.ExpValue = projectId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = stime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.GreaterThanOrEqual
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = etime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.LessThanOrEqual
            });

            ConditionExpression ce4 = new ConditionExpression();
            ce4.ExpLogical = eLogicalOperator.And;
            ce4.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.WaitAppr,
                ExpOperater = eConditionOperator.Equal
            });
            ce4.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.ApprSuccess,
                ExpLogical = eLogicalOperator.Or,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(ce4);
            #endregion

            Epm_DangerousWork result = new Epm_DangerousWork();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qcWorkReal = new QueryCondition();
                ConditionExpression ceqcWorkReal = null;
                qcWorkReal.PageInfo.isAllowPage = false;
                qcWorkReal.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
                result = proxy.GetDangerousWorkList(qc).Data.FirstOrDefault();
                if (result != null)
                {
                    ceqcWorkReal = new ConditionExpression();
                    ceqcWorkReal.ExpName = "WorkId";
                    ceqcWorkReal.ExpValue = result.Id;
                    ceqcWorkReal.ExpOperater = eConditionOperator.Equal;
                    ceqcWorkReal.ExpLogical = eLogicalOperator.And;
                    qcWorkReal.ConditionList.Add(ceqcWorkReal);
                    var WorkRealSceneModel = proxy.GetWorkRealSceneList(qcWorkReal).Data.FirstOrDefault();
                    var data = (new
                    {
                        WorkId = result.Id.ToString(),
                        TaskName = result.TaskName,
                        WorkStartTime = result.StartTime,
                        WorkEndTime = result.EndTime,
                        WorkContent = result.TaskContent,
                        //1：待审核 2：审核通过 3：作业待审核
                        Flag = (result.State == (int)ApprovalState.WaitAppr ? 1 : (result.State == (int)ApprovalState.ApprSuccess ? 2 : (result.State == (int)ApprovalState.WorkPartAppr ? 3 : 0))),
                        Type = 1,//日志提报时间等于危险作业提交时间
                        WorkRealSceneState = WorkRealSceneModel != null ? WorkRealSceneModel.State : 0,
                        WorkRealSceneID = WorkRealSceneModel != null ? WorkRealSceneModel.Id.ToString() : "0"
                    });

                    return Json(data);
                }
                else {
                    result = proxy.GetDangerousWorkList(qc2).Data.FirstOrDefault();
                    if (result != null)
                    {
                        ceqcWorkReal = new ConditionExpression();
                        ceqcWorkReal.ExpName = "WorkId";
                        ceqcWorkReal.ExpValue = result.Id;
                        ceqcWorkReal.ExpOperater = eConditionOperator.Equal;
                        ceqcWorkReal.ExpLogical = eLogicalOperator.And;
                        qcWorkReal.ConditionList.Add(ceqcWorkReal);
                        var WorkRealSceneModel = proxy.GetWorkRealSceneList(qcWorkReal).Data.FirstOrDefault();
                        var data = (new
                        {
                            WorkId = result.Id.ToString(),
                            TaskName = result.TaskName,
                            WorkStartTime = result.StartTime,
                            WorkEndTime = result.EndTime,
                            WorkContent = result.TaskContent,
                            //1：待审核 2：审核通过 3：作业待审核 4:作业审核不通过
                            Flag = (result.State == (int)ApprovalState.WaitAppr ? 1 : (result.State == (int)ApprovalState.ApprSuccess ? 2 : (result.State == (int)ApprovalState.WorkPartAppr ? 3 : 0))),
                            Type = 2, //日志提报时间再作业时间内容
                            WorkRealSceneState = WorkRealSceneModel != null ? WorkRealSceneModel.State : 0,
                            WorkRealSceneID = WorkRealSceneModel != null ? WorkRealSceneModel.Id.ToString() : "0"
                        });

                        return Json(data);
                    }
                }

                return Json(new { Flag = 0 });
            }
        }

        /// <summary>
        /// 根据projectId获取危险作业信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult GetWorkInfo1(long projectId, string submitTime)
        {
            DateTime stime = Convert.ToDateTime(Convert.ToDateTime(submitTime).ToString("yyyy-MM-dd"));
            DateTime etime = Convert.ToDateTime(Convert.ToDateTime(submitTime).ToString("yyyy-MM-dd") + "  23:59:59");

            #region 查询条件 在作业时间内
            QueryCondition qc2 = new QueryCondition();
            ConditionExpression ce2 = null;
            qc2.PageInfo.isAllowPage = false;
            qc2.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
            if (projectId != 0)
            {
                ce2 = new ConditionExpression();
                ce2.ExpName = "ProjectId";
                ce2.ExpValue = projectId;
                ce2.ExpOperater = eConditionOperator.Equal;
                ce2.ExpLogical = eLogicalOperator.And;
                qc2.ConditionList.Add(ce2);
            }
            qc2.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "StartTime",
                ExpValue = etime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.LessThanOrEqual
            });

            qc2.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "StartTime",
                ExpValue = stime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.GreaterThanOrEqual
            });

            ConditionExpression ce3 = new ConditionExpression();
            ce3.ExpLogical = eLogicalOperator.And;
            ce3.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.ApprSuccess,
                ExpOperater = eConditionOperator.Equal
            });
            //ce3.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "State",
            //    ExpValue = (int)ApprovalState.ApprSuccess,
            //    ExpLogical = eLogicalOperator.Or,
            //    ExpOperater = eConditionOperator.Equal
            //});
            qc2.ConditionList.Add(ce3);
            #endregion

            #region 查询条件 等于提交时间
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            qc.PageInfo.isAllowPage = false;
            qc.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
            if (projectId != 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectId";
                ce.ExpValue = projectId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = stime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.GreaterThanOrEqual
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = etime,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.LessThanOrEqual
            });

            ConditionExpression ce4 = new ConditionExpression();
            ce4.ExpLogical = eLogicalOperator.And;
            ce4.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.WaitAppr,
                ExpOperater = eConditionOperator.Equal
            });
            //ce4.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "State",
            //    ExpValue = (int)ApprovalState.ApprSuccess,
            //    ExpLogical = eLogicalOperator.Or,
            //    ExpOperater = eConditionOperator.Equal
            //});
            qc.ConditionList.Add(ce4);
            #endregion

            List<Epm_DangerousWork> result = new List<Epm_DangerousWork>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<long> workids = null;
                result = proxy.GetDangerousWorkList(qc).Data;

                var result2 = proxy.GetDangerousWorkList(qc2).Data;
                if (result2.Count > 0)
                {
                    result.AddRange(result2);
                }
                QueryCondition qcWorkReal = new QueryCondition();
                ConditionExpression ceqcWorkReal = null;
                qcWorkReal.PageInfo.isAllowPage = false;
                qcWorkReal.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
                if (result.Count > 0)
                {
                    workids = result.Select(p => p.Id).ToList();
                    ceqcWorkReal = new ConditionExpression();
                    ceqcWorkReal.ExpName = "WorkId";
                    ceqcWorkReal.ExpValue = workids.JoinToString(",");
                    ceqcWorkReal.ExpOperater = eConditionOperator.In;
                    ceqcWorkReal.ExpLogical = eLogicalOperator.And;
                    qcWorkReal.ConditionList.Add(ceqcWorkReal);
                    var WorkRealSceneModel = proxy.GetWorkRealSceneList(qcWorkReal).Data;

                    var data = result.Select(p => new
                    {
                        WorkId = p.Id.ToString(),
                        TaskName = p.TaskName,
                        WorkStartTime = p.StartTime,
                        WorkEndTime = p.EndTime,
                        WorkContent = p.TaskContent,
                        State = p.State,
                        Type = 1,//日志提报时间等于危险作业提交时间
                        WorkRealSceneState = WorkRealSceneModel.Where(x => x.WorkId == p.Id).FirstOrDefault() != null ? WorkRealSceneModel.Where(x => x.WorkId == p.Id).FirstOrDefault().State : 0,
                        WorkRealSceneID = WorkRealSceneModel.Where(x => x.WorkId == p.Id).FirstOrDefault() != null ? WorkRealSceneModel.Where(x => x.WorkId == p.Id).FirstOrDefault().Id.ToString() : "0"
                    });
                    return Json(data);
                }
            }

            return Json(new { Flag = 0 });
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.LogManage, Right = SystemRight.Check)]
        [HttpPost]
        public ActionResult Audit(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_SupervisorLog model = new Epm_SupervisorLog();
                model.Id = id;
                model.State = (int)ApprovalState.ApprSuccess;
                result = proxy.AuditSupervisorLog(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.LogManage, Right = SystemRight.UnCheck)]
        [HttpPost]
        public ActionResult Reject(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_SupervisorLog model = new Epm_SupervisorLog();
                model.Id = id;
                model.State = (int)ApprovalState.ApprFailure;
                result = proxy.AuditSupervisorLog(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.LogManage, Right = SystemRight.Invalid)]
        [HttpPost]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Epm_SupervisorLog model = new Epm_SupervisorLog();
                model.Id = id;
                model.State = (int)ApprovalState.Discarded;
                result = proxy.AuditSupervisorLog(model);
            }
            return Json(result.ToResultView());
        }
    }

    /// <summary>
    /// 监理日志关联资料
    /// </summary>
    /// <param name="projectID"></param>
    /// <param name="milepostId"></param>
    /// <returns></returns>
    //public ActionResult RelationFile(long projectId, long milepostId)
    //{
    //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
    //    {
    //        QueryCondition qc = new QueryCondition()
    //        {
    //            PageInfo = new PageListInfo()
    //            {
    //                isAllowPage = false
    //            }
    //        };
    //        qc.ConditionList.Add(new ConditionExpression()
    //        {
    //            ExpName = "ProjectId",
    //            ExpValue = projectId,
    //            ExpOperater = eConditionOperator.Equal,
    //            ExpLogical = eLogicalOperator.And
    //        });
    //        qc.ConditionList.Add(new ConditionExpression()
    //        {
    //            ExpName = "MilepostId",
    //            ExpValue = milepostId,
    //            ExpOperater = eConditionOperator.Equal,
    //            ExpLogical = eLogicalOperator.And
    //        });

    //        var result = proxy.GetProjectDataList(qc);
    //        return View(result.Data);
    //    }
    //}
}
