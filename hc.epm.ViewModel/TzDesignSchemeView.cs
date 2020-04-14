using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzDesignSchemeView
    {
        public TzDesignSchemeView()
        {
            TzDesignScheme=new Epm_TzDesignScheme();
        TzProjectProposal = new Epm_TzProjectProposal();
    }
        public Epm_TzDesignScheme TzDesignScheme { get; set; }

        public Epm_TzProjectProposal TzProjectProposal { get; set; }
    }
    
}
