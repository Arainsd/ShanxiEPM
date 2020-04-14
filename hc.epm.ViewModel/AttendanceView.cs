using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public  class AttendanceView
    {
        /// <summary>
        /// 考勤人员类型(多个用,隔开)
        /// </summary>
        public List<string> AttendanceList { get; set; }

        ///<summary>
        ///误差时间（分钟）
        ///</summary>
        public int? MarginError { get; set; }

        ///<summary>
        ///考勤次数
        ///</summary>
        public int? Num { get; set; }

        ///<summary>
		///考勤时间
		///</summary>
		public List<string> AttendanceTimeList { get; set; }
    }
}
