using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers.Mobile
{
    /// <summary>
    /// H5 页面
    /// </summary>
    public class H5Controller : Controller
    {
        protected ClientProxyExType ProxyEx(HttpRequestBase request, string userName = "youke")
        {
            ClientProxyExType cpet = Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null)
            {
                //TODO:用户登录后需要修改为用户信息，同时给applicationcontext赋值用户信息看在多用户登录情况下服务中是否生效,否则如写日志等操作需要在客户端将用户id传递过去
                cpet = new ClientProxyExType();
                cpet.UserID = userName;
                cpet.IP_Client = request.UserHostAddress;
                cpet.IP_WebServer = NetTools.GetLocalMachineIP4();
                Session[ConstStr_Session.CurrentProxyExType] = cpet;
            }
            return cpet;
        }

        [HttpGet]
        public ActionResult BimInfo(long id = 0)
        {
            Result<List<Epm_QuestionBIM>> result = new Result<List<Epm_QuestionBIM>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var bimModelResult = proxy.GetBimModel(id);
                if (bimModelResult.Flag == EResultFlag.Success && bimModelResult.Data != null)
                {
                    ViewBag.BIMAddress = bimModelResult.Data.BIMAddress;
                }
            }
            ViewBag.BimId = id.ToString();
            return View();
        }
        [HttpGet]
        public ActionResult KpiInvestment()
        {
            return View();
        }
        [HttpGet]
        public ActionResult KpiConstruct()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">问题ID</param>
        /// <param name="bimId"></param>
        /// <param name="comList"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RelationBim(long id = 0, long bimId = 0, string bimList = "", long projectId = 0)
        {
            Result<List<Epm_QuestionBIM>> result = new Result<List<Epm_QuestionBIM>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (bimList.Length != 0)
                {
                    List<Epm_QuestionBIM> bList = new List<Epm_QuestionBIM>();
                    bList = JsonConvert.DeserializeObject<List<Epm_QuestionBIM>>(bimList);
                    bimId = bList[0].ComponentId.Split('_')[0].ToLongReq();
                }

                Result<Epm_Bim> bimModelResult = new Result<Epm_Bim>();
                if (bimId != 0)
                {
                    bimModelResult = proxy.GetBimModel(bimId);
                    ViewBag.BimId = bimId.ToString();
                }
                else
                {
                    #region 根据项目ID获取模型数据

                    #region 条件
                    //QueryCondition qc = new QueryCondition();
                    //ConditionExpression ce = null;
                    //if (projectId != 0)
                    //{
                    //    ce = new ConditionExpression();
                    //    ce.ExpName = "projectId";
                    //    ce.ExpValue = projectId;
                    //    ce.ExpOperater = eConditionOperator.Equal;
                    //    ce.ExpLogical = eLogicalOperator.And;
                    //    qc.ConditionList.Add(ce);
                    //}

                    //ce = new ConditionExpression();
                    //ce.ExpName = "State";
                    //ce.ExpValue = (int)ApprovalState.ApprSuccess;
                    //ce.ExpOperater = eConditionOperator.Equal;
                    //ce.ExpLogical = eLogicalOperator.And;
                    //qc.ConditionList.Add(ce);

                    //qc.PageInfo.isAllowPage = false;
                    #endregion

                    Result<List<Epm_Bim>> resultBimList = new Result<List<Epm_Bim>>();
                    var modelList = proxy.GetBimModelListByProjectId(projectId);
                    var bimModel = modelList.Data.OrderByDescending(t => t.CreateTime).FirstOrDefault();
                    bimModelResult.Data = bimModel;
                    if (bimModelResult.Data != null)
                    {
                        bimModelResult.Flag = EResultFlag.Success;
                        ViewBag.BimId = bimModelResult.Data.SId;
                    }
                    else
                    {
                        ViewBag.BimId = "-1";
                        ViewBag.ComponentJson = "";
                        ViewBag.BIMAddress = "";
                    }
                    #endregion
                }

                if (ViewBag.BimId != "-1")
                {
                    if (bimList.Length == 0)
                    {
                        result = proxy.GetComponentListByQuestionId(id);
                        ViewBag.ComponentJson = JsonConvert.SerializeObject(result.Data);
                    }
                    else
                    {
                        List<Epm_QuestionBIM> bList = new List<Epm_QuestionBIM>();
                        bList = JsonConvert.DeserializeObject<List<Epm_QuestionBIM>>(bimList);
                        ViewBag.ComponentJson = JsonConvert.SerializeObject(bList);
                    }
                    if (bimModelResult.Flag == EResultFlag.Success && bimModelResult.Data != null)
                    {
                        ViewBag.BIMAddress = bimModelResult.Data.BIMAddress;
                    }
                }

            }
            return View();
        }

        //关于我们
        [HttpGet]
        public ActionResult AboutUs()
        {
            return View();
        }
        //我的问题
        [HttpGet]
        public ActionResult Questions()
        {
            return View();
        }
        //更多问题
        [HttpGet]
        public ActionResult IssuesList()
        {
            return View();
        }
        //问题详情
        [HttpGet]
        public ActionResult Problem()
        {
            return View();
        }
    }
}