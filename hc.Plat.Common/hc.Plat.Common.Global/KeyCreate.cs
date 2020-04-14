using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{

    public class KeyCreate
    {
        //利用时间+随机码生成主键
        public static string KeyCreateStr()
        {
            Random R = new Random();
            string strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmssms");
            string strRandomResult = R.Next(1, 1000).ToString();
            return strDateTimeNumber + strRandomResult;
        }

        /// <summary>
        /// 利用时间+随机码生成20位长度的数字
        /// </summary>
        /// <returns></returns>
        public static string SmsKeyStr()
        {
            Random R = new Random();
            string strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            string strRandomResult = R.Next(1, 9).ToString() + R.Next(10, 99).ToString() + R.Next(100, 999).ToString();
            return strDateTimeNumber + strRandomResult;
        }


        /// <summary>
        /// 帐号注册6位数字验证码
        /// </summary>
        /// <returns></returns>
        public static string AccountVerfCode()
        {
            Random R = new Random();
            string strRandomResult = R.Next(1, 9).ToString() + R.Next(10, 99).ToString() + R.Next(100, 999).ToString();
            return strRandomResult;
        }
    }

    /// <summary>
    /// 根据当期时间获取长整型数据
    /// </summary>
    public class HcIdWorker
    {
        //机器标识位数
        private const int WORKER_ID_BITS = 4;
        //机器标识位的最大值
        private const long MAX_WORKER_ID = -1L ^ -1L << WORKER_ID_BITS;
        //毫秒内自增位
        private const int SEQUENCE_BITS = 10;
        //自增位最大值
        private const long SEQUENCE_Mask = -1L ^ -1L << SEQUENCE_BITS;
        private const long twepoch = 1398049504651L;
        //时间毫秒值向左偏移位
        private const int timestampLeftShift = SEQUENCE_BITS + WORKER_ID_BITS;
        //机器标识位向左偏移位
        private const int WORKER_ID_SHIFT = SEQUENCE_BITS;
        private static readonly DateTime START_TIME = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        private static readonly object LOCK = new object();
        private static long sequence = 0L;
        private static long lastTimestamp = -1L;
        private static long workerId = 12L;
       
        /// <summary>
        /// 获取下一个id值
        /// </summary>
        /// <returns></returns>
        public static long NextId()
        {
            lock (LOCK)
            {
                long timestamp = TimeGen();
                //当前时间小于上一次时间，错误
                if (timestamp < HcIdWorker.lastTimestamp)
                {
                    throw new Exception(string.Format("Clock moved backwards. Refusing to generate id for %d milliseconds", lastTimestamp - timestamp));
                }
                //当前毫秒内
                if (HcIdWorker.lastTimestamp == timestamp)
                {
                    //+1 求余
                    sequence = (sequence + 1) & SEQUENCE_Mask;
                    //当前毫秒内计数满了，等待下一秒
                    if (HcIdWorker.sequence == 0)
                    {
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else //不是当前毫秒内
                {
                    HcIdWorker.sequence = 0; //重置当前毫秒计数
                }
                HcIdWorker.lastTimestamp = timestamp;

                //当前毫秒值 | 机器标识值 | 当前毫秒内自增值
                long nextId = ((timestamp - twepoch << timestampLeftShift))
                | (HcIdWorker.workerId << WORKER_ID_SHIFT) | (HcIdWorker.sequence);
                return nextId;
            }
        }
        /// <summary>
        /// 等待下一个毫秒
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long tilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }
        /// <summary>
        /// 获取当前时间的Unix时间戳
        /// </summary>
        /// <returns></returns>
        private static long TimeGen()
        {
            return (DateTime.UtcNow.Ticks - START_TIME.Ticks) / 10000;
        }
    }
}
