using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MonitorView
    {
        public MonitorView()
        {
            MonitorDetails = new List<Epm_MonitorDetails>();
            Monitor = new Epm_Monitor();
            MonitorRectifRecord = new Epm_MonitorRectifRecord();
            MonitorDetailBIM = new List<Epm_MonitorDetailBIM>();
            FileList = new List<Base_Files>();
        }
        public Epm_Monitor Monitor { get; set; }
        /// <summary>
        /// 明细数据
        /// </summary>
        public List<Epm_MonitorDetails> MonitorDetails { get; set; }

        public Epm_MonitorRectifRecord MonitorRectifRecord { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Base_Files> FileList { get; set; }

        /// <summary>
        /// 检查关联组件表
        /// </summary>
        public List<Epm_MonitorDetailBIM> MonitorDetailBIM { get; set; }
    }
}
