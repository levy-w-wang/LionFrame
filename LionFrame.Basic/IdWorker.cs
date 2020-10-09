using System;
using System.Diagnostics;

namespace LionFrame.Basic
{
    public class IdWorker
    {
        //机器ID
        private static long workerId;
        private static long twepoch = 1602216993000; //唯一时间，这是一个避免重复的随机量，自行设定不要大于当前时间戳
        private static long sequence = 0L;
        private static int workerIdBits = 10; //机器码字节数。10个字节用来保存机器码(定义为Long类型会出现，最大偏移64位，所以左移64位没有意义) maxWorkerId = 2的workerIdBits次方 
        public static long maxWorkerId = -1L ^ -1L << workerIdBits; //最大机器ID
        private static int sequenceBits = 10; //计数器字节数，10个字节用来保存计数码
        private static int workerIdShift = sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
        private static int timestampLeftShift = sequenceBits + workerIdBits; //时间戳左移动位数就是机器码和计数器总字节数
        public static long sequenceMask = -1L ^ -1L << sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private long lastTimestamp = -1L;

        /// <summary>
        /// 机器码 workerId不同处不要使用一样的  避免重复
        /// </summary>
        /// <param name="workerId"></param>
        public IdWorker(long workerId)
        {
            if (workerId > maxWorkerId || workerId < 0)
                throw new Exception($"worker Id can't be greater than {maxWorkerId} or less than 0 ");
            IdWorker.workerId = workerId;
        }

        public long NextId()
        {
            lock (this)
            {
                long timestamp = TimeGen();
                if (this.lastTimestamp > timestamp)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    while (timestamp < lastTimestamp)
                    { //如果当前时间戳比上一次生成ID时时间戳还小，抛出异常，因为不能保证现在生成的ID之前没有生成过
                        if (stopwatch.ElapsedMilliseconds > 1000 * 3)
                        {
                            LogHelper.Logger.Fatal($"Clock moved backwards. Refusing to generate id for {this.lastTimestamp - timestamp} milliseconds");
                            throw new Exception($"Clock moved backwards. Refusing to generate id for {this.lastTimestamp - timestamp} milliseconds");
                        }
                        timestamp = TimeGen();
                    }
                    stopwatch.Stop();
                }
                else if (this.lastTimestamp == timestamp)
                { //同一微妙中生成ID
                    IdWorker.sequence = (IdWorker.sequence + 1) & IdWorker.sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (IdWorker.sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙
                        timestamp = TillNextMillis(this.lastTimestamp);
                    }
                }
                else
                { //不同微秒生成ID
                    IdWorker.sequence = 0; //计数清0
                }

                this.lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long nextId = (timestamp - twepoch << timestampLeftShift) | IdWorker.workerId << IdWorker.workerIdShift | IdWorker.sequence;
                return nextId;
            }
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long TillNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (timestamp <= lastTimestamp)
            {
                var mis = sw.ElapsedMilliseconds;
                if (mis > 1000 * 5)//如果五秒钟都没有得到比上次执行时间大的时间挫，就报异常
                {
                    throw new Exception($"时间可能出现了回拨，请检查服务器时间。上次时间戳:{lastTimestamp},本次时间戳:{timestamp}");
                }
                timestamp = TimeGen();
            }
            sw.Stop();
            return timestamp;
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        private long TimeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

    }
}
