using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MaterialView
    {
        public Epm_Material Epm_Material { get; set; }
        public MaterialView()
        {
            MaterialDetails = new List<Epm_MaterialDetails>();
            FileList = new List<Base_Files>();

        }

        ///<summary>
        ///验收设备名称用，分割
        ///</summary>
        public string Names { get; set; }

        /// <summary>
        /// 明细数据
        /// </summary>
        public List<Epm_MaterialDetails> MaterialDetails { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Base_Files> FileList { get; set; }
    }
}
