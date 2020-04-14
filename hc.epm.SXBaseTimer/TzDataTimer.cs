/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.SXBaseTimer
 * 文件名：  TzDataTimer
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/31 14:54:57
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.SXBaseTimer
{
    /// <summary>
    /// 投资系统防渗改造项目同步
    /// </summary>
    public partial class SXBaseTimer
    {
        private BusinessDataContext businessContext = new BusinessDataContext();

        #region 防渗改造

        /// <summary>
        /// 防渗改造项目同步
        /// </summary>
        public void SysReformRecord()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_ReformRecord>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_ReformRecord>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_ReformRecord();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;
                        }

                        data.ProjectName = item.ProjectName;
                        if (!string.IsNullOrWhiteSpace(item.ProjectId))
                        {
                            data.ProjectId = Convert.ToInt64(item.ProjectId);
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeWhole = item.ProjectCodeWhole;
                            data.ProjectCodeProject = item.ProjectCodeProject;

                        }
                        data.RemarkType = item.RemarkType;
                        data.StationName = item.StationName;
                        if (!string.IsNullOrWhiteSpace(item.StationName))
                        {
                            var oilStation = GetOilStation(item.StationName);
                            if(oilStation != null)
                            {
                                data.StationId = oilStation.Id;
                            }
                        }
                        data.StationCodeInvest = item.StationCodeInvest;
                        data.StationCodeWhole = item.StationCodeWhole;
                        data.StationCodeProject = item.StationCodeProject;

                        data.CompanyName = item.CompanyName;
                        data.CompanyId = GetCompanyIdByName(item.CompanyName);
                        data.LimitType = item.LimitType;
                        data.SourceFund = item.SourceFund;
                        data.Investment = item.Investment.ToDecimal();

                        data.ItemNumber = item.ItemNumber;
                        data.ItemTime = item.ItemTime.ToDateTime();
                        data.ItemUnit = item.ItemUnit;
                        data.ItemMoney = item.ItemMoney.ToDecimal();
                        data.InitialNumber = item.InitialNumber;

                        data.InitialMoney = item.InitialMoney.ToDecimal();
                        data.RemarkStartTime = item.RemarkStartTime.ToDateTime();
                        data.RemarkEndTime = item.RemarkEndTime.ToDateTime();
                        data.RemarkMoney = item.RemarkMoney.ToDecimal();
                        data.DecisionMaker = item.DecisionMaker;

                        data.LeaderPerson = item.LeaderPerson;
                        data.Operator = item.Operator;
                        data.PipetteCoding = item.PipetteCoding;
                        data.InitialSalesOfRefinedOil = item.InitialSalesOfRefinedOil.ToDecimal();
                        data.RemarkSalesOfRefinedOil = item.RemarkSalesOfRefinedOil.ToDecimal();

                        data.InitialSalesOfGas = item.InitialSalesOfGas.ToDecimal();
                        data.RemarkSalesOfGas = item.RemarkSalesOfGas.ToDecimal();
                        data.IRR = item.IRR.ToDecimal();
                        data.ProjectContent = item.ProjectContent;
                        data.Remark = item.Remark;

                        data.AnnexType = item.AnnexType;
                        data.AnnexAddress = item.AnnexAddress;
                        data.State = item.State;
                        data.Approver = item.Approver;
                        data.ApproverId = GetUserIdByName(item.Approver);

                        data.ApproveTime = item.ApproveTime;
                        if (!string.IsNullOrWhiteSpace(item.ApproveResult))
                        {
                            //data.ApproveResult = item.ApproveResult;
                            // todo: 根据实际的值确定该字段的值；
                        }
                        data.AreaCompanyName = string.IsNullOrWhiteSpace(item.AreaCompanyName) ? "陕西公司" : item.AreaCompanyName;
                        data.AreaCompanyCode = string.IsNullOrWhiteSpace(item.AreaCompanyCode) ? "10" : item.AreaCompanyCode;
                        data.ApproveRemark = item.ApproveRemark;
                        data.CreateTime = item.CreateTime;

                        data.CreateUserName = item.CreateUserName;
                        data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;
                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_ReformRecord>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_ReformRecord>.Get().Update(data);
                        }

                        var attachList = GetAttachsByProName(item.ProjectName);
                        if (attachList.Any())
                        {
                            string tableName = data.GetType().Name;
                            attachList.ForEach(p =>
                            {
                                p.DataId = data.Id;
                                p.TableName = tableName;

                                p.IsDelete = false;

                                p.CreateTime = DateTime.Now;
                                p.CreateUserId = data.CreateUserId;
                                p.CreateUserName = data.CreateUserName;

                                p.OperateTime = DateTime.Now;
                                p.OperateUserId = data.CreateUserId;
                                p.OperateUserName = data.CreateUserName;
                            });

                            var oldAttachs = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(p => p.DataId == data.Id && p.Name == tableName).ToList();
                            if (oldAttachs.Any())
                            {
                                DataOperateBusiness<Epm_TzAttachs>.Get().DeleteRange(oldAttachs);
                            }
                            DataOperateBusiness<Epm_TzAttachs>.Get().AddRange(attachList);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }

        #endregion

        #region 加油站项目提出前

        /// <summary>
        /// 加油站项目提出前相关信息同步
        /// </summary>
        public void SysProjectProposal()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzProjectProposal>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzProjectProposal();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }

                        data.ProjectName = item.ProjectName;
                        if (!string.IsNullOrWhiteSpace(item.ProjectId))
                        {
                            //data.ProjectId = Convert.ToInt64(item.ProjectId);
                            data.ProjectCode = item.ProjectCodeInvest;
                            data.ProjectCodeXt = item.ProjectCodeInvest;
                            //data.ProjectCodeWhole = item.ProjectCodeWhole;
                            //data.ProjectCodeProject = item.ProjectCodeProject;
                        }

                        data.Nature = item.ProjectNatureType;
                        data.NatureName = item.ProjectNatureTypeName;
                        data.StationName = item.StationName;
                        if (!string.IsNullOrWhiteSpace(item.StationName))
                        {
                            var oilStation = GetOilStation(item.StationName);
                            if (oilStation != null)
                            {
                                data.StationId = oilStation.Id;
                            }
                        }
                        data.StationCode = item.StationCodeInvest;
                        data.StationCodeXt = item.StationCodeWhole;

                        data.StationCodeJg = item.StationCodeProject;
                        data.ApplyTime = item.ApplyTime.ToDateTime();
                        data.CompanyName = item.CompanyName;
                        data.CompanyId = GetCompanyIdByName(item.CompanyName);

                        data.CompanyCodeXt = item.CompanyCodeInvest;
                        //data.CompanyCodeWhole = item.CompanyCodeWhole;
                        //data.CompanyCodeProject = item.CompanyCodeProject;
                        data.ProvinceCode = item.ProvinceCode;
                        data.ProvinceName = item.ProvinceName;

                        data.ProvinceCodeXt = item.ProvinceCodeInvest;
                        //data.ProvinceCodeWhole = item.ProvinceCodeWhole;
                        //data.ProvinceCodeProject = item.ProvinceCodeProject;
                        data.Recommender = item.Recommender;
                        data.RecommenderJob = item.RecommenderJob;

                        data.RecommenderDept = item.RecommenderCompany;
                        data.DeclarerUser = item.Declarer;
                        data.Position = item.Position;
                        data.PositionType = item.PositionType;
                        data.Position = item.ProjectPosition;

                        data.StationType = item.StationType;
                        data.StationTypeName = item.StationTypeCode;
                        data.PredictMoney = item.PredictMoney.ToDecimal();
                        data.CNG = item.PredictDayGas.ToDecimal();
                        data.LNG = item.PredictDayOil.ToDecimal();

                        data.Remark = item.Remark;
                        data.State = item.State.Value;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzProjectProposal>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzProjectProposal>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 组织评审材料

        /// <summary>
        /// 组织评审材料相关信息同步
        /// </summary>
        //public void SysTzFormTalkFile()
        //{
        //    try
        //    {
        //        QueryCondition qc = new QueryCondition()
        //        {
        //            PageInfo = new PageListInfo()
        //            {
        //                isAllowPage = false
        //            }
        //        };

        //        var result = DataOperate.QueryListSimple<Temp_TzFormTalkFile>(businessContext, qc);
        //        if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
        //        {
        //            foreach (var item in result.Data)
        //            {
        //                bool isAdd = false;
        //                var data = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Single(p => p.ProjectName == item.ProjectName);
        //                if (data == null)
        //                {
        //                    isAdd = true;
        //                    data = new Epm_TzFormTalkFile();
        //                    data.ProjectName = item.ProjectName;
        //                    data.CreateTime = item.CreateTime;
        //                    data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
        //                    data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

        //                }
        //                data.ProjectName = item.ProjectName;
        //                if (!string.IsNullOrWhiteSpace(item.ProjectId))
        //                {
        //                    data.ProjectId = Convert.ToInt64(item.ProjectId);
        //                    data.ProjectCodeInvest = item.ProjectCodeInvest;
        //                    data.ProjectCodeInvest = item.ProjectCodeInvest;
        //                    data.ProjectCodeWhole = item.ProjectCodeWhole;
        //                    data.ProjectCodeProject = item.ProjectCodeProject;
        //                }

        //                data.Id = item.Id;
        //                data.ProjectId = Convert.ToInt64(item.ProjectId);
        //                data.ProjectName = item.ProjectName;
        //                data.ProjectCodeInvest = item.ProjectCodeInvest;
        //                data.ProjectCodeWhole = item.ProjectCodeWhole;
        //                data.ProjectCodeProject = item.ProjectCodeProject;
        //                data.Header = item.Header;
        //                data.Marker = item.Marker;
        //                data.Writer = item.Writer;
        //                data.ApplyPerson = item.ApplyPerson;
        //                data.ApplyPersonID = Convert.ToInt64(item.ApplyPersonID);
        //                data.ApplyPersonInvest = item.ApplyPersonInvest;
        //                data.ApplyPersonWhole = item.ApplyPersonWhole;
        //                data.ApplyPersonProject = item.ApplyPersonProject;
        //                data.ApplytTime = Convert.ToDateTime(item.ApplytTime);
        //                data.OperationTypeType = item.OperationTypeType;
        //                data.OperationTypeName = item.OperationTypeName;
        //                data.Remark = item.Remark;
        //                data.StateType = item.StateType;
        //                data.StateName = item.StateName;
        //                data.State = item.State;

        //                data.Remark = item.Remark;
        //                data.State = item.State;
        //                data.OperateTime = DateTime.Now;
        //                data.OperateUserId = 0;
        //                data.OperateUserName = "admin";

        //                if (isAdd)
        //                {
        //                    DataOperateBusiness<Epm_TzFormTalkFile>.Get().Add(data);
        //                }
        //                else
        //                {
        //                    DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(data);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Equals(this, ex.Message);
        //    }
        //}


        #endregion

        #region 初次谈判

        /// <summary>
        /// 初次谈判相关信息同步
        /// </summary>
        public void SysTzInitialTalk()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzInitialTalk>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzInitialTalk>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzInitialTalk();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;
                        if (!string.IsNullOrWhiteSpace(item.ProjectId))
                        {
                            data.ProjectId = Convert.ToInt64(item.ProjectId);
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeWhole = item.ProjectCodeWhole;
                            data.ProjectCodeProject = item.ProjectCodeProject;
                        }

                        data.Id = item.Id;
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.TalkTime = Convert.ToDateTime(item.TalkTime);
                        data.TalkAdress = item.TalkAdress;
                        data.Fees = Convert.ToDecimal(item.Fees);
                        data.FeesTime = Convert.ToDateTime(item.FeesTime);
                        data.OurSide = item.OurSide;
                        data.OtherSide = item.OtherSide;
                        data.TalkResultType = item.TalkResultType;
                        data.TalkResultName = item.TalkResultName;
                        data.Remark = item.Remark;
                        data.State = item.State;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzInitialTalk>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzInitialTalk>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 土地协议出让谈判信息

        /// <summary>
        /// 土地协议出让谈判信息同步
        /// </summary>
        public void SysTzLandTalk()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzLandTalk>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzLandTalk>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzLandTalk();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;
                        if (!string.IsNullOrWhiteSpace(item.ProjectId))
                        {
                            data.ProjectId = Convert.ToInt64(item.ProjectId);
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeWhole = item.ProjectCodeWhole;
                            data.ProjectCodeProject = item.ProjectCodeProject;
                        }

                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.TalkTime = Convert.ToDateTime(item.TalkTime);
                        data.TalkAdress = item.TalkAdress;
                        data.FirstPay = Convert.ToDateTime(item.FirstPay);
                        data.FirstPayTime = item.FirstPayTime;
                        data.Fees = Convert.ToDecimal(item.Fees);
                        data.FeesTime = Convert.ToDateTime(item.FeesTime);
                        data.TalkResultType = item.TalkResultType;
                        data.TalkResultName = item.TalkResultName;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.Remark = item.Remark;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;
                        data.Operator = item.Operator;
                        data.OperatorID = item.OperatorID;
                        data.OperatorInvest = item.OperatorInvest;
                        data.OperatorWhole = item.OperatorWhole;
                        data.OperatorProject = item.OperatorProject;
                        data.OperatorTime = Convert.ToDateTime(item.OperatorTime);
                        data.State = item.State;

                        //data.OurNegotiator = item.OurNegotiator;
                        //data.Negotiator = item.Negotiator;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzLandTalk>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzLandTalk>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 项目批复请示

        /// <summary>
        /// 项目批复请示相关信息同步
        /// </summary>
        public void SysTzProjectApproval()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzProjectApproval>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzProjectApproval>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzProjectApproval();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;
                        if (!string.IsNullOrWhiteSpace(item.ProjectId))
                        {
                            data.ProjectId = Convert.ToInt64(item.ProjectId);
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeInvest = item.ProjectCodeInvest;
                            data.ProjectCodeWhole = item.ProjectCodeWhole;
                            data.ProjectCodeProject = item.ProjectCodeProject;
                        }

                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.ApplyId = item.ApplyId;
                        data.ApplyName = item.ApplyName;
                        data.ApplyInvest = item.ApplyInvest;
                        data.ApplyWhole = item.ApplyWhole;
                        data.ApplyProject = item.ApplyProject;
                        data.ApplyTime = Convert.ToDateTime(item.ApplyTime);
                        data.LimitTypeType = item.LimitTypeType;
                        data.LimitTypeName = item.LimitTypeName;
                        data.Drafter = item.Drafter;
                        data.SignPeople = item.SignPeople;
                        data.Signer = item.Signer;
                        data.Titanic = item.Titanic;
                        data.Remark = item.Remark;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzProjectApproval>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzProjectApproval>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 加油站试运行申请

        /// <summary>
        /// 加油站试运行申请相关信息同步
        /// </summary>
        public void SysTzProjectPolit()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzProjectPolit>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzProjectPolit>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzProjectPolit();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;

                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.CompanyId = Convert.ToInt64(item.CompanyId);
                        data.CompanyName = item.CompanyName;
                        data.StartDate = Convert.ToDateTime(item.StartDate);
                        data.EndDate = Convert.ToDateTime(item.EndDate);
                        data.AcceptDate = Convert.ToDateTime(item.AcceptDate);
                        data.RectFinishDate = Convert.ToDateTime(item.RectFinishDate);
                        data.FinalDate = Convert.ToDateTime(item.FinalDate);
                        data.AuditDate = Convert.ToDateTime(item.AuditDate);
                        data.FullFiles = item.FullFiles;
                        data.AcceptOpinion = item.AcceptOpinion;
                        data.ProjectName = item.ProjectName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzProjectPolit>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 项目提出审核

        /// <summary>
        /// 项目提出审核相关信息同步
        /// </summary>
        public void SysTzProSubmissionApprova()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzProSubmissionApprova>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzProSubmissionApprova>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzProSubmissionApprova();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;

                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.Approvar = item.Approvar;
                        data.ApprovarTime = Convert.ToDateTime(item.ApprovarTime);
                        data.OthersType = item.OthersType;
                        data.OthersName = item.OthersName;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzProSubmissionApprova>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzProSubmissionApprova>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 二次、三次、四次。。。谈判

        /// <summary>
        /// 二次、三次、四次。。。谈判相关信息同步
        /// </summary>
        public void SysTzSecondTakl()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzSecondTakl>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzSecondTakl>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzSecondTakl();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectName = item.ProjectName;

                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.TalkTime = Convert.ToDateTime(item.TalkTime);
                        data.TalkAdress = item.TalkAdress;
                        data.OurNegotiator = item.OurNegotiator;
                        data.Negotiator = item.Negotiator;
                        data.TalkResult = item.TalkResult;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzSecondTakl>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzSecondTakl>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 二次、三次、四次。。。谈判审核

        /// <summary>
        /// 二次、三次、四次。。。谈判审核相关信息同步
        /// </summary>
        public void SysTzSecondTalkAudit()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzSecondTalkAudit>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzSecondTalkAudit();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.Approvar = item.Approvar;
                        data.ApprovarId = item.ApprovarId;
                        data.ApprovarInvest = item.ApprovarInvest;
                        data.ApprovarWhole = item.ApprovarWhole;
                        data.ApprovarProject = item.ApprovarProject;
                        data.SecondWillType = item.SecondWillType;
                        data.SecondWillName = item.SecondWillName;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.State_Type = item.State_Type;
                        data.State_Name = item.State_Name;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzSecondTalkAudit>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 现场调研

        /// <summary>
        /// 现场调研相关信息同步
        /// </summary>
        public void SysTzSiteSurvey()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzSiteSurvey>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzSiteSurvey>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzSiteSurvey();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.InspectStTime = Convert.ToDateTime(item.InspectStTime);
                        data.IncestPerson = item.IncestPerson;
                        data.IncestPersonId = Convert.ToInt64(item.IncestPersonId);
                        data.IncestJobType = item.IncestJobType;
                        data.IncestJobName = item.IncestJobName;
                        data.Location = item.Location;
                        data.EnvironmentType = item.EnvironmentType;
                        data.EnvironmentName = item.EnvironmentName;
                        data.NatureType = item.NatureType;
                        data.NatureName = item.NatureName;
                        data.UseType = item.UseType;
                        data.UseName = item.UseName;
                        data.Area = Convert.ToDecimal(item.Area);
                        data.Form = Convert.ToDecimal(item.Form);
                        data.IsWithType = item.IsWithType;
                        data.IsWithName = item.IsWithName;
                        data.ExpectAmount = Convert.ToInt32(item.ExpectAmount);
                        data.TrafficFlow = Convert.ToInt32(item.TrafficFlow);
                        data.OilGuess = Convert.ToDecimal(item.OilGuess);
                        data.DieselGR = Convert.ToDecimal(item.DieselGR);
                        data.Price = Convert.ToDecimal(item.Price);
                        data.GasGuess = Convert.ToDecimal(item.GasGuess);
                        data.LawPerson = item.LawPerson;
                        data.LawPersonId = Convert.ToInt64(item.LawPersonId);
                        data.LawJobType = item.LawJobType;
                        data.LawJobName = item.LawJobName;
                        data.PropertyIsType = item.PropertyIsType;
                        data.PropertyIsName = item.PropertyIsName;
                        data.QualificationIsType = item.QualificationIsType;
                        data.QualificationIs = item.QualificationIs;
                        data.DisputesIsType = item.DisputesIsType;
                        data.DisputesIsName = item.DisputesIsName;
                        data.LicensesTypeType = item.LicensesTypeType;
                        data.LicensesTypeName = item.LicensesTypeName;
                        data.EngineeringPerson = item.EngineeringPerson;
                        data.EngineeringPersonId = Convert.ToInt64(item.EngineeringPersonId);
                        data.EngineeringJobType = item.EngineeringJobType;
                        data.EngineeringJobName = item.EngineeringJobName;
                        data.ImageIsType = item.ImageIsType;
                        data.ImageIsName = item.ImageIsName;
                        data.Tent = Convert.ToDecimal(item.Tent);
                        data.OilT = Convert.ToInt32(item.OilT);
                        data.OilQ = Convert.ToInt32(item.OilQ);
                        data.GasT = Convert.ToInt32(item.GasT);
                        data.GasQ = Convert.ToInt32(item.GasQ);
                        data.Tank = Convert.ToDecimal(item.Tank);
                        data.Station = Convert.ToDecimal(item.Station);
                        data.GasStorageWell = Convert.ToDecimal(item.GasStorageWell);
                        data.InforSystemType = item.InforSystemType;
                        data.InforSystemName = item.InforSystemName;
                        data.TranSType = item.TranSType;
                        data.TranSName = item.TranSName;
                        data.BusinessPerson = item.BusinessPerson;
                        data.BusinessPersonId = Convert.ToInt64(item.BusinessPersonId);
                        data.BusinessJobType = item.BusinessJobType;
                        data.BusinessJobName = item.BusinessJobName;
                        data.SalesIsType = item.SalesIsType;
                        data.SalesIsName = item.SalesIsName;
                        data.Distance = Convert.ToDecimal(item.Distance);
                        data.Source = item.Source;
                        data.Oilsales = Convert.ToDecimal(item.Oilsales);
                        data.MethodsIsType = item.MethodsIsType;
                        data.MethodsIsName = item.MethodsIsName;
                        data.GasSales = Convert.ToDecimal(item.GasSales);
                        data.SecurityPerson = item.SecurityPerson;
                        data.SecurityPersonId = Convert.ToInt64(item.SecurityPersonId);
                        data.SecurityJobType = item.SecurityJobType;
                        data.SecurityJobName = item.SecurityJobName;
                        data.ProtectionIsType = item.ProtectionIsType;
                        data.ProtectionIsName = item.ProtectionIsName;
                        data.HiddenIsType = item.HiddenIsType;
                        data.HiddenIsName = item.HiddenIsName;
                        data.MeasuresIs = item.MeasuresIs;
                        data.InforPerson = item.InforPerson;
                        data.InforPersonID = Convert.ToInt64(item.InforPersonID);
                        data.InforJobType = item.InforJobType;
                        data.InforJobName = item.InforJobName;
                        data.Plans = item.Plans;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzSiteSurvey>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzSiteSurvey>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 评审材料管理员审核

        /// <summary>
        /// 评审材料管理员审核相关信息同步
        /// </summary>
        public void SysTTzTalkFileAudit()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkFileAudit>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkFileAudit();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.Decider = item.Decider;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkFileAudit>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkFileAudit>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 评审材料审核

        /// <summary>
        /// 评审材料审核相关信息同步
        /// </summary>
        public void SysTzTalkFileHeadAudit()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkFileHeadAudit>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkFileHeadAudit();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 领导签发

        /// <summary>
        /// 领导签发相关信息同步
        /// </summary>
        public void SysTzTalkLeaderSign()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkLeaderSign>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkLeaderSign();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 评审会记录

        /// <summary>
        /// 评审会记录相关信息同步
        /// </summary>
        public void SysTzTalkRecord()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkRecord>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkRecord>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkRecord();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.TalkTime = Convert.ToDateTime(item.TalkTime);
                        data.TalkAdress = item.TalkAdress;
                        data.Compere = item.Compere;
                        data.Expert = item.Expert;
                        data.Attender = item.Attender;
                        data.ConclusionType = item.ConclusionType;
                        data.ConclusionName = item.ConclusionName;
                        data.Content = item.Content;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkRecord>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkRecord>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 评审会投资部门确认

        /// <summary>
        /// 评审会投资部门确认相关信息同步
        /// </summary>
        public void SysTzTalkRecordConfirm()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkRecordConfirm>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkRecordConfirm();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.Reviewer = item.Reviewer;
                        data.OthersType = item.OthersType;
                        data.OthersName = item.OthersName;
                        data.Idea = item.Idea;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 评审会签

        /// <summary>
        /// 评审会签相关信息同步
        /// </summary>
        public void SysTzTalkSign()
        {
            try
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };

                var result = DataOperate.QueryListSimple<Temp_TzTalkSign>(businessContext, qc);
                if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                {
                    foreach (var item in result.Data)
                    {
                        bool isAdd = false;
                        var data = DataOperateBusiness<Epm_TzTalkSign>.Get().Single(p => p.ProjectName == item.ProjectName);
                        if (data == null)
                        {
                            isAdd = true;
                            data = new Epm_TzTalkSign();
                            data.ProjectName = item.ProjectName;
                            data.CreateTime = item.CreateTime;
                            data.CreateUserName = string.IsNullOrWhiteSpace(item.CreateUserName) ? "admin" : item.CreateUserName;
                            data.CreateUserId = GetUserIdByName(item.CreateUserName) ?? 0;

                        }
                        data.DataId = Convert.ToInt64(item.DataId);
                        data.ProjectId = Convert.ToInt64(item.ProjectId);
                        data.ProjectName = item.ProjectName;
                        data.ProjectCodeInvest = item.ProjectCodeInvest;
                        data.ProjectCodeWhole = item.ProjectCodeWhole;
                        data.ProjectCodeProject = item.ProjectCodeProject;
                        data.SignId = item.SignId;
                        data.SignName = item.SignName;
                        data.SignDepartmentId = item.SignDepartmentId;
                        data.SignDepartName = item.SignDepartName;
                        data.OthersType = item.OthersType;
                        data.OthersName = item.OthersName;
                        data.Opinion = item.Opinion;
                        data.Remark = item.Remark;
                        data.OperationTypeType = item.OperationTypeType;
                        data.OperationTypeName = item.OperationTypeName;
                        data.StateType = item.StateType;
                        data.StateName = item.StateName;

                        data.Remark = item.Remark;
                        data.State = item.State;
                        data.OperateTime = DateTime.Now;
                        data.OperateUserId = 0;
                        data.OperateUserName = "admin";

                        if (isAdd)
                        {
                            DataOperateBusiness<Epm_TzTalkSign>.Get().Add(data);
                        }
                        else
                        {
                            DataOperateBusiness<Epm_TzTalkSign>.Get().Update(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Equals(this, ex.Message);
            }
        }


        #endregion

        #region 私有方法

        /// <summary>
        /// 根据人员信息名称获取人员 ID
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private long? GetUserIdByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            var user = DataOperateBasic<Base_User>.Get().Single(p => p.UserName == userName);
            if (user == null)
            {
                return null;
            }
            return user.Id;
        }

        /// <summary>
        /// 根据分公司名称获取分公司 ID
        /// </summary>
        /// <param name="companyName">分公司名称</param>
        /// <returns></returns>
        private long? GetCompanyIdByName(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return null;
            companyName = companyName.Replace("分公司", "");
            var company = DataOperateBasic<Base_Company>.Get().Single(p => p.Name.StartsWith(companyName) && p.PId == 10);
            if (company == null)
            {
                return null;
            }
            return company.Id;
        }

        private Base_Company GetOilStation(string oilStationName)
        {
            if (string.IsNullOrWhiteSpace(oilStationName))
                return null;
            var oilStation = DataOperateBasic<Base_Company>.Get().Single(p => p.Name == oilStationName && p.OrgType == "4");
            return oilStation;
        }

        /// <summary>
        /// 根据项目名称获取临时表中的相关附件
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        private List<Epm_TzAttachs> GetAttachsByProName(string projectName)
        {
            List<Epm_TzAttachs> list = new List<Epm_TzAttachs>();
            QueryCondition qc = new QueryCondition()
            {
                PageInfo = new PageListInfo()
                {
                    isAllowPage = false
                }
            };
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "TableName",
                ExpValue = projectName,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });

            var result = DataOperate.QueryListSimple<Temp_TzAttachs>(businessContext, qc);
            if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
            {
                result.Data.ForEach(p =>
                {
                    list.Add(new Epm_TzAttachs()
                    {
                        FilePath = p.FilePath,
                        Name = p.Name
                    });
                });
            }

            return list;
        }

        #endregion
    }
}
