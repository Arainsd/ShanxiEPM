using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class ProjectManagementController : BaseWebController
    {
        // GET: ProjectManagement
        /// <summary>
        /// 项目管理列表 
        /// </summary>
        public ActionResult ProjectManagementList()
        {
            return View();
        }

        /// <summary>
        /// 土地协议出让谈判
        /// </summary>
        /// <returns></returns>
        public ActionResult TzLandTalk()
        {
            return View();
        }

        /// <summary>
        /// 评审材料上报
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialReview()
        {
            return View();
        }

        /// <summary>
        /// 上会材料上报
        /// </summary>
        /// <returns></returns>
        public ActionResult ConferenceMaterials()
        {
            return View();
        }
        /// <summary>
        /// 上会材料上报列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ConferenceMaterialsList()
        {
            return View();
        }
        /// <summary>
        /// 项目批复信息记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectApprovalRecord()
        {
            return View();
        }
        /// <summary>
        /// 项目批复信息记录列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectApprovalRecordList()
        {
            return View();
        }
        /// <summary>
        /// 项目评审记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectReviewRecords()
        {
            return View();
        }
        /// <summary>
        /// 项目评审记录列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectReviewRecordsList()
        {
            return View();
        }
        /// <summary>
        /// 上会材料添加
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult AddConferenceMaterials(long id)
        //{
        //    string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        List<Epm_TzAttachs> tzAttachs = !string.IsNullOrEmpty(fileDataJson) ? JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson) : null;
        //        var result = proxy.AddConferenceMaterials(typeof(Epm_TzProjectProposal).Name, id, tzAttachs, InvestmentEnclosure.ConferenceMaterials);
        //        return Json(result);
        //    }
        //}

        /// <summary>
        /// 项目提出申请新增
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectApplicationAdd()
        {
            return View();
        }

        /// <summary>
        /// 项目提出申请修改
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectApplicationEdit()
        {
            return View();
        }


        /// <summary>
        /// 现场勘查
        /// </summary>
        /// <returns></returns>
        public ActionResult SceneInvestigate()
        {
            return View();
        }

        /// <summary>
        /// 初次谈判
        /// </summary>
        /// <returns></returns>
        public ActionResult TzInitialTalk()
        {
            return View();
        }

       # region  
        //项目中期视图

        /// <summary>
        /// 设计方案
        /// </summary>
        /// <returns></returns>
        public ActionResult DesignScheme()
        {
            return View();
        }
        /// <summary>
        /// 设计方案列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DesignSchemeList()
        {
            return View();
        }
        /// <summary>
        /// 施工图纸会审
        /// </summary>
        /// <returns></returns>
        public ActionResult ConstructionDrawings()
        {
            return View();
        }

        /// <summary>
        /// 施工图纸会审 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult ConstructionDrawingsAdd()
        {
            return View();
        }

        /// <summary>
        /// 施工图纸会审 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ConstructionDrawingsEdit()
        {
            return View();
        }


        /// <summary>
        /// 招标申请
        /// </summary>
        /// <returns></returns>
        public ActionResult TenderApply()
        {
            return View(); 
        }

        /// <summary>
        /// 发起招标申请
        /// </summary>
        /// <returns></returns>
        public ActionResult TenderApplyAdd()
        {
            return View();
        }

        /// <summary>
        /// 甲供物资管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialManageList()
        {
            return View();
        }
        /// <summary>
        /// 招标结果
        /// </summary>
        /// <returns></returns>
        public ActionResult TenderResult()
        {
            return View();
        }

        /// <summary>
        /// 结果上传
        /// </summary>
        /// <returns></returns>
        public ActionResult TenderResultAdd()
        {
            return View();
        }

        /// <summary>
        /// 甲供物资管理
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialManage()
        {
            return View(); 
        }
        /// <summary>
        /// 甲供物资管理编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialManageEdit()
        {
            return View();
        }
        /// <summary>
        /// 甲供物资申请列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialApplyList()
        {
            return View();
        }
        /// <summary>
        /// 甲供物资申请
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialApply()
        {
            return View();
        }
        /// <summary>
        /// 甲供物资申请编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialApplyEdit()
        {
            return View();
        }
        /// <summary>
        /// 甲供物资申请详情
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialApplyDetail()
        {
            return View();
        }

        /// <summary>
        /// 竣工验收结果
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalAccepation()
        {
            return View();
        }
        /// <summary>
        /// 竣工验收结果编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalAccepationEdit()
        {
            return View();
        }
        /// <summary>
        /// 竣工验收结果详情
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalAccepationDetail()
        {
            return View();
        }
        /// <summary>
        /// 竣工验收结果上传
        /// </summary>
        /// <returns></returns>
        public ActionResult FinalAccepationAdd()
        {
            return View();
        } 

        /// <summary>
        /// 开工申请列表
        /// </summary>
        /// <returns></returns>
        public ActionResult StartApplyLIst()
        {
            return View();
        }
        /// <summary>
        /// 开工申请
        /// </summary>
        /// <returns></returns>
        public ActionResult StartApply()
        {
            return View(); 
        }
        /// <summary>
        /// 开工申请编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult StartApplyEdit()
        {
            return View();
        }
        public ActionResult StartApplyDetail()
        {
            return View();
        }
        #endregion   
    }
}