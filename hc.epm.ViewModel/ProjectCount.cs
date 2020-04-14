using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectCount
    {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 未开工项目数
        /// </summary>
        public int NoStartCount { get; set; }
        /// <summary>
        /// 开工项目数
        /// </summary>
        public int StartCount { get; set; }
        /// <summary>
        /// 完工项目数
        /// </summary>
        public int FinshCount { get; set; }
        /// <summary>
        /// 验收项目数
        /// </summary>
        public int AcceptanceCount { get; set; }
        /// <summary>
        /// 正在施工工地项目数
        /// </summary>
        public int ConstructionCount { get; set; }
        /// <summary>
        /// 投运项目数
        /// </summary>
        public int CommissioningCount { get; set; }

        public int sort { get; set; }


        //已完成施工图设计的项目
        public int DesignSchemeCount { get; set; }
        //省公司验收项目
        public int CompletionAcceptanceCount { get; set; }

        //已完成招标的项目
        public int TenderingApplyCount { get; set; }

        //转资项目
        public int CapitalTransferCount { get; set; }

        //投运项目
        public int ProjectPolitCount { get; set; }

        //新建
        public int BeingBuiltCount { get; set; }


    }
}
