using hc.epm.ViewModel;
using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class Score
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
        /// 层级
        /// </summary>
        public string level { get; set; }
        public string addScore { get; set; }
        public List<Companies> companies { get; set; }


    }

}

