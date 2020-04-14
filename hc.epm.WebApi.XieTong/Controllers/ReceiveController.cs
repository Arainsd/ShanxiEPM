using hc.epm.DataModel.Business;
using hc.epm.Web.ClientProxy;
using hc.epm.WebAPI.Models;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace hc.epm.WebApi.XieTong.Controllers
{
    public class ReceiveController : ApiController
    {
        private ClientProxyExType ProxyEx
        {
            get {
                ClientProxyExType cpet = new ClientProxyExType();
                cpet.UserID = "0";
                cpet.IP_Client = HttpContext.Current.Request.UserHostAddress;  
                cpet.IP_WebServer = NetTools.GetLocalMachineIP4();
                return cpet;
            }
            
        }

        /// <summary>
        /// 加油站试运行审批结果获取
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAuditRecord(ProjectAuditRecord model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (model == null)
                {
                    throw new Exception("获取数据失败！");
                }
                result = SendData(model);
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "GetAuditRecord");
            }
            finally
            {

            }
            return result;
        }

        private Result<bool> SendData(ProjectAuditRecord model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx))
                {
                    if (model != null)
                    {
                        Epm_ProjectAuditRecord record = new Epm_ProjectAuditRecord();

                        record.WorkFlowId = model.Id;
                        record.AuditName = model.AuditName;
                        record.AuditRemark = model.AuditRemark;
                        record.AuditSign = model.AuditSign;
                        record.AuditState = model.AuditState;
                        record.AuditTime = Convert.ToDateTime(model.AuditTime);
                        record.AuditUserCode = model.AuditUserCode;
                        record.StepId = model.StepId;
                        record.StepName = model.StepName;

                        record.NextAuditName = model.NextAuditName;
                        record.NextAuditUserCode = model.NextAuditUserCode;
                        record.NextStepId = model.NextStepId;
                        record.NextStepName = model.NextStepName;
                        record.ApprovalState = model.ApprovalState;

                        result = proxy.AddProjectAuditRecord(record);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "SendData");
            }
            return result;
        }
    }
}