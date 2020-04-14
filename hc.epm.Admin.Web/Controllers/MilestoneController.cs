using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
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
    public class MilestoneController : BaseController
    {
        // GET: Test
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Browse)]
        public ActionResult Index(long parentId = 0, int pageIndex = 1, int pageSize = 10)
        {
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostViewList(parentId, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }

            SetMilestionCategory(true, parentId.ToString());

            return View(result.Data);
        }

        private void SetMilestionCategory(bool isDefault, string category = "")
        {
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = new ConditionExpression()
                {
                    ExpName = "ParentId",
                    ExpValue = 0,
                    ExpOperater = eConditionOperator.Equal
                };
                qc.ConditionList.Add(ce);
                qc.PageInfo.isAllowPage = false;
                qc.SortList.Add(new SortExpression("Code", eSortType.Asc));
                Result<List<Epm_Milepost>> data = proxy.GetMilepostListQc(qc);

                if (!isDefault && category == "")
                {
                    category = data.Data[0].Id.ToString();
                }
                ViewBag.ParentName = data.Data.ToSelectList("Name", "Id", isDefault, category);
            }
        }

        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            SetMilestionCategory(false);
            return View();
        }

        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataConfigId"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_Milepost model, string dataConfigId)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.ParentId.ToString()))
            {
                view.Flag = false;
                view.Message = "里程碑分类不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
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
                List<long> list = dataConfigId.SplitString(",").ToLongList();
                result = proxy.AddMilepostAndData(model, list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_Milepost> result = new Result<Epm_Milepost>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostById(id);
            }

            SetMilestionCategory(false, id.ToString());

            return View(result.Data);
        }

        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataConfigID"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_Milepost model, string dataConfigID)
        {
            List<long> list = dataConfigID.SplitString(",").ToLongList();
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.ParentId.ToString()))
            {
                view.Flag = false;
                view.Message = "里程碑分类不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
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
            Result<Epm_Milepost> companyResult = new Result<Epm_Milepost>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepost(model, list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除里程碑
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteMilepostbyIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改里程碑状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        //[AuthCheck(Module = AdminModule.MilepostConfig, Right = SystemRight.Enable)]
        [HttpPost]
        public ActionResult ChangeState(long id, bool state)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMilepostState(id, state);
            }
            return Json(result.ToResultView());
        }
    }
}