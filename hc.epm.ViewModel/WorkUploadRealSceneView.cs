using hc.epm.DataModel.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class WorkUploadRealSceneView
    {
        public long Id { get; set; }

        public string name { get; set; }

        public string time { get; set; }
        public int Type { get; set; }
        public List<Base_Files> Attachs { get; set; }
    }
}
