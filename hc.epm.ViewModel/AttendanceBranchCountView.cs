using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class AttendanceBranchCountView
    {
        //分公司Id
        public string PId { get; set; }
        //分公司名字
        public string PreName { get; set; }

        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        //项目总数
        public int ProjectCount { get; set; }
        //总考勤人数
        public int AttendanceCount { get; set; }
        //规定打卡次数
        public int RegulationsCount { get; set; }
        //实际打卡次数
        public int ActualCount { get; set; }
        //考勤率
        public string AttendanceRate { get; set; }
        //打卡时间
        public string AttendanceTime { get; set; }
        public int sort { get; set; }

        public string dateTime { get; set; }

        public string Name { get; set; }
        public string userName { get; set; }
        public long? Id { get; set; }
        public string projectId { get; set; }
        public string userID { get; set; }
    }
}
