using hc.epm.ViewModel;
using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 整改单位
    /// </summary>
    public class Companies
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 整改人
        /// </summary>
        public List<Companies> personnelList { get; set; }

    }

}

