using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class TzStartTenderingAndSupplyView
    {
        //开工申请
        public Epm_TzProjectStartApply TzProjectStartApply { get; set; }
        ////工期和手续
        public Epm_TimeLimitAndProcedure timeAndCrossings { get; set; }


        public TzStartTenderingAndSupplyView()
        {
            TzProjectStartApply = new Epm_TzProjectStartApply();
            timeAndCrossings = new Epm_TimeLimitAndProcedure();
        }

    }
}
