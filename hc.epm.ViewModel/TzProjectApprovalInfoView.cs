using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzProjectApprovalInfoView
    {
        public TzProjectApprovalInfoView()
        {
            TzProjectApprovalInfo = new Epm_TzProjectApprovalInfo();
            TzProjectProposal = new Epm_TzProjectProposal();
        }

        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        public Epm_TzProjectApprovalInfo TzProjectApprovalInfo { get; set; }

        public string StationIds { get; set; }

        public string CompanyIds { get; set; }
    }
}
