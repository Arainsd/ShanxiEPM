using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectCompanyContractView
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public long? CompanyId { get; set; }
        public string CompanyName { get; set; }
        ///<summary>
        ///单位类型
        ///</summary>
        public string Type { get; set; }

        public string IsContractAdd { get; set; }
        public long? ContractId { get; set; }
        public string ContractName { get; set; }
        public string ContractCode { get; set; }
        public string ContractFiles { get; set; }
        public DateTime? ContractStartTime { get; set; }
        public DateTime? ContractEndTime { get; set; }
        public DateTime? ContractSignTime { get; set; }
        public decimal? ContractAmount { get; set; }

        public string IsOrderAdd { get; set; }
        public long? OrderId { get; set; }
        public string OrderName { get; set; }
        public string OrderCode { get; set; }
        public string OrderFiles { get; set; }
        public DateTime? OrderStartTime { get; set; }
        public DateTime? OrderEndTime { get; set; }
        public DateTime? OrderSignTime { get; set; }
        public decimal? OrderAmount { get; set; }
    }
}
