using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ConstituteView 
    {
        public Epm_Constitute Constitute { get; set; }

        /// <summary>
        /// 明细数据
        /// </summary>
        public List<Epm_WorkMainPoints> WorkMainPoints { get; set; }
    }
}
