using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 甲供物资管理控制台
    /// </summary>
    public class TzSupMatManagementController : BaseWebController
    {
        // GET: TzSupMatManagement 
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="materialCategory">物资种类</param>
        /// <param name="productName">品名</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="supplierAddress">供应商地址</param>
        /// <param name="state">状态（默认值：-1 - 全部，0 - 已启用，1 - 已停用）</param>
        /// <param name="pageIndex">当前页面</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Browse)]
        public ActionResult Index(string materialCategory = "", string productName = "", string specification = "", string supplierName = "", string supplierAddress = "", int state = -1, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.materialCategory = materialCategory;
            ViewBag.productName = productName;
            ViewBag.supplierName = supplierName;
            ViewBag.supplierAddress = supplierAddress;
            ViewBag.specification = specification;
            ViewBag.state = state;
            ViewBag.PageIndex = pageIndex;
            Result<List<Epm_TzSupMatManagement>> result = new Result<List<Epm_TzSupMatManagement>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(materialCategory))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "MaterialCategory";
                    ce.ExpValue = "%" + materialCategory + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(productName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProductName";
                    ce.ExpValue = "%" + productName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(specification))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Specification";
                    ce.ExpValue = "%" + specification + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(supplierName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "SupplierName";
                    ce.ExpValue = "%" + supplierName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(supplierAddress))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "SupplierAddress";
                    ce.ExpValue = "%" + supplierAddress + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (state > -1)
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = state;
                    ce.ExpLogical = eLogicalOperator.And;
                    ce.ExpOperater = eConditionOperator.Equal;
                    qc.ConditionList.Add(ce);
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion
                result = proxy.GetTzSupMatManagementList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            var pathUrl = ConfigurationManager.AppSettings["PathUrl"]; ;
            ViewBag.PathUrl = pathUrl;
            return View(result.Data);
        }
        /// <summary>
        /// 甲供物资添加页面
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 甲供物资添加方法
        /// </summary>
        /// <param name="model">甲供物资相关信息对象</param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Add)]
        public ActionResult Add(Epm_TzSupMatManagement model)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzSupMatManagement(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="ids">编辑的ids字符串</param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Enable)]
        public ActionResult UpdateState(string ids, int state)
        {
            Result<int> result = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzSupMatManagementState(idList, state);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 保存并读取物资文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Import)]
        public ActionResult SaveAndReadFile()
        {
            Result<int> result = new Result<int>();
            List<int> list = new List<int>();
            try
            {
                List<Epm_TzSupMatManagement> supList = new List<Epm_TzSupMatManagement>();

                HttpPostedFileBase file = Request.Files[0];
                var fileName = file.FileName;
                //判断目录是否存在,不存在创建
                if (!Directory.Exists(ImportOrExportPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ImportOrExportPath);
                    directoryInfo.Create();
                }
                //将文件重新命名，保证唯一性
                string pathFile = ImportOrExportPath + "success_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                //将上传的文件保存
                file.SaveAs(pathFile);
                //方式一：创建数据表
                DataTable dt = ExcelHelperNew.ExcelToTable(pathFile);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //供应商名称、电话、联系人、地址
                    string supplierName = dt.Rows[i]["供应商名称*"].ToString();
                    if (string.IsNullOrEmpty(supplierName))
                    {
                        list.Add(i);
                        continue;
                    }
                    string supplierTel = dt.Rows[i]["供应商电话"].ToString();
                    string supplierContacts = dt.Rows[i]["供应商联系人"].ToString();
                    string supplierAddress = dt.Rows[i]["供应商地址*"].ToString();
                    if (string.IsNullOrEmpty(supplierAddress))
                    {
                        list.Add(i);
                        continue;
                    }
                    //甲供物资种类、规格、品名、单价
                    string materialCategory = dt.Rows[i]["物资种类*"].ToString();
                    if (string.IsNullOrEmpty(materialCategory))
                    {
                        list.Add(i);
                        continue;
                    }
                    string specification = dt.Rows[i]["规格*"].ToString();
                    if (string.IsNullOrEmpty(specification))
                    {
                        list.Add(i);
                        continue;
                    }
                    string productName = dt.Rows[i]["品名*"].ToString();
                    if (string.IsNullOrEmpty(productName))
                    {
                        list.Add(i);
                        continue;
                    }
                    string unitePrice = dt.Rows[i]["单价*"].ToString();
                    if (string.IsNullOrEmpty(unitePrice))
                    {
                        list.Add(i);
                        continue;
                    }
                    //数据转换model
                    Epm_TzSupMatManagement model = new Epm_TzSupMatManagement();
                    model.SupplierName = supplierName;
                    model.SupplierTel = supplierTel;
                    model.SupplierContacts = supplierContacts;
                    model.SupplierAddress = supplierAddress;
                    model.MaterialCategory = materialCategory;
                    model.Specification = specification;
                    model.ProductName = productName;
                    model.UnitePrice = unitePrice.ToDecimalReq();
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                    {
                        //根据供应商名称查询对应供应商，获取ID
                        Result<Base_Company> resultModel = proxy.GetCompanyByName(supplierName);
                        //关联供应商ID
                        if (resultModel.Flag == EResultFlag.Success && resultModel.Data != null)
                        {
                            var supplierId = resultModel.Data.Id;

                            //批量做重复物资判断(同一供应商、同一物资、同一规格、同一品名的物资只能存在一条数据)
                            var modelInfo = proxy.GetTzSupMatManagementModelBy(supplierId, model.MaterialCategory, model.ProductName, model.Specification);
                            if (modelInfo.Flag == EResultFlag.Success && modelInfo.Data != null)
                            {
                                list.Add(i);
                                continue;
                            }
                            else {
                                model.SupplierId = supplierId;
                                supList.Add(model);
                            }
                        }
                        else {
                            list.Add(i);
                            continue;
                        }
                    }
                }

                if (supList.Count > 0)
                {
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                    {
                        var rows = proxy.AddRangeTzSupMatManagement(supList);

                        result.Data = rows.Data;
                        result.Flag = EResultFlag.Success;
                    }
                }
                string errorPath = "";
                //导入数据异常时，记录错误信息进行导出，方便客户下次修改，再次执行导入。
                if (list.Count > 0)
                {
                    //待导出文件路径,将文件重新命名，保证唯一性
                    errorPath = ImportOrExportPath + "failure_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                    //导出失败的Excel文档
                    ExcelHelperNew.TableToExcel(dt, errorPath, list);
                }

                return Json(new
                {
                    Data = result.Data,
                    Flag = result.Flag,
                    failureCount = list.Count,//导入失败条数
                    successCount = dt.Rows.Count - list.Count,//导入成功条数
                    download = errorPath,//失败文件下载链接
                    fileName = Path.GetFileName(errorPath)
                });

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "导入程序异常:" + ex.Message);

            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 文档导入导出路径
        /// </summary>
        private static string ImportOrExportPath
        {
            get
            {
                string value = ConfigurationManager.AppSettings["ImportOrExportPath"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置文档导入导出路径！");
                }
                value = string.Format("{0}{1}\\{2}\\{3}\\", value, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                return value;
            }
        }
    }
}