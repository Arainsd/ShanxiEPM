using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 甲供物资申请
    /// </summary>
    public class TzSupplyMaterialApplyController : BaseWebController
    {
        // GET: TzSupplyMaterialApply
        /// <summary>
        /// 甲供物资申请列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="title"></param>
        /// <param name="supName"></param>
        /// <param name="state"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string title = "", string supplierName = "", string state = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.title = title;
            ViewBag.projectName = projectName;
            ViewBag.supplierName = supplierName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            Result<List<Epm_TzSupplyMaterialApply>> result = new Result<List<Epm_TzSupplyMaterialApply>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = "%" + projectName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(title))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ApplyTitle";
                    ce.ExpValue = "%" + title.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(supplierName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Supplier";
                    ce.ExpValue = "%" + supplierName.Trim() + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(state))
                {
                    int d = (int)(PreProjectState)Enum.Parse(typeof(PreProjectState), state);
                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = d;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "CreateTime",
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

                        ExpName = "CreateTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzSupplyMaterialApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //审批状态
                ViewBag.State = typeof(SupApprovalState).AsSelectList(true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 甲供物资申请录入页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true);

                //申请人
                ViewBag.ApplyUserName = CurrentUser.RealName;
                ViewBag.ApplyUserId = CurrentUser.UserId;
                //申请时间
                ViewBag.StartApplyTime = DateTime.Now.ToString("yyyy-MM-dd");
                //申请部门
                ViewBag.ApplyDepartmentId = "";
                ViewBag.ApplyDepartment = "";

                //申请单位
                ViewBag.ApplyCompanyName = CurrentUser.CompanyName;
                ViewBag.ApplyCompanyId = CurrentUser.CompanyId;

                //标题
                ViewBag.ApplyTitle = "工程甲供物资订单审批流程" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");

                //获取用户信息
                var userInfo = proxy.GetUserModel(CurrentUser.UserId);
                if (userInfo.Data != null)
                {
                    if (userInfo.Data.DepartmentId != null)
                    {
                        long dempId = userInfo.Data.DepartmentId.Value;
                        if (dempId != 0)
                        {
                            var companyInfo = proxy.GetCompanyModel(dempId);

                            if (companyInfo.Data != null)
                            {
                                ViewBag.ApplyDepartmentId = companyInfo.Data.Id;
                                ViewBag.ApplyDepartment = companyInfo.Data.Name;
                            }
                        }
                    }
                }

                //物资种类
                //var hh = proxy.GetTzSupMatManagementListBy("", "").Data.Where(t => !string.IsNullOrEmpty(t.MaterialCategory))
                //    .Select(t => new { name = t.MaterialCategory, no = t.MaterialCategory })
                //    .Distinct().ToList();
                //ViewBag.SupMatManagement = hh.ToSelectList("name", "no", true);

                return View();
            }
        }

        /// <summary>
        /// 获取物资种类数据集合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSupMatManagementList(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();

                qc.PageInfo.isAllowPage = false;
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SupplierId",
                    ExpValue = id,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });

                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = 0,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });

                var result = proxy.GetTzSupMatManagementList(qc).Data.Where(t => !string.IsNullOrEmpty(t.MaterialCategory))
                    .Select(t => new { name = t.MaterialCategory, no = t.MaterialCategory })
                    .Distinct().ToList();

                return Json(result);
            }
        }

        /// <summary>
        /// 动态获取品名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSupMatProductName(long SupplierId = 0, string name = "", string productName = "")
        {
            Result<List<Epm_TzSupMatManagement>> result = new Result<List<Epm_TzSupMatManagement>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupMatManagementListBy(SupplierId, name, productName);
                if (result.Data != null)
                {
                    var resultList = result.Data.Where(t => !string.IsNullOrEmpty(t.ProductName))
                       .Select(t => new { name = t.ProductName, no = t.ProductName }).Distinct().ToList();
                    return Json(resultList);
                }
                else
                {
                    return Json(result);
                }
            }
        }

        /// <summary>
        /// 动态获取规格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSupMatSpecification(long SupplierId = 0, string name = "", string productName = "")
        {
            Result<List<Epm_TzSupMatManagement>> result = new Result<List<Epm_TzSupMatManagement>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupMatManagementListBy(SupplierId, name, productName);
                if (result.Data != null)
                {
                    var resultList = result.Data.Where(t => !string.IsNullOrEmpty(t.Specification))
                     .Select(t => new { name = t.Specification, no = t.Specification });
                    return Json(resultList);
                }
                else
                {
                    return Json(result);
                }
            }
        }


        /// <summary>
        /// 根据物资种类、品名、规格获取物资管理详细信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="productName"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSupMatManagementModel(long SupplierId, string name, string productName, string specification)
        {
            Result<Epm_TzSupMatManagement> result = new Result<Epm_TzSupMatManagement>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupMatManagementModelBy(SupplierId, name, productName, specification);
            }
            return Json(result.Data);
        }


        /// <summary>
        /// 甲供物资申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzSupplyMaterialApply model)
        {
            Result<int> result = new Result<int>();

            string tzSupMatApplyList = Request.Form["tzSupMatApplyList"];
            if (!string.IsNullOrEmpty(tzSupMatApplyList))
            {
                model.TzSupMatApplyList = JsonConvert.DeserializeObject<List<Epm_TzSupMatApplyList>>(tzSupMatApplyList);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzSupplyMaterialApply(model);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 甲供物资申请修改页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_TzSupplyMaterialApply> result = new Result<Epm_TzSupplyMaterialApply>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupplyMaterialApplyModel(id);

                var compamyList = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.CompanyName = compamyList.Data.ToSelectList("Name", "Id", true, result.Data.CompanyId.ToString());

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 甲供物资申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_TzSupplyMaterialApply model)
        {
            Result<int> result = new Result<int>();

            string tzSupMatApplyList = Request.Form["tzSupMatApplyList"];
            if (!string.IsNullOrEmpty(tzSupMatApplyList))
            {
                model.TzSupMatApplyList = JsonConvert.DeserializeObject<List<Epm_TzSupMatApplyList>>(tzSupMatApplyList);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzSupplyMaterialApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 甲供物资申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Epm_TzSupplyMaterialApply> result = new Result<Epm_TzSupplyMaterialApply>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupplyMaterialApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsApply, Right = SystemRight.Check)]
        public ActionResult UpdateState(string ids, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(ids))
            {
                view.Flag = false;
                view.Message = "请选择要操作的数据";
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
                result = proxy.UpdateTzSupplyMaterialApplyState(idList, state);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 甲供物资报表
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplyMaterialReport(int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;

            Result<List<SupplyMaterialReportView>> result = new Result<List<SupplyMaterialReportView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetSupplyMaterialReport(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 甲供物资供应商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ActionResult SupplyList(long projectId)
        {
            Result<List<SuppListView>> result = new Result<List<SuppListView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                #endregion

                result = proxy.GetSuppList(projectId, qc);
                ViewBag.Total = result.AllRowsCount;
                //ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }
        /// <summary>
        /// 批量导出弹层
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchExport()
        {
            return View();
        }
        /// <summary>
        /// 打印视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintMaterial(long id)
        {
            Result<Epm_TzSupplyMaterialApply> result = new Result<Epm_TzSupplyMaterialApply>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupplyMaterialApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        public ActionResult CreatePdf(string ids)
        {

            Result<int> resultInfo = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            string pathUrl = "";
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                string path = ConfigurationManager.AppSettings["PDFPathUrl"];
                string fileName = path + "甲供物资申请单" + DateTime.Now.ToString("yyyymmddhhMMssfff");
                if (!Directory.Exists(fileName))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fileName);
                    directoryInfo.Create();
                }
                string pdfPath = "";
                foreach (var item in idList)
                {
                    var result = proxy.GetTzSupplyMaterialApplyModel(item).Data;

                    if (result != null)
                    {
                        string pdfName = "";
                        pdfPath = "";
                        //物资信息
                        string html32 = "";
                        int index = 0;
                        foreach (var temp in result.TzSupMatApplyList)
                        {
                            index++;
                            html32 += "<tr class='tab-conten'><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + index + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.SupMatManagement + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'> " + temp.ProductName + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.Specification + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.UnitPrice.ToString("0.00") + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.Number.ToString("0.00") + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.Money.ToString("0.00") + "</td><td style='border: 1px solid #D9D8D8;padding: 2px'>" + temp.SupplierName + "</td><td style='border: 1px solid #D9D8D8;padding: 2px;height: 40px;'>" + temp.Remark + "</td></tr>";
                        }
                        //---读html模板页面到stringbuilder对象里----
                        string[] format = new string[20];//定义和htmlyem标记数目一致的数组 
                        StringBuilder htmltext = new StringBuilder();
                        try
                        {
                            string htmlPath = "~/HtmlTemp.html";
                            htmlPath = Server.MapPath(htmlPath);
                            using (StreamReader sr = new StreamReader(htmlPath))
                            {
                                String line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    htmltext.Append(line);
                                }
                                sr.Close();
                            }
                            format[0] = result.ApplyTitle;
                            format[1] = result.ApplyUserName;
                            format[2] = result.SupApplyTime.ToString("yyyy-mm-dd");
                            format[3] = result.ApplyDepartment;
                            format[4] = result.ContractCode;
                            format[5] = result.ContractNumber;
                            format[6] = result.ArrivalContacts;
                            format[7] = result.ArrivalContactsTel;
                            format[8] = result.ProjectName;
                            format[9] = result.ApprovalNo;
                            format[10] = html32;
                            format[11] = result.TotleMoney.ToString("0.00");

                            format[12] = result.SupplierContacts;
                            format[13] = result.SupplierTel;
                            format[14] = result.SupplierAddress;

                            format[15] = result.ApprovalName;
                            format[16] = result.ApprovalDep;
                            format[17] = result.StateName;
                            format[18] = result.OperateTime.ToString("yyyy-mm-dd");
                            format[19] = result.SignNameUrl;
                            //----------替换htm里的标记为你想加的内容 
                            for (int i = 0; i < 20; i++)
                            {
                                htmltext.Replace("$htmlformat[" + i + "]", format[i]);
                            }

                            string html = htmltext.ToString();
                            pdfName = result.ProjectName;
                            pdfPath = fileName + "/" + pdfName + ".pdf";
                            PDFHelper.ExportPDF(pdfPath, html);
                        }
                        catch
                        {
                            resultInfo.Flag = EResultFlag.Failure;
                        }
                    }
                }
                if (idList.Count == 1)
                {
                    pathUrl = pdfPath;
                }
                else
                {
                    string fileNamepath = System.IO.Path.GetFileName(fileName);
                    string paths = fileName;
                    //using (ZipFile zip = ZipFile.Create(paths))
                    //{
                    //    zip.BeginUpdate();
                    //    zip.CommitUpdate();
                    //}
                    pathUrl = paths + ".zip";
                    Utility.CreateZip(fileName, pathUrl);
                }
                string pathName = System.IO.Path.GetFileName(pathUrl);

                return Json(new
                {
                    Flag = resultInfo.Flag,
                    fileName = pathName,
                    download = pathUrl//文件下载链接
                });
            }
        }
    }
}
