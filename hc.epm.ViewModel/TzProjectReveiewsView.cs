using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzProjectReveiewsView
    {
        public TzProjectReveiewsView()
        {
            TzProjectReveiews = new Epm_TzProjectReveiews();
            TzProjectProposal = new Epm_TzProjectProposal();
        }

        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        public Epm_TzProjectReveiews TzProjectReveiews { get; set; }
    }
}
