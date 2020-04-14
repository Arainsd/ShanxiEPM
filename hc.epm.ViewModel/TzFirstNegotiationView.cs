using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzFirstNegotiationView
    {
        public TzFirstNegotiationView()
        {
            TzFirstNegotiation = new Epm_TzFirstNegotiation();
            TzProjectProposal = new Epm_TzProjectProposal();
        }

        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        public Epm_TzFirstNegotiation TzFirstNegotiation { get; set; }
    }
}
