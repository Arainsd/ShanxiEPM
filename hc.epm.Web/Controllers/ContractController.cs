using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.UI.Common;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Business;
using hc.epm.Web.ClientProxy;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Basic;
using Newtonsoft.Json;
using hc.epm.Common;
using System.Configuration;
using System.Diagnostics;

using Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace hc.epm.Web.Controllers
{
    public class ContractController : BaseWebController
    {
        private static Process process = new Process();
        string outputDirPath = @"D:\文件\";
        /// <summary>
        /// 合同管理列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string contractType = "", string name = "", string startTime = "", string endTime = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.state = typeof(ApprovalState).AsSelectList(true, state, new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            int type = 0;
            if (contractType == ContractType.Contract.ToString())
            {
                type = 1;
            }
            else if (contractType == ContractType.FrameContract.ToString())
            {
                type = 2;
            }
            else if (contractType == ContractType.Order.ToString())
            {
                type = 3;
            }

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            if (type == (int)ContractType.Order)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = 0,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
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
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "name",
                    ExpValue = "%" + name + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "StartTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.GreaterThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                DateTime stime = Convert.ToDateTime(endTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "EndTime",
                    ExpValue = stime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                var approvalState = state.ToEnumReq<ApprovalState>();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = int.Parse(approvalState.GetValue().ToString()),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            if (type > 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ContractType",
                    ExpValue = type,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }

            if (base.CurrentUser.CompanyType != "Owner")//第三方单位只可查看 合同乙方相关内容
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SecondPartyId",
                    ExpValue = base.CurrentUser.CompanyId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }

            Result<List<Epm_Contract>> result = new Result<List<Epm_Contract>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.ContractType = typeof(ContractType).AsSelectList(true);

            ViewBag.CurUserId = CurrentUser.UserId;

            return View(result.Data);
        }

        /// <summary>
        /// 添加合同
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];

            List<string> removeValues = new List<string>();
            removeValues.Add(ContractType.Order.ToString());
            ViewBag.ContractTypeName = typeof(ContractType).AsSelectList(true, "", removeValues);

            return View();
        }

        /// <summary>
        /// 添加合同（提交方法）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Add)]
        public ActionResult Add(Epm_Contract model)
        {
            ResultView<int> view = new ResultView<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            string contractTypeName = Request.Form["ContractTypeName"];
            if (contractTypeName == ContractType.Contract.ToString())
            {
                model.ContractType = 1;
            }
            else if (contractTypeName == ContractType.FrameContract.ToString())
            {
                model.ContractType = 2;
            }
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编码不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddContract(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改合同
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_Contract> result = new Result<Epm_Contract>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractModel(id);
            }
            int st = result.Data.ContractType.Value;
            List<string> removeValues = new List<string>();
            removeValues.Add(ContractType.Order.ToString());
            ViewBag.ContractTypeName = typeof(ContractType).AsSelectList(true, st == 0 ? "" : ((ContractType)st).ToString(), removeValues);

            return View(result.Data);
        }

        /// <summary>
        /// 修改合同（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Contract model)
        {
            ResultView<int> view = new ResultView<int>();

            //合同附件集合
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            string contractTypeName = Request.Form["ContractTypeName"];
            if (contractTypeName == ContractType.Contract.ToString())
            {
                model.ContractType = 1;
            }
            else if (contractTypeName == ContractType.FrameContract.ToString())
            {
                model.ContractType = 2;
            }

            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编码不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateContract(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看合同详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Epm_Contract> result = new Result<Epm_Contract>();
            ViewBag.Check = false;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractModel(id);

                var project = proxy.GetProject(result.Data.ProjectId.Value).Data;
                if (project != null && project.PMId == CurrentUser.UserId)
                {
                    ViewBag.Check = true;
                }
            }

            return View(result.Data);
        }

        /// <summary>
        /// 审核、驳回、废弃
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason">原因</param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Check)]
        public ActionResult UpdateState(long id, string state, string reason)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(state))
            {
                view.Flag = false;
                view.Message = "状态不能为空";
                return Json(view);
            }
            //判断权限
            //if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprSuccess)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.Contract.ToString(), SystemRight.Check.ToString(), true);
            //}
            //else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprFailure)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.Contract.ToString(), SystemRight.UnCheck.ToString(), true);
            //}
            //else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.Discarded)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.Contract.ToString(), SystemRight.Invalid.ToString(), true);
            //}


            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateContractState(id, state);
            }
            return Json(result.ToResultView());
        }


        [HttpPost]
        public ActionResult Delete(long id)
        {
            //ResultView<int> view = new ResultView<int>();
            //if (string.IsNullOrEmpty(state))
            //{
            //    view.Flag = false;
            //    view.Message = "状态不能为空";
            //    return Json(view);
            //}
            //ApprovalState app;
            //if (!Enum.TryParse(state, out app))
            //{
            //    view.Flag = false;
            //    view.Message = "状态值不正确";
            //    return Json(view);
            //}
            //if (app != ApprovalState.Enabled || app != ApprovalState.Discarded)
            //{
            //    view.Flag = false;
            //    view.Message = "草稿，已废弃状态下，才可删除";
            //    return Json(view);
            //}
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteContractByIds(new List<long> { id });
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 根据合同ID查询详细项目文件信息
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Contract, Right = SystemRight.Info)]
        public ActionResult PreviewContract(long id)
        {
            ViewBag.Id = id;
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractModelFile(id);
            }
            return View(result.Data);
        }
        #region 文件在线预览
        /// <summary>
        /// 文件在线预览
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public void Preview(string fileId, string fileNames,  long contractId)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetContractModelFileName(contractId, fileNames);
            }
            fileNames = System.IO.Path.GetFileName(result.Data[0].Url);
            string type = Path.GetExtension(fileNames);
            string path = ConfigurationManager.AppSettings["Previews"];
            string inFilePath = path + fileNames;
            //try
            //{
            //    DelectDir(path);
            //}
            //catch (Exception ex)
            //{ }

            if (type == ".pdf")
            {
                PreviewPDF(inFilePath);
            }
            else if (type == ".xlsx"|| type == ".xls")
            {
                PreviewEXL(inFilePath);
            }
            else if (type == ".docx" || type == ".doc")
            {
                PreviewWORD(inFilePath);
            }

        }
        /// <summary>
        /// 在线预览PDF
        /// </summary>
        /// <returns></returns>
        public ActionResult PreviewPDF(string inFilePath)
        {
            if (string.IsNullOrWhiteSpace(inFilePath))
            {
                throw new Exception("未配置内网资源服务器预览地址！");
            }
            Response.ContentType = "application/pdf";
            string fileName = inFilePath.Substring(inFilePath.LastIndexOf('\\') + 1);
            Response.AddHeader("content-disposition", "filename=" + fileName);
            Response.WriteFile(inFilePath);
            Response.End();
            return View();
        }
        /// <summary>
        /// 在线预览EXL
        /// </summary>
        /// <param name="inFilePath"></param>
        /// <returns></returns>
        public ActionResult PreviewEXL(string inFilePath)
        {
            string outDirPath = ConfigurationManager.AppSettings["Previews"];
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook xls = null;
            excel = new Microsoft.Office.Interop.Excel.Application();
            object missing = Type.Missing;
            object trueObject = true;
            excel.Visible = false;
            excel.DisplayAlerts = false;


            string randomName = DateTime.Now.Ticks.ToString();  //output fileName


            xls = excel.Workbooks.Open(inFilePath, missing, trueObject, missing,
                                        missing, missing, missing, missing, missing, missing, missing, missing,
                                        missing, missing, missing);


            //Save Excel to Html
            object format = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
            Workbook wsCurrent = xls;
            String outputFile = outDirPath + randomName + ".html";
            wsCurrent.SaveAs(outputFile, format, missing, missing, missing,
                              missing, XlSaveAsAccessMode.xlNoChange, missing,
                              missing, missing, missing, missing);
            excel.Quit();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = outputFile;
            process.Start();
            return View();
        }
        public ActionResult PreviewWORD(string inFilePath)
        {
            string outDirPath = ConfigurationManager.AppSettings["Previews"];
            object missingType = Type.Missing;
            object readOnly = true;
            object isVisible = false;
            object documentFormat = 8;
            string randomName = DateTime.Now.Ticks.ToString();
            object htmlFilePath = outDirPath + randomName + ".htm";
            string directoryPath = outDirPath + randomName + ".files";

            object filePath = inFilePath;
            //Open the word document in background
            Microsoft.Office.Interop.Word.ApplicationClass applicationClass = new Microsoft.Office.Interop.Word.ApplicationClass();
            applicationClass.Documents.Open(ref filePath,
                                            ref readOnly,
                                            ref missingType, ref missingType, ref missingType,
                                            ref missingType, ref missingType, ref missingType,
                                            ref missingType, ref missingType, ref isVisible,
                                            ref missingType, ref missingType, ref missingType,
                                            ref missingType, ref missingType);
            applicationClass.Visible = false;
            Document document = applicationClass.ActiveDocument;

            //Save the word document as HTML file
            document.SaveAs(ref htmlFilePath, ref documentFormat, ref missingType,
                            ref missingType, ref missingType, ref missingType,
                            ref missingType, ref missingType, ref missingType,
                            ref missingType, ref missingType, ref missingType,
                            ref missingType, ref missingType, ref missingType,
                            ref missingType);
            applicationClass.Quit();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = htmlFilePath.ToString();
            process.Start();
            return View();
        }
        /// <summary>
        /// 删除指定路径下文件
        /// </summary>
        /// <param name="srcPath"></param>
        /// <returns></returns>
        public ActionResult DelectDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();//返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    System.IO.File.Delete(i.FullName);
                }
            }
            return View();
        }
        #endregion

        #region 下载文件
        /// <summary>
        /// 下载EXL文件
        /// </summary>
        /// <param name="inFilePath"></param>
        /// <returns></returns>
        public ActionResult PreviewEXLXZ(string inFilePath)
        {
            //string fileNames, string type
            inFilePath = ConfigurationManager.AppSettings["PreviewEXL"];
            if (string.IsNullOrWhiteSpace(inFilePath))
            {
                throw new Exception("未配置内网资源服务器预览地址！");
            }
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = inFilePath.Substring(inFilePath.LastIndexOf('\\') + 1);
            Response.AddHeader("content-disposition", "filename=" + fileName);
            Response.WriteFile(inFilePath);
            Response.End();
            return View();
        }
        #endregion


    }
}
