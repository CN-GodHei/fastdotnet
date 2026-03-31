
namespace Fastdotnet.Core.Utils;

/// <summary>
/// 雪花ID生成器
/// </summary>
public static class SnowflakeIdGenerator
{
    // 机器ID - 在分布式环境中应确保每个节点的ID唯一
    private static readonly long _machineId = 1;
    
    // 机器ID位数
    private static readonly long _machineIdBits = 5L;
    
    // 序列号位数
    private static readonly long _sequenceBits = 12L;
    
    // 最大序列号
    private static readonly long _maxSequence = -1L ^ (-1L << (int)_sequenceBits);
    
    // 机器ID左移位数
    private static readonly long _machineIdShift = _sequenceBits;
    
    // 时间戳左移位数
    private static readonly long _timestampLeftShift = _sequenceBits + _machineIdBits;
    
    // 序列号
    private static long _sequence = 0L;
    
    // 上次时间戳
    private static long _lastTimestamp = -1L;
    
    // 起始时间戳 (2020-01-01)
    private static readonly long _twepoch = 1577836800000L;
    public static long NextId()
    {
        return YitIdHelper.NextId();
    }
    /// <summary>
    /// 生成下一个ID
    /// </summary>
    /// <returns></returns>
    public static string NextStrId()
    {
        return YitIdHelper.NextId().ToString();
        //lock (typeof(SnowflakeIdGenerator))
        //{
        //    var timestamp = GetTimestamp();

        //    // 如果当前时间小于上次时间戳，说明系统时钟回退过，抛出异常
        //    if (timestamp < _lastTimestamp)
        //    {
        //        throw new Exception("时钟向后移动，无法生成ID");
        //    }

        //    // 如果是同一毫秒内生成的，则进行序列号递增
        //    if (_lastTimestamp == timestamp)
        //    {
        //        _sequence = (_sequence + 1) & _maxSequence;
        //        // 如果序列号超过最大值，则等待下一毫秒
        //        if (_sequence == 0)
        //        {
        //            timestamp = WaitNextMillis(_lastTimestamp);
        //        }
        //    }
        //    else
        //    {
        //        // 如果是下一毫秒，则重置序列号
        //        _sequence = 0L;
        //    }

        //    _lastTimestamp = timestamp;

        //    // 组合ID
        //    var id = ((timestamp - _twepoch) << (int)_timestampLeftShift) |
        //           (_machineId << (int)_machineIdShift) |
        //           _sequence;
        //    // 转换为字符串返回
        //    return id.ToString();
        //}
    }
    
    /// <summary>
    /// 等待下一毫秒
    /// </summary>
    /// <param name="lastTimestamp"></param>
    /// <returns></returns>
    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = GetTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetTimestamp();
        }
        return timestamp;
    }
    
    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    private static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}