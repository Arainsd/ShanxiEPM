using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.Common.Extend;
using hc.Plat.WebAPI.Base.Common;
using System.Configuration;

namespace hc.Plat.WebAPI.Base.Controllers
{
    /// <summary>
    /// 用户相关信息接口
    /// </summary>
    public class UserController : BaseAPIController
    {
        /// <summary>
        /// 获取用户工作菜单
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [APIAuthorize]
        [HttpGet]
        public object GetUserMenu(long projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return APIResult.GetSuccessNoDatas();
                }

                var user = CurrentUserView;
                if (user != null)
                {
                    var list = Enum<BusinessType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).Select(p => new { code = p.Key }).ToList();
                    if (user.CompanyType.Equals(RoleType.Owner.ToString()))
                    {
                        list = list.Where(p =>
                            p.code == BusinessType.SecurityCheck.ToString() ||
                            p.code == BusinessType.Question.ToString()).ToList();
                    }
                    if (user.CompanyType.Equals(RoleType.Supplier.ToString()))
                    {
                        using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                        {
                            if (proxy.IsSupervisor(projectId, user.UserId))
                            {
                                list = list.Where(p =>
                                p.code == BusinessType.Log.ToString() ||
                                p.code == BusinessType.Equipment.ToString() ||
                                p.code == BusinessType.Visa.ToString() ||
                                p.code == BusinessType.Question.ToString() ||
                                p.code == BusinessType.SecurityCheck.ToString() || //返回监理现场检查类型；
                                p.code == BusinessType.Rectification.ToString() || //发起整改单
                                p.code == BusinessType.Dangerous.ToString()).ToList();
                            }
                            else
                            {
                                list = list.Where(p =>
                                p.code == BusinessType.Track.ToString() ||
                                p.code == BusinessType.DelayApply.ToString() ||
                                p.code == BusinessType.Question.ToString()).ToList();

                                //list.Add(new
                                //{
                                //    code = SystemRight.UploadSecurityCheck.ToString()
                                //});
                                list.Add(new
                                {
                                    code = SystemRight.UploadWork.ToString()
                                });
                            }
                        }
                    }
                    list.Add(new
                    {
                        code = SystemRight.UploadSecurityCheck.ToString()
                    });
                    list.Add(new
                    {
                        code = SystemRight.FaceAI.ToString()
                    });
                    return APIResult.GetSuccessResult(list);
                }
                return APIResult.GetErrorResult(MsgCode.InvalidToken);
            }
            catch (Exception ex)
            {
                return APIResult.GetErrorResult(ex.Message);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <returns></returns>
        [APIAuthorize]
        [HttpGet]
        public object GetUser()
        {
            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("获取用户信息失败！");
                }

                Dictionary<long, string> userPhoto = null;
                bool userFace = false;
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
                {
                    userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { user.UserId });
                    var userFaceResult = proxy.GetAIUserFace(user.UserId);
                    if (userFaceResult.Flag == EResultFlag.Success && userFaceResult.Data != null)
                    {
                        userFace = true;
                    }
                }
                var data = new
                {
                    userId = user.UserId,
                    userName = user.RealName,
                    img = AppCommonHelper.GetUserProfilePhoto(user.UserId, userPhoto, user),
                    phone = user.Phone,
                    companyName = user.CompanyName,
                    qq = user.Qq,
                    weChat = user.WeChat,
                    mail = user.Email,
                    isFace = userFace
                };

                return APIResult.GetSuccessResult(data);
            }
            catch
            {
                return APIResult.GetErrorResult(MsgCode.UserInfoError, "获取用户信息错误！");
            }
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="model">密码修改实体</param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object UpdatePwd(UserPwd model)
        {
            if (model == null)
            {
                return Json(APIResult.GetErrorResult("请输入旧密码！"));
            }

            string oldpwd = model.oldpwd;
            string pwd = model.pwd;
            if (string.IsNullOrWhiteSpace(oldpwd))
            {
                return Json(APIResult.GetErrorResult("请输入旧密码！"));
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                return Json(APIResult.GetErrorResult("请输入新密码！"));
            }
            if (pwd.Equals(oldpwd))
            {
                return Json(APIResult.GetErrorResult("新密码不能和旧密码相同！"));
            }

            oldpwd = APIAESTool.AesDecrypt(oldpwd);
            oldpwd = DesTool.DesEncrypt(oldpwd);

            pwd = APIAESTool.AesDecrypt(pwd);
            pwd = DesTool.DesEncrypt(pwd);

            var user = CurrentUserView;

            if (user != null)
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var result = proxy.UpdatePassword(user.UserId, oldpwd, pwd);
                    if (result.Flag == EResultFlag.Success)
                    {
                        return Json(APIResult.GetSuccessResult("密码修改成功！"));
                    }
                    return Json(APIResult.GetErrorResult(MsgCode.CommonError, result.Exception.Decription));
                }
            }
            return Json(APIResult.GetErrorResult(MsgCode.InvalidToken));
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="tel">手机号</param>
        /// <param name="pwd">密码，加密</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [APIAuthorize]
        public object ForgetPwd(string tel, string pwd, string code)
        {
            //if (string.IsNullOrWhiteSpace(tel))
            //{
            //    return Json(new APIResult(MsgCode.CommonError, "请输入电话号码！"));
            //}
            //if (string.IsNullOrWhiteSpace(pwd))
            //{
            //    return Json(new APIResult(MsgCode.CommonError, "请输入新密码！"));
            //}
            //if (string.IsNullOrWhiteSpace(code))
            //{
            //    return Json(new APIResult(MsgCode.CommonError, "请输入新密码！"));
            //}

            //if (CurrentUserView != null)
            //{
            //    if (!CurrentUserView.PassWord.Equals(pwd))
            //    {
            //        return Json(new APIResult(MsgCode.CommonError, "旧密码不正确！"));
            //    }


            //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(CurrentUserId)))
            //    {
            //        var result = proxy.UpdatePassword(CurrentUserView.Id, pwd);
            //        if (result.Flag == EResultFlag.Success)
            //        {
            //            return Json(new APIResult(MsgCode.Success, "密码修改成功！"));
            //        }
            //        return Json(new APIResult(MsgCode.CommonError, result.Exception.Decription));
            //    }
            //}
            return Json(APIResult.GetErrorResult("获取用户信息错误！"));
        }

        /// <summary>
        /// 初始化接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object InitInfo()
        {
            //Dictionary<long, string> projectList = new Dictionary<long, string>();
            List<Epm_Project> projectList = new List<Epm_Project>();
            Epm_AdPutRecord ad = new Epm_AdPutRecord();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<Epm_OilStation> oilList = new List<Epm_OilStation>();
            string adUrl = "";
            List<string> attendance = new List<string>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                QueryCondition qc = new QueryCondition();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "AdTargetNum",
                    ExpValue = 0,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                // todo : 添加广告查询条件
                var adResult = proxy.GetAdShowList(qc);
                if (adResult.Flag == EResultFlag.Success && adResult.Data.Any())
                {
                    ad = adResult.Data.FirstOrDefault() ?? new Epm_AdPutRecord();
                    var filesResult = proxy.GetBaseFiles(new List<long>() { ad.Id });
                    if (filesResult.Flag == EResultFlag.Success && filesResult.Data.Any())
                    {
                        string type = ad.GetType().Name;
                        var file = filesResult.Data.FirstOrDefault(p => p.TableId == ad.Id && p.TableName == type && (p.ImageType == "big" || p.ImageType == "start"));
                        adUrl = file == null ? "" : file.Url;
                    }
                }

                var user = CurrentUserView;

                QueryCondition qc1 = new QueryCondition()
                {
                    PageInfo = GetPageInfo(1, false)
                };

                qc1.PageInfo.isAllowPage = false;

                qc1.ConditionList = new List<ConditionExpression>
                {
                    new ConditionExpression()
                    {
                        ExpName = "State",
                        ExpValue = (int) ProjectState.Construction,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    }
                };
                qc1.SortList.Add(new SortExpression()
                {
                    SortName = "OperateTime",
                    SortType = eSortType.Desc
                });

                if (user != null)
                {
                    var proList = proxy.GetProjectListById(user.CompanyId, user.UserId).Data;
                    if (proList != null)
                    {
                        projectList = proxy.GetProjectListById(user.CompanyId, user.UserId).Data;
                    }
                }
                //获取油站列表
                var oilStationResult = proxy.GetOilStationAllList();
                if (oilStationResult.Flag == EResultFlag.Success && oilStationResult.Data.Any())
                {
                    oilList = oilStationResult.Data;
                }

                //考勤设置信息
                var attendanceList = proxy.GetAttendanceModel();

                if (attendanceList.Flag == EResultFlag.Success && attendanceList.Data != null)
                {
                    attendance = attendanceList.Data.AttendanceTimeList;
                }
            }
            //签到距离
            string value = ConfigurationManager.AppSettings["FaceAIDistance"];
            var data = new
            {
                adInfo = new
                {
                    id = ad.Id,
                    image = AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/screen.jpg",
                    url = ""
                },
                projectList = projectList.Select(p => new
                {
                    projectId = p.Id,
                    projectName = p.Name,
                    latitude = p.ProjectSubjectId == null ? 0 : GetOilInfo(oilList, p.ProjectSubjectId.Value) == null ? 0 : GetOilInfo(oilList, p.ProjectSubjectId.Value).Latitude,
                    longitude = p.ProjectSubjectId == null ? 0 : GetOilInfo(oilList, p.ProjectSubjectId.Value) == null ? 0 : GetOilInfo(oilList, p.ProjectSubjectId.Value).Longitude,
                    //latitude = "34.231696",
                    //longitude = "108.93393",
                    //oilStationName = GetOilInfo(oilList, p.ProjectSubjectId.Value) == null ? "" : GetOilInfo(oilList, p.ProjectSubjectId.Value).Name,
                    distance = value ?? "0",
                }),
                waitList = new
                {
                    approvalCount = 0, //dic.ContainsKey("approvalCount") ? dic["approvalCount"] : 0,
                    unreadMsg = 0 //dic.ContainsKey("unreadMsg") ? dic["unreadMsg"] : 0
                },
                timedReminderList = getAttendence(attendance),
                sysSetting = AppCommonHelper.SystemSetting
            };

            return Json(APIResult.GetSuccessResult(data));
        }

        /// <summary>
        /// 获取打卡信息
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        private object getAttendence(List<string> attendance)
        {
            //距离提醒打卡时间
            string time = ConfigurationManager.AppSettings["ReminderTime"];
            int timtInt = Convert.ToInt32(time);
            if (attendance != null && attendance.Count > 0)
            {
                return attendance.Select(t => new
                {
                    hour = Convert.ToInt32((Convert.ToDateTime(t).AddMinutes(-timtInt)).ToShortTimeString().Split(':')[0]),
                    minute = Convert.ToInt32((Convert.ToDateTime(t).AddMinutes(-timtInt)).ToShortTimeString().Split(':')[1]),
                    title = "打卡提醒",  //提醒标题
                    content = "距离打卡时间还有" + time + "分钟,别忘记打卡哦~",   //提醒内容
                }).OrderBy(p => p.hour);
            }

            return (new List<object>() { }).ToArray();
        }

        private Epm_OilStation GetOilInfo(List<Epm_OilStation> oilList, long projectSubjectId)
        {
            Epm_OilStation value = null;
            if (oilList.Any())
            {
                value = oilList.Where(t => t.Id == projectSubjectId).FirstOrDefault();
            }
            return value;
        }

        /// <summary>
        /// 获取 APP 版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetAppVersion()
        {
            string appNum = AppNum;
            if (string.IsNullOrWhiteSpace(appNum))
            {
                return APIResult.GetErrorResult("获取更新版本失败！");
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                var result = proxy.GetAppVersion(appNum);

                if (result.Flag == EResultFlag.Failure)
                {
                    return APIResult.GetErrorResult(result.Exception);
                }

                if (result.Data == null)
                {
                    return APIResult.GetSuccessNoData();
                }

                var data = new
                {
                    appNum = result.Data.AppNum,  //包名
                    versionCode = result.Data.VersionCode,  //版本号
                    version = result.Data.VersionSn,    //版本名称
                    url = result.Data.Url,  //版本下载路径
                    isForced = result.Data.IsForced,    //是否强更：1-强制更新、2-普通更新
                    log = result.Data.UpdateLog     //版本更新说明
                };

                return APIResult.GetSuccessResult(data);
            }
        }
    }
}
