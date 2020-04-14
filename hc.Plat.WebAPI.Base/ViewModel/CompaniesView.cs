using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 附件资源实体
    /// </summary>
    public class CompaniesView
    {
        public CompaniesView() { personnelList = new List<CompaniesView>(); }
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }


        /// <summary>
        /// 人员集合
        /// </summary>
        public List<CompaniesView> personnelList { get; set; }
    }
}