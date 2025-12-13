namespace Fastdotnet.WebApi.Extensions;

public static class NewtonsoftJsonExtensions
{
    /// <summary>
    /// 配置全局 Newtonsoft.Json 序列化设置（用于 ASP.NET Core MVC）
    /// </summary>
    public static IMvcBuilder AddNewtonsoftJsonWithCustomSettings(this IMvcBuilder builder)
    {
        return builder.AddNewtonsoftJson(options =>
        {
            ConfigureNewtonsoftJson(options.SerializerSettings);
        });
    }

    /// <summary>
    /// 可复用的 Newtonsoft.Json 全局配置方法（也可用于其他场景，如手动序列化）
    /// </summary>
    static void ConfigureNewtonsoftJson(JsonSerializerSettings setting)
    {
        setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //不改变字段大小写：还是注释吧，总有人分不清大小写，但程序不会，通吃就行
        setting.ContractResolver = new DefaultContractResolver();
    }
}