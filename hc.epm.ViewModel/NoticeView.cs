using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class NoticeView
    {
        public string SId { get; set; }
        public long Id { get; set; }
        ///<summary>
        ///标题
        ///</summary>
        public string Title { get; set; }

        ///<summary>
        ///内容
        ///</summary>
        public string Content { get; set; }

        ///<summary>
        ///发布人Id
        ///</summary>
        public long? SendUserId { get; set; }

        ///<summary>
        ///发布人Name
        ///</summary>
        public string SendUserName { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 发布途径0无 1PC 2APP 3PC和APP
        /// </summary>
        public int? WayOfRelease { get; set; }

        ///<summary>
        ///状态 1 启用 0 禁用
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }
        ///<summary>
        ///接收单位Id 逗号分隔
        ///</summary>
        public string CompanyIds { get; set; }

        ///<summary>
        ///接收单位Name
        ///</summary>
        public string CompanyNames { get; set; }
        ///<summary>
        ///接收项目Id
        ///</summary>
        public string ProjectIds { get; set; }

        ///<summary>
        ///接收项目Name
        ///</summary>
        public string ProjectNames { get; set; }
        ///<summary>
        ///接收人Id
        ///</summary>
        public string UserIds { get; set; }

        ///<summary>
        ///接收人Name
        ///</summary>
        public string UserNames { get; set; }
    }
}
