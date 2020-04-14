using System.Collections.Generic;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 甘特图
    /// </summary>
    public class Gantt
    {
        public Gantt()
        {
            values = new List<GanttItem>();
        }

        /// <summary>
        /// 甘特图每行左侧第一例
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 甘特图美化左侧第二列(第一列描述)
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 甘特图日期范围项
        /// </summary>
        public List<GanttItem> values { get; set; }
    }

    /// <summary>
    /// 甘特图日期范围项
    /// </summary>
    public class GanttItem
    {
        /// <summary>
        /// 结束时间，以毫秒为换算单位(例如：/Date(1320192000000)/，计算方式为时间变量减去时间初始值（1970-1-1）的差值换算为毫秒)
        /// </summary>
        public string to { get; set; }

        /// <summary>
        /// 开始时间，以毫秒为换算单位(例如：/Date(1320192000000)/，计算方式为时间变量减去时间初始值（1970-1-1）的差值换算为毫秒)
        /// </summary>
        public string from { get; set; }

        /// <summary>
        /// 鼠标悬停时显示文本
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 甘特图显示项
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 甘特图自定义样式名
        /// </summary>
        public string customClass { get; set; }

        /// <summary>
        /// 一个直接应用于甘特项的数据对象
        /// </summary>
        public string dataObj { get; set; }
    }
}
