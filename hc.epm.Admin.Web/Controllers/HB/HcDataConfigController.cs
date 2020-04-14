using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;

namespace hc.epm.Admin.Web.Controllers
{
    public class HcDataConfigController : BaseController
    {
        public ActionResult Index(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "Name";
            ce.ExpValue = "%" + name + "%";
            ce.ExpOperater = eConditionOperator.Like;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_DataConfig>> result = new Result<List<Epm_DataConfig>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDataConfigListWhr(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        //添加资料
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Epm_DataConfig model)
        {
            ResultView<int> view = new ResultView<int>();

            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "资料名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "资料编号不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                view.Flag = false;
                view.Message = "资料说明不能为空";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddDataConfig(model, fileList);
            }
            return Json(result.ToResultView());
        }

        //修改资料
        public ActionResult Edit(long id)
        {
            Result<Epm_DataConfig> result = new Result<Epm_DataConfig>();
            Result<List<Base_Files>> list = new Result<List<Base_Files>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDataConfigById(id);
            }
            return View(result.Data);
        }
        [HttpPost]
        public ActionResult Edit(Epm_DataConfig model)
        {
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "资料名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "资料编号不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                view.Flag = false;
                view.Message = "资料说明不能为空";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            Result<Epm_DataConfig> companyResult = new Result<Epm_DataConfig>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDataConfig(model, fileList);
            }
            return Json(result.ToResultView());
        }

        //删除
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleMilestoneIds(list);
            }
            return Json(result.ToResultView());
        }

        [HttpPost]
        public ActionResult ChangeState(long id, bool state, int type)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeDataConfigState(id, state, type);
            }
            return Json(result.ToResultView());
        }
    }
}