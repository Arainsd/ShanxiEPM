using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class SupervisorLogStatisticView
    {
        //姓名
        public string pm { get; set; }
        public string linkMan { get; set; }
        ///<summary>
        /// 项目
        ///</summary>
        public string projectName { get; set; }
        public long projectId { get; set; }
        
        ///<summary>
        /// 供应商
        ///</summary>
        public string companyName { get; set; }

        ///<summary>
        /// 入场天数
        ///</summary>
        public string limit { get; set; }

        /// <summary>
        /// 实际天数
        /// </summary>
        public string ActualDays { get; set; }

        ///<summary>
        /// 提交时间
        ///</summary>
        public string submitTime { get; set; }

        public string CreateTime { get; set; }

        /// <summary>
        /// 工程类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 分公司
        /// </summary>
        public string crtCompanyName { get; set; }

    }
}
