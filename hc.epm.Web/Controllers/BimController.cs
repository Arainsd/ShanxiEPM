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
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class BimController : BaseWebController
    {
        /// <summary>
        /// 查询模型列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string name = "", string startTime = "", string endTime = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.state = state;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + projectName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(startTime))
            {
                DateTime stime = Convert.ToDateTime(startTime);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SubmitDate",
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

                    ExpName = "SubmitDate",
                    ExpValue = etime,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.LessThanOrEqual
                });
            }
            if (!string.IsNullOrEmpty(state))
            {
                int statu = int.Parse(((ApprovalState)(Enum.Parse(typeof(ApprovalState), state))).GetValue().ToString());
                ce = new ConditionExpression();
                ce.ExpName = "State";
                ce.ExpValue = statu;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_Bim>> result = new Result<List<Epm_Bim>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            ViewBag.ApprovalState = typeof(ApprovalState).AsSelectList(true, "", new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });
            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            return View(result.Data);
        }

        /// <summary>
        /// 上传模型
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Draw };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.VersionNo = subjects[DictionaryType.Draw].ToSelectList("Name", "No", false, "FirstVersion");
                // new SelectList()
            }

            ViewBag.UserID = ApplicationContext.Current.UserID;
            ViewBag.UserName = ApplicationContext.Current.UserName;
            ViewBag.CompanyId = ApplicationContext.Current.CompanyId;
            ViewBag.CompanyName = ApplicationContext.Current.CompanyName;
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME] as string;

            return View();
        }

        /// <summary>
        /// 上传模型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Add)]
        public ActionResult Add(Epm_Bim model)
        {
            ResultView<int> view = new ResultView<int>();

            //上传模型
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            //if (string.IsNullOrEmpty(model.Name))
            //{
            //    view.Flag = false;
            //    view.Message = "名称不能为空";
            //    return Json(view);
            //}
            //if (string.IsNullOrEmpty(model.VersionOrder))
            //{
            //    view.Flag = false;
            //    view.Message = "版本号不能为空";
            //    return Json(view);
            //}

            if (fileListFile!=null)
            {
                model.BIMState = BIMModelState.BIMLightWeight.ToString();
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddBim(model, fileListFile);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改模型
        /// </summary>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_Bim> result = new Result<Epm_Bim>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimModel(id);

                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Draw };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.VersionNo = subjects[DictionaryType.Draw].ToSelectList("Name", "No", false, result.Data.VersionNo);

            }

            return View(result.Data);
        }

        /// <summary>
        /// 修改模型（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_Bim model)
        {
            ResultView<int> view = new ResultView<int>();

            //上传模型
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传图片json字符串
            List<Base_Files> fileListFile = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            //if (string.IsNullOrEmpty(model.Name))
            //{
            //    view.Flag = false;
            //    view.Message = "名称不能为空";
            //    return Json(view);
            //}
            //if (string.IsNullOrEmpty(model.VersionOrder))
            //{
            //    view.Flag = false;
            //    view.Message = "版本号不能为空";
            //    return Json(view);
            //}
            if (fileListFile != null)
            {
                model.BIMState = BIMModelState.BIMLightWeight.ToString();
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateBim(model, fileListFile);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看模型详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Epm_Bim> result = new Result<Epm_Bim>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 废弃模型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Invalid)]
        public ActionResult Archive(long id, string state)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeBimState(id, state);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 审核/驳回模型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Reject(long id, string state, string reason = "")
        {
            //权限检查
            //if ((ApprovalState)(Enum.Parse(typeof(ApprovalState), state)) == ApprovalState.ApprFailure)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.Bim.ToString(), SystemRight.UnCheck.ToString(), true);
            //}
            //else if ((ApprovalState)(Enum.Parse(typeof(ApprovalState), state)) == ApprovalState.ApprSuccess)
            //{
            //    Helper.IsCheck(HttpContext, WebModule.Bim.ToString(), SystemRight.Check.ToString(), true);
            //}
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.RejectBim(id, state, reason);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除（废弃/草稿）状态模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Delete)]
        public ActionResult Delete(string id)
        {
            Result<int> result = new Result<int>();
            List<long> list = id.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteBimByIds(list);
            }
            return Json(result.ToResultView());
        }
        public ActionResult BimModel(long id)
        {
            Result<Epm_Bim> result = new Result<Epm_Bim>();
            List<Epm_QuestionBIM> list = new List<Epm_QuestionBIM>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimModel(id);

                var qbList = proxy.GetComponentListByBimId(id).Data;
                if (qbList != null)
                {
                    Epm_QuestionBIM qm = null;
                    foreach (var item in qbList)
                    {
                        qm = new Epm_QuestionBIM();
                        qm.ComponentId = item.ComponentId; //"1036857218118586368_317829";
                        qm.ComponentPosition = item.ComponentPosition;// "3157.9013671875,-47.87890625,-298.1455078125,0.7853981633974483,0,0.7853981633974483,-14252.572887584003,-5468.046301073756,-5862.356906876444";
                        list.Add(qm);
                    }
                }
            }

            ViewBag.ProjectId = result.Data.ProjectId;
            ViewBag.ProjectName = result.Data.ProjectName;
            ViewBag.UserName = CurrentUser.RealName;

            ViewBag.ComponentJson = JsonConvert.SerializeObject(list);

            return View(result.Data);
        }

        /// <summary>
        /// 生成BIM图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateImg(long id, string img)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<Base_Files> fileList = SaveImg(img, id);
                result = proxy.CreateImgBim(id, img, fileList);
                result.Flag = EResultFlag.Success;
                result.Data = 0;
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// base64 转图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public List<Base_Files> SaveImg(string img, long id)
        {
            List<Base_Files> fileList = new List<Base_Files>();
            Base_Files fileBim = new Base_Files();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var fileResult = proxy.GetFilesByTable("Epm_Bim", id);
                if (fileResult.Data.Count > 0)
                {
                    var fileInfo = fileResult.Data.Where(t => t.TableColumn == "Thumbnail").ToList();
                    if (fileInfo.Count() == 0)
                    {
                        string thumbnailPath = ConfigurationManager.AppSettings["ThumbnailPath"];
                        if (!string.IsNullOrWhiteSpace(thumbnailPath))
                        {
                            if (!thumbnailPath.EndsWith("\\"))
                            {
                                thumbnailPath += "\\";
                            }
                            #region 拼接相对路径
                            //拼接相对路径
                            string RelativePath = thumbnailPath.Substring(0, thumbnailPath.Length - 1);
                            RelativePath = string.Format("{0}\\{1}\\{2}\\", RelativePath.Substring(RelativePath.LastIndexOf("\\") + 1), DateTime.Today.Year, DateTime.Today.Month);

                            // 缩略图存储文件夹按年月格式生成
                            thumbnailPath = string.Format("{0}{1}\\{2}\\", thumbnailPath, DateTime.Today.Year, DateTime.Today.Month);
                            if (!Directory.Exists(thumbnailPath))
                            {
                                Directory.CreateDirectory(thumbnailPath);
                            }
                            #endregion

                            #region base64 转图片并保存
                            img = img.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
                            byte[] bytes = Convert.FromBase64String(img);
                            MemoryStream memStream = new MemoryStream(bytes);
                            Image mImage = Image.FromStream(memStream);
                            Bitmap bp = new Bitmap(mImage);
                            MemoryStream ms = new MemoryStream();

                            string smallReName = Guid.NewGuid() + ".jpg";
                            bp.Save(thumbnailPath + smallReName, System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径

                            #endregion

                            //获取文件大小
                            var file = new FileInfo(thumbnailPath.Replace("\\", "/") + smallReName);
                            var size = GetLength(file.Length);

                            fileBim.Size = size;
                            fileBim.TableColumn = "Thumbnail";
                            fileBim.Url = RelativePath.Replace("\\", "/") + smallReName;
                            fileBim.Name = smallReName;
                            fileBim.ImageType = "small";
                            fileList.Add(fileBim);
                        }
                    }
                }
            }

            return fileList;
        }

        private string GetLength(long lengthOfDocument)
        {
            if (lengthOfDocument < 1024)
            {
                return string.Format(Math.Round((lengthOfDocument / 1.0), 2).ToString() + "B");
            }
            else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0), 2).ToString() + "KB");
            }
            else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0), 2).ToString() + "M");
            }
            else
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0 / 1024.0), 2).ToString() + "GB");
            }
        }

        /// <summary>
        /// 根据项目获取模型数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListByProjectId(long projectId)
        {
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (projectId != 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "projectId";
                ce.ExpValue = projectId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo.isAllowPage = false;

            Result<List<Epm_Bim>> result = new Result<List<Epm_Bim>>();
            int falg = 0;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBimList(qc);
                if (result.Data.Count > 0)
                {
                    if (result.Data.Where(t => t.VersionNo == "BlueprintVersion").Count() > 0)
                    {
                        falg = 2;
                    }
                    else if (result.Data.Where(t => t.VersionNo == "FirstVersion").Count() > 0)
                    {
                        falg = 1;
                    }
                }
            }
            return Json(falg);
        }

        /// <summary>
        /// 获取BIM构件属性
        /// </summary>
        /// <param name="bimId"></param>
        /// <param name="externalId"></param>
        /// <returns></returns>
        public ActionResult GetBimPropertyInfo(string bimId, string externalId)
        {
            string compId = externalId.Split('_')[1];
            string SQLString = "select id, externalId, propertyTypeName, propertySetName, propertyname, ifcurl, value, modelName, groupname from model_property where externalId = " + compId + " and value != ''";
            var resultJson = "";
            List<CustomPropertyView> customList = new List<CustomPropertyView>();

            Result<DataSet> result = new Result<DataSet>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 查询模型属性信息
                var bimInfo = proxy.GetBimModel(bimId.ToLongReq());

                if (bimInfo.Data != null)
                {
                    string path = bimInfo.Data.BIMDBPath;
                    result = proxy.GetBimProperty(path, SQLString);

                    if (result.Data != null)
                    {
                        if (result.Data.Tables.Count > 0)
                        {
                            resultJson = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data.Tables[0]);
                            customList = JsonConvert.DeserializeObject<List<CustomPropertyView>>(resultJson);
                        }
                    }
                }
                #endregion

                #region 查询模型自定义属性

                #region 条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;

                ce = new ConditionExpression();
                ce.ExpName = "BimId";
                ce.ExpValue = bimId.ToLongReq();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);

                ce = new ConditionExpression();
                ce.ExpName = "externalId";
                ce.ExpValue = compId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);

                qc.PageInfo.isAllowPage = false;
                #endregion

                var bimResult = proxy.GetCustomPropertyList(qc);
                if (bimResult.Data.Count > 0)
                {
                    CustomPropertyView view = null;
                    foreach (var item in bimResult.Data)
                    {
                        view = new CustomPropertyView();
                        view.id = item.Id.ToString();
                        view.externalId = item.externalId;
                        view.propertyTypeName = item.PropertyType;
                        view.propertySetName = "";
                        view.propertyname = item.PropertyKey;
                        view.ifcurl = "";
                        view.value = item.PropertyValue;
                        view.modelName = "";
                        view.groupname = "";
                        view.username = item.CreateUserName;
                        view.createdate = item.CreateTime.Value.ToString("yyyy-MM-dd");

                        customList.Add(view);
                    }
                }

                #endregion
            }
            return Json(customList);
        }

        /// <summary>
        /// 添加BIM自定义属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[AuthCheck(Module = WebModule.Bim, Right = SystemRight.Add)]
        public ActionResult AddCustom(string externalId, string propertyKey, string propertyValue)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var bim = proxy.GetBimModel(externalId.Split('_')[0].ToLongReq());

                EPM_CustomProperty model = new EPM_CustomProperty();
                model.externalId = externalId.Split('_')[1];
                model.BimId = externalId.Split('_')[0].ToLongReq();
                model.PropertyKey = propertyKey;
                model.PropertyValue = propertyValue;
                model.PropertyType = "custom";
                model.ProjectId = bim.Data.ProjectId;
                model.ProjectName = bim.Data.ProjectName;
                model.IsDelete = false;
                result = proxy.AddCustomProperty(model);
            }
            return Json(result.ToResultView());
        }
    }
}