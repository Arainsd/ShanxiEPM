using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class NewProjectCompanyContractView
    {
        public Epm_ProjectCompany Epm_ProjectCompany { get; set; }
        public string ContractCode { get; set; }
        public DateTime? ContractStartTime { get; set; }
        public DateTime? ContractEndTime { get; set; }
        public DateTime? ContractSignTime { get; set; }
        public decimal? ContractAmount { get; set; }
        
        public string OrderCode { get; set; }
        public DateTime? OrderStartTime { get; set; }
        public DateTime? OrderEndTime { get; set; }
        public DateTime? OrderSignTime { get; set; }
        public decimal? OrderAmount { get; set; }
    }
}
