using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using hc.epm.DataModel.Business;
using hc.epm.DataModel.Basic;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.epm.Common;
using hc.epm.DataModel.BaseCore;
using hc.epm.ViewModel.AppView;
using hc.Plat.WebAPI.Base.Common;
using hc.Plat.Common.Extend;

namespace hc.Plat.WebAPI.Base.Controllers
{
    /// <summary>
    /// 咨询动态接口
    /// </summary>
    public class NewsController : BaseAPIController
    {
        /// <summary>
        /// 获取首页数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetNewsInfo()
        {
            List<Epm_AdPutRecord> adList = new List<Epm_AdPutRecord>();
            QuestionModel question = new QuestionModel();
            List<Epm_Project> projectList = new List<Epm_Project>();
            List<Base_Files> files = new List<Base_Files>();

            List<long> ids = new List<long>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                #region 广告
                // 获取广告编码
                string value = ConfigurationManager.AppSettings["AdCode"] ?? "";

                QueryCondition qcad = new QueryCondition()
                {
                    PageInfo = GetPageInfo(1, true)
                };

                qcad.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "AdTargetNum",
                    ExpValue = value,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                var adResult = proxy.GetAdShowList(qcad);
                if (adResult.Flag == EResultFlag.Success && adResult.Data.Any())
                {
                    adList = adResult.Data;
                    ids.AddRange(adResult.Data.Select(p => p.Id));
                }

                #endregion

                #region 头像

                //头像
                Dictionary<long, string> userPhoto = new Dictionary<long, string>();
                var questionResult = proxy.GetHotQuestion();
                if (questionResult.Flag == EResultFlag.Success && questionResult.Data != null)
                {
                    question = questionResult.Data;
                    if (question != null)
                    {
                        userPhoto = AppCommonHelper.GetUserProfilePhotoList(proxy, new List<long>() { question.submitUserId.Value });
                    }
                }

                #endregion

                #region 项目展示

                QueryCondition qcProject = new QueryCondition()
                {
                    PageInfo = GetPageInfo(1, true)
                };
                qcProject.ConditionList = new List<ConditionExpression>
                {
                    new ConditionExpression()
                    {
                        ExpName = "State",
                        ExpValue = (int) ProjectState.Construction,
                        ExpOperater = eConditionOperator.Equal,
                        ExpLogical = eLogicalOperator.And
                    }
                };
                qcProject.SortList.Add(new SortExpression()
                {
                    SortName = "OperateTime",
                    SortType = eSortType.Desc
                });
                var projectResult = proxy.GetProjectList(qcProject);
                if (projectResult.Flag == EResultFlag.Success && projectResult.Data.Any())
                {
                    projectList = projectResult.Data.Take(1).ToList();
                }

                #endregion

                #region 获取所有附件

                if (ids.Any())
                {
                    files = AppCommonHelper.GetBaseFileList(proxy, ids);
                }

                #endregion

                #region 组织数据
                //附件
                Func<long, BaseBusiness, string> getImageUrl = delegate (long id, BaseBusiness model)
                {
                    string type = model.GetType().Name;
                    var file = files.FirstOrDefault(p => p.TableId == id && p.TableName == type && (p.ImageType == "small" || p.ImageType == "start"));
                    if (file == null)
                        return "";
                    return AppCommonHelper.GetResourceUrl(file);
                };

                var data = new
                {
                    banner = adList.Select(p => new
                    {
                        id = p.Id,
                        image = getImageUrl(p.Id, p),
                        url = p.AdUrl ?? ""
                    }),
                    hotQuestionList = (question.id == 0) ? null : (new
                    {
                        question.id,
                        question.name,
                        desc = question.workContent,
                        question.type,
                        question.answerCount,
                        submitTime = string.Format("{0:yyyy-MM-dd}", question.submitTime),
                        question.submitUserName,
                        headerUrl = AppCommonHelper.GetUserProfilePhoto(question.submitUserId.Value, userPhoto, null),
                        files = files.Where(x => x.TableId == question.id)
                    }),
                    projectList = projectList.Select(p => new
                    {
                        id = p.Id,
                        name = p.Name ?? "",
                        description = p.Description ?? "",
                        imageUrl = AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/projectdefaut.png"
                        //projectFiles.Where(x => x.TableId == p.Id && x.TableColumn == "Thumbnail").FirstOrDefault() == null ? "" : (AppCommonHelper.SystemSetting["resourceUrl"] + projectFiles.Where(x => x.TableId == p.Id && x.TableColumn == "Thumbnail").FirstOrDefault().Url)
                    }),
                    companyInfo = new
                    {
                        shortname = "中国石油本省销售分公司",
                        content = "",
                        address = "本省省武汉市江汉区常青路149号",
                        phone = "15829297065",
                        linkman = "姜斌",
                        imageUrl = AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/zylog.png"
                    },
                };

                #endregion

                return Json(APIResult.GetSuccessResult(data));
            }
        }
    }
}