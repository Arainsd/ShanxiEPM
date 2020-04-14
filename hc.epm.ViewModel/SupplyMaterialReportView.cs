using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class SupplyMaterialReportView
    {
        ///<summary>
		///项目名称
		///</summary>
		public string ProjectName { get; set; }

        ///<summary>
		///所属项目ID
		///</summary>
		public long? ProjectId { get; set; }

        ///<summary>
		///分公司ID
		///</summary>
		public long? CompanyId { get; set; }
        
        ///<summary>
        ///分公司名称
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
		///库站ID
		///</summary>
		public long? StationId { get; set; }

        ///<summary>
		///库站名称
		///</summary>
		public string StationName { get; set; }

        /// <summary>
        /// 采购数量
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// 验收数量
        /// </summary>
        public int? AcceptNumber { get; set; }

        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// 供应商数量
        /// </summary>
        public int? CompanyNumber { get; set; }
    }
}
