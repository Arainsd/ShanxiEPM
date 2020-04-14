using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectAttendanceView
    {
        /// <summary>
        /// 考勤人员类型(多个用|隔开)
        /// </summary>
        public string Attendance { get; set; }

        ///<summary>
		///考勤时间(多个用|隔开)
		///</summary>
		public string AttendanceTime { get; set; }

        ///<summary>
		///误差时间（分钟）
		///</summary>
		public int? MarginError { get; set; }
    }
}
