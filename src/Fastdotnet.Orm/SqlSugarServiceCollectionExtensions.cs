using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

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
            var options = configuration.GetSection(SqlSugarOptions.SectionName).Get<SqlSugarOptions>();

            if (options?.Connections == null || !options.Connections.Any())
            {
                throw new ArgumentNullException(nameof(options), "Database connection configuration is missing or empty.");
            }

            // 创建SqlSugarScope
            var scope = new SqlSugarScope(options.Connections, db =>
            {
                // 从外部的serviceProvider获取ILogger
                var logger = serviceProvider.GetRequiredService<ILogger<SqlSugarClient>>();

                // AOP配置
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    // 使用日志框架记录SQL
                    logger.LogInformation("SqlSugar Executing SQL: {Sql}", sql);
                };

                db.Aop.OnError = ex =>
                {
                    // 使用日志框架记录错误
                    logger.LogError(ex, "SqlSugar SQL execution error: {Message}", ex.Message);
                };

                // 配置全局软删除过滤器
                db.QueryFilter.AddTableFilter<ISoftDelete>(u => u.IsDeleted == false);
                
                // 插入前自动填充雪花ID和创建时间
                db.Aop.DataExecuting = (oldValue, entityInfo) =>
                {
                    if (entityInfo.OperationType == DataFilterType.InsertByObject)
                    {
                        // 自动填充雪花ID（如果ID为0）
                        if (entityInfo.PropertyName == nameof(IBaseEntity.Id) && (long)oldValue == 0)
                        {
                            entityInfo.SetValue(SnowflakeIdGenerator.NextId());
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
    /// 为插件执行Code First，扫描插件目录下的所有程序集
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="pluginAssembly">插件的主程序集，用于定位插件目录</param>
    public static void UsePluginCodeFirst(this IServiceProvider serviceProvider, Assembly pluginAssembly)
    {
        if (pluginAssembly == null || string.IsNullOrEmpty(pluginAssembly.Location))
        {
            return;
        }

        var sqlClient = serviceProvider.GetRequiredService<ISqlSugarClient>();

        // 直接从传入的插件程序集中找出所有符合条件的实体
        var entityTypes = pluginAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
            .ToArray();

        // 同时也检查当前AppDomain中其他已加载的插件相关程序集
        var pluginName = pluginAssembly.GetName().Name?.Split('.').FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(pluginName))
        {
            var pluginAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && (a.GetName().Name?.StartsWith(pluginName) ?? false));

            foreach (var assembly in pluginAssemblies)
            {
                var additionalEntityTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
                    .ToArray();

                entityTypes = entityTypes.Concat(additionalEntityTypes).ToArray();
            }
        }

        if (entityTypes.Any())
        {
            sqlClient.CodeFirst.InitTables(entityTypes);
        }
    }
}