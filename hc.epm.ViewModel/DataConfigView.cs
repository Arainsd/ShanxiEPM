using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class DataConfigView 
    {
        public Epm_DataConfig DataConfig { get; set; }

        ///<summary>
		///里程碑表Id
		///</summary>
		public long? MilepostId { get; set; }

        ///<summary>
        ///里程碑名称
        ///</summary>
        public string MilepostName { get; set; }
    }
}
