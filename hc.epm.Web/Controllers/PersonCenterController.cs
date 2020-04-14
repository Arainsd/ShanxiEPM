using hc.epm.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class PersonCenterController : BaseController
    {
        // GET: PersonCenter
        public ActionResult ToDoList()
        {
            //我的待办
            return View();
        }

        public ActionResult ProblemList()
        {
            //我的问题
            return View();
        }

        public ActionResult UpdatePass()
        {
            //修改密码
            return View();
        }

        public ActionResult PersonInfo()
        {
            //个人信息
            return View();
        }
    }
}