using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 时间控制view
    /// </summary>
    [Serializable]
    public class TimeSettingView
    {
        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 参数详情
        /// </summary>
        public TimeSettingParamsView Params { get; set; }
    }
    [Serializable]
    public class TimeSettingParamsView
    {
        /// <summary>
        /// 资格预审
        /// </summary>
        public string Prequalification { get; set; }

        /// <summary>
        /// 资格后审
        /// </summary>
        public string Postqualification { get; set; }

        /// <summary>
        /// 邀请招标
        /// </summary>
        public string Invitation { get; set; }
    }
}
