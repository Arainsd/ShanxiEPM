using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TemplateView
    {
        public TemplateView()
        {
            Template = new Epm_Template();
            TemplateDetails = new List<Epm_TemplateDetails>();
        }
        public Epm_Template Template { get; set; }

        public List<Epm_TemplateDetails> TemplateDetails { get; set; }
    }
}
