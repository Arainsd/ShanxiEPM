using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.ClientSite.XtWorkFlow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 协同审批流程表单数据提交
    /// </summary>
    public class XtWorkFlowSubmitService
    {
        /// <summary>
        /// 发起协同审批申请，并获取协同申请返回的申请流程 ID
        /// </summary>
        /// <typeparam name="T">审批对象类型</typeparam>
        /// <param name="model">审批对象实体</param>
        /// <param name="requestName">申请名称</param>
        /// <param name="creatorId">协同申请人 ID</param>
        /// <param name="xtWorkFlowCode">协同工作流编码</param>
        /// <returns></returns>
        public static string GetFlowId<T>(T model, string requestName, string creatorId, XtWorkFlowCode xtWorkFlowCode)
            where T : class
        {
            Dictionary<string, object> dic = GetProperties(model);
            WorkflowRequestInfo data = CreateWorkFlowRequestInfo(dic, requestName, creatorId, xtWorkFlowCode);
            return GetFlowId(data, Convert.ToInt32(creatorId));
        }

        /// <summary>
        /// 将对象数据转换为字典数据
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <returns></returns>
        private static Dictionary<string, object> GetProperties(object obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var properties = obj.GetType().GetProperties();
            foreach (var item in properties)
            {
                var value = item.GetValue(obj, null);
                if (value == null)
                {
                    dic.Add(item.Name, "");
                }
                else
                {
                    var valueType = value.GetType();
                    if (valueType.IsGenericType)
                    {
                        List<Dictionary<string, object>> listDic = new List<Dictionary<string, object>>();
                        var list = (IEnumerable)value;
                        foreach (var itemObj in list)
                        {
                            listDic.Add(GetProperties(itemObj));
                        }
                        dic.Add(item.Name, listDic);
                    }
                    else
                    {
                        dic.Add(item.Name, value);
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// 创建协同审批数据
        /// </summary>
        /// <param name="dic">审批表单对象数据</param>
        /// <param name="requestName">申请名称</param>
        /// <param name="creatorId">协同申请人 ID</param>
        /// <param name="xtWorkFlowCode">协同工作流编码</param>
        /// <returns></returns>
        private static WorkflowRequestInfo CreateWorkFlowRequestInfo(Dictionary<string, object> dic, string requestName, string creatorId, XtWorkFlowCode xtWorkFlowCode)
        {
            string workFlowId = ((int)xtWorkFlowCode).ToString();

            if (string.IsNullOrWhiteSpace(workFlowId))
            {
                throw new Exception("未配置相关审批流程！");
            }

            List<WorkflowDetailTableInfo> detailTableInfos = new List<WorkflowDetailTableInfo>();
            WorkflowRequestInfo data = new WorkflowRequestInfo()
            {
                requestName = requestName + DateTime.Now.ToString(),
                creatorId = creatorId,
                requestLevel = "0",             // 0 正常， 1 重要， 2 紧急

                workflowMainTableInfo = new WorkflowMainTableInfo()
                {
                    requestRecords = new WorkflowRequestTableRecord[]
                    {
                        GetRequestTableRecord(dic,detailTableInfos)
                    }
                },
                workflowDetailTableInfos = detailTableInfos.ToArray(),
                workflowBaseInfo = new WorkflowBaseInfo()
                {
                    workflowId = workFlowId
                }
            };

            return data;
        }

        /// <summary>
        /// 根据表单内容创建协同请求表单记录
        /// </summary>
        /// <param name="obj">表单数据字典</param>
        /// <param name="detailTableInfos">表单子表信息</param>
        /// <returns></returns>
        private static WorkflowRequestTableRecord GetRequestTableRecord(Dictionary<string, object> obj, List<WorkflowDetailTableInfo> detailTableInfos)
        {
            List<WorkflowRequestTableField> tableFields = new List<WorkflowRequestTableField>();
            foreach (var item in obj)
            {
                WorkflowRequestTableField field = new WorkflowRequestTableField();
                field.fieldName = item.Key;
                if (item.Value == null)
                {
                    field.fieldValue = "";
                    tableFields.Add(field);
                }
                else
                {
                    var valueType = item.Value.GetType();
                    if (valueType.Name == "String")
                    {
                        field.fieldValue = (item.Value ?? "").ToString();
                        tableFields.Add(field);
                    }
                    else if (valueType.IsGenericType)
                    {
                        var data = (IEnumerable)item.Value;
                        foreach (var itemValue in data)
                        {
                            if (item.Value != null && itemValue.GetType().Name == typeof(Dictionary<string, object>).Name)
                            {
                                WorkflowDetailTableInfo detailTableInfo = new WorkflowDetailTableInfo();
                                var dic = itemValue as Dictionary<string, object>;
                                detailTableInfo.workflowRequestTableRecords = new WorkflowRequestTableRecord[]
                                {
                                    GetRequestTableRecord(dic, detailTableInfos)
                                };
                                detailTableInfos.Add(detailTableInfo);
                            }
                        }
                    }
                }
            }

            return new WorkflowRequestTableRecord()
            {
                workflowRequestTableFields = tableFields.ToArray()
            };
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
            return result;
        }


        /// <summary>
        /// 根据附件列表生成协同附件地址
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string CreateXtAttachPath(List<Epm_TzAttachs> list)
        {
            string attachUrl = string.Empty;
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                    if (string.IsNullOrWhiteSpace(attachUrl))
                    {
                        attachUrl = fileUrl;
                    }
                    else
                    {
                        attachUrl = attachUrl + "|" + fileUrl;
                    }
                }
            }
            return attachUrl;
        }
    }
}
