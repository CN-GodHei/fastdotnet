using Fastdotnet.Core.Models.Base;
using Fastdotnet.Core.Models.Interfaces;
using Fastdotnet.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
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
                var logger = serviceProvider.GetService<ILogger<ISqlSugarClient>>();
                logger?.LogError(ex, "Failed to remove plugin entities from SqlSugar cache via reflection.");
            }
        }
    }
}