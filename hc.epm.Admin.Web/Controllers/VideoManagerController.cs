using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class VideoManagerController : BaseController
    {
        // GET: VideoManager
        public ActionResult Index(string projectName = "", string CameraName = "", string CameraState = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.CameraName = CameraName;
            ViewBag.CameraState = CameraState;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(projectName))
            {
                ConditionExpression ce1 = new ConditionExpression();
                ce1.ExpName = "projectName";
                ce1.ExpValue = "%" + projectName + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            if (!string.IsNullOrEmpty(CameraName))
            {
                ConditionExpression ce1 = new ConditionExpression();
                ce1.ExpName = "CameraName";
                ce1.ExpValue = "%" + CameraName + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            if (!string.IsNullOrEmpty(CameraState))
            {
                ConditionExpression ce1 = new ConditionExpression();
                ce1.ExpName = "CameraState";
                ce1.ExpValue = "%" + CameraState + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetVideoManageList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        public ActionResult Add()
        {

            return View();
        }
        /// <summary>
        /// 设备新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Base_VideoManage mode)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Add.ToString(), true);
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                proxy.InnerChannel.OperationTimeout = Convert.ToDateTime("00:02:00").TimeOfDay;
                result = proxy.AddVideoManage(mode);
            }
            return Json(result.ToResultView());
        }
        public ActionResult Edit(long id)
        {
            Result<Base_VideoManage> result = new Result<Base_VideoManage>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBaseVideoManageById(id);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 修改摄像设备信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Base_VideoManage model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Modify.ToString(), true);

            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.UrlAddress))
            {
                view.Flag = false;
                view.Message = "视频源链接不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.CameraName))
            {
                view.Flag = false;
                view.Message = "设备名称不能为空";
                return Json(view);
            }

            Result<Base_VideoManage> BaseVideoManage = new Result<Base_VideoManage>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                BaseVideoManage = proxy.GetBaseVideoManages(model.Id);
                BaseVideoManage.Data.CameraName = model.CameraName;
                BaseVideoManage.Data.Companyname = model.Companyname;
                BaseVideoManage.Data.CompanyPerson = model.CompanyPerson;
                BaseVideoManage.Data.DescribeInfo = model.DescribeInfo;
                BaseVideoManage.Data.UrlAddress = model.UrlAddress;
                BaseVideoManage.Data.VerificationCode = model.VerificationCode;
                BaseVideoManage.Data.ProjectName = model.ProjectName;
                BaseVideoManage.Data.DeviceSequence = model.DeviceSequence;
                result = proxy.UpdateBaseVideoManage(model);
            }
            return Json(result.ToResultView());
        }
        public ActionResult Detail(long id)
        {
            Result<Base_VideoManage> result = new Result<Base_VideoManage>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetBaseVideoManageById(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 查询有效项目
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexProject(string name = "", string CreateUserName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.CreateUserName = CreateUserName;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(name))
            {
                ConditionExpression ce1 = new ConditionExpression();
                ce1.ExpName = "Name";
                ce1.ExpValue = "%" + name + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            if (!string.IsNullOrEmpty(CreateUserName))
            {
                ConditionExpression ce1 = new ConditionExpression();
                ce1.ExpName = "CreateUserName";
                ce1.ExpValue = "%" + CreateUserName + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetIndexProject(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 删除摄像设备
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="belong"></param>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Delete.ToString(), true);

            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteBaseVideoManage(list);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 激活设备
        /// </summary>
        /// <param name="id">设备 ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Activated(string id)
        {
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrWhiteSpace(id))
            {
                view.Flag = false;
                view.Message = "请选择要激活的设备！";
                return Json(view);
            }
            long idValue = 0;
            if(!long.TryParse(id, out idValue))
            {
                view.Flag = false;
                view.Message = "激活设备操作失败！";
                return Json(view);
            }

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var result = proxy.ActivatedVideo(idValue);
                return View(result.ToResultView());
            }
        }

    }
}