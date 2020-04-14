using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class CheckItemView
    {
        public string id { get; set; }

        ///<summary>
        ///父Id
        ///</summary>
        public long? parentId { get; set; }

        ///<summary>
        ///父Name
        ///</summary>
        public string parentName { get; set; }

        ///<summary>
        ///检查项名
        ///</summary>
        public string name { get; set; }

        ///<summary>
        ///层级1,2,3
        ///</summary>
        public int? level { get; set; }

        /// <summary>
        /// 打分范围
        /// </summary>
        public string scoreRange { get; set; }

        //public string roleType { get; set; }

        /// <summary>
        /// 整改负责人范围
        /// </summary>
        public string rectificationManName { get; set; }

        public List<CheckItemView> children { get; set; }

        //public bool selected { get; set; }

    }
}
