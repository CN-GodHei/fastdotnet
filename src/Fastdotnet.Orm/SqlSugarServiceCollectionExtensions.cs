using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using Fastdotnet.Core.Models.LogModels;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Fastdotnet.Orm;

public static class SqlSugarServiceCollectionExtensions
{
    /// <summary>
    /// 添加SqlSugar服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. 注册 SqlSugarScope 服务
        services.AddSingleton(serviceProvider =>
        {
            var options = configuration.GetSection("SqlSugar").Get<SqlSugarOptions>();

            if (options?.Connections == null || !options.Connections.Any())
            {
                throw new ArgumentNullException(nameof(options), "数据库连接配置缺失或为空。");
            }

            // 创建SqlSugarScope
            var scope = new SqlSugarScope(options.Connections, db =>
            {
                // AOP配置
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    try
                    {
                        // 使用日志框架记录SQL
                        var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                        //logger?.LogInformation("SqlSugar Executing SQL: {Sql}", sql);
                        
                        // 根据配置决定是否将SQL执行日志记录到专门的日志表中
                        if (options.EnableSqlExecutionLogging)
                        {
                            // 在SQL执行后记录日志
                            var sqlCopy = sql;
                            var parsCopy = pars?.ToArray() ?? new SugarParameter[0];
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                            db.Aop.OnLogExecuted = (sqlExecuted, parsExecuted) =>
                            {
                                stopwatch.Stop();
                                RecordSqlExecutionToLogTable(serviceProvider, sqlCopy, parsCopy, stopwatch.ElapsedMilliseconds.ToString(), null);
                                db.Aop.OnLogExecuted = null; // 重置事件处理程序
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录AOP处理过程中的任何错误，避免影响主流程
                        var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                        logger?.LogError(ex, "Error in SqlSugar OnLogExecuting AOP handler");
                    }
                };

                db.Aop.OnError = ex =>
                {
                    try
                    {
                        // 使用日志框架记录错误
                        var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                        logger?.LogError(ex, "SqlSugar SQL execution error: {Message}", ex.Message);
                        
                        // 根据配置决定是否将SQL错误记录到专门的日志表中
                        if (options.EnableSqlExecutionLogging)
                        {
                            // 安全地处理异常中的参数
                            SugarParameter[] parameters = null;
                            if (ex.Parametres is IEnumerable<SugarParameter> sugarParams)
                            {
                                parameters = sugarParams.ToArray();
                            }
                            else if (ex.Parametres is IEnumerable enumerable)
                            {
                                var paramList = new List<SugarParameter>();
                                foreach (var param in enumerable)
                                {
                                    if (param is SugarParameter sugarParam)
                                    {
                                        paramList.Add(sugarParam);
                                    }
                                }
                                parameters = paramList.ToArray();
                            }
                            else
                            {
                                parameters = new SugarParameter[0];
                            }
                            
                            RecordSqlExecutionToLogTable(serviceProvider, ex.Sql, parameters, "0", ex);
                        }
                    }
                    catch (Exception ex2)
                    {
                        // 记录AOP处理过程中的任何错误，避免影响主流程
                        var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                        logger?.LogError(ex2, "Error in SqlSugar OnError AOP handler");
                    }
                };

                // 配置全局软删除过滤器
                db.QueryFilter.AddTableFilter<ISoftDelete>(u => u.IsDeleted == false);
                //db.QueryFilter.Add<ISoftDelete>(u => u.IsDeleted == false);
                
                // 插入前自动填充雪花ID和创建时间
                db.Aop.DataExecuting = (oldValue, entityInfo) =>
                {
                    if (entityInfo.OperationType == DataFilterType.InsertByObject)
                    {
                        // 自动填充雪花ID（如果ID为0）
                        if (entityInfo.PropertyName == nameof(IBaseEntity.Id) && oldValue == null)
                        {
                            entityInfo.SetValue(SnowflakeIdGenerator.NextStrId());
                            entityInfo.SetValue(YitIdHelper.NextId());
                        }
                        
                        // 自动填充创建时间（如果CreateTime为默认值）
                        else if (entityInfo.PropertyName == nameof(IBaseEntity.CreateTime) && (DateTime)oldValue == default)
                        {
                            // 使用本地时间，确保所见即所得
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                    else if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                    {
                        // 自动填充更新时间
                        if (entityInfo.PropertyName == nameof(IBaseEntity.UpdateTime))
                        {
                            // 使用本地时间，确保所见即所得
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                };
            });

            return scope;
        });

        // 2. 注册 ISqlSugarClient，使其解析为已注册的 SqlSugarScope
        services.AddSingleton<ISqlSugarClient>(sp => sp.GetRequiredService<SqlSugarScope>());

        return services;
    }

    /// <summary>
    /// 记录SQL执行日志到专门的日志表中
    /// </summary>
    /// <param name="rootServiceProvider">根服务提供者</param>
    /// <param name="sql">执行的SQL语句</param>
    /// <param name="parameters">SQL参数</param>
    /// <param name="elapsedMilliseconds">执行耗时（毫秒）</param>
    /// <param name="exception">异常信息（如果有）</param>
    private static void RecordSqlExecutionToLogTable(IServiceProvider rootServiceProvider, string sql, SugarParameter[] parameters, string elapsedMilliseconds, Exception exception)
    {
        try
        {
            // 在新的作用域中解析服务，避免生命周期问题
            using var scope = rootServiceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            // 构造完整的SQL语句（包含参数值）
            var formattedSql = sql;
            
            // 如果有参数，则尝试构建完整的SQL语句
            if (parameters != null && parameters.Length > 0)
            {
                formattedSql = FormatSqlWithParameters(sql, parameters);
            }

            // 创建SQL执行日志对象
            var sqlExecutionLog = new SqlExecutionLog
            {
                RequestId = RequestIdManager.CurrentRequestId,
                FullSql = formattedSql,
                ElapsedMilliseconds = elapsedMilliseconds,
                HasError = exception != null,
                ErrorMessage = exception?.Message,
                StackTrace = exception?.StackTrace,
                CreateTime = DateTime.Now
            };

            // 同步记录日志到数据库，避免异步任务可能的问题
            try
            {
                // 获取SqlSugar客户端
                var sqlClient = serviceProvider.GetService<ISqlSugarClient>();
                //if (sqlClient == null) 
                //{
                //    var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                //    logger?.LogWarning("无法获取SqlSugarClient实例");
                //    return;
                //}

                // 尝试将日志记录到日志数据库
                var logDb = sqlClient.AsTenant().GetConnection("log") ?? sqlClient;
                
                // 添加调试日志
                var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                //logger?.LogInformation("准备记录SQL执行日志到数据库: {Sql}", formattedSql);
                
                var result = logDb.Insertable(sqlExecutionLog).ExecuteCommand();
                //logger?.LogInformation("SQL执行日志记录结果: {Result}", result);
            }
            catch (Exception ex)
            {
                // 记录日志记录过程中的任何错误，避免影响主流程
                var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                //logger?.LogError(ex, "Error recording SQL execution log to database. SQL: {Sql}", formattedSql);
            }
        }
        catch (Exception ex)
        {
            // 记录日志记录过程中的任何错误，避免影响主流程
            var logger = rootServiceProvider.GetService<ILogger<SqlSugarClient>>();
            //logger?.LogError(ex, "Error in RecordSqlExecutionToLogTable method");
        }
    }

    /// <summary>
    /// 格式化SQL语句，将参数值嵌入到SQL中
    /// </summary>
    /// <param name="sql">原始SQL语句</param>
    /// <param name="parameters">SQL参数</param>
    /// <returns>包含参数值的完整SQL语句</returns>
    private static string FormatSqlWithParameters(string sql, SugarParameter[] parameters)
    {
        if (string.IsNullOrEmpty(sql) || parameters == null || parameters.Length == 0)
        {
            return sql ?? string.Empty;
        }

        var formattedSql = sql;
        
        // 按参数名长度降序排列，避免短名称替换影响长名称（例如@p1和@p10的情况）
        var sortedParameters = parameters.OrderByDescending(p => p.ParameterName?.Length ?? 0);
        
        foreach (var parameter in sortedParameters)
        {
            if (string.IsNullOrEmpty(parameter.ParameterName))
                continue;

            var parameterValue = GetParameterValue(parameter);
            formattedSql = formattedSql.Replace(parameter.ParameterName, parameterValue);
        }

        return formattedSql;
    }

    /// <summary>
    /// 获取参数的字符串表示形式
    /// </summary>
    /// <param name="parameter">SQL参数</param>
    /// <returns>参数值的字符串表示</returns>
    private static string GetParameterValue(SugarParameter parameter)
    {
        if (parameter.Value == null || parameter.Value == DBNull.Value)
        {
            return "NULL";
        }

        // 根据参数值的类型进行适当的格式化
        switch (parameter.Value)
        {
            case string str:
                return $"'{str.Replace("'", "''")}'"; // 转义单引号
            case DateTime dateTime:
                return $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            case bool boolValue:
                return boolValue ? "1" : "0"; // SQL中通常用1/0表示布尔值
            case int _:
            case long _:
            case short _:
            case byte _:
            case decimal _:
            case float _:
            case double _:
                return parameter.Value.ToString();
            default:
                return $"'{parameter.Value}'";
        }
    }

    /// <summary>
    /// 为插件执行Code First，扫描插件目录下的所有程序集
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="pluginAssembly">插件的主程序集，用于定位插件目录</param>
    public static void UsePluginCodeFirst(this IServiceProvider serviceProvider, Assembly pluginAssembly)
    {
        if (pluginAssembly == null)
        {
            return;
        }

        var sqlClient = serviceProvider.GetRequiredService<ISqlSugarClient>();

        // 直接从传入的插件程序集中找出所有符合条件的实体
        var entityTypes = pluginAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
            .ToArray();
        
        if (entityTypes.Any())
        {
            sqlClient.CodeFirst.InitTables(entityTypes);
        }
    }

    /// <summary>
    /// 从SqlSugar缓存中移除插件的实体类型
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="pluginAssembly">要卸载的插件程序集</param>
    public static void RemovePluginCodeFirst(this IServiceProvider serviceProvider, Assembly pluginAssembly)
    {
        if (pluginAssembly == null)
        {
            return;
        }

        var sqlClient = serviceProvider.GetRequiredService<ISqlSugarClient>();

        // 找出该程序集中所有被SqlSugar管理的实体
        var entityTypesInAssembly = pluginAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
            .ToHashSet();

        if (entityTypesInAssembly.Any())
        {
            try
            {
                // 通过反射获取私有字段 _entityInfoList
                var entityMaintenance = sqlClient.EntityMaintenance;
                var fieldInfo = typeof(EntityMaintenance).GetField("EntityList", BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (fieldInfo != null)
                {
                    // 获取当前缓存的实体列表
                    var entityInfoList = fieldInfo.GetValue(entityMaintenance) as IList;
                    if (entityInfoList != null)
                    {
                        // 创建一个列表来存储要移除的项，避免在遍历时修改集合
                        var itemsToRemove = new List<object>();
                        foreach (var item in entityInfoList)
                        {
                            var entityType = item.GetType().GetProperty("EntityType")?.GetValue(item) as Type;
                            if (entityType != null && entityTypesInAssembly.Contains(entityType))
                            {
                                itemsToRemove.Add(item);
                            }
                        }

                        // 从列表中移除
                        foreach (var item in itemsToRemove)
                        {
                            entityInfoList.Remove(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录反射操作可能出现的错误
                var logger = serviceProvider.GetService<ILogger<SqlSugarClient>>();
                logger?.LogError(ex, "Failed to remove plugin entities from SqlSugar cache via reflection.");
            }
        }
    }
}