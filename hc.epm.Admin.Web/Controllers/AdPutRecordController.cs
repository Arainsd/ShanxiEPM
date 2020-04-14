using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class AdPutRecordController : BaseController
    {
        /// <summary>
        /// 广告投放列表
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Browse)]
        public ActionResult Index(string AdName = "", string AdTargetName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.AdName = AdTargetName;
            ViewBag.AdName = AdName;
            ViewBag.pageIndex = pageIndex;

            //广告位下拉框数据
            Result<List<Epm_AdTarget>> list = new Result<List<Epm_AdTarget>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                list = proxy.GetAdTargetList();
            }
            ViewBag.AdTargetList = list.Data;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;

            if (!string.IsNullOrEmpty(AdName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "AdName";
                ce.ExpValue = "%" + AdName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(AdTargetName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "AdTargetName";
                ce.ExpValue = "%" + AdTargetName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            SortExpression sort = new SortExpression("Sort", eSortType.Asc);
            qc.SortList.Add(sort);

            Result<List<Epm_AdPutRecord>> result = new Result<List<Epm_AdPutRecord>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetAdPutRecordList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 添加广告投放
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> typeList = new List<DictionaryType>() { DictionaryType.AdType };
                var types = proxy.GetTypeListByTypes(typeList).Data;

                //返回新闻分类列表            
                ViewBag.SelAdTypeName = types[DictionaryType.AdType].ToSelectList("Name", "Id", true);
            }

            //广告位下拉框数据
            Result<List<Epm_AdTarget>> list = new Result<List<Epm_AdTarget>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                list = proxy.GetAdTargetList();
            }
            ViewBag.AdTargetList = list.Data;

            return View();
        }

        /// <summary>
        /// 添加广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_AdPutRecord model)
        {
            ResultView<int> view = new ResultView<int>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            if (string.IsNullOrEmpty(model.AdName))
            {
                view.Flag = false;
                view.Message = "广告名称不能为空";
                return Json(view);
            }
            if (model.StartTime>model.EndTime)
            {
                view.Flag = false;
                view.Message = "开始时间不能大于结束时间";
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
                model.EndTime = Convert.ToDateTime(model.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                result = proxy.AddAdPutRecord(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改广告投放
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            //广告位类型下拉框数据
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> typeList = new List<DictionaryType>() { DictionaryType.AdType };
                var types = proxy.GetTypeListByTypes(typeList).Data;
                //返回新闻分类列表            
                ViewBag.SelAdTypeName = types[DictionaryType.AdType].ToSelectList("Name", "No", true);
            }

            //广告位下拉框数据
            Result<List<Epm_AdTarget>> list = new Result<List<Epm_AdTarget>>();
            Result<Epm_AdPutRecord> adList = new Result<Epm_AdPutRecord>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                list = proxy.GetAdTargetList();
                adList = proxy.GetAdPutRecordById(id);
            }
            ViewBag.AdTargetList = list.Data;

            return View(adList.Data);
        }

        /// <summary>
        /// 修改广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_AdPutRecord model)
        {
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.AdName))
            {
                view.Flag = false;
                view.Message = "广告名称不能为空";
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
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                model.EndTime = Convert.ToDateTime(model.EndTime.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                result = proxy.UpdateAdPutRecord(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除广告投放
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteAdPutRecordByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改广告投放状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Ad, Right = SystemRight.Enable)] 
        [HttpPost]
        public ActionResult ChangeState(long id, int state)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeAdPutRecordState(id, state);
            }
            return Json(result.ToResultView());
        }
    }
}