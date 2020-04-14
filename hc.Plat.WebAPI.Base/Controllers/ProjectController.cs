using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.Plat.WebAPI.Base.Common;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace hc.Plat.WebAPI.Base.Controllers
{
    /// <summary>
    /// 项目
    /// </summary>
    public partial class ProjectController : BaseAPIController
    {
        #region 项目
        /// <summary>
        /// 获取个人项目列表及KPI
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectKpi(long projectId, int pageIndex = 1)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
                    if (projectId > 0)
                    {
                        QueryCondition qc = new QueryCondition();
                        qc.PageInfo.isAllowPage = false;
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "Id",
                            ExpValue = projectId,
                            ExpOperater = eConditionOperator.Equal,
                            ExpLogical = eLogicalOperator.And
                        });
                        result = proxy.GetProjectList(qc);
                    }
                    else
                    {
                        result = proxy.GetProjectListById(user.CompanyId, user.UserId);
                    }

                    var kpi = proxy.GetProjectKPIListByWhr(DateTime.Now.Year.ToString(), user.UserId).Data;

                    var data = new
                    {
                        projectList = result.Data.Select(p => new
                        {
                            id = p.SId,
                            projectName = p.Name ?? "",
                            userName = p.ContactUserName ?? "",
                            phone = p.ContactPhone ?? "",
                            startDate = string.Format("{0:yyyy-MM-dd}", p.StartDate),
                            endDate = string.Format("{0:yyyy-MM-dd}", p.EndDate),
                            amount = p.Amount.ToString("0.###### "),
                            state = p.State
                        }),
                        kpi = new
                        {
                            projectNum = kpi == null ? 0 : kpi.TotelNum,//今年项目总数
                            finishNum = kpi == null ? 0 : kpi.FinishNum,//已完工项目
                            doingNum = kpi == null ? 0 : kpi.ConstrunctionNum,//施工中项目
                            delayNum = kpi == null ? 0 : kpi.DelayNum //已延期项目
                        }
                    };
                    return APIResult.GetSuccessResult(data, pageIndex, result.Data.Count, AppCommonHelper.PageSize);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="id">项目 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                var result = proxy.GetProjectModel(id);
                if (result.Flag == EResultFlag.Success)
                {
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return APIResult.GetSuccessNoData();
                    }
                    var project = result.Data;
                    List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, project.Id);

                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo() { isAllowPage = false }
                    };
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Id",
                        ExpValue = project.ProjectSubjectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    var oil = proxy.GetOilStationList(qc).Data.FirstOrDefault();

                    var data = new
                    {
                        id = project.SId,
                        code = project.Code ?? "",
                        projectName = project.Name ?? "",
                        projectNature = project.ProjectNatureName ?? "",
                        nature = project.ProjectNature ?? "",
                        costCourse = project.CostCourse ?? "",
                        subjectNo = project.SubjectName ?? "",
                        projectSubjectName = project.ProjectSubjectName ?? "",
                        area = project.City ?? "" + project.Area ?? "",
                        address = project.Address ?? "",
                        longitude = (oil != null && oil.Longitude.HasValue ? oil.Longitude.Value : decimal.Zero),
                        latitude = (oil != null && oil.Latitude.HasValue ? oil.Latitude.Value : decimal.Zero),
                        amount = project.Amount.HasValue ? project.Amount.ToString("0.###### ") : "0 ",
                        description = project.Description ?? "",
                        pmName = project.PMName ?? "",
                        pmPhone = project.PMPhone ?? "",
                        companyId = project.CompanyId ?? 0,
                        companyName = project.CompanyName ?? "",
                        contactUserName = project.ContactUserName ?? "",
                        contactPhone = project.ContactPhone ?? "",
                        replyNumber = project.ReplyNumber ?? "",
                        projectContent = project.ProjectContent ?? "",
                        remark = project.Remark ?? "",
                        state = project.State ?? 0,
                        shutDownTime = string.Format("{0:yyyy-MM-dd}", project.ShutdownTime),
                        planStartTime = string.Format("{0:yyyy-MM-dd}", project.PlanWorkStartTime),
                        planEndTime = string.Format("{0:yyyy-MM-dd}", project.PlanWorkEndTime),
                        limit = project.Limit == null ? "" : project.Limit.ToString(),
                        planOpenTime = string.Format("{0:yyyy-MM-dd}", project.PlanOpeningTime),
                        planShutdownLimit = project.PlanShutdowLimit == null ? "" : project.PlanShutdowLimit.ToString(),
                        planPackStartTime = string.Format("{0:yyyy-MM-dd}", project.PlanPackStartTime),
                        planPackEndTime = string.Format("{0:yyyy-MM-dd}", project.PlanPackEndTime),
                        planReinforceStartTime = string.Format("{0:yyyy-MM-dd}", project.PlanReinforceStartTime),
                        planReinforceEndTime = string.Format("{0:yyyy-MM-dd}", project.PlanReinforceEndTime),
                        files = AppCommonHelper.GetFileList(files),
                        blueprint = project.BluePrintValue ?? "",
                        gasolinediese = project.GasolineDieselRatio ?? ""
                    };

                    return APIResult.GetSuccessResult(data);
                }
                return APIResult.GetErrorResult(result.Exception);
            }
        }

        /// <summary>
        /// 获取项目工期管理明细信息
        /// </summary>
        /// <param name="id">项目 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectTimeLimitInfo(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetProjectModel(id);
                    if (result.Flag == EResultFlag.Success)
                    {
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }
                        if (result.Data == null)
                        {
                            return APIResult.GetSuccessNoData();
                        }
                        List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, result.Data.Id);

                        List<Base_Files> stopfileList = new List<Base_Files>();
                        List<Base_Files> meetingfileList = new List<Base_Files>();
                        if (files.Count > 0)
                        {
                            foreach (var item in files)
                            {
                                if (item.TableColumn == "stop" || item.TableColumn == "Stop")
                                {
                                    stopfileList.Add(item);
                                }

                                if (item.TableColumn == "meeting" || item.TableColumn == "Meeting")
                                {
                                    meetingfileList.Add(item);
                                }
                            }
                        }
                        var data = new
                        {
                            id = result.Data.SId,
                            shutDownTime = string.Format("{0:yyyy-MM-dd}", result.Data.ShutdownTime),
                            planStartTime = string.Format("{0:yyyy-MM-dd}", result.Data.PlanWorkStartTime),
                            planEndTime = string.Format("{0:yyyy-MM-dd}", result.Data.PlanWorkEndTime),
                            limit = result.Data.Limit == null ? "" : result.Data.Limit.ToString(),
                            planOpenTime = string.Format("{0:yyyy-MM-dd}", result.Data.PlanOpeningTime),
                            planShutdownLimit = result.Data.PlanShutdowLimit == null ? "" : result.Data.PlanShutdowLimit.ToString(),
                            stopFiles = AppCommonHelper.GetFileList(stopfileList),
                            meetingFiles = AppCommonHelper.GetFileList(meetingfileList),
                            actionButton = AppCommonHelper.CreateButtonRightProjectLimit(user, BusinessType.Project.ToString())
                        };

                        return APIResult.GetSuccessResult(data);
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }
        /// <summary>
        /// 修改项目工期信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object UpdateProjectDurationInfo()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            try
            {
                var user = CurrentUserView;
                if (user != null)
                {
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        Epm_Project project = new Epm_Project
                        {
                            Id = form["id"].ToLongReq(),
                            ShutdownTime = form["shutDownTime"].ToDateTime(),
                            //PlanWorkStartTime = model.planStartTime,
                            //PlanWorkEndTime = model.planEndTime,
                            //Limit = form["limit"].ToInt32Req(),
                            PlanOpeningTime = form["planOpenTime"].ToDateTime(),
                            //PlanShutdowLimit = form["PlanShutdowLimit"].ToDateTime(),
                            //PlanPackStartTime = model.planPackStartTime,
                            //PlanPackEndTime = model.planPackEndTime,
                            //PlanReinforceStartTime = model.planReinforceStartTime,
                            //PlanReinforceEndTime = model.planReinforceEndTime
                        };

                        TimeSpan ts = (project.PlanOpeningTime - project.ShutdownTime).Value;
                        project.PlanShutdowLimit = ts.Days;

                        // todo: 附件处理
                        List<Base_Files> files = AppCommonHelper.UploadFile(http, user);

                        var result = proxy.UpdateTimelimit(project, files, false);
                        if (result.Flag == EResultFlag.Success && result.Data > 0)
                        {
                            return APIResult.GetSuccessResult("工期相关信息提交成功！");
                        }
                        return APIResult.GetErrorResult(result.Exception);
                    }
                }
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目总批复构成
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectConstitutes(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var projectResult = proxy.GetProjectModel(projectId);

                    if (projectResult.Flag == EResultFlag.Success)
                    {
                        if (projectResult.Data == null)
                        {
                            return APIResult.GetErrorResult("所选项目不存在！");
                        }
                        var result = proxy.GetProjectConstituteByProjectId(projectId);
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }
                        if (result.Data == null || !result.Data.Any())
                        {
                            return APIResult.GetSuccessNoDatas();
                        }

                        var resultData = result.Data.OrderBy(p => p.Sort).ToList();
                        var project = projectResult.Data;

                        List<ConstitutesModel> list = new List<ConstitutesModel>();
                        list.Add(new ConstitutesModel()
                        {
                            key = "项目金额",
                            value = (project.Amount == null ? "0" : string.IsNullOrEmpty(project.Amount.Value.ToString()) ? "0" : project.Amount.Value.ToString("0.######")) + " 万元"
                        });
                        list.Add(new ConstitutesModel()
                        {
                            key = "甲供设备金额",
                            value = (project.AProvideAmount == null ? "0" : string.IsNullOrEmpty(project.AProvideAmount.Value.ToString()) ? "0" : project.AProvideAmount.Value.ToString("0.######")) + " 万元",
                            childs = resultData.Where(p => p.IsAProvide == true).OrderBy(p => p.Sort).Select(p =>
                                new ConstitutesModel()
                                {
                                    key = p.ConstituteValue,
                                    value = (string.IsNullOrEmpty(p.Amount.ToString()) ? "0" : p.Amount.ToString("0.######")) + " 万元"
                                }).ToList()
                        });
                        list.Add(new ConstitutesModel()
                        {
                            key = "剩余金额",
                            value = (project.BalanceAmount == null ? "0" : string.IsNullOrEmpty(project.BalanceAmount.Value.ToString()) ? "0" : project.BalanceAmount.Value.ToString("0.######")) + " 万元"
                        });
                        list.AddRange(resultData.Where(p => p.IsAProvide == false || p.IsAProvide == null)
                            .OrderBy(p => p.Sort).Select(p => new ConstitutesModel()
                            {
                                key = p.ConstituteValue,
                                value = (string.IsNullOrEmpty(p.Amount.ToString()) ? "0" : p.Amount.ToString("0.######")) + " 万元"
                            }));

                        return APIResult.GetSuccessResult(list);
                    }
                    return APIResult.GetErrorResult(projectResult.Exception);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 获取已维护项目工程要点
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectWorkPoints(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetProjectPointsByProjectId(projectId);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }
                    var s = new List<object>();
                    var data = result.Data.GroupBy(g => new { g.DicKey, g.DicValue }).OrderBy(g => g.Key.DicKey).Select(g => new
                    {
                        category = g.Key.DicValue,
                        details = result.Data.Where(p => p.DicKey == g.Key.DicKey).OrderBy(p => p.Sort).Select(p => new
                        {
                            id = p.SId,
                            key = "",
                            name = p.WorkMain,
                            companyName = "",
                            num = "",
                            isCharging = true,
                            sort = p.Sort ?? 0,
                            remark = p.Description ?? "",
                            type = string.IsNullOrWhiteSpace(p.WorkMainValues) ? "input" : "select",
                            select = string.IsNullOrWhiteSpace(p.WorkMainValues) ? null : ((",请选择;" + p.WorkMainValues).Split(';').ToList().Select(k => new
                            {
                                id = k.Split(',')[0],
                                name = k.Split(',')[1]
                            })),
                            //value = "",
                            value = AppCommonHelper.GetWorkMainValue(p.Val, p.WorkMainValues),
                            unit = p.Unit ?? ""
                        })
                    });


                    // 项目所需权限
                    List<Button> btns = new List<Button>();
                    var userRight = user.Rights.Where(p => p.Value.Contains(BusinessType.Project.ToString()));
                    if (userRight.Any())
                    {
                        foreach (var keyValuePair in userRight)
                        {
                            if (keyValuePair.Value.Contains(SystemRight.SetMainPoints.ToString()))
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = SystemRight.SetMainPoints.GetText();
                                btn.rightAction = SystemRight.SetMainPoints.ToString();
                                btn.color = "";
                                btns.Add(btn);
                            }
                        }
                    }

                    var list = new
                    {
                        list = data,
                        actionButton = btns
                    };

                    return APIResult.GetSuccessResult(list);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        public static string GetWorkMainValue(string val, string WorkMainValues)
        {
            string result = "";
            if (!string.IsNullOrEmpty(WorkMainValues))
            {
                if (!string.IsNullOrEmpty(val))
                {
                    var list = (((WorkMainValues).Split(';').ToList().Select(k => new
                    {
                        id = k.Split(',')[0],
                        name = k.Split(',')[1]
                    })).Where(t => t.id == val || t.name == val)).ToList();

                    if (list.Count > 0)
                    {
                        result = list[0].name;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(val))
                {
                    result = val;
                }
            }
            return result;
        }

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[APIAuthorize]
        //public object AddProjectWorkPoints(ProjectWorkPointModel model)
        //{
        //    try
        //    {
        //        if (model == null)
        //        {
        //            return APIResult.GetErrorResult("请填写相关内容！");
        //        }
        //        var user = CurrentUserView;
        //        if (user != null)
        //        {
        //            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
        //            {
        //                int sort = 0;
        //                var pointsList = proxy.GetProjectPointsList(model.projectId);
        //                if (pointsList.Data.Count > 0)
        //                {
        //                    sort = pointsList.Data.OrderByDescending(t => t.Sort).FirstOrDefault().Sort.Value + 1;
        //                }
        //                List<Epm_ProjectWorkMainPoints> list = new List<Epm_ProjectWorkMainPoints>();
        //                Epm_ProjectWorkMainPoints point = new Epm_ProjectWorkMainPoints
        //                {
        //                    Id = model.id,
        //                    ProjectId = model.projectId,
        //                    Val = model.value
        //                };
        //                list.Add(point);
        //                var result = proxy.AddProjectPoints(list, model.projectId);
        //                if (result.Flag == EResultFlag.Success && result.Data > 0)
        //                {
        //                    return APIResult.GetSuccessResult("工程内容要点提交成功！");
        //                }
        //                return APIResult.GetErrorResult(result.Exception);
        //            }
        //        }
        //        return APIResult.GetErrorResult(MsgCode.InvalidToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        return APIResult.GetErrorResult(ex.Message);
        //    }
        //}

        /// <summary>
        /// 修改工程内容要点
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object EditProjectWorkPoints()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            string model = form["model"];
            List<ProjectWorkPointModel> list = new List<ProjectWorkPointModel>();
            List<Epm_ProjectWorkMainPoints> listPoints = new List<Epm_ProjectWorkMainPoints>();

            try
            {
                if (model == null)
                {
                    return APIResult.GetErrorResult("请填写相关内容！");
                }
                var user = CurrentUserView;
                if (user != null)
                {
                    if (!AppCommonHelper.CheckRight(BusinessType.Project, SystemRight.SetMainPoints, user))
                    {
                        return APIResult.GetErrorResult(MsgCode.Unauthorized);
                    }

                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        if (!string.IsNullOrEmpty(model))
                        {
                            list = JsonConvert.DeserializeObject<List<ProjectWorkPointModel>>(model);
                        }
                        if (list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                Epm_ProjectWorkMainPoints point = new Epm_ProjectWorkMainPoints
                                {
                                    Id = item.id,
                                    ProjectId = item.projectId,
                                    Val = item.key
                                };
                                listPoints.Add(point);
                            }
                        }

                        var result = proxy.UpdateProjectPoints(listPoints, listPoints[0].ProjectId.Value);
                        if (result.Flag == EResultFlag.Success && result.Data > 0)
                        {
                            return APIResult.GetSuccessResult("工程内容要点修改成功！");
                        }
                        return APIResult.GetErrorResult(result.Exception);
                    }
                }
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 获取项目服务商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetProjectCompanies(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetProjectCompanyList(projectId);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoData();
                    }
                    List<long> ids = new List<long>();
                    List<long> contractIds = result.Data.Where(t => t.ContractId > 0).Select(t => t.ContractId.Value).ToList();
                    List<long> orderIds = result.Data.Where(t => t.OrderId > 0).Select(t => t.OrderId.Value).ToList();

                    ids.AddRange(contractIds);
                    ids.AddRange(orderIds);

                    List<Base_Files> fileList = AppCommonHelper.GetBaseFileList(proxy, ids);

                    List<FileView> fileViewList = AppCommonHelper.GetFileList(fileList);

                    List<string> idseee = fileViewList.Select(t => t.id).ToList();

                    List<long> companyIds = result.Data.Where(t => t.CompanyId.HasValue).Select(t => t.CompanyId.Value).ToList();
                    var inWhr = companyIds.JoinToString(",");
                    QueryCondition qc = new QueryCondition();
                    qc.PageInfo.isAllowPage = false;
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "CompanyId",
                        ExpValue = inWhr,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.In
                    });

                    var baseUserListResult = proxy.GetUserList(qc);

                    List<Base_User> baseUserList = new List<Base_User>();
                    if (baseUserListResult.Flag == EResultFlag.Success && baseUserListResult.Data != null)
                    {
                        baseUserList = baseUserListResult.Data;
                    }


                    List<PeoplesView> namesList = new List<PeoplesView>();

                    if (result.Data.Count > 0)
                    {
                        foreach (var item in result.Data)
                        {
                            if (item.Type == "监理")
                            {
                                PeoplesView zj = new PeoplesView();
                                zj.type = "总监";
                                zj.id = item.PMId ?? 0;
                                zj.name = item.PM ?? "";
                                zj.phone = item.PMPhone ?? "";
                                zj.idNew = item.PMId_New ?? 0;
                                zj.nameNew = item.PM_New ?? "";
                                zj.phoneNew = item.PMPhone_New ?? "";
                                zj.ProjectCompanyId = item.Id.ToString();

                                PeoplesView xcjl = new PeoplesView();
                                xcjl.type = "现场监理";
                                xcjl.id = item.LinkManId ?? 0;
                                xcjl.name = item.LinkMan ?? "";
                                xcjl.phone = item.LinkPhone ?? "";
                                xcjl.idNew = item.LinkManId_New ?? 0;
                                xcjl.nameNew = item.LinkMan_New ?? "";
                                xcjl.phoneNew = item.LinkPhone_New ?? "";
                                xcjl.ProjectCompanyId = item.Id.ToString();

                                namesList.Add(zj);
                                namesList.Add(xcjl);
                            }
                            else if (item.Type == "设计费" || item.Type == "地勘" || item.Type == "危废处置")
                            {
                                PeoplesView zj = new PeoplesView();
                                zj.type = "本省地区负责人";
                                zj.id = item.PMId ?? 0;
                                zj.name = item.PM ?? "";
                                zj.phone = item.PMPhone ?? "";
                                zj.idNew = item.PMId_New ?? 0;
                                zj.nameNew = item.PM_New ?? "";
                                zj.phoneNew = item.PMPhone_New ?? "";
                                zj.ProjectCompanyId = item.Id.ToString();

                                namesList.Add(zj);
                            }
                            else if (item.Type == "土建" || item.Type == "内衬")
                            {
                                PeoplesView zfzr = new PeoplesView();
                                zfzr.type = "项目经理";
                                zfzr.id = item.PMId;
                                zfzr.name = item.PM;
                                zfzr.phone = item.PMPhone;
                                zfzr.idNew = item.PMId_New;
                                zfzr.nameNew = item.PM_New;
                                zfzr.phoneNew = item.PMPhone_New;
                                zfzr.ProjectCompanyId = item.Id.ToString();

                                PeoplesView xcfzr = new PeoplesView();
                                xcfzr.type = "现场负责人";
                                xcfzr.id = item.LinkManId;
                                xcfzr.name = item.LinkMan;
                                xcfzr.phone = item.LinkPhone;
                                xcfzr.idNew = item.LinkManId_New;
                                xcfzr.nameNew = item.LinkMan_New;
                                xcfzr.phoneNew = item.LinkPhone_New;
                                xcfzr.ProjectCompanyId = item.Id.ToString();

                                namesList.Add(zfzr);
                                namesList.Add(xcfzr);
                            }
                            else if (item.Type == "安装" || item.Type == "包装" || item.Type == "加固" || item.Type == "油罐清洗费")
                            {
                                PeoplesView zfzr = new PeoplesView();
                                zfzr.type = "本省地区负责人";
                                zfzr.id = item.PMId ?? 0;
                                zfzr.name = item.PM ?? "";
                                zfzr.phone = item.PMPhone ?? "";
                                zfzr.idNew = item.PMId_New ?? 0;
                                zfzr.nameNew = item.PM_New ?? "";
                                zfzr.phoneNew = item.PMPhone_New ?? "";
                                zfzr.ProjectCompanyId = item.Id.ToString();

                                PeoplesView xcfzr = new PeoplesView();
                                xcfzr.type = "现场负责人";
                                xcfzr.id = item.LinkManId ?? 0;
                                xcfzr.name = item.LinkMan ?? "";
                                xcfzr.phone = item.LinkPhone ?? "";
                                xcfzr.idNew = item.LinkManId_New ?? 0;
                                xcfzr.nameNew = item.LinkMan_New ?? "";
                                xcfzr.phoneNew = item.LinkPhone_New ?? "";
                                xcfzr.ProjectCompanyId = item.Id.ToString();

                                namesList.Add(zfzr);
                                namesList.Add(xcfzr);
                            }
                            else
                            {
                                PeoplesView zfzr = new PeoplesView();
                                zfzr.type = "本省地区负责人";
                                zfzr.id = item.PMId ?? 0;
                                zfzr.name = item.PM ?? "";
                                zfzr.phone = item.PMPhone ?? "";
                                zfzr.idNew = item.PMId_New ?? 0;
                                zfzr.nameNew = item.PM_New ?? "";
                                zfzr.phoneNew = item.PMPhone_New ?? "";
                                zfzr.ProjectCompanyId = item.Id.ToString();

                                namesList.Add(zfzr);
                            }
                        }
                    }

                    int isForS = 0;
                    bool isBCU = proxy.IsBranchCompanyUser(user.UserId);
                    if (isBCU)
                    {
                        isForS = 1;
                    }
                    else
                    {
                        bool isSupervisor = proxy.IsSupervisor(projectId, user.UserId);
                        if (isSupervisor)
                        {
                            isForS = 2;
                        }
                    }



                    var list = result.Data.OrderBy(t => t.IsAProvide).ThenBy(t => t.Sort).Select(p => new
                    {
                        id = p.Id,
                        companyId = p.CompanyId ?? 0,
                        companyName = p.CompanyName ?? "",
                        type = p.Type ?? "",
                        //linkManId = p.LinkManId ?? 0,
                        //linkMan = p.LinkMan ?? "",
                        //linkPhone = p.LinkPhone ?? "",
                        //pmManId = p.PMId ?? 0,
                        //pmMan = p.PM ?? "",
                        //pmPhone = p.PMPhone ?? "",
                        //linkManIdNew = p.LinkManId_New ?? 0,
                        //linkManNew = p.LinkMan_New ?? "",
                        //linkPhoneNew = p.LinkPhone_New ?? "",
                        //pmManIdNew = p.PMId_New ?? 0,
                        //pmManNew = p.PM_New ?? "",
                        //pmPhoneNew = p.PMPhone_New ?? "",
                        peoples = namesList.Where(t => t.ProjectCompanyId == p.Id.ToString()).Select(m => new
                        {
                            type = m.type ?? "",
                            id = m.id ?? 0,
                            name = m.name ?? "",
                            phone = m.phone ?? "",
                            idNew = m.idNew ?? 0,
                            nameNew = m.nameNew ?? "",
                            phoneNew = m.phoneNew ?? "",
                        }),
                        state = p.State,
                        userList = baseUserList.Where(t => t.CompanyId == p.CompanyId).Select(m => new { id = m.Id, name = m.UserName, phone = m.Phone }).ToList(),
                        files = fileViewList.Where(t => t.tableId.ToLongReq() == p.OrderId || t.tableId.ToLongReq() == p.ContractId).ToList(),
                        actionButton = AppCommonHelper.CreateButtonRightProject(user, BusinessType.Project.ToString(), p.State, isForS)
                    });
                    // TODO: 服务商相关合同附件处理
                    var data = new
                    {
                        list
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 修改项目服务商ProjectCompanyModel model
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object EditProjectCompany()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            string model = form["param"];
            string id = form["id"];
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写相关内容！");
                }
                List<PeoplesView> namesList = new List<PeoplesView>();
                var user = CurrentUserView;
                if (user != null)
                {
                    if (!AppCommonHelper.CheckRight(BusinessType.Project, SystemRight.SetCustomerUser, user))
                    {
                        return APIResult.GetErrorResult(MsgCode.Unauthorized);
                    }
                    if (!string.IsNullOrEmpty(model))
                    {
                        namesList = JsonConvert.DeserializeObject<List<PeoplesView>>(model);
                    }
                    Epm_ProjectCompany company = null;
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        if (namesList.Count > 0)
                        {
                            company = proxy.GetProjectCompany(id.ToLongReq()).Data;
                            foreach (var item in namesList)
                            {
                                //if (item.type == "现场监理" || item.type == "设计负责人" || item.type == "现场负责人")
                                //{
                                //    company.LinkManId_New = item.idNew;
                                //    company.LinkMan_New = item.nameNew;
                                //    company.LinkPhone_New = item.phoneNew;
                                //}
                                if (item.type == "总监" || item.type == "本省地区负责人" || item.type == "项目经理")
                                {
                                    company.PMId_New = item.idNew;
                                    company.PM_New = item.nameNew;
                                    company.PMPhone_New = item.phoneNew;
                                }
                                else
                                {
                                    company.LinkManId_New = item.idNew;
                                    company.LinkMan_New = item.nameNew;
                                    company.LinkPhone_New = item.phoneNew;
                                }
                                //if (item.type == "本省负责人")
                                //{
                                //    company.SafeManId_New = item.idNew;
                                //    company.SafeMan_New = item.nameNew;
                                //    company.SafePhone_New = item.phoneNew;
                                //}
                            }
                        }

                        var result = proxy.UpdateProjectCompanyPmInfo(company, user.UserId);
                        if (result.Flag == EResultFlag.Success && result.Data)
                        {
                            return APIResult.GetSuccessResult("修改项目服务商相关联系人信息操作成功！");
                        }
                        return APIResult.GetErrorResult(result.Exception);
                    }
                }
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <param name="name">供应商名称</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetSupplierCompany(string name, int pageIndex)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            CurrentPageIndex = pageIndex,
                            isAllowPage = true,
                            PageRowCount = 50
                        }
                    };

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "Name",
                            ExpValue = "%" + name + "%",
                            ExpOperater = eConditionOperator.Like,
                            ExpLogical = eLogicalOperator.And
                        });
                    }

                    var result = proxy.GetCompanyList(qc);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }
                    var data = result.Data.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, 50);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 获取外部手续
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetBluePrint(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var projectResult = proxy.GetProjectModel(projectId);

                    if (projectResult.Flag == EResultFlag.Success)
                    {
                        if (projectResult.Data == null)
                        {
                            return APIResult.GetErrorResult("所选项目不存在！");
                        }
                        var project = projectResult.Data;

                        List<Base_Files> fileList = AppCommonHelper.GetBaseFileList(proxy, project.Id).Where(t => t.TableColumn == "Constitute").ToList();

                        var data = AppCommonHelper.GetFileList(fileList);
                        return APIResult.GetSuccessResult(data);
                    }
                    return APIResult.GetErrorResult(projectResult.Exception);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 根据业务类型和项目 ID 获取业务数据列表
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <param name="projectId">项目 ID</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="version">接口版本</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetBusinessDataList(string type, long projectId, int pageIndex = 1, int version = 0)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return APIResult.GetErrorResult("请选择业务类型！");
            }
            if (type.ToLower().Equals(BusinessType.Draw.ToString().ToLower()))
            {
                return GetDrawList(projectId, pageIndex);
            }
            if (type.ToLower().Equals(BusinessType.Model.ToString().ToLower()))
            {
                return GetModelList(projectId, pageIndex);
            }
            if (type.ToLower().Equals(BusinessType.Schedule.ToString().ToLower()))
            {
                return GetScheduleList(projectId, version);
            }
            if (type.ToLower().Equals(BusinessType.Contract.ToString().ToLower()))
            {
                return GetContraceList(projectId, pageIndex);
            }
            if (type.ToLower().Equals(BusinessType.Question.ToString().ToLower()))
            {
                return GetQuestionList(projectId, "", pageIndex);
            }
            if (type.ToLower().Equals(BusinessType.Plan.ToString().ToLower()))
            {
                return GetPlanList(projectId, version);
            }
            if (type.ToLower().Equals(BusinessType.Change.ToString().ToLower()))
            {
                return GetChangeList(projectId, pageIndex);
            }
            if (type.ToLower().Equals(BusinessType.Visa.ToString().ToLower()))
            {
                return GetVisaList(projectId, pageIndex);
            }
            //if (type.ToLower().Equals(BusinessType.Log.ToString().ToLower()))
            //{
            //    return null;
            //}
            //if (type.ToLower().Equals(BusinessType.SecurityCheck.ToString().ToLower()))
            //{
            //    return GetMonitorList(projectId, pageIndex);
            //}
            return APIResult.GetErrorResult("请选择业务类型！");
        }
        #endregion

        #region 问题
        /// <summary>
        /// 根据项目 ID 和业务类型获取问题列表
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="type">业务类型</param>
        /// <param name="pageIndex">当前页码， 默认为 1</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetQuestionList(long projectId, string type, int pageIndex = 1)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(pageIndex)
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
                if (!string.IsNullOrWhiteSpace(type))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BusinessTypeNo",
                        ExpValue = type,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                }
                return GetQuestionByQc(qc, user);
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 根据所属项目及业务 ID 获取问题列表
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="businessId">业务 ID</param>
        /// <param name="type">业务类型</param>
        /// <param name="user">当前登录用户</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetDetailsQuestionList(long projectId, long businessId, string type, int pageIndex = 1)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(pageIndex, false)
                };
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "BusinessId",
                    ExpValue = businessId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                if (!string.IsNullOrWhiteSpace(type))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BusinessTypeNo",
                        ExpValue = type,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                }
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = 1,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                return GetQuestionByQc(qc, user);
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        private APIResult GetQuestionByQc(QueryCondition qc, UserView user)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                var result = proxy.GetQuestionList(qc);

                if (result.Flag == EResultFlag.Failure)
                {
                    return APIResult.GetErrorResult(result.Exception);
                }
                if (result.Data == null || !result.Data.Any())
                {
                    return APIResult.GetSuccessNoDatas();
                }
                var questionIds = result.Data.Select(p => p.Id).ToList();
                var userIds = result.Data.Select(p => p.SubmitUserId ?? 0).Distinct().ToList();

                var questionTrackResult = proxy.GetQuestionTrackCount(questionIds);
                Dictionary<long, int> dic = new Dictionary<long, int>();
                if (questionTrackResult.Flag == EResultFlag.Success && questionTrackResult.Data != null)
                {
                    dic = questionTrackResult.Data;
                }

                Func<long, int> getAnswerCount = delegate (long id)
                {
                    if (dic.ContainsKey(id))
                        return dic[id];
                    return 0;
                };

                Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, userIds);
                List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, questionIds);


                var data = result.Data.Select(p => new
                {
                    id = p.Id,
                    name = p.Title,
                    desc = p.Description,
                    type = p.BusinessTypeNo,
                    projectId = p.ProjectId,
                    submitUserName = p.SubmitUserName,
                    submitTime = string.Format("{0:yyyy-MM-dd HH:mm}", p.SubmitTime),
                    headerUrl = AppCommonHelper.GetUserProfilePhoto(p.SubmitUserId ?? 0, userPhoto, user),
                    //files = AppCommonHelper.GetFileList(files, user.UserCode),
                    files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.Id).ToList(), true),
                    answerCount = getAnswerCount(p.Id),
                    state = p.State
                });
                return APIResult.GetSuccessResult(data, qc.PageInfo.CurrentPageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
            }
        }

        /// <summary>
        /// 根据问题 ID 获取问题详情
        /// </summary>
        /// <param name="id">问题 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetQuestionDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择问题！");
            }
            var user = CurrentUserView;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                var result = proxy.GetQuestionModel(id);
                if (result.Flag == EResultFlag.Failure)
                {
                    return APIResult.GetErrorResult(result.Exception);
                }
                if (result.Data == null)
                {
                    return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                }
                Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { result.Data.SubmitUserId });

                // 项目所需权限
                List<Button> btns = new List<Button>();
                if (user != null)
                {
                    var userRight = user.Rights.Where(p => p.Value.Contains(BusinessType.Question.ToString()));
                    if (userRight.Any() && result.Data.State == 1)
                    {
                        foreach (var keyValuePair in userRight)
                        {
                            if (keyValuePair.Value.Contains(SystemRight.Reply.ToString()))
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = SystemRight.Reply.GetText();
                                btn.rightAction = SystemRight.Reply.ToString();
                                btn.color = "";
                                btns.Add(btn);
                                continue;
                            }
                            if (keyValuePair.Value.Contains(SystemRight.Close.ToString()) && user.UserId == result.Data.CreateUserId)
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = SystemRight.Close.GetText();
                                btn.rightAction = SystemRight.Close.ToString();
                                btn.color = "";
                                btns.Add(btn);
                            }
                        }
                    }
                }

                string bim = "";
                if (result.Data.QuestionBims != null && result.Data.QuestionBims.Count() > 0)
                {
                    bim = AppCommonHelper.SystemSetting["relationBimUrl"] + result.Data.Id + "&bimId=" + result.Data.QuestionBims.FirstOrDefault().BIMId + "&projectId=" + result.Data.ProjectId;
                }

                var questionResult = proxy.GetQuestionTrackCount(new List<long>() { id });

                var data = new
                {
                    id = result.Data.Id,
                    projectName = result.Data.ProjectName ?? "",
                    name = result.Data.Title ?? "",
                    answerCount = ((questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0) ? questionResult.Data[id].ToString() : ""),
                    submitTime = string.Format("{0:yyyy-MM-dd}", result.Data.SubmitTime),
                    headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.SubmitUserId, userPhoto, user),
                    state = result.Data.State,
                    isAccident = result.Data.IsAccident,
                    submitUserName = result.Data.SubmitUserName ?? "",
                    type = result.Data.BusinessTypeName ?? "",
                    problemTypeName = result.Data.ProblemTypeName,
                    receiveCompanyName = result.Data.RecCompanyName ?? "",
                    relationBim = bim,
                    desc = result.Data.Description ?? "",
                    proposal = result.Data.Proposal ?? "",
                    files = AppCommonHelper.GetFileList(result.Data.Attachs),
                    actionButton = result.Data.State == 2 ? null : btns
                };
                return APIResult.GetSuccessResult(data);
            }
        }

        /// <summary>
        /// 新增问题
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddQuesiont(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    List<Epm_ProjectCompany> companyList = new List<Epm_ProjectCompany>();
                    List<Base_TypeDictionary> typeList = new List<Base_TypeDictionary>();
                    var companyResult = proxy.GetProjectCompanyList(projectId);

                    if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null)
                    {
                        companyList = companyResult.Data;
                    }

                    List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.ProblemType };
                    var dicResult = proxy.GetTypeListByTypes(list);
                    if (dicResult.Flag == EResultFlag.Success && dicResult.Data != null &&
                        dicResult.Data.ContainsKey(DictionaryType.ProblemType))
                    {
                        typeList = dicResult.Data[DictionaryType.ProblemType];
                    }

                    QueryCondition qc = new QueryCondition
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = false
                        }
                    };
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BIMState",
                        ExpValue = BIMModelState.BIMLightWeightSuccess.ToString(),
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    var bimList = proxy.GetBimList(qc);
                    var data = new
                    {
                        companies = companyList.Where(p => !string.IsNullOrEmpty(p.CompanyName)).Select(p => new
                        {
                            id = (p.CompanyId == null) ? "" : p.CompanyId.ToString(),
                            name = p.CompanyName
                        }).Distinct(),
                        typeList = typeList.Select(p => new
                        {
                            id = p.No,
                            name = p.Name
                        }),
                        url = bimList.Data.Count > 0 ? AppCommonHelper.SystemSetting["relationBimUrl"] + "&projectId=" + projectId : "",
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 提交问题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddQuestion()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            //QuestionInfoModel model
            if (form["projectId"].ToLongReq() <= 0 || string.IsNullOrWhiteSpace(form["projectName"]))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }

            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                QuestionView view = new QuestionView();

                view.ProjectId = form["projectId"].ToLongReq();
                view.ProjectName = form["projectName"];
                view.ProblemTypeNo = form["type"];
                view.ProblemTypeName = form["typeName"];
                view.BusinessTypeNo = form["businessType"];
                view.BusinessTypeName = form["businessName"];
                view.BusinessId = string.IsNullOrEmpty(form["businessId"]) ? 0 : form["businessId"].ToLongReq();
                view.Title = form["title"];
                view.Description = form["desc"];
                view.Proposal = form["proposal"];
                view.IsAccident = form["isAccident"] == "true" ? true : form["isAccident"] == "false" ? false : form["isAccident"].ToBoolReq();
                view.RecCompanyId = form["companyId"].ToLongReq();
                view.RecCompanyName = form["companyName"];

                // todo:附件处理
                view.Attachs = AppCommonHelper.UploadFile(http, user);
                // todo:BIM 模型处理
                view.QuestionBims = JsonConvert.DeserializeObject<List<Epm_QuestionBIM>>(form["bim"]);

                var result = proxy.AddQuestion(view);
                if (result.Flag == EResultFlag.Success)
                {
                    return APIResult.GetSuccessResult("问题提交成功！");
                }
                return APIResult.GetErrorResult(result.Exception);
            }
        }

        /// <summary>
        /// 根据问题 ID 获取问题回复列表
        /// </summary>
        /// <param name="questionId">问题 ID</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetQuestionTrack(long questionId, int pageIndex = 1)
        {
            var user = CurrentUserView;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(pageIndex)
                };

                if (pageIndex == 0)
                {
                    qc.PageInfo = GetPageInfo(1, false);
                }

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "QuestionId",
                    ExpValue = questionId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                var result = proxy.GetQuestionTrack(qc);

                if (result.Flag == EResultFlag.Failure)
                {
                    return APIResult.GetErrorResult(result.Exception);
                }
                if (result.Data == null || !result.Data.Any())
                {
                    return APIResult.GetSuccessNoDatas();
                }

                List<long> userIds = result.Data.Select(p => p.CreateUserId).Distinct().ToList();
                Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, userIds);

                var businessIds = result.Data.Select(p => p.QuestionId ?? 0).ToList();
                List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds);

                var data = result.Data.Select(p => new
                {
                    id = p.Id,
                    content = p.Content,
                    submitUser = p.CreateUserName,
                    headerUrl = AppCommonHelper.GetUserProfilePhoto(p.CreateUserId, userPhoto, null),
                    submitTime = string.Format("{0:yyyy-MM-dd HH:mm}", p.CreateTime),
                    files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.Id).ToList(), true)
                });
                return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
            }
        }

        /// <summary>
        /// 回复或关闭问题
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object OperationQuestion(BusinessCheck model, UserView user)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                if (model.rightAction.ToLower().Equals(SystemRight.Close.ToString().ToLower()))
                {
                    var result = proxy.CloseQuestion(model.businessId);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = 2,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
                if (model.rightAction.ToLower().Equals(SystemRight.Reply.ToString().ToLower()))
                {
                    Epm_QuestionTrack track = new Epm_QuestionTrack
                    {
                        QuestionId = model.businessId,
                        Content = model.reason
                    };

                    var result = proxy.ReplyQuestion(track);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    return APIResult.GetSuccessResult("操作成功！");
                }
                return APIResult.GetErrorResult("操作失败！");
            }
        }
        #endregion

        #region 沟通
        /// <summary>
        /// 获取沟通列表
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="key"></param>
        /// <param name="type">沟通类型</param>>
        /// <param name="time">发布时间</param>>
        /// <param name="publisherId">发布人 ID</param>>
        /// <param name="state">问题状态</param>
        /// <param name="pageIndex">当前页码，默认为 1</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetApproverList(long projectId, string type, string time, string key = "", string state = "-1", long? publisherId = 0, int pageIndex = 1)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BusinessTypeNo",
                        ExpValue = "Completed",
                        ExpOperater = eConditionOperator.NotEqual,
                        ExpLogical = eLogicalOperator.And
                    });
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsApprover",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
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
                    else
                    {
                        var projectIds = proxy.GetProjectListById(user.CompanyId, user.UserId).Data.Select(p => p.Id).ToList();
                        if (projectIds.Count > 0)
                        {
                            var inWhr = projectIds.JoinToString(",");
                            qc.ConditionList.Add(new ConditionExpression()
                            {
                                ExpName = "ProjectId",
                                ExpValue = inWhr,
                                ExpLogical = eLogicalOperator.And,
                                ExpOperater = eConditionOperator.In
                            });
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(type))
                    {
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "BusinessTypeNo",
                            ExpValue = "%" + type + "%",
                            ExpOperater = eConditionOperator.Like,
                            ExpLogical = eLogicalOperator.And
                        });
                    }
                    if (publisherId > 0)
                    {
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "SendUserId",
                            ExpValue = publisherId,
                            ExpOperater = eConditionOperator.Equal,
                            ExpLogical = eLogicalOperator.And
                        });
                    }
                    if (!string.IsNullOrWhiteSpace(time))
                    {
                        DateTime timeValue;
                        if (DateTime.TryParse(time, out timeValue))
                        {
                            qc.ConditionList.Add(new ConditionExpression()
                            {
                                ExpName = "SendTime",
                                ExpValue = timeValue,
                                ExpOperater = eConditionOperator.GreaterThanOrEqual,
                                ExpLogical = eLogicalOperator.And
                            });
                            qc.ConditionList.Add(new ConditionExpression()
                            {
                                ExpName = "SendTime",
                                ExpValue = timeValue.AddDays(1),
                                ExpOperater = eConditionOperator.LessThan,
                                ExpLogical = eLogicalOperator.And
                            });
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        int stateValue;
                        if (int.TryParse(state, out stateValue))
                        {
                            switch (key)
                            {
                                // 待审批
                                case "1":
                                    {
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "BusinessState",
                                            ExpValue = string.Format("{0},{1},{2},{3},{4}", (int)ApprovalState.WaitAppr, (int)ConfirmState.WaitConfirm, (int)ApprovalState.WorkPartAppr, (int)RectificationState.Rectificationed, (int)RectificationState.WaitRectification),
                                            ExpOperater = eConditionOperator.In,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "ApproverId",
                                            ExpValue = user.UserId,
                                            ExpOperater = eConditionOperator.Equal,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                    }
                                    break;
                                // 已提报
                                case "2":
                                    {
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "SendUserId",
                                            ExpValue = user.UserId,
                                            ExpOperater = eConditionOperator.Equal,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                    }
                                    break;
                                // 已审核
                                case "3":
                                    {
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "BusinessState",
                                            ExpValue = string.Format("{0},{1},{2},{3},{4},{5}", (int)ApprovalState.ApprSuccess, (int)ApprovalState.ApprFailure, (int)ConfirmState.Confirm, (int)ConfirmState.ConfirmFailure, (int)RectificationState.RectificationSuccess, (int)RectificationState.RectificationOk),
                                            ExpOperater = eConditionOperator.In,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "ApproverId",
                                            ExpValue = user.UserId,
                                            ExpOperater = eConditionOperator.Equal,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                    }
                                    break;
                                default:
                                    {
                                        qc.ConditionList.Add(new ConditionExpression()
                                        {
                                            ExpName = "BusinessState",
                                            ExpValue = stateValue,
                                            ExpOperater = eConditionOperator.Equal,
                                            ExpLogical = eLogicalOperator.And
                                        });
                                    }
                                    break;
                            }
                        }
                    }

                    var result = proxy.GetQuestions(qc);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }

                    var businessIds = result.Data.Select(p => p.businessId).ToList();
                    var userIds = result.Data.Select(p => p.createUserId).Distinct().ToList();

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, userIds);

                    List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds);

                    var data = result.Data.Select(p => new
                    {
                        p.id,
                        name = p.name ?? "",
                        files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.businessId).ToList(), true).ToList(),
                        p.answerCount,
                        submitTime = string.Format("{0:yyyy-MM-dd}", p.submitTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(p.createUserId, userPhoto, user),
                        type = p.type ?? "",
                        businessChild = p.businessChild ?? "",
                        p.state,
                        p.businessId,
                        submitUserName = p.submitUserName ?? "",
                        workContent = Utils.CutString(p.workContent, 140),
                        p.projectId,
                        p.projectName,
                        p.businessState,
                        p.action
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }
        #endregion

        #region 详情
        /// <summary>
        /// 根据业务类型和业务 ID 获取业务数据详情
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <param name="id">业务 ID</param>
        /// <returns></returns>
        [HttpGet]
        public object GetBusinessDataDetail(string type, long id)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return APIResult.GetErrorResult("请选择业务类型！");
            }

            if (type.ToLower().Equals(BusinessType.Draw.ToString().ToLower()))
            {
                return GetDrawDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Model.ToString().ToLower()))
            {
                return GetModelDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Contract.ToString().ToLower()))
            {
                return GetContractDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Question.ToString().ToLower()))
            {
                return GetQuestionDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Change.ToString().ToLower()))
            {
                return GetChangeDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Visa.ToString().ToLower()))
            {
                return GetVisaDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Log.ToString().ToLower()))
            {
                //return GetSupervisorLogDetail(id);//旧接口日志查询
                return GetSupervisorNewLogDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.SecurityCheck.ToString().ToLower()))
            {
                return GetMonitorDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Rectification.ToString().ToLower()))
            {
                return GetRecifDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Dangerous.ToString().ToLower()))
            {
                return GetDangerousWorkDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Equipment.ToString().ToLower()))
            {
                return GetMaterialDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.Track.ToString().ToLower()))
            {
                return GetMaterielDetail(id);
            }
            if (type.ToLower().Equals(BusinessType.DelayApply.ToString().ToLower()))
            {
                return GetPlanDelayDetail(id);
            }
            return APIResult.GetErrorResult("请选择业务类型！");
        }
        #endregion

        #region 消息
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetMsgList(int pageIndex = 1)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BussinesType",
                        ExpValue = "Completed",
                        ExpOperater = eConditionOperator.NotEqual,
                        ExpLogical = eLogicalOperator.And
                    });

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsDelete",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });

                    ConditionExpression ce4 = new ConditionExpression();
                    ce4.ExpLogical = eLogicalOperator.And;
                    ce4.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "RecId",
                        ExpValue = user.UserId,
                        ExpOperater = eConditionOperator.Equal
                    });
                    ce4.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Type",
                        ExpValue = 1,
                        ExpLogical = eLogicalOperator.Or,
                        ExpOperater = eConditionOperator.Equal
                    });
                    qc.ConditionList.Add(ce4);

                    var result = proxy.GetMassageList(qc);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }
                    var data = result.Data.Select(p => new
                    {
                        id = p.Id,
                        title = p.Title ?? "",
                        content = p.Content ?? "",
                        isRead = p.IsRead,
                        sendName = p.SendName,
                        sendTime = p.SendTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        type = p.Type,
                        projectId = p.ProjectId,
                        projectName = p.ProjectName,
                        businessId = p.BussinessId,
                        businessType = p.BussinesType ?? "",
                        businessChild = p.BussinesChild ?? "",
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 设置消息已读未读接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object SetMsgState()
        {
            Result<int> result = new Result<int>();
            var http = HttpContext.Current;
            var form = http.Request.Form;
            string ids = form["ids"];
            string isall = form["isAll"];
            string state = form["state"];
            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("未登录或登录超时！");
                }
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    bool all = (isall == "true" ? true : isall == "false" ? false : isall.ToBoolReq());
                    bool statu = (state == "true" ? true : state == "false" ? false : state.ToBoolReq());
                    if (all)
                    {
                        result = proxy.UpdateAllMassageState(user.UserId, statu);
                    }
                    else {
                        List<Epm_Massage> list = new List<Epm_Massage>();
                        if (!string.IsNullOrEmpty(ids))
                        {
                            list = JsonConvert.DeserializeObject<List<Epm_Massage>>(ids);
                        }

                        if (list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                Epm_Massage model = new Epm_Massage();
                                model.Id = item.Id;
                                model.IsRead = statu;
                                model.ReadTime = DateTime.Now;

                                result = proxy.UpdateMassage(model);
                            }
                        }
                    }
                    if (result.Flag == EResultFlag.Success)
                    {
                        return APIResult.GetSuccessResult("消息已标记" + (statu ? "已读" : "未读") + "！");
                    }
                    return APIResult.GetSuccessResult(result.Exception);
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 获取登录用户待办事项总数和未读消息总数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetMsgAndApproverNum()
        {
            var user = CurrentUserView;
            if (user != null)
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    #region 代办条件
                    QueryCondition approverQc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = false
                        }
                    };
                    approverQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BusinessTypeNo",
                        ExpValue = "Completed",
                        ExpOperater = eConditionOperator.NotEqual,
                        ExpLogical = eLogicalOperator.And
                    });
                    approverQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ApproverId",
                        ExpValue = user.UserId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    approverQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsApprover",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    approverQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsDelete",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    approverQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "BusinessState",
                        ExpValue = string.Format("{0},{1},{2},{3},{4} ", (int)ApprovalState.WaitAppr, (int)ConfirmState.WaitConfirm, (int)ApprovalState.WorkPartAppr, (int)RectificationState.WaitRectification, (int)RectificationState.Rectificationed),
                        ExpOperater = eConditionOperator.In,
                        ExpLogical = eLogicalOperator.And
                    });
                    #endregion

                    var approverResult = proxy.GetApproverList(approverQc);

                    #region 待办消息条件
                    QueryCondition msgQc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = false
                        }
                    };
                    msgQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "RecId",
                        ExpValue = user.UserId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    msgQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsRead",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    msgQc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsDelete",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    #endregion

                    var msgResult = proxy.GetMassageList(msgQc);

                    #region 系统消息条件
                    QueryCondition msgQc1 = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = false
                        }
                    };
                    msgQc1.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Type",
                        ExpValue = 1,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    msgQc1.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsRead",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    msgQc1.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "IsDelete",
                        ExpValue = false,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    #endregion

                    var sysResult = proxy.GetMassageList(msgQc1);
                    var data = new
                    {
                        approvalCount = approverResult.AllRowsCount,
                        unreadMsg = msgResult.AllRowsCount + sysResult.AllRowsCount
                    };
                    return Json(APIResult.GetSuccessResult(data));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }
        #endregion

        #region 分页、权限、下载
        /// <summary>
        /// 获取新增页面预加载数据接口
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <param name="projectId">所属项目 ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetAddPageData(string type, long projectId, string time = "")
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return APIResult.GetErrorResult("请选择业务类型！");
            }
            if (type.ToLower().Equals(BusinessType.SecurityCheck.ToString().ToLower()))
            {
                return AddMonitor(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Log.ToString().ToLower()))
            {
                return AddSupervisorLog(projectId, time);
            }
            if (type.ToLower().Equals(BusinessType.DelayApply.ToString().ToLower()))
            {
                return AddPlanDelay(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Dangerous.ToString().ToLower()))
            {
                return AddDangerousWork(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Visa.ToString().ToLower()))
            {
                return AddVisa(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Equipment.ToString().ToLower()) ||
                type.Equals(BusinessType.Track.ToString().ToLower()))
            {
                return AddMaterial(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Question.ToString().ToLower()))
            {
                return AddQuesiont(projectId);
            }
            if (type.ToLower().Equals(SystemRight.UploadWork.ToString().ToLower()))
            {
                return AddWorkRealScenen(projectId);
            }
            if (type.ToLower().Equals(SystemRight.UploadSecurityCheck.ToString().ToLower()))
            {
                return AddRectifRecord(projectId);
            }
            if (type.ToLower().Equals(BusinessType.Rectification.ToString().ToLower()))
            {
                return NewAddMonitor(projectId);
            }
            return APIResult.GetErrorResult("请选择具体业务类型！");
        }

        /// <summary>
        /// 业务权限操作
        /// <para>变更、签证、延期申请、问题、验收(接收)</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object DoBusiness(BusinessCheck model)
        {
            if (string.IsNullOrWhiteSpace(model.businessType))
            {
                return APIResult.GetErrorResult("请选择业务类型");
            }
            if (string.IsNullOrWhiteSpace(model.rightAction))
            {
                return APIResult.GetErrorResult("请选择操作类型！");
            }

            string businessTpye = model.businessType.ToLower();

            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            if (businessTpye.Equals(BusinessType.Change.ToString().ToLower()))
            {
                return AuditChange(model, user);
            }
            if (businessTpye.Equals(BusinessType.Visa.ToString().ToLower()))
            {
                return AuditVisa(model, user);
            }
            if (businessTpye.Equals(BusinessType.Equipment.ToString().ToLower()) || businessTpye.Equals(BusinessType.Track.ToString().ToLower()))
            {
                return AuditMaterial(model, user);
            }
            if (businessTpye.Equals(BusinessType.DelayApply.ToString().ToLower()))
            {
                return AuditPlanDealy(model, user);
            }
            if (businessTpye.Equals(BusinessType.Question.ToString().ToLower()))
            {
                return OperationQuestion(model, user);
            }
            if (businessTpye.Equals(BusinessType.Contract.ToString().ToLower()))
            {
                return AuditContract(model, user);
            }
            if (businessTpye.Equals(BusinessType.Draw.ToString().ToLower()))
            {
                return AuditDraw(model, user);
            }
            if (businessTpye.Equals(BusinessType.Model.ToString().ToLower()))
            {
                return AuditModel(model, user);
            }
            if (businessTpye.Equals(BusinessType.Dangerous.ToString().ToLower()))
            {
                return AuditDangerous(model, user);
            }
            if (businessTpye.Equals(BusinessType.Log.ToString().ToLower()))
            {
                return AuditSupervisorLog(model, user);
            }
            if (businessTpye.Equals(BusinessType.Project.ToString().ToLower()))
            {
                return AuditProjectCompanies(model, user);
            }
            if (businessTpye.Equals(BusinessType.Rectification.ToString().ToLower()))
            {
                return AuditSecurityCheck(model, user);
            }

            return APIResult.GetErrorResult("操作失败！");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object DownloadFile(long id)
        {
            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                var result = proxy.GetBaseFile(id);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    var file = result.Data;

                    byte[] bytes = AppCommonHelper.GetFilesBytes(file.Group, file.Url, user.UserCode, file.Name);

                    if (bytes.Length <= 0)
                    {
                        return Json(APIResult.GetErrorResult("文件已丢失或已被删除！"));
                    }
                    else
                    {
                        return Json(APIResult.GetSuccessResult(bytes));
                    }
                }
                else
                {
                    return Json(APIResult.GetErrorResult("文件已丢失或已被删除！"));
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取新增页面预加载数据接口
        /// </summary>
        /// <param name="projectId">所属项目 ID</param>
        /// <param name="checkidlist">级别ID</param>
        /// <param name="level">级别ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object LevelChecklist(long projectId, long checkidlist, int level)
        {
            return LevelChecklists(projectId, checkidlist, level);
        }
    }
}