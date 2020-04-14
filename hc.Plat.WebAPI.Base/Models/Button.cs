namespace hc.Plat.WebAPI.Base.Models
{
    /// <summary>
    /// 操作按钮
    /// </summary>
    public class Button
    {
        /// <summary>
        /// 权限 ID
        /// </summary>
        public string rightId { get; set; }

        /// <summary>
        /// 操作代码
        /// </summary>
        public string rightAction { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 颜色(16进制)
        /// </summary>
        public string color { get; set; }
    }
}