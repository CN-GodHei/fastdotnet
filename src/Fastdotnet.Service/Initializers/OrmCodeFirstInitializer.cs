using Fastdotnet.Core.Initializers;
using SqlSugar;
using System;
using System.Linq;
using System.Reflection;

namespace Fastdotnet.Service.Initializers;

/// <summary>
/// ORM Code First 初始化器
/// </summary>
public class OrmCodeFirstInitializer : IApplicationInitializer
{
    private readonly ISqlSugarClient _sqlSugarClient;

    public OrmCodeFirstInitializer(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
    }

    public async Task InitializeAsync()
    {
        // 扫描当前应用域中的所有程序集
        var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
            .ToArray();

        if (entityTypes.Any())
        {
            _sqlSugarClient.CodeFirst.InitTables(entityTypes);
        }

        await Task.CompletedTask;
    }
}
