using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class InspectView
    {
        public long? InspectId { get; set; }
        ///<summary>
        /// 项目Id
        ///</summary>
        public long? ProjectId { get; set; }

        ///<summary>
        /// 项目
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        /// 检查单名
        ///</summary>
        public string InspectName { get; set; }

        ///<summary>
        /// 检查地址
        ///</summary>
        public string InspectAddress { get; set; }

        ///<summary>
        /// 检查日期
        ///</summary>
        public DateTime? InspectDate { get; set; }

        ///<summary>
        /// 检查人Id
        ///</summary>
        public long? InspectUserId { get; set; }

        ///<summary>
        /// 检查人
        ///</summary>
        public string InspectUserName { get; set; }
        ///<summary>
        /// 检查配置表Id
        ///</summary>
        public long? CheckId { get; set; }

        ///<summary>
        /// 
        ///</summary>
        public string CheckName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public long? CheckParentId { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string CheckParentName { get; set; }

        ///<summary>
        /// 层级
        ///</summary>
        public int? Level { get; set; }

        ///<summary>
        /// 是否选中
        ///</summary>
        public bool? Choice { get; set; }

        ///<summary>
        /// 备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        /// 整改负责人，如：土建现场负责人
        ///</summary>
        public string RectifRecordPerson { get; set; }

        ///<summary>
        /// 满分
        ///</summary>
        public int? ScoreMax { get; set; }

        ///<summary>
        /// 得分
        ///</summary>
        public decimal? Score { get; set; }

        ///<summary>
        /// 状态 待整改、已整改、整改通过、整改不通过
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        /// 检查项表Id
        ///</summary>
        public long? InspectItemId { get; set; }

        public string InspectItemIdStr { get; set; }

        ///<summary>
        /// 类型 DW：单位；RY：人员
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        /// 得失分单位或人员Id
        ///</summary>
        public long? GainLossUserId { get; set; }

        ///<summary>
        /// 得失分单位或人员
        ///</summary>
        public string GainLossUserName { get; set; }

        ///<summary>
        /// 得失分单位或人员Id
        ///</summary>
        public long? GainLossCompanyId { get; set; }

        ///<summary>
        /// 得失分单位或人员
        ///</summary>
        public string GainLossCompanyName { get; set; }
    }
}
