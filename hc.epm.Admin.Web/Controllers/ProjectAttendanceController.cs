using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    /// <summary>
    /// 考勤设置
    /// </summary>
    public class ProjectAttendanceController : BaseController
    {
        /// <summary>
        /// 考勤设置列表
        /// </summary>
        /// <returns></returns>
        // GET: ProjectAttendance
        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            QueryCondition qc = new QueryCondition();

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_ProjectAttendance>> result = new Result<List<Epm_ProjectAttendance>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectAttendanceList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 添加考勤设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            DateTime time = DateTime.Now;
            var h = time.ToShortTimeString();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.PostType };
                var diclist = proxy.GetTypeListByTypes(dic).Data;
                //岗位
                var list = diclist[DictionaryType.PostType].Where(t => t.CreateUserName == "admin").ToList();
                ViewBag.list = list;
            }

            Result<AttendanceView> result = new Result<AttendanceView>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetAttendanceModel();
            }

            return View(result.Data);
        }

        [HttpPost]
        public ActionResult Add(ProjectAttendanceView model)
        {
            //表单校验
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddProjectAttendance(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改考勤设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            return View();
        }
    }
}