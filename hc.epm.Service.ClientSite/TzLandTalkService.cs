/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzLandTalkService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/23 10:36:57
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {///<summary>
     ///添加:
     ///</summary>
     /// <param name="model">要添加的model</param>
     /// <returns>受影响的行数</returns>
        public Result<int> AddTzLandTalk(Epm_TzLandTalk model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzLandTalk>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandTalk.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzLandTalk");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzLandTalk(Epm_TzLandTalk model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzLandTalk>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandTalk.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzLandTalk");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzLandTalkByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzLandTalk>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzLandTalk>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandTalk.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzLandTalkByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzLandTalk>> GetTzLandTalkList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzLandTalk>> result = new Result<List<Epm_TzLandTalk>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzLandTalk>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzLandTalkList");
            }
            return result;
        }

        /// <summary>
        /// 土地谈判协议列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzLandTalkAndProjectList(QueryCondition qc)
        {
            Result<List<TzProjectProposalView>> result = new Result<List<TzProjectProposalView>>();
            try
            {
                var query = from a in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false)
                            join b in context.Epm_TzLandTalk.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                            from tt in temp.DefaultIfEmpty()
                            select new TzProjectProposalView
                            {
                                TzProjectProposal = a,
                                TzLandTalk = tt
                            };
                string projectName = "";
                string projectNature = "";
                string companyName = "";
                string startTime = "";
                string endTime = "";
                if (qc != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "ProjectName":
                                    {
                                        projectName = value;
                                        break;
                                    }
                                case "projectNature":
                                    {
                                        projectNature = value;
                                        break;
                                    }
                                case "companyName":
                                    {
                                        companyName = value;
                                        break;
                                    }
                                case "startTime":
                                    {
                                        startTime = value;
                                        break;
                                    }
                                case "endTime":
                                    {
                                        endTime = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                      && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                      && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == ""));
                }
                else {
                    DateTime startTime1 = Convert.ToDateTime(startTime);
                    DateTime endTime1 = Convert.ToDateTime(endTime);

                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                          && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                          && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == "")
                                          && (t.TzLandTalk.TalkTime.Value >= startTime1 && t.TzLandTalk.TalkTime.Value <= endTime1));
                }

                result.AllRowsCount = query.Count();
                query = query.OrderByDescending(t => t.TzLandTalk.OperateTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount);
                result.Data = query.ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzLandTalkAndProjectList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzLandTalk> GetTzLandTalkModel(long id)
        {
            Result<Epm_TzLandTalk> result = new Result<Epm_TzLandTalk>();
            try
            {
                var model = DataOperateBusiness<Epm_TzLandTalk>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzLandTalkModel");
            }
            return result;
        }

    }
}
