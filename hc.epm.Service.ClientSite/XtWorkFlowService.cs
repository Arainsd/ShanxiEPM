/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  XtWorkFlowService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/28 9:48:50
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.Common;
using hc.epm.Service.ClientSite.XtWorkFlow;
using hc.epm.ViewModel;
using System;
using System.Linq;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 协同流程审批创建相关服务
    /// </summary>
    public class XtWorkFlowService
    {
        /// <summary>
        /// 发起流程申请
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CreateApplyWorkFlow(ProjectApprovalApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfSyxsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmc",
                            fieldValue = model.txt_xmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sqdw",
                            fieldValue = model.sub_sqdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_kgrq",
                            fieldValue = model.date_kgrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_jgrq",
                            fieldValue = model.date_jgrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_ysrq",
                            fieldValue = model.date_ysrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_zgrq",
                            fieldValue = model.date_zgrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_jssj",
                            fieldValue = model.date_jssj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sjsj",
                            fieldValue = model.date_sjsj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_zlsfqq",
                            fieldValue = model.txts_zlsfqq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_ysyj",
                            fieldValue = model.txt_ysyj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sqrq",
                            fieldValue = model.date_sqrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sqr",
                            fieldValue = model.hr_sqr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_fj",
                            fieldType = "http:",
                            fieldValue = model.file_fj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_sgdw",
                            fieldType = "http:",
                            fieldValue = model.file_sgdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_gcjjzs ",
                            fieldType = "http:",
                            fieldValue = model.file_gcjjzs
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.txt_xmmc + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 开工报告流程申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateStartsApplyWorkFlow(TzStartsApplyApprovalView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfjsxmkgbg).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sqr",
                            fieldValue = model.hr_sqr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sqdw",
                            fieldValue = model.sub_sqdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dept_sqbm",
                            fieldValue = model.dept_sqbm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sqrq",
                            fieldValue = model.date_sqrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_lxdh",
                            fieldValue = model.txt_lxdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jsxmmc",
                            fieldValue = model.txt_jsxmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sjgm",
                            fieldValue = model.txt_sjgm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmjyswh",
                            fieldValue = model.txt_xmjyswh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "float_gstz_js",
                            fieldValue = model.float_gstz_js
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_kybgwh",
                            fieldValue = model.txt_kybgwh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "float_gstz_ky",
                            fieldValue = model.float_gstz_ky
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_cbsjwh",
                            fieldValue = model.txt_cbsjwh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "float_cbsjtz",
                            fieldValue = model.float_cbsjtz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_zjly",
                            fieldValue = model.select_zjly
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jhtzqk",
                            fieldValue = model.txt_jhtzqk
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_jsgq_ks",
                            fieldValue = model.date_jsgq_ks
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_jsgq_js",
                            fieldValue = model.date_jsgq_js
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_gknr",
                            fieldValue = model.txts_gknr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_xmgljg",
                            fieldValue = model.txts_xmgljg
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_ztbs",
                            fieldValue = model.txts_ztbs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_sjdwtz",
                            fieldValue = model.txts_sjdwtz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_sgdwls",
                            fieldValue = model.txts_sgdwls
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_jldwls",
                            fieldValue = model.txts_jldwls
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_sgqqzb",
                            fieldValue = model.txts_sgqqzb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_yssb",
                            fieldValue = model.txts_yssb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_hjyx",
                            fieldValue = model.txts_hjyx
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_xmgljg",
                            fieldType = "http:",
                            fieldValue = model.file_xmgljg
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_lxpf",
                            fieldType = "http:",
                            fieldValue = model.file_lxpf
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_sgzzsj",
                            fieldType = "http:",
                            fieldValue = model.file_sgzzsj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_sgjcry",
                            fieldType = "http:",
                            fieldValue = model.file_sgjcry
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_sbcljf",
                            fieldType = "http:",
                            fieldValue = model.file_sbcljf
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_zyzds",
                            fieldType = "http:",
                            fieldValue = model.file_zyzds
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_gcxxjd",
                            fieldValue = model.txts_gcxxjd
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bmfzr",
                            fieldValue = model.hr_bmfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_zgld",
                            fieldValue = model.hr_zgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "file_fj",
                            fieldValue = model.file_fj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_gcxxjdb",
                            fieldValue = model.txts_gcxxjdb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "role_jsry",
                            fieldValue = model.role_jsry
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jsxmm",
                            fieldValue = model.txt_jsxmm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_qfrq",
                            fieldValue = model.date_qfrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_dw",
                            fieldValue = model.dep_dw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sgdw",
                            fieldValue = model.txt_sgdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmjl",
                            fieldValue = model.txt_xmjl
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_aqkscjsg",
                            fieldValue = model.txt_aqkscjsg
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jldw",
                            fieldValue = model.txt_jldw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jlgcs",
                            fieldValue = model.txt_jlgcs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_aqkscjjl",
                            fieldValue = model.txt_aqkscjjl
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_jhjsgq",
                            fieldValue = model.int_jhjsgq
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.txt_jsxmmc + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 设计方案提交流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateDesignSchemeWorkFlow(TzDesignSchemeWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfSjfa).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationTypeName",
                            fieldValue = model.StationTypeName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectCode",
                            fieldValue = model.ProjectCode
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProvinceName",
                            fieldValue = model.ProvinceName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DesignUnit",
                            fieldValue = model.DesignUnit
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StandarName",
                            fieldValue = model.StandarName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Estimate",
                            fieldValue = model.Estimate
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "TotalInvestment",
                            fieldValue = model.TotalInvestment
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OtheInvestment",
                            fieldValue = model.OtheInvestment
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "InviteTime",
                            fieldValue = model.InviteTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DesignUnitCharge",
                            fieldValue = model.DesignUnitCharge
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DesignJob",
                            fieldValue = model.DesignJob
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectManager",
                            fieldValue = model.ProjectManager
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectJob",
                            fieldValue = model.ProjectJob
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandArea",
                            fieldValue = model.LandArea
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineofOilStage",
                            fieldValue = model.MachineofOilStage
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineofGasStage",
                            fieldValue = model.MachineofGasStage
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "GasWells",
                            fieldValue = model.GasWells
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OilTank",
                            fieldValue = model.OilTank
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Shelter",
                            fieldValue = model.Shelter
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationRoom",
                            fieldValue = model.StationRoom
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ConvenienceRoom",
                            fieldValue = model.ConvenienceRoom
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ReleaseInvestmentAmount",
                            fieldValue = model.ReleaseInvestmentAmount
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApprovalNo",
                            fieldValue = model.ApprovalNo
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OtherProject",
                            fieldValue = model.OtherProject
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "IsSynchro",
                            fieldValue = model.IsSynchro
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "EngineeringCost",
                            fieldValue = model.EngineeringCost
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandCosts",
                            fieldValue = model.LandCosts
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName ="OtherExpenses",
                            fieldValue = model.OtherExpenses
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 施工图纸会审流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateConDrawingWorkFlow(TzConDrawingWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfsgtu).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationTypeName",
                            fieldValue = model.StationTypeName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectCode",
                            fieldValue = model.ProjectCode
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProvinceName",
                            fieldValue = model.ProvinceName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ReviewTime",
                            fieldValue = model.ReviewTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Moderator",
                            fieldValue = model.Moderator
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ReviewAddress",
                            fieldValue = model.ReviewAddress
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ReviewExperts",
                            fieldValue = model.ReviewExperts
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Participants",
                            fieldValue = model.Participants
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Conclusion",
                            fieldValue = model.Conclusion
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandCosts",
                            fieldValue = model.LandCosts
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OtherExpenses",
                            fieldValue = model.OtherExpenses
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "EngineeringCost",
                            fieldValue = model.EngineeringCost
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 评审材料上报流程申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateFormTalkFileWorkFlow(TzFormTalkFileWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfPsclsb).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectLeaderName",
                            fieldValue = model.ProjectLeaderName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectDecisionerName",
                            fieldValue = model.ProjectDecisionerName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 项目评审记录流程申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateProjectReveiewsWorkFlow(TzProjectReveiewsWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfxmps).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ReveiewDate",
                            fieldValue = model.ReveiewDate
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "HostUser",
                            fieldValue = model.HostUser
                        },
                         new WorkflowRequestTableField()
                        {
                            fieldName = "Address",
                            fieldValue = model.Address
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ConclusionName",
                            fieldValue = model.ConclusionName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OtherInfo",
                            fieldValue = model.OtherInfo
                        },
                         new WorkflowRequestTableField()
                        {
                            fieldName = "InvitedExperts",
                            fieldValue = model.InvitedExperts
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Attendees",
                            fieldValue = model.Attendees
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PerfectContent",
                            fieldValue = model.PerfectContent
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 上会材料上报流程申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateMeetingFileReportWorkFlow(TzMeetingFileReportWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfShclsb).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OilSalesTotal",
                            fieldValue = model.OilSalesTotal
                        },
                         new WorkflowRequestTableField()
                        {
                            fieldName = "CNGY",
                            fieldValue = model.CNGY
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LNGQ",
                            fieldValue = model.LNGQ
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
        /// <summary>
        /// 项目批复流程申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateProjectApprovalInfoWorkFlow(TzProjectApprovalInfoWorkFlowView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfxmpf).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OilSalesTotal",
                            fieldValue = model.OilSalesTotal
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CNGY",
                            fieldValue = model.CNGY
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LNGQ",
                            fieldValue = model.LNGQ
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationTypeName",
                            fieldValue = model.StationTypeName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProvinceName",
                            fieldValue = model.ProvinceName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LimitName",
                            fieldValue = model.LimitName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StandardName",
                            fieldValue = model.StandardName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ProjectCode",
                            fieldValue = model.ProjectCode
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ApprovalNo",
                            fieldValue = model.ApprovalNo
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "SignerName",
                            fieldValue = model.SignerName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Name",
                            fieldValue = model.Name
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "SignPeopleName",
                            fieldValue = model.SignPeopleName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DecisionMakerNam",
                            fieldValue = model.DecisionMakerNam
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "FieldManagerName",
                            fieldValue = model.FieldManagerName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "HeadOperationsName",
                            fieldValue = model.HeadOperationsName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "TotalInvestment",
                            fieldValue = model.TotalInvestment
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ContractPayment",
                            fieldValue = model.ContractPayment
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "EngineeringCost",
                            fieldValue = model.EngineeringCost
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandCosts",
                            fieldValue = model.LandCosts
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OtherExpenses",
                            fieldValue = model.OtherExpenses
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DateFirstScheme",
                            fieldValue = model.DateFirstScheme
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "IssuedPlan",
                            fieldValue = model.IssuedPlan
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Temp_TzAttachs",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineOfOilStage",
                            fieldValue = model.MachineOfOilStage
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineOfOil",
                            fieldValue = model.MachineOfOil
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineOfGasStage",
                            fieldValue = model.MachineOfGasStage
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "MachineOfGas",
                            fieldValue = model.MachineOfGas
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Shelter",
                            fieldValue = model.Shelter
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "OilTank",
                            fieldValue = model.OilTank
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "StationRoom",
                            fieldValue = model.StationRoom
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "TankNumber",
                            fieldValue = model.TankNumber
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "GasWells",
                            fieldValue = model.GasWells
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandPaymentType",
                            fieldValue = model.LandPaymentType
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "AssetTypeName",
                            fieldValue = model.AssetTypeName
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "InvestmentAmount",
                            fieldValue = model.InvestmentAmount
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "RemarkOnLandCost",
                            fieldValue = model.RemarkOnLandCost
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandStatus",
                            fieldValue = model.LandStatus
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LandUse",
                            fieldValue = model.LandUse
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "AreaOfLand",
                            fieldValue = model.AreaOfLand
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ExpectedPaymentThisYear",
                            fieldValue = model.ExpectedPaymentThisYear
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "EstimatedTimeOfSales",
                            fieldValue = model.EstimatedTimeOfSales
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "UnitFeasibilityCompilation",
                            fieldValue = model.UnitFeasibilityCompilation
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "FormOfEquityInvestment",
                            fieldValue = model.FormOfEquityInvestment
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ImplementationCcompany",
                            fieldValue = model.ImplementationCcompany
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "RegisteredCapital",
                            fieldValue = model.RegisteredCapital
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "HoldingTheProportion",
                            fieldValue = model.HoldingTheProportion
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DailyDieselSales",
                            fieldValue = model.DailyDieselSales
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "DailyGasolineSales",
                            fieldValue = model.DailyGasolineSales
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ChaiQibi",
                            fieldValue = model.ChaiQibi
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "CNG",
                            fieldValue = model.CNG
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "LNG",
                            fieldValue = model.LNG
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "PayBackPeriod",
                            fieldValue = model.PayBackPeriod
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "InternalRateReturn",
                            fieldValue = model.InternalRateReturn
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "AnnualNonOilIncome",
                            fieldValue = model.AnnualNonOilIncome
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "NonOilAnnualCost",
                            fieldValue = model.NonOilAnnualCost
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "ScheduledComTime",
                            fieldValue = model.ScheduledComTime
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "FeasibleApprovalDate",
                            fieldValue = model.FeasibleApprovalDate
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "HasEInformation",
                            fieldValue = model.HasEInformation
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 建设工程设计变更申请流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzChangeApplyWorkFlow(TzDesiginChangeApplyApprovalView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfGcsjbg).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sqr",
                            fieldValue = model.hr_sqr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_sqmb",
                            fieldValue = model.dep_sqmb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sqdw",
                            fieldValue = model.sub_sqdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sqrq",
                            fieldValue = model.date_sqrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gcmc",
                            fieldValue = model.txt_gcmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_htzj",
                            fieldValue = model.int_htzj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_bgzj",
                            fieldValue = model.int_bgzj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_jswd",
                            fieldValue = model.sub_jswd
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_lxdh",
                            fieldValue = model.txt_lxdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sgdw",
                            fieldValue = model.txt_sgdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sgdwlxdh",
                            fieldValue = model.txt_sgdwlxdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jldw",
                            fieldValue = model.txt_jldw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jldwlxf",
                            fieldValue = model.txt_jldwlxf
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sjdw",
                            fieldValue = model.txt_sjdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sjdwlxdh ",
                            fieldValue = model.txt_sjdwlxdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgdyy",
                            fieldValue = model.txt_bgdyy
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgdnr",
                            fieldValue = model.txt_bgdnr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gcljgc",
                            fieldValue = model.txt_gcljgc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgdgq",
                            fieldValue = model.txt_bgdgq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "fj",
                            fieldType = "http:",
                            fieldValue = model.fj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bmfzr",
                            fieldValue = model.hr_bmfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_zgld",
                            fieldValue = model.hr_zgld
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.txt_gcmc + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 建设工程项目管理人员变更申请流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzPeopleChgApplyWorkFlow(TzPeopleChgApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfGcglrybg).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sqr",
                            fieldValue = model.hr_sqr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_sqbm",
                            fieldValue = model.dep_sqbm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sqdw",
                            fieldValue = model.sub_sqdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sqrq",
                            fieldValue = model.date_sqrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmc",
                            fieldValue = model.txt_xmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_jsdz",
                            fieldValue = model.txt_jsdz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sgdw",
                            fieldValue = model.sub_sgdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_fzr",
                            fieldValue = model.txt_fzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bmfzr",
                            fieldValue = model.hr_bmfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_zgld",
                            fieldValue = model.hr_zgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "fj",
                            fieldType = "http:",
                            fieldValue = model.fj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "Fj",
                            fieldType = "http:",
                            fieldValue = model.Fj
                        },
                    }
                }
            };
            WorkflowRequestTableRecord[] tableDetailRecords = model.list.Select(p => new WorkflowRequestTableRecord()
            {
                workflowRequestTableFields = new WorkflowRequestTableField[]
                {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgqzyzsmc",
                            fieldValue = p.txt_bgqzyzsmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bghzyzsmc",
                            fieldValue = p.txt_bghzyzsmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bggw",
                            fieldValue = p.txt_bggw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgqry ",
                            fieldValue = p.txt_bgqry
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bghry",
                            fieldValue = p.txt_bghry
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bgqzyzsh",
                            fieldValue = p.txt_bgqzyzsh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bghzyzsh",
                            fieldValue = p.txt_bghzyzsh
                        },
                }
            }).ToArray();

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.txt_xmmc + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowDetailTableInfos = new WorkflowDetailTableInfo[]
                {
                    new WorkflowDetailTableInfo()
                    {
                        workflowRequestTableRecords = tableDetailRecords
                    }
                },

                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 陕西省各竞争对手加油（气）站现状上报流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzRivalReportWorkFlow(TzRivalStationReportView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfjzdsjyz).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sbr",
                            fieldValue = model.hr_sbr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "data_sbrq",
                            fieldValue = model.data_sbrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sbdw",
                            fieldValue = model.sub_sbdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dept_sbbm",
                            fieldValue = model.dept_sbbm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "data_tjjzrq",
                            fieldValue = model.data_tjjzrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sel_ds",
                            fieldValue = model.sel_ds
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_jyzzs",
                            fieldValue = model.int_jyzzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_qsyyjyz",
                            fieldValue = model.int_qsyyjyz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_qsyycng",
                            fieldValue = model.int_qsyycng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_qsyylng",
                            fieldValue = model.int_qsyylng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zsyzs",
                            fieldValue = model.int_zsyzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zsyjyzyys",
                            fieldValue = model.int_zsyjyzyys
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zsycng",
                            fieldValue = model.int_zsycng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zsylng",
                            fieldValue = model.int_zsylng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zshzs ",
                            fieldValue = model.int_zshzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zshjyzyys",
                            fieldValue = model.int_zshjyzyys
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zshcng",
                            fieldValue = model.int_zshcng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_zshlng",
                            fieldValue = model.int_zshlng
                        },

                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycqpzs",
                            fieldValue = model.int_ycqpzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycqpjjzyys",
                            fieldValue = model.int_ycqpjjzyys
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycqpcng",
                            fieldValue = model.int_ycqpcng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycqplng",
                            fieldValue = model.int_ycqplng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycsyzs",
                            fieldValue = model.int_ycsyzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycsyjyzyys",
                            fieldValue = model.int_ycsyjyzyys
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycsycng",
                            fieldValue = model.int_ycsycng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_ycsylng",
                            fieldValue = model.int_ycsylng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_shzzs",
                            fieldValue = model.int_shzzs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_shzjzyyys",
                            fieldValue = model.int_shzjzyyys
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_shzcng",
                            fieldValue = model.int_shzcng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_shzlng",
                            fieldValue = model.int_shzlng
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bz",
                            fieldValue = model.txt_bz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bmfzr",
                            fieldValue = model.hr_bmfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.hr_sbr + DateTime.Now.ToString(),
                creatorId = model.hr_sbr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sbr));

            return result;
        }

        /// <summary>
        /// 加油（气）站开发资源上报流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzDevWorkFlow(TzDevResourceReportView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfYxkfjyz).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sbr",
                            fieldValue = model.hr_sbr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "data_sbrq",
                            fieldValue = model.data_sbrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dept_sbdw",
                            fieldValue = model.dept_sbdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bfzr",
                            fieldValue = model.hr_bfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dpt_bm",
                            fieldValue = model.dpt_bm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fglds",
                            fieldValue = model.hr_fglds
                        },
                    }
                }
            };

            WorkflowRequestTableRecord[] tableDetailRecords = model.list.Select(p => new WorkflowRequestTableRecord()
            {
                workflowRequestTableFields = new WorkflowRequestTableField[]
                {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmc",
                            fieldValue = p.txt_xmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_ds",
                            fieldValue = p.select_ds
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_qx",
                            fieldValue = p.txt_qx
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmc",
                            fieldValue = p.txt_xmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmwz",
                            fieldValue = p.txt_xmwz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_xmxz",
                            fieldValue = p.select_xmxz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_yjztz",
                            fieldValue = p.int_yjztz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_kyxs ",
                            fieldValue = p.int_kyxs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_qcb",
                            fieldValue = p.int_qcb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "data_lwyxzsj",
                            fieldValue = p.data_lwyxzsj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "data_jhlxsj",
                            fieldValue = p.data_jhlxsj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_yzxm",
                            fieldValue = p.txt_yzxm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_yzdh",
                            fieldValue = p.txt_yzdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_bz",
                            fieldValue = p.txt_bz
                        },
                }
            }).ToArray();

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.dept_sbdw + DateTime.Now.ToString(),
                creatorId = model.hr_sbr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowDetailTableInfos = new WorkflowDetailTableInfo[]
                {
                    new WorkflowDetailTableInfo()
                    {
                        workflowRequestTableRecords = tableDetailRecords
                    }
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sbr));

            return result;
        }

        /// <summary>
        /// 工程甲供物资订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzOrdersWorkFlow(GcGoodsOrdersApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfWzddsp).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmtm",
                            fieldValue = model.txt_xmtm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_erp",
                            fieldValue = model.txt_erp
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmc",
                            fieldValue = model.txt_xmmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_fwdz",
                            fieldValue = model.txt_fwdz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_xzfs",
                            fieldValue = model.select_xzfs
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_sjr",
                            fieldValue = model.txt_sjr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_dh",
                            fieldValue = model.txt_dh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_lxr",
                            fieldValue = model.txt_lxr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_lxrdh",
                            fieldValue = model.txt_lxrdh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_wzzl",
                            fieldValue = model.select_wzzl
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_dgd",
                            fieldValue = model.txt_dgd
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_sqr",
                            fieldValue = model.hr_sqr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_sqdw",
                            fieldValue = model.sub_sqdw
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_sqbm",
                            fieldValue = model.dep_sqbm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_sqsj",
                            fieldValue = model.date_sqsj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_zgld",
                            fieldValue = model.hr_zgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_fgld",
                            fieldValue = model.hr_fgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_bmfzr",
                            fieldValue = model.hr_bmfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "hr_csfzr",
                            fieldValue = model.hr_csfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt",
                            fieldValue = model.txt
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_yb",
                            fieldValue = model.txt_yb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_dqn",
                            fieldValue = model.date_dqn
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gysmc",
                            fieldValue = model.txt_gysmc
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gysmcx",
                            fieldValue = model.txt_gysmcx
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gysmc_dy",
                            fieldValue = model.txt_gysmc_dy
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_wzzlx",
                            fieldValue = model.select_wzzlx
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_xmmcx",
                            fieldValue = model.txt_xmmcx
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gysdz",
                            fieldValue = model.txt_gysdz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gysyb",
                            fieldValue = model.txt_gysyb
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "select_wzzln",
                            fieldValue = model.select_wzzln
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "role_zgld",
                            fieldValue = model.role_zgld
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "role_tzcfzr",
                            fieldValue = model.role_tzcfzr
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "role_flswg",
                            fieldValue = model.role_flswg
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "date_pzrq",
                            fieldValue = model.date_pzrq
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_htbsxh",
                            fieldValue = model.txt_htbsxh
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "sub_dw",
                            fieldValue = model.sub_dw
                        },
                    }
                }
            };

            WorkflowRequestTableRecord[] tableDetailRecords = model.list.Select(p => new WorkflowRequestTableRecord()
            {
                workflowRequestTableFields = new WorkflowRequestTableField[]
                {
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_zm",
                            fieldValue = p.dep_zm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "dep_jyz",
                            fieldValue = p.dep_jyz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_pm",
                            fieldValue = p.txt_pm
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_gg",
                            fieldValue = p.txt_gg
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "float_dj",
                            fieldValue = p.float_dj
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "float_je",
                            fieldValue = p.float_je
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_dhdz",
                            fieldValue = p.txt_dhdz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txts_bz ",
                            fieldValue = p.txts_bz
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "int_mount",
                            fieldValue = p.int_mount
                        },
                        new WorkflowRequestTableField()
                        {
                            fieldName = "txt_zm",
                            fieldValue = p.txt_zm
                        },
                }
            }).ToArray();

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.txt_xmmcx + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowDetailTableInfos = new WorkflowDetailTableInfo[]
                {
                    new WorkflowDetailTableInfo()
                    {
                        workflowRequestTableRecords = tableDetailRecords
                    }
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 项目提出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzProjectProposalWorkFlow(XtTzProjectProposalView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfXmtcsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 项目性质
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()// 提出时间
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()// 站库名称
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()// 地市公司
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()// 推荐人姓名
                        {
                            fieldName = "Recommender",
                            fieldValue = model.Recommender
                        },
                        new WorkflowRequestTableField()// 推荐人职务
                        {
                            fieldName = "RecommenderJob",
                            fieldValue = model.RecommenderJob
                        },
                        new WorkflowRequestTableField()// 推荐人单位
                        {
                            fieldName = "RecommenderDept",
                            fieldValue = model.RecommenderDept
                        },
                        new WorkflowRequestTableField()// 申报人
                        {
                            fieldName = "DeclarerUser",
                            fieldValue = model.DeclarerUser
                        },
                        new WorkflowRequestTableField()// 地理位置
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()// 项目地理位置
                        {
                            fieldName = "ProjectAddress",
                            fieldValue = model.ProjectAddress
                        },
                        new WorkflowRequestTableField()// 加油站类别
                        {
                            fieldName = "StationType",
                            fieldValue = model.StationType
                        },
                        new WorkflowRequestTableField()// 估计金额
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()// 估计气日销量（CNG）
                        {
                            fieldName = "CNGY",
                            fieldValue = model.CNGY
                        },
                        new WorkflowRequestTableField()// 估计油日销量
                        {
                            fieldName = "OilSalesTotal ",
                            fieldValue = model.OilSalesTotal
                        },
                        new WorkflowRequestTableField()// 估计气日销量（LNG）
                        {
                            fieldName = "LNGQ",
                            fieldValue = model.LNGQ
                        },
                        new WorkflowRequestTableField()// 附件类型
                        {
                            fieldName = "Temp_TzAttachs ",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));
            return result;
        }

        /// <summary>
        /// 现场踏勘
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzResearchWorkFlow(XtTzResearchView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfXctksq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 项目性质
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()// 提出时间
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()// 站库名称
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()// 所属地市公司
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()// 地理位置
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()// 估计金额
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()// 起始时间
                        {
                            fieldName = "ResearchStartDate",
                            fieldValue = model.ResearchStartDate
                        },
                        new WorkflowRequestTableField()// 详细地理位置
                        {
                            fieldName = "Address",
                            fieldValue = model.Address
                        },
                        new WorkflowRequestTableField()// 周边环境
                        {
                            fieldName = "EnvironmentTypeName",
                            fieldValue = model.EnvironmentTypeName
                        },
                        new WorkflowRequestTableField()// 土地性质
                        {
                            fieldName = "LandNatureName",
                            fieldValue = model.LandNatureName
                        },
                        new WorkflowRequestTableField()// 土地用途
                        {
                            fieldName = "LandUseName",
                            fieldValue = model.LandUseName
                        },
                        new WorkflowRequestTableField()// 土地面积
                        {
                            fieldName = "LandArea",
                            fieldValue = model.LandArea
                        },
                        new WorkflowRequestTableField()// 土地形状（临街长度）
                        {
                            fieldName = "LandShape",
                            fieldValue = model.LandShape
                        },
                        new WorkflowRequestTableField()// 是否符合地区建站规划
                        {
                            fieldName = "IsMeetAreaPlan",
                            fieldValue = model.IsMeetAreaPlan
                        },
                        new WorkflowRequestTableField()// 周边车辆保有量
                        {
                            fieldName = "AroundCarCount",
                            fieldValue = model.AroundCarCount
                        },
                        new WorkflowRequestTableField()// 平均日车流量
                        {
                            fieldName = "DailyTraffic",
                            fieldValue = model.DailyTraffic
                        },
                        new WorkflowRequestTableField()// 成品油销量测算
                        {
                            fieldName = "OilSaleTotal",
                            fieldValue = model.OilSaleTotal
                        },
                        new WorkflowRequestTableField()// 柴汽比
                        {
                            fieldName = "DieselGasolineRatio",
                            fieldValue = model.DieselGasolineRatio
                        },
                        new WorkflowRequestTableField()// 土地价格
                        {
                            fieldName = "LandPrice",
                            fieldValue = model.LandPrice
                        },
                        new WorkflowRequestTableField()// 气销量测算
                        {
                            fieldName = "GasSaleTotal",
                            fieldValue = model.GasSaleTotal
                        },
                        new WorkflowRequestTableField()// 产权清晰
                        {
                            fieldName = "PropertyRights",
                            fieldValue = model.PropertyRights
                        },
                        new WorkflowRequestTableField()// 资产主体资格
                        {
                            fieldName = "AssetSubject",
                            fieldValue = model.AssetSubject
                        },
                        new WorkflowRequestTableField()// 纠纷判断
                        {
                            fieldName = "DisputesJudgment",
                            fieldValue = model.DisputesJudgment
                        },
                        new WorkflowRequestTableField()// 证照类型
                        {
                            fieldName = "License",
                            fieldValue = model.License
                        },
                        new WorkflowRequestTableField()// 形象工程是否符合行业规划
                        {
                            fieldName = "IndustryPlanning",
                            fieldValue = model.IndustryPlanning
                        },
                        new WorkflowRequestTableField()// 罩棚
                        {
                            fieldName = "Shelter",
                            fieldValue = model.Shelter
                        },
                        new WorkflowRequestTableField()// 油罐
                        {
                            fieldName = "OilTank",
                            fieldValue = model.OilTank
                        },
                        new WorkflowRequestTableField()// 加油枪(台)
                        {
                            fieldName = "MachineOfOilStage",
                            fieldValue = model.MachineOfOilStage
                        },
                        new WorkflowRequestTableField()//  加油枪(个)
                        {
                            fieldName = "MachineOfOil",
                            fieldValue = model.MachineOfOil
                        },
                        new WorkflowRequestTableField()// 加气枪(台)
                        {
                            fieldName = "MachineOfGasStage",
                            fieldValue = model.MachineOfGasStage
                        },
                        new WorkflowRequestTableField()// 加气枪(个) 
                        {
                            fieldName = "MachineOfGas",
                            fieldValue = model.MachineOfGas
                        },
                        new WorkflowRequestTableField()// 站房
                        {
                            fieldName = "StationRoon",
                            fieldValue = model.StationRoon
                        },
                        new WorkflowRequestTableField()// 储气井
                        {
                            fieldName = "GasWells",
                            fieldValue = model.GasWells
                        },
                        new WorkflowRequestTableField()// 信息化系统
                        {
                            fieldName = "HasInformationSystem",
                            fieldValue = model.HasInformationSystem
                        },
                        new WorkflowRequestTableField()// 当前成品油日销量
                        {
                            fieldName = "CurrentSalesVolume",
                            fieldValue = model.CurrentSalesVolume
                        },
                        new WorkflowRequestTableField()// 改造必要性
                        {
                            fieldName = "ReformName",
                            fieldValue = model.ReformName
                        },
                        new WorkflowRequestTableField()// 油运品距
                        {
                            fieldName = "CargoDistance",
                            fieldValue = model.CargoDistance
                        },
                        new WorkflowRequestTableField()// 当前气日销量
                        {
                            fieldName = "GasDailySales",
                            fieldValue = model.GasDailySales
                        },
                        new WorkflowRequestTableField()// 特殊销售手段
                        {
                            fieldName = "SalesMeans",
                            fieldValue = model.SalesMeans
                        },


                        new WorkflowRequestTableField()// 销量可实现性
                        {
                            fieldName = "SalesRealizability",
                            fieldValue = model.SalesRealizability
                        },
                        new WorkflowRequestTableField()// 油品来源
                        {
                            fieldName = "SourceOfOil",
                            fieldValue = model.SourceOfOil
                        },
                        new WorkflowRequestTableField()// 环保问题
                        {
                            fieldName = "Environmental",
                            fieldValue = model.Environmental
                        },
                        new WorkflowRequestTableField()// 隐患问题
                        {
                            fieldName = "hiddenDanger",
                            fieldValue = model.hiddenDanger
                        },
                        new WorkflowRequestTableField()// 改进措施
                        {
                            fieldName = "ImprovementMeasures",
                            fieldValue = model.ImprovementMeasures
                        },
                        new WorkflowRequestTableField()// 改进措施
                        {
                            fieldName = "Improvement",
                            fieldValue = model.Improvement
                        },
                        new WorkflowRequestTableField()// 投资调研人
                        {
                            fieldName = "tzResearchUserName",
                            fieldValue = model.tzResearchUserName
                        },
                        new WorkflowRequestTableField()// 投资职务
                        {
                            fieldName = "tzJobName",
                            fieldValue = model.tzJobName
                        },
                        new WorkflowRequestTableField()// 法律调研人
                        {
                            fieldName = "flResearchUserName",
                            fieldValue = model.flResearchUserName
                        },
                        new WorkflowRequestTableField()// 法律职务
                        {
                            fieldName = "flJobName",
                            fieldValue = model.flJobName
                        },
                        new WorkflowRequestTableField()// 工程调研人
                        {
                            fieldName = "gcResearchUserName",
                            fieldValue = model.gcResearchUserName
                        },
                        new WorkflowRequestTableField()// 工程职务
                        {
                            fieldName = "gcJobName",
                            fieldValue = model.gcJobName
                        },
                        new WorkflowRequestTableField()// 经营调研人
                        {
                            fieldName = "jyResearchUserName",
                            fieldValue = model.jyResearchUserName
                        },
                        new WorkflowRequestTableField()// 经营职务
                        {
                            fieldName = "jyJobName",
                            fieldValue = model.jyJobName
                        },

                         new WorkflowRequestTableField()// 安全调研人
                        {
                            fieldName = "aqResearchUserName",
                            fieldValue = model.aqResearchUserName
                        },
                        new WorkflowRequestTableField()// 安全职务
                        {
                            fieldName = "aqJobName",
                            fieldValue = model.aqJobName
                        },
                         new WorkflowRequestTableField()// 信息调研人
                        {
                            fieldName = "xxResearchUserName",
                            fieldValue = model.xxResearchUserName
                        },
                        new WorkflowRequestTableField()// 信息职务
                        {
                            fieldName = "xxJobName",
                            fieldValue = model.xxJobName
                        },
                        new WorkflowRequestTableField()// 附件类型
                        {
                            fieldName = "Temp_TzAttachs ",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 项目谈判
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzFirstNegotiationWorkFlow(XtTzNegotiationView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfXmtpsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 项目性质
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()// 提出时间
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()// 站库名称
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()// 所属地市公司
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()// 地理位置
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()// 估计金额
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()// 谈判时间
                        {
                            fieldName = "TalkTime",
                            fieldValue = model.TalkTime
                        },
                        new WorkflowRequestTableField()// 谈判地点
                        {
                            fieldName = "TalkAdress",
                            fieldValue = model.TalkAdress
                        },
                        new WorkflowRequestTableField()// 土地竞拍支付第一笔出让金
                        {
                            fieldName = "Fees",
                            fieldValue = model.Fees
                        },
                        new WorkflowRequestTableField()// 第一笔出让金日期
                        {
                            fieldName = "FeesTime",
                            fieldValue = model.FeesTime
                        },
                        new WorkflowRequestTableField()// 我方谈判人
                        {
                            fieldName = "OurNegotiators",
                            fieldValue = model.OurNegotiators
                        },
                        new WorkflowRequestTableField()// 对方谈判人
                        {
                            fieldName = "OtherNegotiators",
                            fieldValue = model.OtherNegotiators
                        },
                        new WorkflowRequestTableField()// 谈判结果
                        {
                            fieldName = "TalkResultName",
                            fieldValue = model.TalkResultName
                        },
                        new WorkflowRequestTableField()// 附件类型
                        {
                            fieldName = "Temp_TzAttachs ",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 土地出让协议谈判
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzLandNegotiationWorkFlow(XtTzNegotiationView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfTdcrxitpsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 项目性质
                        {
                            fieldName = "NatureName",
                            fieldValue = model.NatureName
                        },
                        new WorkflowRequestTableField()// 提出时间
                        {
                            fieldName = "ApplyTime",
                            fieldValue = model.ApplyTime
                        },
                        new WorkflowRequestTableField()// 站库名称
                        {
                            fieldName = "StationName",
                            fieldValue = model.StationName
                        },
                        new WorkflowRequestTableField()// 所属地市公司
                        {
                            fieldName = "CompanyName",
                            fieldValue = model.CompanyName
                        },
                        new WorkflowRequestTableField()// 地理位置
                        {
                            fieldName = "Position",
                            fieldValue = model.Position
                        },
                        new WorkflowRequestTableField()// 估计金额
                        {
                            fieldName = "PredictMoney",
                            fieldValue = model.PredictMoney
                        },
                        new WorkflowRequestTableField()// 谈判时间
                        {
                            fieldName = "TalkTime",
                            fieldValue = model.TalkTime
                        },
                        new WorkflowRequestTableField()// 谈判地点
                        {
                            fieldName = "TalkAdress",
                            fieldValue = model.TalkAdress
                        },
                        new WorkflowRequestTableField()// 土地竞拍支付第一笔出让金
                        {
                            fieldName = "Fees",
                            fieldValue = model.Fees
                        },
                        new WorkflowRequestTableField()// 第一笔出让金日期
                        {
                            fieldName = "FeesTime",
                            fieldValue = model.FeesTime
                        },
                        new WorkflowRequestTableField()// 我方谈判人
                        {
                            fieldName = "OurNegotiators",
                            fieldValue = model.OurNegotiators
                        },
                        new WorkflowRequestTableField()// 对方谈判人
                        {
                            fieldName = "OtherNegotiators",
                            fieldValue = model.OtherNegotiators
                        },
                        new WorkflowRequestTableField()// 谈判结果
                        {
                            fieldName = "TalkResultName",
                            fieldValue = model.TalkResultName
                        },
                        new WorkflowRequestTableField()// 附件类型
                        {
                            fieldName = "Temp_TzAttachs ",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 招标申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzTenderingApplyWorkFlow(XtTzTenderingApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfZbsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 承办部门
                        {
                            fieldName = "UndertakeDepartment",
                            fieldValue = model.UndertakeDepartment
                        },
                        new WorkflowRequestTableField()// 联系人
                        {
                            fieldName = "UndertakeContacts",
                            fieldValue = model.UndertakeContacts
                        },
                        new WorkflowRequestTableField()// 联系电话
                        {
                            fieldName = "UndertakeTel",
                            fieldValue = model.UndertakeTel
                        },
                        new WorkflowRequestTableField()// 批复文件或者纪要
                        {
                            fieldName = "Minutes",
                            fieldValue = model.Minutes
                        },
                        new WorkflowRequestTableField()// 招标名称
                        {
                            fieldName = "TenderingName",
                            fieldValue = model.TenderingName
                        },
                        new WorkflowRequestTableField()// 招标类型
                        {
                            fieldName = "TenderingType",
                            fieldValue = model.TenderingType
                        },
                        new WorkflowRequestTableField()// 招标方式
                        {
                            fieldName = "BidName",
                            fieldValue = model.BidName
                        },
                        new WorkflowRequestTableField()// 资金预算及依据
                        {
                            fieldName = "CapitalBudget",
                            fieldValue = model.CapitalBudget
                        },
                        new WorkflowRequestTableField()// 项目概述
                        {
                            fieldName = "ProjectSummary",
                            fieldValue = model.ProjectSummary
                        },
                        new WorkflowRequestTableField()// 附件类型
                        {
                            fieldName = "Temp_TzAttachs ",
                            fieldType = "http:",
                            fieldValue = model.Temp_TzAttachs
                        }
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 招标结果
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzBidResultWorkFlow(XtTzBidResultView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfZbjgsq).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                        new WorkflowRequestTableField() // 项目名称
                        {
                            fieldName = "ProjectName",
                            fieldValue = model.ProjectName
                        },
                        new WorkflowRequestTableField()// 承办部门
                        {
                            fieldName = "UndertakeDepartment",
                            fieldValue = model.UndertakeDepartment
                        },
                        new WorkflowRequestTableField()// 联系人
                        {
                            fieldName = "UndertakeContacts",
                            fieldValue = model.UndertakeContacts
                        },
                        new WorkflowRequestTableField()// 联系电话
                        {
                            fieldName = "UndertakeTel",
                            fieldValue = model.UndertakeTel
                        },
                        new WorkflowRequestTableField()// 批复文件或者纪要
                        {
                            fieldName = "Minutes",
                            fieldValue = model.Minutes
                        },
                        new WorkflowRequestTableField()// 招标方式
                        {
                            fieldName = "BidName",
                            fieldValue = model.BidName
                        },
                        new WorkflowRequestTableField()// 资金预算及依据
                        {
                            fieldName = "CapitalBudget",
                            fieldValue = model.CapitalBudget
                        },
                        new WorkflowRequestTableField()// 项目概述
                        {
                            fieldName = "ProjectSummary",
                            fieldValue = model.ProjectSummary
                        },
                        new WorkflowRequestTableField()// 邀请谈判理由
                        {
                            fieldName = "InvitationNegotiate",
                            fieldValue = model.InvitationNegotiate
                        },
                        new WorkflowRequestTableField()// 拟邀请潜在谈判人
                        {
                            fieldName = "InvitationNegotiator",
                            fieldValue = model.InvitationNegotiator
                        },
                        new WorkflowRequestTableField() //公示公司一
                        {
                            fieldName = "BidderOne",
                            fieldValue = model.BidderOne
                        },
                        new WorkflowRequestTableField()//公示价格一
                        {
                            fieldName = "QuotationOne",
                            fieldValue = model.QuotationOne
                        },
                        new WorkflowRequestTableField()//公示备注一
                        {
                            fieldName = "RemarkOne",
                            fieldValue = model.RemarkOne
                        },
                        new WorkflowRequestTableField()//公示公司二
                        {
                            fieldName = "BidderTwo",
                            fieldValue = model.BidderTwo
                        },
                        new WorkflowRequestTableField()//公示价格二
                        {
                            fieldName = "QuotationTwo",
                            fieldValue = model.QuotationTwo
                        },
                        new WorkflowRequestTableField()//公示备注二
                        {
                            fieldName = "RemarkTwo",
                            fieldValue = model.RemarkTwo
                        },
                        new WorkflowRequestTableField()//公示公司三
                        {
                            fieldName = "BidderThree",
                            fieldValue = model.BidderThree
                        },
                        new WorkflowRequestTableField()//公示价格三
                        {
                            fieldName = "QuotationThree",
                            fieldValue = model.QuotationThree
                        },
                        new WorkflowRequestTableField()//公示备注三
                        {
                            fieldName = "RemarkThree",
                            fieldValue = model.RemarkThree
                        },
                        new WorkflowRequestTableField()//拟推荐单位
                        {
                            fieldName = "RecommendUnit",
                            fieldValue = model.RecommendUnit
                        },
                        new WorkflowRequestTableField()//推荐理由
                        {
                            fieldName = "RecommendReason",
                            fieldValue = model.RecommendReason
                        },
                    }
                }
            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 提交表单数据至协同创建审批流接口，返回生成的流程 ID
        /// </summary>
        /// <param name="data">表单数据</param>
        /// <param name="userId">创建人 ID(协同平台的用户 ID)</param>
        /// <returns></returns>
        private static string GetFlowId(WorkflowRequestInfo data, int userId)
        {
            WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
            var result = workFlowClient.doCreateWorkflowRequest(data, userId);

            int returnFlowId;
            if (int.TryParse(result, out returnFlowId))
            {
                if (returnFlowId > 0)
                {
                    return result;
                }
                else
                {
                    string error = "";
                    switch (returnFlowId)
                    {
                        case -1:
                            error = "创建流程失败！";
                            break;
                        case -2:
                            error = "用户没有流程创建权限！";
                            break;
                        case -3:
                            error = "创建流程基本信息失败！";
                            break;
                        case -4:
                            error = "保存表单主表信息失败！";
                            break;
                        case -5:
                            error = "更新紧急程度失败！";
                            break;
                        case -6:
                            error = "流程操作者失败！";
                            break;
                        case -7:
                            error = "流转至下一节点失败！";
                            break;
                        case -8:
                            error = "节点附加操作失败！";
                            break;
                        default:
                            error = "发起流程申请失败！";
                            break;
                    }
                    
                    throw new Exception(error);

                }
            }

            //WriteLog(BusinessType.Approver.GetText(), SystemRight.Check.GetText(), string.Format("流程ID{0}{1}用户ID{2}返回编码{3}", data.workflowBaseInfo.workflowId, data.workflowBaseInfo.workflowName, userId, returnFlowId.ToString()));

            return result;
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="filePaht"></param>
        /// <returns></returns>
        public static string GetXtAttachPaht(string filePaht)
        {
            string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("TepmoraryPath");
            string resourceRoot = System.Configuration.ConfigurationManager.AppSettings.Get("XtTZDownloadUrl");
            if (string.IsNullOrWhiteSpace(filePaht))
            {
                return string.Empty;
            }
            return filePaht.Replace(baseFaleUrl, resourceRoot).Replace("\\", "/");
        }

        /// <summary>
        /// 甲供物资申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzSupplyMaterialApplyWorkFlow(XtTzSupplyMaterialApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.WfJgwzsq).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                       new WorkflowRequestTableField(){ fieldName = "ApplyTitles", fieldValue = model.ApplyTitle},
                       new WorkflowRequestTableField(){ fieldName = "ApplyUserNames", fieldValue = model.ApplyUserName},
                       new WorkflowRequestTableField(){ fieldName = "CreateTimes", fieldValue = model.CreateTime},
                       new WorkflowRequestTableField(){ fieldName = "ApplyDepartments", fieldValue = model.ApplyDepartment},
                       new WorkflowRequestTableField(){ fieldName = "ApplyCompanyNames", fieldValue = model.ApplyCompanyName},
                       new WorkflowRequestTableField(){ fieldName = "ProjectNames", fieldValue = model.ProjectName},
                       new WorkflowRequestTableField(){ fieldName = "StationNames", fieldValue = model.StationName},
                       new WorkflowRequestTableField(){ fieldName = "ApprovalNos", fieldValue = model.ApprovalNo},
                       new WorkflowRequestTableField(){ fieldName = "ContractNames", fieldValue = model.ContractName},
                       new WorkflowRequestTableField(){ fieldName = "ContractNumbers", fieldValue = model.ContractNumber},
                       new WorkflowRequestTableField(){ fieldName = "ErpCodes", fieldValue = model.ErpCode},
                       new WorkflowRequestTableField(){ fieldName = "ArrivalContactss", fieldValue = model.ArrivalContacts},
                       new WorkflowRequestTableField(){ fieldName = "ArrivalAddresss", fieldValue = model.ArrivalAddress},
                       new WorkflowRequestTableField(){ fieldName = "Suppliers", fieldValue = model.Supplier},
                       new WorkflowRequestTableField(){ fieldName = "SupplierCodes", fieldValue = model.SupplierCode},
                       new WorkflowRequestTableField(){ fieldName = "SupplierContactss", fieldValue = model.SupplierContacts},
                       new WorkflowRequestTableField(){ fieldName = "SupplierTels", fieldValue = model.SupplierTel},
                       new WorkflowRequestTableField(){ fieldName = "SupplierAddresss", fieldValue = model.SupplierAddress},
                       new WorkflowRequestTableField(){ fieldName = "Numbers", fieldValue = model.Number},
                       new WorkflowRequestTableField(){ fieldName = "Moneys", fieldValue = model.Money},
                       new WorkflowRequestTableField(){ fieldName = "LeadershipNames", fieldValue = model.LeadershipName},
                       new WorkflowRequestTableField(){ fieldName = "ArrivalContactsTels", fieldValue = model.ArrivalContactsTel},

                    }

                }

        };
            WorkflowRequestTableRecord[] tableDetailRecords = model.list.Select(p => new WorkflowRequestTableRecord()
            {
                workflowRequestTableFields = new WorkflowRequestTableField[]
                 {
                       new WorkflowRequestTableField(){ fieldName = "MaterialCategorys", fieldValue = p.MaterialCategory},
                       new WorkflowRequestTableField(){ fieldName = "ProductNames", fieldValue = p.ProductName},
                       new WorkflowRequestTableField(){ fieldName = "Specifications", fieldValue = p.Specification},
                       new WorkflowRequestTableField(){ fieldName = "UnitPrices", fieldValue = p.UnitPrice},
                       new WorkflowRequestTableField(){ fieldName = "Moneys", fieldValue = p.Moneys},
                       new WorkflowRequestTableField(){ fieldName = "CLNumber", fieldValue = p.CLNumber},
                 }
            }).ToArray();

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowDetailTableInfos = new WorkflowDetailTableInfo[]
                {
                    new WorkflowDetailTableInfo()
                    {
                        workflowRequestTableRecords = tableDetailRecords
                    }
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));
            
            return result;
        }

        /// <summary>
        /// 开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string CreateTzProjectStartApplyWorkFlow(XtTzProjectStartApplyView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfkgsq).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                       new WorkflowRequestTableField(){ fieldName = "ProjectName", fieldValue = model.ProjectName},
                       new WorkflowRequestTableField(){ fieldName = "ApplyTitle", fieldValue = model.ApplyTitle},
                       new WorkflowRequestTableField(){ fieldName = "ApplyUserName", fieldValue = model.ApplyUserName},
                       new WorkflowRequestTableField(){ fieldName = "CreateTime", fieldValue = model.CreateTime},
                       new WorkflowRequestTableField(){ fieldName = "ApplyDepartment", fieldValue = model.ApplyDepartment},
                       new WorkflowRequestTableField(){ fieldName = "ApplyTel", fieldValue = model.ApplyTel},
                       new WorkflowRequestTableField(){ fieldName = "DesignScale", fieldValue = model.DesignScale},
                       new WorkflowRequestTableField(){ fieldName = "BuildNumber", fieldValue = model.BuildNumber},
                       new WorkflowRequestTableField(){ fieldName = "InvestmentEstimateAmount", fieldValue = model.InvestmentEstimateAmount},
                       new WorkflowRequestTableField(){ fieldName = "FeasibilityReport", fieldValue = model.FeasibilityReport},
                       new WorkflowRequestTableField(){ fieldName = "ApprovalNo", fieldValue = model.ApprovalNo},
                       new WorkflowRequestTableField(){ fieldName = "ReplyInvestmentAmount", fieldValue = model.ReplyInvestmentAmount},
                       new WorkflowRequestTableField(){ fieldName = "FundsSource", fieldValue = model.FundsSource},
                       new WorkflowRequestTableField(){ fieldName = "CurrentPlanned", fieldValue = model.CurrentPlanned},
                       new WorkflowRequestTableField(){ fieldName = "PlanStartTime", fieldValue = model.PlanStartTime},
                       new WorkflowRequestTableField(){ fieldName = "PlanEndTime", fieldValue = model.PlanEndTime},
                       new WorkflowRequestTableField(){ fieldName = "BuildCycle", fieldValue = model.BuildCycle},
                       new WorkflowRequestTableField(){ fieldName = "ProjectSummary", fieldValue = model.ProjectSummary},
                       new WorkflowRequestTableField(){ fieldName = "ProjectManagement", fieldValue = model.ProjectManagement},
                       new WorkflowRequestTableField(){ fieldName = "BuildDeploy", fieldValue = model.BuildDeploy},
                       new WorkflowRequestTableField(){ fieldName = "DesignUnits", fieldValue = model.DesignUnits},
                       new WorkflowRequestTableField(){ fieldName = "ConstructionName", fieldValue = model.ConstructionName},
                       new WorkflowRequestTableField(){ fieldName = "ProjectManager", fieldValue = model.ProjectManager},
                       new WorkflowRequestTableField(){ fieldName = "ConstructionScore", fieldValue = model.ConstructionScore},
                       new WorkflowRequestTableField(){ fieldName = "ConstructionSituation", fieldValue = model.ConstructionSituation},
                       new WorkflowRequestTableField(){ fieldName = "SupervisionUnit", fieldValue = model.SupervisionUnit},
                       new WorkflowRequestTableField(){ fieldName = "SupervisionEngineer", fieldValue = model.SupervisionEngineer},
                       new WorkflowRequestTableField(){ fieldName = "SupervisionScore", fieldValue = model.SupervisionScore},
                       new WorkflowRequestTableField(){ fieldName = "SupervisionSituation", fieldValue = model.SupervisionSituation},
                       new WorkflowRequestTableField(){ fieldName = "ConstructionReady", fieldValue = model.ConstructionReady},
                       new WorkflowRequestTableField(){ fieldName = "MainEquipment", fieldValue = model.MainEquipment},
                       new WorkflowRequestTableField(){ fieldName = "Environment", fieldValue = model.Environment},

                       new WorkflowRequestTableField(){ fieldName = "Temp_TzAttachs", fieldType = "http:", fieldValue = model.Temp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "xmglTemp_TzAttachs", fieldType = "http:", fieldValue = model.xmglTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "lxpfTemp_TzAttachs", fieldType = "http:", fieldValue = model.lxpfTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "jspTemp_TzAttachs", fieldType = "http:", fieldValue = model.jspTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "sgjcTemp_TzAttachs", fieldType = "http:", fieldValue = model.sgjcTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "fgsyTemp_TzAttachs", fieldType = "http:", fieldValue = model.fgsyTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "gcjsxmTemp_TzAttachs", fieldType = "http:", fieldValue = model.gcjsxmTemp_TzAttachs},
                       new WorkflowRequestTableField(){ fieldName = "LeadershipName", fieldType = "http:", fieldValue = model.LeadershipName},
                       new WorkflowRequestTableField(){ fieldName = "gcxxTemp_TzAttachs", fieldType = "http:", fieldValue = model.gcxxTemp_TzAttachs},

                    }
                }
            };
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.ProjectName + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }

        /// <summary>
        /// 竣工验收申请
        /// </summary>
        /// <param name = "model" ></ param >
        /// < returns ></ returns >
        public static string CreateCompletionAcceptanceWorkFlow(XtCompletionAcceptanceView model)
        {
            string workFlowId = ((int)XtWorkFlowCode.Wfjgyssq).ToString();
            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }
            WorkflowRequestTableRecord[] tableRecords = new WorkflowRequestTableRecord[]
            {
                new WorkflowRequestTableRecord()
                {
                    workflowRequestTableFields = new WorkflowRequestTableField[]
                    {
                       new WorkflowRequestTableField(){ fieldName = "ProjectSubjectName", fieldValue = model.ProjectSubjectName},
                       new WorkflowRequestTableField(){ fieldName = "ProjectName", fieldValue = model.Name},
                       new WorkflowRequestTableField(){ fieldName = "ProjectTypeName", fieldValue = model.ProjectTypeName},
                       new WorkflowRequestTableField(){ fieldName = "StartDate", fieldValue = model.StartDate.ToString()},
                       new WorkflowRequestTableField(){ fieldName = "EndDate", fieldValue = model.EndDate.ToString()},
                       new WorkflowRequestTableField(){ fieldName = "ContactUserName", fieldValue = model.ContactUserName},
                       new WorkflowRequestTableField(){ fieldName = "ContactPhone", fieldValue = model.ContactPhone},
                       new WorkflowRequestTableField(){ fieldName = "Address", fieldValue = model.Address},
                       new WorkflowRequestTableField(){ fieldName = "Description", fieldValue = model.Description},
                       new WorkflowRequestTableField(){ fieldName = "Title", fieldValue = ""},
                       new WorkflowRequestTableField(){ fieldName = "RecTime", fieldValue = model.EndDate.ToString()},
                       new WorkflowRequestTableField(){ fieldName = "Content", fieldValue = model.Remark},
                       new WorkflowRequestTableField(){ fieldName = "AcceptanceResult", fieldValue = model.ProjectTypeName},
                       new WorkflowRequestTableField(){ fieldName = "RecCompanyName", fieldValue = model.CrtCompanyName},
                       new WorkflowRequestTableField(){ fieldName = "RecUserName", fieldValue = model.ContactUserName},
                       new WorkflowRequestTableField(){ fieldName = "Epm_TzAttachs", fieldType = "http:",fieldValue = model.Temp_TzAttachs}

                    }

                }

            };

            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = model.Name + DateTime.Now.ToString(),
                creatorId = model.hr_sqr,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急
                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = tableRecords
                },
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };
            string result = GetFlowId(data, Convert.ToInt32(model.hr_sqr));

            return result;
        }
    }
}
