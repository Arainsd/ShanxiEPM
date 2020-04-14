using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MaterialViewNew
    {
        public Epm_Material Epm_Material { get; set; }
        public MaterialViewNew()
        {
            MaterialDetails = new List<MaterialDetailsView>();

        }

        ///<summary>
        ///验收设备名称用，分割
        ///</summary>
        public string Names { get; set; }

        /// <summary>
        /// 明细数据
        /// </summary>
        public List<MaterialDetailsView> MaterialDetails { get; set; }
    }
}
