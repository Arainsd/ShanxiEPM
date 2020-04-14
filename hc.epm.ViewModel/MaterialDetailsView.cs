using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MaterialDetailsView
    {
        public MaterialDetailsView()
        {
            FileList = new List<Base_Files>();
            MaterialDetails = new Epm_MaterialDetails();
        }

        public Epm_MaterialDetails MaterialDetails { get; set; }

        public List<Base_Files> FileList { get; set; }
    }
}
