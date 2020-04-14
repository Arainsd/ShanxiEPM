using hc.epm.Common;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ProjectKPITimer
{
    public class ProjectKPITimer
    {
        /// <summary>
        /// 项目KPI
        /// </summary>
        public void UpdateProjectKPI()
        {
            string year = DateTime.Now.Year.ToString();
            DateTime stime = Convert.ToDateTime(year + "-01-01");
            DateTime etime = Convert.ToDateTime((Convert.ToInt32(year) + 1).ToString() + "-01-01");
            var ProjectList = DataOperateBusiness<Epm_Project>.Get().GetList(t => !t.IsDelete && t.PlanWorkStartTime >= stime && t.PlanWorkStartTime < etime).ToList();

            var kpi = DataOperateBusiness<Epm_ProjectKPI>.Get().GetList(t => t.Years == year).FirstOrDefault();
            if (ProjectList.Count > 0)
            {
                int delayNum = 0;
                DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                var delayList = ProjectList.Where(t => t.PlanWorkEndTime <= nowTime).ToList();
                if (delayList.Count > 0)
                {
                    foreach (var item in delayList)
                    {
                        var planList = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.IsFinish != 1 && t.ProjectId == item.Id).ToList();
                        if (planList.Count > 0)
                        {
                            delayNum = delayNum + 1;
                        }
                    }
                }
                if (kpi != null)//更新
                {
                    kpi.TotelNum = ProjectList.Count();
                    kpi.ConstrunctionNum = ProjectList.Where(t => t.State == (int)ProjectState.Construction).Count();
                    kpi.FinishNum = ProjectList.Where(t => t.State == (int)ProjectState.Success).Count();
                    kpi.DelayNum = delayNum;
                    kpi.OperateTime = DateTime.Now;
                    kpi.OperateUserId = 0;
                    kpi.OperateUserName = "系统同步";
                    DataOperateBusiness<Epm_ProjectKPI>.Get().Update(kpi);
                }
                else//添加
                {
                    kpi = new Epm_ProjectKPI();
                    kpi.TotelNum = ProjectList.Count();
                    kpi.ConstrunctionNum = ProjectList.Where(t => t.State == (int)ProjectState.Construction).Count();
                    kpi.FinishNum = ProjectList.Where(t => t.State == (int)ProjectState.Success).Count();
                    kpi.DelayNum = delayNum;
                    kpi.Years = year;
                    kpi.IsDelete = false;
                    kpi.CreateTime = DateTime.Now;
                    kpi.CreateUserId = 0;
                    kpi.CreateUserName = "系统同步";
                    kpi.OperateTime = DateTime.Now;
                    kpi.OperateUserId = 0;
                    kpi.OperateUserName = "系统同步";
                    DataOperateBusiness<Epm_ProjectKPI>.Get().Add(kpi);
                }
            }
        }
    }
}
