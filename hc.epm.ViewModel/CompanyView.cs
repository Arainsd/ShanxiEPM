using hc.epm.DataModel.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class CompanyView
    {
        public CompanyView()
        {
            baseFiles = new List<Base_Files>();
        }
        [NotMapped]
        public List<Base_Files> baseFiles { get; set; }
        public long Id { get; set; }

        /// <summary>
        /// 上级单位Id
        /// </summary>

        public long PId { get; set; }
        ///<summary>
        ///上级单位编码
        ///</summary>
        public string PreCode { get; set; }

        ///<summary>
        ///上级单位名称
        ///</summary>
        public string PreName { get; set; }

        ///<summary>
        ///单位编码
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        ///单位名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///简称
        ///</summary>
        public string ShortName { get; set; }

        ///<summary>
        ///单位类型
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        ///单位电话
        ///</summary>
        public string Phone { get; set; }

        ///<summary>
        ///电子邮箱
        ///</summary>
        public string Email { get; set; }

        public long? LinkManId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 负责人电话
        /// </summary>
        public string LinkPhone { get; set; }
        ///<summary>
        ///所在省市
        ///</summary>
        public string Address { get; set; }

        ///<summary>
        ///详细地址
        ///</summary>
        public string AddressInfo { get; set; }

        ///<summary>
        ///简介
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///传真电话
        ///</summary>
        public string FaxPhone { get; set; }

        /// <summary>
        /// 公司所在地
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 公司 Logo 地址(不包含域名部分)
        /// </summary>
        public string Logo { get; set; }

        public string OrgType { get; set; }

        public string AddressName { get; set; }

        public bool? IsBlacklist { get; set; }//是否黑名单

        /// <summary>
        /// 级别
        /// </summary>
        public string CompanyRank { get; set; }

        /// <summary>
        /// 级别名称
        /// </summary>
        public string CompanyRankName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string CompanyType { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string CompanyTypeName { get; set; }
    }
}
