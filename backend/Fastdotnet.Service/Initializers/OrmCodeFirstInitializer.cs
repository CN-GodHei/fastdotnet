
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
    public int Order => -1000;

    public async Task InitializeAsync()
    {
        // 获取主业务数据库连接
        var mainDb = _sqlSugarClient.AsTenant().GetConnection("main");

        // 获取日志数据库连接
        var logDb = _sqlSugarClient.AsTenant().GetConnection("log");

        // 扫描当前应用域中的所有程序集
        var allEntityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<SugarTable>() != null)
            .ToArray();

        // 分离日志实体和业务实体
        var logEntityTypes = allEntityTypes
            .Where(t => t.Namespace != null && t.Namespace.EndsWith("LogModels"))
            .ToArray();

        var mainEntityTypes = allEntityTypes
            .Where(t => !logEntityTypes.Contains(t))
            .ToArray();

        // 为主业务库初始化表
        if (mainEntityTypes.Any())
        {

#if DEBUG
            try
            {
                foreach (var item in mainEntityTypes)
                {
                    Console.WriteLine($"正在初始化主业务库表：{item.FullName}");
                    mainDb.CodeFirst.SplitTables().InitTables(item);
                }
            }
            catch (Exception e)
            {

                throw;
            }
#else
                    mainDb.CodeFirst.SplitTables().InitTables(mainEntityTypes);

#endif
        }

        // 为日志库初始化表
        if (logEntityTypes.Any())
        {
            logDb.CodeFirst.SplitTables().InitTables(logEntityTypes);
        }

        await Task.CompletedTask;
    }
}