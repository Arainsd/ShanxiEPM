/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzGasStationService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/21 16:46:03
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzProjectProposal(Epm_TzProjectProposal model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Nature))
                {
                    string projectNatureType = model.Nature.ToLower();
                    string[] natures = new string[] { "KUOJ", "GAIJ", "QIANJ" };

                    // 如果是【扩建】、【改建】、【迁建】，加油站是选择项，故要根据选择的加油站信息获取其他相关数据
                    if (natures.Contains(projectNatureType))
                    {
                        if (model.StationId == null || model.StationId == 0)
                        {
                            throw new Exception("请选择加油站！");
                        }

                        var station = DataOperateBusiness<Epm_OilStation>.Get().GetModel(model.StationId.Value);
                        if (station == null)
                        {
                            throw new Exception("所选加油站不存在！");
                        }
                        model.StationCodeJg = station.Code;
                        model.StationCode = station.Code;

                        //var codeMapResult = GetCodeMap(((int)CodeMapType.OilStation).ToString(), ((int)SysMapType.BimToTz).ToString(), model.StationId.ToString());
                        //if (codeMapResult.Flag == EResultFlag.Success && codeMapResult.Data != null)
                        //{
                        //    model.StationCode = codeMapResult.Data.To_Code;
                        //}
                    }
                }

                //long companyId = CurrentCompanyID.ToInt64Req();

                //var company = DataOperateBasic<Base_Company>.Get().GetModel(companyId);
                //if (company == null)
                //{
                //    throw new Exception("未获取到所在单位！");
                //}

                //model.CompanyId = companyId;
                //model.CompanyName = CurrentCompanyName;

                //var companyCodeMapResult = GetCodeMap(((int)CodeMapType.Org).ToString(), ((int)SysMapType.BimToTz).ToString(), CurrentCompanyID);
                //if (companyCodeMapResult.Flag == EResultFlag.Success && companyCodeMapResult.Data != null)
                //{
                //    model.CompanyCodeXt = companyCodeMapResult.Data.To_Code;
                //}

                //companyCodeMapResult = GetCodeMap(((int)CodeMapType.Org).ToString(), ((int)SysMapType.BimToTz).ToString(), "10");
                //if (companyCodeMapResult.Flag == EResultFlag.Success && companyCodeMapResult.Data != null)
                //{
                //    model.CompanyCodeXt = companyCodeMapResult.Data.To_Code;
                //}
                model.ProvinceCode = "10";
                model.ProvinceName = "陕西";

                SetCreateUser(model);
                SetCurrentUser(model);

                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);

                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (model.Nature != "XMXZTXJY")
                {
                    #region  项目提出调用协同接口
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                    {
                        XtTzProjectProposalView view = new XtTzProjectProposalView();

                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        else
                        {
                            view.hr_sqr = baseUser.ObjeId;
                        }

                        view.ProjectName = model.ProjectName;
                        view.NatureName = model.NatureName;
                        view.ApplyTime = string.Format("{0:yyyy-MM-dd}", model.ApplyTime);
                        view.StationName = model.StationName;
                        view.CompanyName = model.CompanyName.ToString();
                        view.Recommender = model.Recommender;
                        view.RecommenderJob = model.RecommenderJob;
                        view.RecommenderDept = model.RecommenderDept;
                        view.DeclarerUser = model.DeclarerUser;
                        view.Position = model.Position;
                        view.StationType = model.StationType;
                        view.PredictMoney = model.PredictMoney.ToString();
                        view.CNGY = model.CNG.ToString();
                        view.OilSalesTotal = model.OilSalesTotal.ToString();
                        view.LNGQ = model.LNG.ToString();
                        view.ProjectAddress = model.ProjectAddress;

                        //上传附件
                        if (model.TzAttachs != null && model.TzAttachs.Any())
                        {
                            //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                            //foreach (var item in model.TzAttachs)
                            //{
                            //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                            //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;

                            //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                            //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                            //}
                            //if (view.Temp_TzAttachs != null)
                            //{
                            //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                            //}
                            view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                        }
                        model.WorkFlowId = XtWorkFlowService.CreateTzProjectProposalWorkFlow(view);
                        //model.WorkFlowId = XtWorkFlowSubmitService.GetFlowId(view, view.ProjectName, view.hr_sqr, XtWorkFlowCode.WfXmtcsq);
                    }
                    #endregion
                }

                var rows = DataOperateBusiness<Epm_TzProjectProposal>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectProposal.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzProjectProposal");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectProposal(Epm_TzProjectProposal model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请选择要修改的内容");
                }
                var data = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetList(t => t.Id == model.Id && t.State != (int)PreProjectState.Discarded && t.State != (int)PreProjectState.Closed).FirstOrDefault();
                if (data == null)
                {
                    throw new Exception("要修改的信息不存在！");
                }

                data.ProjectName = model.ProjectName;
                data.Nature = model.Nature;
                data.NatureName = model.NatureName;
                data.Recommender = model.Recommender;
                data.RecommenderDept = model.RecommenderDept;

                data.RecommenderJob = model.RecommenderJob;
                data.DeclarerUser = model.DeclarerUser;
                data.Position = model.Position;
                data.PositionType = model.PositionType;
                data.Position = model.Position;

                data.StationType = model.StationType;
                data.StationTypeName = model.StationTypeName;
                data.PredictMoney = model.PredictMoney;
                data.CNG = model.CNG;
                data.LNG = model.LNG;
                data.Remark = model.Remark;

                if (!string.IsNullOrWhiteSpace(model.Nature))
                {
                    string projectNatureType = model.Nature.ToLower();
                    string[] natures = new string[] { "KUOJ", "GAIJ", "QIANJ" };

                    // 如果是【扩建】、【改建】、【迁建】，加油站是选择项，故要根据选择的加油站信息获取其他相关数据
                    if (natures.Contains(projectNatureType))
                    {
                        if (model.StationId == null || model.StationId == 0)
                        {
                            throw new Exception("请选择加油站！");
                        }

                        if (data.StationId != model.StationId)
                        {
                            var station = DataOperateBusiness<Epm_OilStation>.Get().GetModel(model.StationId.Value);
                            if (station == null)
                            {
                                throw new Exception("所选加油站不存在！");
                            }

                            data.StationCode = station.Code;

                            //var codeMapResult = GetCodeMap(((int)CodeMapType.OilStation).ToString(), ((int)SysMapType.BimToTz).ToString(), model.StationId.ToString());
                            //if (codeMapResult.Flag == EResultFlag.Success && codeMapResult.Data != null)
                            //{
                            //    data.CompanyCodeXt = codeMapResult.Data.To_Code;
                            //}
                        }
                    }
                }
                //long companyId = CurrentCompanyID.ToInt64Req();

                //var company = DataOperateBasic<Base_Company>.Get().GetModel(companyId);
                //if (company == null)
                //{
                //    throw new Exception("未获取到所在单位！");
                //}

                //model.CompanyId = companyId;
                //model.CompanyName = CurrentCompanyName;
                //model.CompanyCodeProject = CurrentCompanyID;

                //var companyCodeMapResult = GetCodeMap(((int)CodeMapType.Org).ToString(), ((int)SysMapType.BimToTz).ToString(), CurrentCompanyID);
                //if (companyCodeMapResult.Flag == EResultFlag.Success && companyCodeMapResult.Data != null)
                //{
                //    model.CompanyCodeInvest = companyCodeMapResult.Data.To_Code;
                //}

                //companyCodeMapResult = GetCodeMap(((int)CodeMapType.Org).ToString(), ((int)SysMapType.BimToTz).ToString(), "10");
                //if (companyCodeMapResult.Flag == EResultFlag.Success && companyCodeMapResult.Data != null)
                //{
                //    model.ProjectCodeInvest = companyCodeMapResult.Data.To_Code;
                //}
                //model.ProjectCodeProject = "10";

                SetCurrentUser(model);

                //上传附件
                //if (model.TzAttachs != null && model.TzAttachs.Any())
                //{
                //    AddFilesBytzTable(model, model.TzAttachs);
                //}
                AddFilesBytzTable(model, model.TzAttachs);

                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (model.Nature != "XMXZTXJY")
                {
                    #region  项目提出调用协同接口
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                    {
                        XtTzProjectProposalView view = new XtTzProjectProposalView();

                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        else
                        {
                            view.hr_sqr = baseUser.ObjeId;
                        }

                        view.ProjectName = model.ProjectName;
                        view.NatureName = model.NatureName;
                        view.ApplyTime = string.Format("{0:yyyy-MM-dd}", model.ApplyTime);
                        view.StationName = model.StationName;
                        view.CompanyName = model.CompanyName.ToString();
                        view.Recommender = model.Recommender;
                        view.RecommenderJob = model.RecommenderJob;
                        view.RecommenderDept = model.RecommenderDept;
                        view.DeclarerUser = model.DeclarerUser;
                        view.Position = model.Position;
                        view.StationType = model.StationType;
                        view.PredictMoney = model.PredictMoney.ToString();
                        view.CNGY = model.CNG.ToString();
                        view.OilSalesTotal = model.OilSalesTotal.ToString();
                        view.LNGQ = model.LNG.ToString();
                        view.ProjectAddress = model.ProjectAddress;

                        //上传附件
                        if (model.TzAttachs != null && model.TzAttachs.Any())
                        {
                            //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                            //foreach (var item in model.TzAttachs)
                            //{
                            //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                            //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                            //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                            //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                            //}
                            //if (view.Temp_TzAttachs != null)
                            //{
                            //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                            //}
                            view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                        }

                        model.WorkFlowId = XtWorkFlowService.CreateTzProjectProposalWorkFlow(view);
                        //model.WorkFlowId = XtWorkFlowSubmitService.GetFlowId(view,view.ProjectName,view.hr_sqr,XtWorkFlowCode.WfXmtcsq);
                    }
                    #endregion
                }
                var rows = DataOperateBusiness<Epm_TzProjectProposal>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzProjectProposal.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectProposal");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectProposalByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectProposal>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectProposal.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzProjectProposalByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetSingleTzProjectProposalList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzProjectProposal>(context, qc);

                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        var model = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(i => i.ProjectId == item.Id).FirstOrDefault();
                        if (model != null)
                        {
                            item.ApprovalNo = model.ApprovalNo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSingleTzProjectProposalList");
            }
            return result;
        }

        /// <summary>
        /// 获取项目批复通过并且批复号不为空的项目
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectProposal>> GetProjectProposalList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                int state = (int)PreProjectState.ApprovalSuccess;
                var query = (from a in context.Epm_TzProjectProposal.Where(p => !p.IsDelete)
                             join b in context.Epm_TzProjectApprovalInfo.Where(p => !p.IsDelete) on a.Id equals b.ProjectId
                             select new
                             {
                                 a,      // 项目提出信息
                                 b      // 项目批复请示
                             }).Where(t => t.a.State == state && t.b.State == state);

                if (qc.ConditionList != null && qc.ConditionList.Any())
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
                                        query = query.Where(p => p.a.ProjectName.Contains(value));
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;
                int total = query.Count();
                var list = query.OrderByDescending(p => p.a.ApplyTime).Skip(skip).Take(take).AsEnumerable().Select(p => new Epm_TzProjectProposal()
                {
                    Id=p.a.Id,
                    ProjectName = p.a.ProjectName,
                    ProjectCode = p.a.ProjectCode,
                    Nature = p.a.Nature,
                    NatureName = p.a.NatureName,
                    StationId = p.a.StationId,
                    StationName = p.a.StationName,
                    StationCode = p.a.StationCode,
                    StationCodeJg = p.a.StationCodeJg,
                    ApplyTime = p.a.ApplyTime,
                    ProvinceCode = p.a.ProvinceCode,
                    ProvinceName = p.a.ProvinceName,
                    CompanyId = p.a.CompanyId,
                    CompanyName = p.a.CompanyName,
                    Recommender = p.a.Recommender,
                    RecommenderJob = p.a.RecommenderJob,
                    RecommenderDept = p.a.RecommenderDept,
                    DeclarerUser = p.a.DeclarerUser,
                    PositionType = p.a.PositionType,
                    Position = p.a.Position,
                    ProjectAddress = p.a.ProjectAddress,
                    StationType = p.a.StationType,
                    StationTypeName = p.a.StationTypeName,
                    PredictMoney = p.a.PredictMoney,
                    CNG = p.a.CNG,
                    LNG = p.a.LNG,
                    OilSalesTotal = p.a.OilSalesTotal,
                    ApprovalNo = p.b.ApprovalNo,
                }).ToList();

                result.Data = list;
                result.AllRowsCount = total;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetProjectProposalList");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
        /// <summary>
        /// 获取项目详情列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectProposalList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from a in context.Epm_TzProjectProposal.Where(p => !p.IsDelete)
                            join b in context.Epm_TzSiteSurvey.Where(p => !p.IsDelete) on a.Id equals b.ProjectId into bref
                            from b in bref.DefaultIfEmpty()
                            join c in context.Epm_TzInitialTalk.Where(p => !p.IsDelete) on a.Id equals c.ProjectId into cref
                            from c in cref.DefaultIfEmpty()
                            join d in context.Epm_TzLandTalk.Where(p => !p.IsDelete) on a.Id equals d.ProjectId into dref
                            from d in dref.DefaultIfEmpty()
                            join e in context.Epm_TzFormTalkFile.Where(p => !p.IsDelete) on a.Id equals e.ProjectId into eref
                            from e in eref.DefaultIfEmpty()
                            join f in context.Epm_TzProjectApproval.Where(p => !p.IsDelete) on a.Id equals f.ProjectId into fref
                            from f in fref.DefaultIfEmpty()
                            select new
                            {
                                a,      // 项目提出信息
                                b,      // 现场考察信息
                                c,      // 初次谈判信息
                                d,      // 土地协议转让谈判
                                e,      // 组织材料评审
                                f       // 项目批复请示
                            };

                if (qc.ConditionList != null && qc.ConditionList.Any())
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
                                        query = query.Where(p => p.a.ProjectName.Contains(value));
                                        break;
                                    }
                                case "projectNature":
                                    {
                                        query = query.Where(p => p.a.Nature == value);
                                        break;
                                    }
                                case "companyName":
                                    {
                                        query = query.Where(p => p.a.CompanyName.Contains(value));
                                        break;
                                    }
                                case "startTime":
                                    {
                                        DateTime startTime;
                                        if (DateTime.TryParse(value, out startTime))
                                        {
                                            query = query.Where(p => p.a.ApplyTime >= startTime);
                                        }
                                        break;
                                    }
                                case "endTime":
                                    {
                                        DateTime endTime;
                                        if (DateTime.TryParse(value, out endTime))
                                        {
                                            query = query.Where(p => p.a.ApplyTime < endTime);
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;
                int total = query.Count();
                var list = query.OrderByDescending(p => p.a.ApplyTime).Skip(skip).Take(take).AsEnumerable().Select(p => new TzProjectProposalView()
                {
                    TzProjectProposal = p.a,
                    TzSiteSurvey = p.b,
                    TzInitialTalk = p.c,
                    TzLandTalk = p.d,
                    TzFormTalkFile = p.e,
                    TzProjectApproval = p.f
                }).ToList();

                List<Epm_TzProjectProposal> data = new List<Epm_TzProjectProposal>();

                List<long> projectIds = list.Select(p => p.TzProjectProposal.Id).ToList();

                var talkRecordList = DataOperateBusiness<Epm_TzTalkRecord>.Get().GetList(p => !p.IsDelete && projectIds.Contains(p.ProjectId ?? 0)).ToList();
                //var talkRecordConfirmList = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().GetList(p => !p.IsDelete && projectIds.Contains(p.ProjectId ?? 0)).ToList();
                var talkSignList = DataOperateBusiness<Epm_TzTalkSign>.Get().GetList(p => !p.IsDelete && projectIds.Contains(p.ProjectId ?? 0)).ToList();
                var talkLeaderSignList = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().GetList(p => !p.IsDelete && projectIds.Contains(p.ProjectId ?? 0)).ToList();
                var secordTalkList = DataOperateBusiness<Epm_TzSecondTakl>.Get().GetList(p => !p.IsDelete && projectIds.Contains(p.ProjectId ?? 0)).ToList();
                foreach (var item in list)
                {
                    // 项目提出
                    Epm_TzProjectProposal ProjectProposal = new Epm_TzProjectProposal();
                    ProjectProposal = item.TzProjectProposal;
                    data.Add(ProjectProposal);

                    // 现场调研
                    if (item.TzSiteSurvey != null && item.TzSiteSurvey.Id != 0)
                    {
                        ProjectProposal.ProjectStage = "现场调研";
                        ProjectProposal.OperateTypeName = "现场考察";
                    }
                    else
                    {
                        ProjectProposal.ProjectStage = "项目提出";
                        var proSubmissionApprova = DataOperateBusiness<Epm_TzProSubmissionApprova>.Get().Single(p => p.ProjectId == ProjectProposal.Id);
                        if (proSubmissionApprova != null)
                        {
                            ProjectProposal.OperateTypeName = proSubmissionApprova.OperationTypeName;
                        }
                        else
                        {
                            ProjectProposal.OperateTypeName = "未审核";
                        }
                        continue;
                    }

                    // 初次谈判或土地转让谈判
                    string[] projectNatures = new string[] { "KUOJ", "QIANJ", "XINJ" };
                    if (!projectNatures.Contains(ProjectProposal.Nature))
                    {
                        if (item.TzInitialTalk != null && item.TzInitialTalk.Id != 0)
                        {
                            ProjectProposal.ProjectStage = "初次谈判";
                            ProjectProposal.OperateTypeName = item.TzInitialTalk.OperateUserName;
                        }
                    }
                    else
                    {
                        if (item.TzLandTalk != null && item.TzLandTalk.Id != 0)
                        {
                            ProjectProposal.ProjectStage = "土地协议出让谈判";
                            ProjectProposal.OperateTypeName = item.TzLandTalk.OperateUserName;
                        }
                    }

                    // 组织评审材料
                    if (item.TzFormTalkFile != null && item.TzFormTalkFile.Id > 0)
                    {
                        ProjectProposal.ProjectStage = "组织评审材料";
                        ProjectProposal.OperateTypeName = item.TzFormTalkFile.OperateUserName;
                    }

                    // 项目评审
                    var proTalkRecordList = talkRecordList.Where(p => p.ProjectId == ProjectProposal.Id).OrderBy(p => p.CreateTime).ToList();
                    if (proTalkRecordList.Any())
                    {
                        var firstTalkRecord = proTalkRecordList.FirstOrDefault();
                        if (firstTalkRecord != null)
                        {
                            ProjectProposal.ProjectStage = "项目评审";
                            ProjectProposal.OperateTypeName = "投资部门确认";
                            //var talkRecordConfirm = talkRecordConfirmList.FirstOrDefault(p => p.DataId == firstTalkRecord.Id);
                            //if(talkRecordConfirm != null)
                            //{
                            //}

                            if (talkSignList.Any(p => p.DataId == firstTalkRecord.Id))
                            {
                                ProjectProposal.OperateTypeName = "加管部门会签";
                            }
                            if (talkLeaderSignList.Any(p => p.DataId == firstTalkRecord.Id))
                            {
                                ProjectProposal.OperateTypeName = "领导签发";
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // 二次谈判
                    if (!projectNatures.Contains(ProjectProposal.Nature))
                    {
                        var secordTalk = secordTalkList.OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (secordTalk != null)
                        {
                            ProjectProposal.ProjectStage = "二次谈判";

                            var secordTalkAudit = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Single(p => !p.IsDelete && p.ProjectId == ProjectProposal.Id && p.DataId == secordTalk.Id);
                            if (secordTalkAudit != null)
                            {
                                ProjectProposal.OperateTypeName = secordTalkAudit.OperationTypeName;
                            }
                            else
                            {
                                ProjectProposal.OperateTypeName = "未审核";
                            }
                        }
                    }

                    // 项目二次评审
                    if (proTalkRecordList.Any())
                    {
                        var secondTalkRecord = proTalkRecordList.Skip(1).FirstOrDefault();
                        if (secondTalkRecord != null)
                        {
                            ProjectProposal.ProjectStage = "二次评审";
                            ProjectProposal.OperateTypeName = "投资部门确认";
                            //var talkRecordConfirm = talkRecordConfirmList.FirstOrDefault(p => p.DataId == firstTalkRecord.Id);
                            //if(talkRecordConfirm != null)
                            //{
                            //}

                            if (talkSignList.Any(p => p.DataId == secondTalkRecord.Id))
                            {
                                ProjectProposal.OperateTypeName = "加管部门会签";
                            }
                            if (talkLeaderSignList.Any(p => p.DataId == secondTalkRecord.Id))
                            {
                                ProjectProposal.OperateTypeName = "领导签发";
                            }
                        }
                    }


                    // 三次谈判
                    if (!projectNatures.Contains(ProjectProposal.Nature))
                    {
                        var threeTalk = secordTalkList.OrderBy(p => p.CreateTime).FirstOrDefault();
                        if (threeTalk != null)
                        {
                            ProjectProposal.ProjectStage = "三次谈判";

                            var threeTalkAudit = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Single(p => !p.IsDelete && p.ProjectId == ProjectProposal.Id && p.DataId == threeTalk.Id);
                            if (threeTalkAudit != null)
                            {
                                ProjectProposal.OperateTypeName = threeTalkAudit.OperationTypeName;
                            }
                            else
                            {
                                ProjectProposal.OperateTypeName = "未审核";
                            }
                        }
                    }

                    // 项目批复
                    if (item.TzProjectApproval != null && item.TzProjectApproval.Id != 0)
                    {
                        ProjectProposal.Titanic = item.TzProjectApproval.Titanic;
                        ProjectProposal.ProjectStage = "项目批复";
                        ProjectProposal.OperateTypeName = item.TzProjectApproval.OperationTypeName;
                        continue;
                    }
                }

                result.Data = data;
                result.AllRowsCount = total;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetTzGasStationList");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzProjectProposal> GetTzProjectProposalModel(long id)
        {
            Result<Epm_TzProjectProposal> result = new Result<Epm_TzProjectProposal>();
            try
            {
                var model = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(id);

                List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                tzAttachsList = GetFilesByTZTable("Epm_TzProjectProposal", id).Data;
                if (tzAttachsList != null && tzAttachsList.Any())
                {
                    model.TzAttachs = tzAttachsList;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectProposalModel");
            }
            return result;
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<TzProjectProposalInfoView> GetTzProjectProposalALL(long id)
        {
            Result<TzProjectProposalInfoView> result = new Result<TzProjectProposalInfoView>();
            try
            {
                TzProjectProposalInfoView view = new TzProjectProposalInfoView();

                //加油站项目信息表
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(p => p.Id == id && (p.State == (int)PreProjectState.ApprovalSuccess || p.State == (int)PreProjectState.Closed || p.State == (int)PreProjectState.WaitApproval));

                if (tzProjectProposal != null)
                {
                    view.TzProjectProposal = tzProjectProposal;
                    view.IsColseed = true;
                    //获取项目提出相关附件
                    view.TzProjectProposal.TzAttachs = GetFilesByTZTable("Epm_TzProjectProposal", id).Data;

                    if (tzProjectProposal.State == (int)PreProjectState.Closed)
                    {
                        view.IsColseed = false;
                    }

                    #region 现场调研表
                    TzResearchAllView researView = new TzResearchAllView();

                    //现场工程方面调研
                    var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfEngineering != null)
                    {
                        if (tzResearchOfEngineering.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        researView.TzResearchOfEngineering = tzResearchOfEngineering;
                    }
                    //信息方面调研
                    var tzResearchOfInformation = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfInformation != null)
                    {
                        researView.TzResearchOfInformation = tzResearchOfInformation;
                        //获取现场调研相关附件
                        researView.TzResearchOfInformation.TzAttachs = GetFilesByTZTable("Epm_TzResearchOfInformation", tzResearchOfInformation.Id).Data;
                    }

                    //现场投资调研
                    var tzResearchOfInvestment = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfInvestment != null)
                    {
                        researView.TzResearchOfInvestment = tzResearchOfInvestment;
                    }

                    //现场法律调研
                    var tzResearchOfLaw = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfLaw != null)
                    {
                        researView.TzResearchOfLaw = tzResearchOfLaw;
                    }

                    //经营方面调研
                    var tzResearchOfManagement = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfManagement != null)
                    {
                        researView.TzResearchOfManagement = tzResearchOfManagement;
                    }

                    //安全方面调研
                    var tzResearchOfSecurity = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzResearchOfSecurity != null)
                    {
                        researView.TzResearchOfSecurity = tzResearchOfSecurity;
                    }

                    view.TzResearchAllView = researView;

                    #endregion

                    //初次谈判
                    var tzFirstNegotiation = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzFirstNegotiation != null)
                    {
                        if (tzFirstNegotiation.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        view.TzFirstNegotiation = tzFirstNegotiation;
                        //获取初次谈判相关附件
                        view.TzFirstNegotiation.TzAttachs = GetFilesByTZTable("Epm_TzFirstNegotiation", tzFirstNegotiation.Id).Data;
                    }

                    //土地谈判协议
                    var tzLandNegotiation = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.Submitted || p.State == (int)PreProjectState.Closed));
                    if (tzLandNegotiation != null)
                    {
                        if (tzLandNegotiation.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        view.TzLandNegotiation = tzLandNegotiation;
                        //获取土地协议相关附件
                        view.TzLandNegotiation.TzAttachs = GetFilesByTZTable("Epm_TzLandNegotiation", tzLandNegotiation.Id).Data;
                    }

                    //评审材料上报
                    var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.ApprovalSuccess || p.State == (int)PreProjectState.Closed || p.State == (int)PreProjectState.WaitApproval));
                    if (tzFormTalkFile != null)
                    {
                        if (tzFormTalkFile.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        view.TzFormTalkFile = tzFormTalkFile;
                        //获取评审材料上报信息
                        //获取评审材料上报附件
                        view.TzFormTalkFile.TzAttachs = GetFilesByTZTable("Epm_TzFormTalkFile", tzFormTalkFile.Id).Data;
                    }

                    //项目评审
                    var tzProjectReveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.ApprovalSuccess || p.State == (int)PreProjectState.Closed || p.State == (int)PreProjectState.WaitApproval));
                    if (tzProjectReveiews != null)
                    {
                        if (tzProjectReveiews.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        //获取评审会记录附件
                        tzProjectReveiews.TzAttachs = GetFilesByTZTable("Epm_TzProjectReveiews", tzProjectReveiews.Id).Data;

                        view.TzProjectReveiews = tzProjectReveiews;
                    }

                    //会议决策
                    var meetingFileReport = DataOperateBusiness<Epm_MeetingFileReport>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.ApprovalSuccess || p.State == (int)PreProjectState.Closed || p.State == (int)PreProjectState.WaitApproval));
                    if (meetingFileReport != null)
                    {
                        if (meetingFileReport.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                        //获取评审会记录附件
                        meetingFileReport.TzAttachs = GetFilesByTZTable("Epm_MeetingFileReport", meetingFileReport.Id).Data;

                        meetingFileReport.LNG = tzProjectProposal.LNG;
                        meetingFileReport.OilSalesTotal = tzProjectProposal.OilSalesTotal;
                        meetingFileReport.CNG = tzProjectProposal.CNG;

                        view.MeetingFileReport = meetingFileReport;
                    }

                    //项目批复请示
                    var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Single(p => p.ProjectId == id && (p.State == (int)PreProjectState.ApprovalSuccess || p.State == (int)PreProjectState.Closed || p.State == (int)PreProjectState.WaitApproval));
                    if (tzProjectApprovalInfo != null)
                    {
                        view.TzProjectApprovalInfo = tzProjectApprovalInfo;
                        //获取组织材料评审附件
                        view.TzProjectApprovalInfo.TzAttachs = GetFilesByTZTable("Epm_TzProjectApprovalInfo", tzProjectApprovalInfo.Id).Data;

                        view.TzProjectApprovalInfo.LNG = tzProjectProposal.LNG;
                        view.TzProjectApprovalInfo.OilSalesTotal = tzProjectProposal.OilSalesTotal;
                        view.TzProjectApprovalInfo.CNG = tzProjectProposal.CNG;

                        if (tzProjectApprovalInfo.State == (int)PreProjectState.ApprovalSuccess || tzProjectApprovalInfo.State == (int)PreProjectState.Closed)
                        {
                            view.IsColseed = false;
                        }
                    }

                    result.Data = view;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectProposalALL");
            }
            return result;
        }

        public Result<int> AddConferenceMaterials(string tableName, long id, List<Epm_TzAttachs> fileList, InvestmentEnclosure ie)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (fileList != null && fileList.Any())
                {
                    result.Data = AddConferenceFiles(tableName, id, fileList, InvestmentEnclosure.ConferenceMaterials);
                }
                return result;
            }
            catch (Exception)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = 0;
                return result;
            }
        }

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectProposalState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            //DbContextTransaction transaction = context.Database.BeginTransaction();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(item);
                    var userModel = DataOperateBasic<Base_User>.Get().GetModel(model.OperateUserId);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        //model.OperateUserId = model.CreateUserId;
                        //model.OperateUserName = model.CreateUserName;
                        //model.OperateTime = DateTime.Now;
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzProjectProposal>.Get().Update(model);

                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            TzResearchView view = new TzResearchView();
                            view.ProjectId = model.Id;
                            view.State = (int)PreProjectState.WaitSubmitted;
                            AddTzSiteSurvey(view);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                        if (model.Nature != "XMXZTXJY")
                        {
                            #region 更新RPA数据
                            var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                            //var rpaModel = new RPAProjectProposalView()
                            var rpaModel = new RPA_ProjectProposal();
                            var omadsModel = new OMADS_ProjectProposal();
                            var tempModel = new TEMP_ProjectProposal();
                            #region rpaModel
                            rpaModel.ApplyTime = model.ApplyTime;
                            rpaModel.Auditor = "";
                            rpaModel.CompanyName = model.CompanyName;
                            rpaModel.companys = CurrentCompanyName;
                            rpaModel.DataId = null;
                            rpaModel.Declarer = model.DeclarerUser;
                            var files = GetFilesByTZTable("Epm_TzProjectProposal", model.Id);

                            for (int i = 0; i < files.Data.Count; i++)
                            {
                                rpaModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                            }
                            rpaModel.NatureName = model.NatureName;
                            rpaModel.Position = model.Position;
                            rpaModel.PredictDayGas = model.LNG + model.CNG;
                            rpaModel.PredictDayOil = model.OilSalesTotal;
                            rpaModel.PredictMoney = model.PredictMoney;
                            rpaModel.ProjectAddress = model.ProjectAddress;
                            rpaModel.ProjectCode = model.ProjectCode;
                            rpaModel.ProjectName = model.ProjectName;
                            rpaModel.ProvincesName = model.ProvinceName;
                            rpaModel.Recommender = model.Recommender;
                            rpaModel.RecommenderCompany = model.RecommenderDept;
                            rpaModel.RecommenderJob = model.RecommenderJob;
                            rpaModel.Remark = model.Remark;
                            rpaModel.StationCode = model.StationCode;
                            rpaModel.StationName = model.StationName;
                            rpaModel.StationType = model.StationTypeName;
                            rpaModel.ProjectType = model.ProjectType;
                            rpaModel.TableName = "";
                            var strName = model.CompanyName.Substring(0, 2);
                            switch (strName)
                            {
                                case "西安":
                                    rpaModel.username = "sxxayw";
                                    break;
                                case "咸阳":
                                    rpaModel.username = "sxxyyw";
                                    break;
                                case "渭南":
                                    rpaModel.username = "sxwnyw";
                                    break;
                                case "宝鸡":
                                    rpaModel.username = "sxbjyw";
                                    break;
                                case "铜川":
                                    rpaModel.username = "sxtcyw";
                                    break;
                                case "商洛":
                                    rpaModel.username = "sxslyw";
                                    break;
                                case "汉中":
                                    rpaModel.username = "sxhzyw";
                                    break;
                                case "安康":
                                    rpaModel.username = "sxakyw";
                                    break;
                                case "榆林":
                                    rpaModel.username = "sxylyw";
                                    break;
                                case "延安":
                                    rpaModel.username = "sxyayw";
                                    break;
                                default:
                                    break;
                            }

                            rpaModel.WriteMark = 0;
                            rpaModel.WriteResult = 0;
                            rpaModel.FollowOperate = "";
                            rpaModel.FileNumber = files.Data.Count;
                            rpaModel.status1 = 0;
                            //SetCreateUser(rpaModel);
                            //SetCurrentUser(rpaModel);
                            rpaModel.OperateUserId = model.CreateUserId;
                            rpaModel.OperateUserName = model.CreateUserName;
                            rpaModel.OperateTime = DateTime.Now;
                            rpaModel.CreateUserId = model.CreateUserId;
                            rpaModel.CreateUserName = model.CreateUserName;
                            rpaModel.OperateTime = DateTime.Now;
                            #endregion
                            #region omadsModel
                            omadsModel.ApplyTime = model.ApplyTime;
                            omadsModel.Auditor = "";
                            omadsModel.CompanyName = model.CompanyName;
                            omadsModel.companys = CurrentCompanyName;
                            omadsModel.DataId = null;
                            omadsModel.Declarer = model.DeclarerUser;
                            for (int i = 0; i < files.Data.Count; i++)
                            {
                                omadsModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                            }
                            omadsModel.NatureName = model.NatureName;
                            omadsModel.Position = model.Position;
                            omadsModel.PredictDayGas = model.LNG + model.CNG;
                            omadsModel.PredictDayOil = model.OilSalesTotal;
                            omadsModel.PredictMoney = model.PredictMoney;
                            omadsModel.ProjectAddress = model.ProjectAddress;
                            omadsModel.ProjectCode = model.ProjectCode;
                            omadsModel.ProjectName = model.ProjectName;
                            omadsModel.ProvincesName = model.ProvinceName;
                            omadsModel.Recommender = model.Recommender;
                            omadsModel.RecommenderCompany = model.RecommenderDept;
                            omadsModel.RecommenderJob = model.RecommenderJob;
                            omadsModel.Remark = model.Remark;
                            omadsModel.StationCode = model.StationCode;
                            omadsModel.StationName = model.StationName;
                            omadsModel.StationType = model.StationTypeName;
                            omadsModel.ProjectType = model.ProjectType;
                            omadsModel.TableName = "";
                            omadsModel.username = rpaModel.username;
                            omadsModel.WriteMark = 0;
                            omadsModel.WriteResult = 0;
                            omadsModel.FollowOperate = "";
                            omadsModel.FileNumber = files.Data.Count.ToString();
                            omadsModel.status1 = 0;
                            //SetCreateUser(omadsModel);
                            //SetCurrentUser(omadsModel);

                            omadsModel.OperateUserId = model.CreateUserId;
                            omadsModel.OperateUserName = model.CreateUserName;
                            omadsModel.OperateTime = DateTime.Now;
                            omadsModel.CreateUserId = model.CreateUserId;
                            omadsModel.CreateUserName = model.CreateUserName;
                            omadsModel.OperateTime = DateTime.Now;
                            #endregion
                            #region tempModel
                            tempModel.ApplyTime = model.ApplyTime;
                            tempModel.Auditor = "";
                            tempModel.CompanyName = model.CompanyName;
                            tempModel.companys = CurrentCompanyName;
                            tempModel.DataId = null;
                            tempModel.Declarer = model.DeclarerUser;
                            for (int i = 0; i < files.Data.Count; i++)
                            {
                                tempModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                            }
                            tempModel.NatureName = model.NatureName;
                            tempModel.Position = model.Position;
                            tempModel.PredictDayGas = model.LNG + model.CNG;
                            tempModel.PredictDayOil = model.OilSalesTotal;
                            tempModel.PredictMoney = model.PredictMoney;
                            tempModel.ProjectAddress = model.ProjectAddress;
                            tempModel.ProjectCode = model.ProjectCode;
                            tempModel.ProjectName = model.ProjectName;
                            tempModel.ProvincesName = model.ProvinceName;
                            tempModel.Recommender = model.Recommender;
                            tempModel.RecommenderCompany = model.RecommenderDept;
                            tempModel.RecommenderJob = model.RecommenderJob;
                            tempModel.Remark = model.Remark;
                            tempModel.StationCode = model.StationCode;
                            tempModel.StationName = model.StationName;
                            tempModel.StationType = model.StationTypeName;
                            tempModel.ProjectType = model.ProjectType;
                            tempModel.TableName = "";
                            tempModel.username = rpaModel.username;
                            tempModel.WriteMark = 0;
                            tempModel.WriteResult = 0;
                            tempModel.FollowOperate = "";
                            tempModel.FileNumber = files.Data.Count.ToString();
                            tempModel.status1 = 0;
                            //SetCreateUser(tempModel);
                            //SetCurrentUser(tempModel);
                            tempModel.OperateUserId = model.CreateUserId;
                            tempModel.OperateUserName = model.CreateUserName;
                            tempModel.OperateTime = DateTime.Now;
                            tempModel.CreateUserId = model.CreateUserId;
                            tempModel.CreateUserName = model.CreateUserName;
                            tempModel.OperateTime = DateTime.Now;
                            #endregion
                            DataOperateBusiness<RPA_ProjectProposal>.Get().Add(rpaModel);
                            DataOperateBusiness<OMADS_ProjectProposal>.Get().Add(omadsModel);
                            DataOperateBusiness<TEMP_ProjectProposal>.Get().Add(tempModel);
                            //transaction.Commit();
                            #endregion
                        }
                    }
                    else
                    {
                        throw new Exception("该项目信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectProposalState");
            }
            return result;
        }

        /// <summary>
        /// 获取项目进度信息列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public Result<List<TzProjectScheduleView>> GetTzProjectScheduleList(QueryCondition qc)
        {
            Result<List<TzProjectScheduleView>> result = new Result<List<TzProjectScheduleView>>();
            try
            {
                //加油站项目信息表
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetList(p => (p.State == (int)PreProjectState.ApprovalSuccess));

                if (qc.ConditionList != null && qc.ConditionList.Any())
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
                                        tzProjectProposal = tzProjectProposal.Where(p => p.ProjectName.Contains(value));
                                        break;
                                    }
                                case "Nature":
                                    {
                                        tzProjectProposal = tzProjectProposal.Where(p => p.Nature == value);
                                        break;
                                    }
                                case "StationName":
                                    {
                                        tzProjectProposal = tzProjectProposal.Where(p => p.StationName.Contains(value));
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;
                int total = tzProjectProposal.Count();
                var projectList = tzProjectProposal.OrderByDescending(p => p.ApplyTime).Skip(skip).Take(take).ToList();

                if (projectList.Count() > 0)
                {
                    List<TzProjectScheduleView> list = new List<TzProjectScheduleView>();
                    foreach (var item in projectList)
                    {
                        TzProjectScheduleView view = new TzProjectScheduleView();
                        if (item != null)
                        {
                            view.TzProjectProposal = true;
                            view.Nature = item.Nature;
                            view.ProjectName = item.ProjectName;

                            var projectId = item.Id;

                            //现场工程方面调研
                            var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                            if (tzResearchOfEngineering != null)
                            {
                                view.TzResearchOfEngineering = true;
                            }

                            //初次谈判
                            var tzFirstNegotiation = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                            if (tzFirstNegotiation != null)
                            {
                                view.TzFirstNegotiation = true;
                            }

                            //土地谈判协议
                            var tzLandNegotiation = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                            if (tzLandNegotiation != null)
                            {
                                view.TzLandNegotiation = true;
                            }

                            //评审材料上报
                            var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzFormTalkFile != null)
                            {
                                view.TzFormTalkFile = true;
                            }

                            //项目评审
                            var tzProjectReveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzProjectReveiews != null)
                            {
                                view.TzProjectReveiews = true;
                            }

                            //会议决策
                            var meetingFileReport = DataOperateBusiness<Epm_MeetingFileReport>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (meetingFileReport != null)
                            {
                                view.MeetingFileReport = true;
                            }

                            //项目批复请示
                            var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzProjectApprovalInfo != null)
                            {
                                view.TzProjectApprovalInfo = true;
                            }

                            //设计方案
                            var tzDesignScheme = DataOperateBusiness<Epm_TzDesignScheme>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzDesignScheme != null)
                            {
                                view.TzDesignScheme = true;
                            }

                            //图纸会审
                            var tzConDrawing = DataOperateBusiness<Epm_TzConDrawing>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzConDrawing != null)
                            {
                                view.TzConDrawing = true;
                            }

                            //招标管理
                            var tzBidResult = DataOperateBusiness<Epm_TzBidResult>.Get().Single(p => p.ProjectId == projectId.ToString() && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzBidResult != null)
                            {
                                view.TzBidResult = true;
                            }

                            //物资申请
                            var tzSupplyMaterialApply = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzSupplyMaterialApply != null)
                            {
                                view.TzSupplyMaterialApply = true;
                            }

                            //开工申请
                            var tzProjectStartApply = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzProjectStartApply != null)
                            {
                                view.TzProjectStartApply = true;
                            }

                            //施工管理
                            view.TzConstruceManage = false;

                            //竣工管理
                            var completionAcceptanceResUpload = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (completionAcceptanceResUpload != null)
                            {
                                view.TzCompletedManage = true;
                            }

                            //试运行
                            var tzProjectPolit = DataOperateBusiness<Epm_TzProjectPolit>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                            if (tzProjectPolit != null)
                            {
                                view.TzProjectPolit = true;
                            }
                            list.Add(view);
                        }
                    }

                    result.Data = list;
                    result.AllRowsCount = total;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectScheduleList");
            }
            return result;
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> CloseTzProjectProposal(long projectId)
        {
            Result<int> result = new Result<int>();
            try
            {
                //投资项目信息
                var model = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (model != null)
                {
                    SetCurrentUser(model);
                    model.State = (int)PreProjectApprovalState.Closed;
                    var rows = DataOperateBusiness<Epm_TzProjectProposal>.Get().Update(model);
                    //现场踏勘
                    //工程发面调研
                    var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfEngineering != null)
                    {
                        tzResearchOfEngineering.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Update(tzResearchOfEngineering);
                    }
                    //信息方面调研
                    var tzResearchOfInformation = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfInformation != null)
                    {
                        tzResearchOfInformation.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().Update(tzResearchOfInformation);
                    }
                    //投资方面调研
                    var tzResearchOfInvestment = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfInvestment != null)
                    {
                        tzResearchOfInvestment.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().Update(tzResearchOfInvestment);
                    }
                    //法律方面调研
                    var tzResearchOfLaw = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfLaw != null)
                    {
                        tzResearchOfLaw.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().Update(tzResearchOfLaw);

                    }
                    //经营方面调研
                    var tzResearchOfManagement = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfManagement != null)
                    {
                        tzResearchOfManagement.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().Update(tzResearchOfManagement);
                    }
                    //安全方面调研
                    var tzResearchOfSecurity = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzResearchOfSecurity != null)
                    {
                        tzResearchOfSecurity.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().Update(tzResearchOfSecurity);
                    }

                    //项目谈判
                    var first = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (first != null)
                    {
                        first.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Update(first);
                    }

                    //土地谈判协议
                    var land = DataOperateBusiness<Epm_TzLandNegotiation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (land != null)
                    {
                        land.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Update(land);
                    }

                    //评审材料上报
                    var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (tzFormTalkFile != null)
                    {
                        tzFormTalkFile.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(tzFormTalkFile);
                    }

                    //项目评审记录
                    var reveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (reveiews != null)
                    {
                        reveiews.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Update(reveiews);
                    }

                    // 上会材料上报（会议决策）
                    var meeting = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (meeting != null)
                    {
                        meeting.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().Update(meeting);
                    }

                    //项目批复
                    var approvalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                    if (approvalInfo != null)
                    {
                        approvalInfo.State = (int)PreProjectApprovalState.Closed;
                        rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(approvalInfo);
                    }

                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    throw new Exception("该项目信息不存在");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CloseTzProjectProposal");
            }
            return result;
        }
    }
}
