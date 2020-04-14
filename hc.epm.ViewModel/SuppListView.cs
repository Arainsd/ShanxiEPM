using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class SuppListView
    {
        ///<summary>
        ///供应商名称
        ///</summary>
        public string SupplierName { get; set; }

        ///<summary>
        ///供应商电话
        ///</summary>
        public string Phone { get; set; }

        public List<ProductList> children { get; set; }
        
    }

    public class ProductList
    {
        ///<summary>
		///物资种类
		///</summary>
		public string SupMatManagement { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }
        /// <summary>
        /// 采购数量
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// 验收数量
        /// </summary>
        public int? AcceptNumber { get; set; }
    }
}
