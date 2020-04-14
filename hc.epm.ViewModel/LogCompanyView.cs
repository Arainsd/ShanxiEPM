using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class LogCompanyView
    {
        /// <summary>
        /// 单位ID
        /// </summary>
        public long? id { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 到场施工人员信息
        /// </summary>

        public List<PeoplesView> personnelList { get; set; }
    }
}
