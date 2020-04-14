using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.Plat.WebAPI.Base.Common;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.Plat.Common.Extend;
using System.Configuration;

namespace hc.Plat.WebAPI.Base.Controllers
{
    public partial class ProjectController
    {
        #region 图纸

        /// <summary>
        /// 获取项目图纸接口
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        private object GetDrawList(long projectId, int pageIndex = 1)
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
                    QueryCondition qc = new QueryCondition
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });

                    var result = proxy.GetDrawList(qc);

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
                        name = p.Name ?? "",
                        versionName = p.VersionName ?? "",
                        isNew = p.IsNew ?? false,
                        state = p.State ?? 0,
                        submitUserName = p.CreateUserName ?? "",
                        submitTime = string.Format("{0:yyyy-MM-dd}", p.SubmitDate)
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 根据图纸 ID 获取图纸详情
        /// </summary>
        /// <param name="id">图纸 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetDrawDetail(long id)
        {
            if (id <= 0)
            {
                return Json(APIResult.GetErrorResult("请选择图纸！"));
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetDrawModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }

                    var draw = result.Data;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { result.Data.CreateUserId });

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Draw.ToString();
                    var data = new
                    {
                        id = draw.Id,
                        projectName = draw.ProjectName ?? "",
                        name = draw.Name ?? "",
                        desc = draw.Desciption ?? "",
                        version = draw.VersionOrder ?? "",
                        versionName = draw.VersionName ?? "",
                        submitUserName = draw.SubmitUserName ?? "",
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(draw.CreateUserId, userPhoto, user),
                        submitTime = string.Format("{0:yyyy-MM-dd}", draw.SubmitDate),
                        submitCompanyName = draw.SubmitCompanyName ?? "",
                        state = draw.State ?? 0,
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        remark = draw.Remark,
                        answerCount = answerCount,
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)draw.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, draw.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 图纸的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditDraw(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteDrawByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.RejectDraw(model.businessId, ((int)state).ToString(), "");
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 模型

        /// <summary>
        /// 根据项目 ID 获取模型列表
        /// </summary>
        /// <param name="projectId">项目 ID ps:1013047335028133888</param>
        /// <param name="pageIndex">当前页码，默认为 1</param>
        /// <returns></returns>
        private object GetModelList(long projectId = 0, int pageIndex = 1)
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
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });

                    var result = proxy.GetBimList(qc);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }

                    var businessIds = result.Data.Select(p => p.Id).ToList();
                    List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds, true);

                    var data = result.Data.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name ?? "",
                        type = AppCommonHelper.GetFileType(p.Name).ToString(),
                        imageUrlSmall = (p.BIMState == BIMModelState.NotBIM.ToString() ? "" : (files.Where(x => x.TableId == p.Id && x.TableColumn == "Thumbnail").FirstOrDefault() == null ? (AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/bimdoing.png") : (AppCommonHelper.SystemSetting["resourceUrl"] + files.Where(x => x.TableId == p.Id && x.TableColumn == "Thumbnail").FirstOrDefault().Url))),
                        imageUrlBimg = "",
                        url = (p.BIMState == BIMModelState.NotBIM.ToString() ? "" : AppCommonHelper.SystemSetting["bimUrl"]),
                        size = "",
                        time = string.Format("{0:yyyy-MM-dd}", p.SubmitDate),
                        state = (int)((BIMModelState)Enum.Parse(typeof(BIMModelState), p.BIMState))
                    });

                    return Json(APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 根据模型ID 获取模型详情
        /// </summary>
        /// <param name="id">模型ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetModelDetail(long id)
        {
            if (id <= 0)
            {
                return Json(APIResult.GetErrorResult("请选择模型！"));
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetBimModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }

                    var model = result.Data;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { model.CreateUserId });

                    string businessType = BusinessType.Model.ToString();

                    var businessIds = new List<long>() { model.Id };
                    List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds, true);

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    var file = new List<Epm_Bim>() { model }.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name ?? "",
                        type = AppCommonHelper.GetFileType(p.Name).ToString(),
                        imageUrlSmall = (p.BIMState == BIMModelState.NotBIM.ToString() ? "" : (files.Where(x => x.TableColumn == "Thumbnail").FirstOrDefault() == null ? (AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/bimdoing.png") : (AppCommonHelper.SystemSetting["resourceUrl"] + files.Where(x => x.TableColumn == "Thumbnail").FirstOrDefault().Url))),
                        imageUrlBimg = "",
                        url = AppCommonHelper.SystemSetting["bimUrl"],
                        size = "",
                        time = string.Format("{0:yyyy-MM-dd}", p.SubmitDate),
                        state = (int)((BIMModelState)Enum.Parse(typeof(BIMModelState), p.BIMState))
                    });

                    var data = new
                    {
                        id = model.Id,
                        projectName = model.ProjectName ?? "",
                        name = model.Name ?? "",
                        desc = model.Desciption ?? "",
                        version = model.VersionOrder ?? "",
                        versionName = model.VersionName ?? "",
                        submitUserName = model.SubmitUserName ?? "",
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(model.CreateUserId, userPhoto, user),
                        submitTime = string.Format("{0:yyyy-MM-dd}", model.SubmitDate),
                        submitCompanyName = model.SubmitCompanyName ?? "",
                        state = model.State ?? 0,
                        files = file,
                        remark = model.Remark,
                        answerCount = answerCount,
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)model.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, model.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 模型的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditModel(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteBimByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.RejectBim(model.businessId, ((int)state).ToString(), "");
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 进度

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetPlannedList(long projectId, long planId = 0)
        {
            return APIResult.GetSuccessResult(null);
        }

        #endregion

        #region 签证

        /// <summary>
        /// 获取签证列表
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <param name="pageIndex">当前页数</param>
        /// <returns></returns>
        private object GetVisaList(long projectId, int pageIndex)
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
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });

                    var result = proxy.GetVisaListByQc(qc);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }
                    var businessIds = result.Data.Select(p => p.Id).ToList();
                    var userIds = result.Data.Select(p => p.CreateUserId).Distinct().ToList();
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, userIds);
                    List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds);

                    var data = result.Data.Select(p => new
                    {
                        id = p.Id,
                        visaTypeName = p.VisaTypeName ?? "",
                        amount = p.VisaAmount.HasValue ? p.VisaAmount.ToString("0.###### ") : "0 ",
                        submitTime = string.Format("{0:yyyy-MM-dd}", p.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(p.CreateUserId, userPhoto, user),
                        submitUserName = p.CreateUserName ?? "",
                        state = p.State ?? 0,
                        answerCount = 0,
                        files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.Id).ToList(), true),
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 根据签证 ID 获取签证详情
        /// </summary>
        /// <param name="id">签证 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetVisaDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择签证！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetVisaModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    var visa = result.Data;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { visa.CreateUserId });

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Visa.ToString();
                    var data = new
                    {
                        id = visa.Id,
                        name = visa.VisaTitle ?? "",
                        projectName = visa.ProjectName ?? "",
                        visaCode = visa.VisaNo,
                        visaTypeName = visa.VisaTypeName ?? "",
                        amount = visa.VisaAmount.HasValue ? visa.VisaAmount.ToString("0.###### ") : "0 ",
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        submitTime = string.Format("{0:yyyy-MM-dd}", visa.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(visa.CreateUserId, userPhoto, user),
                        state = visa.State,
                        submitUserName = visa.CreateUserName ?? "",
                        visaStartTime = string.Format("{0:yyyy-MM-dd}", visa.VisaStartTime),
                        visaEndTime = string.Format("{0:yyyy-MM-dd}", visa.VisaEndTime),
                        visaContent = visa.VisaContent ?? "",
                        visaReason = visa.VisaResean,
                        answerCount = answerCount,
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)visa.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, visa.CreateUserId))
                    };
                    return Json(APIResult.GetSuccessResult(data));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取签证提交页面预加载数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddVisa(long projectId)
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
                    List<Base_TypeDictionary> typeList = new List<Base_TypeDictionary>();

                    List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.VisaType };
                    var dicResult = proxy.GetTypeListByTypes(list);
                    if (dicResult.Flag == EResultFlag.Success && dicResult.Data != null &&
                        dicResult.Data.ContainsKey(DictionaryType.VisaType))
                    {
                        typeList = dicResult.Data[DictionaryType.VisaType];
                    }

                    var data = new
                    {
                        typeList = typeList.Select(p => new
                        {
                            id = p.No,
                            name = p.Name
                        })
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 新增签证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddVisa()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["projectId"].ToLongReq() == 0 || string.IsNullOrWhiteSpace(form["projectName"]))
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
                VisaView visa = new VisaView();
                visa.ProjectId = form["projectId"].ToLongReq();
                visa.ProjectName = form["projectName"];
                visa.VisaTypeName = form["typeName"];
                visa.VisaNo = form["code"];
                visa.VisaTitle = form["title"];
                visa.VisaContent = form["content"];
                visa.VisaResean = form["reason"];
                visa.VisaAmount = form["amount"].ToDecimal();
                visa.VisaStartTime = form["startTime"].ToDateTimeReq();
                visa.VisaEndTime = form["endTime"].ToDateTimeReq();
                visa.State = (int)ApprovalState.WaitAppr;

                List<Base_Files> files = AppCommonHelper.UploadFile(http, user);

                var result = proxy.AddVisa(visa, files);

                if (result.Flag == EResultFlag.Success)
                {
                    return APIResult.GetSuccessResult("签证提交成功！");
                }
                return APIResult.GetErrorResult(result.Exception);
            }
        }


        /// <summary>
        /// 签证的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditVisa(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteVisaByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.UpdateVisaState(model.businessId, ((int)state).ToString());

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }


        #endregion

        #region 变更

        /// <summary>
        /// 根据项目 ID 获取变更列表
        /// </summary>
        /// <param name="projectId">项目 ID ps：1013047335028133888</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        private object GetChangeList(long projectId, int pageIndex = 1)
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
                    var project = proxy.GetProjectModel(projectId);

                    if (project.Flag == EResultFlag.Success)
                    {
                        if (project.Data == null)
                        {
                            return APIResult.GetErrorResult("所选项目不存在！");
                        }

                        var projectName = project.Data.Name;

                        QueryCondition qc = new QueryCondition();
                        ConditionExpression ce = null;
                        if (projectId > 0)
                        {
                            ce = new ConditionExpression();
                            ce.ExpName = "ProjectId";
                            ce.ExpValue = projectId;
                            ce.ExpOperater = eConditionOperator.Equal;
                            ce.ExpLogical = eLogicalOperator.And;
                            qc.ConditionList.Add(ce);
                        }
                        qc.PageInfo = GetPageInfo(pageIndex);
                        var result = proxy.GetChangListByQc(qc);
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }

                        if (result.Data == null || !result.Data.Any())
                        {
                            return APIResult.GetSuccessNoDatas();
                        }
                        var businessIds = result.Data.Select(p => p.Id).ToList();
                        var userIds = result.Data.Select(p => p.SubmitUserId ?? 0).Distinct().ToList();
                        Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, userIds);
                        List<Base_Files> files = AppCommonHelper.GetBaseFileList(proxy, businessIds);

                        var questionResult = proxy.GetQuestionCount(businessIds);
                        Dictionary<long, int> dic = new Dictionary<long, int>();
                        if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null)
                        {
                            dic = questionResult.Data;
                        }
                        Func<long, int> getQuestionCount = delegate (long id)
                        {
                            if (dic.ContainsKey(id))
                                return dic[id];
                            return 0;
                        };

                        var data = result.Data.Select(p => new
                        {
                            id = p.Id,
                            amount = p.TotalAmount.HasValue ? p.TotalAmount.ToString("0.###### ") : "0 ",
                            changeAmount = p.ChangeAmount.HasValue ? p.ChangeAmount.ToString("0.###### ") : "0 ",
                            increaseAmount = p.AddAmount.HasValue ? p.AddAmount.ToString("0.###### ") : "0 ",
                            reduceAmount = p.ReduceAmount.HasValue ? p.ReduceAmount.ToString("0.###### ") : "0 ",
                            submitTime = string.Format("{0:yyyy-MM-dd}", p.CreateTime),
                            state = p.State ?? 0,
                            headerUrl = AppCommonHelper.GetUserProfilePhoto(p.CreateUserId, userPhoto, user),
                            submitUserName = p.CreateUserName,
                            answerCount = getQuestionCount(p.Id),
                            files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.Id).ToList(), true)
                        });
                        return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount,
                            AppCommonHelper.PageSize);
                    }
                    return APIResult.GetErrorResult(project.Exception);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 根据变更 ID 获取百变更详情
        /// </summary>
        /// <param name="id">变更 ID ps:1013054110498426880</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetChangeDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择变更！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetChangeModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    var change = result.Data;
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        change.CreateUserId
                    });

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Change.ToString();
                    var data = new
                    {
                        id = change.Id,
                        name = change.ChangeName ?? "",
                        projectName = change.ProjectName ?? "",
                        code = change.ChangeNo ?? "",
                        amount = change.TotalAmount.HasValue ? change.TotalAmount.ToString("0.###### ") : "0 ",
                        increaseAmount = change.AddAmount.HasValue ? change.AddAmount.ToString("0.###### ") : "0 ",
                        reduceAmount = change.ReduceAmount.HasValue ? change.ReduceAmount.ToString("0.###### ") : "0 ",
                        changeAmount = change.ChangeAmount.HasValue ? change.ChangeAmount.ToString("0.###### ") : "0 ",
                        changeStartTime = string.Format("{0:yyyy-MM-dd}", change.ChangeStartTime),
                        changeEndTime = string.Format("{0:yyyy-MM-dd}", change.ChangeEndTime),
                        changeContent = change.ChangeContent ?? "",
                        changeReason = change.ChangeReason ?? "",
                        submitTime = string.Format("{0:yyyy-MM-dd}", change.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(change.CreateUserId, userPhoto, user),
                        state = change.State,
                        submitUserName = change.CreateUserName ?? "",
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        answerCount = answerCount,
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)change.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, change.CreateUserId))
                    };
                    return Json(APIResult.GetSuccessResult(data));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 变更的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditChange(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteChangeByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.UpdateChangeState(model.businessId, ((int)state).ToString());
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        #endregion

        #region 合同

        /// <summary>
        /// 根据项目 ID 获取合同列表接口
        /// </summary>
        /// <param name="projectId">项目 ID ps：1013047335028133888</param>
        /// <param name="pageIndex">当前页码</param>
        /// <returns></returns>
        private object GetContraceList(long projectId, int pageIndex = 1)
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
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = GetPageInfo(pageIndex)
                    };

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    qc.SortList.Add(new SortExpression()
                    {
                        SortName = "CreateTime",
                        SortType = eSortType.Desc
                    });

                    var result = proxy.GetContractList(qc);
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
                        name = p.Name ?? "",
                        code = p.Code ?? "",
                        firstPartyName = p.FirstPartyName ?? "",
                        secondPartyName = p.SecondPartyName ?? "",
                        signTime = string.Format("{0:yyyy-MM-dd}", p.SignTime),
                        state = p.State ?? 0
                    });
                    return APIResult.GetSuccessResult(data, pageIndex, result.AllRowsCount, AppCommonHelper.PageSize);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 根据合同 ID 获取合同详情
        /// </summary>
        /// <param name="id">合同 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetContractDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择合同！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetContractModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Flag == EResultFlag.Success && result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    var contract = result.Data;
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                       contract.CreateUserId
                    });

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Contract.ToString();
                    var data = new
                    {
                        id = contract.Id,
                        name = contract.Name ?? "",
                        projectName = contract.ProjectName ?? "",
                        code = contract.Code ?? "",
                        submitTime = string.Format("{0:yyyy-MM-dd}", contract.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(contract.CreateUserId, userPhoto, user),
                        contractTypeName = ((ContractType)contract.ContractType).GetText(),
                        firstPartyName = contract.FirstPartyName ?? "",
                        secondPartyName = contract.SecondPartyName ?? "",
                        buildDays = Convert.ToInt32(contract.BuildDays).ToString() + "天",
                        startTime = string.Format("{0:yyyy-MM-dd}", contract.StartTime),
                        endTime = string.Format("{0:yyyy-MM-dd}", contract.EndTime),
                        signTime = string.Format("{0:yyyy-MM-dd}", contract.SignTime),
                        amount = contract.Amount.HasValue ? contract.Amount.Value.ToString("0.###### ") : "0 ",
                        remark = contract.Remark,
                        state = contract.State,
                        submitUserName = contract.CreateUserName ?? "",
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        answerCount = answerCount,
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)contract.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, contract.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }


        /// <summary>
        /// 合同的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditContract(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteContractByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.UpdateContractState(model.businessId, ((int)state).ToString());

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 日志

        /// <summary>
        /// 根据日志 ID 获取监理日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetSupervisorLogDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择监理日志！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetSupervisorLogModelNew(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    var log = result.Data;

                    var projectResult = proxy.GetProjectModel(result.Data.ProjectId.Value);
                    if (projectResult.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult("获取监理日志详情失败！");
                    }
                    if (projectResult.Data == null)
                    {
                        return APIResult.GetErrorResult("获取监理日志详情失败!");
                    }
                    var project = projectResult.Data;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    List<Epm_WorkUploadRealScene> workRealSceneList = new List<Epm_WorkUploadRealScene>();
                    List<Base_Files> Scenefiles = new List<Base_Files>();
                    var dangerWorksResult = proxy.GetWorkRealSceneByLogId(id);
                    if (dangerWorksResult.Flag == EResultFlag.Success && dangerWorksResult.Data != null)
                    {
                        workRealSceneList = dangerWorksResult.Data;
                        List<long> workUploadIds = dangerWorksResult.Data.Select(p => p.Id).ToList();
                        Scenefiles = AppCommonHelper.GetBaseFileList(proxy, workUploadIds, true);
                    }

                    //根据监理日志获取待办危险作业
                    List<Epm_DangerousWork> workList = new List<Epm_DangerousWork>();
                    var workLogResult = proxy.GetDangerousWorkByLogId(id);
                    if (workLogResult.Flag == EResultFlag.Success && workLogResult.Data != null)
                    {
                        workList = workLogResult.Data;
                    }

                    int scheduleValue = string.IsNullOrWhiteSpace(result.Data.Schedule) ? 0 : Convert.ToInt32(result.Data.Schedule);

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Log.ToString();
                    var data = new
                    {
                        id = log.Id,
                        projectName = log.ProjectName ?? "",
                        //planName = log.PlanName ?? "",
                        //nextPlanName = log.nextPlanName ?? "",
                        //planState = log.planState == 0 ? "施工中" : log.planState == 1 ? "已完工" : "",
                        files = AppCommonHelper.GetFileList(log.Attachs),
                        answerCount = answerCount,
                        submitTime = string.Format("{0:yyyy-MM-dd}", log.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(log.CreateUserId, userPhoto, user),
                        state = log.State,
                        submitUserName = log.CreateUserName ?? "",
                        windPower = log.WindPower ?? "",
                        temperature = log.Temperature ?? "",
                        weatherName = log.TypeName ?? "",
                        workContent = log.Content ?? "",
                        tomorrowProject = log.TomorrowProject ?? "",
                        schedule = scheduleValue > 0 ? string.Format("延期 {0} 天", scheduleValue) : "正常",
                        remark = log.Remark ?? "",
                        day = string.Format("第 {0} 天/总工期 {1} 天", ((log.SubmitTime ?? DateTime.Today) - (project.PlanWorkStartTime ?? DateTime.Today)).Days, project.Limit ?? 0),
                        reason = log.Reason ?? "",
                        supervisorLogCompanies = log.SupervisorLogCompanys.Select(p => new
                        {
                            companyName = p.CompanyName,
                            workPeopleType = p.WorkPeopleType,
                            peopleNum = p.PeopleNumber
                        }),
                        dangerWorks = workRealSceneList.Select(p => new
                        {
                            id = p.Id,
                            workId = p.WorkId,
                            name = p.WorkName ?? "",
                            state = p.State ?? 0,
                            submitTime = string.Format("{0:yyyy-MM-dd}", p.UploadTime),
                            files = AppCommonHelper.GetFileList(Scenefiles.Where(x => x.TableId == p.Id).ToList(), true)
                        }),
                        dangerWorksWaitAppr = workList.Select(p => new
                        {
                            id = p.Id,
                            name = p.TaskName ?? "",
                            state = p.State ?? 0,
                            workTime = string.Format("{0:yyyy-MM-dd}", p.StartTime)
                        }),
                        planList = GetPlanList(log.PlanName),
                        nextPlanList = GetPlanList(log.nextPlanName),
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)log.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, log.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }

            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }
        private object GetPlanList(string planName)
        {
            if (string.IsNullOrEmpty(planName))
            {
                return (new List<object>() { }).ToArray();
            }
            else
            {
                var data = planName.Split(',').ToList().Select(p => new
                {
                    id = 0,
                    name = p
                });
                return data;
            }
        }

        /// <summary>
        /// 获取提交监理日志页面预加载数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddSupervisorLog(long projectId, string time = "")
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
                    List<string> PostList = new List<string>() { "土建", "包装", "安装", "加固", "内衬", "油罐清洗费" };
                    string a = "'土建', '包装', '安装', '加固', '内衬', '油罐清洗费'";
                    List<Epm_ProjectCompany> companyList = new List<Epm_ProjectCompany>();
                    List<Base_TypeDictionary> typeList = new List<Base_TypeDictionary>();
                    List<Base_TypeDictionary> Job_ScopesList = new List<Base_TypeDictionary>();
                    List<Epm_Plan> planList = new List<Epm_Plan>();
                    List<Epm_DangerousWork> dangerWorks = new List<Epm_DangerousWork>();
                    List<Epm_DangerousWork> dangerWorksWaitAppr = new List<Epm_DangerousWork>();
                    Epm_Plan firstPlan = new Epm_Plan();

                    var projectResult = proxy.GetProjectModel(projectId);
                    if (projectResult.Flag == EResultFlag.Success && projectResult.Data != null)
                    {
                        var companyResult = proxy.GetProjectCompanyList(projectId);

                        if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null)
                        {
                            companyList = companyResult.Data.Where(p => p.ProjectId == projectId && a.Contains(p.Type) && p.Type != "油罐" && !string.IsNullOrWhiteSpace(p.CompanyName)).ToList();
                        }

                        List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.Weather };
                        var dicResult = proxy.GetTypeListByTypes(list);
                        if (dicResult.Flag == EResultFlag.Success && dicResult.Data != null &&
                            dicResult.Data.ContainsKey(DictionaryType.Weather))
                        {
                            typeList = dicResult.Data[DictionaryType.Weather];
                        }

                        List<DictionaryType> Job_Scopes = new List<DictionaryType>() { DictionaryType.Job_Scopes };
                        var dicResults = proxy.GetTypeListByTypes(Job_Scopes);
                        if (dicResults.Flag == EResultFlag.Success && dicResults.Data != null &&
                            dicResults.Data.ContainsKey(DictionaryType.Job_Scopes))
                        {
                            Job_ScopesList = dicResults.Data[DictionaryType.Job_Scopes];
                        }
                        QueryCondition qc = new QueryCondition();
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "ProjectId",
                            ExpValue = projectId,
                            ExpOperater = eConditionOperator.Equal,
                            ExpLogical = eLogicalOperator.And
                        });

                        // 施工计划
                        var planResult = proxy.GetPlanList(qc);

                        if (planResult.Flag == EResultFlag.Success && planResult.Data != null)
                        {
                            planList = planResult.Data;
                            firstPlan = planResult.Data.Where(p => p.IsFinish == null || p.IsFinish == 0)
                                .OrderBy(p => p.StartTime).FirstOrDefault();
                        }

                        var dangerWorksResult = DangerworkList(user, projectId, time);
                        if (dangerWorksResult.Flag == EResultFlag.Success && dangerWorksResult.Data != null &&
                            dangerWorksResult.Data.Any())
                        {
                            dangerWorks = dangerWorksResult.Data;
                        }
                        //待审核危险作业数据
                        var dangerWorksWaitApprResult = DangerworkListWaitAppr(user, projectId, time);
                        if (dangerWorksWaitApprResult.Flag == EResultFlag.Success && dangerWorksWaitApprResult.Data != null &&
                            dangerWorksWaitApprResult.Data.Any())
                        {
                            dangerWorksWaitAppr = dangerWorksWaitApprResult.Data;
                        }

                        int delayDay = firstPlan == null
                            ? 0
                            : ((string.IsNullOrEmpty(time) ? DateTime.Today : Convert.ToDateTime(time)) - (firstPlan.DelayTime ?? firstPlan.EndTime)).Value.Days;

                        int cd = projectResult.Data.PlanWorkStartTime.HasValue ? (((string.IsNullOrEmpty(time) ? DateTime.Today : Convert.ToDateTime(time)) - projectResult.Data.PlanWorkStartTime.Value.Date).Days + 1) : 0;


                        string[] companys = { "土建", "包装", "安装", "加固", "内衬", "油罐清洗费" };
                        string[] workType = { "项目经理", "现场负责人", "安全员", "本省地区负责人" };


                        List<LogCompanyView> viewlist = new List<LogCompanyView>();
                        PeoplesView xmjg = null;
                        foreach (var item in companyList)
                        {
                            List<PeoplesView> namesList = new List<PeoplesView>();
                            LogCompanyView logCompanyView = new LogCompanyView();
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
                        var data = new
                        {
                            day = string.Format("第 {0} 天/总工期 {1} 天", (cd < 0 ? 0 : (cd > projectResult.Data.Limit ? projectResult.Data.Limit : cd)), (projectResult.Data.PlanWorkStartTime == null ? 0 : projectResult.Data.Limit)),
                            schedule = delayDay <= 0 ? "正常" : string.Format("延期 {0} 天", delayDay), // 延期天数
                            delayDay,
                            submitTime = string.IsNullOrEmpty(time) ? DateTime.Now.ToString("yyyy-MM-dd") : time,
                            companies = viewlist,

                            //工种查询
                            jobScopes = Job_ScopesList.Where(t => t.Type == "Job_Scopes").Select(p => new
                            {
                                id = p.No,
                                name = p.Name
                            }),
                            weathers = typeList.Select(p => new
                            {
                                id = p.No,
                                name = p.Name
                            }),
                            planList = planList.Where(t => !t.StartDate.HasValue).OrderBy(t => t.StartTime).Select(p => new
                            {
                                id = p.Id.ToString(),
                                name = p.Name
                            }),
                            nextPlanList = planList.Where(t => t.StartDate.HasValue && !t.EndDate.HasValue).OrderBy(t => t.StartTime).Select(p => new
                            {
                                id = p.Id.ToString(),
                                name = p.Name
                            }),
                            dangerWorks = dangerWorks.Select(p => new
                            {
                                id = p.Id,
                                name = p.TaskContent,
                                state = p.State
                            }),
                            dangerWorksWaitAppr = dangerWorksWaitAppr.Select(p => new
                            {
                                id = p.Id,
                                name = p.TaskName ?? "",
                                workTime = string.Format("{0:yyyy-MM-dd}", p.StartTime),
                                state = p.State ?? 0
                            }),
                        };

                        return APIResult.GetSuccessResult(data);
                    }
                    return APIResult.GetErrorResult(projectResult.Exception);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取危险作业和作业实景信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="submitTime"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetWorkSenceList(long projectId, string submitTime)
        {
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    List<Epm_DangerousWork> dangerWorks = new List<Epm_DangerousWork>();
                    List<Epm_DangerousWork> dangerWorksWaitAppr = new List<Epm_DangerousWork>();

                    var dangerWorksResult = DangerworkList(user, projectId, submitTime);
                    if (dangerWorksResult.Flag == EResultFlag.Success && dangerWorksResult.Data != null &&
                        dangerWorksResult.Data.Any())
                    {
                        dangerWorks = dangerWorksResult.Data;
                    }
                    //待审核危险作业数据
                    var dangerWorksWaitApprResult = DangerworkListWaitAppr(user, projectId, submitTime);
                    if (dangerWorksWaitApprResult.Flag == EResultFlag.Success && dangerWorksWaitApprResult.Data != null &&
                        dangerWorksWaitApprResult.Data.Any())
                    {
                        dangerWorksWaitAppr = dangerWorksWaitApprResult.Data;
                    }

                    var data = new
                    {
                        dangerWorks = dangerWorks.Select(p => new
                        {
                            id = p.Id,
                            name = p.TaskName,
                            state = p.State
                        }),
                        dangerWorksWaitAppr = dangerWorksWaitAppr.Select(p => new
                        {
                            id = p.Id,
                            name = p.TaskName ?? "",
                            workTime = p.StartTime,
                            state = p.State ?? 0
                        }),
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }


        /// <summary>
        /// 提交监理日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddSupervisorLog()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["projectId"].ToLongReq() == 0 || string.IsNullOrWhiteSpace(form["projectName"]))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            if (string.IsNullOrWhiteSpace(form["workCompanies"]))
            {
                return APIResult.GetErrorResult("请填写现场施工单位！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    SupervisorLogView model = new SupervisorLogView();
                    model.ProjectId = form["projectId"].ToLongReq();
                    model.ProjectName = form["projectName"];
                    model.TypeNo = form["weatherNo"];
                    model.TypeName = form["weatherName"];
                    model.SubmitTime = string.IsNullOrWhiteSpace(form["submitTime"]) ? DateTime.Now.AddDays(1) : Convert.ToDateTime(form["submitTime"].ToString());
                    model.WindPower = form["windPower"];
                    model.Temperature = form["temperature"];
                    //新开工程节点
                    string planId = "";
                    string planName = "";
                    if (!string.IsNullOrEmpty(form["planId"]))
                    {
                        List<Epm_Plan> planList = JsonConvert.DeserializeObject<List<Epm_Plan>>(form["planId"]);
                        if (planList != null && planList.Any())
                        {
                            foreach (var item in planList)
                            {
                                planId = planId + item.Id + ",";
                                planName = planName + item.Name + ",";
                            }
                        }
                    }
                    model.PlanId = string.IsNullOrEmpty(planId) ? "" : planId.Substring(0, planId.Length - 1);
                    model.PlanName = string.IsNullOrEmpty(planName) ? "" : planName.Substring(0, planName.Length - 1);

                    model.Content = form["content"];
                    model.TomorrowProject = form["tomorrowProject"];
                    model.Schedule = form["delayDay"];
                    model.Reason = form["reason"];
                    model.State = (int)ApprovalState.WaitAppr;
                    //新完工程节点
                    string nextPlanId = "";
                    string nextPlanName = "";
                    if (!string.IsNullOrEmpty(form["nextPlanId"]))
                    {
                        List<Epm_Plan> planList = JsonConvert.DeserializeObject<List<Epm_Plan>>(form["nextPlanId"]);
                        if (planList != null && planList.Any())
                        {
                            foreach (var item in planList)
                            {
                                nextPlanId = nextPlanId + item.Id + ",";
                                nextPlanName = nextPlanName + item.Name + ",";
                            }
                        }
                    }
                    model.nextPlanId = string.IsNullOrEmpty(nextPlanId) ? "" : nextPlanId.Substring(0, nextPlanId.Length - 1);
                    model.nextPlanName = string.IsNullOrEmpty(nextPlanName) ? "" : nextPlanName.Substring(0, nextPlanName.Length - 1);

                    List<SupervisorLogCompanyModel> workCompanys = JsonConvert.DeserializeObject<List<SupervisorLogCompanyModel>>(form["workCompanies"]);

                    if (workCompanys != null && workCompanys.Any())
                    {
                        model.SupervisorLogCompanys = workCompanys.Select(p => new Epm_SupervisorLogCompany()
                        {
                            PeopleNumber = p.peopleNum,
                            CompanyId = p.companyId,
                            ManagerName = p.name,//姓名
                            CompanyName = p.companyName,
                            WorkPeopleType = p.workPeopleTypeName,//工种
                            Permit = p.Permit,//入场许可
                            BePresent = p.BePresent,//是否到场
                        }).ToList();

                        model.Epm_ProjectCompany = workCompanys.Select(p => new Epm_ProjectCompany()
                        {
                            PM = p.PM,
                            PMPhone = p.PMPhone,
                            LinkMan = p.LinkMan,
                            LinkPhone = p.LinkPhone,
                            SafeMan = p.SafeMan,
                            SafePhone = p.SafePhone
                        }).ToList();
                    }
                    List<long> workIds = null;
                    if (!string.IsNullOrWhiteSpace(form["dangerWorks"]))
                    {
                        List<DangerourWorkModel> dangerourWork = JsonConvert.DeserializeObject<List<DangerourWorkModel>>(form["dangerWorks"]);
                        if (dangerourWork.Count > 0)
                        {
                            workIds = dangerourWork.Select(p => p.id).ToList();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(form["dangerWorksWaitAppr"]))
                    {
                        List<DangerourWorkModel> dangerourWork = JsonConvert.DeserializeObject<List<DangerourWorkModel>>(form["dangerWorksWaitAppr"]);
                        if (dangerourWork.Count > 0)
                        {
                            if (workIds == null)
                            {
                                workIds = dangerourWork.Select(p => p.id).ToList();
                            }
                            else
                            {
                                workIds.AddRange(dangerourWork.Select(p => p.id).ToList());
                            }
                        }
                    }
                    // todo: 附件处理
                    var files = AppCommonHelper.UploadFile(http, user);
                    model.Attachs = files;
                    using (ClientSiteClientProxy proxys = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        var result = proxys.AddSupervisorLogNew(model, workIds);
                        if (result.Flag == EResultFlag.Success)
                        {
                            return APIResult.GetSuccessResult("监理日志提交成功！");
                        }
                        return APIResult.GetErrorResult(result.Exception);
                    }

                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 监理日志的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditSupervisorLog(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteSupervisorlogByIdNew(model.businessId);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    Epm_SupervisorLog log = new Epm_SupervisorLog();
                    log.Id = model.businessId;
                    log.State = (int)state;
                    var result = proxy.AuditSupervisorLog(log);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 专项验收

        /// <summary>
        /// 获取专项验收详情
        /// </summary>
        /// <param name="id">专项验收 ID ps:1013047727157809152</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetSpecialAccepDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择专项验收！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetSpecialAcceptanceModel(id);
                    if (result.Flag == EResultFlag.Success && result.Data != null)
                    {
                        Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy,
                            new List<long>()
                            {
                                result.Data.CreateUserId
                            });

                        var data = new
                        {
                            id = result.Data.Id,
                            projectName = result.Data.ProjectName ?? "",
                            name = result.Data.Title ?? "",
                            files = AppCommonHelper.GetFileList(result.Data.AttachList),
                            answerCount = 0,
                            browseCount = 0,
                            submitTime = string.Format("{0:yyyy-MM-dd}", result.Data.CreateTime),
                            headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                            type = "",
                            state = result.Data.State,
                            submitUserName = result.Data.CreateUserName ?? "",
                            checkAcceptanceUnit = result.Data.RecCompanyName,
                            checkAcceptanceContent = result.Data.Content
                        };
                        return APIResult.GetSuccessResult(data);
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        #endregion

        #region 工器具机械验收(材料设备验收)

        /// <summary>
        /// 材料设备验收详情
        /// </summary>
        /// <param name="id">材料设备验收 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetMaterialDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择工器具机械！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetMaterialModel(id);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    if (result.Data == null || result.Data.Epm_Material == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }

                    var material = result.Data.Epm_Material;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy,
                        new List<long>()
                        {
                            material.CreateUserId
                        });

                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });

                    var model = proxy.GetMaterialModel(id);
                    List<long> deList = model.Data.MaterialDetails.Select(t => t.Id).ToList();
                    List<FileView> fileList = new List<FileView>();
                    List<Base_Files> filelistAll = new List<Base_Files>();
                    if (deList.Any())
                    {
                        filelistAll = AppCommonHelper.GetBaseFileList(proxy, deList);
                        //fileList = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, deList));
                    }
                    string businessType = BusinessType.Equipment.ToString();

                    var enumList = Enum<EquipmentFileType>.AsEnumerable();

                    var data = new
                    {
                        id = material.Id,
                        projectName = material.ProjectName,
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(material.CreateUserId, userPhoto, user),
                        companyName = material.SupplierName,
                        address = material.CheckAddress,
                        receiveCompany = material.CheckCompanyName,
                        submitTime = string.Format("{0:yyyy-MM-dd}", material.CheckTime),
                        state = material.State,
                        submitUserName = material.CreateUserName ?? "",
                        content = material.CheckContent,
                        details = result.Data.MaterialDetails.Select(p => new
                        {
                            id = p.Id,
                            name = p.Name ?? "",
                            model = p.Model ?? "",
                            num = p.Qty ?? 0,
                            remark = p.Remark ?? "",
                            fileList = enumList.Select(a => new
                            {
                                title = a.GetText(),
                                files = AppCommonHelper.GetFileList(filelistAll.Where(t => t.TableColumn == a.ToString()).ToList()).ToList()
                            }),
                        }).ToList(),
                        answerCount = ((questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0) ? questionResult.Data[id].ToString() : ""),
                        actionButton = AppCommonHelper.GetRightByState(((ConfirmState)material.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, "Materiel", material.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 物料接收详情
        /// </summary>
        /// <param name="id">物料接收 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetMaterielDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择材料设备验收！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetMaterielModel(id);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    if (result.Data == null || result.Data.Epm_Materiel == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }

                    var material = result.Data.Epm_Materiel;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy,
                        new List<long>()
                        {
                            material.CreateUserId
                        });

                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });

                    string businessType = BusinessType.Track.ToString();
                    var data = new
                    {
                        id = material.Id,
                        projectName = material.ProjectName,
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(material.CreateUserId, userPhoto, user),
                        companyName = material.SupplierName,
                        address = material.ReceiveAddress,
                        receiveCompany = material.ReceiveCompanyName,
                        submitTime = string.Format("{0:yyyy-MM-dd}", material.CreateTime),
                        state = material.State,
                        submitUserName = material.CreateUserName ?? "",
                        content = "",
                        details = result.Data.MaterielDetails.Select(p => new
                        {
                            id = p.Id,
                            name = p.Name ?? "",
                            model = p.Model ?? "",
                            num = p.Qty ?? 0,
                            unit = p.Unit ?? ""
                        }).ToList(),
                        answerCount = ((questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0) ? questionResult.Data[id].ToString() : ""),
                        actionButton = AppCommonHelper.GetRightByState(((ConfirmState)material.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, "Material", material.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 新增物料验收/材料验收
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddMaterial(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                return APIResult.GetSuccessNoDatas();
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 新增物料验收/接收
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddMaterial()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["projectId"].ToLongReq() <= 0 || string.IsNullOrWhiteSpace(form["projectName"]))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }

            if (string.IsNullOrWhiteSpace(form["businessType"]))
            {
                return APIResult.GetErrorResult("请确定业务类型！");
            }

            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }

            List<MaterialItemModel> itemList = JsonConvert.DeserializeObject<List<MaterialItemModel>>(form["itemList"]);

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                // 工器具验收
                if (form["businessType"].ToLower().Equals(BusinessType.Equipment.ToString().ToLower()))
                {
                    MaterialView view = new MaterialView();
                    Epm_Material material = new Epm_Material();

                    material.ProjectId = form["projectId"].ToLongReq();
                    material.ProjectName = form["projectName"];
                    material.SupplierId = form["companyId"].ToLongReq();
                    material.SupplierName = form["companyName"];
                    material.CheckCompanyId = user.CompanyId;
                    material.CheckCompanyName = user.CompanyName;
                    material.CheckAddress = form["address"];
                    material.CheckContent = form["content"];
                    material.State = (int)ConfirmState.WaitConfirm;

                    view.Epm_Material = material;
                    view.FileList = AppCommonHelper.UploadFile(http, user);

                    if (itemList != null && itemList.Any())
                    {
                        view.MaterialDetails = itemList.Select(p => new Epm_MaterialDetails()
                        {
                            Name = p.name,
                            Qty = p.num,
                            Unit = p.unit,
                            Model = p.model
                        }).ToList();
                    }

                    var result = proxy.AddMaterial(view);
                    if (result.Flag == EResultFlag.Success)
                    {
                        return APIResult.GetSuccessResult("操作成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }

                // 材料验收
                if (form["businessType"].ToLower().Equals(BusinessType.Track.ToString().ToLower()))
                {
                    MaterielView view = new MaterielView();
                    Epm_Materiel material = new Epm_Materiel();

                    material.ProjectId = form["projectId"].ToLongReq();
                    material.ProjectName = form["projectName"];
                    material.ReceiveCompanyId = user.CompanyId; ;
                    material.ReceiveCompanyName = user.CompanyName;
                    material.SupplierId = form["companyId"].ToLongReq();
                    material.SupplierName = form["companyName"];
                    material.ReceiveAddress = form["address"];
                    material.Remark = form["content"];
                    material.State = (int)ConfirmState.WaitConfirm;
                    material.ReceiveTime = DateTime.Now;
                    material.ReceiveUserName = user.RealName;
                    material.ReceiveUserId = user.UserId;
                    view.Epm_Materiel = material;
                    view.FileList = AppCommonHelper.UploadFile(http, user);
                    if (itemList != null && itemList.Any())
                    {
                        view.MaterielDetails = itemList.Select(p => new Epm_MaterielDetails()
                        {
                            Name = p.name,
                            Qty = p.num,
                            Unit = p.unit,
                            Model = p.model
                        }).ToList();
                    }

                    var result = proxy.AddMateriel(view);
                    if (result.Flag == EResultFlag.Success)
                    {
                        return APIResult.GetSuccessResult("操作成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }

                return APIResult.GetErrorResult("请确定业务类型！");
            }
        }

        /// <summary>
        /// 上传工器具验收图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object UploadEquipment()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                var FileList = AppCommonHelper.UploadFile(http, user);

                var result = proxy.AddFilesByTables("", FileList, false);

                var resourceUrl = AppCommonHelper.SystemSetting["resourceUrl"];

                if (result.Flag == EResultFlag.Success && result.Data.Any())
                {
                    var data = result.Data.Where(t => !string.IsNullOrEmpty(t.ImageType)).Select(p => new
                    {
                        id = p.Id,
                        url = resourceUrl + p.Url
                    });

                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
                return APIResult.GetErrorResult(result.Exception);
            }
        }

        /// <summary>
        /// 工器具验收提交数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddEquipment()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["projectId"].ToLongReq() <= 0 || string.IsNullOrWhiteSpace(form["projectName"]))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }

            List<MaterialItemModel> itemList = JsonConvert.DeserializeObject<List<MaterialItemModel>>(form["itemList"]);

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                MaterialViewNew view = new MaterialViewNew();
                Epm_Material material = new Epm_Material();

                material.ProjectId = form["projectId"].ToLongReq();
                material.ProjectName = form["projectName"];
                material.SupplierId = form["companyId"].ToLongReq();
                material.SupplierName = form["companyName"];
                material.CheckCompanyId = user.CompanyId;
                material.CheckCompanyName = user.CompanyName;
                material.State = (int)ConfirmState.WaitConfirm;

                view.Epm_Material = material;

                if (itemList != null && itemList.Any())
                {
                    List<MaterialDetailsView> vlist = new List<MaterialDetailsView>();
                    foreach (var item in itemList)
                    {
                        MaterialDetailsView detail = new MaterialDetailsView();

                        detail.MaterialDetails.Qty = item.num;
                        detail.MaterialDetails.Remark = item.remark;
                        detail.MaterialDetails.Model = item.model;
                        detail.MaterialDetails.Name = item.name;

                        detail.FileList = new List<Base_Files>();
                        if (item.files.Any())
                        {
                            foreach (var temp in item.files)
                            {
                                long id = temp.id.ToLongReq();
                                Base_Files file = proxy.GetBaseFile(id).Data;
                                Base_Files oldfile = new Base_Files();
                                if (file != null)
                                {
                                    var fileList = proxy.GetBaseFileByGuid(file.GuidId).Data;
                                    if (fileList.Any())
                                    {
                                        oldfile = fileList.Where(t => string.IsNullOrEmpty(t.ImageType)).FirstOrDefault();
                                    }
                                    if (temp.type == "0")
                                    {
                                        file.IsDelete = false;
                                        oldfile.IsDelete = false;
                                    }
                                    else
                                    {
                                        file.IsDelete = true;
                                        oldfile.IsDelete = true;
                                    }
                                    detail.FileList.Add(file);
                                    detail.FileList.Add(oldfile);
                                }
                            }
                        }
                        vlist.Add(detail);
                    }
                    view.MaterialDetails = vlist;
                }

                var result = proxy.AddMaterialNew(view);

                if (result.Flag == EResultFlag.Success)
                {
                    return APIResult.GetSuccessResult("操作成功！");
                }
                return APIResult.GetErrorResult(result.Exception);
            }
        }

        /// <summary>
        /// 物料接收(材料验收)的审核、驳回、作废操作
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditMaterial(BusinessCheck model, UserView user)
        {
            string businessTpye = model.businessType.ToLower();
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };

                        Result<int> deleteResult;
                        if (model.businessType == (BusinessType.Equipment.ToString()))
                        {
                            deleteResult = proxy.DeleteMaterialByIds(ids);
                        }
                        else
                        {
                            deleteResult = proxy.DeleteMaterielByIds(ids);
                        }

                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ConfirmState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ConfirmState.Confirm;
                            break;
                        case SystemRight.UnCheck:
                            state = ConfirmState.ConfirmFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ConfirmState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    // 验收
                    if (businessTpye.Equals(BusinessType.Equipment.ToString().ToLower()))
                    {
                        var result = proxy.UpdateMaterialState(model.businessId, state);
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }
                    }
                    // 接收
                    else if (businessTpye.Equals(BusinessType.Track.ToString().ToLower()))
                    {
                        var result = proxy.ChangeMaterielState(model.businessId, state);
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }
                    }
                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        #endregion

        #region 质量(安全)培训

        /// <summary>
        /// 获取质量(安全)培训详情
        /// </summary>
        /// <param name="id">质量(安全)培训详情 ID ps:1013048183670050817</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetTrainDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择培训！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetTrainModel(id);
                    if (result.Flag == EResultFlag.Success && result.Data != null)
                    {
                        Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy,
                            new List<long>()
                            {
                                result.Data.TrainUserId ?? 0
                            });

                        var data = new
                        {
                            id = result.Data.Id,
                            projectName = result.Data.ProjectName,
                            name = result.Data.Title,
                            answerCount = 0,
                            browseCount = 0,
                            files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                            submitTime = string.Format("{0:yyyy-MMdd}", result.Data.CreateTime),
                            headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                            type = "",
                            state = result.Data.State,
                            submitUserName = result.Data.CreateUserName,
                            trainPersonnel = result.Data.TrainUserName,
                            trainUnit = result.Data.TrainCompanyName,
                            joinTrainUnit = result.Data.CompanyNames,
                            trainStartTime = string.Format("{0:yyyy-MM-dd HH:mm}", result.Data.StartTime),
                            trainEndTime = string.Format("{0:yyyy-MM-dd HH:mm}", result.Data.EndTime),
                            trainContent = result.Data.Content
                        };

                        return Json(APIResult.GetSuccessResult(data));
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        #endregion

        #region 现场检查

        /// <summary>
        /// 现场检查详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetMonitorDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择检查！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetInspectModel(id);
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }

                    //获取非常规作业列表
                    var workList = proxy.GetIUnconventionalWorkList(id).Data;

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    var resultItem = proxy.GetInspectItemList(id);
                    if (resultItem.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(resultItem.Exception);
                    }
                    var inspect = result.Data;
                    var item = resultItem.Data.ToList();
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    var itemLevel2 = item.OrderBy(t => t.CreateTime).Where(t => t.Level == 2 && t.Choice == true).ToList();
                    var itemLevel3 = item.OrderBy(t => t.CreateTime).Where(t => t.Level == 3).ToList();
                    var data = new
                    {
                        id = inspect.Id,
                        projectName = inspect.ProjectName,
                        name = inspect.InspectName,
                        submitUserName = inspect.InspectUserName,
                        submitTime = inspect.InspectDate.ToString("yyyy-MM-dd"),
                        address = inspect.InspectAddress,
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                        state = (int)RectificationState.WorkFinish,
                        checkItems = itemLevel2.Select(a => new
                        {
                            id = a.Id,
                            parentId = 0,
                            parentName = a.CheckParentName,
                            name = a.CheckParentName + "->" + a.CheckName,
                            level = a.Level,
                            remark = a.Remark ?? "",
                            children = itemLevel3.Where(t => t.CheckParentId == a.CheckId).Select(b => new
                            {
                                id = b.Id,
                                parentId = 0,
                                parentName = b.CheckParentName,
                                name = b.CheckName,
                                level = b.Level,
                                selected = b.Choice,
                                rectification = new[] { new { id = "", name = b.RectifRecordPerson } },
                                score = new string[1] { b.Score.ToString() == "0.00" ? "0" : b.Score.ToString("#.##") ?? "0" },
                                state = b.State,
                                childText = GetchildText(workList, b.CheckId.Value, b.CheckName),
                                children = GetWorkList(workList, b.CheckId.Value, b.CheckName)
                            })
                        }),
                        answerCount = 0,
                        actionButton = AppCommonHelper.GetRightByState((RectificationState.WorkFinish).ToString(), AppCommonHelper.CreateButtonRight(user, "SecurityCheck", result.Data.CreateUserId))
                    };

                    return Json(APIResult.GetSuccessResult(data));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取非常规作业和复查、复核数据
        /// </summary>
        /// <param name="workList"></param>
        /// <param name="checkId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private object GetWorkList(List<EPM_UnconventionalWork> workList, long checkId, string name)
        {
            if (workList.Any())
            {
                var data = workList.OrderBy(t => t.CreateTime).Where(t => t.Level == 4 && t.CheckParentId == checkId).Select(a => new
                {
                    id = a.Id,
                    parentId = a.CheckParentId,
                    parentName = name,
                    name = a.Name,
                    level = a.Level,
                    children = workList.OrderBy(t => t.CreateTime).Where(t => t.Level == 5 && t.CheckParentId == a.CheckId).Select(b => new
                    {
                        id = b.Id,
                        parentId = b.CheckParentId,
                        parentName = a.Name,
                        name = b.Name,
                        level = b.Level,
                        score = new int[1] { b.Score ?? 0 }
                    })
                });

                return data;
            }
            else
            {
                return (new List<object>() { }).ToArray();
            }
        }

        private object GetWorkRecifList(List<EPM_UnconventionalWork> workList, long checkId, string name)
        {
            if (workList.Any())
            {
                var levelList5 = workList.Where(t => t.Level == 5 && t.ScoreRang.Split(',')[0].ToInt32Req() > t.Score);

                if (levelList5.Any())
                {
                    string ids = string.Join(",", levelList5.Select(t => t.CheckParentId));
                    var levelList4 = workList.OrderBy(t => t.CreateTime).Where(t => t.Level == 4 && t.CheckParentId == checkId && ids.Contains(t.CheckId.ToString()));

                    var data = levelList4.Select(a => new
                    {
                        id = a.Id,
                        parentId = a.CheckParentId,
                        parentName = name,
                        name = a.Name,
                        level = a.Level,
                        children = levelList5.OrderBy(t => t.CreateTime).Where(t => t.CheckParentId == a.CheckId).Select(b => new
                        {
                            id = b.Id,
                            parentId = b.CheckParentId,
                            parentName = a.Name,
                            name = b.Name,
                            level = b.Level,
                            score = new int[1] { b.Score ?? 0 }
                        })
                    });
                    return data;
                }
                else
                {
                    return (new List<object>() { }).ToArray();
                }
            }
            else
            {
                return (new List<object>() { }).ToArray();
            }
        }

        private string GetchildText(List<EPM_UnconventionalWork> workList, long checkId, string name)
        {
            string result = "";
            if (workList.Any())
            {
                var list = workList.Where(t => t.CheckParentId == checkId);
                if (list.Any())
                {
                    if (FCGZY.Contains(name))
                    {
                        result = name + "详情";
                    }
                    else
                    {
                        result = "查看详情";
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 整改单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetRecifDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择检查！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetRectificationModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    var resultItem = proxy.GetRectificationItemList(id);
                    if (resultItem.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(resultItem.Exception);
                    }
                    var resultRecord = proxy.GetRectificationRecordList(id);
                    if (resultRecord.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(resultRecord.Exception);
                    }

                    var rectif = result.Data;
                    var item = resultItem.Data;
                    var record = resultRecord.Data;

                    //获取非常规作业列表
                    var workList = proxy.GetIUnconventionalWorkList(rectif.InsppectId.Value).Data;
                    //已整改列表
                    var rectificationedList = resultItem.Data.Where(t => t.State == (int)RectificationState.Rectificationed).ToList();

                    string businessType = BusinessType.SecurityCheck.ToString();
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    //获取已提交整改附件
                    var fileidlist = item.Where(t => t.Level == 2).Select(t => t.Id).ToList();
                    var filelist = AppCommonHelper.GetBaseFileList(proxy, fileidlist);

                    //判断登录人是否监理
                    var companys = proxy.GetProjectCompanyByProjectId(rectif.ProjectId.Value).Data;
                    var company = companys.Where(t => (t.PMId == user.UserId || t.LinkManId == user.UserId) && (t.IsSupervisor == 1 || t.Type == "监理")).FirstOrDefault();
                    var isSupervisor = company == null ? false : true;

                    #region 项目所需权限

                    // 项目所需权限
                    List<Button> btns = new List<Button>();
                    if (user != null)
                    {
                        var userRight = user.Rights.Where(p => p.Value.Contains(BusinessType.SecurityCheck.ToString()));
                        if (userRight.Any())
                        {
                            foreach (var keyValuePair in userRight)
                            {
                                if (keyValuePair.Value.Contains(SystemRight.AuditRectif.ToString()) && rectificationedList.Count > 0 && (isSupervisor || rectif.InspectUserId == user.UserId))
                                {
                                    Button btn = new Button();
                                    btn.rightId = keyValuePair.Key;
                                    btn.title = SystemRight.AuditRectif.GetText();
                                    btn.rightAction = SystemRight.AuditRectif.ToString();
                                    btn.color = AppCommonHelper.GetButtonColor(SystemRight.AuditRectif);
                                    btns.Add(btn);
                                    continue;
                                }
                                if (keyValuePair.Value.Contains(SystemRight.RejectRectif.ToString()) && rectificationedList.Count > 0 && (isSupervisor || rectif.InspectUserId == user.UserId))
                                {
                                    Button btn = new Button();
                                    btn.rightId = keyValuePair.Key;
                                    btn.title = SystemRight.RejectRectif.GetText();
                                    btn.rightAction = SystemRight.RejectRectif.ToString();
                                    btn.color = AppCommonHelper.GetButtonColor(SystemRight.RejectRectif);
                                    btns.Add(btn);
                                    continue;
                                }
                                if (keyValuePair.Value.Contains(SystemRight.UploadSecurityCheck.ToString()) && rectificationedList.Count == 0 && rectif.RectifRecordUserId == user.UserId)
                                {
                                    Button btn = new Button();
                                    btn.rightId = keyValuePair.Key;
                                    btn.title = SystemRight.UploadSecurityCheck.GetText();
                                    btn.rightAction = SystemRight.UploadSecurityCheck.ToString();
                                    btn.color = AppCommonHelper.GetButtonColor(SystemRight.UploadSecurityCheck);
                                    btns.Add(btn);
                                }
                            }
                        }
                    }

                    #endregion

                    var data = new
                    {
                        id = rectif.Id,
                        projectName = rectif.ProjectName,
                        name = rectif.RectificateTitle,
                        submitUserName = rectif.InspectUserName,
                        submitTime = rectif.InspectDate.ToString("yyyy-MM-dd"),
                        address = rectif.InspectAddress,
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                        state = (int)rectif.State,
                        checkItems = item.Where(t => t.Level == 2).Select(a => new
                        {
                            id = a.Id,
                            parentId = 0,
                            parentName = a.CheckParentName,
                            name = a.CheckParentName + "->" + a.CheckName,
                            level = a.Level,
                            remark = a.Remark ?? "",
                            state = a.State,
                            files = AppCommonHelper.GetFileList(filelist.Where(t => t.TableId == a.Id).ToList()),
                            children = item.Where(t => t.Level == 3 && t.CheckParentId == a.CheckId).Select(b => new
                            {
                                id = b.Id,
                                parentId = 0,
                                parentName = b.CheckParentName,
                                name = b.CheckName,
                                level = b.Level,
                                rectification = new[] { new { id = "", name = rectif.RectifRecordUserName } },
                                childText = GetchildText(workList, b.CheckId.Value, b.CheckName),
                                children = GetWorkRecifList(workList, b.CheckId.Value, b.CheckName)
                            })
                        }),
                        answerCount = 0,
                        actionButton = btns //AppCommonHelper.GetRightByState(((RectificationState)result.Data.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, result.Data.CreateUserId, isSupervisor))
                    };

                    return Json(APIResult.GetSuccessResult(data));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 临时用电,动火,高空作业,受限空间,管线打开,吊装,挖掘
        /// </summary>
        private static string FCGZY
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FCGZY"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "";
                }
                return value;
            }
        }
        /// <summary>
        /// 复核、复查
        /// </summary>
        private static string FCFH
        {
            get
            {
                string value = ConfigurationManager.AppSettings["FCFH"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "";
                }
                return value;
            }
        }

        /// <summary>
        /// 获取提交检查页面预加载数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddMonitor(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                Epm_Project project = null;
                string roleType = string.Empty;
                List<Epm_CheckItem> checkItemList = null;
                List<Epm_InspectItem> inspectItemList = null;
                List<Epm_InspectItem> itemDraftList = null;
                long zjlUserId = 0;
                long jlUserId = 0;

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    bool isAgency = proxy.IsAgencyUser(user.UserId);
                    if (!isAgency)
                    {
                        bool isBranch = proxy.IsBranchCompanyUser(user.UserId);
                        if (!isBranch)
                        {
                            bool isSupervisor = proxy.IsSupervisor(projectId, user.UserId);
                            if (isSupervisor)
                            {
                                roleType = RoleTypeEnum.JL.ToString();
                            }
                            else
                            {
                                roleType = RoleTypeEnum.ZJL.ToString();
                            }
                        }
                        else
                        {
                            roleType = RoleTypeEnum.FGS.ToString();
                            var getProjectCompanyResult = proxy.GetProjectCompanyByProjectId(projectId);
                            if (getProjectCompanyResult.Flag == EResultFlag.Success && getProjectCompanyResult.Data != null)
                            {
                                var company = getProjectCompanyResult.Data.FirstOrDefault(t => t.IsSupervisor == 1);
                                if (company != null)
                                {
                                    if (company.LinkManId != null)
                                    {
                                        jlUserId = company.LinkManId.Value;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        roleType = RoleTypeEnum.SGS.ToString();
                    }

                    project = proxy.GetProject(projectId).Data;

                    checkItemList = proxy.GetCheckItemAll().Data;

                    inspectItemList = proxy.GetInspectItemByProjectId(projectId).Data;

                    itemDraftList = proxy.GetInspectItemDraft(projectId).Data;

                }

                var data = new
                {
                    address = project.Name,
                    date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    checkItems = checkItemList.OrderBy(t => t.Sort).Where(t => t.RoleType == roleType && t.Level == 1).Select(a => new
                    {
                        id = a.Id,
                        parentId = 0,
                        parentName = "",
                        name = a.Name,
                        level = 1,
                        selected = GetSelected(itemDraftList, a.Id),

                        children = checkItemList.OrderBy(t => t.Sort).Where(t => t.RoleType == roleType && t.Level == 2 && t.ParentId == a.Id).Select(b => new
                        {
                            id = b.Id,
                            parentId = b.ParentId,
                            parentName = a.Name + "->" + b.Name,
                            name = b.Name,
                            level = 2,
                            selected = true,

                            /* 20190328 wjq :发现整改负责人有存在未空的情况，按道理不应该未空，最终还是要维护好数据。 */

                            children = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3 && t.ParentId == b.Id && !string.IsNullOrEmpty(t.RectificationManName)).Select(c => new
                            {
                                id = c.Id,
                                parentId = c.ParentId,
                                parentName = c.ParentName,
                                name = c.Remark,
                                level = 3,
                                selected = GetSelected(itemDraftList, c.Id),

                                rectification = c.RectificationManName.Split(',').Select(d => new
                                {
                                    id = d,
                                    name = string.IsNullOrEmpty(d) ? "" : ((RectificationPeople)Enum.Parse(typeof(RectificationPeople), d)).GetText()

                                }),  //[{id:”QGDW”, name:”清罐单位现场负责人”},{id:”TJDW”,name:”土建单位现场负责人”}]  --整改负责人

                                addRectification = GetRectificationVal(itemDraftList, c.Id),
                                score = string.IsNullOrEmpty(c.ScoreRange) ? (new string[1] { "10" }) : c.ScoreRange.Split(','), //值范围[10,5,3,0]
                                addScore = GetScore(itemDraftList, c.Id, c.ScoreRange),
                                isChange = !FCGZY.Contains(c.Remark),
                                childText = (!"SGS,FGS,JL,ZJL".Contains(roleType) ? "" : (FCFH.Contains(c.ParentName) ? "查看详情" : (FCGZY.Contains(c.Remark) ? (c.Remark + "检查") : ""))),
                                children = GetChildren(checkItemList, inspectItemList, a.Name, b.Name, c.Remark, roleType, project, jlUserId, zjlUserId)
                            })
                        })
                    })
                };

                return APIResult.GetSuccessResult(data);
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }
        private object GetChildren(List<Epm_CheckItem> checkItem, List<Epm_InspectItem> inspectItemList, string name1, string name2, string name3, string roleType, Epm_Project project, long jlUserId, long zjlUserId)
        {
            if (FCFH.Contains(name2))
            {
                long crateUserId = 0;
                if (roleType == "SGS")
                {
                    crateUserId = project.ContactUserId.Value;
                }
                else if (roleType == "FGS")
                {
                    if (name3.Contains("站经理"))
                    {
                        crateUserId = zjlUserId;
                    }
                    if (name3.Contains("监理"))
                    {
                        crateUserId = jlUserId;
                    }
                }

                if (!"SGS,FGS".Contains(roleType))
                {
                    return (new List<object>() { }).ToArray();
                }

                if (inspectItemList != null)
                {
                    var item = inspectItemList.Where(p => p.CreateUserId == crateUserId && p.Level == 2 && p.CheckParentName == name1).OrderByDescending(p => p.CreateTime).FirstOrDefault();
                    if (item != null)
                    {
                        var data = inspectItemList.Where(t => t.InspectId == item.InspectId && t.Level == 2 && !FCFH.Contains(t.CheckName)).Select(c => new
                        {
                            id = c.Id,
                            parentId = c.CheckParentId,
                            parentName = c.CheckParentName,
                            name = c.CheckName,
                            level = 4,
                            children = inspectItemList.Where(t => t.Level == 3 && t.CheckParentId == c.CheckId).Select(a => new
                            {
                                id = a.Id,
                                parentId = a.CheckParentId,
                                parentName = a.CheckParentName,
                                name = a.CheckName,
                                level = 5,
                                score = new decimal[1] { a.Score.Value },
                                addScore = a.Score,
                                isChange = false
                            })
                        });
                        return data;
                    }
                    else
                    {
                        return (new List<object>() { }).ToArray();
                    }
                }
            }
            else if (FCGZY.Contains(name3))
            {
                //非常规作业数据
                var data = checkItem.OrderBy(t => t.Sort).Where(t => t.RoleType == "FCGZY" && t.Level == 2 && t.ParentName.Trim() == name3.Trim()).Select(c => new
                {
                    id = c.Id,
                    parentId = c.ParentId,
                    parentName = c.ParentName,
                    name = c.Name,
                    level = 4,
                    children = checkItem.OrderBy(t => t.Sort).Where(t => t.RoleType == "FCGZY" && t.Level == 3 && t.ParentId == c.Id).Select(a => new
                    {
                        id = a.Id,
                        parentId = a.ParentId,
                        parentName = a.ParentName,
                        name = a.Remark,
                        level = 5,
                        score = a.ScoreRange.Split(','),
                        addScore = a.ScoreRange.Split(',')[0],
                        isChange = true
                    })
                });
                return data;
            }

            return (new List<object>() { }).ToArray();
        }

        private bool GetSelected(List<Epm_InspectItem> itemDraftList, long checkId)
        {
            if (itemDraftList != null)
            {
                var item = itemDraftList.Where(t => t.CheckId == checkId).FirstOrDefault();
                if (item != null)
                {
                    return item.Choice.Value;
                }
            }
            return false;
        }
        private object GetRectificationVal(List<Epm_InspectItem> itemDraftList, long checkId)
        {
            if (itemDraftList != null)
            {
                var item = itemDraftList.Where(t => t.CheckId == checkId).FirstOrDefault();
                if (item != null)
                {
                    var data = new
                    {
                        id = item.RectifRecordPersonKey,
                        name = item.RectifRecordPerson
                    };
                    return data;
                }
            }
            return new object();
        }
        private string GetScore(List<Epm_InspectItem> itemDraftList, long checkId, string scoreRange)
        {
            if (itemDraftList != null)
            {
                var item = itemDraftList.Where(t => t.CheckId == checkId).FirstOrDefault();
                if (item != null)
                {
                    return item.Score.ToString("0.######");
                }
            }
            return scoreRange.Split(',')[0];
        }

        /// <summary>
        /// 新增检查
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddMonitor()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("未登录或登录超时！");
                }
                if (string.IsNullOrWhiteSpace(form["projectId"]))
                {
                    throw new Exception("错误：服务器无法获取项目信息！");
                }
                long projectId = form["projectId"].ToLongReq();

                var time = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(form["date"]))
                {
                    time = form["date"].ToDateTime().Value;
                }

                if (string.IsNullOrWhiteSpace(form["param"]))
                {
                    throw new Exception("错误：服务器无法获取提交数据！");
                }
                List<CheckView> cvlist = JsonConvert.DeserializeObject<List<CheckView>>(form["param"]);

                int type = 0;
                if (!string.IsNullOrWhiteSpace(form["type"]))
                {
                    type = form["type"].ToInt32().Value;
                }

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.AddMonitorNew(projectId, time, cvlist, type);
                    if (result.Flag == EResultFlag.Success && result.Data)
                    {
                        return APIResult.GetSuccessResult("保存成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 物料接收(材料验收)的审核、驳回、作废操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AuditMonitor(long id, string action, string reason)
        {
            ApprovalState state;
            switch (action)
            {
                case "check":
                    state = ApprovalState.ApprSuccess;
                    break;
                case "uncheck":
                    state = ApprovalState.ApprFailure;
                    break;
                case "invalid":
                    state = ApprovalState.Discarded;
                    break;
                default:
                    return APIResult.GetErrorResult("操作失败！");
            }

            var user = CurrentUserView;
            if (user == null)
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                return APIResult.GetSuccessResult("操作成功！");
            }
        }
        #endregion

        #region 危险作业


        /// <summary>
        /// 获取提交危险作业页面预加载数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddDangerousWork(long projectId)
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

                    List<DictionaryType> list = new List<DictionaryType>() { DictionaryType.WorkType };
                    var dicResult = proxy.GetTypeListByTypes(list);
                    if (dicResult.Flag == EResultFlag.Success && dicResult.Data != null &&
                        dicResult.Data.ContainsKey(DictionaryType.WorkType))
                    {
                        typeList = dicResult.Data[DictionaryType.WorkType];
                    }
                    var data = new
                    {
                        //监理，危险作业报备，作业单位的选择范围：（土建，包装，内衬,加固，清罐，安装）监理单位不需要看到.
                        companies = companyList.Where(p => p.CompanyId.HasValue && p.CompanyId != 0 && (p.Type == "土建" || p.Type == "包装" || p.Type == "内衬" || p.Type == "加固" || p.Type == "清罐" || p.Type == "安装")).Select(p => new
                        {
                            id = p.CompanyId.ToString(),
                            name = p.CompanyName
                        }),
                        typeList = typeList.Select(p => new
                        {
                            id = p.No,
                            name = p.Name
                        })
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 新增危险作业
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddDangerousWork()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            try
            {
                if (form["projectId"].ToLongReq() == 0 || string.IsNullOrWhiteSpace(form["projectName"]))
                {
                    throw new Exception("请选择所属项目！");
                }
                if (form["workCompanyId"].ToLongReq() == 0 || string.IsNullOrWhiteSpace(form["workCompanyName"]))
                {
                    throw new Exception("请选择作业单位！");
                }
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("未登录或登录超时！");
                }

                Epm_DangerousWork model = new Epm_DangerousWork();
                List<Base_Files> files = AppCommonHelper.UploadFile(http, user);

                model.ProjectId = form["projectId"].ToLongReq();
                model.ProjectName = form["projectName"];
                model.TaskName = form["name"];
                model.StartTime = form["startTime"].ToDateTime();
                model.EndTime = form["endTime"].ToDateTime();
                model.TaskTypeNo = form["typeNo"];
                model.TaskTypeName = form["typeName"];
                model.TaskArea = form["area"];
                model.TaskContent = form["content"];
                model.WorkCompanyId = form["workCompanyId"].ToLongReq();
                model.workCompanyName = form["workCompanyName"];
                model.Protective = form["protective"];
                model.State = (int)ApprovalState.WaitAppr;

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.AddDangerousWork(model, files);
                    if (result.Flag == EResultFlag.Success && result.Data > 0)
                    {
                        return APIResult.GetSuccessResult("新增危险作业提交成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 获取危险作业详情
        /// </summary>
        /// <param name="id">危险作业详情</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetDangerousWorkDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择危险作业！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetDangerousWorkModel(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    string businessType = BusinessType.Dangerous.ToString();
                    QueryCondition qc = new QueryCondition()
                    {
                        PageInfo = new PageListInfo()
                        {
                            isAllowPage = false
                        }
                    };
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "WorkId",
                        ExpValue = id,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });
                    qc.SortList.Add(new SortExpression()
                    {
                        SortName = "CreateTime",
                        SortType = eSortType.Desc
                    });

                    List<Base_Files> files = new List<Base_Files>();
                    List<Epm_WorkUploadRealScene> workRealSceneList = new List<Epm_WorkUploadRealScene>();
                    var workRealScenenResult = proxy.GetWorkRealSceneList(qc);
                    if (workRealScenenResult.Flag == EResultFlag.Success && workRealScenenResult.Data != null)
                    {
                        workRealSceneList = workRealScenenResult.Data;
                        List<long> workUploadIds = workRealScenenResult.Data.Select(p => p.Id).ToList();
                        files = AppCommonHelper.GetBaseFileList(proxy, workUploadIds, true);
                    }

                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });

                    var data = new
                    {
                        id = result.Data.SId,
                        projectName = result.Data.ProjectName,
                        name = result.Data.TaskName,
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        submitTime = string.Format("{0:yyyy-MM-dd}", result.Data.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                        state = result.Data.State,
                        submitUserName = result.Data.CreateUserName,
                        dangerousType = result.Data.TaskTypeNo,
                        dangerousRegion = result.Data.TaskArea,
                        dangerousProtectiveMeasures = result.Data.Protective,
                        dangerousStartTime = string.Format("{0:yyyy-MM-dd}", result.Data.StartTime),
                        dangerousEndTime = string.Format("{0:yyyy-MM-dd}", result.Data.EndTime),
                        dangerousContent = result.Data.TaskContent,
                        answerCount = ((questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0) ? questionResult.Data[id].ToString() : ""),
                        dangerWorks = workRealSceneList.Select(p => new
                        {
                            id = p.Id,
                            workId = p.WorkId,
                            name = result.Data.TaskName ?? "",
                            state = p.State ?? 0,
                            submitTime = string.Format("{0:yyyy-MM-dd}", p.UploadTime),
                            files = AppCommonHelper.GetFileList(files.Where(x => x.TableId == p.Id && string.IsNullOrEmpty(x.ImageType)).ToList(), true)
                        }),
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)result.Data.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, result.Data.CreateUserId))
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 新增上传作业实景
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddWorkRealScenen(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                var result = DangerworkList(user, projectId);
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
                    name = p.TaskContent
                });
                return APIResult.GetSuccessResult(data);
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取未上传作业实景的危险作业
        /// </summary>
        /// <param name="user">当前登录用户</param>
        /// <param name="projectId">项目 ID</param>
        /// <param name="time"></param>
        /// <returns></returns>
        private Result<List<Epm_DangerousWork>> DangerworkList(UserView user, long projectId, string time = "")
        {
            QueryCondition qc = new QueryCondition
            {
                PageInfo = new PageListInfo()
                {
                    isAllowPage = false
                }
            };

            // 查询条件未添加施工结束时间大于当前时间，以防止危险作业延期的情况，导致超过截止时间后，无法上传作业实景
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ProjectId",
                ExpValue = projectId,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.ApprSuccess,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });

            DateTime stime;
            DateTime etime;
            if (string.IsNullOrEmpty(time))
            {
                stime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                etime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + "  23:59:59");
            }
            else
            {
                stime = Convert.ToDateTime(time);
                etime = Convert.ToDateTime(time + "  23:59:59");
            }

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "StartTime",
                ExpValue = stime,
                ExpOperater = eConditionOperator.GreaterThanOrEqual,
                ExpLogical = eLogicalOperator.And
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "StartTime",
                ExpValue = etime,
                ExpOperater = eConditionOperator.LessThanOrEqual,
                ExpLogical = eLogicalOperator.And
            });


            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                QueryCondition workScenenQc = new QueryCondition();
                workScenenQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "UploadTime",
                    ExpValue = DateTime.Today,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                workScenenQc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = projectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                // 排除当前已上传作业实景的危险作业
                var workScenenResult = proxy.GetWorkRealSceneList(workScenenQc);
                if (workScenenResult.Flag == EResultFlag.Success && workScenenResult.Data != null && workScenenResult.Data.Any())
                {
                    var workIds = workScenenResult.Data.Select(p => p.WorkId ?? 0).Distinct().ToList();
                    if (workIds.Any())
                    {
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "Id",
                            ExpValue = string.Join(",", workIds),
                            ExpOperater = eConditionOperator.NotIn,
                            ExpLogical = eLogicalOperator.And
                        });
                    }
                }

                var result = proxy.GetDangerousWorkList(qc);
                return result;
            }
        }

        /// <summary>
        /// 获取待审核危险作业数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="projectId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private Result<List<Epm_DangerousWork>> DangerworkListWaitAppr(UserView user, long projectId, string time = "")
        {
            QueryCondition qc = new QueryCondition
            {
                PageInfo = new PageListInfo()
                {
                    isAllowPage = false
                }
            };

            // 查询条件未添加施工结束时间大于当前时间，以防止危险作业延期的情况，导致超过截止时间后，无法上传作业实景
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ProjectId",
                ExpValue = projectId,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.WaitAppr,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });

            //DateTime stime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //DateTime etime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + "  23:59:59");

            DateTime stime;
            DateTime etime;
            if (string.IsNullOrEmpty(time))
            {
                stime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                etime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + "  23:59:59");
            }
            else
            {
                stime = Convert.ToDateTime(time);
                etime = Convert.ToDateTime(time + "  23:59:59");
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = stime,
                ExpOperater = eConditionOperator.GreaterThanOrEqual,
                ExpLogical = eLogicalOperator.And
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateTime",
                ExpValue = etime,
                ExpOperater = eConditionOperator.LessThanOrEqual,
                ExpLogical = eLogicalOperator.And
            });



            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                var result = proxy.GetDangerousWorkList(qc);
                return result;
            }
        }

        /// <summary>
        /// 上传作业实景
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddWorkRealScenen()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["Id"].ToLongReq() == 0)
            {
                return APIResult.GetErrorResult("请选择危险作业！");
            }

            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    return APIResult.GetErrorResult(MsgCode.InvalidToken);
                }

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    Epm_WorkUploadRealScene model = new Epm_WorkUploadRealScene();
                    model.WorkId = form["Id"].ToLongReq();
                    model.ProjectId = form["projectId"].ToLongReq();
                    model.State = (int)ApprovalState.WaitAppr;
                    model.Remark = "";
                    model.UploadTime = DateTime.Now;
                    model.WorkName = form["Name"];

                    List<Base_Files> files = new List<Base_Files>();
                    files = AppCommonHelper.UploadFile(http, user);

                    var result = proxy.AddWorkRealScenen(model, files);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    return APIResult.GetSuccessResult("危险作业实景上传成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 删除上传作业实景
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object DeleteWorkRealScenen()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["Id"].ToLongReq() == 0)
            {
                return APIResult.GetErrorResult("请选择危险上传作业实景！");
            }

            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    return APIResult.GetErrorResult(MsgCode.InvalidToken);
                }

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    long workId = form["Id"].ToLongReq();
                    var result = proxy.DeleteWorkRealScenen(workId);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    return APIResult.GetSuccessResult("危险作业实景删除成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// 危险作业的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditDangerous(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteDangerousWorkByIds(ids);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var work = proxy.GetDangerousWorkModel(model.businessId);

                    var result = proxy.UpdateDangerousWorkState(model.businessId, state);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (work.Data.State == (int)ApprovalState.WorkPartAppr)
                    {
                        if (state == ApprovalState.ApprFailure)
                        {
                            state = ApprovalState.ApprFailure;
                        }
                        else
                        {
                            state = ApprovalState.WorkFinish;
                        }
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 延期

        /// <summary>
        /// 获取提交监理日志页面预加载数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddPlanDelay(long projectId)
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
                    var companyList = new List<Epm_ProjectCompany>();
                    List<Epm_Plan> planList = new List<Epm_Plan>();
                    var companyResult = proxy.GetProjectCompanyList(projectId);

                    if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null)
                    {
                        companyList = companyResult.Data.Where(t => !string.IsNullOrEmpty(t.CompanyName)).ToList();
                    }
                    QueryCondition qc = new QueryCondition();

                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ProjectId",
                        ExpValue = projectId,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    });

                    // 施工计划
                    var planResult = proxy.GetPlanList(qc);

                    if (planResult.Flag == EResultFlag.Success && planResult.Data != null)
                    {
                        planList = planResult.Data;
                    }

                    var data = new
                    {
                        companies = companyList.Select(p => new
                        {
                            id = p.CompanyId == null ? "" : p.CompanyId.ToString(),
                            name = p.CompanyName
                        }).Distinct(),
                        plans = planList.OrderBy(t => t.StartTime).Select(p => new
                        {
                            id = p.Id,
                            name = p.Name,
                            startTime = string.Format("{0:yyyy-MM-dd}", p.StartTime),
                            endTime = string.Format("{0:yyyy-MM-dd}", p.EndTime)
                        })
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 新增延期
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddPlanDelay(PlanDelayModel view)
        {
            if (view == null)
            {
                return APIResult.GetErrorResult("请填写要提交的内容！");
            }
            if (view.projectId == 0 || string.IsNullOrWhiteSpace(view.projectName))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }

            if (string.IsNullOrWhiteSpace(view.delayCompanies))
            {
                return APIResult.GetErrorResult("请选择责任单位！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                PlanDelayView model = new PlanDelayView();
                model.ProjectId = view.projectId;
                model.ProjectName = view.projectName;
                model.PlanId = view.planId;
                model.PlanName = view.planName;
                model.DelayDay = view.delayDay;
                model.Reason = view.reason;
                model.State = (int)ApprovalState.WaitAppr;

                List<PlanDelayCompanyModel> delayCompanys = JsonConvert.DeserializeObject<List<PlanDelayCompanyModel>>(view.delayCompanies);

                if (delayCompanys != null && delayCompanys.Any())
                {
                    if (delayCompanys.Count > 0)
                    {
                        foreach (var item in delayCompanys)
                        {
                            Epm_PlanDelayCompany plan = new Epm_PlanDelayCompany();

                            if (item.delayDay > 0)
                            {
                                plan.CompanyId = item.id;
                                plan.CompanyName = item.name;
                                plan.DelayDay = item.delayDay;

                                model.PlanDelayCompanys.Add(plan);
                            }
                            else
                            {
                                return APIResult.GetErrorResult("延期天数应该为数字！");
                            }
                        }
                    }
                }

                // todo: 附件处理
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.AddPlanDelay(model);
                    if (result.Flag == EResultFlag.Success)
                    {
                        return APIResult.GetSuccessResult("延期申请提交成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取延期申请详情
        /// </summary>
        /// <param name="id">延期 ID</param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object GetPlanDelayDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择延期申请！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetPlanDelayInfo(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });

                    string businessType = BusinessType.DelayApply.ToString();
                    var data = new
                    {
                        id = result.Data.SId,
                        projectName = result.Data.ProjectName,
                        planName = result.Data.PlanName,
                        files = AppCommonHelper.GetFileList(AppCommonHelper.GetBaseFileList(proxy, id)),
                        submitTime = string.Format("{0:yyyy-MM-dd}", result.Data.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(result.Data.CreateUserId, userPhoto, user),
                        state = result.Data.State,
                        submitUserName = result.Data.CreateUserName,
                        delayDay = result.Data.DelayDay,
                        reason = result.Data.Reason,
                        remark = result.Data.Remark,
                        companies = result.Data.PlanDelayCompanys.Select(p => new
                        {
                            id = p.Id,
                            companyId = p.CompanyId,
                            companyName = p.CompanyName,
                            delayDay = p.DelayDay
                        }),
                        answerCount = ((questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0) ? questionResult.Data[id].ToString() : ""),
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)result.Data.State).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, result.Data.CreateUserId))
                    };

                    return APIResult.GetSuccessResult(data);
                }
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }


        /// <summary>
        /// 延期申请(材料验收)的审核、驳回、作废操作
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditPlanDealy(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        var deleteResult = proxy.DeletePlanDelay(model.businessId);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.Check:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.UnCheck:
                            state = ApprovalState.ApprFailure;
                            break;
                        case SystemRight.Invalid:
                            state = ApprovalState.Discarded;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }

                    Epm_PlanDelay delay = new Epm_PlanDelay
                    {
                        Id = model.businessId,
                        State = (int)state
                    };

                    var result = proxy.AuditPlanDelay(delay);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

        #endregion

        #region 施工计划

        /// <summary>
        /// 获取施工计划列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private object GetPlanList(long projectId, int version)
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
                    var result = proxy.GetPlanViewList(projectId);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoData();
                    }

                    List<PlanModel> list = new List<PlanModel>();
                    for (int i = result.Data.Count - 1; i >= 0; i--)
                    {
                        list.Add(ToPlanModel(result.Data[i]));
                    }
                    int limit = 0;
                    if (result.Flag == EResultFlag.Success && result.Data.Any())
                    {
                        //limit = resultProject.Data.Limit == null ? 0 : resultProject.Data.Limit.Value;

                        DateTime startTime = result.Data.Where(t => t.StartTime.HasValue).OrderBy(t => t.StartTime).FirstOrDefault().StartTime.Value;
                        DateTime endTime = result.Data.Where(t => t.EndTime.HasValue).OrderByDescending(t => t.EndTime).FirstOrDefault().EndTime.Value;
                        DateTime? delayTime = null;
                        if (result.Data.Where(t => t.DelayTime.HasValue).Any())
                        {
                            delayTime = result.Data.Where(t => t.DelayTime.HasValue).OrderByDescending(t => t.DelayTime).FirstOrDefault().DelayTime;
                        }

                        endTime = (delayTime.HasValue && delayTime.Value > endTime) ? delayTime.Value : endTime;

                        TimeSpan sp = endTime.Subtract(startTime);

                        limit = sp.Days + 1;

                    }
                    var data = new
                    {
                        limit = limit,
                        list = list,
                    };
                    switch (version)
                    {
                        case 0:
                            return APIResult.GetSuccessResult(list);
                        case 1:
                            return APIResult.GetSuccessResult(data);
                        default:
                            break;
                    }

                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 进度跟踪列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private object GetScheduleList(long projectId, int version)
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
                    var result = proxy.GetPlanViewList(projectId);

                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }

                    List<PlanModel> list = new List<PlanModel>();
                    for (int i = result.Data.Count - 1; i >= 0; i--)
                    {
                        list.Add(ToPlanModel(result.Data[i]));
                    }
                    int limit = 0;
                    if (result.Flag == EResultFlag.Success && result.Data.Any())
                    {
                        //limit = resultProject.Data.Limit == null ? 0 : resultProject.Data.Limit.Value;

                        DateTime startTime = result.Data.Where(t => t.StartTime.HasValue).OrderBy(t => t.StartTime).FirstOrDefault().StartTime.Value;
                        DateTime endTime = result.Data.Where(t => t.EndTime.HasValue).OrderByDescending(t => t.EndTime).FirstOrDefault().EndTime.Value;
                        DateTime? delayTime = null;
                        if (result.Data.Where(t => t.DelayTime.HasValue).Any())
                        {
                            delayTime = result.Data.Where(t => t.DelayTime.HasValue).OrderByDescending(t => t.DelayTime).FirstOrDefault().DelayTime;
                        }

                        endTime = (delayTime.HasValue && delayTime.Value > endTime) ? delayTime.Value : endTime;

                        TimeSpan sp = endTime.Subtract(startTime);

                        limit = sp.Days + 1;

                    }
                    var data = new
                    {
                        limit = limit,
                        list = list,
                    };
                    switch (version)
                    {
                        case 0:
                            return APIResult.GetSuccessResult(list);
                        case 1:
                            return APIResult.GetSuccessResult(data);
                        default:
                            break;
                    }

                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        private PlanModel ToPlanModel(PlanView view)
        {
            PlanModel model = new PlanModel();
            model.id = view.Id.ToString();
            model.name = view.Name;
            model.startTime = string.Format("{0:yyyy-MM-dd}", view.StartTime);
            model.endTime = string.Format("{0:yyyy-MM-dd}", (view.DelayTime.HasValue ? view.DelayTime : view.EndTime));

            //DateTime start = Convert.ToDateTime(model.startTime);
            //DateTime end = Convert.ToDateTime(model.endTime);
            //TimeSpan sp = end.Subtract(start);
            //int days = sp.Days;

            model.limit = view.BuildDays.Value.ToString("#.##");
            model.limitState = "1";
            if (view.FactEndTime.HasValue)
            {
                var time = view.DelayTime.HasValue ? view.DelayTime : view.EndTime;
                if (view.FactEndTime < time)
                {
                    //绿色 提前完工，已完工，完工时间 小于 计划完工时间 or 变更完工时间
                    model.limitState = "2";
                }
                else if (view.FactEndTime > time)
                {
                    //红色 延期完工，已完工，完工时间 大于 计划完工时间 or 变更完工时间
                    model.limitState = "3";
                }
                else
                {
                    //蓝色 正常完工，已完工，完工时间 等于 计划完工时间 or 变更完工时间
                    model.limitState = "5";
                }
            }
            else
            {
                if (view.DelayTime.HasValue)
                {
                    //橘色 变更工期，未完工，已变更
                    model.limitState = "4";
                }
                else
                {
                    //灰色 计划工期，未完工，未变更
                    model.limitState = "1";
                }
            }

            if (view.children.Any())
            {
                foreach (var item in view.children)
                {
                    model.child.Add(ToPlanModel(item));
                }
            }

            return model;
        }
        #endregion

        #region 项目服务商
        /// <summary>
        /// 项目服务商的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditProjectCompanies(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    if (model.rightAction.Equals(SystemRight.Delete.ToString()))
                    {
                        List<long> ids = new List<long>()
                        {
                            model.businessId
                        };
                        var deleteResult = proxy.DeleteSupervisorlogByIdNew(model.businessId);
                        if (deleteResult.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(deleteResult.Exception);
                        }
                        var data1 = new
                        {
                            state = 1,
                            title = string.Empty
                        };
                        return APIResult.GetSuccessResult(data1, "操作成功！");
                    }

                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    ApprovalState state;
                    switch (action)
                    {
                        case SystemRight.SetCustomerUser:
                            state = ApprovalState.WaitAppr;
                            break;
                        case SystemRight.AuditCustomerUser:
                            state = ApprovalState.ApprSuccess;
                            break;
                        case SystemRight.RejectCustomerUser:
                            state = ApprovalState.ApprFailure;
                            break;
                        default:
                            {
                                throw new Exception("操作失败！");
                            }
                    }
                    var result = proxy.AuditProjectCompanyPmAndLink(model.businessId, state);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    int st = 0;
                    if (model.waitDo == "1")
                    {
                        st = 1;
                    }
                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var data = new
                    {
                        state = model.waitDo == "1" ? st : (int)state,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 提报整改结果
        /// <summary>
        /// 提报整改结果
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object AddRectifRecord(long projectId)
        {
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                Epm_Rectification rectif = null;
                List<Epm_RectificationItem> item = new List<Epm_RectificationItem>();
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetRectificationItemListByProjectId(projectId);
                    if (result.Data == null || !result.Data.Any())
                    {
                        return APIResult.GetSuccessNoDatas();
                    }
                    item = result.Data;

                    rectif = proxy.GetRectificationModel(result.Data[0].RectificationId.Value).Data;
                }

                var data = item.Where(t => t.Level == 2).Select(a => new
                {
                    id = a.Id,
                    address = rectif.InspectAddress,
                    people = rectif.InspectUserName,
                    date = rectif.InspectDate.ToString("yyyy-MM-dd"),
                    name = a.CheckParentName + "->" + a.CheckName,
                    remark = a.Remark ?? "",
                    checkItems = item.Where(t => t.Level == 3).Select(b => new
                    {
                        id = b.Id,
                        parentId = 0,
                        parentName = b.CheckParentName,
                        name = b.CheckName,
                        level = 3,
                        rectification = new[] { new { id = "", name = rectif.RectifRecordUserName } }
                    }).Distinct()
                });
                return APIResult.GetSuccessResult(data);
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 获取未提报整改结果的现场检查
        /// </summary>
        /// <param name="user"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        //private Result<List<Epm_Monitor>> RectifRecordList(UserView user, long projectId)
        //{
        //    QueryCondition qc = new QueryCondition
        //    {
        //        PageInfo = new PageListInfo()
        //        {
        //            isAllowPage = false
        //        }
        //    };
        //    //待整改状态，当前项目，整改单位为当前登录人所属单位
        //    qc.ConditionList.Add(new ConditionExpression()
        //    {
        //        ExpName = "ProjectId",
        //        ExpValue = projectId,
        //        ExpOperater = eConditionOperator.Equal,
        //        ExpLogical = eLogicalOperator.And
        //    });
        //    qc.ConditionList.Add(new ConditionExpression()
        //    {
        //        ExpName = "State",
        //        ExpValue = (int)RectificationState.WaitRectification,
        //        ExpOperater = eConditionOperator.Equal,
        //        ExpLogical = eLogicalOperator.And
        //    });
        //    qc.ConditionList.Add(new ConditionExpression()
        //    {
        //        ExpName = "RectifCompanyId",
        //        ExpValue = user.CompanyId,
        //        ExpOperater = eConditionOperator.Equal,
        //        ExpLogical = eLogicalOperator.And
        //    });
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
        //    {
        //        //var result = proxy.GetMonitorList(qc);
        //        return null;
        //    }
        //}

        /// <summary>
        /// 提报整改结果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddRectifRecord()
        {
            var user = CurrentUserView;
            if (user != null)
            {
                try
                {
                    var http = HttpContext.Current;
                    var form = http.Request.Form;
                    if (form["id"].ToLongReq() == 0)
                    {
                        return APIResult.GetErrorResult("请选择现场检查！");
                    }

                    var id = form["id"].ToLongReq();
                    var content = form["content"];
                    List<Base_Files> files = AppCommonHelper.UploadFile(http, user);

                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        var result = proxy.AddMonitorRectifRecord(id, content, files);
                        if (result.Flag == EResultFlag.Failure)
                        {
                            return APIResult.GetErrorResult(result.Exception);
                        }
                        return APIResult.GetSuccessResult("上传整改结果成功！");
                    }
                }
                catch (Exception ex)
                {
                    return APIResult.GetErrorResult(ex.Message);
                }
            }
            else
            {
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
        }

        /// <summary>
        /// 整改结果的审核、驳回、作废、删除
        /// </summary>
        /// <param name="model">审核内容</param>
        /// <param name="user">当前用户信息</param>
        /// <returns></returns>
        private object AuditSecurityCheck(BusinessCheck model, UserView user)
        {
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    SystemRight action = (SystemRight)Enum.Parse(typeof(SystemRight), model.rightAction);
                    RectificationState state;
                    switch (action)
                    {
                        case SystemRight.AuditRectif:
                            state = RectificationState.RectificationSuccess;
                            break;
                        case SystemRight.RejectRectif:
                            state = RectificationState.RectificationOk;
                            break;
                        default:
                            throw new Exception("操作失败！");
                    }

                    var result = proxy.ChangeMonitorState(model.businessId, state, string.Empty);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }

                    var approver = proxy.GetApproverModelByBusinId(model.businessId).Data;
                    var rectif = proxy.GetRectificationModel(approver.BusinessId.Value).Data;
                    var data = new
                    {
                        // model.waitDo 1表示需要删除的，无须返回状态值
                        state = model.waitDo == "1" ? 1 : rectif.State,
                        title = approver != null ? approver.Title : string.Empty
                    };
                    return APIResult.GetSuccessResult(data, "操作成功！");
                }
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }
        #endregion

        #region 新日志提交
        /// <summary>
        /// 提交监理日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddLogInfo()
        {
            var http = HttpContext.Current;
            var form = http.Request.Form;
            if (form["projectId"].ToLongReq() == 0 || string.IsNullOrWhiteSpace(form["projectName"]))
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }
            if (string.IsNullOrWhiteSpace(form["workCompanies"]))
            {
                return APIResult.GetErrorResult("请填写现场施工单位！");
            }

            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    SupervisorLogView model = new SupervisorLogView();
                    model.ProjectId = form["projectId"].ToLongReq();
                    model.ProjectName = form["projectName"];
                    model.TypeNo = form["weatherNo"];
                    model.TypeName = form["weatherName"];
                    model.SubmitTime = string.IsNullOrWhiteSpace(form["submitTime"]) ? DateTime.Now.AddDays(1) : Convert.ToDateTime(form["submitTime"].ToString());
                    model.WindPower = form["windPower"];
                    model.Temperature = form["temperature"];
                    //新开工程节点
                    string planId = "";
                    string planName = "";
                    if (!string.IsNullOrEmpty(form["planId"]))
                    {
                        List<Epm_Plan> planList = JsonConvert.DeserializeObject<List<Epm_Plan>>(form["planId"]);
                        if (planList != null && planList.Any())
                        {
                            foreach (var item in planList)
                            {
                                planId = planId + item.Id + ",";
                                planName = planName + item.Name + ",";
                            }
                        }
                    }
                    model.PlanId = string.IsNullOrEmpty(planId) ? "" : planId.Substring(0, planId.Length - 1);
                    model.PlanName = string.IsNullOrEmpty(planName) ? "" : planName.Substring(0, planName.Length - 1);

                    model.Content = form["content"];
                    model.TomorrowProject = form["tomorrowProject"];
                    model.Schedule = form["delayDay"];
                    model.Reason = form["reason"];
                    model.State = (int)ApprovalState.WaitAppr;
                    //新完工程节点
                    string nextPlanId = "";
                    string nextPlanName = "";
                    if (!string.IsNullOrEmpty(form["nextPlanId"]))
                    {
                        List<Epm_Plan> planList = JsonConvert.DeserializeObject<List<Epm_Plan>>(form["nextPlanId"]);
                        if (planList != null && planList.Any())
                        {
                            foreach (var item in planList)
                            {
                                nextPlanId = nextPlanId + item.Id + ",";
                                nextPlanName = nextPlanName + item.Name + ",";
                            }
                        }
                    }
                    model.nextPlanId = string.IsNullOrEmpty(nextPlanId) ? "" : nextPlanId.Substring(0, nextPlanId.Length - 1);
                    model.nextPlanName = string.IsNullOrEmpty(nextPlanName) ? "" : nextPlanName.Substring(0, nextPlanName.Length - 1);

                    List<SuperSaveinfo> workCompanys = JsonConvert.DeserializeObject<List<SuperSaveinfo>>(form["workCompanies"]);

                    if (workCompanys != null && workCompanys.Any())
                    {
                        foreach (var item in workCompanys)
                        {
                            Epm_SupervisorLogCompany com = new Epm_SupervisorLogCompany();
                            com.CompanyId = item.companyId;//供应商ID
                            com.CompanyName = item.companyName;//姓名
                            com.ManagerName = item.managerName;
                            foreach (var temp in item.personnelList)
                            {
                                Epm_ProjectlLogName projectlLogName = new Epm_ProjectlLogName();
                                try
                                {
                                    projectlLogName.personid = temp.id.Value;
                                }
                                catch (Exception)
                                { }
                                projectlLogName.name = temp.name;
                                projectlLogName.phone = temp.phone;
                                projectlLogName.type = temp.type;
                                com.ProjectlLogName.Add(projectlLogName);
                            }
                            foreach (var temp in item.workPersonnel)
                            {
                                Epm_AttendanceList attendanceList = new Epm_AttendanceList();
                                attendanceList.bepresent = temp.bePresent;
                                attendanceList.name = temp.name;
                                attendanceList.permit = temp.permit;
                                attendanceList.workPeopleTypeId = temp.workPeopleTypeId;
                                attendanceList.workPeopleTypeName = temp.workPeopleTypeName;
                                com.AttendanceList.Add(attendanceList);
                            }
                            model.SupervisorLogCompanys.Add(com);
                        }
                    }
                    List<long> workIds = null;
                    if (!string.IsNullOrWhiteSpace(form["dangerWorks"]))
                    {
                        List<DangerourWorkModel> dangerourWork = JsonConvert.DeserializeObject<List<DangerourWorkModel>>(form["dangerWorks"]);
                        if (dangerourWork.Count > 0)
                        {
                            workIds = dangerourWork.Select(p => p.id).ToList();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(form["dangerWorksWaitAppr"]))
                    {
                        List<DangerourWorkModel> dangerourWork = JsonConvert.DeserializeObject<List<DangerourWorkModel>>(form["dangerWorksWaitAppr"]);
                        if (dangerourWork.Count > 0)
                        {
                            if (workIds == null)
                            {
                                workIds = dangerourWork.Select(p => p.id).ToList();
                            }
                            else
                            {
                                workIds.AddRange(dangerourWork.Select(p => p.id).ToList());
                            }
                        }
                    }
                    // todo: 附件处理
                    var files = AppCommonHelper.UploadFile(http, user);
                    model.Attachs = files;
                    using (ClientSiteClientProxy proxys = new ClientSiteClientProxy(ProxyEx(user)))
                    {
                        var result = proxys.AddProjectlLogList(model, workIds);
                        if (result.Flag == EResultFlag.Success)
                        {
                            return APIResult.GetSuccessResult("监理日志提交成功！");
                        }
                        return APIResult.GetErrorResult(result.Exception);
                    }
                }

            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }
        #endregion

        #region 获取监理日志详情
        /// <summary>
        /// 根据日志 ID 获取监理日志详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        private object GetSupervisorNewLogDetail(long id)
        {
            if (id <= 0)
            {
                return APIResult.GetErrorResult("请选择监理日志！");
            }
            var user = CurrentUserView;
            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.GetSupervisorLogModelNew(id);
                    if (result.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult(result.Exception);
                    }
                    if (result.Data == null)
                    {
                        return Json(APIResult.GetErrorResult("该信息已被删除或不存在！"));
                    }
                    var log = result.Data;

                    var projectResult = proxy.GetProjectModel(result.Data.ProjectId.Value);
                    if (projectResult.Flag == EResultFlag.Failure)
                    {
                        return APIResult.GetErrorResult("获取监理日志详情失败！");
                    }
                    if (projectResult.Data == null)
                    {
                        return APIResult.GetErrorResult("获取监理日志详情失败!");
                    }
                    var project = projectResult.Data;

                    Dictionary<long, string> userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>()
                    {
                        result.Data.CreateUserId
                    });

                    List<Epm_WorkUploadRealScene> workRealSceneList = new List<Epm_WorkUploadRealScene>();
                    List<Base_Files> Scenefiles = new List<Base_Files>();
                    var dangerWorksResult = proxy.GetWorkRealSceneByLogId(id);
                    if (dangerWorksResult.Flag == EResultFlag.Success && dangerWorksResult.Data != null)
                    {
                        workRealSceneList = dangerWorksResult.Data;
                        List<long> workUploadIds = dangerWorksResult.Data.Select(p => p.Id).ToList();
                        Scenefiles = AppCommonHelper.GetBaseFileList(proxy, workUploadIds, true);
                    }

                    //根据监理日志获取待办危险作业
                    List<Epm_DangerousWork> workList = new List<Epm_DangerousWork>();
                    var workLogResult = proxy.GetDangerousWorkByLogId(id);
                    if (workLogResult.Flag == EResultFlag.Success && workLogResult.Data != null)
                    {
                        workList = workLogResult.Data;
                    }

                    int scheduleValue = string.IsNullOrWhiteSpace(result.Data.Schedule) ? 0 : Convert.ToInt32(result.Data.Schedule);

                    var answerCount = "";
                    var questionResult = proxy.GetQuestionCount(new List<long>() { id });
                    if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null && questionResult.Data.Count > 0)
                    {
                        answerCount = questionResult.Data[id].ToString();
                    }

                    string businessType = BusinessType.Log.ToString();

                    List<SuperSaveinfo> workCompanys = new List<SuperSaveinfo>();
                    var data = new
                    {
                        id = log.Id,
                        projectName = log.ProjectName ?? "",
                        files = AppCommonHelper.GetFileList(log.Attachs),
                        answerCount = answerCount,
                        submitTime = string.Format("{0:yyyy-MM-dd}", log.CreateTime),
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(log.CreateUserId, userPhoto, user),
                        state = log.State,
                        submitUserName = log.CreateUserName ?? "",
                        windPower = log.WindPower ?? "",
                        temperature = log.Temperature ?? "",
                        weatherName = log.TypeName ?? "",
                        workContent = log.Content ?? "",
                        tomorrowProject = log.TomorrowProject ?? "",
                        schedule = scheduleValue > 0 ? string.Format("延期 {0} 天", scheduleValue) : "正常",
                        remark = log.Remark ?? "",
                        day = string.Format("第 {0} 天/总工期 {1} 天", ((log.SubmitTime ?? DateTime.Today) - (project.PlanWorkStartTime ?? DateTime.Today)).Days, project.Limit ?? 0),
                        reason = log.Reason ?? "",
                        supervisorLogCompanies = log.SupervisorLogCompanys.Select(p => new
                        {
                            companyName = p.CompanyName,

                            personnelList = p.ProjectlLogName.Select(k => new
                            {
                                id = k.personid,
                                name = k.name,
                                phone = k.phone,
                                type = k.type,
                            }),
                            workPersonnel = p.AttendanceList.Select(l => new
                            {
                                workPeopleTypeId = l.workPeopleTypeId,
                                name = l.name,
                                workPeopleTypeName = l.workPeopleTypeName,
                                bepresent = l.bepresent == "0" ? true : false,
                                permit = l.permit == "0" ? true : false,

                            }),
                        }),

                        dangerWorks = workRealSceneList.Select(p => new
                        {
                            id = p.Id,
                            workId = p.WorkId,
                            name = p.WorkName ?? "",
                            state = p.State ?? 0,
                            submitTime = string.Format("{0:yyyy-MM-dd}", p.UploadTime),
                            files = AppCommonHelper.GetFileList(Scenefiles.Where(x => x.TableId == p.Id).ToList(), true)
                        }),
                        dangerWorksWaitAppr = workList.Select(p => new
                        {
                            id = p.Id,
                            name = p.TaskName ?? "",
                            state = p.State ?? 0,
                            workTime = string.Format("{0:yyyy-MM-dd}", p.StartTime)
                        }),
                        planList = GetPlanList(log.PlanName),
                        nextPlanList = GetPlanList(log.nextPlanName),
                        actionButton = AppCommonHelper.GetRightByState(((ApprovalState)log.State.Value).ToString(), AppCommonHelper.CreateButtonRight(user, businessType, log.CreateUserId))
                    };
                    return APIResult.GetSuccessResult(data);
                }
            }

            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }
        #endregion


        /// <summary>
        /// 新增整改单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object AddRectification()
        {

            var http = HttpContext.Current;
            var form = http.Request.Form;
            try
            {
                #region 数据验证判断
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("未登录或登录超时！");
                }
                //判断项目ID不能为空
                if (string.IsNullOrWhiteSpace(form["projectId"]))
                {
                    throw new Exception("错误：服务器无法获取项目信息！");
                }
                long projectId = form["projectId"].ToLongReq();
                //判断时间不能为空
                var time = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(form["time"]))
                {
                    time = form["time"].ToDateTime().Value;
                }
                //判断提交LIST不能为空
                if (string.IsNullOrWhiteSpace(form["rectification"]))
                {
                    throw new Exception("错误：服务器无法获取提交数据！");
                }
                List<checkItemesPer> cvlist = JsonConvert.DeserializeObject<List<checkItemesPer>>(form["rectification"]);
                //类型不能为控
                int type = 0;
                if (!string.IsNullOrWhiteSpace(form["type"]))
                {
                    type = form["type"].ToInt32().Value;
                }
                #endregion
                #region 整改单新增
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {

                    var result = proxy.AddRectificationNew(projectId, time, cvlist, type);
                    if (result.Flag == EResultFlag.Success && result.Data)
                    {
                        return APIResult.GetSuccessResult("保存成功！");
                    }
                    return APIResult.GetErrorResult(result.Exception);
                }
                #endregion
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
        }

    }
}