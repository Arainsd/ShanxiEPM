using log4net;
using log4net.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/****************************************************************
*   Author: zhhui
*   CLR版本：4.0.30319.42000
*   创建时间：2017-01-17 18:58:06
*   
*   类 名： LogHelper
*   功能说明：
*
*   修改历史：
*
*
*****************************************************************/

//[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"Log4Net.config", Watch = true)]
namespace hc.Plat.Common.Global
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {
        private static readonly ConcurrentDictionary<Type, ILog> _loggers = new ConcurrentDictionary<Type, ILog>();

        static LogHelper() { XmlConfigurator.Configure(); }
        /// <summary>
        /// 重新设置配置文件
        /// </summary>
        /// <param name="path"></param>
        public static void SetConfig(string path)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            XmlConfigurator.Configure(file);
        }
        /// <summary>
        /// 获取记录器
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static ILog GetLogger(Type source)
        {
            if (_loggers.ContainsKey(source))
            {
                return _loggers[source];
            }
            else
            {
                ILog logger = LogManager.GetLogger(source);
                _loggers.TryAdd(source, logger);
                return logger;
            }
        }

        /* Log a message object */

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Debug(object source, string message)
        {
            Debug(source.GetType(), message);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="ps"></param>
        public static void Debug(object source, string message, params object[] ps)
        {
            Debug(source.GetType(), string.Format(message, ps));
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Debug(Type source, string message)
        {
            ILog logger = GetLogger(source);
            if (logger.IsDebugEnabled)
                logger.Debug(message);
        }

        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Info(object source, object message)
        {
            Info(source.GetType(), message);
        }

        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Info(Type source, object message)
        {
            ILog logger = GetLogger(source);
            if (logger.IsInfoEnabled)
                logger.Info(message);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Warn(object source, object message)
        {
            Warn(source.GetType(), message);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Warn(Type source, object message)
        {
            ILog logger = GetLogger(source);
            if (logger.IsWarnEnabled)
                logger.Warn(message);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Error(object source, object message)
        {
            Error(source.GetType(), message);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Error(Type source, object message)
        {
            ILog logger = GetLogger(source);
            if (logger.IsErrorEnabled)
                logger.Error(message);
        }

        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Fatal(object source, object message)
        {
            Fatal(source.GetType(), message);
        }

        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void Fatal(Type source, object message)
        {
            ILog logger = GetLogger(source);
            if (logger.IsFatalEnabled)
                logger.Fatal(message);
        }

        /* Log a message object and exception */

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(object source, object message, Exception exception)
        {
            Debug(source.GetType(), message, exception);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Debug(Type source, object message, Exception exception)
        {
            GetLogger(source).Debug(message, exception);
        }

        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(object source, object message, Exception exception)
        {
            Info(source.GetType(), message, exception);
        }

        /// <summary>
        /// 关键信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(Type source, object message, Exception exception)
        {
            GetLogger(source).Info(message, exception);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(object source, object message, Exception exception)
        {
            Warn(source.GetType(), message, exception);
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(Type source, object message, Exception exception)
        {
            GetLogger(source).Warn(message, exception);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(object source, object message, Exception exception)
        {
            Error(source.GetType(), message, exception);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(Type source, object message, Exception exception)
        {
            GetLogger(source).Error(message, exception);
        }

        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(object source, object message, Exception exception)
        {
            Fatal(source.GetType(), message, exception);
        }

        /// <summary>
        /// 失败信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Fatal(Type source, object message, Exception exception)
        {
            GetLogger(source).Fatal(message, exception);
        }
    }

}