using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Utils;
using Fastdotnet.Core.IService;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
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
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
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
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
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
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
            }
            
            await GetLogDb().Insertable(log).ExecuteCommandAsync();
        }

        // 添加同步方法以支持某些场景
        public void AddOperationLog(OperationLog log)
        {
            // 自动填充RequestId
            if (string.IsNullOrEmpty(log.RequestId))
            {
                log.RequestId = RequestIdManager.CurrentRequestId;
            }
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
            }
            
            GetLogDb().Insertable(log).ExecuteCommand();
        }

        public void AddExceptionLog(ExceptionLog log)
        {
            // 自动填充RequestId
            if (string.IsNullOrEmpty(log.RequestId))
            {
                log.RequestId = RequestIdManager.CurrentRequestId;
            }
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
            }
            
            GetLogDb().Insertable(log).ExecuteCommand();
        }

        public void AddDebugLog(DebugLog log)
        {
            // 自动填充RequestId
            if (string.IsNullOrEmpty(log.RequestId))
            {
                log.RequestId = RequestIdManager.CurrentRequestId;
            }
            
            // 确保有创建时间
            if (log.CreatedAt == default)
            {
                log.CreatedAt = DateTime.Now;
            }
            
            GetLogDb().Insertable(log).ExecuteCommand();
        }
    }
}