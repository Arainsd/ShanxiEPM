using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class WeeklyView
    {
        /// <summary>
        /// 项目汇总报表
        /// </summary>
        public List<Epm_ProjectCountWeekly> projectCounts {get;set;}
                    
        /// <summary>
        /// 项目汇总
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews { get; set; }
        /// <summary>
        /// 项目新建
        /// </summary>
        public List<Epm_ProjectCountWeekly> projectCounts1 { get; set; }
        /// <summary>
        /// 正在施工
        /// </summary>
        //public List<ProjectView> projectViews1 { get; set; }
        /// <summary>
        /// 项目新建
        /// </summary>
        public List<Epm_ProjectCountWeekly> projectCounts2 { get; set; }
        /// <summary>
        /// 新增项目汇总
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews7 { get; set; }
        /// <summary>
        /// 未完成设计
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews2 { get; set; }
        /// <summary>
        /// 完工未投运
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews8 { get; set; }
        /// <summary>
        /// 正在施工
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews4 { get; set; }
        /// <summary>
        /// 未完成招标
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews5 { get; set; }
        /// <summary>
        /// 在建
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews6 { get; set; }
        /// <summary>
        /// 改造 项目汇总表
        /// </summary>
        //public List<ProjectCount> projectCounts3 { get; set; }
        /// <summary>
        /// 改造项目汇总
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews11 { get; set; }
        /// <summary>
        /// 未开工
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews12 { get; set; }
        /// <summary>
        /// 完工未投运
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews13 { get; set; }
        /// <summary>
        /// 在建
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews14 { get; set; }
        /// <summary>
        /// 正在施工
        /// </summary>
        public List<Epm_ProjectWeekly> projectViews16 { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
    }
}
