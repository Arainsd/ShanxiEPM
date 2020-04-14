using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ReformRecordView
    {
        public ReformRecordView()
        {
            Attachs = new List<Epm_TzAttachs>();

        }
        public Epm_ReformRecord ReformRecord { get; set; }

        public List<Epm_TzAttachs> Attachs { get; set; }

        public string StationIds { get; set; }

        public string CompanyIds { get; set; }
    }
}
