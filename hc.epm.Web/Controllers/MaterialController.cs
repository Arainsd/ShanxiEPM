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
    /// 工器具验收
    /// </summary>
    public class MaterialController : BaseWebController
    {
        /// <summary>
        /// 获取工器具机械验收列表
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="companyName">验收单位</param>
        /// <param name="deviceName">材料设备名称</param>
        /// <param name="supplier">供应商</param>
        /// <param name="startTime">验收开始时间</param>
        /// <param name="endTime">验收结束时间</param>
        /// <param name="state">状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string companyName = "", string deviceName = "", string supplier = "", string startTime = "", string endTime = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.deviceName = deviceName;
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
                    ExpName = "CheckCompanyName",
                    ExpValue = "%" + companyName + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = "%" + deviceName + "%",
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
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CheckTime",
                    ExpValue = startTime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CheckTime",
                    ExpValue = endTime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = state.ToEnumReq<ConfirmState>();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)approvalState,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            #endregion

            Result<List<Epm_Material>> result = new Result<List<Epm_Material>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterialList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.CurrentUserId = CurrentUser.UserId;

            return View(result.Data);
        }

        /// <summary>
        /// 新增工器具验收
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Add)]
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
        /// 新增工器具验收(提交数据)
        /// </summary>
        /// <param name="material"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_Material model)
        {
            ResultView<int> view = new ResultView<int>();
            MaterialView material = new MaterialView();
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

            List<Base_Files> files = new List<Base_Files>();
            if (!string.IsNullOrWhiteSpace(fileDataJsonFile))
            {
                files = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonFile);//将文件信息json字符
                material.FileList = files;
            }
            
            string materialDetails = Request.Form["MaterialDetails"];
            if (!string.IsNullOrWhiteSpace(materialDetails))
            {
                material.MaterialDetails = JsonConvert.DeserializeObject<List<Epm_MaterialDetails>>(materialDetails);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                material.Epm_Material= model;
                result = proxy.AddMaterial(material);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改工器具验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<MaterialView> result = new Result<MaterialView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterialModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改工器具验收(提交数据)
        /// </summary>
        /// <param name="material"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_Material model)
        {
            ResultView<int> view = new ResultView<int>();
            MaterialView material = new MaterialView();
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
                material.FileList = files;
            }

            string materialDetails = Request.Form["MaterialDetails"];
            if (!string.IsNullOrWhiteSpace(materialDetails))
            {
                material.MaterialDetails = JsonConvert.DeserializeObject<List<Epm_MaterialDetails>>(materialDetails);
            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                material.Epm_Material = model;
                result = proxy.UpdateMaterial(material);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看工器具验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<MaterialView> result = new Result<MaterialView>();
            Result<List<Base_Company>> company = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMaterialModel(id);
                
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                company = proxy.GetCompanyList(qc);
                ViewBag.CompanyList = company.Data.ToSelectList("Name", "Id", true);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Check)]
        public ActionResult Audit(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMaterialState(id, ConfirmState.Confirm);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.UnCheck)]
        public ActionResult Reject(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMaterialState(id, ConfirmState.ConfirmFailure);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 废弃
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Invalid)]
        public ActionResult Discard(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMaterialState(id, ConfirmState.Discarded);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Material, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteMaterialByIds(new List<long>() { id });
            }
            return Json(result.ToResultView());
        }
    }
}