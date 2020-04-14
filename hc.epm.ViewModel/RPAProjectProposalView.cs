using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class RPAProjectProposalView
    {
        public RPAProjectProposalView()
        {
            rPA_ProjectProposal = new RPA_ProjectProposal();
            oMADS_ProjectProposal = new OMADS_ProjectProposal();
            tEMP_ProjectProposal = new TEMP_ProjectProposal();
        }
        public RPA_ProjectProposal rPA_ProjectProposal { get; set; }
        public OMADS_ProjectProposal oMADS_ProjectProposal  { get; set; }
        public TEMP_ProjectProposal tEMP_ProjectProposal { get; set; }
    }
}
