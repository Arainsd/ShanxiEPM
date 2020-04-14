using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.TimerManage
{
    public class TaskView
    {
        /// <summary>
        /// id编号，勿重复
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyPath { get; set; }
        /// <summary>
        /// 类的完全限定名（从命名空间到类名）
        /// </summary>
        public string ClassPath { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 间隔时间
        /// </summary>
        public string IntervalTime { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string ExcuteTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
    }
}
