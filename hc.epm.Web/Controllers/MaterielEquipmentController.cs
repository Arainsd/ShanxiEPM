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
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 材料设备验收
    /// </summary>
    public class MaterielEquipmentController : BaseWebController
    {
        /// <summary>
        /// 材料设备验收
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="companyName">验收单位</param>
        /// <param name="supplier">供应商</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="state">状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string companyName = "",  string supplier = "", string startTime = "", string endTime = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.supplier = supplier;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.state = typeof(ConfirmState).AsSelectList(true, state);
            ViewBag.pageIndex = pageIndex;

            #region 条件
            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = "%" + projectName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CompanyName",
                    ExpValue = "%" + companyName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(supplier))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SupplierName",
                    ExpValue = "%" + supplier + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ReceiveTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime etime = Convert.ToDateTime(endTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ReceiveTime",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = state.ToEnumReq<PreProjectApprovalState>();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)approvalState,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            #endregion

            Result<List<Epm_Materiel>> result = new Result<List<Epm_Materiel>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterielList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.CurrentUserId = CurrentUser.UserId;
            ViewBag.State = typeof(PreProjectApprovalState).AsSelectList(true);

            return View(result.Data);
        }

        /// <summary>
        /// 新增材料设备验收
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;
            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            ViewBag.CompanyId = ApplicationContext.Current.CompanyId;
            ViewBag.CompanyName = ApplicationContext.Current.CompanyName;

            return View();
        }

        /// <summary>
        /// 新增材料设备验收(提交数据)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_Materiel model)
        {
            ResultView<int> view = new ResultView<int>();
            MaterielView materiel = new MaterielView();
            #region 表单验证
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            #endregion

            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                List<Base_Files> files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符
                materiel.FileList = files;
            }
            string materielDetails = Request.Form["MaterielDetail"];
            if (!string.IsNullOrWhiteSpace(materielDetails))
            {
                materiel.MaterielDetails = JsonConvert.DeserializeObject<List<Epm_MaterielDetails>>(materielDetails);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                materiel.Epm_Materiel = model;
                result = proxy.AddMateriel(materiel);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改材料设备验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<MaterielView> result = new Result<MaterielView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterielModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改材料设备验收接收(提交数据)
        /// </summary>
        /// <param name="material"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_Materiel model)
        {
            ResultView<int> view = new ResultView<int>();
            MaterielView materiel = new MaterielView();
            #region 表单验证
            if (!model.ProjectId.HasValue || model.ProjectId.Value == 0 || string.IsNullOrWhiteSpace(model.ProjectName))
            {
                view.Flag = false;
                view.Message = "项目名称不能为空";
                return Json(view);
            }
            #endregion

            //上传附件
            string fileDataJsonFile = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                List<Base_Files> files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符
                materiel.FileList = files;
            }
            string materielDetails = Request.Form["MaterielDetail"];
            if (!string.IsNullOrWhiteSpace(materielDetails))
            {
                materiel.MaterielDetails = JsonConvert.DeserializeObject<List<Epm_MaterielDetails>>(materielDetails);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                materiel.Epm_Materiel = model;
                result = proxy.UpdateMateriel(materiel);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看材料设备验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<MaterielView> result = new Result<MaterielView>();
            Result<List<Base_Company>> company = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterielModel(id);

                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                company = proxy.GetCompanyList(qc);
                ViewBag.CompanyList = company.Data.ToSelectList("Name", "Id", true);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Check)]
        public ActionResult Audit(string ids, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(ids))
            {
                view.Flag = false;
                view.Message = "请选择要批复的数据";
                return Json(view);
            }
            if (string.IsNullOrEmpty(state))
            {
                view.Flag = false;
                view.Message = "状态不能为空";
                return Json(view);
            }
            List<long> idList = ids.SplitString(",").ToLongList();

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMaterielALLState(idList, state);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.UnCheck)]
        public ActionResult Reject(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMaterielState(id, ConfirmState.ConfirmFailure);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Invalid)]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMaterielState(id, ConfirmState.Discarded);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Materiel, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteMaterielByIds(new List<long>() { id });
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查询物资申请详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTzSupMatApplyList(long id)
        {
            Result<List<Epm_TzSupMatApplyList>> result = new Result<List<Epm_TzSupMatApplyList>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();

                qc.PageInfo.isAllowPage = false;
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "UseType",
                    ExpValue = false,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SupMatApplyId",
                    ExpValue = id,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                result = proxy.GetTzSupMatApplyList(qc);
            }
            return Json(result.Data);
        }
    }
}