using Fastdotnet.Core.Models.Logs;
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
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddExceptionLogAsync(ExceptionLog log)
    {
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }

    public async Task AddDebugLogAsync(DebugLog log)
    {
        await GetLogDb().Insertable(log).ExecuteCommandAsync();
    }
}
