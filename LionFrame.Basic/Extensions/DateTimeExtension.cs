using System;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 时间静态扩展
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 1970时间戳
        /// </summary>
        public static DateTime DateTime1970 = new DateTime(1970, 1, 1).ToLocalTime();

        /// <summary>
        /// 获取从 1970-01-01 到现在的秒数。
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimeStamp()
        {
            return (long)(DateTime.Now.ToLocalTime() - DateTime1970).TotalSeconds;
        }

        /// <summary>
        /// 计算 1970-01-01 到指定 <see cref="DateTime"/> 的秒数。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetUnixTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToLocalTime() - DateTime1970).TotalSeconds;
        }

        /// <summary>
        /// 获取从 1970-01-01 到现在的毫秒数。
        /// </summary>
        /// <returns></returns>
        public static long GetJsTimeStamp()
        {
            return (long)(DateTime.Now.ToLocalTime() - DateTime1970).TotalMilliseconds;
        }

        /// <summary>
        /// 计算 1970-01-01 到指定 <see cref="DateTime"/> 的毫秒数。
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetJsTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToLocalTime() - DateTime1970).TotalMilliseconds;
        }

        /// <summary>
        /// 根据时间戳返回时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="timeKind">默认为本地类型</param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromTs(this long timeStamp)
        {
            var dateTime = (timeStamp > 915120000000) ? DateTime1970.AddMilliseconds(timeStamp).ToLocalTime() : DateTime1970.AddSeconds(timeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 可空类型 根据时间戳返回时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromTs(this long? timeStamp)
        {
            if (timeStamp == null)
            {
                return Convert.ToDateTime("1999-1-1");
            }
            var tempTimeStamp = timeStamp ?? 0;
            var dateTime = (timeStamp > 915120000000) ? DateTime1970.AddMilliseconds(tempTimeStamp).ToLocalTime() : DateTime1970.AddSeconds(tempTimeStamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 获取yyyyMMddHHmmss时间
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string yyyMMddHHmmss(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取yyyyMMddHHmmss时间
        /// yyyy/MM/dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string yyyMMddHHmmss2(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 获取yyyyMMddHHmmss时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string yyyMMddHHmmss3(this DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 获取 yyyMMdd 时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string yyyMMdd(this DateTime time)
        {
            return time.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 获取平均时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>两者平均时间</returns>
        public static DateTime GetAverageTime(this DateTime startTime, DateTime endTime)
        {
            var timeSpan = endTime.Subtract(startTime);
            var milliseconds = timeSpan.TotalMilliseconds / 2;
            var averageTime = startTime.AddMilliseconds(milliseconds);
            return averageTime;
        }

        /// <summary>获取中文间隔时间差</summary>
        /// <param name="time"></param>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public static string TimeSpanChinese(this DateTime time, DateTime nowTime)
        {
            TimeSpan timeSpan = time.Subtract(nowTime);
            int num1 = 1440;
            int num2 = 60;
            if (timeSpan.TotalMinutes >= num1 * 4)
                return $"{nowTime.Year}年{ nowTime.Month}月{ nowTime.Day}日";
            if (timeSpan.TotalMinutes >= num1 * 3 && timeSpan.TotalMinutes < num1 * 4)
                return $"{timeSpan.Days}天前";
            if (timeSpan.TotalMinutes >= num1 * 2 && timeSpan.TotalMinutes < num1 * 3)
                return $"{timeSpan.Days}天前";
            if (timeSpan.TotalMinutes > num1 && timeSpan.TotalMinutes < num1 * 2)
                return $"{timeSpan.Days}天前";
            if (timeSpan.TotalMinutes < num1 && timeSpan.TotalMinutes >= num2)
                return $"{(timeSpan.Hours)}小时前";
            if (timeSpan.TotalMinutes < num2 && timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.Minutes}分钟前";
            return "刚刚";
        }
    }
}
