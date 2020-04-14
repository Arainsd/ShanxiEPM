using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class TzTenderingCountView
    {
        public long Id { get; set; }
        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///加油站名称
        ///</summary>
        public string StationName { get; set; }

        ///<summary>
        ///项目性质名称
        ///</summary>
        public string NatureName { get; set; }
        /// <summary>
        /// 申请次数
        /// </summary>
        public int counts { get; set; }

        public DateTime? OperateTime { get; set; }
    }
}
