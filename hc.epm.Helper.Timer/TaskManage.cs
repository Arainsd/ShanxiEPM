using FluentScheduler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Helper.Timer
{
    public class TaskManage
    {
        /// <summary>
        /// app path
        /// </summary>
        private static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="startJob"></param>
        /// <param name="endJob"></param>
        /// <param name="exJob"></param>
        public static void Start(Action<JobStartInfo> startJob, Action<JobEndInfo> endJob, Action<JobExceptionInfo> exJob)
        {
            //先停止所有JobManager
            JobManager.Stop();

            InitTask(startJob, endJob, exJob);
        }

        private static void InitTask(Action<JobStartInfo> startJob, Action<JobEndInfo> endJob, Action<JobExceptionInfo> exJob)
        {
            Dictionary<TaskConfig, Action> list = GetTask();
            //注册定时任务并执行
            Registry registry = new Registry();
            foreach (var item in list)
            {
                //根据不同任务类型确定不同的定时执行策略
                int week = (Int32)DateTime.Now.DayOfWeek;
                TaskType type = (TaskType)item.Key.TaskType;
                //执行时间
                int hour = 0;
                int min = 0;
                if (!string.IsNullOrEmpty(item.Key.ExcuteTime))
                {
                    var array = item.Key.ExcuteTime.Split(':');
                    hour = Convert.ToInt32(array[0]);
                    min = Convert.ToInt32(array[1]);
                }
                switch (type)
                {
                    case TaskType.OnceTime://指定时间运行一次
                        var time = DateTime.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunOnceAt(time);
                        break;
                    case TaskType.IntervalSeconds://间隔多少秒重复运行
                        var seconds = int.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(seconds).Seconds();
                        break;
                    case TaskType.IntervalDay://间隔多少天重复运行
                        var days = int.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(days).Days().At(hour, min);
                        break;
                    case TaskType.IntervalMonthOfDay://每月几号运行    
                        var monthOfDays = int.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(1).Months().On(monthOfDays).At(hour, min);
                        break;
                    case TaskType.IntervalWeekOfDay://每周几运行
                        DayOfWeek weekOfDays = (DayOfWeek)int.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(1).Weeks().On(weekOfDays).At(hour, min);
                        //也可以使用.ToRunEvery()
                        break;
                    case TaskType.IntervalMonthLastDay://每月最后一天运行
                        int lastDay = int.Parse(item.Key.IntervalTime);
                        registry.Schedule(() => item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(1).Months().OnTheLastDay().At(hour, min);
                        break;
                    default:
                        break;
                }
                //立即执行一个在每月的星期一 3:00 的计划任务
                //Schedule<MyComplexJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);
                //           registry.Schedule(() =>
                //item.Value()).WithName(item.Key.MethodName).ToRunNow().AndEvery(1).Seconds();
            }
            JobManager.JobStart += startJob;
            JobManager.JobEnd += endJob;
            JobManager.JobException += exJob;
            //JobManager.JobStart += (startInfo) =>
            //{
            //    Console.WriteLine(startInfo.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + " Action Start:" + startInfo.Name);
            //};
            //JobManager.JobEnd += (endInfo) =>
            //{
            //    Console.WriteLine(endInfo.StartTime.ToString("yyyy-MM-dd HH:mm:ss") + " Action End:" + endInfo.Name + ";运行持续时间:" + endInfo.Duration.TotalMilliseconds + "毫秒；下次运行时间:" + endInfo.NextRun);
            //};
            //JobManager.JobException += (ex) =>
            //{
            //    Console.WriteLine("异常Action:" + ex.Name + ";消息:" + ex.Exception.Message);
            //};
            JobManager.Initialize(registry);

            //JobManager.RemoveJob("WriteTimeString");
            //Console.ReadLine();
        }

        /// <summary>
        /// 移除定时任务
        /// </summary>
        /// <param name="methodName">任务名称</param>
        public static void Remove(string methodName)
        {
            //移除所有
            if (string.IsNullOrEmpty(methodName))
            {
                var list = GetAllTask();
                foreach (var item in list)
                {
                    JobManager.RemoveJob(item.Name);
                }
            }
            else
            {
                JobManager.RemoveJob(methodName);
            }
        }
        /// <summary>
        /// 停止定时任务
        /// </summary>
        public static void Stop()
        {
            JobManager.Stop();

        }
        /// <summary>
        /// 开始定时任务
        /// </summary>
        public static void Start()
        {
           
            JobManager.Start();
            //var allTask = JobManager.AllSchedules;
            //var rs = JobManager.RunningSchedules;
        }
        /// <summary>
        /// 获取所有定时任务
        /// </summary>
        /// <returns></returns>
        public static List<Schedule> GetAllTask()
        {
            var allTask = JobManager.AllSchedules;
            return allTask.ToList();

        }
        /// <summary>
        /// 获取所有正在运行的定时任务
        /// </summary>
        public static List<Schedule> GetAllRunningTask()
        {
            var allTask = JobManager.RunningSchedules;
            return allTask.ToList();

        }
        /// <summary>
        /// 获取定时任务
        /// </summary>
        /// <returns></returns>
        private static Dictionary<TaskConfig, Action> GetTask()
        {
            //加载任务配置
            var config = LoadConfig();
            Dictionary<TaskConfig, Action> list = new Dictionary<TaskConfig, Action>();
            //读取定时任务
            foreach (var item in config)
            {
                //只取得以timer开头的方法
                var methods = GetClassMethod(item.AssemblyPath, item.ClassPath).Where(i => i.Key.Name == item.MethodName);
                if (methods.Count() > 0)
                {
                    foreach (var m in methods)
                    {
                        var t = m.Value;
                        Action go = () =>
                        {
                            InvokeMethod(m.Key, t);
                        };
                        list.Add(item, go);
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public static List<TaskConfig> LoadConfig()
        {
            string jsonPath = path + "/TaskConfig.json";
            string result = File.ReadAllText(jsonPath);
            var list = JsonConvert.DeserializeObject<List<TaskConfig>>(result);

            return list;
        }

        /// <summary>  
        /// 获取类的属性、方法  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="className">类名</param>  
        private static Dictionary<MethodInfo, Type> GetClassMethod(string assemblyName, string className)
        {

            Dictionary<MethodInfo, Type> result = new Dictionary<MethodInfo, Type>();
            if (!String.IsNullOrEmpty(assemblyName) && !String.IsNullOrEmpty(className))
            {
                assemblyName = path + assemblyName;
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type type = assembly.GetType(className, true, true);
                if (type != null)
                {
                    //类的属性  
                    //List<string> propertieList = new List<string>();
                    //PropertyInfo[] propertyinfo = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    //foreach (PropertyInfo p in propertyinfo)
                    //{
                    //    propertieList.Add(p.ToString());
                    //}

                    //类的方法  
                    List<string> methods = new List<string>();
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    foreach (MethodInfo mi in methodInfos)
                    {
                        methods.Add(mi.Name);
                        result.Add(mi, type);
                        //方法的参数  
                        //foreach (ParameterInfo p in mi.GetParameters())  
                        //{  

                        //}  
                        //方法的返回值  
                        //string returnParameter = mi.ReturnParameter.ToString();  
                    }
                    return result;
                }
            }
            return result;

        }
        /// <summary>
        /// 获取某个月
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private static DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

        public static void InvokeMethod(MethodInfo m, Type t)
        {
            Object obj = Activator.CreateInstance(t);
            if (m != null)
            {
                m.Invoke(obj, null);
            }
        }
    }
}
