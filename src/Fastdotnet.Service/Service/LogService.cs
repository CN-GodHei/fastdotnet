using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Utils;
using Fastdotnet.Service.IService;
using SqlSugar;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service;

/// <summary>
/// 日志服务实现
/// </summary>
public class LogService : ILogService
{
    private readonly SqlSugarScope _sqlSugarScope;

    public LogService(SqlSugarScope sqlSugarScope)
    {
        _sqlSugarScope = sqlSugarScope;
    }

    private ISqlSugarClient GetLogDb()
    {
        // 显式切换到日志数据库
        return _sqlSugarScope.GetConnection("log");
    }

    public async Task AddOperationLogAsync(OperationLog log)
    {
        // 自动填充RequestId
        if (string.IsNullOrEmpty(log.RequestId))
        {
            log.RequestId = RequestIdManager.CurrentRequestId;
        }
        
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddExceptionLogAsync(ExceptionLog log)
    {
        // 自动填充RequestId
        if (string.IsNullOrEmpty(log.RequestId))
        {
            log.RequestId = RequestIdManager.CurrentRequestId;
        }
        
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddDebugLogAsync(DebugLog log)
    {
        // 自动填充RequestId
        if (string.IsNullOrEmpty(log.RequestId))
        {
            log.RequestId = RequestIdManager.CurrentRequestId;
        }
        
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }
}