using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class MessageView
    {
        public long Id { get; set; }

        ///<summary>
        ///站内消息标题
        ///</summary>
        public string Title { get; set; }
        ///<summary>
        ///站内消息发送时间
        ///</summary>
        public DateTime SendTime { get; set; }

        ///<summary>
        ///站内消息发送者
        ///</summary>
        public string SendUserName { get; set; }

        public string BusinessUrl { get; set; }

        public long BussinessId { get; set; }
        public string BussinesType { get; set; }
    }
}
