using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MilepostView
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        ///<summary>
        /// 资料编码
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        /// 资料名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// 状态
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        /// 排序
        ///</summary>
        public int? Sort { get; set; }

        ///// <summary>
        ///// 复选框的值 Id
        ///// </summary>
        //public string checkboxValue { get; set; }

        ///// <summary>
        ///// 父级Id
        ///// </summary>
        //public string parentId { get; set; }

        ////public List<MilepostView> children { get; set; }
        /// <summary>
        /// 项目性质
        /// </summary>
        public string ParentName { get; set; }
    }
}
